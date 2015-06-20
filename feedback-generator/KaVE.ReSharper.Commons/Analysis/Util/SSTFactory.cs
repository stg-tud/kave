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

using KaVE.ReSharper.Commons.Analysis.Transformer;

namespace KaVE.ReSharper.Commons.Analysis.Util
{
    public interface ISSTFactory
    {
        /// <summary>
        ///     ArgumentCollector are used for the arguments of a invocation.
        ///     For every argument a reference is created (if the argument isn't a reference itself).
        /// </summary>
        SSTArgumentCollector ArgumentCollector();

        /// <summary>
        ///     ReferenceCollector are used for the elements of a primitive operation (like arithmethic operations).
        ///     Every constant value is ignored.
        /// </summary>
        SSTReferenceCollector ReferenceCollector();

        /// <summary>
        ///     AssignmentGenerator analyses the right-hand side of a assignment to determine die assignment's source.
        /// </summary>
        SSTAssignmentGenerator AssignmentGenerator();

        /// <summary>
        ///     ScopeTransformer analyses statements and blocks of statements.
        ///     Such scopes can be a whole method or the body of a loop etc.
        /// </summary>
        SSTScopeTransformer ScopeTransformer();

        IScope Scope();
        IUniqueVariableNameGenerator TempVariableGenerator();
    }
}