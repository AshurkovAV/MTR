using System.Collections.Generic;
using Core.Infrastructure;
using Medical.AppLayer.Examination.ViewModels;

namespace Medical.AppLayer.Services
{
    
    public interface IExaminationService
    {
        OperationResult<IEnumerable<int>> GetExamsIds(int scope, int version, IEnumerable<int?> groups = null);
        OperationResult<IEnumerable<ExamResultModel>> RunExams(int id, int scope, int version, IEnumerable<object> examsList = null);

        OperationResult ApplyExams(int id, int scope, IEnumerable<ExamResultModel> examResultList);
        OperationResult ApplyZslExams(int id, int scope, IEnumerable<ExamResultModel> examResultList);
    }
}
