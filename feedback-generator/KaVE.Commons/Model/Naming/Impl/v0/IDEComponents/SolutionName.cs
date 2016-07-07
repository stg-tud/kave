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
 */

using KaVE.Commons.Model.Naming.IDEComponents;
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Model.Naming.Impl.v0.IDEComponents
{
    public class SolutionName : Name, ISolutionName
    {
        private static readonly WeakNameCache<ISolutionName> Registry =
            WeakNameCache<ISolutionName>.Get(id => new SolutionName(id));

        public new static ISolutionName Get(string identifier)
        {
            return Registry.GetOrCreate(identifier);
        }

        private SolutionName(string identifier)
            : base(identifier) {}
    }
}