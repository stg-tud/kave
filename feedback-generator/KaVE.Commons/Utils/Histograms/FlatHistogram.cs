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
using System.Linq;
using System.Text;
using KaVE.Commons.Utils.Assertion;

namespace KaVE.Commons.Utils.Histograms
{
    public class FlatHistogram
    {
        private readonly double[] _bins;

        public FlatHistogram(int numBins)
        {
            _bins = new double[numBins];
        }

        // bin is zero based, maxBin is arraysize
        public void Add(int binOneBased, int maxBin)
        {
            Asserts.That(binOneBased > 0);
            Asserts.That(binOneBased <= maxBin);

            var bin = binOneBased - 1;
            var length = 1/(double) maxBin;
            var start = bin*length;
            var end = (bin + 1)*length;

            for (int curBin = 0; curBin < _bins.Length; curBin++)
            {
                var curLength = 1/(double) _bins.Length;
                var curStart = curBin*curLength;
                var curEnd = (curBin + 1)*curLength;

                var isOutside = start > curEnd || end < curStart;
                if (!isOutside)
                {
                    var hasOverlap = end <= curEnd || start >= curStart;
                    if (hasOverlap)
                    {
                        var maxStart = Math.Max(start, curStart);
                        var maxEnd = Math.Min(end, curEnd);
                        var overlap = maxEnd - maxStart;
                        var overlapRatio = overlap/length;
                        _bins[curBin] += overlapRatio;
                    }
                    else
                    {
                        var isSpanningOver = start <= curStart && end >= curEnd;
                        if (isSpanningOver)
                        {
                            var overlapRatio = curLength/length;
                            _bins[curBin] += overlapRatio;
                        }
                    }
                }
            }
        }

        public double[] GetBins()
        {
            return _bins;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            var sum = _bins.Sum();
            var i = 1;
            foreach (var binVal in _bins)
            {
                var ratio = binVal/sum*100;
                var str = string.Format("{0}: {1,5:0.0}% ({2:#0.00})\n", i++, ratio, binVal);
                sb.Append(str);
            }
            sb.Append("(based on ");
            sb.Append((long) Math.Round(sum));
            sb.Append(" values)\n");

            var res = sb.ToString().Replace(',', '.');
            return res;
        }

        public int GetSize()
        {
            return (int) Math.Round(_bins.Sum());
        }
    }
}