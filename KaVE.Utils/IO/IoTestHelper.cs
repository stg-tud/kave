using System;
using System.IO;

namespace KaVE.Utils.IO
{
    public static class IoTestHelper
    {
        private static readonly Random Random = new Random();

        /// <summary>
        /// Creates a new directory in the system's temporary-files directory and returns its full path.
        /// </summary>
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