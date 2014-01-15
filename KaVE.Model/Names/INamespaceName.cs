namespace KaVE.Model.Names
{
    public interface INamespaceName : IName
    {
        INamespaceName ParentNamespace { get; }
        string Name { get; }

        /// <summary>
        /// <returns>Wheather this is the global (or default) namespace</returns>
        /// </summary>
        bool IsGlobalNamespace { get; }
    }
}