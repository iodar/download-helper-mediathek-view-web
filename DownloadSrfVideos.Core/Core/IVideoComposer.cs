using System.Collections.Generic;

namespace DownloadSrfVideos.Core.Core
{
    public interface IVideoComposer
    {
        string ComposeVideoFromSegments(IList<string> videoSegments);
    }
}