using AmazingGameCLient.Abstractions;
using AmazingGameCLient.Models;
using AmazingGameCLient.Options;
using AmazingGameCLient.Responses;
using GameClient;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Item = AmazingGameCLient.Models.Item;

namespace AmazingGameCLient.Services
{
    internal class SessionService : ISessionService
    {
        private UserProfile _cachedProfile;
        private Item[] _cachedShopItems;

        private GrpcChannel _channel;
        private AmazingGame.AmazingGameClient _client;

        private AsyncDuplexStreamingCall<GetCoinsRequest, GetCoinsResponse> _coinsStream;
        private AsyncDuplexStreamingCall<Empty, GetShopItemsResponse> _shopStream;
        private AsyncDuplexStreamingCall<GetProfileItemsRequest, GetProfileItemsResponse> _profileItemsStream;
        private AsyncDuplexStreamingCall<BuyItemRequest, BuyItemResponse> _buyItemStream;
        private AsyncDuplexStreamingCall<SellItemRequest, SellItemResponse> _sellItemStream;

        private readonly ConnectionOptions _connectionOptions;

        public SessionService(IConfiguration appConfig)
        {
            _connectionOptions = appConfig.GetSection(nameof(ConnectionOptions)).Get<ConnectionOptions>()!;
        }

        public async Task<int> GetBalance(string nickname)
        {
            await _coinsStream.RequestStream.WriteAsync(new GetCoinsRequest { Nickname = nickname });
            await _coinsStream.ResponseStream.MoveNext();

            return _coinsStream.ResponseStream.Current.Coins;
        }

        public async Task<Item[]> GetProfileItems(string nickname)
        {
            await _profileItemsStream.RequestStream.WriteAsync(new GetProfileItemsRequest { Nickname = nickname });

            await _profileItemsStream.ResponseStream.MoveNext();

            var items = _profileItemsStream.ResponseStream.Current.Items
                    .Select(x => new Item
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Price = x.Price
                    })
                    .ToArray();

            return items;
        }

        public async Task<Item[]> GetShopItems()
        {
            await _shopStream.RequestStream.WriteAsync(new Empty());

            await _shopStream.ResponseStream.MoveNext();
            var items = _shopStream.ResponseStream.Current.Items
                .Select(x => new Item 
                    { 
                        Id = x.Id, 
                        Name = x.Name, 
                        Price = x.Price 
                    })
                .ToArray();

            return items;
        }

        public void SetCacheProfile(UserProfile profile)
        {
            _cachedProfile = profile;
        }

        public void SetCacheShopItems(Item[] items)
        {
            _cachedShopItems = items;
        }

        public async void StartSession(string token)
        {
            var credentials = CallCredentials.FromInterceptor(async (context, metadata) =>
            {
                metadata.Add("Authorization", $"Bearer {token}");
            });

            _channel = GrpcChannel.ForAddress(_connectionOptions.ServerAddress, new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Create(new SslCredentials(), credentials)
            });

            _client = new AmazingGame.AmazingGameClient(_channel);

            _coinsStream = _client.GetCoins();
            _shopStream = _client.GetShopItems();
            _profileItemsStream = _client.GetProfileItems();
            _buyItemStream = _client.BuyItem();
            _sellItemStream = _client.SellItem();
        }

        public async Task EndSession()
        {
            await Task.WhenAll(
                _coinsStream.RequestStream.CompleteAsync(),
                _shopStream.RequestStream.CompleteAsync(),
                _profileItemsStream.RequestStream.CompleteAsync(),
                _buyItemStream.RequestStream.CompleteAsync(),
                _sellItemStream.RequestStream.CompleteAsync());

            _coinsStream.Dispose();
            _shopStream.Dispose();
            _channel.Dispose();
        }

        public async Task<BaseItemsResponse> BuyItemAsync(int itemId, string nickname)
        {
            var request = new BuyItemRequest
            {
                ItemId = itemId,
                Nickname = nickname
            };

            await _buyItemStream.RequestStream.WriteAsync(request);
            await _buyItemStream.ResponseStream.MoveNext();

            var response = MapToBuyItemsResponse(_buyItemStream.ResponseStream.Current);
            return response;
        }

        public async Task<BaseItemsResponse> SellItemAsync(int itemId, string nickname)
        {
            var request = new SellItemRequest
            {
                ItemId = itemId,
                Nickname = nickname
            };

            await _sellItemStream.RequestStream.WriteAsync(request);
            await _sellItemStream.ResponseStream.MoveNext();

            var response = MapToSellItemsResponse(_sellItemStream.ResponseStream.Current);
            return response;

        }

        private static BaseItemsResponse MapToBuyItemsResponse(BuyItemResponse response)
        {
            return new BaseItemsResponse
            {
                Coins = response.Profile.Coins,
                Items = response.Profile.Items
                    .Select(x => new Item 
                        {   Id = x.Id, 
                            Name = x.Name, 
                            Price = x.Price 
                        })
                    .ToArray(),
                IsSuccess = response.IsSuccess
            };
        }

        private static BaseItemsResponse MapToSellItemsResponse(SellItemResponse response)
        {
            return new BaseItemsResponse
            {
                Coins = response.Profile.Coins,
                Items = response.Profile.Items
                    .Select(x => new Item
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Price = x.Price
                    })
                    .ToArray(),
                IsSuccess = response.IsSuccess
            };
        }
    }
}
