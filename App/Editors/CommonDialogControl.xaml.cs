using System;
using System.Windows;
using System.Windows.Controls;
using Core.Extensions;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace Medical.AppLayer.Editors
{
    /// <summary>
    /// Interaction logic for CommonWindow.xaml
    /// </summary>
    public partial class CommonDialogControl : BaseMetroDialog
    {
        private Action _okCallback;
        public Action OkCallback {
            set
            {
                _okCallback = value;
                ButtonOk.Visibility = _okCallback.IsNotNull() ? Visibility.Visible : Visibility.Hidden;
            }
        }
        private Action _cancelCallback;
        public Action CancelCallback
        {
            set
            {
                _cancelCallback = value;
                ButtonCancel.Visibility = _cancelCallback.IsNotNull() ? Visibility.Visible : Visibility.Hidden;
            }
        }
        public CommonDialogControl(object model, UserControl control)
        {
            InitializeComponent();

            DataContext = model;

            Grid.SetRow(control, 0);
            Grid.Children.Add(control);
        }

        private void Button_Ok(object sender, RoutedEventArgs e)
        {
            _okCallback();
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            _cancelCallback();
        }
    }
}
