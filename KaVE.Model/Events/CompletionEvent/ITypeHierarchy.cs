using System.Collections.Generic;
using KaVE.Model.Names;

namespace KaVE.Model.Events.CompletionEvent
{
    /// <summary>
    /// Represents one level of a type hierarchy.
    /// </summary>
    public interface ITypeHierarchy
    {
        /// <summary>
        /// The type at this level in the type hierarchy.
        /// </summary>
        ITypeName Element { get; set; }

        /// <summary>
        /// The direct superclass of the type at this level.
        /// </summary>
        ITypeHierarchy Extends { get; }

        /// <summary>
        /// The interfaces directly implemented by the type at this level.
        /// </summary>
        ISet<ITypeHierarchy> Implements { get; } 
    }
}