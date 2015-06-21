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
 *    - Dennis Albrecht
 *    - Sebastian Proksch
 */

using System.Drawing;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.CodeCompletion;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure.LookupItems;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure.Match;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.TextControl;
using JetBrains.UI.Icons;
using JetBrains.UI.Icons.ColorIcons;
using JetBrains.UI.Icons.ComposedIcons;
using JetBrains.UI.RichText;
using JetBrains.Util;

namespace KaVE.VS.FeedbackGenerator.CodeCompletion
{
    internal class PBNProposalWrappedLookupItem : IWrappedLookupItem
    {
        private readonly ILookupItem _wrappedItem;

        private readonly int _probability;

        public PBNProposalWrappedLookupItem(ILookupItem wrappedItem, double probability)
        {
            _wrappedItem = wrappedItem;
            _probability = (int) (probability*100);
        }

        public RichText DisplayName
        {
            get
            {
                var original = _wrappedItem.DisplayName.Clone();
                original.SetStyle(FontStyle.Bold);
                var probStyle = new TextStyle {ForegroundColor = Color.Gray};
                return original.Append(" (" + _probability + "%)", probStyle);
            }
        }

        // TODO RS9: fix for stable release
        /*
        public string OrderingString
        {
            get
            {
                var inverseProbability = 100 - _probability;
                var orderingPrefix = "0";
                if (inverseProbability < 100)
                {
                    orderingPrefix = "00";
                }
                else if (inverseProbability < 10)
                {
                    orderingPrefix = "000";
                }
                return orderingPrefix + inverseProbability + _wrappedItem.OrderingString;
            }
        }
        */

        public IconId Image
        {
            get { return CompositeIconId.Compose(new ColorIconId(Color.Gold), _wrappedItem.Image); }
        }

        public bool AcceptIfOnlyMatched(LookupItemAcceptanceContext itemAcceptanceContext)
        {
            return _wrappedItem.AcceptIfOnlyMatched(itemAcceptanceContext);
        }

        public MatchingResult Match(PrefixMatcher prefixMatcher, ITextControl textControl)
        {
            return _wrappedItem.Match(prefixMatcher, textControl);
        }

        public void Accept(ITextControl textControl,
            TextRange nameRange,
            LookupItemInsertType lookupItemInsertType,
            Suffix suffix,
            ISolution solution,
            bool keepCaretStill)
        {
            _wrappedItem.Accept(textControl, nameRange, lookupItemInsertType, suffix, solution, keepCaretStill);
        }

        public TextRange GetVisualReplaceRange(ITextControl textControl, TextRange nameRange)
        {
            return _wrappedItem.GetVisualReplaceRange(textControl, nameRange);
        }

        public bool Shrink()
        {
            return _wrappedItem.Shrink();
        }

        public void Unshrink()
        {
            _wrappedItem.Unshrink();
        }

        public LookupItemPlacement Placement { get; set; }

        public RichText DisplayTypeName
        {
            get { return _wrappedItem.DisplayTypeName; }
        }

        public bool CanShrink
        {
            get { return _wrappedItem.CanShrink; }
        }

        public int Multiplier
        {
            get { return _wrappedItem.Multiplier; }
            set { _wrappedItem.Multiplier = value; }
        }

        public EvaluationMode Mode { get; set; }

        public bool IsDynamic
        {
            get { return _wrappedItem.IsDynamic; }
        }

        public bool IgnoreSoftOnSpace
        {
            get { return _wrappedItem.IgnoreSoftOnSpace; }
            set { _wrappedItem.IgnoreSoftOnSpace = value; }
        }

        public bool IsStable { get; set; }

        public string Identity
        {
            get { return _wrappedItem.Identity; }
        }

        public ILookupItem Item
        {
            get { return _wrappedItem; }
        }
    }
}