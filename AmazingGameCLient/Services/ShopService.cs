using AmazingGameCLient.Abstractions;

namespace AmazingGameCLient.Services
{
    internal class ShopService : IShopService
    {
        public Task<bool> Buy(int itemId, int profileId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Sell(int itemId, int profileId)
        {
            throw new NotImplementedException();
        }
    }
}
