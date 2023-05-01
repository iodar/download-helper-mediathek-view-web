using System.Collections.Generic;

namespace DownloadSrfVideos.Core.Core
{
    public interface IVideoComposer
    {
        string ComposeVideoFromSegments(string outVideoFilePath, IList<string> listOfDownloadedVideoSegments);
    }
}