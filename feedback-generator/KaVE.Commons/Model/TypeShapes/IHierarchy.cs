using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.TypeShapes
{
    public interface IHierarchy<TMember> where TMember : IMemberName
    {
        [NotNull]
        TMember Element { get; set; }

        [CanBeNull]
        TMember Super { get; set; }

        [CanBeNull]
        TMember First { get; set; }

        bool IsDeclaredInParentHierarchy { get; }
    }
}