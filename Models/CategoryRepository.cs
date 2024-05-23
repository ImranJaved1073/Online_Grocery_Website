using Microsoft.Data.SqlClient;

namespace Ecommerce.Models
{
    public class CategoryRepository : GenericRepository<Category>
    {
        private readonly string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=newDb;Integrated Security=True;Trust Server Certificate=True";
        public CategoryRepository() : base(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=newDb;Integrated Security=True;Trust Server Certificate=True")
        {
        }

        public List<Category> GetNames()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id,CategoryName,CategoryDescription,ImgPath,CreatedOn FROM Category";
                var reader = command.ExecuteReader();
                var names = new List<Category>();
                while (reader.Read())
                {
                    names.Add(new Category
                    {
                        Id = reader.GetInt32(0),
                        CategoryName = reader.GetString(1),
                        CategoryDescription = reader.GetString(2),
                        ImgPath = reader.GetString(3),
                        CreatedOn = reader.GetDateTime(4)
                    });
                }
                return names;
            }
        }

        public List<Category> GetParents()
        {
            var query = @"SELECT 
                            c.Id,
                            c.CategoryName,
                            c.CategoryDescription,
                            c.ImgPath,
                            c.CreatedOn,
                            c.ParentCategoryID,
                            pc.CategoryName AS ParentCategoryName
                        FROM 
                            Category c
                        LEFT JOIN 
                            Category pc ON c.ParentCategoryID = pc.Id";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ParentCategoryID",DBNull.Value);
                SqlDataReader reader = cmd.ExecuteReader();
                var names = new List<Category>();
                while (reader.Read())
                {
                    Category c = new();
                    c.Id = reader.GetInt32(0);
                    c.CategoryName = reader.GetString(1);
                    c.CategoryDescription = reader.IsDBNull(2) ? null : reader.GetString(2);
                    c.ImgPath = reader.IsDBNull(3) ? null : reader.GetString(3);
                    c.CreatedOn = reader.GetDateTime(4);
                    c.ParentCategoryID = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5);
                    c.ParentCategoryName = reader.IsDBNull(6) ? null : reader.GetString(6);
                    names.Add(c);
                }
                return names;
                
            }
        }

        public List<Category> GetSubCategories(int parentCategoryId)
        {
            var query = @"SELECT 
                    c.Id,
                    c.CategoryName,
                    c.CategoryDescription,
                    c.ImgPath,
                    c.CreatedOn,
                    c.ParentCategoryID,
                    pc.CategoryName AS ParentCategoryName
                FROM 
                    Category c
                LEFT JOIN 
                    Category pc ON c.ParentCategoryID = pc.Id
                WHERE 
                    c.ParentCategoryID = @ParentCategoryID";

            var subCategories = new List<Category>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@ParentCategoryID", parentCategoryId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Category c = new Category
                            {
                                Id = reader.GetInt32(0),
                                CategoryName = reader.GetString(1),
                                CategoryDescription = reader.IsDBNull(2) ? null : reader.GetString(2),
                                ImgPath = reader.IsDBNull(3) ? null : reader.GetString(3),
                                CreatedOn = reader.GetDateTime(4),
                                ParentCategoryID = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5),
                                ParentCategoryName = reader.IsDBNull(6) ? null : reader.GetString(6)
                            };
                            subCategories.Add(c);
                        }
                    }
                }
            }

            return subCategories;
        }

        public List<Category> GetNonParentCategories()
        {
            var query = @"SELECT 
                    c.Id,
                    c.CategoryName,
                    c.CategoryDescription,
                    c.ImgPath,
                    c.CreatedOn,
                    c.ParentCategoryID
                FROM 
                    Category c
                WHERE 
                    c.Id NOT IN (SELECT DISTINCT ParentCategoryID FROM Category WHERE ParentCategoryID IS NOT NULL)";

            var nonParentCategories = new List<Category>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var cmd = new SqlCommand(query, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Category c = new Category
                            {
                                Id = reader.GetInt32(0),
                                CategoryName = reader.GetString(1),
                                CategoryDescription = reader.IsDBNull(2) ? null : reader.GetString(2),
                                ImgPath = reader.IsDBNull(3) ? null : reader.GetString(3),
                                CreatedOn = reader.GetDateTime(4),
                                ParentCategoryID = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5)
                            };
                            nonParentCategories.Add(c);
                        }
                    }
                }
            }

            return nonParentCategories;
        }


        public new Category Get(int id)
        {
            var query = @"
                        SELECT 
                            c.Id,
                            c.CategoryName,
                            c.CategoryDescription,
                            c.ImgPath,
                            c.CreatedOn,
                            c.ParentCategoryID,
                            pc.CategoryName AS ParentCategoryName
                        FROM 
                            Category c
                        LEFT JOIN 
                            Category pc ON c.ParentCategoryID = pc.Id
                        WHERE 
                            c.Id = @Id";

            using (var connection = new SqlConnection(connectionString))
            {
                Category c = new();
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {

                            c.Id = reader.GetInt32(0);
                            c.CategoryName = reader.GetString(1);
                            c.CategoryDescription = reader.IsDBNull(2) ? null : reader.GetString(2);
                            c.ImgPath = reader.IsDBNull(3) ? null : reader.GetString(3);
                            c.CreatedOn = reader.GetDateTime(4);
                            c.ParentCategoryID = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5);
                            c.ParentCategoryName = reader.IsDBNull(6) ? null : reader.GetString(6);

                        }
                    }
                }
                return c;
            }


        }


        //public List<Category> GetParentCategory()
        //{
        //    // get catergories where parent category is not get parent category id and name also
        //    using (var connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();
        //        var command = connection.CreateCommand();
        //        command.CommandText = "SELECT Id,CategoryName FROM Category where ParentCategoryID is not null";
        //        var reader = command.ExecuteReader();
        //        var names = new List<Category>();
        //        while (reader.Read())
        //        {
        //            names.Add(new Category
        //            {
        //                Id = reader.GetInt32(0),
        //                CategoryName = reader.GetString(1)
        //            });
        //        }
        //        return names;
        //    }
        //}

        public new List<Category> Search(string search)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id,CategoryName,CategoryDescription,ImgPath,CreatedOn FROM Category where CategoryName like @search";
                command.Parameters.AddWithValue("@search", "%" + search + "%");
                var reader = command.ExecuteReader();
                var names = new List<Category>();
                while (reader.Read())
                {
                    names.Add(new Category
                    {
                        Id = reader.GetInt32(0),
                        CategoryName = reader.GetString(1),
                        CategoryDescription = reader.GetString(2),
                        ImgPath = reader.GetString(3),
                        CreatedOn = reader.GetDateTime(4)
                    });
                }
                return names;
            }
        }
    }
}
