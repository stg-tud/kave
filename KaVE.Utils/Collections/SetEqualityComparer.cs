using System.Collections.Generic;
using System.Linq;

namespace KaVE.Utils.Collections
{
    /// <summary>
    /// Compares sets based on the equality of their content.
    /// </summary>
    /// <typeparam name="TElement"></typeparam>
    public class SetEqualityComparer<TElement> : IEqualityComparer<ISet<TElement>>
    {
        /// <summary>
        /// Uses <see cref="ISet{T}.SetEquals"/> to check for equality.
        /// </summary>
        /// <returns><code>true</code>, if the sets have the same size and for
        /// every element in <code>x</code> there is an equal element in
        /// <code>y</code>. <code>false</code> otherwise.</returns>
        public bool Equals(ISet<TElement> x, ISet<TElement> y)
        {
            return x.SetEquals(y);
        }

        /// <summary>
        /// The code is the sum of the hashcodes of the set's elements, i.e.,
        /// it is equal for sets containing equal elements.
        /// </summary>
        public int GetHashCode(ISet<TElement> obj)
        {
            return obj.Aggregate(0, (hc, next) => hc + next.GetHashCode());
        }
    }
}