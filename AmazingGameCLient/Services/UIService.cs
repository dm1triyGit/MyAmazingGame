using AmazingGameCLient.Abstractions;
using AmazingGameCLient.Enums;
using AmazingGameCLient.Profile;

namespace AmazingGameCLient.Services
{
    internal class UIService
    {
        private ClientStates _clientState;
        private UserProfile? _userProfile;

        private readonly ILoginService _loginService;
        private readonly ISessionService _sessionService;
        private readonly IShopValidatorService _shopValidatorService;
        private readonly IShopService _shopService;

        public UIService(ILoginService loginService, ISessionService sessionService, IShopValidatorService shopValidatorService, IShopService shopService)
        {
            _clientState = ClientStates.Login;
            _loginService = loginService;
            _sessionService = sessionService;
            _shopValidatorService = shopValidatorService;
            _shopService = shopService;
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

            _userProfile = await _loginService.Login(input);

            if (_userProfile == null)
            {
                Console.WriteLine("Произошла ошибка. Попробуйте позже...");
                return true;
            }

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
                Console.WriteLine($"{SessionOptions.Money}) Монеты");
                Console.WriteLine($"{SessionOptions.Shop}) Магазин");
                Console.WriteLine($"{SessionOptions.Items}) Инвентарь");
                Console.WriteLine($"{SessionOptions.Exit}) Выйти из профиля");
            }

            var line = Console.ReadLine();
            if (int.TryParse(line, out int input))
            {
                switch ((SessionOptions)input)
                {
                    case SessionOptions.Money:
                        var balance = await _sessionService.GetBalance(_userProfile!.Id);
                        Console.WriteLine($"Количество монет: {balance}");
                        break;
                    case SessionOptions.Shop:
                        await ShopDialogAsync();
                        break;
                    case SessionOptions.Items:
                        var profileItems = await _sessionService.GetProfileItems(_userProfile!.Id);

                        if (profileItems != null)
                        {
                            Console.WriteLine("Предметы в инвентаре:");
                            foreach (var item in profileItems)
                            {
                                Console.WriteLine(item.Name);
                            }
                        }
                        break;
                    case SessionOptions.Exit:
                        await _loginService.Logout();
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
            Console.WriteLine($"{ShopOptions.Buy}) Покупка");
            Console.WriteLine($"{ShopOptions.Sell}) Продажа");
            Console.WriteLine($"{ShopOptions.Cancel}) Отмена");

            var shopOptionsline = Console.ReadLine();
            if (int.TryParse(shopOptionsline, out int input))
            {
                switch ((ShopOptions)input)
                {
                    case ShopOptions.Buy:
                        var isValidBuy = _shopValidatorService.ValidateBuy(selectedItem, _userProfile!);
                        if(!isValidBuy)
                        {
                            Console.WriteLine("Недостаточно денег, либо предмет уже есть");
                            return;
                        }
                        var isSuccessBuy = await _shopService.Buy(selectedItem.Id, _userProfile!.Id);

                        if(!isSuccessBuy)
                        {
                            Console.WriteLine("Произошла ошибка при покупке");
                        }
                        Console.WriteLine($"Вы успешно приобрели - {selectedItem.Name}");
                        break;
                    case ShopOptions.Sell:
                        var isValidSell = _shopValidatorService.ValidateSell(selectedItem, _userProfile!);
                        if (!isValidSell)
                        {
                            Console.WriteLine("У вас нет предмета");
                            return;
                        }

                        var isSuccessSell = await _shopService.Sell(selectedItem.Id, _userProfile!.Id);
                        if (!isSuccessSell)
                        {
                            Console.WriteLine("Произошла ошибка при продаже");
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
