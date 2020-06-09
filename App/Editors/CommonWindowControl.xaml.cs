using System;
using System.Windows;
using System.Windows.Controls;
using Core.Extensions;
using MahApps.Metro.Controls;

namespace Medical.AppLayer.Editors
{
    /// <summary>
    /// Interaction logic for CommonWindow.xaml
    /// </summary>
    public partial class CommonWindowControl : MetroWindow
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
        public CommonWindowControl(Window owner, double width, double height, object model, UserControl control)
        {
            InitializeComponent();
            try
            {
                Owner = owner;
                Height = height;
                Width = width;
                DataContext = model;

                Grid.SetRow(control, 0);
                Grid.Children.Add(control);
            }
            catch (Exception ex)
            {
            }
            
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
