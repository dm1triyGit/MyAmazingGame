using AmazingGameServer.DAL.Dto;

namespace AmazingGameServer.DAL.Abstractions
{
    public interface IGameRepository
    {
        Task<Profile?> GetProfileAsync(string nickname);
        Task<Item[]> GetItemsAsync();
        Task<int> UpdateProfile(Profile profile);
        Task<int> CreateProfileAsync(Profile profile);
    }
}
