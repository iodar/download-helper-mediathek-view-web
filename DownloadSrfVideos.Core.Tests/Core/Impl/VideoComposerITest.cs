using System;
using System.IO;
using DownloadSrfVideos.Core.Core;
using DownloadSrfVideos.Core.Core.Impl;
using FFMpegCore;
using NUnit.Framework;

namespace DownloadSrfVideos.Core.Tests.Core.Impl
{
    [TestFixture(Category = "Integration")]
    public class VideoComposerITest : TestBase
    {
        [Test]
        public void WhenListOfSegmentsIsProvided_ThenItShouldBeConcatenatedIntoASingleFile()
        {
            var outFilePath = Path.Combine(TempDirectory, "output.mp4");
            var sourceFiles = Directory.GetFiles(ResourcesDirectory, "segment*.mp4");
            IVideoComposer videoComposer = new VideoComposer();

            videoComposer.ComposeVideoFromSegments(outFilePath, sourceFiles);
            
            Assert.That(FFProbe.Analyse(outFilePath).Duration, Is.EqualTo(TimeSpan.FromSeconds(18.913)));
        }

        [Test]
        public void WhenListOfSegmentsIsProvided_ThenItShouldReturnTargetFilePath()
        {
            var outFilePath = Path.Combine(TempDirectory, "output.mp4");
            var sourceFiles = Directory.GetFiles(ResourcesDirectory, "segment*.mp4");
            IVideoComposer videoComposer = new VideoComposer();

            var targetFilePath = videoComposer.ComposeVideoFromSegments(outFilePath, sourceFiles);
            
            Assert.That(targetFilePath, Is.EqualTo(outFilePath));
        }
    }
}