using DataLightning.Core;
using DataLightning.Core.Operators;
using DataLightning.Examples.Questions.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataLightning.Examples.Questions
{
    public class Engine
    {
        private readonly PassThroughUnit<Question> _questions;
        private readonly PassThroughUnit<Answer> _answers;
        private readonly PassThroughUnit<User> _users;

        private readonly Dictionary<int, long> _userVersionGuard = new Dictionary<int, long>();
        private readonly Dictionary<int, long> _questionVersionGuard = new Dictionary<int, long>();
        private readonly Dictionary<int, long> _answerVersionGuard = new Dictionary<int, long>();

        private int _maxQuestionId = 0;
        private int _maxAnswerId = 0;
        private int _maxUserId = 0;

        public Engine(IQaApiPublisher qaApiPublisher)
        {
            _questions = new PassThroughUnit<Question>();
            _answers = new PassThroughUnit<Answer>();
            _users = new PassThroughUnit<User>();

            _questions.Subscribe(new CallbackSubcriber<Question>(q => _maxQuestionId = Math.Max(_maxQuestionId, q.Id)));
            _answers.Subscribe(new CallbackSubcriber<Answer>(a => _maxAnswerId = Math.Max(_maxAnswerId, a.Id)));
            _users.Subscribe(new CallbackSubcriber<User>(u => _maxUserId = Math.Max(_maxUserId, u.Id)));

            var qaJoin = new Join<Question, Answer>(
                _questions, _answers,
                q => q.Id, a => a.QuestionId,
                q => q.Id, a => a.Id);

            new QaApiContentMaker(qaJoin).Subscribe(new CallbackSubcriber<QaApiContent>(qaApiPublisher.Publish));

            var userStatistics = new MultiJoin(
                new JoinDefinitionAdapter<User>(_users, "User", u => u.Id, u => u.Id),
                new JoinDefinitionAdapter<Question>(_questions, "Questions", q => q.UserId, q => q.Id),
                new JoinDefinitionAdapter<Answer>(_answers, "Answers", a => a.UserId, a => a.Id));

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

        public int AddUser(string userName)
        {
            User user = new User
            {
                Id = _maxUserId + 1,
                UserName = userName
            };

            _users.Push(user);
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

            _questions.Push(question);
            return question.Id;
        }

        public void UpsertQuestion(int questionId, long version, int userId, string text)
        {
            if(_questionVersionGuard.ContainsKey(questionId) && _questionVersionGuard[questionId] >= version)
                return;

            _questionVersionGuard[questionId] = version;

            Question question = new Question
            {
                Id = questionId,
                UserId = userId,
                Text = text
            };

            _questions.Push(question);
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

            _answers.Push(answer);
            return answer.Id;
        }
    }
}