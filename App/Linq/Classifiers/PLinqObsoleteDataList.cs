using System.Collections.Generic;
using System.Linq;
using DataModel;
using Medical.AppLayer.Models.ClassifierModels;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Linq.Classifiers
{
    public class PLinqObsoleteDataList : PLinqListBase<EditObsoleteData>
    {
        public PLinqObsoleteDataList(IMedicineRepository repository)
            : base(repository, null)
        {
        }

        public override void GenerateData()
        {
            var result = Repository.GetAll<globalObsoleteData>();
            if (result.Success)
            {
                Data = new List<EditObsoleteData>(result.Data.Select(p => new EditObsoleteData(p)));
            }
        }
    }
}
