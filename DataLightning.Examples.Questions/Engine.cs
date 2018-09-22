using DataLightning.Core;
using DataLightning.Core.Operators;
using DataLightning.Examples.Questions.Model;
using Orleans;
using Orleans.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataLightning.Examples.Questions
{
    public class Engine
    {
        private IClusterClient _client;
        private int _maxQuestionId = 0;
        private int _maxAnswerId = 0;
        private int _maxUserId = 0;

        public Engine(IQaApiPublisher qaApiPublisher)
        {
            StartOrleansClient();

            var questions = _client.GetGrain<IPassThroughUnit<Question>>("questions");
            var answers = _client.GetGrain<IPassThroughUnit<Answer>>("answers");
            var users = _client.GetGrain<IPassThroughUnit<User>>("users");

            questions.Subscribe(new CallbackSubcriber<Question>(q => _maxQuestionId = Math.Max(_maxQuestionId, q.Id)));
            answers.Subscribe(new CallbackSubcriber<Answer>(a => _maxAnswerId = Math.Max(_maxAnswerId, a.Id)));
            users.Subscribe(new CallbackSubcriber<User>(u => _maxUserId = Math.Max(_maxUserId, u.Id)));

            var qaJoin = new Join<Question, Answer>(questions, answers, q => q.Id, a => a.QuestionId);
            new QaApiContentMaker(qaJoin).Subscribe(new CallbackSubcriber<QaApiContent>(qaApiPublisher.Publish));

            var userStatistics = new MultiJoin(
                new JoinDefinitionAdapter<User>(users, "User", u => u.Id),
                new JoinDefinitionAdapter<Question>(questions, "Questions", q => q.UserId),
                new JoinDefinitionAdapter<Answer>(answers, "Answers", a => a.UserId));

            var statMapper = new Mapper<IDictionary<string, IList<object>>, UserStatistic>(userStatistics, data =>
            {
                if (!data.ContainsKey("User") || data["User"].Count != 1)
                    return null;

                return new UserStatistic
                {
                    User = (User)data["User"].Single(),
                    QuestionsCount = data.ContainsKey("Questions") ? data["Questions"].Count : 0,
                    AnswerCount = data.ContainsKey("Answers") ? data["Answers"].Count : 0
                };
            });

            statMapper.Subscribe(new CallbackSubcriber<UserStatistic>(s => s?.ToString()));
        }

        private void StartOrleansClient()
        {
            var clientBuilder = new ClientBuilder()
              .UseLocalhostClustering()
              .Configure<ClusterOptions>(options =>
              {
                  options.ClusterId = "dev";
                  options.ServiceId = "Orleans2GettingStarted";
              });

            _client = clientBuilder.Build();

            _client.Connect().GetAwaiter().GetResult();

            //var random = new Random();
            //string sky = "blue";

            //while (sky == "blue") // if run in Ireland, it exits loop immediately
            //{
            //    int grainId = random.Next(0, 500);
            //    double temperature = random.NextDouble() * 40;
            //    var sensor = _client.GetGrain<ITemperatureSensorGrain>(grainId);
            //    await sensor.SubmitTemperatureAsync((float)temperature);
            //}
        }

        public int AddUser(string userName)
        {
            User user = new User
            {
                Id = _maxUserId + 1,
                UserName = userName
            };

            var users = _client.GetGrain<IPassThroughUnit<User>>("users");
            users.Push(user);

            return user.Id;
        }

        public int AddQuestion(int userId, string text)
        {
            Question question = new Question
            {
                Id = _maxQuestionId + 1,
                UserId = userId,
                Text = text
            };

            var questions = _client.GetGrain<IPassThroughUnit<Question>>("questions");
            questions.Push(question);
            return question.Id;
        }

        public int AddAnswer(int questionId, int userId, string text)
        {
            Answer answer = new Answer
            {
                Id = _maxAnswerId + 1,
                UserId = userId,
                QuestionId = questionId,
                Text = text
            };

            var answers = _client.GetGrain<IPassThroughUnit<Answer>>("answers");
            answers.Push(answer);
            return answer.Id;
        }
    }
}