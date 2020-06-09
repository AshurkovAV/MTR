using System.ComponentModel.DataAnnotations;
using DataModel;
using Medical.DatabaseCore.Services.Classifiers;

namespace Medical.AppLayer.Models.ClassifierModels
{
    public class EditLocalF003 : ClassifierBaseVm<localF003>
    {
        public EditLocalF003(localF003 classifier)
            : base(classifier)
        {
            
        }

        public EditLocalF003()
        {

        }

        [Display(AutoGenerateField = false)]
        public int LocalF003Id
        {
            get { return Classifier.LocalF003Id; }
            set { Classifier.LocalF003Id = value; }
        }

        [Display(Name = "Код МО")]
        [StringLength(6)]
        public string Code
        {
            get { return Classifier.Code; }
            set { Classifier.Code = value; }
        }

        [Display(Name = "Краткое название")]
        [StringLength(250)]
        public string ShortName
        {
            get { return Classifier.ShortName; }
            set { Classifier.ShortName = value; }
        }

        [Display(AutoGenerateField = false)]
        //[Display(Name = "Название должности руководителя")]
        [StringLength(50)]
        public string PositionName
        {
            get { return Classifier.PositionName; }
            set { Classifier.PositionName = value; }
        }

        [Display(Name = "Фамилия руководителя")]
        [StringLength(50)]
        public string Surname
        {
            get { return Classifier.Surname; }
            set { Classifier.Surname = value; }
        }

        [Display(Name = "Имя руководителя")]
        [StringLength(50)]
        public string Name
        {
            get { return Classifier.Name; }
            set { Classifier.Name = value; }
        }

        [Display(Name = "Отчество руководителя")]
        [StringLength(50)]
        public string Patronymic
        {
            get { return Classifier.Patronymic; }
            set { Classifier.Patronymic = value; }
        }

        [Display(Name = "Телефон")]
        [StringLength(40)]
        public string Phone
        {
            get { return Classifier.Phone; }
            set { Classifier.Phone = value; }
        }

        [Display(AutoGenerateField = false)]
        //[Display(Name = "ОКАТО")]
        [StringLength(5)]
        public string OKATO
        {
            get { return Classifier.OKATO; }
            set { Classifier.OKATO = value; }
        }

        

        [Display(Name = "Полный адрес")]
        [StringLength(254)]
        public string FullAddress
        {
            get { return Classifier.FullAddress; }
            set { Classifier.FullAddress = value; }
        } 
    }
}
