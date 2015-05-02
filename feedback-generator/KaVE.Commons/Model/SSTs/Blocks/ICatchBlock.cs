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

using KaVE.Commons.Model.Names;
using KaVE.Commons.Utils.Collections;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.SSTs.Blocks
{
    public interface ICatchBlock
    {
        [NotNull]
        IParameterName Parameter { get; }

        [NotNull]
        IKaVEList<IStatement> Body { get; }

        /// <summary>
        ///     catch block does not define a  specific exception type and catches everything.
        ///     E.g., try { ... } catch { ... }
        /// </summary>
        bool IsGeneral { get; }

        /// <summary>
        ///     catch block defines a type, but does not introduce a name for the caught exception.
        ///     E.g., try { ... } catch(Exception) { ... }
        /// </summary>
        bool IsUnnamed { get; }
    }
}