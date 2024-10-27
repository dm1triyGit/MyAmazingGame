using AmazingGameServer.BLL.Abstractions;
using AmazingGameServer.BLL.Options;
using AmazingGameServer.BLL.Responses;
using AmazingGameServer.DAL.Abstractions;
using AmazingGameServer.DAL.Dto;

namespace AmazingGameServer.BLL.Services
{
    public class GameService : IGameService
    {
        private readonly ProfileOptions _profileOptions;
        private readonly IGrainFactory _grainFactory;
        private readonly IGameRepository _gameRepository;

        private const string SHOP_KEY = "items-shop";

        public GameService(IConfiguration appConfig, IGrainFactory grainFactory, IGameRepository gameRepository)
        {
            _profileOptions = appConfig.GetSection(nameof(ProfileOptions)).Get<ProfileOptions>()!;
            _grainFactory = grainFactory;
            _gameRepository = gameRepository;
        }

        public async Task CreateGameAsync(Profile profile)
        {
            var profileGrane = _grainFactory.GetGrain<IProfileGrain>(profile.Nickname);
            var shopGrane = _grainFactory.GetGrain<IShopGrain>(SHOP_KEY);
            var shopItems = await _gameRepository.GetItemsAsync();

            await shopGrane.SetItems(shopItems);
            await profileGrane.SetProfile(profile);
        }

        private Profile CreateProfile(string nickname)
        {
            var profile = new Profile
            {
                Nickname = nickname,
                Coins = _profileOptions.StartCoins
            };

            _gameRepository.CreateProfileAsync(profile);

            return profile;
        }

        private int GetPay()
        {
            var min = _profileOptions.LowerCoinsRange;
            var max = _profileOptions.UpperCointRange;

            var randomPay = new Random().Next(min, max);

            return randomPay;
        }

        public async Task EndGameAsync(string nickname)
        {
            var profileGrane = _grainFactory.GetGrain<IProfileGrain>(nickname);

            var profile = await profileGrane.GetProfile();
            await _gameRepository.UpdateProfile(profile);
        }

        public async Task<Profile> GetOrCreateProfileAsync(string nickname)
        {
            var profile = await _gameRepository.GetProfileAsync(nickname);

            if (profile == null)
            {
                profile = CreateProfile(nickname);
            }
            else
            {
                profile.Coins += GetPay();
            }

            return profile;
        }

        public async Task<BuyItemResponse> BuyItemAsync(int itemId, string nickname)
        {
            var profileGrane = _grainFactory.GetGrain<IProfileGrain>(nickname);

            var result = await profileGrane.BuyItem(itemId, nickname);
            var profile = await profileGrane.GetProfile();

            return new BuyItemResponse
            {
                Profile = profile,
                IsSuccess = result
            };
        }

        public async Task<SellItemResponse> SellItemAsync(int itemId, string nickname)
        {
            var profileGrane = _grainFactory.GetGrain<IProfileGrain>(nickname);

            var result = await profileGrane.SellItem(itemId, nickname);
            var profile = await profileGrane.GetProfile();

            return new SellItemResponse
            {
                Profile = profile,
                IsSuccess = result
            };
        }
    }
}
