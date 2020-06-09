using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Core.Extensions;
using Core.Helpers;
using Core.Infrastructure;
using Core.Services;
using Medical.CoreLayer.Service;
using Medical.DatabaseCore.Services.Cache;
using Medical.DatabaseCore.Services.Database;
using Microsoft.Win32;

namespace Medical.AppLayer.Services
{
    public class ReportService : IReportService
    {
        private readonly IMedicineRepository _repository;
        private readonly ICacheRepository _cache;
        private readonly IMessageService _messageService;

        private readonly dynamic _databaseSettings;

        private readonly int _userId;

        public ReportService(IMedicineRepository repository, 
            IAppShareSettings shareSettings, 
            ICacheRepository cache, 
            IMessageService messageService,
            IUserService userService)
        {
            _repository = repository;
            _cache = cache;
            _databaseSettings = shareSettings.Get("database");
            _messageService = messageService;
            _userId = userService.UserId;
        }

        public OperationResult PrepareReports(IEnumerable<object> reportsList, int employeeId, int id, int scope, int? subId = null, bool isPreview = false)
        {
            var result = new OperationResult();
            try
            {
                var report = new FastReport.Report();

                foreach (object o in reportsList)
                {
                    int? reportId = SafeConvert.ToInt32(o.ToString());
                    if (!reportId.HasValue)
                    {
                        throw new InvalidOperationException("Неверный ID={0} отчета".F(o));
                    }
                    var reportResult = _repository.GetReportById(reportId.Value);
                    if (reportResult.Success)
                    {
                        if (reportResult.Data == null || reportResult.Data.Body == null)
                        {
                            throw new InvalidOperationException("В базе отсутствует отчет ID={0} или отсутствуют данные".F(reportId));
                        }

                        report.Load(new MemoryStream(reportResult.Data.Body));
                        report.SetParameterValue(_cache.Get("ParamCache").GetString(scope), id);
                        //TODO Устаревший параметр, только для обратной совместимости отчетов, используйте UserId и таблицу localUser
                        report.SetParameterValue("EmployeeId", employeeId);
                        report.SetParameterValue("Scope", scope);
                        report.SetParameterValue("ReportId", reportResult.Data.FactReportID);
                        report.SetParameterValue("UserId", _userId);
                        report.Dictionary.Connections[0].ConnectionString = (string)_databaseSettings.ConnectionString;
                        var preparedReportResult = _repository.InsertPreparedReport(id, scope, reportResult.Data.FactReportID, reportResult.Data.Name, subId);
                        if (!preparedReportResult.Success)
                        {
                            throw new InvalidOperationException("Ошибка при записи готового отчета, отчет ID={0}".F(reportId));
                        }

                        report.Prepare();
                        var pageCount = report.PreparedPages.Count;
                        var memory = new MemoryStream();
                        report.SavePrepared(memory);
                        var updatePreparedReportResult = _repository.UpdatePreparedReport(preparedReportResult.Id, memory.GetBuffer(), pageCount);
                        if (!updatePreparedReportResult.Success)
                        {
                            throw new InvalidOperationException("Ошибка при обновлении готового отчета ID={0}".F(preparedReportResult.Id));
                        }

                        if (isPreview)
                        {
                            report.ShowPrepared();
                        }
                    }

                }
                report.Dispose();
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public OperationResult PrintReports(IEnumerable<object> reportList, bool? isPreview)
        {

            var result = new OperationResult();
            try
            {
                var report = new FastReport.Report();
                var report2 = new FastReport.Report();
                report2.Prepare();

                int count = 0;

                foreach (var o in reportList)
                {
                    int? reportId = SafeConvert.ToInt32(o.ToString());
                    if (!reportId.HasValue)
                    {
                        throw new InvalidOperationException("Неверный ID={0} отчета".F(o));
                    }
                    var reportResult = _repository.GetPreparedReportById(reportId.Value);
                    if (reportResult.Success)
                    {
                        if (reportResult.Data == null || reportResult.Data.Body == null)
                        {
                            throw new InvalidOperationException("В базе отсутствует отчет ID={0} или отсутствуют данные".F(reportId));
                        }

                        report.LoadPrepared(new MemoryStream(reportResult.Data.Body));
                        for (int i = 0; i < report.PreparedPages.Count; i++)
                        {
                            var page = report.PreparedPages.GetPage(i);
                            report2.PreparedPages.AddSourcePage(page);
                            report2.PreparedPages.AddPage(page);
                            report2.PreparedPages.ModifyPage(count++, page);
                        }
                    }
                }

                if (isPreview == true)
                {
                    report2.ShowPrepared();
                }
                else
                {
                    report2.PrintPrepared();
                }

                report.Dispose();
                report2.Dispose();

            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result; 
        }

        public OperationResult PrintReports(int id, int scope, int? subId = null, bool? isPreview = false)
        {
            var reports = _repository.GetPreparedReportByExternalIdScopeSubId(id, scope, subId);
            if (reports.Success)
            {
                var reportsIds = new List<object>(reports.Data.Select(p => p.PreparedReportId as object));
                return PrintReports(reportsIds, isPreview);
            }

            var result =  new OperationResult();
            result.AddError(reports.LastError);
            return result;
        }

        public void RunReportDesigner()
        {
            try
            {
                var memory = new MemoryStream();
                var template = new XElement("Report",
                        new XAttribute("ScriptLanguage", "CSharp"),
                        new XAttribute("ReportInfo.Created", DateTime.Now),
                        new XAttribute("ReportInfo.Modified", DateTime.Now),
                        new XAttribute("CreatorVersion", "1.8.1.0"),
                        new XElement("Dictionary",
                            new XElement("MsSqlDataConnection",
                                new XAttribute("Name", "Connection"),
                                new XAttribute("ConnectionString", "rijcmlqM3/HbiZANEYP3Y6oNtE7mrUmT0ytl94C6DYS4DXvgc8JqhVjVQGe7Sc2pOMNI3eDZu4y6+BLf6/PCErp5V5iYO+NhUsLGHVpiKUoEc7fdRAKM3dC4G3zKvHq63vBaaJuNWt1RaEUDxyLLh6IMbjf+Iqjj5s+Su4iyajqSNb5u2mTlxcVInKupOBFFxa/QOyK")
                                ),
                            new XElement("Parameter",
                                new XAttribute("Name", "MedicalAccountId"),
                                new XAttribute("DataType", "System.Int32")
                            )),
                        new XElement("ReportPage",
                            new XAttribute("Name", "Page1"),
                            new XAttribute("Landscape", "true"),
                            new XAttribute("PaperWidth", "297"),
                            new XAttribute("PaperHeight", "210"),
                            new XAttribute("RawPaperSize", "9"),
                            new XElement("ReportTitleBand",
                                new XAttribute("Name", "ReportTitle1"),
                                new XAttribute("Width", "1047.06"),
                                new XAttribute("Height", "50.00"))));

                var templateReport = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), template);
                templateReport.Save(memory);
                var report = new FastReport.Report();
                memory.Position = 0;
                report.Load(memory);
                report.Design();
                report.Dispose();
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "Исключение при запуске дизайнера отчетов",typeof(ReportService));

            }
        }

        public void RunReportViewer()
        {
            try
            {
                var dlg = new OpenFileDialog
                {
                    DefaultExt = ".frx",
                    Filter = "Файлы отчетов FastReport .NET (.frx)|*.frx"
                };

                bool? result = dlg.ShowDialog();

                if (result == true)
                {
                    var report = new FastReport.Report();
                    report.Load(dlg.FileName);
                    if (report.Prepare())
                    {
                        report.ShowPrepared(true);
                    }
                    report.Dispose();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "Исключение при запуске просмотра отчетов", typeof(ReportService));
            }
        }
    }
}