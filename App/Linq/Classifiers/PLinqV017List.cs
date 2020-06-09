using System.Collections.Generic;
using System.Linq;
using DataModel;
using Medical.AppLayer.Models.ClassifierModels;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Linq.Classifiers
{
    public class PLinqV017List : PLinqListBase<EditV017>
    {
        public PLinqV017List(IMedicineRepository repository)
            : base(repository, null)
        {
        }

        public override void GenerateData()
        {
            var result = Repository.GetAll<V017>();
            if (result.Success)
            {
                Data = new List<EditV017>(result.Data.Select(p => new EditV017(p)));
            }
        }
    }
}
