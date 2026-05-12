using Duvar.DAL.Data;
using Duvar.DAL.Repos.Abstraction;
using Duvar.DAL.Repos.Implementation;
using Duvar.BLL.Services.Abstraction;
using Duvar.BLL.Services.Implementation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

// ════════════════════════════════════════════════════════════════
//  DUVAR — Program Entry Point
// ════════════════════════════════════════════════════════════════
var builder = WebApplication.CreateBuilder(args);

// ── Database ──────────────────────────────────────────────────
builder.Services.AddDbContext<Duvar01DbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Cookie Authentication ─────────────────────────────────────
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opts =>
    {
        opts.LoginPath          = "/my-admin/login";
        opts.LogoutPath         = "/my-admin/logout";
        opts.AccessDeniedPath   = "/my-admin/login";
        opts.ExpireTimeSpan     = TimeSpan.FromHours(8);
        opts.SlidingExpiration  = true;
        opts.Cookie.HttpOnly    = true;
        opts.Cookie.Name        = "DuvarAdmin";
        opts.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

builder.Services.AddAuthorization();
//register services
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductImageService, ProductImageService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<ICategoryRepo, CategoryRepo>();
builder.Services.AddScoped<IProductRepo, ProductRepo>();
builder.Services.AddScoped<IProductImageRepo, ProductImageRepo>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// ── Migrate & Seed ────────────────────────────────────────────
await DbSeeder.SeedAsync(app.Services);

// ── Middleware Pipeline ───────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// ── Routes ────────────────────────────────────────────────────

// Admin: Auth
app.MapControllerRoute(
    name: "admin_login",
    pattern: "my-admin/login",
    defaults: new { controller = "Auth", action = "Login"});

app.MapControllerRoute(
    name: "admin_logout",
    pattern: "my-admin/logout",
    defaults: new { controller = "Auth", action = "Logout"});

// Admin: Dashboard
app.MapControllerRoute(
    name: "admin_dashboard",
    pattern: "my-admin/mydashboard",
    defaults: new { controller = "Dashboard", action = "Index"});

// Admin: Categories CRUD
app.MapControllerRoute(
    name: "admin_categories",
    pattern: "my-admin/categories/{action=Index}/{id?}",
    defaults: new { controller = "CategoriesAdmin"});

// Admin: Products CRUD + Ajax
app.MapControllerRoute(
    name: "admin_products",
    pattern: "my-admin/products/{action=Index}/{id?}",
    defaults: new { controller = "ProductsAdmin"});

// Public
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
