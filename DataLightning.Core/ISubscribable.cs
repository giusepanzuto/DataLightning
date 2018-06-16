namespace DataLightning.Core
{
    public interface ISubscribable<T>
    {
        ISubscription Subscribe(ISubscriber<T> subscriptor);
    }
}