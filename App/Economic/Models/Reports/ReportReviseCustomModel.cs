namespace Medical.AppLayer.Economic.Models.Reports
{
    public class ReportReviseCustomModel
    {
        public decimal? AmountPayable { get; set; }
        public decimal? AmountFact { get; set; }
        public decimal? AmountRefuse { get; set; }

        public ReportReviseBalanceCustomModel CurrentBalance { get; set; }
        public ReportReviseBalanceCustomModel BeginBalance { get; set; }
        public ReportReviseBalanceCustomModel EndBalance { get; set; }
    }
}
