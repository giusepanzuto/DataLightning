using System;

namespace DataLightning.Core.Operators
{
    public class JoinDefinitionAdapter<T> : SubscriberAdapter<T, object>, IJoinDefinition
    {
        private readonly Func<T, object> _getJoinKey;

        public JoinDefinitionAdapter(ISubscribable<T> subscribable, string name, Func<T, object> getJoinKey) : base(subscribable)
        {
            Name = name;
            _getJoinKey = getJoinKey;
        }

        public string Name { get; }

        public object GetJoinKey(object entity)
        {
            return _getJoinKey((T)entity);
        }
    }
}