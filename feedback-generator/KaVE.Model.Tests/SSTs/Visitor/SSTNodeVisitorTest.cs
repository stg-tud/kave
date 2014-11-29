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

using KaVE.Model.SSTs;
using KaVE.Model.SSTs.Declarations;
using KaVE.Model.SSTs.Visitor;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.Visitor
{
    internal class SSTNodeVisitorTest
    {
        [Test]
        public void AllVisitsAreImplemented()
        {
            var sut = new SSTNodeVisitor<object>();
            sut.Visit((SST) null, null);

            sut.Visit((DelegateDeclaration) null, null);
            sut.Visit((EventDeclaration) null, null);
            sut.Visit((FieldDeclaration) null, null);
            sut.Visit((MethodDeclaration) null, null);
            sut.Visit((PropertyDeclaration) null, null);

            sut.Visit((Statement) null, null);
            sut.Visit((Expression) null, null);
        }
    }
}