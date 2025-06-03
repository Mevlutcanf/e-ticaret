using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.IO;
using e_ticaret.Models;
using Microsoft.EntityFrameworkCore;

namespace e_ticaret.Data
{
    public class ProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Product> GetAll()
        {
            var products = _context.Products
                .OrderBy(p => p.Name)
                .ToList();

            if (!products.Any())
            {
                // Add sample products if none exist
                var sampleProducts = new List<Product>
                {
                    new Product
                    {
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
                        Name = "Akıllı Telefon",
                        Description = "128GB, 8GB RAM, 108MP Kamera",
                        Price = 14999,
                        StockQuantity = 24,
                        ImageUrl = "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9",
                        Rating = 4.8M,
                        Category = "Telefon",
                        IsActive = true
                    },
                    new Product
                    {
                        Name = "Kablosuz Kulaklık",
                        Description = "Aktif Gürültü Önleme, 30 Saat Pil Ömrü",
                        Price = 2499,
                        StockQuantity = 35,
                        ImageUrl = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e",
                        Rating = 4.6M,
                        Category = "Aksesuar",
                        IsActive = true
                    },
                    new Product
                    {
                        Name = "4K Smart TV",
                        Description = "55 inç, HDR, Android TV",
                        Price = 19999,
                        StockQuantity = 8,
                        ImageUrl = "https://images.unsplash.com/photo-1593784991095-a205069470b6",
                        Rating = 4.7M,
                        Category = "Televizyon",
                        IsActive = true
                    },
                    new Product
                    {
                        Name = "Oyun Konsolu",
                        Description = "1TB Depolama, 2 Kontrolcü",
                        Price = 12999,
                        StockQuantity = 15,
                        ImageUrl = "https://images.unsplash.com/photo-1486401899868-0e435ed85128",
                        Rating = 4.9M,
                        Category = "Oyun",
                        IsActive = true
                    },
                    new Product
                    {
                        Name = "Akıllı Saat",
                        Description = "Nabız Ölçer, GPS, Su Geçirmez",
                        Price = 3499,
                        StockQuantity = 30,
                        ImageUrl = "https://images.unsplash.com/photo-1546868871-7041f2a55e12",
                        Rating = 4.4M,
                        Category = "Aksesuar",
                        IsActive = true
                    },
                    new Product
                    {
                        Name = "Drone",
                        Description = "4K Kamera, 30dk Uçuş Süresi",
                        Price = 8999,
                        StockQuantity = 12,
                        ImageUrl = "https://images.unsplash.com/photo-1507582020474-9a35b7d455d9",
                        Rating = 4.6M,
                        Category = "Elektronik",
                        IsActive = true
                    },
                    new Product
                    {
                        Name = "Bluetooth Hoparlör",
                        Description = "Su Geçirmez, 24 Saat Pil",
                        Price = 1299,
                        StockQuantity = 40,
                        ImageUrl = "https://images.unsplash.com/photo-1608043152269-423dbba4e7e1",
                        Rating = 4.3M,
                        Category = "Aksesuar",
                        IsActive = true
                    }
                };

                _context.Products.AddRange(sampleProducts);
                _context.SaveChanges();
                return sampleProducts;
            }

            return products;
        }

        public Product? GetById(int id)
        {
            return _context.Products.Find(id);
        }

        public Product? GetProductById(int id)
        {
            return _context.Products.FirstOrDefault(p => p.Id == id);
        }

        public void Add(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public void Update(Product product)
        {
            var existingProduct = _context.Products.Find(product.Id);
            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Category = product.Category;
                existingProduct.Price = product.Price;
                existingProduct.StockQuantity = product.StockQuantity;
                existingProduct.ImageUrl = product.ImageUrl;
                existingProduct.Rating = product.Rating;
                existingProduct.IsActive = product.IsActive;

                _context.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                // Check if product is used in any orders
                var hasOrders = _context.OrderItems.Any(oi => oi.ProductId == id);
                if (hasOrders)
                {
                    // If product has orders, just mark it as out of stock
                    product.StockQuantity = 0;
                    _context.SaveChanges();
                }
                else
                {
                    _context.Products.Remove(product);
                    _context.SaveChanges();
                }
            }
        }

