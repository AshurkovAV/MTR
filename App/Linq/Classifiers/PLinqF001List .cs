using System.Collections.Generic;
using System.Linq;
using DataModel;
using Medical.AppLayer.Models.ClassifierModels;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Linq.Classifiers
{
    public class PLinqF001List : PLinqListBase<EditF001>
    {
        public PLinqF001List(IMedicineRepository repository)
            : base(repository, null)
        {
        }

        public override void GenerateData()
        {
            var result = Repository.GetAll<F001>();
            if (result.Success)
            {
                Data = new List<EditF001>(result.Data.Select(p => new EditF001(p)));
            }
        }
    }
}
