using System.Collections.Generic;

namespace DataLightning.Core
{
    public abstract class SubscribableBase<T> : ISubscribable<T>
    {
        private readonly IList<ISubscriber<T>> _subscribers = new List<ISubscriber<T>>();

        public ISubscription Subscribe(ISubscriber<T> subscriptor)
        {
            _subscribers.Add(subscriptor);
            return new Subscription<T>(_subscribers, subscriptor);
        }

        protected void PushToSubscribers(T value)
        {
            foreach (var s in _subscribers)
                s.Push(value);
        }
    }
}