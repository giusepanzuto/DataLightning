using System;

namespace DataLightning.Core
{
    public interface ISubscribable<T>
    {
        ISubscription Subscribe(ICalcUnitSubscriber<T> subscriptor);
    }
}