using System;
using System.ComponentModel.DataAnnotations;
using DataModel;
using Medical.DatabaseCore.Services.Classifiers;
using Medical.AppLayer.Attributes;
using DevExpress.Xpf.Editors;

namespace Medical.AppLayer.Models.ClassifierModels
{
    public class EditClinicalExamination : ClassifierBaseVm<globalClinicalExamination>
    {
        public EditClinicalExamination(globalClinicalExamination classifier)
            : base(classifier)
        {
            
        }

        public EditClinicalExamination()
        {

        }

        [Display(AutoGenerateField = false)]
        public int Id
        {
            get { return Classifier.ClinicalExaminationId; }
            set { Classifier.ClinicalExaminationId = value; }
        }

        [Display(Name = "Код услуги")]
        public string ServiceCode
        {
            get { return Classifier.ServiceCode; }
            set { Classifier.ServiceCode = value; }
        }

        [Display(Name = "Профиль")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(V002ItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int ProfileId
        {
            get { return Classifier.ProfileId; }
            set { Classifier.ProfileId = value; }
        }

        [Display(Name = "Специальность")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(V004ItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int SpecialityV004Id
        {
            get { return Classifier.SpecialityId; }
            set { Classifier.SpecialityId = value; }
        }

        [Display(Name = "Возраст")]
        public int Age
        {
            get { return Classifier.Age; }
            set { Classifier.Age = value; }
        }

        [Display(Name = "Возраст в месяцах")]
        public int AgeMonths
        {
            get { return Classifier.AgeMonths; }
            set { Classifier.AgeMonths = value; }
        }

        [Display(Name = "Пол")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(V005ItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int Sex
        {
            get { return Classifier.Sex; }
            set { Classifier.Sex = value; }
        }

        [Display(Name = "Детский профиль")]
        public bool? IsChildren
        {
            get { return Classifier.IsChildren; }
            set { Classifier.IsChildren = value; }
        }

        [Display(Name = "Количество услуг")]
        public int Quantity
        {
            get { return Classifier.Quantity; }
            set { Classifier.Quantity = value; }
        }

    }
}
