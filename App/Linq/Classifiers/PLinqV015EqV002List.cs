using System.Collections.Generic;
using System.Linq;
using DataModel;
using Medical.AppLayer.Models.ClassifierModels;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Linq.Classifiers
{
    public class PLinqV015EqV002List : PLinqListBase<EditV015EqV002>
    {
        public PLinqV015EqV002List(IMedicineRepository repository)
            : base(repository, null)
        {
        }

        public override void GenerateData()
        {
            var result = Repository.GetAll<globalV015EqV002>();
            if (result.Success)
            {
                Data = new List<EditV015EqV002>(result.Data.Select(p => new EditV015EqV002(p)));
            }
        }
    }
}
