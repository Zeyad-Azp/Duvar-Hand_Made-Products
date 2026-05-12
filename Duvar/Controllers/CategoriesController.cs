using Duvar.BLL.Services.Abstraction;
using Duvar.BLL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Duvar.Controllers;

public class CategoriesController : Controller
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index()
    {
        var vm = new CategoryListViewModel
        {
            Categories = await _categoryService.GetCategoriesWithProductsAsync()
        };
        return View(vm);
    }
}
