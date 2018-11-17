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
        public void UpdateQuestionText()
        {
            const int qId = 1;

            _sut.UpsertQuestion(1, new Question
            {
                Id = qId,
                Text = "How to build a project?",
                UserId = 0
            });

            _sut.UpsertAnswer(1, new Answer{Id = 1, Text = "Press F6", QuestionId = qId, UserId = 1});
            _sut.UpsertAnswer(1, new Answer{Id = 2, Text = "Right click on solution and then click Build.", QuestionId = qId, UserId = 1});

            _sut.UpsertQuestion(2, new Question
            {
                Id = qId,
                Text = "How to build a solution?",
                UserId = 0
            });

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

            _sut.UpsertQuestion(2, new Question
            {
                Id = qId,
                Text = "How to build a project?",
                UserId = 0
            });

            _sut.UpsertAnswer(1, new Answer{Id = 1, Text = "Press F6", QuestionId = qId, UserId = 1});
            _sut.UpsertAnswer(1, new Answer{Id = 2, Text = "Right click on solution and then click Build.", QuestionId = qId, UserId = 1});

            _sut.UpsertQuestion(1, new Question
            {
                Id = qId,
                Text = "How to build a solution?",
                UserId = 0
            });

            var expected = new QaApiContent
            {
                QuestionId = qId.ToString(),
                Question = "How to build a project?",
                Answers = new List<string>{
                    "Press F6",
                    "Right click on solution and then click Build."
                }
            };

            _lastResult.Should().BeEquivalentTo(expected);
        }

    }
}