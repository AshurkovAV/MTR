using System.Collections.Generic;
using DataModel;
using Medical.AppLayer.Economic.Models.Helpers;

namespace Medical.AppLayer.Economic.Models
{
    public class EconomicJournalCustomModel
    {

        public FactEconomicAccount Account { get; set; }
        public FactTerritoryAccount TerritoryAccount { get; set; }

        public Dictionary<int, AssistanceDetailsSums> AssistanceDetailsSum { get; set; }

        public decimal? HospitalAmountPayable
        {
            get { return AssistanceDetailsSum.ContainsKey(Globals.Hospital) ? AssistanceDetailsSum[Globals.Hospital].AmountPayable : 0m; }
        }

        public decimal? HospitalAmountRefuse
        {
            get { return AssistanceDetailsSum.ContainsKey(Globals.Hospital) ? AssistanceDetailsSum[Globals.Hospital].AmountRefuse : 0m; }
        }

        public decimal? HospitalAmountFact
        {
            get { return AssistanceDetailsSum.ContainsKey(Globals.Hospital) ? AssistanceDetailsSum[Globals.Hospital].AmountFact : 0m; }
        }

        public decimal? DayHospitalAmountPayable
        {
            get { return AssistanceDetailsSum.ContainsKey(Globals.Dayhospital) ? AssistanceDetailsSum[Globals.Dayhospital].AmountPayable : 0m; }
        }

        public decimal? DayHospitalAmountRefuse
        {
            get { return AssistanceDetailsSum.ContainsKey(Globals.Dayhospital) ? AssistanceDetailsSum[Globals.Dayhospital].AmountRefuse : 0m; }
        }

        public decimal? DayHospitalAmountFact
        {
            get { return AssistanceDetailsSum.ContainsKey(Globals.Dayhospital) ? AssistanceDetailsSum[Globals.Dayhospital].AmountFact : 0m; }
        }

        public decimal? PolyclinicAmountPayable
        {
            get { return AssistanceDetailsSum.ContainsKey(Globals.Polyclinic) ? AssistanceDetailsSum[Globals.Polyclinic].AmountPayable : 0m; }
        }

        public decimal? PolyclinicAmountRefuse
        {
            get { return AssistanceDetailsSum.ContainsKey(Globals.Polyclinic) ? AssistanceDetailsSum[Globals.Polyclinic].AmountRefuse : 0m; }
        }

        public decimal? PolyclinicAmountFact
        {
            get { return AssistanceDetailsSum.ContainsKey(Globals.Polyclinic) ? AssistanceDetailsSum[Globals.Polyclinic].AmountFact : 0m; }
        }

        public decimal? AmbulanceAmountPayable
        {
            get { return AssistanceDetailsSum.ContainsKey(Globals.Ambulance) ? AssistanceDetailsSum[Globals.Ambulance].AmountPayable : 0m; }
        }

        public decimal? AmbulanceAmountRefuse
        {
            get { return AssistanceDetailsSum.ContainsKey(Globals.Ambulance) ? AssistanceDetailsSum[Globals.Ambulance].AmountRefuse : 0m; }
        }

        public decimal? AmbulanceAmountFact
        {
            get { return AssistanceDetailsSum.ContainsKey(Globals.Ambulance) ? AssistanceDetailsSum[Globals.Ambulance].AmountFact : 0m; }
        }

        public decimal? TotalAmountPayable { get; set; }
        public decimal? TotalAmountRefuse { get; set; }
    }

}
