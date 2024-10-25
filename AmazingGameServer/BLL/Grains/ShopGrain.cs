using AmazingGameServer.BLL.Abstractions;
using AmazingGameServer.DAL.Dto;

namespace AmazingGameServer.BLL.Grains
{
    public class ShopState
    {
        public Item[] items = Array.Empty<Item>();
    }

    public class ShopGrain : Grain, IShopGrain
    {
        private readonly ShopState _state = new();

        public Task<Item[]> GetItems()
        {
            return Task.FromResult(_state.items);
        }

        public Task SetItems(Item[] items)
        {
            _state.items = items;
            return Task.CompletedTask;
        }
    }
}
