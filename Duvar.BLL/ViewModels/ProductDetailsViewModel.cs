using Duvar.DAL.Models;

namespace Duvar.BLL.ViewModels
{
    public class ProductDetailsViewModel
    {
        public Product Product { get; set; } = null!;
        public List<ProductImage> Images { get; set; } = new();
        public ProductImage? MainImage { get; set; }
        public List<ProductImage> AdditionalImages { get; set; } = new();
        public List<Product> RelatedProducts { get; set; } = new();
    }
}
