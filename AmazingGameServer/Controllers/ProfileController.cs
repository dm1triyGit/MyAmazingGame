using AmazingGameServer.BLL.Abstractions;
using AmazingGameServer.BLL.Options;
using AmazingGameServer.BLL.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AmazingGameServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IGameService _gameService;
        private readonly AuthOptions _authOptions;

        public ProfileController(IGameService gameService, IConfiguration appConfig)
        {
            _gameService = gameService;
            _authOptions = appConfig.GetSection(nameof(AuthOptions)).Get<AuthOptions>()!;
        }

        [HttpGet("login/{nickname}")]
        public async Task<ProfileResponse> Login(string nickname)
        {
            var profile = await _gameService.GetOrCreateProfileAsync(nickname);
            var token = GetToken(nickname);

            await _gameService.CreateGameAsync(profile);

            return new ProfileResponse { Profile = profile, Token = token };
        }

        [Authorize]
        [HttpGet("logout/{nickname}")]
        public async Task<IActionResult> Logout(string nickname)
        {
            await _gameService.EndGameAsync(nickname);
            return Ok();
        }

        private string GetToken(string nickname)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, nickname) };
            var jwt = new JwtSecurityToken(
                issuer: _authOptions.Issuer,
                audience: _authOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromDays(1)),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authOptions.Key)),
                    SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
