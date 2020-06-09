using System;
using System.ComponentModel.DataAnnotations;
using DataModel;
using DevExpress.Xpf.Editors;
using Medical.AppLayer.Attributes;
using Medical.DatabaseCore.Services.Classifiers;

namespace Medical.AppLayer.Models.ClassifierModels
{
    public class EditShareDoctor : ClassifierBaseVm<shareDoctor>
    {
        public EditShareDoctor(shareDoctor classifier)
            : base(classifier)
        {
            
        }

        public EditShareDoctor()
        {

        }

        [Display(AutoGenerateField = false)]
        public int DoctorId
        {
            get { return Classifier.DoctorId; }
            set { Classifier.DoctorId = value; }
        } 

        [Display(Name = "Код")]
        public string Code {
            get { return Classifier.Code; }
            set { Classifier.Code = value; }
        } 

        [Display(Name = "Имя")]
        public string DName
        {
            get { return Classifier.DName; }
            set { Classifier.DName = value; }
        } 

        [Display(Name = "Фамилия")]
        public string Surname
        {
            get { return Classifier.Surname; }
            set { Classifier.Surname = value; }
        } 

        [Display(Name = "Отчество")]
        public string Patronymic
        {
            get { return Classifier.Patronymic; }
            set { Classifier.Patronymic = value; }
        } 

        [Display(Name = "Дата рождения")]
        public DateTime? Birthday
        {
            get { return Classifier.Birthday; }
            set { Classifier.Birthday = value; }
        } 

        [Display(Name = "Код МО")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(F003CodeItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public string MedicalOrganizationCode
        {
            get { return Classifier.MedicalOrganizationCode; }
            set { Classifier.MedicalOrganizationCode = value; }
        } 
    }
}
