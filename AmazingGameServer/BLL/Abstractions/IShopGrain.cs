using AmazingGameServer.DAL.Dto;

namespace AmazingGameServer.BLL.Abstractions
{
    public interface IShopGrain : IGrainWithStringKey
    {
        Task SetItems(Item[] items);
        Task<Item[]> GetItems();
    }
}
