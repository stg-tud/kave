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
 *    - Sebastian Proksch
 */

using KaVE.Model.SSTs.Declarations;
using KaVE.Model.SSTs.Impl;
using KaVE.Model.SSTs.Impl.Declarations;
using KaVE.Model.SSTs.Impl.Visitor;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.Visitor
{
    internal class SSTNodeVisitorTest
    {
        [Test]
        public void AllVisitsAreImplemented()
        {
            var sut = new AbstractNodeVisitor<object>();
            sut.Visit((SST) null, null);

            sut.Visit((DelegateDeclaration) null, null);
            sut.Visit((IEventDeclaration) null, null);
            sut.Visit((IFieldDeclaration) null, null);
            sut.Visit((IMethodDeclaration) null, null);
            sut.Visit((IPropertyDeclaration) null, null);


            // TODO complete after hierarchy is stable
        }
    }
}