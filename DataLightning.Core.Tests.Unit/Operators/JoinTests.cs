using DataLightning.Core.Operators;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace DataLightning.Core.Tests.Unit.Operators
{
    public class JoinTests
    {
        private readonly Join<TestEntityA, TestEntityB> _sut;
        private readonly PassThroughUnit<TestEntityA> _inputLeft;
        private readonly PassThroughUnit<TestEntityB> _inputRight;

        public JoinTests()
        {
            _inputLeft = new PassThroughUnit<TestEntityA>();
            _inputRight = new PassThroughUnit<TestEntityB>();

            _sut = new Join<TestEntityA, TestEntityB>(_inputLeft, _inputRight,
                value => value.KeyA,
                value => value.KeyB);
        }

        [Fact]
        public void ShouldReturnMatchedElements()
        {
            (IList<TestEntityA>, IList<TestEntityB>) result = (null, null);
            var subscriberMock = new Mock<ISubscriber<(IList<TestEntityA>, IList<TestEntityB>)>>();
            subscriberMock.Setup(s => s.Push(It.IsAny<(IList<TestEntityA>, IList<TestEntityB>)>()))
                .Callback<(IList<TestEntityA>, IList<TestEntityB>)>(value => result = value);
            _sut.Subscribe(subscriberMock.Object);

            TestEntityA value1 = new TestEntityA { KeyA = 1, Value1 = "A" };
            _inputLeft.Push(value1);
            TestEntityB value2 = new TestEntityB { KeyB = 1, Value1 = "B", Value2 = "B" };
            _inputRight.Push(value2);

            var expected = (new List<TestEntityA> { value1 }, new List<TestEntityB> { value2 });

            Assert.Equal(expected.Item1, result.Item1);
            Assert.Equal(expected.Item2, result.Item2);
        }
    }
}