using AmazingGameServer.BLL.Abstractions;
using AmazingGameServer.BLL.Mappers;
using GameServer;
using Grpc.Core;

namespace AmazingGameServer.BLL.Services
{
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
    }
}
