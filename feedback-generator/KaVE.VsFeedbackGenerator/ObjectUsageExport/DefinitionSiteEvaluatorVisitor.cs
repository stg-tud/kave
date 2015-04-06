using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Model.SSTs.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.Visitor;
using KaVE.Commons.Model.SSTs.References;

namespace KaVE.VsFeedbackGenerator.ObjectUsageExport
{
    internal class DefinitionSiteEvaluatorVisitor :
        AbstractNodeVisitor<InvocationCollectorVisitor.QueryContext, DefinitionSite>
    {
        public override DefinitionSite Visit(IInvocationExpression entity,
            InvocationCollectorVisitor.QueryContext context)
        {
            DefinitionSite defSite;
            if (entity.MethodName.IsConstructor)
            {
                defSite = DefinitionSites.CreateDefinitionByConstructor(entity.MethodName);
            }
            else
            {
                defSite = DefinitionSites.CreateDefinitionByReturn(entity.MethodName);
            }

            return defSite;
        }

        public override DefinitionSite Visit(IConstantValueExpression expr,
            InvocationCollectorVisitor.QueryContext context)
        {
            return new DefinitionSite
            {
                kind = DefinitionSiteKind.CONSTANT
            };
        }

        public override DefinitionSite Visit(IReferenceExpression expr, InvocationCollectorVisitor.QueryContext context)
        {
            return expr.Reference.Accept(this, context);
        }

        public override DefinitionSite Visit(IFieldReference fieldRef, InvocationCollectorVisitor.QueryContext context)
        {
            return DefinitionSites.CreateDefinitionByField(fieldRef.FieldName);
        }

        public override DefinitionSite Visit(IPropertyReference propertyRef,
            InvocationCollectorVisitor.QueryContext context)
        {
            return
                DefinitionSites.CreateDefinitionByField(
                    InvocationCollectorVisitor.PropertyToFieldName(propertyRef.PropertyName));
        }

        public override DefinitionSite Visit(IVariableReference varRef, InvocationCollectorVisitor.QueryContext context)
        {
            return DefinitionSites.CreateUnknownDefinitionSite();
        }
    }
}