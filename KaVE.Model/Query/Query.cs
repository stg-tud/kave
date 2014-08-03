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
 *    - Dennis Albrecht
 */

using System.Collections.Generic;

namespace KaVE.Model.Query
{
    // ReSharper disable InconsistentNaming
    public class Query
    {
        public CoReTypeName type { get; set; }
        public DefinitionSite definition { get; set; }
        public CoReTypeName classCtx { get; set; }
        public CoReMethodName methodCtx { get; set; }
        public IList<CallSite> sites { get; set; }
    }

    public class DefinitionSite
    {
        public DefinitionKind kind { get; set; }
        public CoReTypeName type { get; set; }
        public CoReMethodName method { get; set; }
        public CoReFieldName field { get; set; }
        public int arg { get; set; }
    }

    public enum DefinitionKind
    {
        THIS,
        RETURN,
        NEW,
        PARAM,
        FIELD,
        CONSTANT,
        UNKNOWN,
    }

    public class CallSite
    {
        public CallSiteKind kind { get; set; }
        public CoReMethodName call { get; set; }
        public int argumentIndex { get; set; }
    }

    public enum CallSiteKind
    {
        RECEIVER_CALL_SITE,
        PARAM_CALL_SITE,
    }

    // ReSharper restore InconsistentNaming

    public abstract class CoReName
    {
        protected CoReName(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }

    public class CoReTypeName : CoReName {
        public CoReTypeName(string name) : base(name) {}
    }

    public class CoReMethodName : CoReName {
        public CoReMethodName(string name) : base(name) {}
    }

    public class CoReFieldName : CoReName {
        public CoReFieldName(string name) : base(name) {}
    }
}