namespace DataLightning.Core.Tests.Unit;

public class TestSubscribable<T>:ISubscribable<T>
{
    private readonly SubscriptionManager<T> _subscriptionManager = new();

    public ISubscription Subscribe(ISubscriber<T> subscriber) => 
        _subscriptionManager.Subscribe(subscriber);

    public void Emit(T value)
    {
        _subscriptionManager.NotifySubscribers(value);
    }
}