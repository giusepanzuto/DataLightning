using System.IO;

namespace DataLightning.Examples.Questions
{
    public class OutputWriter : IOutputWriter
    {
        public void Push(string name, string content)
        {
            File.WriteAllText(name + ".json", content);
        }
    }
}