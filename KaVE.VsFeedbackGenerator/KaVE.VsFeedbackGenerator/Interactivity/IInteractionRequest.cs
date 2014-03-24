using System;

namespace KaVE.VsFeedbackGenerator.Interactivity
{
    public interface IInteractionRequest<TNotification> where TNotification : Notification
    {
        event EventHandler<InteractionRequestedEventArgs<TNotification>> Raised;
    }
}