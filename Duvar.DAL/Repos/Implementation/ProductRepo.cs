using Duvar.DAL.Data;
using Duvar.DAL.Models;
using Duvar.DAL.Repos.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace Duvar.DAL.Repos.Implementation
{
    public class ProductRepo : IProductRepo
    {
        private readonly Duvar01DbContext _context;
        public ProductRepo(Duvar01DbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));
            await _context.Products.AddAsync(product);
        }

        public async Task DeleteAsync(int id)
        {
            Product? product = await _context.Products.FindAsync(id);
            if (product == null)
                throw new KeyNotFoundException($"product not found.");
            _context.Products.Remove(product);
        }

        public IQueryable<Product> GetAllAsync()
        {
            return _context.Products.OrderBy(c => c.Name)
                                          .Include(p => p.Images)
                                          .Include(p => p.Category);
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                                 .Include(p => p.Images)
                                 .Include(p => p.Category)
                                 .FirstOrDefaultAsync(p => p.Id == id);
        }

        public void Update(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));
            _context.Products.Update(product);
        }
    }
}
