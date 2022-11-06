using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace DataLightning.Core.Tests.Unit
{
    public class SubscriptionTests
    {
        [Fact]
        public void OnDisposingReleaseSubscription()
        {
            var subscribers = new List<ISubscriber<object>>();
            var subscriber = new FakeObjectSubscriber<object>();
            subscribers.Add(subscriber);
            var subscription = new Subscription<object>(subscribers, subscriber);

            subscription.Dispose();

            subscribers.Should().BeEmpty();
        }
    }
}
