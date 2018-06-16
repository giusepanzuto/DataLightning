using DataLightning.Examples.Questions.Model;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace DataLightning.Examples.Questions.Tests
{
    public class EngineTests
    {
        private readonly Mock<IQaApiPublisher> _outputWriterMock;
        private readonly Engine _sut;

        private QaApiContent _lastResult;

        public EngineTests()
        {
            _outputWriterMock = new Mock<IQaApiPublisher>();
            _outputWriterMock.Setup(o => o.Publish(It.IsAny<QaApiContent>()))
                .Callback<QaApiContent>(content => _lastResult = content);

            _sut = new Engine(_outputWriterMock.Object);
        }

        [Fact]
        public void ShouldReturnTheRightId()
        {
            var qId = _sut.AddQuestion(0, "How to build a solution?");

            _sut.AddAnswer(qId, 0, "Press F6");
            _sut.AddAnswer(qId, 0, "Right click on solution and then click Build.");

            _lastResult.QuestionId.Should().BeEquivalentTo(qId.ToString());
        }

        [Fact]
        public void ShouldReturnTheRightContent()
        {
            var qId = _sut.AddQuestion(0, "How to build a solution?");

            _sut.AddAnswer(qId, 0, "Press F6");
            _sut.AddAnswer(qId, 0, "Right click on solution and then click Build.");

            var expected = new QaApiContent
            {
                QuestionId = qId.ToString(),
                Question = "How to build a solution?",
                Answers = new List<string>{
                    "Press F6",
                    "Right click on solution and then click Build."
                }
            };

            _lastResult.Should().BeEquivalentTo(expected);
        }
    }
}