using System.Collections.Generic;
using System.Linq;
using DataModel;
using Medical.AppLayer.Models.ClassifierModels;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Linq.Classifiers
{
    public class PLinqClinicalExaminationList : PLinqListBase<EditClinicalExamination>
    {
        public PLinqClinicalExaminationList(IMedicineRepository repository)
            : base(repository, null)
        {
        }

        public override void GenerateData()
        {
            var result = Repository.GetAll<globalClinicalExamination>();
            if (result.Success)
            {
                Data = new List<EditClinicalExamination>(result.Data.Select(p => new EditClinicalExamination(p)));
            }
        }
    }
}
