using System;
using System.ComponentModel.DataAnnotations;
using DataModel;
using Medical.DatabaseCore.Services.Classifiers;

namespace Medical.AppLayer.Models.ClassifierModels
{
    public class EditV017 : ClassifierBaseVm<V017>
    {
        public EditV017(V017 classifier)
            : base(classifier)
        {
            
        }

        public EditV017()
        {

        }

        [Display(AutoGenerateField = false)]
        public int Id
        {
            get { return Classifier.Id; }
            set { Classifier.Id = value; }
        } 

        [Display(Name = "Код")]
        public int Kod
        {
            get { return Classifier.IDDR; }
            set { Classifier.IDDR = value; }
        }

        [Display(Name = "Название группы")]
        public string Naim
        {
            get { return Classifier.DRNAME; }
            set { Classifier.DRNAME = value; }
        }

        [Display(Name = "Дата начала действия")]
        public DateTime DATEBEG
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
