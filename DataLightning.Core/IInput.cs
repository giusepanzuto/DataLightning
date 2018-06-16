namespace DataLightning.Core
{
    public interface IInput<T> : ISubscriber<T>
    {
        object Key { get; }

        T Value { get; }
    }
}