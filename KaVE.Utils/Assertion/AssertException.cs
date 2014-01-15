using System;
using KaVE.JetBrains.Annotations;

namespace KaVE.Utils.Assertion
{
    public class AssertException : Exception
    {
        public AssertException([NotNull] string message) : base(message)
        {
        }
    }
}