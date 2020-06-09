using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DataModel;
using DevExpress.Xpf.Editors;
using Medical.AppLayer.Attributes;
using Medical.DatabaseCore.Services.Classifiers;

namespace Medical.AppLayer.Examination.ViewModels
{
    [DisplayName(@"Создание/редактирование критерия экспертизы")]
    public class EditCriterionViewModel : ClassifierBaseVm<FactExpertCriterion>
    {
        public EditCriterionViewModel(FactExpertCriterion classifier)
            : base(classifier)
        {
        }

        public EditCriterionViewModel()
        {
        }

        [Display(AutoGenerateField = false)]
        [Category("Поля только для чтения")]
        [DisplayName(@"ID критерия")]
        [Description("ID критерия в БД")]
        [ReadOnly(true)]
        public int ExpertCriterionId
        {
            get { return Classifier.FactExpertCriterionID; }
            set { Classifier.FactExpertCriterionID = value; RaisePropertyChanged(() => ExpertCriterionId); }
        }

        [Display(GroupName = "{Tabs}/Данные критерия", Name = "Название", Order = 0)]
        [Category("Обязательные поля")]
        [DisplayName(@"Название")]
        [Description("Название критерия")]
        [Required(ErrorMessage = @"Поле 'Название' обязательно для заполнения")]
        public string Name
        {
            get { return Classifier.Name; }
            set { Classifier.Name = value; RaisePropertyChanged(() => Name); }
        }

