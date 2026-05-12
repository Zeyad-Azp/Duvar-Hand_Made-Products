using Duvar.BLL.Services.Abstraction;
using Duvar.BLL.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Duvar.Controllers.Admin;

[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public DashboardController(IProductService productService, ICategoryService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;
    }

    // GET /my-admin/mydashboard
    public async Task<IActionResult> Index()
    {
        var vm = new DashboardViewModel
        {
            TotalCategories = await _categoryService.GetTotalCategoriesCountAsync(),
            TotalProducts   = await _productService.GetTotalProductsCountAsync(),
            LatestProducts  = await _productService.GetLatestProductsAsync(5)
        };
        return View(vm);
    }
}
