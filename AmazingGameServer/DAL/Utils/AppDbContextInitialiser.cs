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
                    new Item { Name = "Меч", Price = 30 },
                    new Item { Name = "Шлем", Price = 30 },
                    new Item { Name = "Кираса", Price = 60 },
                    new Item { Name = "Перчи", Price = 25 },
                    new Item { Name = "Поножи", Price = 55 },
                    new Item { Name = "Сапоги", Price = 20 },
                    new Item { Name = "Щит", Price = 40 },
                    new Item { Name = "Крутой меч", Price = 120 },
                    new Item { Name = "Крутая кираса", Price = 160 },
                    new Item { Name = "Крутые перчи", Price = 125 },
                    new Item { Name = "Крутые поножи", Price = 155 },
                    new Item { Name = "Крутые сапоги", Price = 120 },
                };

                _context.Items.AddRange(items);
                await _context.SaveChangesAsync();
            }
        }
    }
}
