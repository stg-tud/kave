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
    public class ForGroum : GroumBase
    {
        public ForGroum(GroumBase initializerGroum, GroumBase conditionGroum, GroumBase bodyGroum, GroumBase updaterGroum)
        {
            InitializerGroum = initializerGroum;
            ConditionGroum = conditionGroum;
            BodyGroum = bodyGroum;
            UpdaterGroum = updaterGroum;
        }

        public GroumBase InitializerGroum { get; private set; }
        public GroumBase ConditionGroum { get; private set; }
        public GroumBase BodyGroum { get; private set; }
        public GroumBase UpdaterGroum { get; private set; }
    }
}