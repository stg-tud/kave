namespace KaVE.VsFeedbackGenerator.Utils
{
    public enum State
    {
        Ok,
        Fail
    }

    public class ExportResult<T>
    {
        public static ExportResult<T> Fail(T data = default(T), string message = null)
        {
            return new ExportResult<T> {Status = State.Fail, Message = message, Data = data};
        }

        public static ExportResult<T> Success(T data = default(T), string message = null)
        {
            return new ExportResult<T> {Status = State.Ok, Message = message, Data = data};
        }

        public static ExportResult<T> CloneWithData<TOther>(ExportResult<TOther> template, T data = default(T))
        {
            return new ExportResult<T> {Status = template.Status, Message = template.Message, Data = data};
        }

        public State Status;
        public string Message;
        public T Data;
    }
}