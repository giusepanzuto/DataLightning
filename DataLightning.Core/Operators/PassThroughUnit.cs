namespace DataLightning.Core.Operators
{
    public class PassThroughUnit<T> : SubscribableBase<T>, IPassThroughUnit<T>
    {
        public void Push(T value)
        {
            PushToSubscribed(value);
        }
    }
}