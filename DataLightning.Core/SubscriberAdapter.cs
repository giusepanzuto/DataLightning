namespace DataLightning.Core
{
    public class SubscriberAdapter<TSource, TAdapted> : ISubscribable<TAdapted>
        where TSource : TAdapted
    {
        public SubscriberAdapter(ISubscribable<TSource> subscribable)
        {
            Adaptee = subscribable;
        }

        public ISubscribable<TSource> Adaptee { get; }

        public ISubscription Subscribe(ISubscriber<TAdapted> subscriber)
        {
            return Adaptee.Subscribe(new CallbackSubcriber<TSource>(value => subscriber.Push(value)));
        }
    }
}