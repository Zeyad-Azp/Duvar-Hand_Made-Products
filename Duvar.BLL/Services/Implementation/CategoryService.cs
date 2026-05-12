using Duvar.DAL.Models;
using Duvar.DAL.Repos.Abstraction;
using Duvar.BLL.Services.Abstraction;
using Duvar.BLL.ViewModels.Admin;
using Microsoft.EntityFrameworkCore;

namespace Duvar.BLL.Services.Implementation
{
    public class CategoryService : ICategoryService
    { 
        private readonly ICategoryRepo _categoryRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageService _imageService;
        
        public CategoryService(ICategoryRepo categoryRepo , IUnitOfWork unitOfWork, IImageService imageService)
        {
            _categoryRepo = categoryRepo;
            _unitOfWork = unitOfWork;
            _imageService = imageService;
        }
        public async Task AddAsync(CategoryFormViewModel category)
        {
            string? imagePath = null;
            if (category.ImageFile != null && category.ImageFile.Length > 0)
            {
                imagePath = await _imageService.SaveImageAsync(category.ImageFile, "categories");
            }

            await _categoryRepo.AddAsync(new Category
            {
                Name = category.Name,
                Description = category.Description,
                ImagePath = imagePath ?? category.CurrentImagePath
            });
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null)
                throw new KeyNotFoundException($"Category not found.");

            if (!string.IsNullOrEmpty(category.ImagePath))
            {
                _imageService.DeleteImage(category.ImagePath);
            }

            await _categoryRepo.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<CategoryFormViewModel>> GetAllAsync()
        {
            var categories = await _categoryRepo.GetAllAsync().ToListAsync();
            if (categories == null || !categories.Any())
                throw new KeyNotFoundException($"No categories found.");
            return categories.Select(c => new CategoryFormViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                CurrentImagePath = c.ImagePath
            }).ToList();
        }

        public async Task<CategoryFormViewModel?> GetByIdAsync(int id)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null)
                throw new KeyNotFoundException($"Category not found.");
            return new CategoryFormViewModel { CurrentImagePath = category.ImagePath, Description = category.Description, Name = category.Name, Id = category.Id };
        }

        public async Task Update(CategoryFormViewModel category)
        {
            var existingCategory = _categoryRepo.GetByIdAsync(category.Id).Result;
            if (existingCategory == null)
                throw new KeyNotFoundException($"Category not found.");

            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;

            if (category.ImageFile != null && category.ImageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(existingCategory.ImagePath))
                {
                    _imageService.DeleteImage(existingCategory.ImagePath);
                }
                existingCategory.ImagePath = _imageService.SaveImageAsync(category.ImageFile, "categories").Result;
            }
            else
            {
                existingCategory.ImagePath = category.CurrentImagePath;
            }

            _categoryRepo.Update(existingCategory);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<Category>> GetCategoriesWithProductsAsync()
        {
            return await _categoryRepo.GetAllAsync()
                .Include(c => c.Products)
                .ToListAsync();
        }

        public async Task<List<Category>> GetFeaturedCategoriesAsync(int count)
        {
            return await _categoryRepo.GetAllAsync()
                .Take(count)
                .ToListAsync();
        }

        public async Task<int> GetTotalCategoriesCountAsync()
        {
            return await _categoryRepo.GetAllAsync().CountAsync();
        }
    }
}
