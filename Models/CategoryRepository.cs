using Dapper;
using Microsoft.Data.SqlClient;

namespace Ecommerce.Models
{
    public class CategoryRepository : GenericRepository<Category>
    {
        private readonly string connectionString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=GroceryDb;Integrated Security=True;Trust Server Certificate=True";
        public CategoryRepository() : base(@"Data Source=(localdb)\ProjectModels;Initial Catalog=GroceryDb;Integrated Security=True;Trust Server Certificate=True")
        {
        }

        public List<Category> GetNames()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Id, CategoryName, CategoryDescription, ImgPath, CreatedOn FROM Category";
                var names = connection.Query<Category>(query).ToList();
                return names;
            }
        }

        public List<Category> GetParents()
        {
            string query = @"SELECT 
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

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var parents = connection.Query<Category>(query).ToList();
                return parents;
            }
        }

        public List<Category> GetCategoriesWithSubCategories()
        {
            var query = @"SELECT * FROM Category WHERE ParentCategoryID is NULL";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var subCategories = connection.Query<Category>(query).ToList();
                return subCategories;
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

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var subCategories = connection.Query<Category>(query, new { ParentCategoryID = parentCategoryId }).ToList();
                return subCategories;
            }
        }

        public List<Category> GetNonParentCategories()
        {
            var query = @"SELECT 
                    c.Id,
                    c.CategoryName,
                    c.CategoryDescription,
                    c.ImgPath,
                    c.CreatedOn
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
                                CategoryDescription = reader.IsDBNull(2) ? DBNull.Value.ToString() : reader.GetString(2),
                                ImgPath = reader.IsDBNull(3) ? null : reader.GetString(3),
                                CreatedOn = reader.GetDateTime(4)
                            };
                            nonParentCategories.Add(c);
                        }
                    }
                }
            }

            return nonParentCategories;
        }


        public override Category Get(int id)
        {
            var query = @"
                        SELECT c.Id,c.CategoryName,c.CategoryDescription,c.ImgPath,c.CreatedOn,c.ParentCategoryID,pc.CategoryName AS ParentCategoryName
                        FROM Category c LEFT JOIN Category pc ON c.ParentCategoryID = pc.Id
                        WHERE c.Id = @Id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var category = connection.QuerySingleOrDefault<Category>(query, new { Id = id });
                return category?? new Category();
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
            string query = @"SELECT Id,CategoryName,CategoryDescription,ImgPath,CreatedOn 
                          FROM Category WHERE CategoryName LIKE @search";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var categories = connection.Query<Category>(query, new { search = "%" + search + "%" }).ToList();
                return categories;
            }
        }
    }
}
