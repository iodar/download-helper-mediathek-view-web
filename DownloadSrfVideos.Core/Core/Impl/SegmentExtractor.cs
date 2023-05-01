using System;
using System.Collections.Generic;
using System.Linq;

namespace DownloadSrfVideos.Core.Core.Impl
{
    public class SegmentExtractor : ISegmentExtractor
    {
        public IList<string> ExtractSegments(string fileContent)
        {
            var linesInFile = fileContent
                .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim())
                .ToList();

            var listOfSegmentNames = linesInFile.Where(line => !line.StartsWith("#")).ToList();
            return listOfSegmentNames;
        }
    }
}