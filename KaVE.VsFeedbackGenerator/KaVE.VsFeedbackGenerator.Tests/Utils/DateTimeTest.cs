using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils
{
    [TestFixture]
    class DateTimeTest
    {
        [Test]
        public void DateTimeIsSerializedToShortDate()
        {
            var d = new DateTime(2013, 12, 6);
            var actual = d.ToString(CultureInfo.InvariantCulture);
            const string expected = "12/06/2013 00:00:00";
            Assert.AreEqual(expected, actual);
         }

        [Test]
        public void DateTimeCanBeReCreatedFromSerialization()
        {
            var d = new DateTime(2013, 12, 6);
            var tmp = d.ToString(CultureInfo.InvariantCulture);
            var actual = DateTime.Parse(tmp, CultureInfo.InvariantCulture);
            Assert.AreEqual(d, actual);
        }
    }
}
