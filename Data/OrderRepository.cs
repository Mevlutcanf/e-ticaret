using System.Text.Json;
using e_ticaret.Models;

namespace e_ticaret.Data
{
    public static class OrderRepository
    {
        private static string OrdersFilePath = "orders.json";
        private static List<Order> _orders = new();

        static OrderRepository()
        {
            LoadOrders();
        }

        private static void LoadOrders()
        {
            if (File.Exists(OrdersFilePath))
            {
                var json = File.ReadAllText(OrdersFilePath);
                _orders = JsonSerializer.Deserialize<List<Order>>(json) ?? new List<Order>();
            }
        }

        private static void SaveOrders()
        {
            var json = JsonSerializer.Serialize(_orders);
            File.WriteAllText(OrdersFilePath, json);
        }

        public static void AddOrder(Order order)
        {
            _orders.Add(order);
            SaveOrders();
        }

        public static List<Order> GetUserOrders(string userId)
        {
            return _orders.Where(o => o.UserId == userId)
                         .OrderByDescending(o => o.OrderDate)
                         .ToList();
        }

        public static List<Order> GetAllOrders()
        {
            return _orders.OrderByDescending(o => o.OrderDate).ToList();
        }

        public static Order? GetOrderById(int id)
        {
            return _orders.FirstOrDefault(o => o.Id == id);
        }

        public static void UpdateStatus(int id, string status)
        {
            var order = GetOrderById(id);
            if (order != null)
            {
                order.OrderStatus = status;
                SaveOrders();
            }
        }

        public static void UpdateOrder(Order updatedOrder)
        {
            var existingOrder = GetOrderById(updatedOrder.Id);
            if (existingOrder != null)
            {
                existingOrder.OrderStatus = updatedOrder.OrderStatus;
                existingOrder.DeliveryDate = updatedOrder.DeliveryDate;
                SaveOrders();
            }
        }
    }
}
