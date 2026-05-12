using Duvar.DAL.Data;
using Duvar.DAL.Repos.Abstraction;
using Microsoft.EntityFrameworkCore.Storage;

namespace Duvar.DAL.Repos.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Duvar01DbContext _context;

        public UnitOfWork(Duvar01DbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
    }
}
