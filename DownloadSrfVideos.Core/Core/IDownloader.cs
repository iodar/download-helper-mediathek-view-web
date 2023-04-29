namespace DownloadSrfVideos.Core.Core
{
    public interface IDownloader
    {
        void DownloadFile(string uri);
        void DownloadFile(string uri, string targetPath);
    }
}