namespace DataLightning.Core.Operators;

public class PassThroughUnit<T> : ISubscriber<T>, ISubscribable<T>
{
    private readonly SubscriptionManager<T> _subscriptionManager = new();

    public void Push(T value)
    {
        _subscriptionManager.NotifySubscribers(value);
    }

    public ISubscription Subscribe(ISubscriber<T> subscriber)
    {
        return _subscriptionManager.Subscribe(subscriber);
    }
}