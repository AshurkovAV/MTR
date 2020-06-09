using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Core;
using Core.Extensions;
using GalaSoft.MvvmLight;

namespace Medical.AppLayer.Search.ViewModels
{
    
    public class PatientSearchFormViewModel : ViewModelBase
    {
        public PatientSearchFormViewModel()
        {
            this.ApplyDefaultValues();    
        }

        [Display(GroupName = @"Уникальные идентификаторы", Name = @"Номер документа ОМС")]
        [DefaultValue(null)]
        public string InsuranceNumber { get; set; }

        [Display(GroupName = @"Уникальные идентификаторы", Name = @"ID пациента")]
        [DefaultValue(null)]
        public int? Id { get; set; }

        [Display(GroupName = @"Персональные данные", Name = @"Фамилия")]
        [DefaultValue(null)]
        public string Surname { get; set; }

        [Display(GroupName = @"Персональные данные", Name = @"Имя")]
        [DefaultValue(null)]
        public string Name { get; set; }

        [Display(GroupName = @"Персональные данные", Name = @"Отчество")]
        [DefaultValue(null)]
        public string Patronymic { get; set; }

        [Display(GroupName = @"Персональные данные", Name = @"Дата рождения")]
        [DefaultValue(null)]
        public DateTime? BirthDate { get; set; }

        [Display(GroupName = @"Персональные данные", Name = @"Пол")]
        [DefaultValue(null)]
        public SexEnum? Sex { get; set; }

        [Display(GroupName = @"Опции", Name = @"С неполной оплатой")]
        [DefaultValue(false)]
        public bool IsUnderpayment { get; set; }

        [Display(AutoGenerateField = false)]
        public new bool IsInDesignMode { get; set; }

        [Display(AutoGenerateField = false)]
        public bool IsNotEmpty {
            get
            {
                return InsuranceNumber.IsNotNullOrWhiteSpace() ||
                       Id.HasValue ||
                       Name.IsNotNullOrWhiteSpace() ||
                       Surname.IsNotNullOrWhiteSpace() ||
                       Patronymic.IsNotNullOrWhiteSpace() ||
                       BirthDate.HasValue ||
                       Sex.HasValue;
            }
        }

        public void UpdateAll()
        {
            GetType().GetProperties().Select(p => p.Name).ForEachAction(RaisePropertyChanged);
        }
    }
}
