namespace DownloadSrfVideos.Core.Core
{
    public interface IVideoDownloadManager
    {
        string DownloadAndComposeVideo(string uri);
        
        void DownloadAndComposeVideo(string uri, string targetPath);
    }
}