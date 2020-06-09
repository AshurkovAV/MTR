using System;

namespace Medical.AppLayer.Economic.Models.Reports
{
    public class ReportReviseBalanceCustomModel
    {
        public int AccountId { get; set; }
        public string AccountNumber { get; set; }
        public DateTime? EconomicDate { get; set; }
        public DateTime? AccountDate { get; set; }
        public decimal? Amount { get; set; }
    }
}
