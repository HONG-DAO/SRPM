using SRPM.Data;
using SRPM.Models;
using Microsoft.EntityFrameworkCore;

namespace SRPM.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<List<User>> GetAllAsync(string? role = null)
        {
            var query = _context.Users.AsQueryable();
            if (!string.IsNullOrEmpty(role))
                query = query.Where(u => u.Role == role);

            return await query.ToListAsync();
        }

        public async Task UpdateProfileAsync(string id, string name, string avatar, string bio, string links)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return;

            user.FullName = name;
            user.AvatarUrl = avatar;
            user.Bio = bio;
            user.SocialLinks = links;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRoleAsync(string id, string role)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return;

            user.Role = role;
            await _context.SaveChangesAsync();
        }
    }
}
