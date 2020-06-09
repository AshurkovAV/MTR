namespace Medical.DataCore.Interface
{
    public interface IPatientAnswer
    {
        string INP { get; set; }
        string InsuranceDocNumber { get; set; }
        string InsuranceDocSeries { get; set; }
        int? InsuranceDocType { get; set; }

        string PolicyNumber { get; set; }
    }
}