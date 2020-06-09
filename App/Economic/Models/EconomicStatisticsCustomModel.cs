using DataModel;

namespace Medical.AppLayer.Economic.Models
{
    public class EconomicStatisticsCustomModel
    {
        public FactEconomicAccount Account { get; set; }
        public FactTerritoryAccount TerritoryAccount { get; set; }
        public FactEconomicPaymentDetail Details { get; set; }
        public decimal AmountSurcharge { get; set; }
        public decimal AmountRefuse { get; set; }
        public decimal AmountFact { get; set; }
    }

    
}
