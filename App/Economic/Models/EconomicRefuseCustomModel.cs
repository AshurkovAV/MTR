using System.Collections.Generic;
using DataModel;
using Medical.AppLayer.Economic.Models.Helpers;

namespace Medical.AppLayer.Economic.Models
{
    public class EconomicRefuseCustomModel
    {
        public FactEconomicRefuse Refuse { get; set; }

        public Dictionary<int, AssistanceSums> AssistanceSum { get; set; }

        public decimal? HospitalAmount
        {
            get { return AssistanceSum.ContainsKey(Globals.Hospital) ? AssistanceSum[Globals.Hospital].Sum : 0m; }
        }

        public decimal? DayHospitalAmount
        {
            get { return AssistanceSum.ContainsKey(Globals.Dayhospital) ? AssistanceSum[Globals.Dayhospital].Sum : 0m; }
        }

        public decimal? PolyclinicAmount
        {
            get { return AssistanceSum.ContainsKey(Globals.Polyclinic) ? AssistanceSum[Globals.Polyclinic].Sum : 0m; }
        }

        public decimal? AmbulanceAmount
        {
            get { return AssistanceSum.ContainsKey(Globals.Ambulance) ? AssistanceSum[Globals.Ambulance].Sum : 0m; }
        }
    }
}
