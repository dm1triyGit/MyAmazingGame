using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AmazingGameServer.BLL.Options
{
    public static class AuthOptions
    {
        public const string ISSUER = "AuthServer"; // издатель токена
        public const string AUDIENCE = "AuthClient"; // потребитель токена
        const string KEY = "amazing_game_secret_key";   // ключ для шифрации
        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
    }
}
