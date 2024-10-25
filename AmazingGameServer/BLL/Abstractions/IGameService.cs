using AmazingGameServer.DAL.Dto;

namespace AmazingGameServer.BLL.Abstractions
{
    public interface IGameService
    {
        Profile CreateProfile(string nickname);
        int GetPay();
    }
}
