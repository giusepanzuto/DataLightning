using System.Collections.Generic;

namespace DataLightning.Core;

public class SubscriptionManager<T>
{
    private readonly List<ISubscriber<T>> _subscribers = new();

    public ISubscription Subscribe(ISubscriber<T> subscriber)
    {
        _subscribers.Add(subscriber);
        return new Subscription<T>(_subscribers, subscriber);
    }

    public void NotifySubscribers(T value)
    {
        foreach (var s in _subscribers)
            s.Push(value);
    }
}