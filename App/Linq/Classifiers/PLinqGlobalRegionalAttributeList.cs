using System.Collections.Generic;
using System.Linq;
using DataModel;
using Medical.AppLayer.Models.ClassifierModels;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Linq.Classifiers
{
    public class PLinqGlobalRegionalAttributeList : PLinqListBase<EditRegionalAttribute>
    {
        public PLinqGlobalRegionalAttributeList(IMedicineRepository repository)
            : base(repository, null)
        {
        }

        public override void GenerateData()
        {
            var result = Repository.GetAll<globalRegionalAttribute>();
            if (result.Success)
            {
                Data = new List<EditRegionalAttribute>(result.Data.Select(p => new EditRegionalAttribute(p)));
            }
        }
    }
}
