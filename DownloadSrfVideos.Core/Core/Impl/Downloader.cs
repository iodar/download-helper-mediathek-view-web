using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DownloadSrfVideos.Core.Core.Impl
{
    public class Downloader : IDownloader
    {
        private string TargetPath { get; }
        
        public Downloader(string targetPath)
        {
            TargetPath = targetPath;
        }

        public void DownloadFile(string uri)
        {
            ExecuteDownloadFile(uri, TargetPath);
        }

        public void DownloadFile(string uri, string targetPath)
        {
            ExecuteDownloadFile(uri, targetPath);
        }

        private static void ExecuteDownloadFile(string uri, string targetPath)
        {
            HttpClient httpClient = new HttpClient();
            var downloadTask = Task.Run(() => httpClient.GetAsync(uri));
            downloadTask.Wait();
            FileStream fileStream = new FileStream(targetPath, FileMode.Create);
            if (downloadTask.Result.StatusCode == HttpStatusCode.OK)
            {
                var stream = downloadTask.Result.Content.ReadAsStream();
                stream.CopyTo(fileStream);
                fileStream.Close();
            }
        }
    }
}