using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Core.Extensions;
using DevExpress.Xpf.Editors;
using FastReport;
using FastReport.Design;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.AppLayer.Report.Helpers;
using Microsoft.Win32;

namespace Medical.AppLayer.Editors
{
    public class ReportControl2 : ButtonEdit
    {
        private readonly EnvironmentSettings _environmentSettings;
        public ReportControl2()
        {
            AllowDefaultButton = false;
            MaxHeight = 50;
            MinHeight = 50;
            AcceptsReturn = true;
            AcceptsTab = true;
            TextWrapping = TextWrapping.Wrap;
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

            Buttons.Add(new ButtonInfo { Name = "LoadReportFromFile", ToolTip = "Загрузить отчет из файла", GlyphKind = GlyphKind.Regular, Command = new RelayCommand(LoadReport) });
            Buttons.Add(new ButtonInfo { Name = "SaveReportToFile", ToolTip = "Сохранить отчет в файл", GlyphKind = GlyphKind.Down, Command = new RelayCommand(SaveReportFile) });
            Buttons.Add(new ButtonInfo { Name = "ViewReport", ToolTip = "Просмотр отчета", GlyphKind = GlyphKind.Search, Command = new RelayCommand(PreviewReport) });
            Buttons.Add(new ButtonInfo { Name = "EditReport", ToolTip = "Редактировать отчет", GlyphKind = GlyphKind.Edit, Command = new RelayCommand(ShowDesigner) });

            _environmentSettings = new EnvironmentSettings();
            _environmentSettings.CustomSaveReport += CustomSaveReport;
        }

        private void LoadReport()
        {
            try
            {
                var dlg = new OpenFileDialog { DefaultExt = ".frx", Filter = "Файлы отчетов FastReport .NET (.frx)|*.frx" };

                if (dlg.ShowDialog() == true)
                {
                    using (var stream = new FileStream(dlg.FileName, FileMode.Open))
                    {
                        using (var memory = new MemoryStream())
                        {
                            stream.CopyTo(memory);
                            var buffer = memory.GetBuffer();
                            if (buffer != null)
                            {
                                EditValue = Encoding.UTF8.GetString(buffer);
                            }
                            
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                //UnityService.Instance.Resolve<MessageManager>().ShowHandledException(exception,"При конвертиции файла отчетов произошло исключение");
            }
        }

        private void CustomSaveReport(object sender, OpenSaveReportEventArgs e)
        {
            try
            {
                var tmp = new MemoryStream();
                e.Report.Save(tmp);
                e.Report.Save(e.FileName);
                var buffer = tmp.GetBuffer();
                if (buffer != null)
                {
                    EditValue = Encoding.UTF8.GetString(buffer);
                }
            }
            catch (Exception exception)
            {
                //UnityService.Instance.Resolve<MessageManager>().ShowHandledException(exception,"При сохранении отчета произошло исключение");
            }
        }

        private void ShowDesigner()
        {
            try
            {
                var report = new FastReport.Report();
                if (EditValue == null || EditValue.ToString().IsNullOrWhiteSpace())
                {
                    report.Load(ReportTemplate.GetDefaultReport());
                }
                else
                {
                    report.Load(new MemoryStream(Encoding.UTF8.GetBytes(EditValue.ToString())));
                }

                report.Design();
                report.Dispose();
            }
            catch (Exception exception)
            {
                // UnityService.Instance.Resolve<MessageManager>().ShowHandledException(exception,"При вызове дизайнера отчетов произошло исключение");
            }
        }

        private void PreviewReport()
        {
            try
            {
                if (EditValue != null && EditValue.ToString().IsNotNullOrWhiteSpace())
                {
                    var report = new FastReport.Report();
                    report.Load(new MemoryStream(Encoding.UTF8.GetBytes(EditValue.ToString())));
                    report.Show();
                    report.Dispose();
                }
                
            }
            catch (Exception exception)
            {
                //UnityService.Instance.Resolve<MessageManager>().ShowHandledException(exception,"При предпросмотре отчета произошло исключение");
            }
        }

        private void SaveReportFile()
        {
            try
            {
                if (EditValue != null && EditValue.ToString().IsNotNullOrWhiteSpace())
                {
                    var dlg = new SaveFileDialog { DefaultExt = ".frx", Filter = "Файлы отчетов FastReport .NET (.frx)|*.frx" };

                    if (dlg.ShowDialog() == true)
                    {
                        using (var stream = new FileStream(dlg.FileName, FileMode.Create))
                        {
                            var tmp = Encoding.UTF8.GetBytes(EditValue.ToString());
                            stream.Write(tmp, 0, tmp.Length);
                        }
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
