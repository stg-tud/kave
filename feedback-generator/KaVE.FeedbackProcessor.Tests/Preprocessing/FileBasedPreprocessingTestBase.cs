using System.IO;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.IO.Archives;
using KaVE.Commons.Utils.Json;
using KaVE.FeedbackProcessor.Preprocessing.Model;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Preprocessing
{
    internal abstract class FileBasedPreprocessingTestBase
    {
        private string _rootDir;
        protected string RawDir;
        protected string MergedDir;
        protected string FinalDir;

        protected PreprocessingIo Io;

        [SetUp]
        public void BaseSetup()
        {
            _rootDir = CreateTempDir();
            RawDir = MkDir("raw");
            MergedDir = MkDir("merged");
            FinalDir = MkDir("final");

            Io = new PreprocessingIo(RawDir, MergedDir, FinalDir);
        }

        private string MkDir(string dirName)
        {
            var dir = Path.Combine(_rootDir, dirName);
            Directory.CreateDirectory(dir);
            return dir;
        }

        private static string CreateTempDir()
        {
            var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(path);
            return path;
        }

        [TearDown]
        public void BaseTeardown()
        {
            if (Directory.Exists(_rootDir))
            {
                Directory.Delete(_rootDir, true);
            }
        }

        protected void Write<T>(string fileName, params T[] ts)
        {
            Assert.IsTrue(fileName.StartsWith(_rootDir));
            using (var wa = new WritingArchive(fileName))
            {
                wa.AddAll(ts);
            }
        }

        protected IKaVEList<T> Read<T>(string fileName)
        {
            Assert.IsTrue(fileName.StartsWith(_rootDir));
            var json = File.ReadAllText(fileName);
            return json.ParseJsonTo<IKaVEList<T>>();
        }
    }
}