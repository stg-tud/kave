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
using KaVE.Utils;

namespace KaVE.CompletionTraceGenerator.Model
{
    public class CompletionAction
    {
        public ActionType Type { get; private set; }
        public Direction? Direction { get; private set; }
        public int? Index { get; private set; }
        public string Token { get; private set; }

        private CompletionAction() {}

        protected bool Equals(CompletionAction other)
        {
            return Type == other.Type && Direction == other.Direction && Index == other.Index &&
                   string.Equals(Token, other.Token);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override string ToString()
        {
            return string.Format("[Type: {0}, Direction: {1}, Index: {2}, Token: {3}]", Type, Direction, Index, Token);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) Type;
                hashCode = (hashCode*397) ^ Direction.GetHashCode();
                hashCode = (hashCode*397) ^ Index.GetHashCode();
                hashCode = (hashCode*397) ^ (Token != null ? Token.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static CompletionAction NewCancel()
        {
            return new CompletionAction {Type = ActionType.Cancel};
        }

        public static CompletionAction NewApply()
        {
            return new CompletionAction {Type = ActionType.Apply};
        }

        public static CompletionAction NewFilter(string token)
        {
            return new CompletionAction {Type = ActionType.Filter, Token = token};
        }

        public static CompletionAction NewMouseGoto(int currentIndex)
        {
            return new CompletionAction {Type = ActionType.MouseGoto, Index = currentIndex};
        }

        public static CompletionAction NewScroll(int startIndex)
        {
            return new CompletionAction {Type = ActionType.Scroll, Index = startIndex};
        }

        public static CompletionAction NewPageStep(Direction direction)
        {
            return new CompletionAction {Type = ActionType.PageStep, Direction = direction};
        }

        public static CompletionAction NewStep(Direction direction)
        {
            return new CompletionAction {Type = ActionType.Step, Direction = direction};
        }
    }
}