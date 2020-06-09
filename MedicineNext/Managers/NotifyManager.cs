using System;
using System.Media;
using DevExpress.Mvvm;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Core;
using Medical.AppLayer.Managers;

namespace MedicineNext.Managers
{
    public class NotifyManager : INotifyManager
    {
        private readonly NotificationService _notificationService;

        public NotifyManager()
        {
            _notificationService = new NotificationService
            {
                UseWin8NotificationsIfAvailable = false,
                PredefinedNotificationTemplate = NotificationTemplate.LongText
            };
        }

        public async void ShowNotify(string text1, string text2 = "", string text3 = "")
        {
            SystemSounds.Exclamation.Play();
            INotification notification;
            await GalaSoft.MvvmLight.Threading.DispatcherHelper.RunAsync(() => {
                notification = _notificationService.CreatePredefinedNotification(text1, text2, text3);
                notification.ShowAsync();
            });
        }
    }
}
