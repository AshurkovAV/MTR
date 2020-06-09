using System.Collections.Generic;
using Core.Infrastructure;

namespace Medical.AppLayer.Services
{
    public interface IReportService
    {
        OperationResult PrepareReports(IEnumerable<object> reportsList, int userId, int id, int scope, int? subId = null, bool isPreview = false);
        OperationResult PrintReports(IEnumerable<object> reportList, bool? isPreview = false);
        OperationResult PrintReports(int id, int scope, int? subId = null, bool? isPreview = false);
        void RunReportDesigner();
        void RunReportViewer();
    }
}
