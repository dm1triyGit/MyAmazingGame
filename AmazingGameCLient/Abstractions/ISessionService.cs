using AmazingGameCLient.Models;

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

        Task<bool> BuyItemAsync(int itemId, string nickname);
        Task<bool> SellItemAsync(int itemId, string nickname);
    }
}
