using System;

namespace DataLightning.Core
{
    public class CallbackSubcriber<T> : ICalcUnitSubscriber<T>
    {
        private readonly Action<T> _callback;

        public CallbackSubcriber(Action<T> callback)
        {
            _callback = callback;
        }

        public void Submit(T value)
        {
            _callback(value);
        }
    }
}