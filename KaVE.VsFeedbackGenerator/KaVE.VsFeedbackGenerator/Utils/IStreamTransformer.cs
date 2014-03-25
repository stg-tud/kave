using System.IO;
using System.IO.Compression;

namespace KaVE.VsFeedbackGenerator.Utils
{
    public interface IStreamTransformer
    {
        string Extention { get; }
        Stream TransformStreamForWrite(Stream stream);
        Stream TransformStreamForRead(Stream stream);
    }

    public class StreamTransformer : IStreamTransformer
    {
        public string Extention
        {
            get { return ""; }
        }

        public Stream TransformStreamForWrite(Stream stream)
        {
            return stream;
        }

        public Stream TransformStreamForRead(Stream stream)
        {
            return stream;
        }
    }

    public class ZipStreamTransformer : IStreamTransformer
    {
        public string Extention
        {
            get { return ".gz"; }
        }

        public Stream TransformStreamForWrite(Stream stream)
        {
            return new GZipStream(stream, CompressionMode.Compress);
        }

        public Stream TransformStreamForRead(Stream stream)
        {
            return new GZipStream(stream, CompressionMode.Decompress);
        }
    }
}