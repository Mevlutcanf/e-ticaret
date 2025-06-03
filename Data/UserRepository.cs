using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using e_ticaret.Models;
using Microsoft.EntityFrameworkCore;

namespace e_ticaret.Data
{
    public class UserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<User> GetAll()
        {
            var users = _context.Users
                .Include(u => u.Orders)
                .ToList();

            if (!users.Any())
            {
                // Add admin user if no users exist
                var adminUser = new User
                {
                    FullName = "Admin User",
                    Email = "admin@example.com",
                    Password = "123",
                    IsAdmin = true,
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(adminUser);
                _context.SaveChanges();
                return new List<User> { adminUser };
            }

            return users;
        }

        public User? GetById(int id)
        {
            return _context.Users
                .Include(u => u.Orders)
                .FirstOrDefault(u => u.Id == id);
        }

        public User? GetByEmail(string email)
        {
            return _context.Users
                .Include(u => u.Orders)
                .FirstOrDefault(u => u.Email == email);
        }

        public void Add(User user)
        {
            user.CreatedAt = DateTime.Now;
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                // Delete associated orders first
                var orders = _context.Orders.Where(o => o.UserId == id);
                _context.Orders.RemoveRange(orders);

                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }

        public bool ValidateCredentials(string email, string password)
        {
            var user = GetByEmail(email);
            if (user == null) return false;
            return user.Password == password;
        }

        public bool IsEmailUnique(string email)
        {
            return !_context.Users.Any(u => u.Email == email);
        }

        public List<User> GetTopCustomers(int count = 5)
        {
            return _context.Users
                .Include(u => u.Orders)
                .OrderByDescending(u => u.Orders.Count)
                .Take(count)
                .ToList();
        }

        public decimal GetTotalSpentByUser(int userId)
        {
            return _context.Orders
                .Where(o => o.UserId == userId)
                .Sum(o => o.TotalAmount);
        }

        public void ClearAllNonAdminUsers()
        {
            var nonAdminUsers = _context.Users.Where(u => !u.IsAdmin).ToList();
            _context.Users.RemoveRange(nonAdminUsers);
            _context.SaveChanges();
        }

        public User? GetUserByEmailAndPassword(string email, string password)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
        }
    }
}
