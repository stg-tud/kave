using System.Collections.Generic;
using KaVE.Model.Names;
using KaVE.Utils;
using Lists = KaVE.Model.Collections.Lists;

namespace KaVE.Model.SSTs
{
    public class MethodDeclaration : Expression
    {
        public IMethodName Name;
        public readonly IList<Expression> Body = Lists.NewList<Expression>();

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        protected bool Equals(MethodDeclaration other)
        {
            return Equals(Name, other.Name) && Equals(Body, other.Body);
        }

        public override int GetHashCode()
        {
            var nameHash = Name != null ? Name.GetHashCode() : 0;
            return nameHash*20349 + HashCodeUtils.For(3456, Body);
        }
    }
}