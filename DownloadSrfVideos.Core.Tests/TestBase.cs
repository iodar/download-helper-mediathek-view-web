using System.IO;
using DownloadSrfVideos.Core.Tests.TestUtility;
using NUnit.Framework;

namespace DownloadSrfVideos.Core.Tests
{
    [SetUpFixture]
    public abstract class TestBase
    {
        protected string ResourcesDirectory => TestEnvironment.ResourceDirectory;
        protected string TempDirectory => TestEnvironment.TempTestFolderDirectory;

        [OneTimeSetUp]
        protected void SetUp()
        {
            if (Directory.Exists(TempDirectory))
            {
                CleanTempDirectory();
            }
            else
            {
                Directory.CreateDirectory(TempDirectory);
            }
        }

        /// <summary>
        /// Clean TempDirectory for unit tests
        /// </summary>
        private void CleanTempDirectory()
        {
            foreach (var directory in Directory.GetDirectories(TempDirectory, "*", SearchOption.AllDirectories))
            {
                DeleteAllFiles(directory);
                Directory.Delete(directory);
            }

            DeleteAllFiles(TempDirectory);
        }

        /// <summary>
        /// Deletes all files in a directory
        /// </summary>
        /// <param name="directory">Directory to delete all files from</param>
        private static void DeleteAllFiles(string directory)
        {
            var filesToDelete = Directory.GetFiles(directory, "*.*");
            foreach (var file in filesToDelete)
            {
                File.Delete(file);
            }
        }
    }
}