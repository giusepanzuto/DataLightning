namespace DataLightning.Core.Operators
{
    public interface IJoinDefinition : ISubscribable<object>
    {
        string Name { get; }

        object GetJoinKey(object entity);

        object GetEntityKey(object entity);
    }
}