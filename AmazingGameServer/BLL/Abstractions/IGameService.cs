using AmazingGameServer.BLL.Responses;
using AmazingGameServer.DAL.Dto;

namespace AmazingGameServer.BLL.Abstractions
{
    public interface IGameService
    {
        Task CreateGameAsync(Profile profile);
        Task EndGameAsync(string nickname);
        Task<Profile> GetOrCreateProfileAsync(string nickname);
        Task<BuyItemResponse> BuyItemAsync(int itemId, string nickname);
        Task<SellItemResponse> SellItemAsync(int itemId, string nickname);
        Task<int> GetCoinsAsync(string nickname);
        Task<Item[]> GetShopItemsAsync();
        Task<Item[]> GetProfileItemsAsync(string nickname);
    }
}
