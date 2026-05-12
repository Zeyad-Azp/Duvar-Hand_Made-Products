using Duvar.DAL.Models;
using Duvar.DAL.Repos.Abstraction;
using Duvar.BLL.Services.Abstraction;

namespace Duvar.BLL.Services.Implementation
{
    public class ProductImageService : IProductImageService
    {
        private readonly IProductImageRepo _productImageRepo;
        private readonly IUnitOfWork _unitOfWork;
        public ProductImageService(IProductImageRepo productImageRepo , IUnitOfWork unitOfWork)
        {
            _productImageRepo = productImageRepo;
            _unitOfWork = unitOfWork;
        }
        public async Task AddAsync(ProductImage productImage)
        {
            if (productImage == null)
                throw new ArgumentNullException(nameof(productImage));
            await _productImageRepo.AddAsync(productImage);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var productImage = await _productImageRepo.GetByIdAsync(id);
            if (productImage == null)
                throw new KeyNotFoundException($"Product image not found.");
            await _productImageRepo.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<ProductImage>> GetAllAsync()
        {
            return await _productImageRepo.GetAllAsync();
        }

        public async Task<ProductImage?> GetByIdAsync(int id)
        {
            return await _productImageRepo.GetByIdAsync(id);
        }
    }
}
