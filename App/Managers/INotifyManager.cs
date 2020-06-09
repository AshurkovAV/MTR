namespace Medical.AppLayer.Managers
{
    public interface INotifyManager
    {
        void ShowNotify(string text1, string text2 = "", string text3 = "");
    }
}
