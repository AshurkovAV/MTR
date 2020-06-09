using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Autofac;
using Core.Extensions;
using Core.Services;
using DataModel;
using DevExpress.Xpf.Editors;
using Medical.AppLayer.Attributes;
using Medical.DatabaseCore.Services.Cache;
using Medical.DatabaseCore.Services.Classifiers;

namespace Medical.AppLayer.Operator.ViewModel
{
    public class EditMeeViewModel : ClassifierBaseVm<FactMEE>
    {
        private static readonly ICacheRepository _cacheRepository;
        static EditMeeViewModel()
        {
            _cacheRepository = Di.I.Resolve<ICacheRepository>();
        }

        public EditMeeViewModel(FactMEE classifier)
            : base(classifier)
        {
        }

        public EditMeeViewModel()
        {

        }

        [Display(Name = @"ID МЭЭ")]
        [ReadOnly(true)]
        public int TerritoryAccountId
        {
            get { return Classifier.MEEId; }
            set { Classifier.MEEId = value; }
        }

        [Display(Name = @"Внешний ID")]
        [ReadOnly(true)]
        public string ExternalGuid
        {
            get { return Classifier.ExternalGuid.IsNullOrWhiteSpace() ? Guid.NewGuid().ToString() : Classifier.ExternalGuid; }
            set { Classifier.ExternalGuid = value; }
        }

        [Display(Name = @"Дата создания/редактирования")]
        [ReadOnly(true)]
        public DateTime? AccountNumber
        {
            get { return _classifier.Date; }
            set { _classifier.Date = value; }
        }

        [Display(Name = @"Пользователь")]
        [ReadOnly(true)]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(UserItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int EmployeeId
        {
            get { return _classifier.EmployeeId; }
        }

        [Display(Name = @"Код причины отказа")]
        [Required(ErrorMessage = @"Поле 'Код причины отказа' обязательно для заполнения")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(F014ItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int? ReasonId
        {
            get { return _classifier.ReasonId; }
            set
            {
                _classifier.ReasonId = value;
                if (_classifier.ReasonId.HasValue)
                {
                    var reason = _cacheRepository.Get(CacheRepository.F014aCache).Get(_classifier.ReasonId) as string;
                    if (reason.IsNotNullOrWhiteSpace())
                    {
                        var refusalPenalty = _cacheRepository.Get(CacheRepository.RefusalPenaltyCache).Get(reason.Replace(".", "").ToInt32Nullable()) as globalRefusalPenalty;
                        if (refusalPenalty != null)
                        {
                            Penalty = refusalPenalty.Penalty;
                            Amount = Price * (refusalPenalty.Percent / 100);
                            RaisePropertyChanged(() => Penalty);
                            RaisePropertyChanged(() => Amount);
                        }
                    }
                }
            }
        }

        [Display(Name = @"Сумма МЭЭ"), DataType(DataType.Currency)]
        [Required(ErrorMessage = @"Поле 'Сумма МЭЭ' обязательно для заполнения")]
        public decimal? Amount
        {
            get { return _classifier.Amount; }
            set { _classifier.Amount = value; }
        }

        [Display(Name = @"Штраф"), DataType(DataType.Currency)]
        [Required(ErrorMessage = @"Поле 'Штраф' обязательно для заполнения")]
        public decimal? Penalty
        {
            get { return _classifier.Penalty; }
            set { _classifier.Penalty = value; }
        }

        

        [Display(Name = @"Источник отказа")]
        [Required(ErrorMessage = @"Поле 'Источник отказа' обязательно для заполнения")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(RefusalSourceItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int? Source
        {
            get { return _classifier.Source; }
            set { _classifier.Source = value; }
        }

        [Display(Name = @"Комментарий"), DataType(DataType.MultilineText)]
        public string Comments
        {
            get { return _classifier.Comments; }
            set { _classifier.Comments = value; }
        }

        [Display(AutoGenerateField = false)]
        public decimal? Price { get; set; }
    }
}
