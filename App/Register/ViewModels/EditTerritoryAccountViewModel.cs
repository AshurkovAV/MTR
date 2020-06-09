using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DataModel;
using DevExpress.Xpf.Editors;
using Medical.AppLayer.Attributes;
using Medical.DatabaseCore.Services.Classifiers;

namespace Medical.AppLayer.Register.ViewModels
{
    public class EditTerritoryAccountViewModel : ClassifierBaseVm<FactTerritoryAccount>
    {
        public EditTerritoryAccountViewModel(FactTerritoryAccount classifier)
            : base(classifier)
        {
            
        }

        public EditTerritoryAccountViewModel()
        {

        }

        [Display(Name = @"ID счета")]
        [ReadOnly(true)]
        public int TerritoryAccountId
        {
            get { return Classifier.TerritoryAccountId; }
            set { Classifier.TerritoryAccountId = value; }
        }

        [Display(Name = @"Внешний ID счета")]
        [Required(ErrorMessage = @"Поле 'Внешний ID счета' обязательно для заполнения")]
        public int? ExternalAccountId
        {
            get { return Classifier.ExternalId; }
            set { Classifier.ExternalId = value; }
        }

        [Display(Name = @"Код территории выставившей счет")]
        [Required(ErrorMessage = @"Поле 'Код территории выставившей счет' обязательно для заполнения")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(F010ItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public string Source
        {
            get { return _classifier.Source; }
            set { _classifier.Source = value; }
        }

        [Display(Name = @"Код территории страхования")]
        [Required(ErrorMessage = @"Поле 'Код Территория страхования' обязательно для заполнения")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(F010ItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public string Destination
        {
            get { return _classifier.Destination; }
            set { _classifier.Destination = value; }
        }

        [Display(Name = @"Номер счета", Description = "Номер счета выставленного территорией")]
        [Required(ErrorMessage = @"Поле 'Номер счета' обязательно для заполнения")]
        public string AccountNumber
        {
            get { return _classifier.AccountNumber; }
            set { _classifier.AccountNumber = value; }
        }
        [Display(Name = @"Отчетный период", Description = "Отчетный период")]
        [Required(ErrorMessage = @"Поле 'Отчетный период' обязательно для заполнения")]
        public DateTime? Date
        {
            get { return _classifier.Date; }
            set { _classifier.Date = value; }
        }
        [Display(Name = @"Дата выставления счета", Description = "Дата выставления счета территорией")]
        [Required(ErrorMessage = @"Поле 'Дата выставления счета' обязательно для заполнения")]
        public DateTime? AccountDate
        {
            get { return _classifier.AccountDate; }
            set { _classifier.AccountDate = value; }
        }
        [Display(Name = @"Сумма счета", Description = "Сумма счета выставленного территорией")]
        [Required(ErrorMessage = @"Поле 'Сумма счета' обязательно для заполнения")]
        [DataType(DataType.Currency)]
        public decimal? Price
        {
            get { return _classifier.Price; }
            set { _classifier.Price = value; }
        }
        [Display(Name = @"Сумма счета принятая к оплате", Description = "Сумма счета выставленного территорией принятая к оплате")]
        [Required(ErrorMessage = @"Поле 'Сумма счета принятая к оплате' обязательно для заполнения")]
        [DataType(DataType.Currency)]
        public decimal? AcceptPrice
        {
            get { return _classifier.AcceptPrice; }
            set { _classifier.AcceptPrice = value; }
        }
        [Display(Name = @"Версия", Description = "Версия информационного взаимодействия")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(VersionItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int? Version
        {
            get { return _classifier.Version; }
            set { _classifier.Version = value; }
        }
        [Display(Name = @"МЭК", Description = "Сумма штрафных санкций после медико-экономического контроля")]
        [DataType(DataType.Currency)]
        public decimal? MECPenalties
        {
            get { return _classifier.MECPenalties; }
            set { _classifier.MECPenalties = value; }
        }
        [Display(Name = @"МЭЭ", Description = "Сумма штрафных санкций после медико-экономической экспертизы")]
        [DataType(DataType.Currency)]
        public decimal? MEEPenalties
        {
            get { return _classifier.MEEPenalties; }
            set { _classifier.MEEPenalties = value; }
        }
        [Display(Name = @"ЭКМП", Description = "Сумма штрафных санкций после экспертизы качества МП")]
        [DataType(DataType.Currency)]
        public decimal? EQMAPenalties
        {
            get { return _classifier.EQMAPenalties; }
            set { _classifier.EQMAPenalties = value; }
        }
        [Display(Name = @"Комментарий", Description = "Комментарий к счету выставленному территорией")]
        [DataType(DataType.MultilineText)]
        public string Comments
        {
            get { return _classifier.Comments; }
            set { _classifier.Comments = value; }
        }

        [Display(Name = @"Экономическая дата", Description = "Дата получения счета на бумажном носителе")]
        [DataType(DataType.Date)]
        public DateTime? EconomicDate
        {
            get { return _classifier.EconomicDate; }
            set { _classifier.EconomicDate = value; }
        }

        [Display(Name = @"Дата планируемого платежа", Description = "Дата планируемого платежа")]
        [DataType(DataType.Date)]
        public DateTime? PaymentDate
        {
            get { return _classifier.PaymentDate; }
            set { _classifier.PaymentDate = value; }
        }

        [Display(Name = @"Фактически оплаченная сумма")]
        [DataType(DataType.Currency)]
        public decimal? TotalPaymentAmount
        {
            get { return _classifier.TotalPaymentAmount; }
            set { _classifier.TotalPaymentAmount = value; }
        }
    }
}
