using DataLightning.Examples.Questions.Model;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace DataLightning.Examples.Questions.Tests
{
    public class EngineTests
    {
        private readonly Engine _sut;

        private QaApiContent _lastResult;

        public EngineTests()
        {
            var outputWriterMock = new Mock<IQaApiPublisher>();
            outputWriterMock.Setup(o => o.Publish(It.IsAny<QaApiContent>()))
                .Callback<QaApiContent>(content => _lastResult = content);

            _sut = new Engine(outputWriterMock.Object);
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
        public void UpdateQuestionText()
        {
            const int qId = 1;

            _sut.UpsertQuestion(qId, 1, 0, "How to build a project?");

            _sut.AddAnswer(qId, 0, "Press F6");
            _sut.AddAnswer(qId, 0, "Right click on solution and then click Build.");

            _sut.UpsertQuestion(qId, 2, 0, "How to build a solution?");

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

        [Fact]
        public void SkipUpdateQuestionWhenOlder()
        {
            const int qId = 1;

            _sut.UpsertQuestion(qId, 2, 0, "question v2");

            _sut.AddAnswer(qId, 0, "a1");
            _sut.AddAnswer(qId, 0, "a2");

            _sut.UpsertQuestion(qId, 1, 0, "oldest one");

            var expected = new QaApiContent
            {
                QuestionId = qId.ToString(),
                Question = "question v2",
                Answers = new List<string>{
                    "a1",
                    "a2"
                }
            };

            _lastResult.Should().BeEquivalentTo(expected);
        }

    }
}