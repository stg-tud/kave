namespace KaVE.Model.Names
{
    public interface IEventName : IMemberName
    {
        ITypeName HandlerType { get; }
    }
}
