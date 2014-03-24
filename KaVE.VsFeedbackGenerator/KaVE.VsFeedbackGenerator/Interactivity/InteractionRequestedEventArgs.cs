using System;

namespace KaVE.VsFeedbackGenerator.Interactivity
{
    public class InteractionRequestedEventArgs<TNotification> : EventArgs where TNotification : Notification
    {
        public TNotification Notification { get; set; }
        public Action Callback { get; set; }
    }
}