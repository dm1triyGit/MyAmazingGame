using AmazingGameServer.DAL.Dto;

namespace AmazingGameServer.BLL.Responses
{
    public class BaseItemResponse
    {
        public Profile Profile { get; set; }
        public bool IsSuccess { get; set; }
    }
}
