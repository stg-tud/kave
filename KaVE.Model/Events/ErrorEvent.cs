namespace KaVE.Model.Events
{
    public class ErrorEvent : IDEEvent
    {
        public string Content { get; set; }
        public string[] StackTrace { get; set; }
    }
}
