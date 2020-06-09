using System.Collections.Generic;
using System.Linq;
using DataModel;
using Medical.AppLayer.Models.ClassifierModels;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Linq.Classifiers
{
    public class PLinqIDCToEventTypeList : PLinqListBase<EditIDCToEventType>
    {
        public PLinqIDCToEventTypeList(IMedicineRepository repository)
            : base(repository, null)
        {
        }

        public override void GenerateData()
        {
            var result = Repository.GetAll<globalIDCToEventType>();
            if (result.Success)
            {
                Data = new List<EditIDCToEventType>(result.Data.Select(p => new EditIDCToEventType(p)));
            }
        }
    }
}
