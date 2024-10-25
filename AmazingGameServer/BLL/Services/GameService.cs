using AmazingGameServer.BLL.Abstractions;
using AmazingGameServer.BLL.Options;
using AmazingGameServer.DAL.Dto;

namespace AmazingGameServer.BLL.Services
{
    public class GameService : IGameService
    {
        private readonly ProfileOptions _profileOptions;
        public GameService(IConfiguration appConfig)
        {
            _profileOptions = appConfig.GetSection(nameof(ProfileOptions)).Get<ProfileOptions>();
        }

        public Profile CreateProfile(string nickname)
        {
            return new Profile
            {
                Nickname = nickname,
                Coins = _profileOptions.StartCoins
            };
        }

        public int GetPay()
        {
            var min = _profileOptions.LowerCoinsRange;
            var max = _profileOptions.UpperCointRange;

            var randomPay = new Random().Next(min, max);

            return randomPay;
        }
    }
}
