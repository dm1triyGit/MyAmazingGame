using AmazingGameCLient.Abstractions;
using AmazingGameCLient.Enums;
using AmazingGameCLient.Models;
using AmazingGameCLient.Responses;

namespace AmazingGameCLient.Services
{
    internal class UIService
    {
        private ClientStates _clientState;
        private UserProfile? _userProfile;
        private Item[]? _shopItems;

        private readonly ILoginService _loginService;
        private readonly ISessionService _sessionService;

        public UIService(ILoginService loginService, ISessionService sessionService)
        {
            _clientState = ClientStates.Login;
            _loginService = loginService;
            _sessionService = sessionService;
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

            _sessionService.StartSession(response.Token);
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
                Console.WriteLine();
                Console.WriteLine($"{(int)SessionOptions.Money}) Монеты");
                Console.WriteLine($"{(int)SessionOptions.Shop}) Магазин");
                Console.WriteLine($"{(int)SessionOptions.Items}) Инвентарь");
                Console.WriteLine($"{(int)SessionOptions.Exit}) Выйти из профиля");
                Console.WriteLine();
            }

            var line = Console.ReadLine();
            if (int.TryParse(line, out int input))
            {
                switch ((SessionOptions)input)
                {
                    case SessionOptions.Money:
                        var balance = _userProfile.Coins;
                        Console.WriteLine($"Количество монет: {balance}");
                        break;

                    case SessionOptions.Shop:
                        await ShopDialogAsync();
                        break;

                    case SessionOptions.Items:
                        var profileItems = _userProfile.Items;

                        if (profileItems == null || profileItems.Length == 0)
                        {
                            Console.WriteLine("У вас нет предметов");
                            break;
                        }

                        Console.WriteLine();
                        Console.WriteLine("Предметы в инвентаре:\n");
                        foreach (var item in profileItems)
                        {
                            Console.WriteLine(item.Name);
                        }
                        Console.WriteLine();
                        break;

                    case SessionOptions.Exit:
                        await EndGame();
                        break;

                    default:
                        return true;
                }

                return false;
            }

            return true;
        }

        private async Task EndGame()
        {
            await _loginService.Logout(_userProfile.Nickname);
            await _sessionService.EndSession();
            _userProfile = null;
            _clientState = ClientStates.Login;
        }

        private async Task ShopDialogAsync()
        {
            _shopItems ??= await _sessionService.GetShopItems();


            if (_shopItems == null)
            {
                Console.WriteLine("Произошла ошибка. Попробуйте снова");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("Предметы магазина:");
            var counter = 1;

            foreach (var item in _shopItems)
            {
                Console.WriteLine($"{counter}) Название - {item.Name} | Стоимость - {item.Price}");
                counter++;
            }
            Console.WriteLine("0) Выход");

            Console.WriteLine();
            Console.WriteLine("Введите номер предмета для покупки или продажи");
            Console.WriteLine();

            var line = Console.ReadLine();

            if (line == "0")
            {
                return;
            }

            Item selectedItem = null;

            if (int.TryParse(line, out int itemNumber) 
                && itemNumber <= _shopItems.Length 
                && itemNumber > 0)
            {
                var index = itemNumber - 1;
                selectedItem = _shopItems[index];
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
            Console.WriteLine();
            Console.WriteLine($"{(int)ShopOptions.Buy}) Покупка");
            Console.WriteLine($"{(int)ShopOptions.Sell}) Продажа");
            Console.WriteLine($"{(int)ShopOptions.Cancel}) Отмена");
            Console.WriteLine();

            var shopOptionsline = Console.ReadLine();
            if (int.TryParse(shopOptionsline, out int input))
            {
                switch ((ShopOptions)input)
                {
                    case ShopOptions.Buy:
                        var isValidBuy = ValidateBuy(selectedItem);
                        if (!isValidBuy)
                        {
                            Console.WriteLine("\nНедостаточно денег, либо предмет уже есть");
                            return;
                        }
                        var buyResponse = await _sessionService.BuyItemAsync(selectedItem.Id, _userProfile!.Nickname);

                        if (!buyResponse.IsSuccess)
                        {
                            Console.WriteLine("Произошла ошибка при покупке");
                            return;
                        }
                        UpdateProfile(buyResponse);
                        Console.WriteLine($"\nВы успешно приобрели - {selectedItem.Name}\n");
                        break;

                    case ShopOptions.Sell:
                        var isValidSell = ValidateSell(selectedItem);
                        if (!isValidSell)
                        {
                            Console.WriteLine("\nУ вас нет предмета");
                            return;
                        }

                        var sellResponse = await _sessionService.SellItemAsync(selectedItem.Id, _userProfile!.Nickname);
                        if (!sellResponse.IsSuccess)
                        {
                            Console.WriteLine("Произошла ошибка при продаже");
                            return;
                        }
                        UpdateProfile(sellResponse);
                        Console.WriteLine($"\nВы успешно продали - {selectedItem.Name}\n");
                        break;

                    case ShopOptions.Cancel:
                        return;
                    default:
                        Console.WriteLine("Введите корректное число");
                        return;
                }
            }
        }

        private bool ValidateBuy(Item? item)
        {
            if (item == null)
            {
                return false;
            }
            if (_userProfile.Coins < item.Price)
            {
                return false;
            }
            if (_userProfile.Items.Any(x => x.Id == item.Id))
            {
                return false;
            }

            return true;
        }

        private bool ValidateSell(Item? item)
        {
            if (item == null)
            {
                return false;
            }
            if (!_userProfile.Items.Any(x => x.Id == item.Id))
            {
                return false;
            }

            return true;
        }

        private void UpdateProfile(BaseItemsResponse response)
        {
            _userProfile.Coins = response.Coins;
            _userProfile.Items = response.Items;
        }
    }
}
