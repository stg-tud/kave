using System.Collections.Generic;
using KaVE.Model.Names;

namespace KaVE.Model.Events.CompletionEvent
{
    public class TypeHierarchy : ITypeHierarchy
    {
        public ITypeName Element { get; set; }
        public ITypeHierarchy Extends { get; set; }
        public ISet<ITypeHierarchy> Implements { get; set; }
    }
}
