/*
 * Copyright 2014 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * Contributors:
 *    - Roman Fojtik
 */

using System;
using System.Collections.Generic;
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Blocks;
using KaVE.Commons.Model.SSTs.Declarations;
using KaVE.Commons.Model.SSTs.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Expressions.LoopHeader;
using KaVE.Commons.Model.SSTs.Impl.Visitor;
using KaVE.Commons.Model.SSTs.References;
using KaVE.Commons.Model.SSTs.Statements;
using KaVE.Commons.Utils.Assertion;

namespace KaVE.Commons.Utils.ObjectUsageExport
{
    internal class InvocationCollectorVisitor : AbstractNodeVisitor<InvocationCollectorVisitor.QueryContext>
    {
        public class QueryContext
        {
            public QueryContext()
            {
                TypeNameQueryDictionary = new Dictionary<ITypeName, Query>();
                IdentifierTypeNameDictionary = new Dictionary<string, ITypeName>();
            }

            internal ITypeName EnclosingType { get; set; }

            internal IMethodName EnclosingMethod { get; set; }

            internal IDictionary<ITypeName, Query> TypeNameQueryDictionary { get; private set; }

            internal IDictionary<string, ITypeName> IdentifierTypeNameDictionary { get; private set; }

            public ICollection<Query> GetQueries()
            {
                return TypeNameQueryDictionary.Values;
            }
        }

        public override void Visit(IMethodDeclaration stmt, QueryContext context)
        {
            // set query for this and base
            try
            {
                var usage = new Query
                {
                    type = context.EnclosingType.ToCoReName(),
                    classCtx = context.EnclosingType.ToCoReName(),
                    methodCtx = context.EnclosingMethod.ToCoReName(),
                    definition = new DefinitionSite
                    {
                        kind = DefinitionSiteKind.THIS,
                    }
                };
                context.TypeNameQueryDictionary[context.EnclosingType] = usage;
            }
            catch (AssertException e)
            {
                // TODO test ad proper handling
                Console.WriteLine("error creating usage:\n{0}", e);
            }

            foreach (var param in stmt.Name.Parameters)
            {
                try
                {
                    CreateQuery(
                        context,
                        param.Name,
                        param.ValueType,
                        DefinitionSites.CreateDefinitionByParam(stmt.Name, stmt.Name.Parameters.IndexOf(param)));
                }
                catch (AssertException e)
                {
                    // TODO test ad proper handling
                    Console.WriteLine("error creating usage:\n{0}", e);
                }
            }

            foreach (var s in stmt.Body)
            {
                s.Accept(this, context);
            }
        }

        public override void Visit(ISST sst, QueryContext context)
        {
            context.EnclosingType = sst.EnclosingType;


            var type = sst.EnclosingType;

            context.IdentifierTypeNameDictionary.Add("this", type);
            context.IdentifierTypeNameDictionary.Add("base", type);
            //CreateQuery();

            foreach (var method in sst.Methods)
            {
                context.EnclosingMethod = method.Name;
                foreach (var field in sst.Fields)
                {
                    field.Accept(this, context);
                }

                foreach (var property in sst.Properties)
                {
                    property.Accept(this, context);
                }

                method.Accept(this, context);
            }
        }

        public override void Visit(IForEachLoop block, QueryContext context)
        {
            block.Declaration.Accept(this, context);

            foreach (var statement in block.Body)
            {
                statement.Accept(this, context);
            }
        }

        public override void Visit(IForLoop block, QueryContext context)
        {
            foreach (var statement in block.Init)
            {
                statement.Accept(this, context);
            }

            block.Condition.Accept(this, context);

            foreach (var statement in block.Body)
            {
                statement.Accept(this, context);
            }

            foreach (var statement in block.Step)
            {
                statement.Accept(this, context);
            }
        }

        public override void Visit(IIfElseBlock block, QueryContext context)
        {
            foreach (var statement in block.Then)
            {
                statement.Accept(this, context);
            }
            foreach (var statement in block.Else)
            {
                statement.Accept(this, context);
            }
        }

        public override void Visit(ILockBlock stmt, QueryContext context)
        {
            foreach (var statement in stmt.Body)
            {
                statement.Accept(this, context);
            }
        }

        public override void Visit(ISwitchBlock block, QueryContext context)
        {
            foreach (var caseBlock in block.Sections)
            {
                foreach (var statement in caseBlock.Body)
                {
                    statement.Accept(this, context);
                }
            }
            foreach (var statement in block.DefaultSection)
            {
                statement.Accept(this, context);
            }
        }

        public override void Visit(ITryBlock block, QueryContext context)
        {
            foreach (var statement in block.Body)
            {
                statement.Accept(this, context);
            }
            foreach (var catchBlock in block.CatchBlocks)
            {
                // TODO: Create query for params in catchblock
                CreateQuery(
                    context,
                    catchBlock.Parameter.Name,
                    catchBlock.Parameter.ValueType,
                    DefinitionSites.CreateUnknownDefinitionSite());

                foreach (var statement in catchBlock.Body)
                {
                    statement.Accept(this, context);
                }
            }
            foreach (var statement in block.Finally)
            {
                statement.Accept(this, context);
            }
        }

        public override void Visit(IUncheckedBlock block, QueryContext context)
        {
            foreach (var statement in block.Body)
            {
                statement.Accept(this, context);
            }
        }

        public override void Visit(IUsingBlock block, QueryContext context)
        {
            foreach (var statement in block.Body)
            {
                statement.Accept(this, context);
            }
        }

        public override void Visit(ILabelledStatement stmt, QueryContext context)
        {
            stmt.Statement.Accept(this, context);
        }

        public override void Visit(IWhileLoop block, QueryContext context)
        {
            block.Condition.Accept(this, context);
            foreach (var statement in block.Body)
            {
                statement.Accept(this, context);
            }
        }

