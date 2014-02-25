using System.Collections.Generic;
using KaVE.JetBrains.Annotations;
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
        [NotNull]
        ITypeName Element { get; set; }

        /// <summary>
        /// The direct superclass of the type at this level.
        /// </summary>
        ITypeHierarchy Extends { get; }

        /// <summary>
        /// The interfaces directly implemented by the type at this level.
        /// </summary>
        [NotNull]
        ISet<ITypeHierarchy> Implements { get; set; }

        /// <summary>
        /// <returns>Wheather this type extends some superclass or implements any interfaces</returns>
        /// </summary>
        bool HasSupertypes { get; }

        /// <summary>
        /// <returns>Wheather this type extends some superclass</returns>
        /// </summary>
        bool HasSuperclass { get; }

        /// <summary>
        /// <returns>Wheather this type implements any interfaces</returns>
        /// </summary>
        bool IsImplementingInterfaces { get; }
    }
}