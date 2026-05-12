using Duvar.BLL.Services.Abstraction;
using Duvar.BLL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Duvar.Controllers;

public class HomeController : Controller
{
    private readonly ICategoryService _categoryService;
    private readonly IProductService _productService;

    public HomeController(ICategoryService categoryService, IProductService productService)
    {
        _categoryService = categoryService;
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        var vm = new HomeViewModel
        {
            FeaturedCategories = await _categoryService.GetFeaturedCategoriesAsync(4),
            FeaturedProducts = await _productService.GetLatestProductsAsync(6)
        };
        return View(vm);
    }
}
