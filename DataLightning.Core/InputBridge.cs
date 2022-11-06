using System;

namespace DataLightning.Core;

public class InputBridge<T> : ISubscriber<T>
{
    public string Name { get; set; }
    public Action<string, T> Callback { get; set; }
    
    public void Push(T value)
    {
        Callback(Name, value);
    }
}