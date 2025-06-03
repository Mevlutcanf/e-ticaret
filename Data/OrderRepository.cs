using System;
using System.Collections.Generic;
using System.Linq;
using e_ticaret.Models;
using Microsoft.EntityFrameworkCore;

namespace e_ticaret.Data
{
    public class OrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Order> GetAll()
        {
            return _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .ToList();
        }

        public Order? GetById(int id)
        {
            return _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefault(o => o.Id == id);
        }

        public List<Order> GetUserOrders(int userId)
        {
            return _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        public void AddOrder(Order order)
        {
            order.OrderNumber = GenerateOrderNumber();
            order.OrderDate = DateTime.Now;
            order.OrderStatus = "Hazırlanıyor";
            order.EstimatedDeliveryDate = DateTime.Now.AddDays(3);

            // Detach existing products from the context
            foreach (var item in order.Items)
            {
                var product = _context.Products.Find(item.ProductId);
                if (product != null)
                {
                    item.Product = product;
                    product.StockQuantity -= item.Quantity;
                    _context.Entry(product).State = EntityState.Modified;
                }
            }

            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        public void Update(Order order)
        {
            _context.Orders.Update(order);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var order = _context.Orders.Find(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                _context.SaveChanges();
            }
        }

        private string GenerateOrderNumber()
        {
            return Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        }

        public List<Order> GetAllOrders()
        {
            return _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        public Order? GetOrderById(int id)
        {
            return _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Include(o => o.User)
                .FirstOrDefault(o => o.Id == id);
        }

        public void UpdateStatus(int id, string status)
        {
            var order = GetOrderById(id);
            if (order != null)
            {
                order.OrderStatus = status;
                _context.SaveChanges();
            }
        }

        public void UpdateOrder(Order updatedOrder)
        {
            var existingOrder = GetOrderById(updatedOrder.Id);
            if (existingOrder != null)
            {
                existingOrder.OrderStatus = updatedOrder.OrderStatus;
                existingOrder.DeliveryDate = updatedOrder.DeliveryDate;
                _context.SaveChanges();
            }
        }

        public void ClearAllOrders()
        {
            var orders = _context.Orders.ToList();
            _context.Orders.RemoveRange(orders);
            _context.SaveChanges();
        }

        public List<Order> GetOrdersByDateRange(DateTime startDate, DateTime endDate)
        {
            return _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Include(o => o.User)
                .Where(o => o.DeliveryDate >= startDate && o.DeliveryDate <= endDate)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }
    }
}
