using System;
using KaVE.JetBrains.Annotations;

namespace KaVE.Utils
{
    public static class EqualityUtils
    {
        public static bool Equals<T>([NotNull] this T self, object other, [NotNull] Predicate<T> equalsIfSameType)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(self, other))
            {
                return true;
            }
            if (other.GetType() != self.GetType())
            {
                return false;
            }
            return equalsIfSameType((T) other);
        }
    }
}
