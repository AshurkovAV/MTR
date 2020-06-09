using System.Collections.Generic;
using System.Linq;
using DataModel;
using Medical.AppLayer.Models.ClassifierModels;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Linq.Classifiers
{
    public class PLinqEconomicPartnerList : PLinqListBase<EditEconomicPartner>
    {
        public PLinqEconomicPartnerList(IMedicineRepository repository)
            : base(repository, null)
        {
        }

        public override void GenerateData()
        {
            var result = Repository.GetAll<EconomicPartner>();
            if (result.Success)
            {
                Data = new List<EditEconomicPartner>(result.Data.Select(p => new EditEconomicPartner(p)));
            }
        }
    }
}
