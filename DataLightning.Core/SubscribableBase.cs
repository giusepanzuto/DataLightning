using System.Collections.Generic;

namespace DataLightning.Core
{
    public abstract class SubscribableBase<T> : ISubscribable<T>
    {
        private readonly List<ISubscriber<T>> _subscribers = new();

        public ISubscription Subscribe(ISubscriber<T> subscriber)
        {
            _subscribers.Add(subscriber);
            return new Subscription<T>(_subscribers, subscriber);
        }

        protected void PushToSubscribers(T value)
        {
            foreach (var s in _subscribers)
                s.Push(value);
        }
    }
}