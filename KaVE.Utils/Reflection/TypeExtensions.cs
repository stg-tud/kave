using System;
using System.Linq.Expressions;

namespace KaVE.Utils.Reflection
{
    public static class TypeExtensions<T>
    {
        public static string GetPropertyName<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            if (expression.Body.NodeType == ExpressionType.MemberAccess)
            {
                return ((MemberExpression) expression.Body).Member.Name;
            }

            if ((expression.Body.NodeType == ExpressionType.Convert) && (expression.Body.Type == typeof (object)))
            {
                return ((MemberExpression) ((UnaryExpression) expression.Body).Operand).Member.Name;
            }

            throw new Exception(
                string.Format(
                    "Invalid expression type: Expected ExpressionType.MemberAccess, Found {0}",
                    expression.Body.NodeType));
        }
    }
}