using System.Collections.Generic;

namespace DataLightning.Core.Tests.Unit;

class FakeObjectSubscriber<T> : ISubscriber<T>
{
    public List<T> Values { get; } = new();


    public void Push(T value)
    {
        Values.Add(value);
    }
}