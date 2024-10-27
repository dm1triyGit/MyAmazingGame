using AmazingGameServer.DAL.Abstractions;
using AmazingGameServer.DAL.DataAccess;
using AmazingGameServer.DAL.Dto;
using Microsoft.EntityFrameworkCore;

namespace AmazingGameServer.DAL.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly AppDbContext _context;

        public GameRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<int> CreateProfileAsync(Profile profile)
        {
            _context.Profiles.Add(profile);
            return _context.SaveChangesAsync();
        }

        public Task<Item[]> GetItemsAsync()
        {
            return _context.Items.ToArrayAsync();
        }

        public Task<Profile?> GetProfileAsync(string nickname)
        {
            return _context.Profiles
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Nickname == nickname);
        }

        public Task<int> UpdateProfile(Profile profile)
        {
            _context.Profiles.Update(profile);
            return _context.SaveChangesAsync();
        }
    }
}
