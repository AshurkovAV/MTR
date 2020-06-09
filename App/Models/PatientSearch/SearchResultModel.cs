using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Medical.AppLayer.Models.PatientSearch
{
    public class SearchResultModel
    {
        [Display(Name = @"Id пациента")]
        public int? PatientId { get; set; }

        [Display(Name = @"Данные счета")]
        public string AccountData { get; set; }

        [Display(Name = @"Данные пациента")]
        public string PatientData { get; set; }

        [Display(Name = @"Данные случая")]
        public string MeventData { get; set; }


        [Display(AutoGenerateField = false)]
        public int? EventId { get; set; }

        [Display(AutoGenerateField = false)]
        public int? ExternalId { get; set; }

        [Display(AutoGenerateField = false)]
        public int? AccountId { get; set; }

        [Display(AutoGenerateField = false)]
        public int? MedicalAccountId { get; set; }


        [Display(AutoGenerateField = false)]
        public int? Version { get; set; }

    }
}
