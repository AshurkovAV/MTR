using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Core.Extensions;
using DataModel;
using DevExpress.Xpf.Editors;
using Medical.AppLayer.Attributes;
using Medical.DatabaseCore.Services.Classifiers;
using Core;

namespace Medical.AppLayer.Operator.ViewModel
{
    public class EditZMecViewModel : ClassifierBaseVm<ZFactSank>
    {
        public EditZMecViewModel(ZFactSank classifier)
            : base(classifier)
        {
        }

        public EditZMecViewModel()
        {

        }

        [Display(Name = @"ID МЭК")]
        [ReadOnly(true)]
        public int TerritoryAccountId
        {
            get { return Classifier.ZSankId; }
            set { Classifier.ZSankId = value; }
        }

        [Display(Name = @"Внешний ID")]
        [ReadOnly(true)]
        public string ExternalGuid
        {
            get { return Classifier.ExternalGuid.IsNullOrWhiteSpace() ? Guid.NewGuid().ToString() : Classifier.ExternalGuid; }
            set { Classifier.ExternalGuid = value; }
        }

        [Display(Name = @"Дата создания/редактирования")]
        public DateTime? AccountNumber
        {
            get { return _classifier.Date; }
            set { _classifier.Date = value; }
        }

        [Display(Name = @"Номер акта")]
        public string NumAct
        {
            get { return _classifier.NumAct; }
            set { _classifier.NumAct = value; }
        }

        [Display(Name = @"Пользователь")]
        [ReadOnly(true)]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(UserItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int EmployeeId
        {
            get { return _classifier.EmployeeId; }
        }

        [Display(Name = @"Сумма отказа"), DataType(DataType.Currency)]
        [Required(ErrorMessage = @"Поле 'Сумма МЭК' обязательно для заполнения")]
        public decimal? Amount
        {
            get { return _classifier.Amount; }
            set { _classifier.Amount = value; }
        }

        [Display(Name = @"Код причины отказа")]
        [Required(ErrorMessage = @"Поле 'Код причины отказа' обязательно для заполнения")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(F014ItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int? ReasonId
        {
            get { return _classifier.ReasonId; }
            set { _classifier.ReasonId = value; }
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
    }
}
