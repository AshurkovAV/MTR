using System.Collections.Generic;
using System.Linq;
using DataModel;
using Medical.AppLayer.Models.ClassifierModels;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Linq.Classifiers
{
    public class PLinqLocalF001List : PLinqListBase<EditLocalF001>
    {
        public PLinqLocalF001List(IMedicineRepository repository)
            : base(repository, null)
        {
        }

        public override void GenerateData()
        {
            var result = Repository.GetAll<localF001>();
            if (result.Success)
            {
                Data = new List<EditLocalF001>(result.Data.Select(p => new EditLocalF001(p)));
            }
        }
    }
}
