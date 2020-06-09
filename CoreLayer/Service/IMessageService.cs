using System;

namespace Medical.CoreLayer.Service
{
    public interface IMessageService
    {
        void ShowError(string message);
        void ShowErrorFormatted(string message, params object[] formatitems);
        void ShowException(Exception ex, string message, Type type);
        void ShowExceptionFormatted(Exception ex, string message, Type type, params object[] formatitems);
        void ShowWarning(string message);
        void ShowWarningFormatted(string message, params object[] formatitems);
        bool AskQuestion(string question, string caption);
        bool AskQuestionFormatted(string question, string caption, params object[] formatitems);
        void ShowMessage(string message, string caption);
        void ShowMessage(string message);
        void ShowMessageFormatted(string message, string caption, params object[] formatitems);

    }
}
