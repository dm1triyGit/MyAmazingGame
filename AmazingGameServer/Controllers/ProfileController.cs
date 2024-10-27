using AmazingGameServer.BLL.Abstractions;
using AmazingGameServer.BLL.Options;
using AmazingGameServer.BLL.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AmazingGameServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IGameService _gameService;

        public ProfileController(IGameService gameService)
        {
            _gameService = gameService;
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
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromDays(1)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
