namespace DataLightning.Core
{
    public interface ISubscriber<T>
    {
        void Push(T value);
    }
}