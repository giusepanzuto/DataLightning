using System.Collections.Generic;

namespace DataLightning.Core
{
    public interface ICalcUnit<TInput, TOutput> : ISubscribable<TOutput>
    {
        IReadOnlyDictionary<object, IInput<TInput>> Inputs { get; }
    }
}