        [Display(GroupName = "{Tabs}/Данные критерия/<Общие настройки->", Name = "Версия взаимодействия")]
        [Category("Обязательные поля")]
        [DisplayName(@"Версия")]
        [Description("Версия взаимодействия")]
        [Required(ErrorMessage = @"Поле 'Версия взаимодействия' обязательно для заполнения")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(VersionItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int? Version
        {
            get { return _classifier.Version; }
            set { _classifier.Version = value; RaisePropertyChanged(() => Version); }
        }

        [Display(GroupName = "{Tabs}/Данные критерия/<Общие настройки->", Name = "Группа критериев")]
        [Category("Обязательные поля")]
        [DisplayName(@"Группа критериев")]
        [Description("Группа критериев в соответствии с назначением критерия")]
        [Required(ErrorMessage = @"Поле 'Группа критериев' обязательно для заполнения")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(ExaminationGroupItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int? Group
        {
            get { return _classifier.Group; }
            set { _classifier.Group = value; RaisePropertyChanged(() => Group); }
        }

        

        [Display(GroupName = "{Tabs}/Данные критерия", Name = "Вес")]
        [Category("Обязательные поля")]
        [DisplayName(@"Вес")]
        [Description("Вес критерия (порядок выполнения)")]
        [Required(ErrorMessage = @"Поле 'Вес' обязательно для заполнения")]
        public int? Weight
        {
            get { return Classifier.Weight; }
            set { Classifier.Weight = value; RaisePropertyChanged(() => Weight); }
        }

        [Display(GroupName = "{Tabs}/Данные критерия/<Общие настройки->", Name = "Вид контроля")]
        [Category("Обязательные поля")]
        [DisplayName(@"Вид контроля")]
        [Description("Вид контроля")]
        [Required(ErrorMessage = @"Поле 'Вид контроля' обязательно для заполнения")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(ExaminationTypeItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int? Source
        {
            get { return _classifier.Type; }
            set { _classifier.Type = value; RaisePropertyChanged(() => Source); }
        }

        [Display(GroupName = "{Tabs}/Данные критерия", Name = "Включен")]
        [Category("Обязательные поля")]
        [DisplayName(@"Включен")]
        [Description("Включен критерий (отображение в окне запуска проверок)")]
        [Required(ErrorMessage = @"Поле 'Включен' обязательно для заполнения")]
        public bool? IsEnable
        {
            get { return _classifier.IsEnable; }
            set { _classifier.IsEnable = value; RaisePropertyChanged(() => IsEnable); }
        }

        [Display(GroupName = "{Tabs}/Данные критерия", Name = "Авто")]
        [DisplayName(@"Авто")]
        [Description("Разрешен запуск критерия при загрузке реестра")]
        [Required(ErrorMessage = @"Поле 'Авто' обязательно для заполнения")]
        public bool? IsAuto
        {
            get { return _classifier.IsAuto; }
            set { _classifier.IsAuto = value; RaisePropertyChanged(() => IsAuto); }
        }

        [Display(GroupName = "{Tabs}/Данные критерия/<Дата действия критерия->", Name = "Дата начала действия")]
        [Category("Обязательные поля")]
        [DisplayName(@"Дата начала действия")]
        [Description("Дата начала действия критерия")]
        [Required(ErrorMessage = @"Поле 'Дата начала действия' обязательно для заполнения")]
        public DateTime? DateBegin
        {
            get { return _classifier.DateBegin; }
            set { _classifier.DateBegin = value; RaisePropertyChanged(() => DateBegin); }
        }

        [Display(GroupName = "{Tabs}/Данные критерия/<Дата действия критерия->", Name = "Дата окончания действия")]
        [Category("Условные поля")]
        [DisplayName(@"Дата окончания действия")]
        [Description("Дата окончания действия критерия")]
        [DataType(DataType.Date)]
        public DateTime? DateEnd
        {
            get { return _classifier.DateEnd; }
            set { _classifier.DateEnd = value; RaisePropertyChanged(() => DateEnd); }
        }

        [Display(GroupName = "{Tabs}/Данные критерия/<Область действия критерия->", Name = "Область действия")]
        [Category("Обязательные поля")]
        [DisplayName(@"Область действия")]
        [Description("Для какой единицы запускается критерий")]
        [Required(ErrorMessage = @"Поле 'Область действия' обязательно для заполнения")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(ScopeItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int? Scope
        {
            get { return _classifier.Scope; }
            set { _classifier.Scope = value; RaisePropertyChanged(() => Scope); }
        }

        [Display(GroupName = "{Tabs}/Данные критерия/<Область действия критерия->", Name = "Область применения ошибок")]
        [Category("Обязательные поля")]
        [DisplayName(@"Область применения ошибок")]
        [Description("На какую область действует ошибка")]
        [Required(ErrorMessage = @"Поле 'Область действия' обязательно для заполнения")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(ScopeItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int? ErrorScope
        {
            get { return _classifier.ErrorScope; }
            set { _classifier.ErrorScope = value; RaisePropertyChanged(() => ErrorScope); }
        }

        [Display(GroupName = "{Tabs}/Данные критерия", Name = "Основание %")]
        [Category("Обязательные поля")]
        [DisplayName(@"Основание")]
        [Description("Основание выставления ошибки согласно справочнику F014")]
        [Required(ErrorMessage = @"Поле 'Основание' обязательно для заполнения")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(F014ItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int? Reason
        {
            get { return _classifier.Reason; }
            set { _classifier.Reason = value; RaisePropertyChanged(() => Reason); }
        }

        [Display(GroupName = "{Tabs}/Данные критерия/<Штрафные санкции->", Name = "Снятие %")]
        [Category("Обязательные поля")]
        [DisplayName(@"Снятие %")]
        [Description("Снятие от суммы представленной к оплате в %")]
        [Required(ErrorMessage = @"Поле 'Снятие %' обязательно для заполнения")]
        public decimal? RefusalPercent
        {
            get { return _classifier.RefusalPercent; }
            set { _classifier.RefusalPercent = value; RaisePropertyChanged(() => RefusalPercent); }
        }

        [Display(GroupName = "{Tabs}/Данные критерия/<Штрафные санкции->", Name = "Штраф %")]
        [Category("Обязательные поля")]
        [DisplayName(@"Штраф %")]
        [Description("Штраф от суммы представленной к оплате в %")]
        [Required(ErrorMessage = @"Поле 'Штраф %' обязательно для заполнения")]
        public decimal? PenaltyPercent
        {
            get { return _classifier.PenaltyPercent; }
            set { _classifier.PenaltyPercent = value; RaisePropertyChanged(() => PenaltyPercent); }
        }

        [Display(GroupName = "{Tabs}/Данные критерия", Name = "Описание"), DataType(DataType.MultilineText)]

        [Required(ErrorMessage = @"Поле 'Описание' обязательно для заполнения")]

        public string Description
        {
            get { return _classifier.Description; }
            set { _classifier.Description = value; RaisePropertyChanged(() => Description); }
        }

        [Display(GroupName = "{Tabs}/Данные критерия", Name = "Комментарий"), DataType(DataType.MultilineText)]
        public string Comments
        {
            get { return _classifier.Comments; }
            set { _classifier.Comments = value; RaisePropertyChanged(() => Comments); }
        }

        [Display(GroupName = "{Tabs}/SQL запрос", Name = "SQL запрос"), DataType(DataType.MultilineText)]
        [DisplayName(@"SQL запрос")]
        [Description("SQL запрос критерия")]
        [Required(ErrorMessage = @"Поле 'SQL запрос' обязательно для заполнения")]
        
        public string Query
        {
            get { return _classifier.Query; }
            set { _classifier.Query = value; RaisePropertyChanged(() => Query); }
        }
    }
}