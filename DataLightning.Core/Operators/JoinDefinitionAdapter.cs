using System;

namespace DataLightning.Core.Operators
{
    public class JoinDefinitionAdapter<T> : SubscriberAdapter<T, object>, IJoinDefinition
    {
        private readonly Func<T, object> _getJoinKey;
        private readonly Func<T, object> _getEntityKey;

        public JoinDefinitionAdapter(ISubscribable<T> subscribable, string name, Func<T, object> getJoinKey, Func<T, object> getEntityKey) : base(subscribable)
        {
            Name = name;
            _getJoinKey = getJoinKey;
            _getEntityKey = getEntityKey;
        }

        public string Name { get; }

        public object GetEntityKey(object entity) => _getEntityKey((T)entity);

        public object GetJoinKey(object entity) => _getJoinKey((T)entity);
    }
}