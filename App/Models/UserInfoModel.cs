using System.Linq;
using Core.Extensions;
using DataModel;

namespace Medical.AppLayer.Models
{
    public class UserInfoModel
    {
        private localUser _user;

        public localUser User
        {
            get { return _user; }
        }

        public int UserId
        {
            get { return _user != null ? _user.UserID : 0; }
        }

        public string UserName {
            get { return _user != null ? _user.Login : "Пользователь не залогинен"; }
        }

        public string FullName
        {
            get { return _user != null ? "{0} {1}.{2}.".F(_user.LastName,_user.FirstName.FirstOrDefault(),_user.Patronymic.FirstOrDefault()) : string.Empty;} 
        }

        internal void SetUser(localUser user)
        {
            _user = user;
        }
    }
}
