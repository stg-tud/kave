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
using System.Drawing;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure.LookupItems;
using JetBrains.UI.Icons;
using JetBrains.UI.Icons.ColorIcons;
using JetBrains.UI.Icons.ComposedIcons;
using JetBrains.UI.RichText;

namespace KaVE.VS.FeedbackGenerator.CodeCompletion
{
    internal class PBNProposalWrappedLookupItem : AbstractWrappedLookupItem
    {
        private readonly int _probability;

        public PBNProposalWrappedLookupItem(ILookupItem wrappedItem, double probability)
        {
            Item = wrappedItem;
            _probability = (int) (probability*100);
        }

        public override LookupItemPlacement Placement
        {
            get
            {
                var p = new LookupItemPlacement(
                    RevisedOrderString,
                    PlacementLocation.Top,
                    SelectionPriority.High)
                {
                    Relevance = long.MaxValue,
                    Rank = Item.Placement.Rank
                };
                return p;
            }
            set { throw new NotImplementedException(); }
        }

        private string RevisedOrderString
        {
            get
            {
                return string.Format(
                    "{0:000}_{1}",
                    100 - _probability,
                    Item.Placement.OrderString);
            }
        }

        public override IconId Image
        {
            get { return CompositeIconId.Compose(new ColorIconId(Color.Gold), Item.Image); }
        }

        public override RichText DisplayName
        {
            get
            {
                var original = Item.DisplayName.Clone();
                original.SetStyle(FontStyle.Bold);
                var probStyle = new TextStyle {ForegroundColor = Color.Gray};
                return original.Append(" (" + _probability + "%)", probStyle);
            }
        }
    }
}