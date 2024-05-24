using Microsoft.Data.SqlClient;

namespace Ecommerce.Models
{
    public class VariantRepository: GenericRepository<ProductVariant>
    {
        private readonly string connString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=newDb;Integrated Security=True;Trust Server Certificate=True";


        public VariantRepository() : base(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=newDb;Integrated Security=True;Trust Server Certificate=True")
        {
        }

        public override List<ProductVariant> Search(string search)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM ProductVariant WHERE Name LIKE @search", conn))
                {
                    cmd.Parameters.AddWithValue("@search", "%" + search + "%");
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<ProductVariant> variants = new();
                    while (reader.Read())
                    {
                        ProductVariant variant = new ProductVariant();
                        variant.Id = reader.GetInt32(0);
                        variant.ProductID = reader.GetInt32(1);
                        variant.ProductCode = reader.GetString(2);
                        variant.Color = reader.GetString(3);
                        variant.Size = reader.GetString(4);
                        variant.VariantDescription = reader.GetString(5);
                        variant.Quantity = reader.GetInt32(6);
                        variant.Price = reader.GetDecimal(7);
                        variant.SalePrice = reader.GetDecimal(8);
                        variant.InStock = reader.GetBoolean(9);
                        variant.CreatedAt = reader.GetDateTime(10);
                        variant.UpdatedAt = reader.GetDateTime(11);
                        variant.ImagePath = reader.GetString(12);
                        variants.Add(variant);
                    }
                    return variants;
                }
            }
        }

        //public ProductVariant GetVariant(string name, int productID)
        //{
        //    using (SqlConnection conn = new SqlConnection(connString))
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = new SqlCommand("SELECT * FROM ProductVariant WHERE Name = @name AND ProductID = @productID", conn))
        //        {
        //            cmd.Parameters.AddWithValue("@name", name);
        //            cmd.Parameters.AddWithValue("@productID", productID);
        //            SqlDataReader reader = cmd.ExecuteReader();
        //            if (reader.Read())
        //            {
        //                ProductVariant variant = new ProductVariant();
        //                variant.Id = reader.GetInt32(0);
        //                variant.Name = reader.GetString(1);
        //                variant.ProductID = reader.GetInt32(2);
        //                variant.Price = reader.GetDecimal(3);
        //                variant.Quantity = reader.GetInt32(4);
        //                variant.CreatedAt = reader.GetDateTime(5);
        //                return variant;
        //            }
        //            return new ProductVariant();
        //        }
        //    }
        //}

        //public List<ProductVariant> GetVariantsByProductID(int productID)
        //{
        //    using (SqlConnection conn = new SqlConnection(connString))
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = new SqlCommand("SELECT * FROM ProductVariant WHERE ProductID = @productID", conn))
        //        {
        //            cmd.Parameters.AddWithValue("@productID", productID);
        //            SqlDataReader reader = cmd.ExecuteReader();
        //            List<ProductVariant> variants = new();
        //            while (reader.Read())
        //            {
        //                ProductVariant variant = new ProductVariant();
        //                variant.Id = reader.GetInt32(0);
        //                variant.Name = reader.GetString(1);
        //                variant.ProductID = reader.GetInt32(2);
        //                variant.Price = reader.GetDecimal(3);
        //                variant.Quantity = reader.GetInt32(4);
        //                variant.CreatedAt = reader.GetDateTime(5);
        //                variants.Add(variant);
        //            }
        //            return variants;
        //        }
        //    }
        //}
        public ProductVariant GetProductVariant(string size, string color, string productCode, int pId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT Id,Size,Color,ProductCode FROM ProductVariant WHERE Size = @size AND Color = @color AND ProductCode = @productCode AND ProductID = @pId", conn))
                {
                    cmd.Parameters.AddWithValue("@size", size);
                    cmd.Parameters.AddWithValue("@color", color);
                    cmd.Parameters.AddWithValue("@productCode", productCode);
                    cmd.Parameters.AddWithValue("@pId", pId);
                    SqlDataReader reader = cmd.ExecuteReader();
                    ProductVariant variant = new();
                    while (reader.Read())
                    {
                        variant.Id = reader.GetInt32(0);
                        variant.Size = reader.GetString(1);
                        variant.Color = reader.GetString(2);
                        variant.ProductCode = reader.GetString(3);
                    }
                    return variant;
                }
            }
        }
    }
}
