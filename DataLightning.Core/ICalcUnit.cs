using System.Collections.Generic;

namespace DataLightning.Core
{
    public interface ICalcUnit
    {
        IReadOnlyDictionary<object, IInput> Inputs { get; }

        ISubscription Subscribe(ICalcUnitSubscriber subscriptor);
    }
}