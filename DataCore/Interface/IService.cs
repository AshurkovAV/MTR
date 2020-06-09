namespace Medical.DataCore.Interface
{
    public interface IService
    {
        string MedicalOrganizationCode { get; set; }
        int? SpecialityCode { get; set; }
    }
}