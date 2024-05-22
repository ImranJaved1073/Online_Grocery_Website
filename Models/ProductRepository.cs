using Microsoft.Data.SqlClient;

namespace Ecommerce.Models
{
    public class ProductRepository: GenericRepository<Product>
    {
        private readonly string connString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=newDb;Integrated Security=True;Trust Server Certificate=True";

        public ProductRepository() : base(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=newDb;Integrated Security=True;Trust Server Certificate=True")
        {
        }
       
    }
}
