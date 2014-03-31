using System;

namespace KaVE.VsFeedbackGenerator.Interactivity
{
    internal class InteractionRequest<TNotification> : IInteractionRequest<TNotification> where TNotification : Notification
    {
        public event EventHandler<InteractionRequestedEventArgs<TNotification>> Raised = delegate { };

        public void Raise(TNotification notification, Action<TNotification> callback)
        {
            Raised(
                this,
                new InteractionRequestedEventArgs<TNotification>
                {
                    Notification = notification,
                    Callback = () => callback(notification)
                });
        }

        public void Delegate(InteractionRequestedEventArgs<TNotification> args)
        {
            Raised(this, args);
        }
    }
}