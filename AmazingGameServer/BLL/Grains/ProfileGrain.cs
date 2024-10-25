using AmazingGameServer.BLL.Abstractions;
using AmazingGameServer.DAL.Dto;

namespace AmazingGameServer.BLL.Grains
{
    public class ProfileState
    {
        public int Id { get; set; }
        public string Nickname { get; set; }
        public int Coins { get; set; }
        public List<Item> ProfileItems { get; set; } = new();
    }

    public class ProfileGrain : Grain, IProfileGrain
    {
        private readonly ProfileState _state = new();
        public async Task<bool> BuyItem(int itemId, string shopKey)
        {
            var shopItems = await GrainFactory.GetGrain<IShopGrain>(shopKey).GetItems();
            var item = shopItems.FirstOrDefault(x => x.Id == itemId);

            if (!ValidateBuy(item))
            {
                return false;
            }

            _state.Coins -= item.Price;
            _state.ProfileItems.Add(item);

            return true;
        }

        public Task<Profile> GetProfile()
        {
            return Task.FromResult(new Profile
            {
                Id = _state.Id,
                Nickname = _state.Nickname,
                Coins = _state.Coins,
                Items = _state.ProfileItems,
            });
        }

        public async Task<bool> SellItem(int itemId, string shopKey)
        {
            var shopItems = await GrainFactory.GetGrain<IShopGrain>(shopKey).GetItems();
            var item = shopItems.FirstOrDefault(x => x.Id == itemId);

            if (!ValidateSell(item))
            {
                return false;
            }

            _state.Coins += item.Price;
            _state.ProfileItems.Remove(item);

            return true;
        }

        public Task SetProfile(Profile profile)
        {
            _state.Id = profile.Id;
            _state.ProfileItems = profile.Items;
            _state.Nickname = profile.Nickname;
            _state.Coins = profile.Coins;

            return Task.CompletedTask;
        }

        private bool ValidateBuy(Item? item)
        {
            if (item == null)
            {
                return false;
            }
            if (_state.Coins < item.Price)
            {
                return false;
            }
            if (_state.ProfileItems.Any(x => x.Id == item.Id))
            {
                return false;
            }

            return true;
        }

        private bool ValidateSell(Item? item)
        {
            if (item == null)
            {
                return false;
            }
            if (_state.ProfileItems.Any(x => x.Id != item.Id))
            {
                return false;
            }

            return true;
        }
    }
}
