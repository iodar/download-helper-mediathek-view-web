using System;
using System.IO;

namespace DownloadSrfVideos.Core.Tests.TestUtility
{
    public static class TestEnvironment
    {
        public static string BaseDirectory => AppDomain.CurrentDomain.BaseDirectory;
        public static string ResourceDirectory => Path.Combine(BaseDirectory, "Resources");
        public static string TempTestFolderDirectory = Path.Combine(BaseDirectory, "TempTest");
    }
}