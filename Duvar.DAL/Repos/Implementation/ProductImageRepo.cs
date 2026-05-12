using Duvar.DAL.Data;
using Duvar.DAL.Models;
using Duvar.DAL.Repos.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace Duvar.DAL.Repos.Implementation
{
    public class ProductImageRepo : IProductImageRepo
    {
        private readonly Duvar01DbContext _context;
        public ProductImageRepo(Duvar01DbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(ProductImage productImage)
        {
            await _context.ProductImages.AddAsync(productImage);
        }

        public async Task DeleteAsync(int id)
        {
            ProductImage? productImage = await _context.ProductImages.FindAsync(id);
            if (productImage == null)
                throw new KeyNotFoundException($"Product image not found.");
            _context.ProductImages.Remove(productImage);
        }

        public async Task<List<ProductImage>> GetAllAsync()
        {
            return await _context.ProductImages.ToListAsync();
        }

        public async Task<ProductImage?> GetByIdAsync(int id)
        {
             return await _context.ProductImages.FindAsync(id);
        }
    }
}
