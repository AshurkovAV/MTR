using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DataModel;
using DevExpress.Xpf.Editors;
using Medical.AppLayer.Attributes;
using Medical.DatabaseCore.Services.Classifiers;

namespace Medical.AppLayer.Economic.ViewModels
{
    public class EditActExpertiseViewModel : ClassifierBaseVm<FactActExpertise>
    {
        public EditActExpertiseViewModel(FactActExpertise classifier)
            : base(classifier)
        {
            
        }

        public EditActExpertiseViewModel()
        {

        }

        [Display(Name = @"ID акта")]
        [ReadOnly(true)]
        public int ActExpertiseId
        {
            get { return Classifier.ActExpertiseId; }
            set { Classifier.MedicalAccountId = value; }
        }
        [Display(Name = @"Номер акта")]
        [Required(ErrorMessage = @"Поле 'Номер акта' обязательно для заполнения")]
        public string NumAct
        {
            get { return _classifier.NumAct; }
            set { _classifier.NumAct = value; }
        }
        [Display(Name = @"Дата акта")]
        [Required(ErrorMessage = @"Поле 'Дата акта' обязательно для заполнения")]
        public DateTime? DateAct
        {
            get { return _classifier.DateAct; }
            set { _classifier.DateAct = value; }
        }
        [Display(Name = @"Код МО")]
        [Required(ErrorMessage = @"Поле 'Код МО' обязательно для заполнения")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(F003FullToActItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public string Mo
        {
            get { return _classifier.Mo; }
            set { _classifier.Mo = value; }
        }
        [Display(Name = @"Код СМО")]
        [Required(ErrorMessage = @"Поле 'Код СМО' обязательно для заполнения")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(F002FullToActItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public string Smo
        {
            get { return _classifier.Smo; }
            set { _classifier.Smo = value; }
        }


        [Display(Name = @"Вид котроля")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(VidControltemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int? VidControlId
        {
            get { return _classifier.VidControlId; }
            set { _classifier.VidControlId = value; }
        }

        [Display(Name = @"Вид экспертизы")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(F006ItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int? VidExpertiseId
        {
            get { return _classifier.VidExpertiseId; }
            set { _classifier.VidExpertiseId = value; }
        }

        [Display(Name = @"Дата проведения экспертизы с")]
        public DateTime? DateExpertiseBegin
        {
            get { return _classifier.DateExpertiseBegin; }
            set { _classifier.DateExpertiseBegin = value; }
        }
        [Display(Name = @"Дата проведения экспертизы по")]
        public DateTime? DateExpertiseEnd
        {
            get { return _classifier.DateExpertiseEnd; }
            set { _classifier.DateExpertiseEnd = value; }
        }
        [Display(Name = @"Статус")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(ActExpertiStatusItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int? ActExspertiStatusId
        {
            get { return _classifier.ActExspertiStatusId; }
            set { _classifier.ActExspertiStatusId = value; }
        }

        [Display(Name = @"Счет")]
        [Required(ErrorMessage = @"Счет обязателен для заполнения")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(MedicalAccountItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int? MedicalAccountId
        {
            get { return _classifier.MedicalAccountId; }
            set { _classifier.MedicalAccountId = value; }
        }

    }
}
