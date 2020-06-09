using System;
using System.ComponentModel.DataAnnotations;
using DataModel;
using Medical.DatabaseCore.Services.Classifiers;

namespace Medical.AppLayer.Models.ClassifierModels
{
    public class EditF015 : ClassifierBaseVm<F015>
    {
        public EditF015(F015 classifier)
            : base(classifier)
        {
            
        }

        public EditF015()
        {

        }

        [Display(AutoGenerateField = false)]
        public int Id
        {
            get { return Classifier.Id; }
            set { Classifier.Id = value; }
        } 

        [Display(Name = "Код")]
        public decimal? Kod
        {
            get { return Classifier.KOD_OK; }
            set { Classifier.KOD_OK = value; }
        }

        [Display(Name = "Название округа")]
        public string Naim
        {
            get { return Classifier.OKRNAME; }
            set { Classifier.OKRNAME = value; }
        }

        [Display(Name = "Дата начала действия")]
        public DateTime? DATEBEG
        {
            get { return Classifier.DATEBEG; }
            set { Classifier.DATEBEG = value; }
        }

        [DisplayFormat(NullDisplayText = "Дата окончания отсутствует")]
        [Display(Name = "Дата окончания действия")]
        public DateTime? DATEEND
        {
            get { return Classifier.DATEEND; }
            set { Classifier.DATEEND = value; }
        } 
    }
}
