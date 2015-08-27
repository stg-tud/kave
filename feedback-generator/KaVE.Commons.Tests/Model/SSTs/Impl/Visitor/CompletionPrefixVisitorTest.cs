using System.Text;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.Model.SSTs.Impl.Visitor;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.SSTs.Impl.Visitor
{
    internal class CompletionPrefixVisitorTest
    {
        [Test]
        public void ShouldGetThePrefix()
        {
            const string testPrefix = "SomeObj";

            var context = new StringBuilder();
            
            var sst = new SST();
            var methodDeclarationContainingCompletionExpression = new MethodDeclaration
            {
                Body =
                    Lists.NewList<IStatement>(
                        new ExpressionStatement
                        {
                            Expression = new CompletionExpression {Token = testPrefix}
                        })
            };
            sst.Methods.Add(methodDeclarationContainingCompletionExpression);
            
            sst.Accept(new CompletionPrefixVisitor(), context);

            Assert.AreEqual(testPrefix, context.ToString());
        }

        [Test]
        public void ShouldUseEmptyStringAsDefault()
        {
            var context = new StringBuilder();

            var sst = new SST();
            sst.Accept(new CompletionPrefixVisitor(), context);

            Assert.AreEqual(string.Empty, context.ToString());
        }
    }
}