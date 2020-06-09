using System.Collections.Generic;
using System.Linq;
using DataModel;
using Medical.AppLayer.Models.ClassifierModels;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Linq.Classifiers
{
    public class PLinqLocalF003List : PLinqListBase<EditLocalF003>
    {
        public PLinqLocalF003List(IMedicineRepository repository)
            : base(repository, null)
        {
        }

        public override void GenerateData()
        {
            var result = Repository.GetAll<localF003>();
            if (result.Success)
            {
                Data = new List<EditLocalF003>(result.Data.Select(p => new EditLocalF003(p)));
            }
        }
    }
}
