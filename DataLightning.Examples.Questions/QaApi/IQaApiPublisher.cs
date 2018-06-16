using DataLightning.Examples.Questions.Model;

namespace DataLightning.Examples.Questions
{
    public interface IQaApiPublisher
    {
        void Publish(QaApiContent content);
    }
}