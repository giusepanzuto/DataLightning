namespace DataLightning.Core
{
    public interface IInput<T> : ICalcUnitSubscriber<T>
    {
        object Key { get; }

        T Value { get; }
    }
}