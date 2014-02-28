﻿using KaVE.JetBrains.Annotations;
using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using KaVE.Utils;

namespace KaVE.Model.Events.CompletionEvent
{
    public class MethodDeclaration
    {
        /// <summary>
        ///     The name of a method.
        /// </summary>
        [NotNull]
        public IMethodName Element { get; private set; }

        /// <summary>
        ///     The implementation of the enclosing method that is referred to by calling
        ///     <code>super.'methodName'(...)</code>.
        /// </summary>
        public IMethodName Super { get; set; }

        /// <summary>
        ///     The declarations of the enclosing method, i.e., the method names specified in interfaces or the highest
        ///     parent class that the enclosing method is an implementation of.
        /// </summary>
        public IMethodName First { get; set; }

        public MethodDeclaration(IMethodName methodName)
        {
            Element = methodName;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        protected bool Equals(MethodDeclaration other)
        {
            return Element.Equals(other.Element) && Equals(Super, other.Super) && Equals(First, other.First);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Element.GetHashCode();
                hashCode = (hashCode*397) ^ (Super != null ? Super.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (First != null ? First.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format("[Element: {0}, Super: {1}, First: {2}]", Element, Super, First);
        }
    }
}