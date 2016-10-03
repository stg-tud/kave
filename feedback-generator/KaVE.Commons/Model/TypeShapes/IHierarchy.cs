using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.TypeShapes
{
    public interface IHierarchy<T>
    {
        [NotNull]
        T Element { get; set; }

        [CanBeNull]
        T Super { get; set; }

        [CanBeNull]
        T First { get; set; }

        bool IsDeclaredInParentHierarchy { get; }
    }
}