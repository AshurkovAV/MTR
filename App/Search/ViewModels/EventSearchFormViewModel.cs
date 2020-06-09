using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Core.Extensions;
using GalaSoft.MvvmLight;

namespace Medical.AppLayer.Search.ViewModels
{
    public class EventSearchFormViewModel : ViewModelBase
    {
        public EventSearchFormViewModel()
        {
            this.ApplyDefaultValues();
        }

        [Display(GroupName = @"Дата оказания МП", Name = @"Дата начала лечения с")]
        [DefaultValue(null)]
        public DateTime? EventBeginDate { get; set; }

        [Display(GroupName = @"Дата оказания МП", Name = @"Дата начала лечения по")]
        [DefaultValue(null)]
        public DateTime? EndEventBeginDate { get; set; }

        [Display(GroupName = @"Дата оказания МП", Name = @"Дата окончания лечения с")]
        [DefaultValue(null)]
        public DateTime? EventEndDate { get; set; }

        [Display(GroupName = @"Дата оказания МП", Name = @"Дата окончания лечения по")]
        [DefaultValue(null)]
        public DateTime? EndEventEndDate { get; set; }

        [Display(GroupName = @"Опции", Name = @"С неполной оплатой")]
        [DefaultValue(false)]
        public bool IsUnderpayment { get; set; }

        [Display(AutoGenerateField = false)]
        public new bool IsInDesignMode { get; set; }

        [Display(AutoGenerateField = false)]
        public bool IsNotEmpty
        {
            get
            {
                return EventBeginDate.HasValue ||
                       EndEventBeginDate.HasValue ||
                       EventEndDate.HasValue ||
                       EndEventEndDate.HasValue;
            }
        }

        public void UpdateAll()
        {
            GetType().GetProperties().Select(p => p.Name).ForEachAction(RaisePropertyChanged);
        }
    }
}
