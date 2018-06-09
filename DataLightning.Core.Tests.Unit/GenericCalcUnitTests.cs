using Moq;
using System.Linq;
using Xunit;

namespace DataLightning.Core.Tests.Unit
{
    public class GenericCalcUnitTests
    {
        private readonly GenericCalcUnit<int, int> _sut;

        public GenericCalcUnitTests()
        {
            _sut = new GenericCalcUnit<int, int>(new object[] { 1, 2, 3 },
                args => args.Values.Sum());
        }

        [Fact]
        public void ShouldContainsNotNullInput()
        {
            foreach (var input in _sut.Inputs.Values)
                Assert.NotNull(input);
        }

        [Fact]
        public void ShouldHaveTheCorrectNumberOfInput()
        {
            Assert.Equal(3, _sut.Inputs.Count);
        }

        [Fact]
        public void ShouldNotifyTheCorrectResultToSubscriber()
        {
            object result = null;
            var subscriptorMock = new Mock<ICalcUnitSubscriber<int>>();
            subscriptorMock
                .Setup(s => s.OnNext(It.IsAny<int>()))
                .Callback<object>(value => result = value);

            _sut.Subscribe(subscriptorMock.Object);

            _sut.Inputs[1].OnNext(2);
            _sut.Inputs[2].OnNext(3);
            _sut.Inputs[3].OnNext(5);

            Assert.Equal(10, result);
        }

        [Fact]
        public void ShouldNotifyTheResultToSubscriber()
        {
            var subscriptorMock = new Mock<ICalcUnitSubscriber<int>>();

            _sut.Subscribe(subscriptorMock.Object);

            _sut.Inputs[1].OnNext(1);

            subscriptorMock.Verify(s => s.OnNext(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void ShouldNotNotifyTheResultToSubscriberIfResultIsNotChanged()
        {
            var subscriptorMock = new Mock<ICalcUnitSubscriber<int>>();

            _sut.Subscribe(subscriptorMock.Object);

            _sut.Inputs[1].OnNext(1);

            _sut.Inputs[1].OnNext(1);
            _sut.Inputs[1].OnNext(1);

            subscriptorMock.Verify(s => s.OnNext(It.IsAny<int>()), Times.Once);
        }


        [Fact]
        public void ShouldReturnASubscriptionOnSubscribe()
        {
            var subscriptorMock = new Mock<ICalcUnitSubscriber<int>>();
            var result = _sut.Subscribe(subscriptorMock.Object);

            Assert.NotNull(result);
        }
    }
}