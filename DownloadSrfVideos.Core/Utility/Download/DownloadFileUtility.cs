using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DownloadSrfVideos.Core.Model;

namespace DownloadSrfVideos.Core.Utility.Download
{
    public static class DownloadFileUtility
    {
        public static DownloadResult ExecuteDownloadFile(string uri, string targetPath)
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
                return DownloadResult.FromSuccessfulDownload(uri, downloadTask.Result.StatusCode, targetPath);
            }
            
            return DownloadResult.FromDownloadFailed(uri, downloadTask.Result.StatusCode);
        }
    }
}