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

using JetBrains.Application;
using KaVE.Commons.Utils.CodeCompletion.Impl;
using KaVE.Commons.Utils.IO;

namespace KaVE.VsFeedbackGenerator.CodeCompletion
{
    [ShellComponent]
    public class InjectablePBNRecommenderStore : SmilePBNRecommenderStore
    {
        public InjectablePBNRecommenderStore(IIoUtils io) : base(@"c:\kave-models\", io) {}
    }
}