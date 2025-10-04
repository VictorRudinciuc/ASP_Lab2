using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using projectAsp.Models;

namespace projectAsp.Services
{
    public class UserStore
    {
        private readonly string _filePath;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        public UserStore()
        {
            var dataDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data");
            if (!Directory.Exists(dataDir)) Directory.CreateDirectory(dataDir);
            _filePath = Path.Combine(dataDir, "users.json");
            if (!File.Exists(_filePath)) File.WriteAllText(_filePath, "[]");
        }

        private async Task<List<User>> ReadAllAsync()
        {
            await _lock.WaitAsync();
            try
            {
                using var s = File.OpenRead(_filePath);
                var users = await JsonSerializer.DeserializeAsync<List<User>>(s);
                return users ?? new List<User>();
            }
            finally { _lock.Release(); }
        }

        private async Task SaveAllAsync(List<User> users)
        {
            await _lock.WaitAsync();
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                using var s = File.Create(_filePath);
                await JsonSerializer.SerializeAsync(s, users, options);
            }
            finally { _lock.Release(); }
        }

        public async Task<User> FindByEmailAsync(string email)
            => (await ReadAllAsync()).FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

        public async Task CreateAsync(User user)
        {
            var all = await ReadAllAsync();
            all.Add(user);
            await SaveAllAsync(all);
        }

        public async Task UpdateAsync(User user)
        {
            var all = await ReadAllAsync();
            var idx = all.FindIndex(u => u.Id == user.Id);
            if (idx >= 0)
            {
                all[idx] = user;
                await SaveAllAsync(all);
            }
        }

        public async Task<List<User>> GetAllAsync() => await ReadAllAsync();
    }
}