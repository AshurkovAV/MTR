using System;
using System.ComponentModel.DataAnnotations;
using DataModel;
using Medical.DatabaseCore.Services.Classifiers;
using Medical.AppLayer.Attributes;
using DevExpress.Xpf.Editors;
using System.Collections.Generic;

namespace Medical.AppLayer.Models.ClassifierModels
{
    public class EditObsoleteData : ClassifierBaseVm<globalObsoleteData>
    {
        public EditObsoleteData(globalObsoleteData classifier)
            : base(classifier)
        {
            
        }

        public EditObsoleteData()
        {

        }

        [Display(AutoGenerateField = false)]
        public int Id
        {
            get { return Classifier.ObsoleteDataID; }
            set { Classifier.ObsoleteDataID = value; }
        }

        [Display(Name = "Старое значение")]
        public int OldValue
        {
            get { return Classifier.OldValue; }
            set { Classifier.OldValue = value; }
        }

        [Display(Name = "Новое значение")]
        public int NewValue
        {
            get { return Classifier.NewValue; }
            set { Classifier.NewValue = value; }
        }

        [Display(Name = "Название справочника")]
        public string ClassifierName
        {
            get { return Classifier.Classifier; }
            set { Classifier.Classifier = value; }
        }
    }
}
