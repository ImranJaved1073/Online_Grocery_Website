using Microsoft.Data.SqlClient;

namespace Ecommerce.Models
{
    public class ProductRepository: GenericRepository<Product>
    {
        private readonly string connString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=newDb;Integrated Security=True;Trust Server Certificate=True";

        public ProductRepository() : base(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=newDb;Integrated Security=True;Trust Server Certificate=True")
        {
        }

        public List<Product> Search(string search)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Products WHERE Name LIKE @search", conn))
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
                        product.Description = reader.GetString(4);
                        product.CreatedAt = reader.GetDateTime(5);
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
                        product.Description = reader.IsDBNull(4) ? null : reader.GetString(4);
                        product.CreatedAt = reader.GetDateTime(5);
                        return product;
                    }
                    return new Product();
                }
            }
        }
       
    }
}
