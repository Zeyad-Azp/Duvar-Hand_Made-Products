using Duvar.BLL.Services.Abstraction;
using Duvar.BLL.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Duvar.Controllers.Admin;

[Authorize(Roles = "Admin")]
public class CategoriesAdminController : Controller
{
    private readonly ICategoryService _categoryService;

    public CategoriesAdminController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    // GET /my-admin/categories
    public async Task<IActionResult> Index()
    {
        var cats = await _categoryService.GetCategoriesWithProductsAsync();
        return View(cats);
    }

    // GET /my-admin/categories/create
    public IActionResult Create() => View(new CategoryFormViewModel());

    // POST /my-admin/categories/create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        try
        {
            await _categoryService.AddAsync(model);
            TempData["Success"] = $"Category '{model.Name}' created successfully.";
            return Redirect("/my-admin/categories");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("ImageFile", ex.Message);
            return View(model);
        }
    }

    // GET /my-admin/categories/edit/{id}
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var catVM = await _categoryService.GetByIdAsync(id);
            if (catVM == null) return NotFound();
            return View(catVM);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // POST /my-admin/categories/edit/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoryFormViewModel model)
    {
        if (!ModelState.IsValid) 
            return View(model);

        try
        {
            await _categoryService.Update(model);
            TempData["Success"] = $"Category '{model.Name}' updated successfully.";
            return Redirect("/my-admin/categories");
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("ImageFile", ex.Message);
            return View(model);
        }
    }

    // POST /my-admin/categories/delete/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _categoryService.DeleteAsync(id);
            TempData["Success"] = "Category deleted.";
            return Redirect("/my-admin/categories");
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
