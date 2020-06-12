namespace Omsit.UpdateSlashScreen.DXSplashScreen.Internal
{
    public interface ISplashScreenEx
    {
        void CloseSplashScreen();
        void Progress(double value);
        void Progress(string value);
        void SetProgressState(bool isIndeterminate);
    }
}