namespace DataLightning.Core.Operators
{
    public class PassThroughUnit<T> : SubscribableBase<T>, ISubscriber<T>
    {
        public void Push(T value)
        {
            PushToSubscribers(value);
        }
    }
}