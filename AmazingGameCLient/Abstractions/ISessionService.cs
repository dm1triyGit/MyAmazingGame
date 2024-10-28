using AmazingGameCLient.Models;
using AmazingGameCLient.Responses;

namespace AmazingGameCLient.Abstractions
{
    internal interface ISessionService
    {
        Task<int> GetBalance(string nickname);
        Task<Item[]> GetShopItems();
        Task<Item[]> GetProfileItems(string nickname);
        void StartSession(string token);
        Task EndSession();
        void SetCacheProfile(UserProfile profile);
        void SetCacheShopItems(Item[] items);
        Task<BaseItemsResponse> BuyItemAsync(int itemId, string nickname);
        Task<BaseItemsResponse> SellItemAsync(int itemId, string nickname);
    }
}
