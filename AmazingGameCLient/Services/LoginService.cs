using AmazingGameCLient.Abstractions;
using AmazingGameCLient.Options;
using AmazingGameCLient.Responses;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace AmazingGameCLient.Services
{
    internal class LoginService : ILoginService
    {
        private readonly HttpClient _httpClient = new();
        private readonly ConnectionOptions _connectionOptions;

        public LoginService(IConfiguration appConfig)
        {
            _connectionOptions = appConfig.GetSection(nameof(ConnectionOptions)).Get<ConnectionOptions>()!;

            _httpClient.BaseAddress = new Uri(_connectionOptions.ServerAddress);
        }

        public async Task<LoginResponse> Login(string nickname)
        {
            var path = $"{_connectionOptions.LoginPath}/{nickname}";
            var response = await _httpClient.GetFromJsonAsync<LoginResponse>(path);
            return response;
        }

        public async Task Logout(string nickname)
        {
            var path = $"{_connectionOptions.LogoutPath}/{nickname}";
            var resp = _httpClient.GetAsync(path);
            await resp;
            
        }
    }
}
