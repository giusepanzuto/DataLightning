using System.Collections.Generic;

namespace DataLightning.Core
{
    public class Subscription<T> : ISubscription
    {
        private readonly IList<ICalcUnitSubscriber<T>> _subscribers;
        private readonly ICalcUnitSubscriber<T> _subscriber;

        public Subscription(IList<ICalcUnitSubscriber<T>> subscribers, ICalcUnitSubscriber<T> subscriber)
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