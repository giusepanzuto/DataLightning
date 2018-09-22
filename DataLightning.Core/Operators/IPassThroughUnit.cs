using Orleans;

namespace DataLightning.Core.Operators
{
    public interface IPassThroughUnit<T>: ISubscriber<T>, ISubscribable<T>, IGrainWithStringKey
    {
    }
}