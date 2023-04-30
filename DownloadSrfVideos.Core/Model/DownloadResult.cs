using System.IO;
using System.Net;

namespace DownloadSrfVideos.Core.Model
{
    public class DownloadResult
    {
        private DownloadResult(string uri, HttpStatusCode httpStatusCode, string? filePath)
        {
            Uri = uri;
            StatusCode = httpStatusCode;
            FileName = Path.GetFileName(filePath);
            FilePath = filePath;
        }

        public HttpStatusCode StatusCode { get; set; }
        public string Uri { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }

        public static DownloadResult FromSuccessfulDownload(string uri, HttpStatusCode httpStatusCode, string filePath)
        {
            return new DownloadResult(uri, httpStatusCode, filePath);
        }

        public static DownloadResult FromDownloadFailed(string uri, HttpStatusCode httpStatusCode)
        {
            return new DownloadResult(uri, httpStatusCode, null);
        }
    }
}