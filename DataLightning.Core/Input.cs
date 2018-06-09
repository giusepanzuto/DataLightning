using System;

namespace DataLightning.Core
{
    public class Input : IInput
    {
        public event EventHandler Changed;

        public Input(object key)
        {
            Key = key;
        }

        public void OnNext(object value)
        {
            Value = value;
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public object Key { get; }

        public object Value { get; private set; }
    }
}