using System;
using System.IO;
using System.Windows;
using System.Windows.Data;
using FastReport;
using FastReport.Design;
using Medical.AppLayer.Report.Helpers;
using Microsoft.Win32;
using Microsoft.Windows.Controls.PropertyGrid;
using Microsoft.Windows.Controls.PropertyGrid.Editors;

namespace Medical.AppLayer.Editors
{
    /// <summary>
    /// Interaction logic for ReportControl.xaml
    /// </summary>
    public partial class ReportControl : ITypeEditor
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", 
            typeof (byte[]),
            typeof (ReportControl),
            new FrameworkPropertyMetadata(null,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        private readonly EnvironmentSettings _environmentSettings;

        public ReportControl()
        {
            InitializeComponent();
            _environmentSettings = new EnvironmentSettings();
            _environmentSettings.CustomSaveReport += CustomSaveReport;
        }

        public byte[] Value
        {
            get { return (byte[])GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        #region ITypeEditor Members

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            var binding = new Binding("Value")
                              {
                                  Source = propertyItem,
                                  Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay
                              };
            BindingOperations.SetBinding(this, ValueProperty, binding);
            return this;
        }

        #endregion

        private void CustomSaveReport(object sender, OpenSaveReportEventArgs e)
        {
            try
            {
                var tmp = new MemoryStream();
                e.Report.Save(tmp);
                e.Report.Save(e.FileName);
                Value = tmp.GetBuffer();
            }
            catch (Exception exception)
            {
                //UnityService.Instance.Resolve<MessageManager>().ShowHandledException(exception,"При сохранении отчета произошло исключение");
            }
        }

        private void ResetValue(object sender, RoutedEventArgs e)
        {
            Value = null;
        }

        private void ShowDesigner(object sender, RoutedEventArgs e)
        {
            try
            {
                var report = new FastReport.Report();
                if (Value == null || Value.Length == 0)
                {
                    report.Load(ReportTemplate.GetDefaultReport());
                }
                else
                {
                    report.Load(new MemoryStream(Value));
                }

                report.Design();
                report.Dispose();
            }
            catch (Exception exception)
            {
               // UnityService.Instance.Resolve<MessageManager>().ShowHandledException(exception,"При вызове дизайнера отчетов произошло исключение");
            }
        }


        private void SelectFile(object sender, RoutedEventArgs e)
        {
            try
            {
                var dlg = new OpenFileDialog
                              {DefaultExt = ".frx", Filter = "Файлы отчетов FastReport .NET (.frx)|*.frx"};

                if (dlg.ShowDialog() == true)
                {
                    using (var stream = new FileStream(dlg.FileName, FileMode.Open))
                    {
                        using (var memory = new MemoryStream())
                        {
                            stream.CopyTo(memory);
                            Value = memory.GetBuffer();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                //UnityService.Instance.Resolve<MessageManager>().ShowHandledException(exception,"При конвертиции файла отчетов произошло исключение");
            }
        }

        private void PreviewReport(object sender, RoutedEventArgs e)
        {
            try
            {
                var report = new FastReport.Report();
                report.Load(new MemoryStream(Value));
                report.Show();
                report.Dispose();
            }
            catch (Exception exception)
            {
                //UnityService.Instance.Resolve<MessageManager>().ShowHandledException(exception,"При предпросмотре отчета произошло исключение");
            }
        }

        private void SaveReportFile(object sender, RoutedEventArgs e)
        {
            try
            {
                var dlg = new SaveFileDialog { DefaultExt = ".frx", Filter = "Файлы отчетов FastReport .NET (.frx)|*.frx" };

                if (dlg.ShowDialog() == true)
                {
                    using (var stream = new FileStream(dlg.FileName, FileMode.Create))
                    {
                        stream.Write(Value, 0, Value.Length);
                    }
                }
            }
            catch (Exception exception)
            {
                //UnityService.Instance.Resolve<MessageManager>().ShowHandledException(exception,"При сохранении отчета произошло исключение");
            }
        }

        
    }
}