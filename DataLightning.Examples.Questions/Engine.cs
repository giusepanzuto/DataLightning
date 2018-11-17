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

        public void UpsertQuestion(long version, Question question)
        {
            if(_questionVersionGuard.ContainsKey(question.Id) && _questionVersionGuard[question.Id] >= version)
                return;

            _questionVersionGuard[question.Id] = version;

            _questions.Push(question);
        }

        public void UpsertUser(long version, User user)
        {
            if(_userVersionGuard.ContainsKey(user.Id) && _userVersionGuard[user.Id] >= version)
                return;

            _userVersionGuard[user.Id] = version;

            _users.Push(user);
        }


        public void UpsertAnswer(long version, Answer answer)
        {
            if (_answerVersionGuard.ContainsKey(answer.Id) && _answerVersionGuard[answer.Id] >= version)
                return;

            _answerVersionGuard[answer.Id] = version;

            _answers.Push(answer);
        }
    }
}