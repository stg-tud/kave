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

using System;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Tests.ExternalSerializationTests
{
    public class TestCase
    {
        [NotNull]
        public readonly string Name;

        [NotNull]
        public readonly Type SerializedType;

        [NotNull]
        public readonly string Input;

        [NotNull]
        public readonly string ExpectedCompact;

        [CanBeNull]
        public readonly string ExpectedFormatted;

        public TestCase([NotNull] string name,
            [NotNull] Type serializedType,
            [NotNull] string input,
            [NotNull] string expectedCompact,
            [CanBeNull] string expectedFormatted)
        {
            Name = name;
            SerializedType = serializedType;
            Input = input;
            ExpectedCompact = expectedCompact;
            ExpectedFormatted = expectedFormatted;
        }

        public override string ToString()
        {
            return string.Format("[TestCase: {0}]", Name);
        }
    }
}