using System;
using System.Collections.Generic;
using KaVE.Model.Events;
using KaVE.VsFeedbackGenerator.Utils;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils
{
    [TestFixture]
    internal class ExportCommandTest
    {
        [TestCase(true), TestCase(false)]
        public void CommandForwardsCanExecute(bool canExecuteOrNot)
        {
            var canExecute = new Predicate<object>(o => canExecuteOrNot);
            var uut = ExportCommand.Create(
                new Mock<ISessionExport>().Object,
                new Mock<Func<IEnumerable<IDEEvent>>>().Object,
                new Mock<Func<string, ILogWriter<IDEEvent>>>().Object,
                canExecute);
            Assert.AreEqual(canExecuteOrNot, uut.CanExecute(null));
        }

        [Test]
        public void CommandInterpredsNoCanExecuteProperly()
        {
            var uut = ExportCommand.Create(
                new Mock<ISessionExport>().Object,
                new Mock<Func<IEnumerable<IDEEvent>>>().Object,
                new Mock<Func<string, ILogWriter<IDEEvent>>>().Object);
            Assert.IsTrue(uut.CanExecute(null));
        }

        [Test]
        public void CommandTest()
        {
            var exportPolicy = new Mock<ISessionExport>();
            var writerFactory = new Mock<Func<string, ILogWriter<IDEEvent>>>();
            var expectedResult = new Mock<ExportResult<IList<IDEEvent>>>();
            var list = new List<IDEEvent>();
            for (var i = 0; i < 25; i ++)
            {
                list.Add(new Mock<IDEEvent>().Object);
            }
            var listGenerator = new Func<IEnumerable<IDEEvent>>(() => list);
            var resultIsCorrectAssertion = new Action<ExportResult<IList<IDEEvent>>>(r => Assert.AreEqual(expectedResult.Object, r));

            exportPolicy.Setup(e => e.Export<IDEEvent>(list, writerFactory.Object)).Returns(expectedResult.Object);

            var uut = ExportCommand.Create(
                exportPolicy.Object,
                listGenerator,
                writerFactory.Object,
                resultHandler: resultIsCorrectAssertion);
            uut.Execute(null);
        }
    }
}