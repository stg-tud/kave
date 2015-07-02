using System;
using KaVE.Commons.Utils.Assertion;

namespace KaVE.RS.SolutionAnalysis
{
    public class MergingHistogram : Histogram
    {
        private readonly int _numSlots;

        public MergingHistogram(int numSlots) : base(numSlots)
        {
            _numSlots = numSlots;
        }

        public void AddRatio(int enumerator, int denominator)
        {
            Asserts.That(denominator >= _numSlots);

            var ratio = enumerator/(1.0*denominator);
            var slot = (int) Math.Ceiling(_numSlots*ratio);
            Add(slot);
        }
    }
}