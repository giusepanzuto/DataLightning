using DataLightning.Examples.Questions.Model;
using Moq;
using Newtonsoft.Json;
using System.Collections.Generic;
using Xunit;

namespace DataLightning.Examples.Questions.Tests
{
    public class EngineTests
    {
        private readonly Mock<IOutputWriter> _outputWriterMock;
        private readonly Engine _sut;

        private (string Name, string Content) _lastResult;

        public EngineTests()
        {
            _outputWriterMock = new Mock<IOutputWriter>();
            _outputWriterMock.Setup(o => o.Push(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((name, content) => _lastResult = (name, content));

            _sut = new Engine(_outputWriterMock.Object);
        }

        [Fact]
        public void ShouldReturnTheRightName()
        {
            var qId = _sut.AddQuestion("How to build a solution?");

            _sut.AddAnswer(qId, "Press F6");
            _sut.AddAnswer(qId, "Right click on solution and then click Build.");

            Assert.Equal(qId.ToString(), _lastResult.Name);
        }

        [Fact]
        public void ShouldReturnTheRightContent()
        {
            var qId = _sut.AddQuestion("How to build a solution?");

            _sut.AddAnswer(qId, "Press F6");
            _sut.AddAnswer(qId, "Right click on solution and then click Build.");

            var expected = new OutputContent
            {
                QuestionId = qId.ToString(),
                Question = "How to build a solution?",
                Answers = new List<string>{
                    "Press F6",
                    "Right click on solution and then click Build."
                }
            };

            Assert.Equal(JsonConvert.SerializeObject(expected), _lastResult.Content);
        }
    }
}