namespace Ecommerce.Models
{
    public class SubCategoryViewModel
    {
        public Category Category { get; set; }
        public List<Category> SubCategories { get; set; }

        public List<Product> Products { get; set; }

        public List<Category> GetSubCategories(int parentCategoryId)
        {
            CategoryRepository categoryRepository = new CategoryRepository();
            SubCategories = categoryRepository.GetSubCategories(parentCategoryId).ToList();
            return SubCategories;
        }

        public List<Product> GetProducts(int categoryId)
        {
            ProductRepository productRepository = new ProductRepository();
            Products = productRepository.GetProductsByCategory(categoryId).ToList();
            return Products;
        }
    }
}
