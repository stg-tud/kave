using System;
using System.IO;

namespace KaVE.Utils.IO
{
    public static class IoTestHelper
    {
        private static readonly Random Random = new Random();

        public static string GetTempDirectoryName()
        {
            var temp = Path.GetTempPath();
            string dir;
            do
            {
                dir = Path.Combine(temp, "dir-" + Random.Next());
            } while (Directory.Exists(dir));
            Directory.CreateDirectory(dir);
            return dir;
        }

        public static string GetTempFileName(string dir)
        {
            string file;
            do
            {
                file = Path.Combine(dir, "file-" + Random.Next());
            } while (File.Exists(file));
            File.Create(file);
            return file;
        }

        /// <summary>
        /// Creates a uniquely named, zero bytes temporary file on disk and returns its full path.
        /// </summary>
        /// <exception cref="IOException">if the file cannot be created</exception>
        public static string GetTempFileName()
        {
            return Path.GetTempFileName();
        }
    }
}