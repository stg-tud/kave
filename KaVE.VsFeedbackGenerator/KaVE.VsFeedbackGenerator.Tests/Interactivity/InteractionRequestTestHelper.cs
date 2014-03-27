using System;
using KaVE.VsFeedbackGenerator.Interactivity;

namespace KaVE.VsFeedbackGenerator.Tests.Interactivity
{
    /// <summary>
    ///     Captures a single interaction request.
    /// </summary>
    public class InteractionRequestTestHelper<T> where T : Notification
    {
        public bool IsRequestRaised { get; private set; }
        public T Context { get; private set; }
        public Action Callback { get; private set; }

        internal InteractionRequestTestHelper(IInteractionRequest<T> request)
        {
            request.Raised += (s, e) =>
            {
                IsRequestRaised = true;
                Context = e.Notification;
                Callback = e.Callback;
            };
        }
    }

    public static class InteractionRequestTestHelperExtensions
    {
        public static InteractionRequestTestHelper<TContext> NewTestHelper<TContext>(this IInteractionRequest<TContext> request)
            where TContext : Notification
        {
            return new InteractionRequestTestHelper<TContext>(request);
        }
    }
}