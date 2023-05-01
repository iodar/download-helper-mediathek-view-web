using System.Collections.Generic;
using System.Linq;
using FFMpegCore;

namespace DownloadSrfVideos.Core.Core.Impl
{
    public class VideoComposer : IVideoComposer
    {
        public string ComposeVideoFromSegments(string outVideoFilePath, IList<string> listOfDownloadedVideoSegments)
        {
            FFMpeg.Join(outVideoFilePath, listOfDownloadedVideoSegments.ToArray());
            return outVideoFilePath;
        }
    }
}