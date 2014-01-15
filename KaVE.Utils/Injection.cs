using System;
using KaVE.JetBrains.Annotations;

namespace KaVE.Utils
{
    /// <summary>
    /// KAVE injection marker.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field,
        AllowMultiple = false, Inherited = true), MeansImplicitUse]
    public class InjectAttribute : Attribute {}
}