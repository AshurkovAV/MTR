using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DataModel;
using DevExpress.Xpf.Editors;
using Medical.AppLayer.Attributes;
using Medical.DatabaseCore.Services.Classifiers;

namespace Medical.AppLayer.Admin.ViewModels
{
    public class EditUserViewModel : ClassifierBaseVm<localUser>
    {
        public EditUserViewModel(localUser classifier)
            : base(classifier)
        {
            
        }

        public EditUserViewModel()
        {

        }

        [Display(Name = @"ID пользователя")]
        [ReadOnly(true)]
        public int TerritoryAccountId
        {
            get { return Classifier.UserID; }
            set { Classifier.UserID = value; }
        }

        [Display(Name = @"Логин пользователя")]
        [Required(ErrorMessage = @"Поле 'Логин пользователя' обязательно для заполнения")]
        public string Login
        {
            get { return _classifier.Login; }
            set { _classifier.Login = value; }
        }

        [Display(Name = @"Пароль пользователя")]
        [Required(ErrorMessage = @"Поле 'Пароль пользователя' обязательно для заполнения")]
        public string Pass
        {
            get { return _classifier.Pass; }
            set { _classifier.Pass = value; }
        }

        [Display(Name = @"Роль пользователя")]
        [Required(ErrorMessage = @"Поле 'Роль пользователя' обязательно для заполнения")]
        [CustomEditor(EditorType = typeof(ComboBoxEdit), ItemSourceType = typeof(RoleItemsSource), Value = "Value", DisplayName = "DisplayName")]
        public int? RoleID_RoleID
        {
            get { return _classifier.RoleID_RoleID; }
            set { _classifier.RoleID_RoleID = value; }
        }

        [Display(Name = @"Активен/неактивен пользователь")]
        public bool Active
        {
            get { return _classifier.Active; }
            set { _classifier.Active = value; }
        }

        [Display(Name = @"Фамилия пользователя")]
        [Required(ErrorMessage = @"Поле 'Фамилия пользователя' обязательно для заполнения")]
        public string LastName
        {
            get { return _classifier.LastName; }
            set { _classifier.LastName = value; }
        }

        [Display(Name = @"Имя пользователя")]
        [Required(ErrorMessage = @"Поле 'Имя пользователя' обязательно для заполнения")]
        public string FirstName
        {
            get { return _classifier.FirstName; }
            set { _classifier.FirstName = value; }
        }

        [Display(Name = @"Отчество пользователя")]
        [Required(ErrorMessage = @"Поле 'Отчество пользователя' обязательно для заполнения")]
        public string Patronymic
        {
            get { return _classifier.Patronymic; }
            set { _classifier.Patronymic = value; }
        }

        [Display(Name = @"Должность пользователя")]
        [Required(ErrorMessage = @"Поле 'Должность пользователя' обязательно для заполнения")]
        public string Position
        {
            get { return _classifier.Position; }
            set { _classifier.Position = value; }
        }

        [Display(Name = @"Телефон пользователя")]
        public string Phone
        {
            get { return _classifier.Phone; }
            set { _classifier.Phone = value; }
        }

        [Display(Name = @"№ Для конфиденциальной информации")]
        public string ConfNumber
        {
            get { return _classifier.ConfNumber; }
            set { _classifier.ConfNumber = value; }
        }
        
    }
}
