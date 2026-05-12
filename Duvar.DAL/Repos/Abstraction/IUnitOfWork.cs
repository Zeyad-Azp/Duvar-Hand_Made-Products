using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Duvar.DAL.Repos.Abstraction
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
        
    }
}
