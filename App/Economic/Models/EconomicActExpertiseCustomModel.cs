using System;
using System.Collections.Generic;
using DataModel;
using Medical.AppLayer.Economic.Models.Helpers;

namespace Medical.AppLayer.Economic.Models
{
    public class EconomicActExpertiseCustomModel : BaseEntity
    {
        public EconomicActExpertiseCustomModel()
        {
        }
        public EconomicActExpertiseCustomModel(FactActExpertise actExpertise)
        {
            ActExpertise = actExpertise;
        }
        public FactActExpertise ActExpertise { get; set; }

        public string NumAct { get; set; }

        public DateTime DateAct { get; set; }

        public string Mo { get; set; }

        public string Smo { get; set; }
        
    }
}
