using System.Collections.Generic;

namespace DownloadSrfVideos.Core.Core
{
    public interface ISegmentExtractor
    {
        IList<string> ExtractSegmentsFromFile(string filePath);
    }
}