using Microsoft.Data.SqlClient;
using NuGet.Protocol;

namespace Ecommerce.Models
{
    public class ProductRepository : GenericRepository<Product>
    {
        private readonly string connString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=newDb;Integrated Security=True;Trust Server Certificate=True";

        public ProductRepository() : base(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=newDb;Integrated Security=True;Trust Server Certificate=True")
        {
        }

        public override List<Product> Search(string search)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Product WHERE Name LIKE @search", conn))
                {
                    cmd.Parameters.AddWithValue("@search", "%" + search + "%");
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Product> products = new();
                    while (reader.Read())
                    {
                        Product product = new Product();
                        product.Id = reader.GetInt32(0);
                        product.Name = reader.GetString(1);
                        product.CategoryID = reader.GetInt32(2);
                        product.BrandID = reader.GetInt32(3);
                        product.UnitID = reader.GetInt32(4);
                        product.Description = reader.IsDBNull(5) ? DBNull.Value.ToString() : reader.GetString(5);
                        product.CreatedAt = reader.GetDateTime(6);
                        product.UpdatedAt = reader.GetDateTime(7);
                        product.ProductCode = reader.GetString(8);
                        product.Quantity = reader.GetInt32(9);
                        product.Weight = reader.GetDecimal(10);
                        product.Price = reader.GetDecimal(11);
                        product.SalePrice = reader.GetDecimal(12);
                        product.ImagePath = reader.GetString(13);
                        products.Add(product);
                    }
                    return products;
                }
            }
        }

        public Product GetProduct(string name, int categoryID, int brandID)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Product WHERE Name = @name AND CategoryID = @categoryID AND BrandID = @brandID", conn))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@categoryID", categoryID);
                    cmd.Parameters.AddWithValue("@brandID", brandID);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        Product product = new Product();
                        product.Id = reader.GetInt32(0);
                        product.Name = reader.GetString(1);
                        product.CategoryID = reader.GetInt32(2);
                        product.BrandID = reader.GetInt32(3);
                        product.UnitID = reader.GetInt32(4);
                        product.Description = reader.IsDBNull(5) ? DBNull.Value.ToString() : reader.GetString(5);
                        product.CreatedAt = reader.GetDateTime(6);
                        product.UpdatedAt = reader.GetDateTime(7);
                        product.ProductCode = reader.GetString(8);
                        product.Quantity = reader.GetInt32(9);
                        product.Weight = reader.GetDecimal(10);
                        product.Price = reader.GetDecimal(11);
                        product.SalePrice = reader.GetDecimal(12);
                        product.ImagePath = reader.GetString(13);
                        return product;
                    }
                    return new Product();
                }
            }
        }

        public List<Product> GetProductsByCategory(int categoryID)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Product WHERE CategoryID = @categoryID", conn))
                {
                    cmd.Parameters.AddWithValue("@categoryID", categoryID);
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Product> products = new();
                    while (reader.Read())
                    {
                        Product product = new Product();
                        product.Id = reader.GetInt32(0);
                        product.Name = reader.GetString(1);
                        product.CategoryID = reader.GetInt32(2);
                        product.BrandID = reader.GetInt32(3);
                        product.UnitID = reader.GetInt32(4);
                        product.Description = reader.IsDBNull(5) ? DBNull.Value.ToString() : reader.GetString(5);
                        product.CreatedAt = reader.GetDateTime(6);
                        product.UpdatedAt = reader.GetDateTime(7);
                        product.ProductCode = reader.GetString(8);
                        product.Quantity = reader.GetInt32(9);
                        product.Weight = reader.GetDecimal(10);
                        product.Price = reader.GetDecimal(11);
                        product.SalePrice = reader.GetDecimal(12);
                        product.ImagePath = reader.GetString(13);
                        products.Add(product);
                    }
                    return products;
                }
            }

        }
    }
}
