using System.ComponentModel.DataAnnotations;
using DataModel;
using Medical.DatabaseCore.Services.Classifiers;

namespace Medical.AppLayer.Models.ClassifierModels
{
    public class EditLocalF001 : ClassifierBaseVm<localF001>
    {
        public EditLocalF001(localF001 classifier)
            : base(classifier)
        {
            
        }

        public EditLocalF001()
        {

        }

        [Display(AutoGenerateField = false)]
        public int LocalF001Id
        {
            get { return Classifier.LocalF001Id; }
            set { Classifier.LocalF001Id = value; }
        } 

        [Display(Name = "Краткое название")]
        [StringLength(40)]
        public string ShortName
        {
            get { return Classifier.ShortName; }
            set { Classifier.ShortName = value; }
        } 

        [Display(Name = "Название должности руководителя")]
        [StringLength(40)]
        public string PositionName
        {
            get { return Classifier.PositionName; }
            set { Classifier.PositionName = value; }
        }

        [Display(Name = "Фамилия руководителя")]
        [StringLength(40)]
        public string Surname
        {
            get { return Classifier.Surname; }
            set { Classifier.Surname = value; }
        }

        [Display(Name = "Имя руководителя")]
        [StringLength(40)]
        public string Name
        {
            get { return Classifier.Name; }
            set { Classifier.Name = value; }
        }

        [Display(Name = "Отчество руководителя")]
        [StringLength(40)]
        public string Patronymic
        {
            get { return Classifier.Patronymic; }
            set { Classifier.Patronymic = value; }
        } 

        [Display(Name = "ОКАТО")]
        [StringLength(5)]
        public string OKATO
        {
            get { return Classifier.OKATO; }
            set { Classifier.OKATO = value; }
        }

        [Display(Name = "Код ТФ")]
        [StringLength(2)]
        public string Code
        {
            get { return Classifier.Code; }
            set { Classifier.Code = value; }
        }

        [Display(Name = "Название региона")]
        [StringLength(40)]
        public string RegionName
        {
            get { return Classifier.RegionName; }
            set { Classifier.RegionName = value; }
        }

        [Display(Name = "Полный адрес"), DataType(DataType.MultilineText)]
        [StringLength(254)]
        public string FullAddress
        {
            get { return Classifier.FullAddress; }
            set { Classifier.FullAddress = value; }
        } 
    }
}
