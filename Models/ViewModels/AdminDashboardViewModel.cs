using System.Collections.Generic;
using System.Linq;
using e_ticaret.Models;

namespace e_ticaret.ViewModels
{
    public class AdminDashboardViewModel
    {
        public List<Product> Products { get; set; } = new();
        public List<Order> RecentOrders { get; set; } = new();
        public int UserCount { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalRevenue => RecentOrders.Sum(o => o.Total);
        public int LowStockCount => Products.Count(p => p.StockQuantity < 5);
        public List<Product> LowStockProducts => Products.Where(p => p.StockQuantity < 5).ToList();
    }
} 