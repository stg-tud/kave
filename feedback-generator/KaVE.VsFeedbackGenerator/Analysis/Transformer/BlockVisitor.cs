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

using System.Collections.Generic;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using KaVE.Model.SSTs;
using KaVE.VsFeedbackGenerator.Analysis.Util;

namespace KaVE.VsFeedbackGenerator.Analysis.Transformer
{
    public class BlockVisitor : TreeNodeVisitor<IList<Statement>>
    {
        private readonly ToNameReducer _toNameReducer;
        private readonly ToExpressionReducer _toExpressionReducer;
        private readonly ToBlockReducer _toBlockReducer;

        public BlockVisitor(UniqueVariableNameGenerator nameGen)
        {
            _toNameReducer = new ToNameReducer(nameGen);
            _toExpressionReducer = new ToExpressionReducer(nameGen);
            _toBlockReducer = new ToBlockReducer(nameGen);
        }
    }
}