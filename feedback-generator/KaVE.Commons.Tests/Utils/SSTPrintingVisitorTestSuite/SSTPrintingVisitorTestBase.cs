using System;
using System.Text;
using KaVE.Commons.Model.SSTs.Visitor;
using KaVE.Commons.Utils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.SSTPrintingVisitorTestSuite
{
    internal class SSTPrintingVisitorTestBase
    {
        private SSTPrintingVisitor _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new SSTPrintingVisitor();
        }

        protected void AssertPrint(ISSTNode sst, params string[] expectedLines)
        {
            var context = new StringBuilder();
            sst.Accept(_sut, context);
            var actual = context.ToString();
            var expected = String.Join(Environment.NewLine, expectedLines);
            Assert.AreEqual(expected, actual);
        }
    }
}