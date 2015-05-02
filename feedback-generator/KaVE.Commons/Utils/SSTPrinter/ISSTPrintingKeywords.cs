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
    public interface ISSTPrintingKeywords
    {
        string IndentationToken { get; }
        string Class { get; }
        string Interface { get; }
        string Enum { get; }
        string Struct { get; }
        string Delegate { get; }
        string Event { get; }
        string PropertyGet { get; }
        string PropertySet { get; }
        string BreakStatement { get; }
        string ContinueStatement { get; }
        string Goto { get; }
        string Return { get; }
        string Throw { get; }
        string New { get; }
        string Foreach { get; }
        string ForeachIn { get; }
        string For { get; }
        string If { get; }
        string Else { get; }
        string Lock { get; }
        string Switch { get; }
        string SwitchCase { get; }
        string SwitchDefault { get; }
        string Try { get; }
        string TryCatch { get; }
        string TryFinally { get; }
        string Unchecked { get; }
        string UnsafeBlock { get; }
        string Using { get; }
        string While { get; }
        string Do { get; }
        string CursorPosition { get; }
        string ComposedExpression { get; }
        string UnknownEntity { get; }
        string StaticModifier { get; }
        string RefModifier { get; }
        string OutModifier { get; }
        string OptModifier { get; }
        string ParamsModifier { get; }
    }
}