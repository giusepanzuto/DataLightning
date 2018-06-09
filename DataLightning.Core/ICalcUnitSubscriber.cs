namespace DataLightning.Core
{
    public interface ICalcUnitSubscriber<T>
    {
        void Submit(T value);
    }
}