using System;
using System.ComponentModel.DataAnnotations;
using DataModel;
using Medical.DatabaseCore.Services.Classifiers;
using Medical.AppLayer.Attributes;
using DevExpress.Xpf.Editors;

namespace Medical.AppLayer.Models.ClassifierModels
{
    public class EditV015EqV002 : ClassifierBaseVm<globalV015EqV002>
    {
        public EditV015EqV002(globalV015EqV002 classifier)
            : base(classifier)
        {
            
        }

        public EditV015EqV002()
        {

        }

        [Display(AutoGenerateField = false)]
        public int Id
        {
            get { return Classifier.V015EqV002Id; }
            set { Classifier.V015EqV002Id = value; }
        }

        [Display(Name = "Профиль")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(V015ItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int? Profile_Id
        {
            get { return Classifier.Profile_Id; }
            set { Classifier.Profile_Id = value; }
        }

        [Display(Name = "Специальность")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(V004ItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int? Speciality_Id
        {
            get { return Classifier.Speciality_Id; }
            set { Classifier.Speciality_Id = value; }
        }

    }
}
