namespace AmazingGameCLient.Abstractions
{
    internal interface IShopService
    {
        Task<bool> Buy(int itemId, int profileId);
        Task<bool> Sell(int itemId, int profileId);
    }
}
