using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DataModel;
using DevExpress.Xpf.Editors;
using Medical.AppLayer.Attributes;
using Medical.DatabaseCore.Services.Classifiers;

namespace Medical.AppLayer.Processing.ViewModels
{
    public class EditProcessingViewModel : ClassifierBaseVm<FactProcessing>
    {
        public EditProcessingViewModel(FactProcessing classifier)
            : base(classifier)
        {
        }

        public EditProcessingViewModel()
        {
        }

        [Display(AutoGenerateField = false)]
        public int ProcessingeId
        {
            get { return Classifier.ProcessingId; }
            set { Classifier.ProcessingId = value; RaisePropertyChanged(() => ProcessingeId); }
        }

        [Display(GroupName = "{Tabs}/Данные", Name = "Название", Order = 0)]
        [DisplayName(@"Название")]
        [Required(ErrorMessage = @"Поле 'Название' обязательно для заполнения")]
        public string Name
        {
            get { return Classifier.Name; }
            set { Classifier.Name = value; RaisePropertyChanged(() => Name); }
        }

        [Display(GroupName = "{Tabs}/Данные критерия/<Общие настройки->", Name = "Версия взаимодействия")]
        [Required(ErrorMessage = @"Поле 'Версия взаимодействия' обязательно для заполнения")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(VersionItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int? Version
        {
            get { return _classifier.Version_VersionID; }
            set { _classifier.Version_VersionID = value; RaisePropertyChanged(() => Version); }
        }

        

        [Display(GroupName = "{Tabs}/Данные критерия", Name = "Вес")]
        [Required(ErrorMessage = @"Поле 'Вес' обязательно для заполнения")]
        public int? Weight
        {
            get { return Classifier.Weight; }
            set { Classifier.Weight = value; RaisePropertyChanged(() => Weight); }
        }

        [Display(GroupName = "{Tabs}/Данные критерия", Name = "Включен")]
        [Required(ErrorMessage = @"Поле 'Включен' обязательно для заполнения")]
        public bool? IsEnable
        {
            get { return _classifier.IsEnable; }
            set { _classifier.IsEnable = value; RaisePropertyChanged(() => IsEnable); }
        }

        [Display(GroupName = "{Tabs}/Данные критерия/<Дата действия критерия->", Name = "Дата начала действия")]
        [Required(ErrorMessage = @"Поле 'Дата начала действия' обязательно для заполнения")]
        [DataType(DataType.Date)]
        public DateTime? DateBegin
        {
            get { return _classifier.DateBegin; }
            set { _classifier.DateBegin = value; RaisePropertyChanged(() => DateBegin); }
        }

        [Display(GroupName = "{Tabs}/Данные критерия/<Дата действия критерия->", Name = "Дата окончания действия")]
        [DataType(DataType.Date)]
        public DateTime? DateEnd
        {
            get { return _classifier.DateEnd; }
            set { _classifier.DateEnd = value; RaisePropertyChanged(() => DateEnd); }
        }

        [Display(GroupName = "{Tabs}/Данные критерия/<Область действия критерия->", Name = "Область действия")]
        [Required(ErrorMessage = @"Поле 'Область действия' обязательно для заполнения")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(ScopeItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int? Scope
        {
            get { return _classifier.Scope_ScopeID; }
            set { _classifier.Scope_ScopeID = value; RaisePropertyChanged(() => Scope); }
        }

        [Display(GroupName = "{Tabs}/Данные критерия/<Область действия критерия->", Name = "Тип функции обработки")]
        [Required(ErrorMessage = @"Поле 'Тип функции обработки' обязательно для заполнения")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(ProcessingTypeItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int? ProcessingType
        {
            get { return _classifier.ProcessingType_ProcessingTypeId; }
            set { _classifier.ProcessingType_ProcessingTypeId = value; RaisePropertyChanged(() => ProcessingType); }
        }

        [Display(GroupName = "{Tabs}/Данные", Name = "Описание"), DataType(DataType.MultilineText)]
        [Required(ErrorMessage = @"Поле 'Описание' обязательно для заполнения")]
        public string Description
        {
            get { return _classifier.Description; }
            set { _classifier.Description = value; RaisePropertyChanged(() => Description); }
        }

        [Display(GroupName = "{Tabs}/Данные", Name = "Комментарий"), DataType(DataType.MultilineText)]
        public string Comments
        {
            get { return _classifier.Comments; }
            set { _classifier.Comments = value; RaisePropertyChanged(() => Comments); }
        }

        [Display(GroupName = "{Tabs}/SQL запрос", Name = "SQL запрос/C# код"), DataType(DataType.MultilineText)]
        [Required(ErrorMessage = @"Поле 'SQL запрос/C# код' обязательно для заполнения")]
        public string Query
        {
            get { return _classifier.Query; }
            set { _classifier.Query = value; RaisePropertyChanged(() => Query); }
        }
    }
}