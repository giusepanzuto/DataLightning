using DataLightning.Core;
using DataLightning.Examples.Questions.Model;
using System.Collections.Generic;
using System.Linq;

namespace DataLightning.Examples.Questions
{
    public class OutputContentMaker : CalcUnitBase<(IList<Question>, IList<Answer>), OutputContent>
    {
        public OutputContentMaker(object input) : base(new[] { input })
        {
        }

        protected override OutputContent Execute(IDictionary<object, (IList<Question>, IList<Answer>)> args, object changedInput)
        {
            var value = args[changedInput];

            Question question = value.Item1.Single();

            return new OutputContent
            {
                QuestionId = question.Id.ToString(),
                Question = question.Text,
                Answers = value.Item2.Select(a => a.Text).ToList()
            };
        }
    }
}