using System.Collections.Generic;

namespace DataLightning.Core
{
    public class Subscription<T> : ISubscription
    {
        private readonly IList<ISubscriber<T>> _subscribers;
        private readonly ISubscriber<T> _subscriber;

        public Subscription(IList<ISubscriber<T>> subscribers, ISubscriber<T> subscriber)
        {
            _subscribers = subscribers;
            _subscriber = subscriber;
        }

        public void Dispose()
        {
            if (_subscribers.Contains(_subscriber))
                _subscribers.Remove(_subscriber);
        }
    }
}