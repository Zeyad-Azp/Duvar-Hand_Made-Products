using Duvar.DAL.Models;

namespace Duvar.BLL.Services.Abstraction
{
    public interface IAuthService
    {
        Task<Admin?> AuthenticateAsync(string username, string password);
    }
}
