using System;
using System.Collections.Generic;

namespace KaVE.Utils.DateTime
{
    public class SimilarDateTimeComparer : IComparer<System.DateTime>
    {
        private readonly uint _diffMillis;

        public SimilarDateTimeComparer(uint diffMillis)
        {
            _diffMillis = diffMillis;
        }

        public int Compare(System.DateTime x, System.DateTime y)
        {
            var diff = Math.Abs((x - y).TotalMilliseconds);
            return (diff <= _diffMillis) ? 0 : x.CompareTo(y);
        }

        public bool Equal(System.DateTime x, System.DateTime y)
        {
            return Compare(x, y) == 0;
        }
    }
}
