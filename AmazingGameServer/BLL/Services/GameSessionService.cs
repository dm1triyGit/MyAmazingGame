using AmazingGameServer.BLL.Abstractions;
using AmazingGameServer.BLL.Mappers;
using GameServer;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace AmazingGameServer.BLL.Services
{
    [Authorize]
    public class GameSessionService: AmazingGame.AmazingGameBase
    {
        private readonly IGameService _gameService;

        public GameSessionService(IGameService gameService)
        {
            _gameService = gameService;
        }

        public override async Task BuyItem(
            IAsyncStreamReader<BuyItemRequest> requestStream, 
            IServerStreamWriter<BuyItemResponse> responseStream,
            ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var request = requestStream.Current;

                var response = await _gameService.BuyItemAsync(request.ItemId, request.Nickname);

                var reply = new BuyItemResponse
                {
                    Profile = response.Profile.MapToResponseProfile(),
                    IsSuccess = response.IsSuccess
                };

                await responseStream.WriteAsync(reply);
            }
        }

        public override async Task SellItem(
            IAsyncStreamReader<SellItemRequest> requestStream,
            IServerStreamWriter<SellItemResponse> responseStream,
            ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var request = requestStream.Current;

                var response = await _gameService.SellItemAsync(request.ItemId, request.Nickname);

                var reply = new SellItemResponse
                {
                    Profile = response.Profile.MapToResponseProfile(),
                    IsSuccess = response.IsSuccess
                };

                await responseStream.WriteAsync(reply);
            }
        }

        public override async Task GetCoins(
            IAsyncStreamReader<GetCoinsRequest> requestStream,
            IServerStreamWriter<GetCoinsResponse> responseStream,
            ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var request = requestStream.Current;

                var response = await _gameService.GetCoinsAsync(request.Nickname);

                var reply = new GetCoinsResponse
                {
                    Coins = response
                };

                await responseStream.WriteAsync(reply);
            }
        }

        public override async Task GetShopItems(
            IAsyncStreamReader<Empty> requestStream,
            IServerStreamWriter<GetShopItemsResponse> responseStream,
            ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var request = requestStream.Current;

                var response = await _gameService.GetShopItemsAsync();

                var reply = new GetShopItemsResponse();
                var items = response.Select(x => x.MapToResponseItem()).ToArray();
                reply.Items.AddRange(items);

                await responseStream.WriteAsync(reply);
            }
        }

        public override async Task GetProfileItems(
            IAsyncStreamReader<GetProfileItemsRequest> requestStream,
            IServerStreamWriter<GetProfileItemsResponse> responseStream,
            ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var request = requestStream.Current;

                var response = await _gameService.GetProfileItemsAsync(request.Nickname);

                var reply = new GetProfileItemsResponse();
                var items = response.Select(x => x.MapToResponseItem()).ToArray();
                reply.Items.AddRange(items);

                await responseStream.WriteAsync(reply);
            }
        }
    }
}
