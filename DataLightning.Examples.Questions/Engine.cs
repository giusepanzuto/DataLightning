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

        private int _maxQuestionId = 0;
        private int _maxAnswerId = 0;
        private int _maxUserId = 0;

        public Engine(IQaApiPublisher qaApiPublisher)
        {
            _questions = new PassThroughUnit<Question>();
            _answers = new PassThroughUnit<Answer>();
            _users = new PassThroughUnit<User>();

            var qaJoin = new Join<Question, Answer>(_questions, _answers, q => q.Id, a => a.QuestionId);

            var uqJoin = new Join<User, Question>(_users, _questions, u => u.Id, q => q.UserId);
            var uaJoin = new Join<User, Answer>(_users, _answers, u => u.Id, a => a.UserId);

            _questions.Subscribe(new CallbackSubcriber<Question>(q => _maxQuestionId = Math.Max(_maxQuestionId, q.Id)));
            _answers.Subscribe(new CallbackSubcriber<Answer>(a => _maxAnswerId = Math.Max(_maxAnswerId, a.Id)));
            _users.Subscribe(new CallbackSubcriber<User>(u => _maxUserId = Math.Max(_maxUserId, u.Id)));

            new QaApiContentMaker(qaJoin).Subscribe(new CallbackSubcriber<QaApiContent>(qaApiPublisher.Publish));
        }

        public int AddUser(string userName)
        {
            User user = new User
            {
                Id = _maxUserId + 1,
                UserName = userName
            };

            _users.Input.Submit(user);
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

            _questions.Input.Submit(question);
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

            _answers.Input.Submit(answer);
            return answer.Id;
        }
    }
}