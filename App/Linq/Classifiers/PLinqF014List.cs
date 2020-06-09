using System.Collections.Generic;
using System.Linq;
using DataModel;
using Medical.AppLayer.Models.ClassifierModels;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Linq.Classifiers
{
    public class PLinqF014List : PLinqListBase<EditF014>
    {
        public PLinqF014List(IMedicineRepository repository)
            : base(repository, null)
        {
        }

        public override void GenerateData()
        {
            var result = Repository.GetAll<F014>();
            if (result.Success)
            {
                Data = new List<EditF014>(result.Data.Select(p => new EditF014(p)));
            }
        }
    }
}
