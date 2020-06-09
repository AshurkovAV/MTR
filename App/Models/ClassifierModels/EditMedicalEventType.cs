using System;
using System.ComponentModel.DataAnnotations;
using DataModel;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Editors;
using Medical.AppLayer.Attributes;
using Medical.DatabaseCore.Services.Classifiers;
using System.Collections.Generic;

namespace Medical.AppLayer.Models.ClassifierModels
{
    public class EditMedicalEventType : ClassifierBaseVm<globalMedicalEventType>
    {
        public EditMedicalEventType(globalMedicalEventType classifier)
            : base(classifier)
        {

        }

        public EditMedicalEventType()
        {

        }

        [Display(Name = "Ключ")]
        public int Key
        {
            get { return Classifier.ID; }
            set { Classifier.ID = value; }
        }

        [Display(Name = "Название")]
        public string Name
        {
            get { return Classifier.Name; }
            set { Classifier.Name = value; }
        } 

    }
}
