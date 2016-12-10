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

using System.Linq;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.TypeShapes;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.Naming;

namespace KaVE.VS.FeedbackGenerator.SessionManager.Anonymize.CompletionEvents
{
    public class TypeShapeAnonymizer
    {
        public ITypeShape Anonymize(ITypeShape typeShape)
        {
            return new TypeShape
            {
                TypeHierarchy = AnonymizeCodeNames(typeShape.TypeHierarchy),
                NestedTypes = Sets.NewHashSetFrom(typeShape.NestedTypes.Select(AnonymizeCodeNames)),
                Delegates = Sets.NewHashSetFrom(typeShape.Delegates.Select(n => n.ToAnonymousName())),
                EventHierarchies = Sets.NewHashSetFrom(typeShape.EventHierarchies.Select(AnonymizeCodeNames)),
                Fields = Sets.NewHashSetFrom(typeShape.Fields.Select(n => n.ToAnonymousName())),
                MethodHierarchies = Sets.NewHashSetFrom(typeShape.MethodHierarchies.Select(AnonymizeCodeNames)),
                PropertyHierarchies = Sets.NewHashSetFrom(typeShape.PropertyHierarchies.Select(AnonymizeCodeNames))
            };
        }

        private static ITypeHierarchy AnonymizeCodeNames(ITypeHierarchy raw)
        {
            if (raw == null)
            {
                return null;
            }

            return new TypeHierarchy
            {
                Element = raw.Element.ToAnonymousName(),
                Extends = AnonymizeCodeNames(raw.Extends),
                Implements = Sets.NewHashSetFrom(raw.Implements.Select(AnonymizeCodeNames))
            };
        }

        private static IMemberHierarchy<IEventName> AnonymizeCodeNames(IMemberHierarchy<IEventName> raw)
        {
            return new EventHierarchy
            {
                Element = raw.Element.ToAnonymousName(),
                Super = raw.Super.ToAnonymousName(),
                First = raw.First.ToAnonymousName()
            };
        }

        private static IMemberHierarchy<IMethodName> AnonymizeCodeNames(IMemberHierarchy<IMethodName> raw)
        {
            return new MethodHierarchy
            {
                Element = raw.Element.ToAnonymousName(),
                Super = raw.Super.ToAnonymousName(),
                First = raw.First.ToAnonymousName()
            };
        }

        private static IMemberHierarchy<IPropertyName> AnonymizeCodeNames(IMemberHierarchy<IPropertyName> raw)
        {
            return new PropertyHierarchy
            {
                Element = raw.Element.ToAnonymousName(),
                Super = raw.Super.ToAnonymousName(),
                First = raw.First.ToAnonymousName()
            };
        }
    }
}