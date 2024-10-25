using AmazingGameCLient.Models;

namespace AmazingGameCLient.Abstractions
{
    internal interface IShopValidatorService
    {
        bool ValidateBuy(Item item, UserProfile profile);
        bool ValidateSell(Item item, UserProfile profile);
    }
}
