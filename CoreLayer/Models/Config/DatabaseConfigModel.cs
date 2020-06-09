using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Input;
using BLToolkit.Data;
using BLToolkit.Data.DataProvider;
using Core;
using Core.Infrastructure;
using Core.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.CoreLayer.Helpers;
using Medical.CoreLayer.Service;
using DataException = BLToolkit.Data.DataException;

namespace Medical.CoreLayer.Models.Config
{
    public class DatabaseConfigModel : ViewModelBase
    {
        private readonly IMessageService _messageService;
        public readonly IAppShareSettings _shareSettings;
        public dynamic _data;
        private bool _inProgress;

        private RelayCommand _saveCommand;
        private RelayCommand _testCommand;


        private readonly Dictionary<string, Func<DatabaseConfigModel, string>> _handlers = new Dictionary<string, Func<DatabaseConfigModel, string>>
        {
            {ProviderName.MsSql, p => (p.IsWindowsAuth ? (string.Format("Data Source={0};Initial Catalog={1};Integrated Security=True;Application Name={2};Connect Timeout={3};", p.DataSource, p.Database, GlobalConfig.AppName, p.Timeout)) :
                string.Format("Data Source={0};Initial Catalog={1};Application Name={2};User Id={3};Password={4};Connect Timeout={5};", p.DataSource, p.Database, GlobalConfig.AppName, p.Username, p.Password, p.Timeout))
            }
        };

        public string Name
        {
            get { return _data.Name; }
            set { _data.Name = value; RaisePropertyChanged(() => Name); }
        }
        public string DataSource
        {
            get { return _data.DataSource; }
            set { _data.DataSource = value; RaisePropertyChanged(() => DataSource); }
        }
        public string Database
        {
            get { return _data.Database; }
            set { _data.Database = value; RaisePropertyChanged(() => Database); }
        }
        public string Username
        {
            get { return _data.Username; }
            set { _data.UserId = value; RaisePropertyChanged(() => Username); }
        }
        public string Password
        {
            get { return _data.Password; }
            set { _data.Password = value; RaisePropertyChanged(() => Password); }
        }
        public string Provider
        {
            get { return _data.Provider; }
            set { _data.Provider = value; RaisePropertyChanged(() => Provider); }
        }
        public bool IsWindowsAuth
        {
            get { return _data.IsWindowsAuth; }
            set { _data.IsWindowsAuth = value; RaisePropertyChanged(() => IsWindowsAuth); }
        }
        public int Timeout
        {
            get { return _data.Timeout; }
            set { _data.Timeout = value; RaisePropertyChanged(() => Timeout); }
        }

        public string ConnectionString
        {
            get
            {
                if (!_handlers.ContainsKey(Provider))
                {
                    return null;
                }
                _data.ConnectionString = _handlers[Provider](this);
                return _data.ConnectionString;
            }
        }

        public bool InProgress
        {
            get { return _inProgress; }
            set { _inProgress = value; RaisePropertyChanged(()=>InProgress); }
        }

        public DatabaseConfigModel(IMessageService messageService, IAppShareSettings shareSettings)
        {
            _messageService = messageService;
            _shareSettings = shareSettings;
            LoadConfig();
        }

        public void LoadConfig()
        {
            _data = _shareSettings.Get("database");
        }

        public string DefaultProfileName { get; set; }

        public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new RelayCommand(Save));

        private void Save()
        {
            _shareSettings.Put("database", _data);
        }

        public ICommand TestCommand => _testCommand ?? (_testCommand = new RelayCommand(Test));

        private void Test()
        {
            Task.Factory.StartNew(InProgressSwitch).ContinueWith(p =>
            {
                try
                {
                    using (var db = new DbManager(Provider.ToDataProvider(), ConnectionString))
                    {
                        if (ConnectionState.Open == db.Connection.State)
                        {
                            _messageService.ShowMessage(CoreMessages.ConnectionSuccess);
                        }
                    }
                }
                catch (DataException exception)
                {
                    _messageService.ShowException(exception, "Ошибка", typeof(DatabaseConfigModel));
                }
            }).ContinueWith(p => InProgressSwitch());
        }

        private void InProgressSwitch()
        {
            InProgress = !InProgress;
        }
    }
}