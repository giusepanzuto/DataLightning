using System;

namespace DataLightning.Core
{
    public class Input<T> : IInput<T>
    {
        public event EventHandler Changed;

        public Input(object key)
        {
            Key = key;
        }

        public void Submit(T value)
        {
            Value = value;
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public object Key { get; }

        public T Value { get; private set; }
    }
}