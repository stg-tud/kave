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
                        var absoluteOverlap = maxEnd - maxStart;
                        Asserts.That(absoluteOverlap >= 0);
                        var overlapRatio = absoluteOverlap/curLength;

                        // max amount a single bin is assigned
                        var binValue = curLength/length;
                        var curValue = binValue*overlapRatio;

                        _bins[curBin] += curValue;
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

            return sb.ToString();
        }
    }
}