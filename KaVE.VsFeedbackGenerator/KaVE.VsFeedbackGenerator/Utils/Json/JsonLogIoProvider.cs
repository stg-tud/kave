using System;
using System.IO;
using System.IO.Compression;

namespace KaVE.VsFeedbackGenerator.Utils.Json
{
    public static class JsonLogIoProvider
    {
        public static IFormatWriter<TMessage> JsonFormatWriter<TMessage>()
        {
            return new FormatWriter<TMessage>(".log", UncompressedReader<TMessage>, UncompressedWriter<TMessage>);
        }

        public static IFormatWriter<TMessage> CompressedJsonFormatWriter<TMessage>()
        {
            return new FormatWriter<TMessage>(".log.gz", CompressedReader<TMessage>, CompressedWriter<TMessage>);
        }

        internal static ILogWriter<TMessage> CompressedWriter<TMessage>(string logFileName)
        {
            Stream logStream = new FileStream(logFileName, FileMode.Append, FileAccess.Write);
            try
            {
                logStream = new GZipStream(logStream, CompressionMode.Compress);
                return new JsonLogWriter<TMessage>(logStream);
            }
            catch (Exception)
            {
                logStream.Close();
                throw;
            }
        }

        internal static ILogWriter<TMessage> UncompressedWriter<TMessage>(string logFileName)
        {
            Stream logStream = new FileStream(logFileName, FileMode.Append, FileAccess.Write);
            try
            {
                return new JsonLogWriter<TMessage>(logStream);
            }
            catch (Exception)
            {
                logStream.Close();
                throw;
            }
        }

        internal static ILogReader<TMessage> CompressedReader<TMessage>(string logFileName)
        {
            Stream logStream = new FileStream(logFileName, FileMode.Open);
            try
            {
                logStream = new GZipStream(logStream, CompressionMode.Decompress);
                return new JsonLogReader<TMessage>(logStream);
            }
            catch (Exception)
            {
                logStream.Close();
                throw;
            }
        }

        internal static ILogReader<TMessage> UncompressedReader<TMessage>(string logFileName)
        {
            Stream logStream = new FileStream(logFileName, FileMode.Open);
            try
            {
                return new JsonLogReader<TMessage>(logStream);
            }
            catch (Exception)
            {
                logStream.Close();
                throw;
            }
        }
    }
}