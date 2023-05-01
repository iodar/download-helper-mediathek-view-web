using System.Linq;
using DownloadSrfVideos.Core.Core;
using DownloadSrfVideos.Core.Core.Impl;
using NUnit.Framework;

namespace DownloadSrfVideos.Core.Tests.Core.Impl
{
    [TestFixture]
    public class SegmentExtractorTest : TestBase
    {
        [Test]
        public void WhenValidPlaylistFileContentIsGiven_ItShouldReturnListOfSegments()
        {
            const string smallPlayListFile = @"#EXTM3U
#EXT-X-TARGETDURATION:10
#EXT-X-ALLOW-CACHE:YES
#EXT-X-PLAYLIST-TYPE:VOD
#EXT-X-VERSION:3
#EXT-X-MEDIA-SEQUENCE:1
#EXTINF:4.000,
segment-1-f6-v1-a1.ts
#EXTINF:6.000,
segment-2-f6-v1-a1.ts
#EXTINF:10.000,
segment-3-f6-v1-a1.ts
#EXTINF:10.000,
segment-4-f6-v1-a1.ts
#EXTINF:10.000,
segment-5-f6-v1-a1.ts
#EXTINF:10.000,
segment-6-f6-v1-a1.ts
#EXTINF:10.000,
segment-7-f6-v1-a1.ts
#EXTINF:10.000,
segment-8-f6-v1-a1.ts
#EXTINF:10.000,
segment-9-f6-v1-a1.ts
#EXTINF:10.000,
segment-10-f6-v1-a1.ts
#EXTINF:10.000,
segment-11-f6-v1-a1.ts
#EXTINF:10.000,
segment-12-f6-v1-a1.ts
#EXTINF:10.000,
segment-13-f6-v1-a1.ts
#EXTINF:10.000,
segment-14-f6-v1-a1.ts
#EXTINF:10.000,
segment-15-f6-v1-a1.ts
#EXTINF:10.000,
segment-16-f6-v1-a1.ts
#EXTINF:10.000,
segment-17-f6-v1-a1.ts";

            ISegmentExtractor segmentExtractor = new SegmentExtractor();

            var listOfSegments = segmentExtractor.ExtractSegments(smallPlayListFile);
            
            Assert.That(listOfSegments, Has.Count.EqualTo(17));
            Assert.That(listOfSegments.First(), Is.EqualTo("segment-1-f6-v1-a1.ts"));
            Assert.That(listOfSegments.Last(), Is.EqualTo("segment-17-f6-v1-a1.ts"));
        }
    }
}