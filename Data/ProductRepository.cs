using System.Collections.Generic;
using System.Linq;
using e_ticaret.Models;

namespace e_ticaret.Data
{
    public static class ProductRepository
    {
        private static List<Product> _products = new List<Product>
        {
            new Product
            {
                Id = 1,
                Name = "Gaming Laptop",
                Description = "16GB RAM, RTX 4060, i7 işlemci",
                Price = 39999,
                ImageUrl = "https://images.unsplash.com/photo-1605134513573-384dcf99a44c?q=80&w=2070&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",  // Gaming laptop
                StockQuantity = 15,
                Rating = 4.5
            },
            new Product
            {
                Id = 2,
                Name = "Akıllı Telefon",
                Description = "128GB, 8GB RAM, 108MP Kamera",
                Price = 14999,
                ImageUrl = "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9",
                StockQuantity = 25,
                Rating = 4.8
            },
            new Product
            {
                Id = 3,
                Name = "Kablosuz Kulaklık",
                Description = "Aktif Gürültü Önleme, 30 Saat Pil",
                Price = 2499,
                ImageUrl = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e",
                StockQuantity = 50,
                Rating = 4.2
            },
            new Product
            {
                Id = 4,
                Name = "4K Monitor",
                Description = "32 inç, 144Hz, HDR",
                Price = 12999,
                ImageUrl = "https://plus.unsplash.com/premium_photo-1680721575441-18d5a0567269?q=80&w=2004&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",  // Monitor
                StockQuantity = 10,
                Rating = 4.7
            },
            new Product
            {
                Id = 5,
                Name = "Mekanik Klavye",
                Description = "RGB Aydınlatma, Blue Switch",
                Price = 1899,
                ImageUrl = "https://images.unsplash.com/photo-1601445638532-3c6f6c3aa1d6",  // Mekanik klavye
                StockQuantity = 30,
                Rating = 4.6
            },
            new Product
            {
                Id = 6,
                Name = "Gamepad",
                Description = "Drift hatası olmayacak şekilde tasarlandı",
                Price = 899,
                ImageUrl = "https://images.unsplash.com/photo-1712390978993-faa4edb44d48?q=80&w=1937&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",
                StockQuantity = 45,
                Rating = 4.3
            },
            new Product
            {
                Id = 7,
                Name = "Webcam",
                Description = "1080p Full HD, Gürültü Önleyici Mikrofon",
                Price = 799,
                ImageUrl = "https://images.unsplash.com/photo-1670278458296-00ff8a63141e?q=80&w=2070&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",  // Webcam
                StockQuantity = 20,
                Rating = 4.1
            },
            new Product
            {
                Id = 8,
                Name = "Tablet",
                Description = "10.4 inç, 64GB, Wifi",
                Price = 4999,
                ImageUrl = "https://images.unsplash.com/photo-1561154464-82e9adf32764",
                StockQuantity = 15,
                Rating = 4.6
            },
            new Product
            {
                Id = 9,
                Name = "SSD Disk",
                Description = "1TB, NVMe M.2",
                Price = 1699,
                ImageUrl = "https://plus.unsplash.com/premium_photo-1721133221361-4f2b2af3b6fe?q=80&w=2070&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D",  // SSD
                StockQuantity = 50,
                Rating = 4.8
            }
        };

        public static List<Product> GetAll() => _products;

        public static bool UpdateStock(int productId, int quantity)
        {
            var product = _products.FirstOrDefault(p => p.Id == productId);
            if (product == null || product.StockQuantity < quantity) return false;

            product.StockQuantity -= quantity;
            return true;
        }

        public static Product? GetById(int id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }

        public static void Update(Product product)
        {
            var existingProduct = GetById(product.Id);
            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.ImageUrl = product.ImageUrl;
                existingProduct.StockQuantity = product.StockQuantity;
            }
        }

        public static void Add(Product product)
        {
            product.Id = _products.Count > 0 ? _products.Max(p => p.Id) + 1 : 1;
            _products.Add(product);
        }
    }
}
