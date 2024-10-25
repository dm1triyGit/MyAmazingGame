using AmazingGameServer.DAL.Dto;

namespace AmazingGameServer.BLL.Abstractions
{
    public interface IProfileGrain: IGrainWithStringKey
    {
        Task<bool> BuyItem(int itemId, string shopKey);
        Task<bool> SellItem(int itemId, string shopKey);
        Task SetProfile(Profile profile);
        Task<Profile> GetProfile();
    }
}
