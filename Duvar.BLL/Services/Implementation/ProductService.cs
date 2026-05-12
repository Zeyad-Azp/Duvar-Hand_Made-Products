using Duvar.DAL.Models;
using Duvar.DAL.Repos.Abstraction;
using Duvar.BLL.Services.Abstraction;
using Duvar.BLL.ViewModels;
using Duvar.BLL.ViewModels.Admin;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace Duvar.BLL.Services.Implementation
{
    public class ProductService : IProductService
    {
        private readonly IProductRepo _productRepo;
        private readonly ICategoryRepo _categoryRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageService _imageService;
        private readonly IProductImageRepo _productImageRepo;
        public ProductService(IProductRepo productRepo , IUnitOfWork unitOfWork , IImageService imageService , ICategoryRepo categoryRepo , IProductImageRepo productImageRepo)
        {
            _imageService = imageService;
            _productRepo = productRepo;
            _unitOfWork = unitOfWork;
            _categoryRepo = categoryRepo;
            _productImageRepo = productImageRepo;
        }
        public async Task AddAsync(ProductFormViewModel productVM)
        {
            var validFiles = productVM.NewImages?.Where(f => f.Length > 0).ToList() ?? new List<IFormFile>();

            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var product = new Product
                {
                    Name = productVM.Name,
                    Description = productVM.Description,
                    Price = productVM.Price,
                    CategoryId = productVM.CategoryId
                };

                await _productRepo.AddAsync(product);
                await _unitOfWork.SaveChangesAsync(); 

                if (validFiles.Any())
                {
                    await ProcessProductImages(product.Id, validFiles, productVM.MainImageFileName);
                }

                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task ProcessProductImages(int productId, List<IFormFile> files, string? mainFileName)
        {
            bool anyMainSet = false;

            foreach (var file in files)
            {
                var path = await _imageService.SaveImageAsync(file, "products");

                bool isMain = !string.IsNullOrEmpty(mainFileName)
                    ? string.Equals(Path.GetFileName(file.FileName), Path.GetFileName(mainFileName), StringComparison.OrdinalIgnoreCase)
                    : false;

                if (!anyMainSet && (isMain || file == files.First()))
                {
                    isMain = true;
                    anyMainSet = true;
                }

                await _productImageRepo.AddAsync(new ProductImage
                {
                    ProductId = productId,
                    ImagePath = path,
                    IsMainImage = isMain
                });
            }
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);

            if (product == null)
                throw new KeyNotFoundException($"Product not found.");

            if (product.Images != null && product.Images.Any())
            {
                foreach (var img in product.Images)
                {
                    _imageService.DeleteImage(img.ImagePath);
                }
            }
            await _productRepo.DeleteAsync(product.Id);
            await _unitOfWork.SaveChangesAsync();
        }

        public IQueryable<ProductFormViewModel> GetAllAsync(int? categoryId)
        {
            var query = _productRepo.GetAllAsync();

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            return query.OrderByDescending(p => p.Id).Select(p => new ProductFormViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoryId,
                ExistingImages = p.Images.Select(i => new ProductImage
                {
                    Id = i.Id,
                    ImagePath = i.ImagePath,
                    IsMainImage = i.IsMainImage
                }).ToList()
            });
        }

        public async Task<AdminProductListViewModel> GetAdminProductListAsync(int? categoryId)
        {
            return new AdminProductListViewModel
            {
                Products = await GetAllAsync(categoryId).Select(p => new Product
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    CategoryId = p.CategoryId,
                    Images = p.ExistingImages.Select(i => new ProductImage
                    {
                        Id = i.Id,
                        ImagePath = i.ImagePath,
                        IsMainImage = i.IsMainImage
                    }).ToList()
                }).ToListAsync(),
                Categories = await _categoryRepo.GetAllAsync().ToListAsync(),
                FilterCategoryId = categoryId
            };
        }

        public async Task<ProductFormViewModel?> GetByIdAsync(int id)
        {
            return await _productRepo.GetByIdAsync(id).ContinueWith(t =>
            {
                var p = t.Result;
                if (p == null) 
                    return null;
                return new ProductFormViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    CategoryId = p.CategoryId,
                    ExistingImages = p.Images.Select(i => new ProductImage
                    {
                        Id = i.Id,
                        ImagePath = i.ImagePath,
                        IsMainImage = i.IsMainImage
                    }).ToList()
                };
            });
        }

        public async Task Update(int id, ProductFormViewModel productVM)
        {
            var product = await _productRepo.GetByIdAsync(id);

            if (product == null) throw new Exception("Product not found");

            product.Name = productVM.Name;
            product.Description = productVM.Description;
            product.Price = productVM.Price;
            product.CategoryId = productVM.CategoryId;

            if (!string.IsNullOrWhiteSpace(productVM.DeleteImageIds))
            {
                HandleImageDeletions(product, productVM.DeleteImageIds);
            }

            if (!string.IsNullOrWhiteSpace(productVM.MainImageFileName))
            {
                // If a new image is selected as main, clear the existing image main index
                productVM.MainImageIndex = null;
            }

            if (productVM.MainImageIndex.HasValue)
            {
                foreach (var img in product.Images)
                    img.IsMainImage = img.Id == productVM.MainImageIndex.Value;
            }
            else if (!string.IsNullOrWhiteSpace(productVM.MainImageFileName))
            {
                // Clear any existing main image flags since a new one is the main
                foreach (var img in product.Images)
                    img.IsMainImage = false;
            }

            var newFiles = productVM.NewImages?.Where(f => f.Length > 0).ToList();
            if (newFiles != null && newFiles.Any())
            {
                await HandleNewImages(product, newFiles, productVM.MainImageFileName);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public void HandleImageDeletions(Product product, string deleteImageIds)
        {
            var ids = deleteImageIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s.Trim(), out var n) ? n : 0)
                .Where(n => n > 0).ToList();

            foreach (var imgId in ids)
            {
                var img = product.Images.FirstOrDefault(i => i.Id == imgId);
                if (img != null)
                {
                    _imageService.DeleteImage(img.ImagePath);
                    product.Images.Remove(img); 
                }
            }
        }

        public async Task HandleNewImages(Product product, List<IFormFile> newFiles, string? mainImageFileName = null)
        {
            bool hasMain = product.Images.Any(i => i.IsMainImage);

            foreach (var file in newFiles)
            {
                var path = await _imageService.SaveImageAsync(file, "products");
                
                bool isMainForThisFile = !string.IsNullOrEmpty(mainImageFileName) 
                    ? string.Equals(Path.GetFileName(file.FileName), Path.GetFileName(mainImageFileName), StringComparison.OrdinalIgnoreCase)
                    : false;

                if (isMainForThisFile) 
                {
                    hasMain = true;
                }

                product.Images.Add(new ProductImage
                {
                    ImagePath = path,
                    IsMainImage = isMainForThisFile || (!hasMain && file == newFiles.First())
                });
                
                if (!hasMain) hasMain = true;
            }
        }

        public async Task SetMainImageAsync(int imageId, int productId)
        {
            var images = await _productImageRepo.GetAllAsync();
            var productImages = images.Where(i => i.ProductId == productId).ToList();
            foreach (var img in productImages)
            {
                img.IsMainImage = img.Id == imageId;
            }
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ProductListViewModel> GetProductListAsync(int? categoryId, string? search)
        {
            var query = _productRepo.GetAllAsync();

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Name.Contains(search) || (p.Description != null && p.Description.Contains(search)));

            var category = categoryId.HasValue ? await _categoryRepo.GetByIdAsync(categoryId.Value) : null;
            
            return new ProductListViewModel
            {
                Products = await query.OrderByDescending(p => p.Id).ToListAsync(),
                Categories = await _categoryRepo.GetAllAsync().OrderBy(c => c.Name).ToListAsync(),
                SelectedCategoryId = categoryId,
                SelectedCategoryName = category?.Name,
                SearchTerm = search
            };
        }

        public async Task<List<Product>> GetFilteredProductsAsync(int? categoryId, string? search)
        {
            var query = _productRepo.GetAllAsync();

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Name.Contains(search) || (p.Description != null && p.Description.Contains(search)));

            return await query.OrderByDescending(p => p.Id).ToListAsync();
        }

        public async Task<ProductDetailsViewModel?> GetProductDetailsAsync(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return null;

            var related = await _productRepo.GetAllAsync()
                .Where(p => p.CategoryId == product.CategoryId && p.Id != id)
                .Take(4)
                .ToListAsync();

            var mainImage = product.Images?.FirstOrDefault(i => i.IsMainImage) ?? product.Images?.FirstOrDefault();

            return new ProductDetailsViewModel
            {
                Product = product,
                Images = product.Images?.OrderByDescending(i => i.IsMainImage).ToList() ?? new List<ProductImage>(),
                MainImage = mainImage,
                AdditionalImages = product.Images?.Where(i => !i.IsMainImage).ToList() ?? new List<ProductImage>(),
                RelatedProducts = related
            };
        }

        public async Task<List<Product>> GetLatestProductsAsync(int count)
        {
            return await _productRepo.GetAllAsync().OrderByDescending(p => p.Id).Take(count).ToListAsync();
        }

        public async Task<int> GetTotalProductsCountAsync()
        {
            return await _productRepo.GetAllAsync().CountAsync();
        }
    }
}
