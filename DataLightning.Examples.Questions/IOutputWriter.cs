namespace DataLightning.Examples.Questions
{
    public interface IOutputWriter
    {
        void Push(string name, string content);
    }
}