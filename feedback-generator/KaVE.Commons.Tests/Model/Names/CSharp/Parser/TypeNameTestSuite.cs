using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaVE.Commons.Model.Names.CSharp.Parser;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Names.CSharp.Parser
{
    [TestFixture]
    class TypeNameTestSuite
    {
        private IKaVEList<TypeNameTestCase> testcases;
        private IKaVEList<string> invalidTypes;

        [SetUp]
        public void Init()
        {
            testcases = TestCaseProvider.ValidTypeNames();
            invalidTypes = TestCaseProvider.InvalidTypeNames();
        }

        [Test]
        public void ValidTypeName()
        {
            foreach (var typeNameTestCase in testcases)
            {
                Assert.DoesNotThrow(delegate { new CsTypeName(typeNameTestCase.GetIdentifier()); });
                var type = new CsTypeName(typeNameTestCase.GetIdentifier());
                Assert.AreEqual(typeNameTestCase.GetIdentifier(), type.Identifier);
                Assert.AreEqual(typeNameTestCase.GetAssembly(), type.Assembly.Identifier);
            }
        }

        [Test]
        public void InvalidTypeName()
        {
            foreach (var identifier in invalidTypes)
            {
                Assert.Catch(delegate { new CsTypeName(identifier); });   
            }
        }
    }
}
