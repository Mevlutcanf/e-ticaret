using System.Text.Json;
using e_ticaret.Models;

namespace e_ticaret.Data
{
    public static class UserRepository
    {
        private static string UsersFilePath = "users.json";
        private static List<User> _users = new();

        static UserRepository()
        {
            LoadUsers();
            if (!_users.Any(u => u.IsAdmin))
            {
                // Varsayılan admin kullanıcısı oluştur
                Add(new User
                {
                    Email = "admin@admin.com",
                    Password = "admin123",
                    FullName = "Admin",
                    IsAdmin = true
                });
            }
        }

        private static void LoadUsers()
        {
            if (File.Exists(UsersFilePath))
            {
                var json = File.ReadAllText(UsersFilePath);
                _users = JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
            }
            else
            {
                _users = new List<User>();
            }
        }

        private static void SaveUsers()
        {
            var json = JsonSerializer.Serialize(_users);
            File.WriteAllText(UsersFilePath, json);
        }

        public static User? GetByEmail(string email)
        {
            return _users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
        }

        public static bool Add(User user)
        {
            if (GetByEmail(user.Email) != null)
                return false;

            user.Id = _users.Count + 1;
            _users.Add(user);
            SaveUsers();
            return true;
        }

        public static User? GetById(int id)
        {
            return _users.FirstOrDefault(u => u.Id == id);
        }

        public static bool Update(User updatedUser)
        {
            var existingUser = GetById(updatedUser.Id);
            if (existingUser == null) return false;

            existingUser.FullName = updatedUser.FullName;
            existingUser.Email = updatedUser.Email;
            if (!string.IsNullOrEmpty(updatedUser.Password))
            {
                existingUser.Password = updatedUser.Password;
            }

            SaveUsers();
            return true;
        }

        public static List<User> GetAll()
        {
            return _users.ToList();
        }
    }
}
