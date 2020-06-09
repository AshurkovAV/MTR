using System;
using System.ComponentModel.DataAnnotations;
using DataModel;
using Medical.DatabaseCore.Services.Classifiers;

namespace Medical.AppLayer.Models.ClassifierModels
{
    public class EditF001 : ClassifierBaseVm<F001>
    {
        public EditF001(F001 classifier)
            : base(classifier)
        {
            
        }

        public EditF001()
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
            get { return Classifier.tf_code; }
            set { Classifier.tf_code = value; }
        }

        [Display(Name = "Наименование")]
        public string LNaim
        {
            get { return Classifier.L_NAIM; }
            set { Classifier.L_NAIM = value; }
        }

        //[Display(Name = "Наименование")]
        //public string Naim
        //{
        //    get { return Classifier.Naim; }
        //    set { Classifier.Naim = value; }
        //}

        //[Display(Name = "Комментарий")]
        //public string Comments
        //{
        //    get { return Classifier.Comments; }
        //    set { Classifier.Comments = value; }
        //}

        //[Display(Name = "Дата начала действия")]
        //public DateTime? DATEBEG
        //{
        //    get { return Classifier.DATEBEG; }
        //    set { Classifier.DATEBEG = value; }
        //}

        //[DisplayFormat(NullDisplayText = "Дата окончания отсутствует")]
        //[Display(Name = "Дата окончания действия")]
        //public DateTime? DATEEND
        //{
        //    get { return Classifier.DATEEND; }
        //    set { Classifier.DATEEND = value; }
        //} 
    }
}
