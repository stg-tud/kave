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
 */

using NUnit.Framework;

namespace KaVE.ReSharper.Commons.Tests_Integration.Analysis.SmokeTests
{
    [TestFixture]
    internal class CodeCompletionInIncompleteDeclarationsSmokeTest : BaseCSharpCodeCompletionTest
    {
        [Test]
        public void NamespaceDeclarationKeyword()
        {
            CompleteInCSharpFile(@"
                names$
            ");
        }

        [Test]
        public void NamespaceDeclarationName()
        {
            CompleteInCSharpFile(@"
                namespace $
            ");
        }

        [Test]
        public void NamespaceDeclarationNameContinuation()
        {
            CompleteInCSharpFile(@"
                namespace N$
            ");
        }

        [Test]
        public void NamespaceDeclarationSubNamespaceName()
        {
            CompleteInCSharpFile(@"
                namespace N.$
            ");
        }

        [Test]
        public void NamespaceDeclarationSubNamespaceNameContinuation()
        {
            CompleteInCSharpFile(@"
                namespace N.M$
            ");
        }

        [Test]
        public void TypeDeclarationKeyword()
        {
            CompleteInCSharpFile(@"
                cla$
            ");
        }

        [Test]
        public void TypeDeclarationName()
        {
            CompleteInCSharpFile(@"
                class $
            ");
        }

        [Test]
        public void ClassDeclarationNameContinuation()
        {
            CompleteInCSharpFile(@"
                class Foo$
            ");
        }

        [Test]
        public void MemberDeclarationOnlyName()
        {
            CompleteInClass(@"
                ToStr$
            ");
        }

        [Test]
        public void MemberDeclarationModifier()
        {
            CompleteInClass(@"
                pu$
            ");
        }

        [Test]
        public void MemberDeclarationValueType()
        {
            CompleteInClass(@"
                public lo$
            ");
        }

        [Test]
        public void MemberDeclarationName()
        {
            CompleteInClass(@"
                public void $
            ");
        }

        [Test]
        public void MemberDeclarationAfterName()
        {
            CompleteInClass(@"
                public void M$
            ");
        }

        [Test]
        public void MethodDeclarationParameterType()
        {
            CompleteInClass(@"
                public void M(in$)
            ");
        }

        [Test]
        public void MethodDeclarationParameterName()
        {
            CompleteInClass(@"
                public void M(int $)
            ");
        }

        [Test]
        public void MethodDeclarationParameterNew()
        {
            CompleteInClass(@"
                public void M(int i, $)
            ");
        }

        [Test]
        public void MethodDeclarationAfterSignature()
        {
            CompleteInClass(@"
                public void M()$
            ");
        }

        [Test]
        public void MethodDeclarationDuplication()
        {
            CompleteInClass(@"
                public void M() {}
                public void M()
                {
                    $
                }");
        }

        [Test]
        public void MethodDeclarationDuplicationOverloadCreation()
        {
            CompleteInClass(@"
                public void M() {}
                public void M($) {}");
        }
    }
}