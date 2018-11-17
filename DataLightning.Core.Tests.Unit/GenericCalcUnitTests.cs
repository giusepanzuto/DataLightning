using DataLightning.Core.Operators;
using Moq;
using System.Linq;
using Xunit;

namespace DataLightning.Core.Tests.Unit
{
    public class GenericCalcUnitTests
    {
        private readonly GenericCalcUnit<int, int> _sut;
        private readonly PassThroughUnit<int> _input1;
        private readonly PassThroughUnit<int> _input2;
        private readonly PassThroughUnit<int> _input3;

        public GenericCalcUnitTests()
        {
            _input1 = new PassThroughUnit<int>();
            _input2 = new PassThroughUnit<int>();
            _input3 = new PassThroughUnit<int>();

            _sut = new GenericCalcUnit<int, int>(args => args.Values.Sum(),
                _input1, _input2, _input3);
        }

        [Fact]
        public void ShouldNotifyTheCorrectResultToSubscriber()
        {
            object result = null;
            var subscriptorMock = new Mock<ISubscriber<int>>();
            subscriptorMock
                .Setup(s => s.Push(It.IsAny<int>()))
                .Callback<object>(value => result = value);

            _sut.Subscribe(subscriptorMock.Object);

            _input1.Push(2);

            Assert.Equal(2, result);

            _input2.Push(3);

            Assert.Equal(5, result);

            _input3.Push(5);

            Assert.Equal(10, result);
        }

        [Fact]
        public void ShouldNotifyTheResultToSubscriber()
        {
            var subscriptorMock = new Mock<ISubscriber<int>>();

            _sut.Subscribe(subscriptorMock.Object);

            _input1.Push(1);

            subscriptorMock.Verify(s => s.Push(It.IsAny<int>()), Times.Once);
            subscriptorMock.Verify(s => s.Push(It.Is<int>(arg => arg == 1)), Times.Once);
        }

        [Fact]
        public void ShouldNotNotifyTheResultToSubscriberIfResultIsNotChanged()
        {
            var subscriptorMock = new Mock<ISubscriber<int>>();

            _sut.Subscribe(subscriptorMock.Object);

            _input1.Push(1);

            _input1.Push(1);
            _input1.Push(1);

            subscriptorMock.Verify(s => s.Push(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void ShouldReturnASubscriptionOnSubscribe()
        {
            var subscriptorMock = new Mock<ISubscriber<int>>();
            var result = _sut.Subscribe(subscriptorMock.Object);

            Assert.NotNull(result);
        }
    }
}