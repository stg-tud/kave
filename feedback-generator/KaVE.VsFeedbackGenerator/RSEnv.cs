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
 *    - Uli Fahrer
 */

using System.Collections.Generic;
using JetBrains.Application;
using JetBrains.Application.Extensions;
using JetBrains.DataFlow;
using JetBrains.Util;
using KaVE.JetBrains.Annotations;
using KaVE.VsFeedbackGenerator.VsIntegration;

namespace KaVE.VsFeedbackGenerator
{
    public interface IRSEnv
    {
        IExtension KaVEExtension { get; }

        IIDESession IDESession { get; }
    }

    [ShellComponent]
    internal class RSEnv : IRSEnv
    {
        public const string ExtensionId = "KaVE.VsFeedbackGenerator";

        private readonly ExtensionManager _extensionManager;
        private readonly IIDESession _ideSession;

        public RSEnv(ExtensionManager extensionManager, IIDESession ideSession)
        {
            _extensionManager = extensionManager;
            _ideSession = ideSession;
        }

        [NotNull]
        public IExtension KaVEExtension
        {
            get
            {
                var extension = _extensionManager.GetExtension(ExtensionId);
                return extension ?? new DevExtension();
            }
        }

        [NotNull]
        public IIDESession IDESession
        {
            get { return _ideSession; }
        }

        private class DevExtension : IExtension
        {
            public void PutData<T>(Key<T> key, T val) where T : class
            {
                throw new System.NotImplementedException();
            }

            public T GetData<T>(Key<T> key) where T : class
            {
                throw new System.NotImplementedException();
            }

            public IEnumerable<KeyValuePair<object, object>> EnumerateData()
            {
                throw new System.NotImplementedException();
            }

            public IEnumerable<FileSystemPath> GetFiles(string fileType)
            {
                throw new System.NotImplementedException();
            }

            public string Id
            {
                get { return ExtensionId; }
            }

            public SemanticVersion Version
            {
                get { return SemanticVersion.Parse("1.0-dev"); }
            }

            public IExtensionMetadata Metadata
            {
                get { throw new System.NotImplementedException(); }
            }

            public bool Supported
            {
                get { throw new System.NotImplementedException(); }
            }

            public IProperty<bool?> Enabled
            {
                get { throw new System.NotImplementedException(); }
            }

            public ListEvents<ExtensionRecord> RuntimeInfoRecords
            {
                get { throw new System.NotImplementedException(); }
            }

            public IExtensionProvider Source
            {
                get { throw new System.NotImplementedException(); }
            }
        }
    }
}