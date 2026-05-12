using Duvar.DAL.Data;
using Duvar.DAL.Models;
using Duvar.DAL.Repos.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace Duvar.DAL.Repos.Implementation
{
    public class CategoryRepo : ICategoryRepo
    {
        private readonly Duvar01DbContext _context;
        public CategoryRepo(Duvar01DbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category)); 
            await _context.Categories.AddAsync(category);
        }

        public async Task DeleteAsync(int id)
        {
            Category? category = await _context.Categories.FindAsync(id);
            if (category == null)
                throw new KeyNotFoundException($"Category not found.");
            _context.Categories.Remove(category);
        }

        public IQueryable<Category> GetAllAsync()
        {
            return _context.Categories.OrderBy(c => c.Name);
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public void Update(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));
            _context.Categories.Update(category);
        }
    }
}
