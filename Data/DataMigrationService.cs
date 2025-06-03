using System.Text.Json;
using e_ticaret.Models;

namespace e_ticaret.Data
{
    public class DataMigrationService
    {
        private readonly ApplicationDbContext _context;

        public DataMigrationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task MigrateDataAsync()
        {
            await MigrateUsersAsync();
            await MigrateOrdersAsync();
            await _context.SaveChangesAsync();
        }

        private async Task MigrateUsersAsync()
        {
            if (!_context.Users.Any() && File.Exists("users.json"))
            {
                var json = await File.ReadAllTextAsync("users.json");
                var users = JsonSerializer.Deserialize<List<User>>(json);
                if (users != null)
                {
                    await _context.Users.AddRangeAsync(users);
                }
            }
        }

        private async Task MigrateOrdersAsync()
        {
            if (!_context.Orders.Any() && File.Exists("orders.json"))
            {
                var json = await File.ReadAllTextAsync("orders.json");
                var orders = JsonSerializer.Deserialize<List<Order>>(json);
                if (orders != null)
                {
                    await _context.Orders.AddRangeAsync(orders);
                }
            }
        }
    }
} 