using Duvar.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Duvar.DAL.Data;

public class Duvar01DbContext : DbContext
{
    public Duvar01DbContext(DbContextOptions<Duvar01DbContext> options)
        : base(options) { }

    public DbSet<Admin>        Admins        => Set<Admin>();
    public DbSet<Category>     Categories    => Set<Category>();
    public DbSet<Product>      Products      => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProductImage>()
            .HasOne(pi => pi.Product)
            .WithMany(p => p.Images)
            .HasForeignKey(pi => pi.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasColumnType("decimal(18,2)");
    }
}
