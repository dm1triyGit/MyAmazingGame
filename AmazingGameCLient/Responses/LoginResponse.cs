using AmazingGameCLient.Models;

namespace AmazingGameCLient.Responses
{
    internal class LoginResponse
    {
        public UserProfile Profile { get; set; }
        public string Token { get; set; }
    }
}
