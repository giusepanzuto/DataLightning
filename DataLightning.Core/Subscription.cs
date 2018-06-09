using System.Collections.Generic;

namespace DataLightning.Core
{
    public class Subscription : ISubscription
    {
        private readonly IList<ICalcUnitSubscriber> _subscribers;
        private readonly ICalcUnitSubscriber _subscriber;

        public Subscription(IList<ICalcUnitSubscriber> subscribers, ICalcUnitSubscriber subscriber)
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