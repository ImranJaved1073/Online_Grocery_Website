using Microsoft.Data.SqlClient;

namespace Ecommerce.Models
{
    public class ProductRepository
    {
        private readonly string connString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=newDb;Integrated Security=True;Trust Server Certificate=True";

        public void Add(Product p)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    connection.Open();

                    string query = @"INSERT INTO Product (product_code, name, category, waight, quantity, regular_price, sale_price, Product_desc, in_stock, img_url,ImageName, created_at)
                                 VALUES (@ProductCode, @Name, @Category, @Weight, @Quantity, @RegularPrice, @SalePrice, @Description, @InStock, @ImageUrl,@ImgName, @CreatedAt)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ProductCode", p.ProductCode);
                        command.Parameters.AddWithValue("@Name", p.Name);
                        command.Parameters.AddWithValue("@Category", p.Category);
                        command.Parameters.AddWithValue("@Weight", p.Weight);
                        command.Parameters.AddWithValue("@Quantity", p.Quantity);
                        command.Parameters.AddWithValue("@RegularPrice", p.RegularPrice);
                        command.Parameters.AddWithValue("@SalePrice", p.SalePrice);
                        command.Parameters.AddWithValue("@Description", p.ProductDescription);
                        command.Parameters.AddWithValue("@InStock", p.InStock);
                        command.Parameters.AddWithValue("@ImageUrl", (object)p.ImageUrl ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ImgName", p.ImageName);
                        command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<Product> GetAllProducts()
        {
            List<Product> products = new List<Product>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    connection.Open();

                    string query = "SELECT * FROM Product";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Product product = new Product
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    ProductCode = reader["product_code"].ToString(),
                                    Name = reader["name"].ToString(),
                                    Category = reader["category"].ToString(),
                                    Weight = reader["waight"].ToString(),
                                    Quantity = Convert.ToInt32(reader["quantity"]),
                                    RegularPrice = Convert.ToDecimal(reader["regular_price"]),
                                    SalePrice = Convert.ToDecimal(reader["sale_price"]),
                                    ProductDescription = reader["Product_desc"].ToString(),
                                    InStock = Convert.ToBoolean(reader["in_stock"]),
                                    ImageUrl = reader["img_url"] != DBNull.Value ? reader["img_url"].ToString() : null,
                                    ImageName = reader["ImageName"].ToString(),
                                    CreatedAt = Convert.ToDateTime(reader["created_at"])
                                };

                                products.Add(product);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return products;
        }

        public Product GetProductByID(int id)
        {
            Product product = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    connection.Open();

                    string query = "SELECT * FROM Product WHERE id = @Id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                product = new Product
                                {
                                    ProductCode = reader["product_code"].ToString(),
                                    Name = reader["name"].ToString(),
                                    Category = reader["category"].ToString(),
                                    Weight = reader["waight"].ToString(),
                                    Quantity = Convert.ToInt32(reader["quantity"]),
                                    RegularPrice = Convert.ToDecimal(reader["regular_price"]),
                                    SalePrice = Convert.ToDecimal(reader["sale_price"]),
                                    ProductDescription = reader["Product_desc"].ToString(),
                                    InStock = Convert.ToBoolean(reader["in_stock"]),
                                    ImageUrl = reader["img_url"] != DBNull.Value ? reader["img_url"].ToString() : null,
                                    ImageName = reader["ImageName"].ToString(),
                                    CreatedAt = Convert.ToDateTime(reader["created_at"])
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log) or rethrow
                throw;
            }

            return product;
        }

        public void Delete(Product p)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    connection.Open();

                    string query = "DELETE FROM Product WHERE Id = @Id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", p.Id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void Edit(Product p)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    connection.Open();

                    string query = @"UPDATE Product 
                                     SET product_code = @ProductCode, 
                                         name = @Name, 
                                         category = @Category, 
                                         waight = @Weight, 
                                         quantity = @Quantity, 
                                         regular_price = @RegularPrice, 
                                         sale_price = @SalePrice, 
                                         Product_desc = @Description, 
                                         in_stock = @InStock, 
                                         img_url = @ImageUrl, 
                                         ImageName = @ImgName
                                     WHERE Id = @Id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ProductCode", p.ProductCode);
                        command.Parameters.AddWithValue("@Name", p.Name);
                        command.Parameters.AddWithValue("@Category", p.Category);
                        command.Parameters.AddWithValue("@Weight", p.Weight);
                        command.Parameters.AddWithValue("@Quantity", p.Quantity);
                        command.Parameters.AddWithValue("@RegularPrice", p.RegularPrice);
                        command.Parameters.AddWithValue("@SalePrice", p.SalePrice);
                        command.Parameters.AddWithValue("@Description", p.ProductDescription);
                        command.Parameters.AddWithValue("@InStock", p.InStock);
                        command.Parameters.AddWithValue("@ImageUrl", (object)p.ImageUrl ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ImgName", p.ImageName);
                        command.Parameters.AddWithValue("@Id", p.Id);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log) or rethrow
                throw;
            }
        }
    }
}
