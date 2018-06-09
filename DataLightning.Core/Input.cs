using System;

namespace DataLightning.Core
{
    public class Input : IInput
    {
        public event EventHandler Changed;

        public void OnNext(object value)
        {
            Value = value;
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public object Value { get; private set; }
    }
}