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

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SmokeTests
{
    [TestFixture]
    internal class CodeCompletionWithIncompleteReferenceSmokeTest : BaseCSharpCodeCompletionTest
    {
        [Test]
        public void UsingKeyword()
        {
            CompleteInCSharpFile(@"
                usin$
            ");
        }

        [Test]
        public void UsingNamespace()
        {
            CompleteInCSharpFile(@"
                using $
            ");
        }

        [Test]
        public void UsingNamespaceContinuation()
        {
            CompleteInCSharpFile(@"
                using Sys$
            ");
        }

        [Test]
        public void UsingSubNamespace()
        {
            CompleteInCSharpFile(@"
                using Sytem.$
            ");
        }

        [Test]
        public void UsingSubNamespaceContinuation()
        {
            CompleteInCSharpFile(@"
                using Sytem.Coll$
            ");
        }

        [Test]
        public void UsingWithAlias()
        {
            CompleteInCSharpFile(@"
                using MyAlias = $
            ");
        }

        [Test]
        public void ClassAnntation()
        {
            CompleteInCSharpFile(@"
                [$]
                public class C {}
            ");
        }

        [Test]
        public void ClassAnntationContinuation()
        {
            CompleteInCSharpFile(@"
                [Not$]
                public class C {}
            ");
        }

        [Test]
        public void ConstructorDelegationKeyword()
        {
            CompleteInCSharpFile(@"
                class C
                {
                    public C(int i) : th$
                    
                    public C(string s) {}
                }");
        }

        [Test]
        public void ConstructorDelegationDisambiguation()
        {
            CompleteInCSharpFile(@"
                class C
                {
                    public C(int i) : this$
                    
                    public C(string s) {}
                }");
        }

        [Test]
        public void ConstructorDelegationParameter()
        {
            CompleteInCSharpFile(@"
                class C
                {
                    public C(int i) : this($)
                    
                    public C(string s) {}
                }");
        }

        [Test]
        public void MemberAnnotation()
        {
            CompleteInClass(@"
                [$]
                public int _f;");
        }

        [Test]
        public void MemberAnnotationContinuation()
        {
            CompleteInClass(@"
                [Ha$]
                public int _f;");
        }

        [Test]
        public void MemberAccessDirect()
        {
            CompleteInClass(@"
                public void M(object o)
                {
                    o.$
                }
            ");
        }

        [Test]
        public void MemberAccessDirectContinuation()
        {
            CompleteInClass(@"
                public void M(object o)
                {
                    o.ToS$
                }
            ");
        }

        [Test]
        public void MemberAccessUnknownType()
        {
            CompleteInClass(@"
                public void M()
                {
                    o.$
                }
            ");
        }

        [Test]
        public void MemberAccessUnknownTypeContinuation()
        {
            CompleteInClass(@"
                public void M()
                {
                    o.Get$
                }
            ");
        }

        [Test]
        public void MemberAccessInCast()
        {
            CompleteInClass(@"
                public void M(object o)
                {
                    var u = (int) o.$
                }
            ");
        }

        [Test]
        public void MemberAccessInCastContinuation()
        {
            CompleteInClass(@"
                public void M(object o)
                {
                    var u = (int) o.GetH$
                }
            ");
        }

        [Test]
        public void CastSource()
        {
            CompleteInClass(@"
                public void M()
                {
                    var u = (int) $
                }
            ");
        }

        [Test]
        public void MemberAccessOnReturnValue()
        {
            CompleteInClass(@"
                public void M()
                {
                    GetHashCode().$
                }
            ");
        }

        [Test]
        public void MemberAccessOnReturnValueContinuation()
        {
            CompleteInClass(@"
                public void M()
                {
                    GetHashCode().GetH$
                }
            ");
        }

        [Test]
        public void MemberAccessOnField()
        {
            CompleteInClass(@"
                private object _f;

                public void M()
                {
                    _f.$
                }
            ");
        }

        [Test]
        public void MemberAccessOnFieldContinuation()
        {
            CompleteInClass(@"
                private object _f;

                public void M()
                {
                    _f.Eq$
                }
            ");
        }

        [Test]
        public void MissingReturn()
        {
            CompleteInClass(@"
                public int M()
                {
                    $
                }
            ");
        }

        [Test]
        public void MissingArgument()
        {
            CompleteInMethod(@"
                this.Equals($);
            ");
        }

        [Test]
        public void MissingArgumentContinuation()
        {
            CompleteInMethod(@"
                this.Equals(th$);
            ");
        }
    }
}