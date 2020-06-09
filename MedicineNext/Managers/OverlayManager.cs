using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Core;
using Core.Extensions;
using DevExpress.Xpf.Core.Native;
using GalaSoft.MvvmLight.Threading;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Medical.AppLayer.Managers;

namespace MedicineNext.Managers
{
    public class OverlayManager : IOverlayManager
    {
        private readonly MetroWindow _mainWindow;
        private ProgressDialogController _controller;
        private MetroWindow _dialog;
        private DateTime? _starttime;

        public OverlayManager()
        {
            _mainWindow = Application.Current.MainWindow as MetroWindow;
        }

        public async void ShowOverlay()
        {
            await ShowOverlay(Constants.OperationBeginTitleMsg, Constants.PleaseWaitMsg);
        }

        public async Task ShowOverlay(string title, string message, int? min = null, int? max = null)
        {
            _starttime = DateTime.Now;
            _dialog = Application.Current.Windows.OfType<MetroWindow>().SingleOrDefault(x => x.IsActive && x != _mainWindow);
            _dialog?.Hide();

            _controller = await _mainWindow.ShowProgressAsync(title, message);
            if (min.HasValue && max.HasValue)
            {
                _controller.Minimum = min.Value;
                _controller.Maximum= max.Value;
            }
            else
            {
                _controller.SetIndeterminate();
            }
            
        }

        public async void HideOverlay()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => _dialog?.ShowDialog());
            _starttime = null;
            await _controller.CloseAsync();
        }

        public void SetOverlayMessage(string message)
        {
            _controller.SetMessage(message);
            _controller.SetIndeterminate();
        }

        public void SetOverlayTitle(string title)
        {
            _controller.SetTitle(title);
        }

        public void SetOverlayTitleAndMessage(string title = "", string message = "")
        {
            if (title.IsNotNullOrEmpty())
            {
                _controller.SetIndeterminate();
                _controller.SetTitle(title);
            }

            if (message.IsNotNullOrEmpty())
            {
                _controller.SetIndeterminate();
                _controller.SetMessage(message);
            }

        }

        /// <summary>
        /// Изменить прогресс в %
        /// </summary>
        /// <param name="percent">прогресс в %</param>

        public void SetOverlayProgress(double percent)
        {
            _controller.Maximum = 100;
            _controller.SetProgress(percent);
        }

        /// <summary>
        /// Изменить прогресс (будет показано примерное время завершения)
        /// </summary>
        /// <param name="total">всего тиков</param>
        /// <param name="progress">выполнено</param>
        public void SetOverlayProgress(double total, double progress)
        {
            _controller.Maximum = total;
            _controller.SetProgress(progress);
            _controller.SetMessage(TimeRemain(total, progress));
        }

        private string TimeRemain(double total, double progress)
        {
            Debug.Assert(_starttime != null, "_starttime != null");
            TimeSpan timespent = DateTime.Now - _starttime.Value;
            double msRemaining = timespent.TotalMilliseconds / progress * (total - progress);
            return Constants.RemainMsg.F(msRemaining.FormatTimeInterval());
        }
    }
}