        public bool UpdateStock(int id, int quantity, bool isDecrement = false)
        {
            var product = _context.Products.Find(id);
            if (product == null) return false;

            if (isDecrement)
            {
                if (product.StockQuantity < quantity) return false;
                product.StockQuantity -= quantity;
            }
            else
            {
                product.StockQuantity = quantity;
            }

            _context.SaveChanges();
            return true;
        }

        public List<Product> GetFeaturedProducts(int count = 8)
        {
            return _context.Products
                .OrderByDescending(p => p.Rating)
                .Take(count)
                .ToList();
        }

        public List<Product> GetLowStockProducts(int threshold = 10)
        {
            return _context.Products
                .Where(p => p.StockQuantity <= threshold)
                .OrderBy(p => p.StockQuantity)
                .ToList();
        }

        public List<Product> SearchProducts(string searchTerm)
        {
            return _context.Products
                .Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm))
                .OrderBy(p => p.Name)
                .ToList();
        }

        public bool IsInStock(int id, int requestedQuantity = 1)
        {
            var product = _context.Products.Find(id);
            return product != null && product.StockQuantity >= requestedQuantity;
        }

        public void DecrementStock(int id, int quantity = 1)
        {
            UpdateStock(id, quantity, true);
        }

        public void IncrementStock(int id, int quantity = 1)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                product.StockQuantity += quantity;
                _context.SaveChanges();
            }
        }

        public decimal GetTotalInventoryValue()
        {
            return _context.Products.Sum(p => p.Price * p.StockQuantity);
        }

        public void InitializeProducts()
        {
            if (!_context.Products.Any())
            {
                var sampleProducts = new List<Product>
                {
                    new Product
                    {
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
                        Name = "Akıllı Telefon",
                        Description = "128GB, 8GB RAM, 108MP Kamera",
                        Price = 14999,
                        StockQuantity = 24,
                        ImageUrl = "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9",
                        Rating = 4.8M,
                        Category = "Telefon",
                        IsActive = true
                    },
                    new Product
                    {
                        Name = "Kablosuz Kulaklık",
                        Description = "Aktif Gürültü Önleme, 30 Saat Pil Ömrü",
                        Price = 2499,
                        StockQuantity = 35,
                        ImageUrl = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e",
                        Rating = 4.6M,
                        Category = "Aksesuar",
                        IsActive = true
                    },
                    new Product
                    {
                        Name = "4K Smart TV",
                        Description = "55 inç, HDR, Android TV",
                        Price = 19999,
                        StockQuantity = 8,
                        ImageUrl = "https://images.unsplash.com/photo-1593784991095-a205069470b6",
                        Rating = 4.7M,
                        Category = "Televizyon",
                        IsActive = true
                    },
                    new Product
                    {
                        Name = "Oyun Konsolu",
                        Description = "1TB Depolama, 2 Kontrolcü",
                        Price = 12999,
                        StockQuantity = 15,
                        ImageUrl = "https://images.unsplash.com/photo-1486401899868-0e435ed85128",
                        Rating = 4.9M,
                        Category = "Oyun",
                        IsActive = true
                    },
                    new Product
                    {
                        Name = "Akıllı Saat",
                        Description = "Nabız Ölçer, GPS, Su Geçirmez",
                        Price = 3499,
                        StockQuantity = 30,
                        ImageUrl = "https://images.unsplash.com/photo-1546868871-7041f2a55e12",
                        Rating = 4.4M,
                        Category = "Aksesuar",
                        IsActive = true
                    },
                    new Product
                    {
                        Name = "Drone",
                        Description = "4K Kamera, 30dk Uçuş Süresi",
                        Price = 8999,
                        StockQuantity = 12,
                        ImageUrl = "https://images.unsplash.com/photo-1507582020474-9a35b7d455d9",
                        Rating = 4.6M,
                        Category = "Elektronik",
                        IsActive = true
                    },
                    new Product
                    {
                        Name = "Bluetooth Hoparlör",
                        Description = "Su Geçirmez, 24 Saat Pil",
                        Price = 1299,
                        StockQuantity = 40,
                        ImageUrl = "https://images.unsplash.com/photo-1608043152269-423dbba4e7e1",
                        Rating = 4.3M,
                        Category = "Aksesuar",
                        IsActive = true
                    }
                };

                _context.Products.AddRange(sampleProducts);
                _context.SaveChanges();
            }
        }

        public void ClearAllProducts()
        {
            var products = _context.Products.ToList();
            _context.Products.RemoveRange(products);
            _context.SaveChanges();
        }
    }
}
