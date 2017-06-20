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

using EnvDTE;
using JetBrains.Application;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.JetBrains.Annotations;

namespace KaVE.VS.FeedbackGenerator.Generators.VisualStudio.EditEventGenerators.EventContext
{
    [ShellComponent]
    public class ContextProvider : IContextProvider
    {
        private ContextGenerator _contextGenerator;

        public ContextProvider()
        {
            ContextGenerator.ContextGeneratorCreated += generator => _contextGenerator = generator;
        }

        public Context GetCurrentContext(Document document)
        {
            return _contextGenerator != null ? _contextGenerator.GetCurrentContext(document) : Context.Default;
        }

        public Context GetCurrentContext(TextPoint startPoint)
        {
            return _contextGenerator != null ? _contextGenerator.GetCurrentContext(startPoint) : Context.Default;
        }
    }

    public interface IContextProvider
    {
        Context GetCurrentContext([NotNull] Document document);
        Context GetCurrentContext([NotNull] TextPoint startPoint);
    }
}