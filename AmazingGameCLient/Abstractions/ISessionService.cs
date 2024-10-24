using AmazingGameCLient.Profile;

namespace AmazingGameCLient.Abstractions
{
    internal interface ISessionService
    {
        Task<int> GetBalance(int profiletId);
        Task<Item[]> GetShopItems();
        Task<Item[]> GetProfileItems(int profiletId);
    }
}
