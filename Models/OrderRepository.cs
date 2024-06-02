using Dapper;
using Microsoft.Data.SqlClient;
namespace Ecommerce.Models
{
    public class OrderRepository: GenericRepository<Orders>
    {
        private readonly string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=newDb;Integrated Security=True;Trust Server Certificate=True";
        public OrderRepository() : base(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=newDb;Integrated Security=True;Trust Server Certificate=True")
        {
        }

        public Orders Get(string orderno)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Orders WHERE OrderNum = @OrderNum";
                var order = connection.QueryFirstOrDefault<Orders>(query, new { OrderNum = orderno });
                return order;
            }
        }
    }
}
