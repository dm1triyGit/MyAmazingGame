using AmazingGameCLient.Models;

namespace AmazingGameCLient.Responses
{
    internal class BaseItemsResponse
    {
        public int Coins { get; set; }
        public Item[] Items { get; set; }
        public bool IsSuccess { get; set; }
    }
}
