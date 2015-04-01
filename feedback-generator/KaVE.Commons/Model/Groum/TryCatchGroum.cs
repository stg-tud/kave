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
namespace KaVE.Commons.Model.Groum
{
    public class TryCatchGroum : GroumBase
    {
        public TryCatchGroum(GroumBase tryGroum, GroumBase finallyGroum, params GroumBase[] catchGroums)
        {
            TryGroum = tryGroum;
            CatchGroums = new ParallelGroum(catchGroums);
            FinallyGroum = finallyGroum;
        }

        public GroumBase TryGroum { get; private set; }
        public ParallelGroum CatchGroums { get; private set; }
        public GroumBase FinallyGroum { get; private set; }
    }
}