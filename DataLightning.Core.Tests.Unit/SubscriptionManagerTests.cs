using System.Linq;
using FluentAssertions;
using Xunit;

namespace DataLightning.Core.Tests.Unit;

public class SubscriptionManagerTests
{
    [Fact]
    public void NotifyOneSubscriber()
    {
        var subscriptionManager = new SubscriptionManager<string>();
        var subscriber = new FakeObjectSubscriber<string>();
        subscriptionManager.Subscribe(subscriber);

        subscriptionManager.NotifySubscribers("AAA");

        subscriber.Values.Should().BeEquivalentTo("AAA");
    }

    [Fact]
    public void NotifyManySubscriber()
    {
        var subscriptionManager = new SubscriptionManager<string>();
        var subscribers = Enumerable.Range(0, 10).Select(i => new FakeObjectSubscriber<string>()).ToList();
        subscribers.ForEach(s => subscriptionManager.Subscribe(s));

        subscriptionManager.NotifySubscribers("AAA");

        subscribers.ForEach(s => s.Values.Should().BeEquivalentTo("AAA"));
    }

    [Fact]
    public void NotNotifyDisposedSubscription()
    {
        var subscriptionManager = new SubscriptionManager<string>();
        var subscriber = new FakeObjectSubscriber<string>();
        var subscription = subscriptionManager.Subscribe(subscriber);
        subscription.Dispose();

        subscriptionManager.NotifySubscribers("AAA");

        subscriber.Values.Should().BeEmpty();
    }
}