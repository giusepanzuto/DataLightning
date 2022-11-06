using System;
using System.Collections.Generic;

namespace DataLightning.Core;

public sealed class InputManager : IDisposable
{
    private readonly List<ISubscription> _subscriptions = new();

    public void Add<T>(string inputName, ISubscribable<T> subscribable, Action<string, T> callback)
    {
        var input = new InputBridge<T>
        {
            Name = inputName,
            Callback = callback
        };

        _subscriptions.Add(subscribable.Subscribe(input));
    }

    public void Dispose()
    {
        _subscriptions.ForEach(s => s.Dispose());
        _subscriptions.Clear();
    }
}