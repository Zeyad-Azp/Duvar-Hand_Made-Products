using Duvar.DAL.Data;
using Duvar.DAL.Models;
using Duvar.BLL.Services.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace Duvar.BLL.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly Duvar01DbContext _db;

        public AuthService(Duvar01DbContext db)
        {
            _db = db;
        }

        public async Task<Admin?> AuthenticateAsync(string username, string password)
        {
            var admin = await _db.Admins
                .FirstOrDefaultAsync(a => a.Username == username);

            if (admin == null || !BCrypt.Net.BCrypt.Verify(password, admin.PasswordHash))
            {
                return null;
            }

            return admin;
        }
    }
}
