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

using System.Globalization;
using KaVE.Commons.Model.Names.VisualStudio;
using KaVE.Commons.Utils;

namespace KaVE.FeedbackProcessor.Cleanup.Heuristics
{
    internal static class TestVsProductionDocumentHeuristic
    {
        public static bool IsTestDocument(this DocumentName document)
        {
            return NameEndsWithTest(document);
        }

        private static bool NameEndsWithTest(this DocumentName document)
        {
            var filename = document.FileName;
            if (filename != null)
            {
                return filename.Contains("test", CompareOptions.IgnoreCase);
            }
            return false;
        }

        public static bool IsProductionDocument(this DocumentName document)
        {
            return !IsTestDocument(document);
        }
    }
}