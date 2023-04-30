using System.IO;
using DownloadSrfVideos.Core.Utility.Download;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace DownloadSrfVideos.Core.Tests.Utility.Download
{
    [TestFixture(Category = "Integration")]
    public class DownloadFileUtilityITest : TestBase
    {
        private string httpEuHttpbinOrgImageJpegUri = "http://eu.httpbin.org/image/jpeg";

        [Test]
        public void WhenCallingDownloadFile_ItShouldDownloadFile()
        {
            var imageTargetPath = Path.Combine(TempDirectory, "image.jpeg");
            DownloadFileUtility.ExecuteDownloadFile(httpEuHttpbinOrgImageJpegUri, imageTargetPath);
            using var imageFileStream = File.Open(imageTargetPath, FileMode.Open);
            Assert.That(imageFileStream, Has.Length.GreaterThan(0));
        }

        [Test]
        public void WhenSuccessfullyDownloadingFile_ThenShouldReturnDownloadResultWithTargetPath()
        {
            var imageTargetPath = Path.Combine(TempDirectory, "image.jpeg");
            var downloadResult = DownloadFileUtility.ExecuteDownloadFile(httpEuHttpbinOrgImageJpegUri, imageTargetPath);
            
            Assert.That(downloadResult.FilePath, Is.EqualTo(imageTargetPath));
        }
        
        [Test]
        public void WhenSuccessfullyDownloadingFile_ThenShouldReturnDownloadResultWithTargetFileName()
        {
            var targetFileName = "image.jpeg";
            var imageTargetPath = Path.Combine(TempDirectory, targetFileName);
            var downloadResult = DownloadFileUtility.ExecuteDownloadFile(httpEuHttpbinOrgImageJpegUri, imageTargetPath);
            
            Assert.That(downloadResult.FileName, Is.EqualTo(targetFileName));
        }

        [Test]
        public void WhenDownloadFails_ThenShouldReturnDownloadResultWithoutTargetPath()
        {
            var imageTargetPath = Path.Combine(TempDirectory, "image.jpeg");
            var downloadResult = DownloadFileUtility.ExecuteDownloadFile("http://eu.httpbin.org/image-foobar/jpeg", imageTargetPath);

            Assert.That(downloadResult.FilePath, Is.Null);
        }
    }
}