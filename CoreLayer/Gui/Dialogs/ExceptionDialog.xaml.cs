using System;
using System.Text;
using System.Windows;
using Core.Utils;

namespace Medical.CoreLayer.Gui.Dialogs
{
    /// <summary>
    /// Interaction logic for AppSplashScreen.xaml
    /// </summary>
    public partial class ExceptionDialog : Window
    {
        public ExceptionDialog()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard();
        }

        private void CopyToClipboard()
        {
            if (IsCopyToClipboard.IsChecked == true)
            {
                Clipboard.SetText(Info.Text);
            }
        }

        public void SetErrorString(string message = null, Exception exceptionThrown = null, Type type = null)
        {
            var sb = new StringBuilder();

            sb.Append(CommonUtils.GetVersionInformationString(type));

            sb.AppendLine();

            if (message != null)
            {
                Reason.Text = message;
                sb.AppendLine(message);
            }
            if (exceptionThrown != null)
            {
                sb.AppendLine("Exception thrown:");
                sb.AppendLine(exceptionThrown.ToString());
                if (exceptionThrown.InnerException != null)
                {
                    sb.AppendLine("Inner exception thrown:");
                    sb.AppendLine(exceptionThrown.InnerException.ToString());
                }
            }
            sb.AppendLine();
            sb.AppendLine("---- Recent log messages:");
            try
            {
                sb.AppendLine("TODO");
            }
            catch (Exception ex)
            {
                sb.AppendLine("Failed to append recent log messages.");
                sb.AppendLine(ex.ToString());
            }
            sb.AppendLine();
            sb.AppendLine("---- Post-error application state information:");
            try
            {
                sb.AppendLine("TODO");
            }
            catch (Exception ex)
            {
                sb.AppendLine("Failed to append application state information.");
                sb.AppendLine(ex.ToString());
            }

            Info.Text = sb.ToString();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard();
            Application.Current.Shutdown();
        }

   
    }
}
