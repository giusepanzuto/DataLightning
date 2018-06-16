using System.Collections.Generic;

namespace DataLightning.Examples.Questions.Model
{
    public class QaApiContent
    {
        public string QuestionId { get; set; }

        public string Question { get; set; }

        public IList<string> Answers { get; set; }
    }
}