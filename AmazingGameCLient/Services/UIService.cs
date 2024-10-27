using AmazingGameCLient.Abstractions;
using AmazingGameCLient.Enums;
using AmazingGameCLient.Models;

namespace AmazingGameCLient.Services
{
    internal class UIService
    {
        private ClientStates _clientState;
        private UserProfile? _userProfile;

        private readonly ILoginService _loginService;
        private readonly ISessionService _sessionService;
        private readonly IShopValidatorService _shopValidatorService;

        public UIService(ILoginService loginService, ISessionService sessionService, IShopValidatorService shopValidatorService)
        {
            _clientState = ClientStates.Login;
            _loginService = loginService;
            _sessionService = sessionService;
            _shopValidatorService = shopValidatorService;
        }

        public async Task StartDialogAsync()
        {
            var exitFlag = false;
            var isIncorrectGameSessionInput = false;

            while (!exitFlag)
            {
                switch (_clientState)
                {
                    case ClientStates.Login:
                        exitFlag = await LoginDialogAsync();
                        break;
                    case ClientStates.GameSession:
                        isIncorrectGameSessionInput = await GameSessionDialogAsync(isIncorrectGameSessionInput);
                        break;
                }
            }
        }

        private async Task<bool> LoginDialogAsync()
        {
            Console.WriteLine("Введите логин или 0 для выхода");

            var input = Console.ReadLine();

            if (input == "0" || string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Спасибо за игру");
                return true;
            }

            var response = await _loginService.Login(input);

            _userProfile = response.Profile;

            if (_userProfile == null)
            {
                Console.WriteLine("Произошла ошибка. Попробуйте позже...");
                return true;
            }

            _sessionService.StartSession();
            _clientState = ClientStates.GameSession;
            return false;
        }

        private async Task<bool> GameSessionDialogAsync(bool isIncorrectGameSessionInput)
        {
            if (isIncorrectGameSessionInput)
            {
                Console.WriteLine($"Введите корректный номер");
            }
            else
            {
                await Task.Delay(500);
                Console.WriteLine($"{(int)SessionOptions.Money}) Монеты");
                Console.WriteLine($"{(int)SessionOptions.Shop}) Магазин");
                Console.WriteLine($"{(int)SessionOptions.Items}) Инвентарь");
                Console.WriteLine($"{(int)SessionOptions.Exit}) Выйти из профиля");
            }

            var line = Console.ReadLine();
            if (int.TryParse(line, out int input))
            {
                switch ((SessionOptions)input)
                {
                    case SessionOptions.Money:
                        var balance = await _sessionService.GetBalance(_userProfile!.Nickname);
                        Console.WriteLine($"Количество монет: {balance}");
                        break;

                    case SessionOptions.Shop:
                        await ShopDialogAsync();
                        break;

                    case SessionOptions.Items:
                        var profileItems = await _sessionService.GetProfileItems(_userProfile!.Nickname);

                        if (profileItems == null || profileItems.Length == 0)
                        {
                            Console.WriteLine("У вас нет предметов");
                            break;
                        }

                        Console.WriteLine("Предметы в инвентаре:");
                        foreach (var item in profileItems)
                        {
                            Console.WriteLine(item.Name);
                        }
                        break;

                    case SessionOptions.Exit:
                        await _loginService.Logout(_userProfile.Nickname);
                        await _sessionService.EndSession();
                        _userProfile = null;
                        _clientState = ClientStates.Login;
                        break;

                    default:
                        return true;
                }

                return false;
            }

            return true;
        }

        private async Task ShopDialogAsync()
        {
            var shopItems = await _sessionService.GetShopItems();

            if (shopItems == null)
            {
                Console.WriteLine("Произошла ошибка. Попробуйте снова");
                return;
            }

            Console.WriteLine("Предметы магазина:");
            var counter = 1;

            foreach (var item in shopItems)
            {
                Console.WriteLine($"{counter}) Название - {item.Name} | Стоимость - {item.Price}");
                counter++;
            }

            Console.WriteLine("Введите номер предмета для покупки или продажи");
            Console.WriteLine("0) Выход");

            var line = Console.ReadLine();

            if (line == "0")
            {
                return;
            }

            Item selectedItem = null;
            if (int.TryParse(line, out int itemNumber) && itemNumber <= shopItems.Length && itemNumber > 0)
            {
                var index = itemNumber - 1;
                selectedItem = shopItems[index];
            }

            if (selectedItem == null)
            {
                Console.WriteLine("Введите корректное число");
                return;
            }

            await ShopOptionsDialogAsync(selectedItem);
        }

        private async Task ShopOptionsDialogAsync(Item selectedItem)
        {
            Console.WriteLine($"{(int)ShopOptions.Buy}) Покупка");
            Console.WriteLine($"{(int)ShopOptions.Sell}) Продажа");
            Console.WriteLine($"{(int)ShopOptions.Cancel}) Отмена");

            var shopOptionsline = Console.ReadLine();
            if (int.TryParse(shopOptionsline, out int input))
            {
                switch ((ShopOptions)input)
                {
                    case ShopOptions.Buy:
                        var isValidBuy = true; //_shopValidatorService.ValidateBuy(selectedItem, _userProfile!);
                        if(!isValidBuy)
                        {
                            Console.WriteLine("Недостаточно денег, либо предмет уже есть");
                            return;
                        }
                        var isSuccessBuy = await _sessionService.BuyItemAsync(selectedItem.Id, _userProfile!.Nickname);

                        if(!isSuccessBuy)
                        {
                            Console.WriteLine("Произошла ошибка при покупке");
                            return;
                        }
                        Console.WriteLine($"Вы успешно приобрели - {selectedItem.Name}");
                        break;
                    case ShopOptions.Sell:
                        var isValidSell = true; //_shopValidatorService.ValidateSell(selectedItem, _userProfile!);
                        if (!isValidSell)
                        {
                            Console.WriteLine("У вас нет предмета");
                            return;
                        }

                        var isSuccessSell = await _sessionService.SellItemAsync(selectedItem.Id, _userProfile!.Nickname);
                        if (!isSuccessSell)
                        {
                            Console.WriteLine("Произошла ошибка при продаже");
                            return;
                        }
                        Console.WriteLine($"Вы успешно продали - {selectedItem.Name}");
                        break;
                    case ShopOptions.Cancel:
                        return;
                    default:
                        Console.WriteLine("Введите корректное число");
                        return;
                }
            }
        }
    }
}
