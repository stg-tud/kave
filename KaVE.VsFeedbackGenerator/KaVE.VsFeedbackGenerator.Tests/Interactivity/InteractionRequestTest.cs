using KaVE.VsFeedbackGenerator.Interactivity;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Interactivity
{
    [TestFixture]
    internal class InteractionRequestTest
    {
        private InteractionRequest<Notification> _uut;

        [SetUp]
        public void SetUpRequest()
        {
            _uut = new InteractionRequest<Notification>();
        }

        [Test]
        public void ShouldRaiseRequest()
        {
            var raised = false;
            _uut.Raised += (sender, args) => raised = true;

            _uut.Raise(new Notification(), n => { });

            Assert.IsTrue(raised);
        }

        [Test]
        public void ShouldPassContextToHandler()
        {
            Notification actualContext = null;
            _uut.Raised += (sender, args) => actualContext = args.Notification;
            var expectedContext = new Notification();

            _uut.Raise(expectedContext, n => { });

            Assert.AreSame(expectedContext, actualContext);
        }

        [Test]
        public void ShouldDelegateToCallbackFromHandler()
        {
            var invoked = false;
            _uut.Raised += (sender, args) => args.Callback();

            _uut.Raise(new Notification(), n => invoked = true);

            Assert.IsTrue(invoked);
        }
    }
}