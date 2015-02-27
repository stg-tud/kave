﻿/*
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

using KaVE.Model.Names;
using KaVE.Model.SSTs.References;
using KaVE.Model.SSTs.Visitor;
using KaVE.Utils;

namespace KaVE.Model.SSTs.Impl.References
{
    public class EventReference : IEventReference
    {
        public IVariableReference Reference { get; set; }
        public IEventName EventName { get; set; }

        public void Accept<TContext>(ISSTNodeVisitor<TContext> visitor, TContext context)
        {
            visitor.Visit(this, context);
        }

        protected bool Equals(EventReference other)
        {
            return Equals(EventName, other.EventName);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            var hcEventName = EventName != null ? EventName.GetHashCode() : 0;
            return 2387645 + hcEventName;
        }
    }
}