using DataLightning.Examples.Questions.Model;
using Newtonsoft.Json;
using System.IO;

namespace DataLightning.Examples.Questions
{
    public class QaApiPublisher : IQaApiPublisher
    {
        private readonly ApiConfiguration _configuration;

        public QaApiPublisher(ApiConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Publish(QaApiContent content)
        {
            var filepath = Path.Combine(_configuration.PublicationPath, "Questions", content.QuestionId);
            File.WriteAllText(filepath, JsonConvert.SerializeObject(content));
        }
    }
}