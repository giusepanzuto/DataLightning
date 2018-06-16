using System;

namespace DataLightning.Core
{
    public class CallbackSubcriber<T> : ISubscriber<T>
    {
        private readonly Action<T> _callback;

        public CallbackSubcriber(Action<T> callback)
        {
            _callback = callback;
        }

        public void Push(T value)
        {
            _callback(value);
        }
    }
}