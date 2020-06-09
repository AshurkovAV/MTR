using System;
using System.Data;
using System.Linq;
using BLToolkit.Data;
using Core.Extensions;
using Core.Helpers;
using Core.Infrastructure;
using Core.Services;
using DataModel;
using GalaSoft.MvvmLight.Messaging;
using Medical.AppLayer.Models;
using Medical.CoreLayer.Helpers;
using Medical.CoreLayer.Service;
using Version = DataModel.Version;


namespace Medical.AppLayer.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class VersionService : IVersionService, IErrorMessage,ITestable
    {
        private readonly IAppShareSettings _settings;
        private readonly IMessageService _messageService;

        private VersionInfoModel _versionInfo;
        public int? VersionId { get { return VersionInfo.VersionId; } }
        public VersionInfoModel VersionInfo
        {
            get { return _versionInfo ?? (_versionInfo = new VersionInfoModel()); }
        }

        public string ErrorMessage { get; private set; }

        public VersionService(IAppShareSettings settings, IMessageService messageService)
        {
            _settings = settings;
            _messageService = messageService;
        }

        public bool Version()
        {
            try
            {
                dynamic config = _settings.Get("database");
                using (var db = new DbManager(((string)config.Provider).ToDataProvider(), (string)config.ConnectionString))
                {
                    if (ConnectionState.Open == db.Connection.State)
                    {
                        db.SetCommand("SELECT * FROM version ");
                        var version = db.ExecuteObject<Version>();
                        if (version == null)
                        {
                            ErrorMessage = "Версия базы не найдена";
                            return false;
                        }
                        else
                        {
                            VersionInfo.SetVersion(version);
                            Messenger.Default.Send(version);
                            return true;
                        }
                    }

                    ErrorMessage = "Ошибка подключения к базе данных";
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "Ошибка при попытке войти в систему", typeof(VersionService));
            }
            
            return false;
        }

        public bool Test()
        {
            //TODO self testing
            return true;
        }


        
    }
}