using System;
using System.ComponentModel.DataAnnotations;
using DataModel;
using Medical.DatabaseCore.Services.Classifiers;
using Medical.AppLayer.Attributes;
using DevExpress.Xpf.Editors;
using System.Collections.Generic;

namespace Medical.AppLayer.Models.ClassifierModels
{
    public class EditLicenseEntry : ClassifierBaseVm<globalLicenseEntry>
    {
        public EditLicenseEntry(globalLicenseEntry classifier)
            : base(classifier)
        {
        }

        public EditLicenseEntry()
        {

        }

        [Display(AutoGenerateField = false)]
        public int Id
        {
            get { return Classifier.LicenseEntryId; }
            set { Classifier.LicenseEntryId = value; }
        }

        [Display(Name = "Условия оказания МП")]
        public int? AssistanceCondition_id
        {
            get { return Classifier.AssistanceCondition_id; }
            set { Classifier.AssistanceCondition_id = value; }
        }

        [Display(Name = "Вид оказания МП")]
        public int? AssistanceType_Id
        {
            get { return Classifier.AssistanceType_Id; }
            set { Classifier.AssistanceType_Id = value; }
        }

        [Display(Name = "Профиль")]
        public int? Profile_Id
        {
            get { return Classifier.Profile_Id; }
            set { Classifier.Profile_Id = value; }
        }


    }
}
