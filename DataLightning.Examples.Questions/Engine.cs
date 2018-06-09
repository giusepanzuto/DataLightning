using DataLightning.Core;
using DataLightning.Core.Operators;
using DataLightning.Examples.Questions.Model;
using Newtonsoft.Json;
using System.Linq;

namespace DataLightning.Examples.Questions
{
    public class Engine
    {
        private readonly IOutputWriter _outputWriter;
        private readonly GenericCalcUnit<Question, Question> _questions;
        private readonly GenericCalcUnit<Answer, Answer> _answers;

        private int _maxQuestionId = 0;
        private int _maxAnswerId = 0;

        public Engine(IOutputWriter outputWriter)
        {
            _questions = new GenericCalcUnit<Question, Question>(new[] { "Q" }, args => args.Values.Single());
            _answers = new GenericCalcUnit<Answer, Answer>(new[] { "A" }, args => args.Values.Single());

            var join = new Join<Question, Answer>(_questions, _answers,
                q => q.Id,
                a => a.QuestionId);

            var maxQ = new MaxId<Question>(_questions, q => q.Id);
            maxQ.Subscribe(new CallbackSubcriber<int>(v => _maxQuestionId = v));

            var maxA = new MaxId<Answer>(_answers, a => a.Id);
            maxA.Subscribe(new CallbackSubcriber<int>(v => _maxAnswerId = v));
            _outputWriter = outputWriter;

            var output = new OutputContentMaker(join);
            output.Subscribe(new CallbackSubcriber<OutputContent>(content => outputWriter.Push(
                content.QuestionId,
                JsonConvert.SerializeObject(content))));
        }

        public int AddQuestion(string text)
        {
            Question question = new Question { Id = _maxQuestionId + 1, Text = text };
            _questions.Inputs["Q"].Submit(question);
            return question.Id;
        }

        public int AddAnswer(int questionId, string text)
        {
            Answer answer = new Answer { Id = _maxAnswerId + 1, QuestionId = questionId, Text = text };
            _answers.Inputs["A"].Submit(answer);
            return answer.Id;
        }
    }
}