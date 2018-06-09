using DataLightning.Core.Operators;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DataLightning.Core.Tests.Unit.Operators
{
    public class JoinTests
    {
        private readonly Join _sut;

        public JoinTests()
        {
            _sut = new Join(
                value => ((TestEntityA)value).Key,
                value => ((TestEntityB)value).Key);
        }

        [Fact]
        public void ShouldReturnMatchedElements()
        {
            object result = null;
            var subscriberMock = new Mock<ICalcUnitSubscriber>();
            subscriberMock.Setup(s => s.OnNext(It.IsAny<object>()))
                .Callback<object>(value => result = value);
            _sut.Subscribe(subscriberMock.Object);

            TestEntityA value1 = new TestEntityA { Key = 1, Value1 = "A" };
            _sut.Inputs["left"].OnNext(value1);
            TestEntityB value2 = new TestEntityB { Key = 1, Value1 = "B", Value2 = "B" };
            _sut.Inputs["right"].OnNext(value2);

            var expected = new Dictionary<object, IEnumerable<object>>
            {
                ["left"] = new[] { value1 },
                ["right"] = new[] { value2 }
            };

            Assert.Equal(expected, result);
        }

        private class TestEntityA
        {
            public int Key { get; set; }
            public string Value1 { get; set; }
        }

        private class TestEntityB
        {
            public int Key { get; set; }
            public string Value1 { get; set; }
            public string Value2 { get; set; }
        }
    }
}
