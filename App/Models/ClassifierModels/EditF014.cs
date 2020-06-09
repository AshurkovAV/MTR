using System;
using System.ComponentModel.DataAnnotations;
using DataModel;
using Medical.DatabaseCore.Services.Classifiers;

namespace Medical.AppLayer.Models.ClassifierModels
{
    public class EditF014 : ClassifierBaseVm<F014>
    {
        public EditF014(F014 classifier)
            : base(classifier)
        {
            
        }

        public EditF014()
        {

        }

        [Display(AutoGenerateField = false)]
        public int Id
        {
            get { return Classifier.Id; }
            set { Classifier.Id = value; }
        } 

        [Display(Name = "Код")]
        public string Kod
        {
            get { return Classifier.Kod; }
            set { Classifier.Kod = value; }
        }

        [Display(Name = "Код основания")]
        public string Osn
        {
            get { return Classifier.Osn; }
            set { Classifier.Osn = value; }
        }

        [Display(Name = "Наименование")]
        public string Naim
        {
            get { return Classifier.Naim; }
            set { Classifier.Naim = value; }
        }

        [Display(Name = "Комментарий")]
        public string Comments
        {
            get { return Classifier.Comments; }
            set { Classifier.Comments = value; }
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
