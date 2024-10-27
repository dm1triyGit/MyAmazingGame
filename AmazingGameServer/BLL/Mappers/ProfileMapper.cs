using GameServer;
using ProfileDto = AmazingGameServer.DAL.Dto.Profile;
using ItemDto = AmazingGameServer.DAL.Dto.Item;

namespace AmazingGameServer.BLL.Mappers
{
    public static class ProfileMapper
    {
        public static Profile MapToResponseProfile(this ProfileDto profileDto)
        {
            var profile = new Profile
            {
                Coins = profileDto.Coins,
                Nickname = profileDto.Nickname,
                Id = profileDto.Id,
            };

            var items = profileDto.Items.Select(x => x.MapToResponseItem());
            profile.Items.AddRange(items);

            return profile;

        }

        public static Item MapToResponseItem(this ItemDto item)
        {
            return new Item
            {
                Id = item.Id,
                Name = item.Name,
                Price = item.Price,
            };
        }
    }
}
