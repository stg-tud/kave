namespace KaVE.Model.Names
{
    /// <summary>
    /// Represents full-qualified names.
    /// </summary>
    public interface IName
    {
        /// <summary>
        /// Returns a unique representation of the qualified name.
        /// </summary>
        string Identifier { get; }
    }
}