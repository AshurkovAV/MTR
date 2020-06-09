using System.Collections.Generic;
using System.Linq;
using DataModel;
using Medical.AppLayer.Models.ClassifierModels;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Linq.Classifiers
{
    public class PLinqF015List : PLinqListBase<EditF015>
    {
        public PLinqF015List(IMedicineRepository repository)
            : base(repository, null)
        {
        }

        public override void GenerateData()
        {
            var result = Repository.GetAll<F015>();
            if (result.Success)
            {
                Data = new List<EditF015>(result.Data.Select(p => new EditF015(p)));
            }
        }
    }
}
