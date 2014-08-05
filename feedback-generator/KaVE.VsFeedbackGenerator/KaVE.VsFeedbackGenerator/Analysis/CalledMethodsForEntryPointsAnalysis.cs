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
 *    - Sven Amann
 *    - Sebastian Proksch
 */

using System;
using System.Collections.Generic;
using System.Linq;
using KaVE.Model.Collections;
using KaVE.Model.Names;
using NuGet;

namespace KaVE.VsFeedbackGenerator.Analysis
{
    internal class CalledMethodsForEntryPointsAnalysis
    {
        private ISet<MethodRef> _entryPoints;

        public IDictionary<IMethodName, ISet<IMethodName>> Analyze(ISet<MethodRef> entryPoints)
        {
            _entryPoints = entryPoints;
            var result = new Dictionary<IMethodName, ISet<IMethodName>>();

            foreach (var ep in _entryPoints)
            {
                var called = Sets.NewHashSet<IMethodName>();
                var analyzed = Sets.NewHashSet<IMethodName>();
                analyzed.AddRange(_entryPoints.Select(e => e.Name));

                Analyze(ep, called, analyzed);
                try
                {
                    result.Add(ep.Name, called);
                }
                catch (ArgumentException)
                {
                    // in case of duplicate method declaration
                }
            }

            return result;
        }

        private void Analyze(MethodRef ep, ISet<IMethodName> called, ISet<IMethodName> analyzed)
        {
            analyzed.Add(ep.Name);

            var calls = ReSharperUtils.FindMethodsInvokedIn(ep);
            foreach (var c in calls)
            {
                if (c.IsAssemblyReference)
                {
                    AddCall(c, called);
                }
                else
                {
                    var isLocal = ep.Name.DeclaringType == c.Name.DeclaringType;
                    var isAbstract = c.Declaration.IsAbstract;
                    var isLocalAbstract = isLocal && isAbstract;

                    if (IsEntrypoint(c) || isLocalAbstract || !isLocal)
                    {
                        AddCall(c, called);
                    }
                    else
                    {
                        var isNotAnalyed = !analyzed.Contains(c.Name);
                        if (isNotAnalyed)
                        {
                            Analyze(c, called, analyzed);
                        }
                    }
                }
            }
        }

        private static void AddCall(MethodRef methodRef, ISet<IMethodName> called)
        {
            var firstDecl = methodRef.GetFirstDeclaration();
            called.Add(firstDecl);
        }

        private bool IsEntrypoint(MethodRef c)
        {
            return _entryPoints.Select(e => e.Name).Contains(c.Name);
        }
    }
}