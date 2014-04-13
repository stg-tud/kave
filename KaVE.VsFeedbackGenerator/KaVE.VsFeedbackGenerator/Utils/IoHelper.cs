using System.IO;
using JetBrains.Application;

namespace KaVE.VsFeedbackGenerator.Utils
{
    public interface IIoHelper
    {
        void TransferViaHttpPost(string url, string srcFile, string filename = null);
        void CopyFile(string src, string trg);
    }

    [ShellComponent]
    public class IoHelper : IIoHelper
    {
        public void TransferViaHttpPost(string url, string srcFile, string filename = null)
        {
            HttpPostFileTransfer.TransferFile(url, srcFile, filename);
        }

        public void CopyFile(string src, string trg)
        {
            File.Copy(src, trg, true);
        }
    }
}
