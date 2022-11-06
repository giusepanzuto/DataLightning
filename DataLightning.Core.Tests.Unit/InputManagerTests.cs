using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace DataLightning.Core.Tests.Unit;

public class InputManagerTests
{
    [Fact]
    public void CallbackInputChanged()
    {
        var calls = new List<int>();
        var inputManager = new InputManager();
        var subscribable = new TestSubscribable<int>();
        inputManager.Add("Input1", subscribable, (_, value) => calls.Add(value));

        subscribable.Emit(12);

        calls.Should().BeEquivalentTo(new[] { 12 });
    }

    [Fact]
    public void AvoidCallbackWhenDisposed()
    {
        var calls = new List<int>();
        var inputManager = new InputManager();
        var subscribable = new TestSubscribable<int>();
        inputManager.Add("Input1", subscribable, (_, value) => calls.Add(value));
        inputManager.Dispose();

        subscribable.Emit(12);

        calls.Should().BeEmpty();
    }


    [Fact]
    public void CallbackValuesManyInputChanged()
    {
        var inputManager = new InputManager();
        var calls = new List<int>();
        var subscribables = Enumerable.Range(0, 10).Select(i => (new TestSubscribable<int>(), i)).ToList();
        subscribables.ForEach(s => inputManager.Add($"Input{s.i}", s.Item1, (_, value) => calls.Add(value)));

        subscribables.ForEach(s => s.Item1.Emit(s.i));

        calls.Should().BeEquivalentTo(Enumerable.Range(0, 10));
    }

    [Fact]
    public void CallbackNamesManyInputChanged()
    {
        var inputManager = new InputManager();
        var calls = new List<string>();
        var subscribables = Enumerable.Range(0, 10).Select(i => (new TestSubscribable<int>(), i)).ToList();
        subscribables.ForEach(s => inputManager.Add($"Input{s.i}", s.Item1, (name, _) => calls.Add(name)));

        subscribables.ForEach(s => s.Item1.Emit(s.i));

        calls.Should().BeEquivalentTo(Enumerable.Range(0, 10).Select(i => $"Input{i}"));
    }
}