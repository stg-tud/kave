using System;

namespace KaVE.VsFeedbackGenerator.Utils
{
    public interface IFormatReader<out TMessage>
    {
        string Extention { get; }
        ILogReader<TMessage> NewReader(string fullpath);
    }

    public class FormatReader<TMessage> : IFormatReader<TMessage>
    {
        private readonly Func<string, ILogReader<TMessage>> _newReader;

        public FormatReader(string extention, Func<string, ILogReader<TMessage>> newReader)
        {
            Extention = extention;
            _newReader = newReader;
        }

        public string Extention
        {
            get; private set; }

        public ILogReader<TMessage> NewReader(string fullpath)
        {
            return _newReader(fullpath);
        }
    }
}
