using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using DataModel;
using Medical.AppLayer.Attributes;
using Medical.AppLayer.Editors;
using Medical.DatabaseCore.Services.Classifiers;
using Microsoft.Windows.Controls.PropertyGrid.Attributes;
using DevExpress.Xpf.Editors;

namespace Medical.AppLayer.Report.ViewModels
{
    [DisplayName(@"Создание/редактирование Отчета")]
    public class EditReportViewModel : ClassifierBaseVm<FactReport>
    {
        public EditReportViewModel(FactReport classifier)
            : base(classifier)
        {
            
        }

        public EditReportViewModel()
        {
           
        }
        [Display(AutoGenerateField = false)]
        [Category("Поля только для чтения")]
        [DisplayName(@"ID отчета")]
        [Description("ID отчета в БД")]
        [ReadOnly(true)]
        public int FactReportID
        {
            get { return Classifier.FactReportID; }
            set { Classifier.FactReportID = value; RaisePropertyChanged(()=>FactReportID);}
        }

        [Display(GroupName = "{Tabs}/Отчет", Name = "Включен/выключен отчет")]
        [Category("Обязательные поля")]
        [DisplayName(@"Включен/выключен отчет")]
        [Description("Включен или выключен отчет для выполнения")]
        [Required(ErrorMessage = @"Поле 'Включен/выключен отчет' обязательно для заполнения")]
        public bool? IsEnable
        {
            get { return Classifier.IsEnable; }
            set { Classifier.IsEnable = value; RaisePropertyChanged(() => IsEnable); RaisePropertyChanged(()=>Error); }
        }

        [Display(GroupName = "{Tabs}/Отчет/<Общие настройки->", Name = "Версия счета")]
        [Category("Обязательные поля")]
        [DisplayName(@"Версия счета")]
        [Description("Версия счета")]
        [Required(ErrorMessage = @"Поле 'Версия счета' обязательно для заполнения")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(VersionItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int? Version
        {
            get { return Classifier.Version; }
            set { Classifier.Version = value; RaisePropertyChanged(() => Version); RaisePropertyChanged(() => Error); }
        }

        [Display(GroupName = "{Tabs}/Отчет/<Общие настройки->", Name = "Тип отчета")]
        [Category("Обязательные поля")]
        [DisplayName(@"Тип отчета")]
        [Description("Типы отчетов")]
        [Required(ErrorMessage = @"Поле 'Тип отчета' обязательно для заполнения")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(ReportTypeItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int? Type
        {
            get { return _classifier.Type; }
            set { _classifier.Type = value; RaisePropertyChanged(() => Type); }
        }

        [Display(GroupName = "{Tabs}/Отчет", Name = "Текст отчета")]
        [Category("Обязательные поля")]
        [DisplayName(@"Отчет")]
        [Description("Текст отчета")]
        [Required(ErrorMessage = @"Поле 'Отчет' обязательно для заполнения")]
        [CustomEditor(EditorType = typeof(ReportControl2))]
        public string Body
        {
            get { return Encoding.UTF8.GetString(Classifier.Body ?? (Classifier.Body = new byte[0])); }
            set { Classifier.Body = Encoding.UTF8.GetBytes(value); RaisePropertyChanged(() => Body); RaisePropertyChanged(() => Error);}
        }

        [Display(GroupName = "{Tabs}/Отчет", Name = "Название отчета")]
        [Category("Обязательные поля")]
        [DisplayName(@"Название отчета")]
        [Description("Название отчета, которое будет показано в окне выбора отчетов")]
        [Required(ErrorMessage = @"Поле 'Название отчета' обязательно для заполнения")]
        public string Name
        {
            get { return _classifier.Name; }
            set { _classifier.Name = value; RaisePropertyChanged(() => Name); RaisePropertyChanged(() => Error); }
        }

        [Display(GroupName = "{Tabs}/Отчет", Name = "Область действия отчета")]
        [Category("Обязательные поля")]
        [DisplayName(@"Область действия отчета")]
        [Description("Область действия отчета")]
        [ItemsSource(typeof(ScopeItemsSource))]
        [Required(ErrorMessage = @"Поле 'Область действия отчета' обязательно для заполнения")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(ScopeItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int? Scope
        {
            get { return _classifier.Scope; }
            set { _classifier.Scope = value; RaisePropertyChanged(() => Scope); RaisePropertyChanged(() => Error); }
        }

        [Display(GroupName = "{Tabs}/Отчет", Name = "Описание отчета")]
        [Category("Условные поля")]
        [DisplayName(@"Описание отчета")]
        [Description("Описание отчета")]
        [DataType(DataType.MultilineText)]
        public string Description
        {
            get { return _classifier.Description; }
            set { _classifier.Description = value; RaisePropertyChanged(() => Description); }
        }

        [Display(GroupName = "{Tabs}/Отчет", Name = "Комментарий")]
        [Category("Условные поля")]
        [DisplayName(@"Комментарий")]
        [Description("Комментарий к отчету")]
        [DataType(DataType.MultilineText)]
        public string Comments
        {
            get { return _classifier.Comments; }
            set { _classifier.Comments = value; RaisePropertyChanged(() => Comments); }
        }
    }
}
