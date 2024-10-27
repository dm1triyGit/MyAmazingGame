using AmazingGameCLient.Responses;

namespace AmazingGameCLient.Abstractions
{
    internal interface ILoginService
    {
        Task<LoginResponse> Login(string nickname);
        Task Logout(string nickname);
    }
}
