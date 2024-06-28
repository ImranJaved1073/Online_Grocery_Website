using Dapper;
using Microsoft.Data.SqlClient;
namespace Ecommerce.Models
{
    public class OrderRepository: GenericRepository<Orders>
    {
        private readonly string connectionString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=GroceryDb;Integrated Security=True;Trust Server Certificate=True";
        public OrderRepository() : base(@"Data Source=(localdb)\ProjectModels;Initial Catalog=GroceryDb;Integrated Security=True;Trust Server Certificate=True")
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

        public void UpdateStatus(Orders order)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string comm = $"Update Orders SET Status='{order.Status}' WHERE Id='{order.Id}'";
                conn.Execute(comm, new { OrderNum = order.OrderNum });
            }
        }
    }
}
