using System;
using System.ComponentModel.DataAnnotations;
using DataModel;
using Medical.DatabaseCore.Services.Classifiers;

namespace Medical.AppLayer.Models.ClassifierModels
{
    public class EditIDCToEventType : ClassifierBaseVm<globalIDCToEventType>
    {
        public EditIDCToEventType(globalIDCToEventType classifier)
            : base(classifier)
        {
            
        }

        public EditIDCToEventType()
        {

        }

        [Display(AutoGenerateField = false)]
        public int Id
        {
            get { return Classifier.Id; }
            set { Classifier.Id = value; }
        } 

        [Display(Name = "Код МКБ10")]
        public string Kod
        {
            get { return Classifier.IDC; }
            set { Classifier.IDC = value; }
        }

        [Display(Name = "Тип случая")]
        public int? EventType_ID
        {
            get { return Classifier.EventType_ID; }
            set { Classifier.EventType_ID = value; }
        }

    }
}
