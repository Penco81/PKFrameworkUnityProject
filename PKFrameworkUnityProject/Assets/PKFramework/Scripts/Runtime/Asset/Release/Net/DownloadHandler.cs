namespace PKFramework.Runtime.Asset
{
    public interface IDownloadHandler
    {
        void OnStart();
        void OnPause(bool paused);
        bool Update();
        void OnCancel();
    }
}