using AmazingGameServer.DAL.DataAccess;
using AmazingGameServer.DAL.Dto;
using Microsoft.EntityFrameworkCore;

namespace AmazingGameServer.DAL.Utils
{
    public class AppDbContextInitialiser
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AppDbContextInitialiser> _logger;

        public AppDbContextInitialiser(AppDbContext context, ILogger<AppDbContextInitialiser> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task InitialiseAsync()
        {
            try
            {
                if (_context.Database.IsSqlite())
                {
                    await _context.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initialising the database.");
                throw;
            }
        }

        public async Task SeedAsync()
        {
            try
            {
                await TrySeedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private async Task TrySeedAsync()
        {
            if (!_context.Items.Any())
            {
                var items = new List<Item>()
                {
                    new Item { Name = "Test1", Price = 1 },
                    new Item { Name = "Test2", Price = 2 },
                    new Item { Name = "Test3", Price = 3 },
                    new Item { Name = "Test4", Price = 4 }
                };


                _context.Items.AddRange(items);
                await _context.SaveChangesAsync();
            }


        }
    }
}
