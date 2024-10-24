using AmazingGameCLient.Profile;

namespace AmazingGameCLient.Abstractions
{
    internal interface ILoginService
    {
        Task<UserProfile> Login(string nickName);
        Task Logout();
    }
}
