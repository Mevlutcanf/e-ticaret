using Microsoft.EntityFrameworkCore;
using e_ticaret.Models;
using System.Security.Cryptography;
using System.Text;

namespace e_ticaret.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed admin user with hashed password
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FullName = "Admin User",
                    Email = "admin@example.com",
                    Password = HashPassword("Admin123!"), // Stronger password
                    IsAdmin = true,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1)
                }
            );

            // Seed sample products
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Gaming Laptop",
                    Description = "16GB RAM, RTX 4060, i7 işlemci",
                    Price = 39999,
                    StockQuantity = 14,
                    ImageUrl = "https://images.unsplash.com/photo-1605134513573-384dcf99a44c",
                    Rating = 4.5M,
                    Category = "Bilgisayar",
                    IsActive = true
                },
                new Product
                {
                    Id = 2,
                    Name = "Akıllı Telefon",
                    Description = "128GB, 8GB RAM, 108MP Kamera",
                    Price = 14999,
                    StockQuantity = 24,
                    ImageUrl = "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9",
                    Rating = 4.8M,
                    Category = "Telefon",
                    IsActive = true
                }
            );
        }
    }
} 