        public override void Visit(IDoLoop block, QueryContext context)
        {
            block.Condition.Accept(this, context);
            foreach (var statement in block.Body)
            {
                statement.Accept(this, context);
            }
        }

        public override void Visit(ILoopHeaderBlockExpression expr, QueryContext context)
        {
            foreach (var statement in expr.Body)
            {
                statement.Accept(this, context);
            }
        }

        public override void Visit(IVariableDeclaration stmt, QueryContext context)
        {
            if (!stmt.IsMissing)
            {
                try
                {
                    CreateQuery(
                        context,
                        stmt.Reference.Identifier,
                        stmt.Type,
                        DefinitionSites.CreateUnknownDefinitionSite());
                }
                catch (AssertException e)
                {
                    // TODO test and proper handling
                    Console.WriteLine("error creating query:\n{0}", e);
                }
            }
        }

        public override void Visit(IFieldDeclaration stmt, QueryContext context)
        {
            try
            {
                CreateQuery(
                    context,
                    stmt.Name.Name,
                    stmt.Name.ValueType,
                    DefinitionSites.CreateDefinitionByField(stmt.Name));
            }
            catch (AssertException e)
            {
                // TODO test and proper handling
                Console.WriteLine("error creating query:\n{0}", e);
            }
        }

        // treating property as field
        public override void Visit(IPropertyDeclaration stmt, QueryContext context)
        {
            try
            {
                CreateQuery(
                    context,
                    stmt.Name.Name,
                    stmt.Name.ValueType,
                    DefinitionSites.CreateDefinitionByField(PropertyToFieldName(stmt.Name)));
            }
            catch (AssertException e)
            {
                // TODO test and proper handling
                Console.WriteLine("error creating query:\n{0}", e);
            }
        }

        public override void Visit(IAssignment stmt, QueryContext context)
        {
            stmt.Expression.Accept(this, context);

            var variableReference = stmt.Reference as IVariableReference;
            if (variableReference != null)
            {
                var expressionValueTypeVisitor = new DefinitionSiteEvaluatorVisitor();
                var newDefinitionSite = stmt.Expression.Accept(expressionValueTypeVisitor, context);

                // TODO @seb: test and proper handling
                if (context.IdentifierTypeNameDictionary.ContainsKey(variableReference.Identifier))
                {
                    var type = context.IdentifierTypeNameDictionary[variableReference.Identifier];
                    // TODO @seb: test and proper handling
                    if (context.TypeNameQueryDictionary.ContainsKey(type))
                    {
                        context.TypeNameQueryDictionary[type].definition = newDefinitionSite;
                    }
                }
            }
        }

        public override void Visit(IExpressionStatement stmt, QueryContext context)
        {
            stmt.Expression.Accept(this, context);
        }

        public override void Visit(IInvocationExpression entity, QueryContext context)
        {
            if (entity.MethodName.IsConstructor)
            {
                var type = entity.MethodName.DeclaringType;
                var usage = FindOrCreateUsage(type, context);
                usage.definition = DefinitionSites.CreateDefinitionByConstructor(entity.MethodName);
            }
            else
            {
                var identifier = entity.Reference.Identifier;

                // TODO @seb: test and proper handling
                if (!context.IdentifierTypeNameDictionary.ContainsKey(identifier))
                {
                    Console.WriteLine("no query found for identifier: {0}", identifier);
                    return;
                }

                var type = context.IdentifierTypeNameDictionary[identifier];
                var usage = FindOrCreateUsage(type, context);

                try
                {
                    usage.sites.Add(CallSites.CreateReceiverCallSite(entity.MethodName.ToCoReName().Name));
                }
                catch (AssertException e)
                {
                    // TODO test and proper handling
                    Console.WriteLine("error creating ReceiverCallSite:\n{0}", e);
                }
            }
        }

        private static void CreateQuery(QueryContext context,
            string identifier,
            ITypeName type,
            DefinitionSite definition)
        {
            // TODO @seb: test and proper handling
            if (context.IdentifierTypeNameDictionary.ContainsKey(identifier))
            {
                Console.WriteLine("identifier exists in IdentifierTypeNameDictionary: {0}", identifier);
            }
            else
            {
                context.IdentifierTypeNameDictionary.Add(identifier, type);
            }

            // TODO @seb: test and proper handling
            if (context.TypeNameQueryDictionary.ContainsKey(type))
            {
                Console.WriteLine("type exists in TypeNameQueryDictionary: {0}", identifier);
            }
            else
            {
                context.TypeNameQueryDictionary.Add(
                    type,
                    new Query
                    {
                        classCtx = context.EnclosingType.ToCoReName(),
                        definition = definition,
                        methodCtx = context.EnclosingMethod.ToCoReName(),
                        type = type.ToCoReName()
                    });
            }
        }

        private static Query FindOrCreateUsage(ITypeName type, QueryContext context)
        {
            if (context.TypeNameQueryDictionary.ContainsKey(type))
            {
                return context.TypeNameQueryDictionary[type];
            }
            try
            {
                var query = new Query
                {
                    type = type.ToCoReName(),
                    classCtx = context.EnclosingType.ToCoReName(),
                    methodCtx = context.EnclosingMethod.ToCoReName(),
                };
                context.TypeNameQueryDictionary[type] = query;
                return query;
            }
            catch (AssertException e)
            {
                // TODO @seb: test and proper handling
                 return new Query();
            }
        }

        public static IFieldName PropertyToFieldName(IPropertyName property)
        {
            var field = string.Format(
                "[{0}] [{1}]._{2}",
                property.ValueType,
                property.DeclaringType,
                property.Name);
            return FieldName.Get(field);
        }
    }
}