namespace DataLightning.Core
{
    public interface ICalcUnitSubscriber<T>
    {
        void OnNext(T value);
    }
}