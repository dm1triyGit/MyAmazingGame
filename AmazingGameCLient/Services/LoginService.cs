using AmazingGameCLient.Abstractions;
using AmazingGameCLient.Profile;

namespace AmazingGameCLient.Services
{
    internal class LoginService : ILoginService
    {
        public Task<UserProfile> Login(string nickName)
        {
            throw new NotImplementedException();
        }

        public Task Logout()
        {
            throw new NotImplementedException();
        }
    }
}
