using AmazingGameServer.DAL.Dto;

namespace AmazingGameServer.BLL.Abstractions
{
    public interface IGameService
    {
        Profile CreateProfile(string nickname);
        int GetPay();
        Task CreateGame(Profile profile);
        Task EndGame(string nickname);
    }
}
