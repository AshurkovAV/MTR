using System.Collections.Generic;
using System.Linq;
using DataModel;
using Medical.AppLayer.Models.ClassifierModels;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Linq.Classifiers
{
    public class PLinqF004List : PLinqListBase<EditF004>
    {
        public PLinqF004List(IMedicineRepository repository)
            : base(repository, null)
        {
        }

        public override void GenerateData()
        {
            var result = Repository.GetAll<F004>();
            if (result.Success)
            {
                Data = new List<EditF004>(result.Data.Select(p => new EditF004(p)));
            }
        }
    }
}
