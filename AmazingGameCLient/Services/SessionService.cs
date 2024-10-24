using AmazingGameCLient.Abstractions;
using AmazingGameCLient.Profile;

namespace AmazingGameCLient.Services
{
    internal class SessionService : ISessionService
    {
        public Task<int> GetBalance(int profiletId)
        {
            throw new NotImplementedException();
        }

        public Task<Item[]> GetProfileItems(int profiletId)
        {
            throw new NotImplementedException();
        }

        public Task<Item[]> GetShopItems()
        {
            throw new NotImplementedException();
        }
    }
}
