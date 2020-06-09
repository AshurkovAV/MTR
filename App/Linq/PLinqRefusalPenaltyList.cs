using System.Collections.Generic;
using System.Linq;
using DataModel;
using Medical.AppLayer.Models.ClassifierModels;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Linq
{
    public class PLinqRefusalPenaltyList : PLinqListBase<EditRefusalPenalty>
    {
        public PLinqRefusalPenaltyList(IMedicineRepository repository)
            : base(repository, null)
        {
        }

        public override void GenerateData()
        {
            var result = Repository.GetAll<globalRefusalPenalty>();
            if (result.Success)
            {
                Data = new List<EditRefusalPenalty>(result.Data.Select(p => new EditRefusalPenalty(p)));
            }
        }
    }
}
