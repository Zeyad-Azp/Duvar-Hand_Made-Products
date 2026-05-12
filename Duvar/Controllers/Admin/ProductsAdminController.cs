using Duvar.BLL.Services.Abstraction;
using Duvar.BLL.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Duvar.Controllers.Admin;

[Authorize(Roles = "Admin")]
public class ProductsAdminController : Controller
{
    private readonly ICategoryService _categoryService;
    private readonly IProductService _productService;

    public ProductsAdminController(ICategoryService categoryService, IProductService productService)
    {
        _categoryService = categoryService;
        _productService = productService;
    }

    // ── GET /my-admin/products ────────────────────────────────────
    public async Task<IActionResult> Index(int? categoryId)
    {
        var vm = await _productService.GetAdminProductListAsync(categoryId);
        return View(vm);
    }

    // ── GET /my-admin/products/create ────────────────────────────
    public async Task<IActionResult> Create()
    {
        return View(new ProductFormViewModel
        {
            Categories = await _categoryService.GetCategoriesWithProductsAsync()
        });
    }

    // ── POST /my-admin/products/create ───────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductFormViewModel model)
    {
        model.Categories = await _categoryService.GetCategoriesWithProductsAsync();

        if (!ModelState.IsValid) return View(model);

        var validFiles = model.NewImages?.Where(f => f is { Length: > 0 }).ToList() ?? new List<IFormFile>();

        if (!validFiles.Any())
        {
            ModelState.AddModelError("NewImages", "Please upload at least one product image.");
            return View(model);
        }

        try
        {
            await _productService.AddAsync(model);
            TempData["Success"] = $"Product '{model.Name}' created successfully.";
            return Redirect("/my-admin/products");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "An error occurred while saving: " + ex.Message);
            return View(model);
        }
    }

    // ── GET /my-admin/products/edit/{id} ─────────────────────────
    public async Task<IActionResult> Edit(int id)
    {
        var productVM = await _productService.GetByIdAsync(id);
        if (productVM == null) return NotFound();

        productVM.Categories = await _categoryService.GetCategoriesWithProductsAsync();
        return View(productVM);
    }

    // ── POST /my-admin/products/edit/{id} ────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductFormViewModel model)
    {
        ModelState.Remove("Categories");
        ModelState.Remove("ExistingImages");
        ModelState.Remove("MainImageFileName");

        if (!ModelState.IsValid) 
        {
            model.Categories = await _categoryService.GetCategoriesWithProductsAsync();
            var productVM = await _productService.GetByIdAsync(id);
            if (productVM != null)
            {
                model.ExistingImages = productVM.ExistingImages;
            }
            return View(model);
        }

        try
        {
            await _productService.Update(id, model);
            TempData["Success"] = $"Product '{model.Name}' updated.";
            return Redirect("/my-admin/products");
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    // ── POST /my-admin/products/delete/{id} ──────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _productService.DeleteAsync(id);
            TempData["Success"] = "Product deleted.";
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        
        return Redirect("/my-admin/products");
    }

    // ── POST /my-admin/products/set-main-image  (AJAX) ───────────
    [HttpPost]
    public async Task<IActionResult> SetMainImage(int imageId, int productId)
    {
        await _productService.SetMainImageAsync(imageId, productId);
        return Json(new { success = true });
    }
}
