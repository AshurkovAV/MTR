using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Autofac;
using BLToolkit.Data;
using Core;
using Core.Extensions;
using Core.Infrastructure;
using Core.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Ionic.Zip;
using Medical.AppLayer.Http;
using Medical.AppLayer.Managers;
using Medical.AppLayer.Services;
using Medical.CoreLayer.Helpers;
using Medical.CoreLayer.Models.Config;
using Medical.CoreLayer.Service;
using UpdateService.Common;
using DataException = BLToolkit.Data.DataException;

namespace Medical.AppLayer.Auth.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IUserService _userService;
        private readonly IVersionService _versionService;
        private readonly IAppLocalSettings _localSettings;
        private readonly IAppShareSettings _shareSettings;
        private readonly IOverlayManager _overlayManager;
        private readonly IMessageService _messageService;

        private RelayCommand _editProfileCommand;
        private RelayCommand _loginCommand;
        private RelayCommand _updateCommand;
        private RelayCommand _testCommand;
        private RelayCommand _saveCommand;

        private bool _isSavePassword;
        private string _login;
        private string _password;
        private bool _hasLoginError;
        private string _errorMessage;
        private string _selectedProfile;
        private bool _isEditProfileOpen;
        public DatabaseConfigModel DatabaseConfig { get; set; }


        public LoginViewModel(
            IUserService userService, 
            IAppLocalSettings localSettings, 
            IAppShareSettings shareSettings, 
            IOverlayManager overlayManager,
            IMessageService messageService)
        {
            _userService = userService;
            _localSettings = localSettings;
            _shareSettings = shareSettings;
            _overlayManager = overlayManager;
            _messageService = messageService;
            _versionService = new VersionService(_shareSettings, _messageService);

            this.ApplyDefaultValues();

            LoadLocalSettings();
            LoadProfiles();
        }

        private void LoadProfiles()
        {
            Profiles = new ObservableCollection<string>(_shareSettings.Profiles);
        }

        private void LoadLocalSettings()
        {
            dynamic settings = _localSettings.Get("user");
            Login = settings.username;
            Password = settings.password;
            SelectedProfile = settings.profile;

            using (var scope = Di.I.BeginLifetimeScope())
            {
                DatabaseConfig = scope.Resolve<DatabaseConfigModel>();
            }
        }

        [DefaultValue(true)]
        public bool IsSavePassword
        {
            get { return _isSavePassword; }
            set
            {
                _isSavePassword = value;
                RaisePropertyChanged(()=>IsSavePassword);
            }
        }

        public string Login
        {
            get { return _login; }
            set
            {
                _login = value;
                RaisePropertyChanged(()=>Login);
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                RaisePropertyChanged(()=>Password);
            }
        }

        [DefaultValue(false)]
        public bool HasLoginError
        {
            get { return _hasLoginError; }
            set
            {
                _hasLoginError = value;
                RaisePropertyChanged(() => HasLoginError);
            }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                RaisePropertyChanged(() => ErrorMessage);
            }
        }

        public string SelectedProfile
        {
            get { return _selectedProfile; }
            set
            {
                _selectedProfile = value;
                _shareSettings.DefaultProfileName = _selectedProfile ?? "development";
                RaisePropertyChanged(() => SelectedProfile);
            }
        }

        public bool IsEditProfileOpen
        {
            get { return _isEditProfileOpen; }
            set
            {
                _isEditProfileOpen = value;
                RaisePropertyChanged(() => IsEditProfileOpen);
            }
        }

        /// <summary>
        /// Название подлючения
        /// </summary>
        public string Name
        {
            get { return DatabaseConfig != null ? DatabaseConfig.Name : string.Empty; }
            set { DatabaseConfig._data.Name = value; RaisePropertyChanged(() => Name); }
        }

        public string DataSource
        {
            get { return DatabaseConfig != null ? DatabaseConfig._data.DataSource : string.Empty; }
            set { DatabaseConfig._data.DataSource = value; RaisePropertyChanged(() => DataSource); }
        }
        public string Database
        {
            get { return DatabaseConfig != null ? DatabaseConfig._data.Database : string.Empty; }
            set { DatabaseConfig._data.Database = value; RaisePropertyChanged(() => Database); }
        }
        public string Username
        {
            get { return DatabaseConfig != null ? DatabaseConfig._data.Username : string.Empty; }
            set { DatabaseConfig._data.Username = value; RaisePropertyChanged(() => Username); }
        }
        public string PasswordProfil
        {
            get { return DatabaseConfig != null ? DatabaseConfig._data.Password : string.Empty; }
            set { DatabaseConfig._data.Password = value; RaisePropertyChanged(() => PasswordProfil); }
        }
        public string Provider
        {
            get { return DatabaseConfig != null ? DatabaseConfig._data.Provider : string.Empty; }
            set { DatabaseConfig._data.Provider = value; RaisePropertyChanged(() => Provider); }
        }
        public bool IsWindowsAuth
        {
            get { return DatabaseConfig != null ? DatabaseConfig._data.IsWindowsAuth : false; }
            set { DatabaseConfig._data.IsWindowsAuth = value; RaisePropertyChanged(() => IsWindowsAuth); }
        }
        public int Timeout
        {
            get { return DatabaseConfig != null ? DatabaseConfig._data.Timeout : 0; }
            set { DatabaseConfig._data.Timeout = value; RaisePropertyChanged(() => Timeout); }
        }

        public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new RelayCommand(Save));

        private void Save()
        {
            try
            {
                var c = DatabaseConfig.ConnectionString;
                DatabaseConfig._shareSettings.Put("database", DatabaseConfig._data);
                _messageService.ShowMessage(CoreMessages.DataSaveSuccess);
            }
            catch (DataException exception)
            {
                _messageService.ShowException(exception, "Ошибка", typeof(DatabaseConfigModel));
            }
            
        }
        public ICommand TestCommand => _testCommand ?? (_testCommand = new RelayCommand(Test));

        private void Test()
        {

            try
            {
                using (var db = new DbManager(DatabaseConfig.Provider.ToDataProvider(), DatabaseConfig.ConnectionString)
                    )
                {
                    if (ConnectionState.Open == db.Connection.State)
                    {
                        _messageService.ShowMessage(CoreMessages.ConnectionSuccess);
                    }
                }
            }
            catch (DataException exception)
            {
                _messageService.ShowException(exception, "Ошибка", typeof (DatabaseConfigModel));
            }
        }



        public ObservableCollection<string> Profiles { get; set; }
        

        public ICommand LoginCommand => _loginCommand ?? (_loginCommand = new RelayCommand(DoLogin, CanLogin));

        public ICommand UpdateCommand => _updateCommand ?? (_updateCommand = new RelayCommand(DoUpdate, CanUpdate));

        private void DoLogin()
        {
            HasLoginError = false;
            _overlayManager.ShowOverlay(Constants.UserAuthTitleMsg, Constants.PleaseWaitMsg)
                .ContinueWith(p =>
                {
                    try
                    {
                        if (_userService.LoginUser(Login, Password))
                        {
                            if (IsSavePassword)
                            {
                                dynamic user = _localSettings.Get("user");
                                user.username = Login;
                                user.password = Password;
                                user.profile = SelectedProfile;
                                _localSettings.Put("user", user);
                            }
                        }
                        else
                        {
                            HasLoginError = true;
                            if (_userService is IErrorMessage)
                            {
                                ErrorMessage = (_userService as IErrorMessage).ErrorMessage;
                            }
                        
                        }
                    
                    }
                    catch (Exception exception)
                    {
                        ErrorMessage = exception.ToString();
                    }
                }
            )
            .ContinueWith(p => _overlayManager.HideOverlay());
        }

        private void DoUpdate()
        {
            var task = _overlayManager.ShowOverlay(Constants.UpdateMtrTitleMsg, Constants.PleaseWaitMsg);
            task.ContinueWith(t => _overlayManager.SetOverlayTitle(Constants.SearchDataTitleMsg))
                .ContinueWith(t =>
                {
                    try
                    {
                        HttpClient client = new HttpClient();
                        var connect = client.GetConnect();
                        if (connect.Success)
                        {
                            _overlayManager.SetOverlayTitle("Соединение установлено.");
                            _versionService.Version();

                            var s = client.GetVersionJson((int)_versionService.VersionId, TypeProductUpdate.DB_TF);
                            if (s.Status == 0)
                            {
                                IApplyUpdateBaseService applyUpdateBaseService =
                                    new ApplyUpdateBaseService(_shareSettings, _overlayManager, _messageService);
                                
                                foreach (var versionMtr in s.Data)
                                {
                                    if (versionMtr.Type == TypeProductUpdate.DB_TF)
                                    {
                                        _overlayManager.SetOverlayTitleAndMessage(Constants.UpdateMtrTitleMsg,
                                            versionMtr.NumberVersion +
                                            " " + versionMtr.NameVersion.Replace("<p>", "").Replace("</p>", "")
                                                .Replace("<strong>", "").Replace("</strong>", ""));
                                        string resultSqlScript =
                                            applyUpdateBaseService.UnZipFileToSqlScript(versionMtr.Data);
                                        if (applyUpdateBaseService.Run(resultSqlScript))
                                        {
                                            _overlayManager.SetOverlayTitle($"Обновление {versionMtr.NumberVersion} выполнено успешно");
                                            Thread.Sleep(4000);
                                        }
                                        else
                                        {
                                            _overlayManager.SetOverlayTitle("Произошла ошибка при обновлении базы данных");
                                            throw new Exception("Произошла ошибка при обновлении базы данных");
                                        }
                                    }

                                }
                                _overlayManager.SetOverlayTitle($"База данных обновлена до последней версии");
                                Thread.Sleep(5000);
                            }
                            else
                            {
                                if (s.Message == "Нет данных для обновления")
                                {
                                    _overlayManager.SetOverlayTitle($"Нет данных для обновления. Вы используете актуальную версию базы данных.");
                                    Thread.Sleep(5000);
                                }
                               
                            }
                            var sclient = client.GetVersionJson((int)_versionService.VersionId, TypeProductUpdate.APP_TFOMS);
                            if (sclient.Status == 0)
                            {
                                IApplyUpdateBaseService applyUpdateBaseService =
                                    new ApplyUpdateBaseService(_shareSettings, _overlayManager, _messageService);

                                foreach (var versionMtr in sclient.Data)
                                {
                                    if (versionMtr.Type == TypeProductUpdate.APP_TFOMS)
                                    {
                                        _overlayManager.SetOverlayTitle($"Есть обновление клиента");

                                        var resultfile =
                                            applyUpdateBaseService.UnZipFileToFile(versionMtr.Data);
                                        if (resultfile.Success)
                                        {
                                            foreach (var file in resultfile.Data)
                                            {
                                                _overlayManager.SetOverlayTitle($"{file}");
                                                Thread.Sleep(100);
                                            }
                                        }
                                        else
                                        {
                                            _overlayManager.SetOverlayTitle($"{resultfile.LastError}");
                                            Thread.Sleep(5000);
                                        }
                                        string path = Directory.GetCurrentDirectory();
                                        Process.Start(path + @"\Omsit.UpdateSlashScreen.exe");

                                        Process[] ps1 = System.Diagnostics.Process.GetProcessesByName("MedicineNext"); //Имя процесса
                                        foreach (Process p1 in ps1)
                                        {
                                            p1.Kill();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (s.Message == "Нет данных для обновления")
                                {
                                    _overlayManager.SetOverlayTitle($"Нет данных для обновления. Вы используете актуальную версию клиента.");
                                    Thread.Sleep(5000);
                                }

                            }
                        }
                        else
                        {
                            _overlayManager.SetOverlayTitle("Сервис не доступен");
                            Thread.Sleep(1000);
                        }
                    }
                    catch (Exception exception)
                    {
                        ErrorMessage = exception.ToString();
                    }

                    //for (int i = 0; i < 1000; i++)
                    //{
                    //    _overlayManager.SetOverlayProgress(1000, i);
                    //    Thread.Sleep(10);

                    //}
                })
                .ContinueWith(t => _overlayManager.HideOverlay());
          
        }

        private bool CanLogin()
        {
            return true;
        }

        private bool CanUpdate()
        {
            return true;
        }

        public ICommand EditProfileCommand => _editProfileCommand ?? (_editProfileCommand = new RelayCommand(EditProfile));

        private void EditProfile()
        {
            DatabaseConfig.DefaultProfileName = SelectedProfile;
            DatabaseConfig.LoadConfig();
            IsEditProfileOpen = true;
            RaisePropertyChanged(() => Name);
            RaisePropertyChanged(() => DataSource);
            RaisePropertyChanged(() => Database);
            RaisePropertyChanged(() => Username);
            RaisePropertyChanged(() => PasswordProfil);
            RaisePropertyChanged(() => Provider);
            RaisePropertyChanged(() => IsWindowsAuth);
            RaisePropertyChanged(() => Timeout);
        }
    }
}
