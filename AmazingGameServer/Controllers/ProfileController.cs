using AmazingGameServer.BLL.Abstractions;
using AmazingGameServer.BLL.Options;
using AmazingGameServer.BLL.Responses;
using AmazingGameServer.DAL.Abstractions;
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
        private readonly IGameRepository _gameRepository;
        private readonly IGameService _gameService;

        public ProfileController(IGameRepository gameRepository, IGameService gameService)
        {
            _gameRepository = gameRepository;
            _gameService = gameService;
        }

        [HttpGet("login/{nickname}")]
        public async Task<ProfileResponse> Login(string nickname)
        {
            var profile = await _gameRepository.GetProfileAsync(nickname);

            if (profile == null)
            {
                profile = _gameService.CreateProfile(nickname);
                //await _gameRepository.CreateProfileAsync(profile);
            }
            else
            {
                profile.Coins += _gameService.GetPay();
            }

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, nickname) };
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new ProfileResponse { Profile = profile, Token = token };
        }

        [Authorize]
        [HttpGet("logout/{nickname}")]
        public async Task<IActionResult> Logout(string nickname)
        {
            return Ok();
        }
    }
}
