using System;
using System.Windows;
using Core.Extensions;
using Core.Infrastructure;
using DevExpress.Xpf.WindowsUI;
using GalaSoft.MvvmLight.Threading;
using Medical.CoreLayer.Gui.Dialogs;

namespace Medical.CoreLayer.Service
{
    public class MessageService : IMessageService
    {
        public  void ShowException(Exception ex)
        {
            ShowException(ex, null, null);
        }

        public void ShowError(string message)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => WinUIMessageBox.Show(
              message,
              CoreMessages.Error,
              MessageBoxButton.OK,
              MessageBoxImage.Error,
              MessageBoxResult.None, MessageBoxOptions.None
              ));
        }

        public void ShowErrorFormatted(string message, params object[] formatitems)
        {
            ShowError(message.F(formatitems));
        }

        public  void ShowException(Exception ex, string message, Type type)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => { 
                var exceptionDialog = new ExceptionDialog();
                exceptionDialog.SetErrorString(message, ex, type);
                exceptionDialog.ShowDialog();
            });
            
        }

        public void ShowExceptionFormatted(Exception ex, string message, Type type, params object[] formatitems)
        {
            ShowException(ex, message.F(formatitems), type);
        }

        public void ShowWarning(string message)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => WinUIMessageBox.Show(
               message,
               CoreMessages.Warning,
               MessageBoxButton.OK,
               MessageBoxImage.Warning,
               MessageBoxResult.None, MessageBoxOptions.None
               ));
        }

        public void ShowWarningFormatted(string message, params object[] formatitems)
        {
            ShowWarning(message.F(formatitems));
        }

        public bool AskQuestion( string question, string caption)
        {
            return WinUIMessageBox.Show(
                question,
                caption,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.Yes, MessageBoxOptions.None
                ) == MessageBoxResult.Yes;
        }

        public bool AskQuestionFormatted(string question, string caption, params object[] formatitems)
        {
            return AskQuestion(question.F(formatitems), caption);
        }

        public void ShowMessage(string message, string caption)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => WinUIMessageBox.Show(
                message,
                caption,
                MessageBoxButton.OK,
                MessageBoxImage.Information,
                MessageBoxResult.None, MessageBoxOptions.None
                ));
           
        }

        public void ShowMessage(string message)
        {
            ShowMessage(message, CoreMessages.Info);
        }

        public void ShowMessageFormatted(string message, string caption, params object[] formatitems)
        {
            ShowMessage(message.F(formatitems),caption);
        }
    }
}
