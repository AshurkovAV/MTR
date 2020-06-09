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

namespace Medical.AppLayer.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class UserService : IUserService, IErrorMessage,ITestable
    {
        private readonly IAppShareSettings _settings;
        private readonly IMessageService _messageService;

        private UserInfoModel _userInfo;

        public int UserId
        {
            get { return UserInfo.UserId; }
        }

        public UserInfoModel UserInfo
        {
            get { return _userInfo ?? (_userInfo = new UserInfoModel()); }
        }

        public string UserName {
            get { return "{0} ({1})".F(UserInfo.FullName, UserInfo.UserName); }
        }

        public string ErrorMessage { get; private set; }

        public UserService(IAppShareSettings settings, IMessageService messageService)
        {
            _settings = settings;
            _messageService = messageService;
        }

        public bool LoginUser(string login, string password)
        {
            try
            {
                dynamic config = _settings.Get("database");
                using (var db = new DbManager(((string)config.Provider).ToDataProvider(), (string)config.ConnectionString))
                {
                    if (ConnectionState.Open == db.Connection.State)
                    {
                        db.SetCommand("SELECT * FROM localUser WHERE Login = @login", db.Parameter("@login", login));
                        var user = db.ExecuteObject<localUser>();
                        if (user == null)
                        {
                            ErrorMessage = "Пользователь не найден";
                            return false;
                        }
                        if (!user.Active)
                        {
                            ErrorMessage = "Пользователь заблокирован";
                            return false;
                        }

                        if (CryptoHelpers.ConfirmPassword(password, Convert.FromBase64String(user.Pass),
                            Convert.FromBase64String(user.Salt)))
                        {
                            UserInfo.SetUser(user);
                            Messenger.Default.Send(UserInfo);
                            return true;
                        }
                    }

                    ErrorMessage = "Ошибка подключения к базе данных";
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "Ошибка при попытке войти в систему", typeof(UserService));
            }
            
            return false;
        }

        public bool LogoutUser()
        {
            /*var user = DatabaseRepository.GetUser(login);
            if (user == null)
            {
                return false;
            }

            var result = DatabaseRepository.SetSid(user.UserID, null);
            if (result.Successful)
            {
                AuthorizedUserRepository.RemoveUser(user.SessionID);
                return true;
            }
            */
            return false;
        }

        public bool IsUserLogged()
        {
            return UserId > 0;
        }


        public bool Test()
        {
            //TODO self testing
            return true;
        }

        
    }
}