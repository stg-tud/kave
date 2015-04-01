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
 *    - Uli Fahrer
 */

namespace KaVE.Commons.Model.ObjectUsage
{
    public class DefinitionSites
    {
        public static DefinitionSite CreateDefinitionByReturn(string methodName)
        {
            var definitionSite = new DefinitionSite
            {
                kind = DefinitionSiteKind.RETURN,
                method = new CoReMethodName(methodName)
            };

            return definitionSite;
        }

        public static DefinitionSite CreateDefinitionByField(string field)
        {
            var definitionSite = new DefinitionSite
            {
                kind = DefinitionSiteKind.FIELD,
                field = new CoReFieldName(field)
            };

            return definitionSite;
        }

        public static DefinitionSite CreateDefinitionByConstructor(string constructor)
        {
            var definitionSite = new DefinitionSite
            {
                kind = DefinitionSiteKind.NEW,
                method = new CoReMethodName(constructor)
            };

            return definitionSite;
        }

        public static DefinitionSite CreateDefinitionByParam(string methodName, int argIndex)
        {
            var definitionSite = new DefinitionSite
            {
                kind = DefinitionSiteKind.PARAM,
                method = new CoReMethodName(methodName),
                argIndex = argIndex
            };

            return definitionSite;
        }

        public static DefinitionSite CreateUnknownDefinitionSite()
        {
            var definitionSite = new DefinitionSite
            {
                kind = DefinitionSiteKind.UNKNOWN,
            };

            return definitionSite;
        }
    }
}