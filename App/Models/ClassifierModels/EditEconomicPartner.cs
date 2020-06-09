using System.ComponentModel.DataAnnotations;
using DataModel;
using Medical.DatabaseCore.Services.Classifiers;

namespace Medical.AppLayer.Models.ClassifierModels
{
    public class EditEconomicPartner : ClassifierBaseVm<EconomicPartner>
    {
        public EditEconomicPartner(EconomicPartner classifier)
            : base(classifier)
        {
            
        }

        public EditEconomicPartner()
        {

        }

        [Display(AutoGenerateField = false)]
        public int EconomicPartnerId
        {
            get { return Classifier.EconomicPartnerId; }
            set { Classifier.EconomicPartnerId = value; }
        } 

        [Display(Name = "Краткое название")]
        [StringLength(250)]
        public string Name
        {
            get { return Classifier.Name; }
            set { Classifier.Name = value; }
        }
        [Display(Name = "Полный адрес")]
        [StringLength(500)]
        public string Adr
        {
            get { return Classifier.Adr; }
            set { Classifier.Adr = value; }
        }
        [Display(Name = "Банк")]
        [StringLength(100)]
        public string Bank
        {
            get { return Classifier.Bank; }
            set { Classifier.Bank = value; }
        }
        [Display(Name = "Расчетный счет")]
        public decimal? Rs
        {
            get { return Classifier.Rs; }
            set { Classifier.Rs = value; }
        }
        
        [Display(Name = "БИК")]
        [StringLength(9)]
        public string Bic
        {
            get { return Classifier.Bic; }
            set { Classifier.Bic = value; }
        }
        [Display(Name = "ИНН")]
        [StringLength(10)]
        public string Inn
        {
            get { return Classifier.Inn; }
            set { Classifier.Inn = value; }
        }
        [Display(Name = "КПП")]
        [StringLength(9)]
        public string Kpp
        {
            get { return Classifier.Kpp; }
            set { Classifier.Kpp = value; }
        }
        [Display(Name = "КБК")]
        public decimal? Kbk
        {
            get { return Classifier.Kbk; }
            set { Classifier.Kbk = value; }
        }
        [Display(Name = "ОКТМО")]
        public string Oktmo
        {
            get { return Classifier.Oktmo; }
            set { Classifier.Oktmo = value; }
        }
        [Display(Name = "ОКАТО")]
        public string Okato
        {
            get { return Classifier.Okato; }
            set { Classifier.Okato = value; }
        }
        [Display(Name = "ОГРН")]
        public string Ogrn
        {
            get { return Classifier.Ogrn; }
            set { Classifier.Ogrn = value; }
        }
    }
}
