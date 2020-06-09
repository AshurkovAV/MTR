using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DataModel;
using DevExpress.Xpf.Editors;
using Medical.AppLayer.Attributes;
using Medical.DatabaseCore.Services.Classifiers;

namespace Medical.AppLayer.Register.ViewModels
{
    public class EditMedicalAccountViewModel : ClassifierBaseVm<FactMedicalAccount>
    {
        public EditMedicalAccountViewModel(FactMedicalAccount classifier)
            : base(classifier)
        {
            
        }

        public EditMedicalAccountViewModel()
        {

        }

        [Display(Name = @"ID счета")]
        [ReadOnly(true)]
        public int TerritoryAccountId
        {
            get { return Classifier.MedicalAccountId; }
            set { Classifier.MedicalAccountId = value; }
        }

        [Display(Name = @"Код МО")]
        [Required(ErrorMessage = @"Поле 'Код МО' обязательно для заполнения")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(F003ItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int? MedicalOrganization
        {
            get { return _classifier.MedicalOrganization; }
            set { _classifier.MedicalOrganization = value; }
        }

        [Display(Name = @"Номер счета")]
        [Required(ErrorMessage = @"Поле 'Номер счета' обязательно для заполнения")]
        public string AccountNumber
        {
            get { return _classifier.AccountNumber; }
            set { _classifier.AccountNumber = value; }
        }

        [Display(Name = @"Отчетный период")]
        [Required(ErrorMessage = @"Поле 'Отчетный период' обязательно для заполнения")]
        public DateTime? Date
        {
            get { return _classifier.Date; }
            set { _classifier.Date = value; }
        }

        [Display(Name = @"Дата выставления счета")]
        [Required(ErrorMessage = @"Поле 'Дата выставления счета' обязательно для заполнения")]
        public DateTime? AccountDate
        {
            get { return _classifier.AccountDate; }
            set { _classifier.AccountDate = value; }
        }

        [Display(Name = @"Сумма счета"), DataType(DataType.Currency)]
        //[Required(ErrorMessage = @"Поле 'Сумма счета' обязательно для заполнения")]
        public decimal? Price
        {
            get { return _classifier.Price; }
            set { _classifier.Price = value; }
        }

        [Display(Name = @"Сумма счета принятая к оплате"), DataType(DataType.Currency)]
        public decimal? AcceptPrice
        {
            get { return _classifier.AcceptPrice; }
            set { _classifier.AcceptPrice = value; }
        }

        [Display(Name = @"МЭК"), DataType(DataType.Currency)]
        public decimal? MECPenalties
        {
            get { return _classifier.MECPenalties; }
            set { _classifier.MECPenalties = value; }
        }

        [Display(Name = @"МЭЭ"), DataType(DataType.Currency)]
        public decimal? MEEPenalties
        {
            get { return _classifier.MEEPenalties; }
            set { _classifier.MEEPenalties = value; }
        }

        [Display(Name = @"ЭКМП"), DataType(DataType.Currency)]
        public decimal? EQMAPenalties
        {
            get { return _classifier.EQMAPenalties; }
            set { _classifier.EQMAPenalties = value; }
        }

        [Display(Name = @"Версия счета")]
        [Required(ErrorMessage = @"Версия счета обязательна для заполнения")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(VersionItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int? Version
        {
            get { return _classifier.Version; }
            set { _classifier.Version = value; }
        }

        [Display(Name = @"Комментарий"), DataType(DataType.MultilineText)]
        public string Comments
        {
            get { return _classifier.Comments; }
            set { _classifier.Comments = value; }
        }



        [Display(Name = @"Статус")]
        public int? Status
        {
            get { return _classifier.Status; }
            set { _classifier.Status = value; }
        }

    }
}
