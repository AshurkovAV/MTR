using System;
using System.Collections.Generic;
using Core.Infrastructure;
using DataModel;
using Medical.AppLayer.Economic.Models.Reports;
using Medical.DataCore.Interface;
using Medical.DataCore.v10PL.PL;
using Medical.DataCore.v21K2.D;
using v30K1 = Medical.DataCore.v30K1;
using v31K1 = Medical.DataCore.v31K1;
using v32K1 = Medical.DataCore.v32K1;

namespace Medical.AppLayer.Services
{
    public interface IComplexReportRepository
    {
        OperationResult<Tuple<List<ReportForm2CustomModel>,int>> CreateForm2Report(DateTime beginDate, DateTime endDate, int direction);
        OperationResult<Tuple<List<ReportReviseCustomModel>, List<ReportReviseCustomModel>, localF001, localF001>> CreateActReviseReport(DateTime beginDate, DateTime endDate, string territory);
        OperationResult<string> TerritoryAccountSummary(int accountId, int version);
        OperationResult<string> MedicalAccountSummary(int medicalAccountId, int version);
        OperationResult<string> DoRegisterEStats<T>(T data) where T : IRegister;
        OperationResult<string> DoZRegisterEStats<T>(T data) where T : IZRegister;
        OperationResult<string> DoRegisterEAnswerStats<T>(T data) where T : IRegisterAnswer;
        OperationResult<string> DoZRegisterEAnswerStats<T>(T data) where T : IZRegisterAnswer;
        OperationResult<string> DoRegisterPlAnswerStats<T>(T data) where T : IRegisterPlAnswer;
        OperationResult<string> DoRegisterDLoadStats(AccountRegisterD data);
        OperationResult<string> DoRegisterDLoadStats(v31K1.D.AccountRegisterD data);
        OperationResult<string> DoRegisterDLoadStats(v32K1.D.AccountRegisterD data);
        OperationResult<string> DoRegisterDLoadStats(v31K1.DV.AccountRegisterD data);
        OperationResult<string> DoRegisterDLoadStats(v30K1.D.AccountRegisterD data);
        OperationResult<string> DoRegisterELoadStats(IRegister data);
        OperationResult<string> DoRegisterELoadStats(IZRegister data);
        OperationResult<string> DoRegisterELoadStats(IRegisterAnswer data);
        OperationResult<string> DoRegisterEDbStats(int id);
        OperationResult<string> DoZRegisterEDbStats(int id);
        OperationResult<string> DoRegisterELoadStats(IZRegisterAnswer data);
    }
}