using Medical.AppLayer.Models;

namespace Medical.AppLayer.Services
{
    public interface IUserService
    {
        int UserId { get; }
        UserInfoModel UserInfo { get; }
        string UserName { get; }
        bool LoginUser(string login, string password);
        bool LogoutUser();
        bool IsUserLogged();
    }
}