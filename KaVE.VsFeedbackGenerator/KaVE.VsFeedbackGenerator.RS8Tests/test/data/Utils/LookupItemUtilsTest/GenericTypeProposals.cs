using System;
using System.Collections.Generic;

namespace CodeExamples.CompletionProposals
{
    /// <summary>
    /// Examples including different kinds of generic types, inside a generic type.
    /// The completion is triggered on the enclosing type (this reference), hence,
    /// the instantiation of the type parameter T is unknown at completion time.
    /// </summary>
    class GenericTypeProposals<T> where T : class, IEnumerable<string>
    {
        private int? _nullableInt;

        private IList<string> _simpleList;

        private IList<GenericTypeProposals<IList<string>>> _complexList;

        private IDictionary<string, object> _dictionary;

        public void TriggerCompletionHerein<A>(A param) where A : new()
        {
            this.{caret}
        }
    }
}
