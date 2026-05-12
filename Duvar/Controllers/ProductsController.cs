using Duvar.BLL.Services.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace Duvar.Controllers;

public class ProductsController : Controller
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IActionResult> Index(int? categoryId, string? search)
    {
        var vm = await _productService.GetProductListAsync(categoryId, search);
        return View(vm);
    }

    // AJAX endpoint for filtering
    [HttpGet]
    public async Task<IActionResult> FilterPartial(int? categoryId, string? search)
    {
        var products = await _productService.GetFilteredProductsAsync(categoryId, search);
        return PartialView("_ProductGrid", products);
    }

    public async Task<IActionResult> Details(int id)
    {
        var vm = await _productService.GetProductDetailsAsync(id);
        if (vm == null) return NotFound();
        return View(vm);
    }
}
