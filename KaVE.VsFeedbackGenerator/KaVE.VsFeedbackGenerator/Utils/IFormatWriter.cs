using System;

namespace KaVE.VsFeedbackGenerator.Utils
{
    public interface IFormatWriter<TMessage> : IFormatReader<TMessage>
    {
        ILogWriter<TMessage> NewWriter(string fullpath);
    }

    public class FormatWriter<TMessage> : FormatReader<TMessage>, IFormatWriter<TMessage>
    {
        private readonly Func<string, ILogWriter<TMessage>> _newWriter;

        public FormatWriter(string extention, Func<string, ILogReader<TMessage>> newReader, Func<string, ILogWriter<TMessage>> newWriter)
            : base(extention, newReader)
        {
            _newWriter = newWriter;
        }

        public ILogWriter<TMessage> NewWriter(string fullpath)
        {
            return _newWriter(fullpath);
        }
    }
}
