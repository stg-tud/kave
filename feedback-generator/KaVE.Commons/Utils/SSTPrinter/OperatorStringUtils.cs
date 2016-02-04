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

using System.Collections.Generic;
using KaVE.Commons.Model.SSTs.Expressions.Assignable;

namespace KaVE.Commons.Utils.SSTPrinter
{
    internal static class OperatorStringUtils
    {
        private static readonly IDictionary<UnaryOperator, string> UnaryOperatorToStringMap = new Dictionary
            <UnaryOperator, string>
        {
            {UnaryOperator.Unknown, "?"},
            {UnaryOperator.Complement, "~"},
            {UnaryOperator.Minus, "-"},
            {UnaryOperator.Plus, "+"},
            {UnaryOperator.Not, "!"},
            {UnaryOperator.PostDecrement, "--"},
            {UnaryOperator.PreDecrement, "--"},
            {UnaryOperator.PostIncrement, "++"},
            {UnaryOperator.PreIncrement, "++"}
        };

        private static readonly IDictionary<BinaryOperator, string> BinaryOperatorToStringMap = new Dictionary
            <BinaryOperator, string>
        {
            {BinaryOperator.And, "&&"},
            {BinaryOperator.BitwiseAnd, "&"},
            {BinaryOperator.BitwiseOr, "|"},
            {BinaryOperator.BitwiseXor, "^"},
            {BinaryOperator.Divide, "/"},
            {BinaryOperator.Equal, "=="},
            {BinaryOperator.GreaterThan, ">"},
            {BinaryOperator.GreaterThanOrEqual, ">="},
            {BinaryOperator.LessThan, "<"},
            {BinaryOperator.LessThanOrEqual, "<="},
            {BinaryOperator.Minus, "-"},
            {BinaryOperator.Modulo, "%"},
            {BinaryOperator.Multiply, "*"},
            {BinaryOperator.NotEqual, "!="},
            {BinaryOperator.Or, "||"},
            {BinaryOperator.Plus, "+"},
            {BinaryOperator.ShiftLeft, "<<"},
            {BinaryOperator.ShiftRight, ">>"},
            {BinaryOperator.Unknown, "?"}
        };

        public static string ToPrettyString(this UnaryOperator @operator)
        {
            if (UnaryOperatorToStringMap.ContainsKey(@operator))
            {
                return UnaryOperatorToStringMap[@operator];
            }

            return @operator.ToString();
        }

        public static string ToPrettyString(this BinaryOperator @operator)
        {
            if (BinaryOperatorToStringMap.ContainsKey(@operator))
            {
                return BinaryOperatorToStringMap[@operator];
            }

            return @operator.ToString();
        }
    }
}