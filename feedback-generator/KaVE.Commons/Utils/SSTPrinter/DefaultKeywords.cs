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
 *    - Andreas Bauer
 */

namespace KaVE.Commons.Utils.SSTPrinter
{
    public class DefaultKeywords : ISSTPrintingKeywords
    {
        public virtual string IndentationToken { get { return "    "; } }
        public virtual string Class { get { return "class"; } }
        public virtual string Interface { get { return "interface"; } }
        public virtual string Enum { get { return "enum"; } }
        public virtual string Struct { get { return "struct"; } }
        public virtual string Delegate { get { return "delegate"; } }
        public virtual string Event { get { return "event"; } }
        public virtual string PropertyGet { get { return "get"; } }
        public virtual string PropertySet { get { return "set"; } }
        public virtual string BreakStatement { get { return "break"; } }
        public virtual string ContinueStatement { get { return "continue"; } }
        public virtual string Goto { get { return "goto"; } }
        public virtual string Return { get { return "return"; } }
        public virtual string Throw { get { return "throw"; } }
        public virtual string New { get { return "new"; } }
        public virtual string Foreach { get { return "foreach"; } }
        public virtual string ForeachIn { get { return "in"; } }
        public virtual string For { get { return "for"; } }
        public virtual string If { get { return "if"; } }
        public virtual string Else { get { return "else"; } }
        public virtual string Lock { get { return "lock"; } }
        public virtual string Switch { get { return "switch"; } }
        public virtual string SwitchCase { get { return "case"; } }
        public virtual string SwitchDefault { get { return "default"; } }
        public virtual string Try { get { return "try"; } }
        public virtual string TryCatch { get { return "catch"; } }
        public virtual string TryFinally { get { return "finally"; } }
        public virtual string Unchecked { get { return "unchecked"; } }
        public virtual string UnsafeBlock { get { return "unsafe { /* content ignored */ }"; } }
        public virtual string Using { get { return "using"; } }
        public virtual string While { get { return "while"; } }
        public virtual string Do { get { return "do"; } }
        public virtual string CursorPosition { get { return "$"; } }
        public virtual string ComposedExpression { get { return "composed"; } }
        public virtual string UnknownEntity { get { return "???"; } }
        public virtual string StaticModifier { get { return "static"; } }
        public virtual string RefModifier { get { return "ref"; } }
        public virtual string OutModifier { get { return "out"; } }
        public virtual string OptModifier { get { return "opt"; } }
        public virtual string ParamsModifier { get { return "params"; } }
    }
}