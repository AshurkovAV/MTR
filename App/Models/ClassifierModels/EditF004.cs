using System.ComponentModel.DataAnnotations;
using DataModel;
using Medical.DatabaseCore.Services.Classifiers;
using System;

namespace Medical.AppLayer.Models.ClassifierModels
{
    public class EditF004 : ClassifierBaseVm<F004>
    {
        public EditF004(F004 classifier)
            : base(classifier)
        {
            
        }

        public EditF004()
        {

        }

       // [Display(AutoGenerateField = false)]
        [Display(Name = "ID")]
        public int Id
        {
            get { return Classifier.Id; }
            set { Classifier.Id = value; }
        }

        [Display(Name = "Код эксперта")]
        [StringLength(7)]
        public string code
        {
            get { return Classifier.code; }
            set { Classifier.code = value; }
        }

       
        [Display(Name = "Должность")]
        [StringLength(254)]
        public string position
        {
            get { return Classifier.position; }
            set { Classifier.position = value; }
        }

        [Display(Name = "Фамилия")]
        [StringLength(40)]
        public string Surname
        {
            get { return Classifier.surname; }
            set { Classifier.surname = value; }
        }

        [Display(Name = "Имя")]
        [StringLength(40)]
        public string Name
        {
            get { return Classifier.fname; }
            set { Classifier.fname = value; }
        }

        [Display(Name = "Отчество")]
        [StringLength(40)]
        public string Patronymic
        {
            get { return Classifier.patronymic; }
            set { Classifier.patronymic = value; }
        }

       

       
        [Display(Name = "СНИЛС")]
        [StringLength(14)]
        public string SNILS
        {
            get { return Classifier.SNILS; }
            set { Classifier.SNILS = value; }
        }

        [Display(Name = "Телефон")]
        [StringLength(40)]
        public string Phone
        {
            get { return Classifier.phone1; }
            set { Classifier.phone1 = value; }
        }

        [Display(Name = "Телефон 2")]
        [StringLength(40)]
        public string Phone2
        {
            get { return Classifier.phone2; }
            set { Classifier.phone2 = value; }
        }

        [Display(Name = "Email")]
        [StringLength(50)]
        public string Email
        {
            get { return Classifier.email; }
            set { Classifier.email = value; }
        }

        [Display(Name = "Код организации")]
        [StringLength(13)]
        public string OrganizationCode
        {
            get { return Classifier.organization_code; }
            set { Classifier.organization_code = value; }
        }

        [Display(Name = "Место работы")]
        [StringLength(254)]
        public string EmploymentPlace
        {
            get { return Classifier.employment_place; }
            set { Classifier.employment_place = value; }
        }

        [Display(Name = "Дата начала")]
        public DateTime DateBegin
        {
            get { return Classifier.date_begin; }
            set { Classifier.date_begin = value; }
        }

        [Display(Name = "Дата окончания")]
        public DateTime? DateEnd
        {
            get { return Classifier.date_end; }
            set { Classifier.date_end = value; }
        }

        [Display(Name = "Дата редактирования")]
        public DateTime? DateEdit
        {
            get { return Classifier.date_edit; }
            set { Classifier.date_edit = value; }
        } 

        [Display(Name = "Специальность в разрешении")]
        public string Speciality
        {
            get { return Classifier.speciality; }
            set { Classifier.speciality = value; }
        }

        [Display(Name = "Опыт работы")]
        public int? experience
        {
            get { return Classifier.experience; }
            set { Classifier.experience = value; }
        } 
        
    }
}
