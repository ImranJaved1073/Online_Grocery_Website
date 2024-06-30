using Microsoft.Data.SqlClient;

namespace Ecommerce.Models
{
    public interface IOrderRepository : IRepository<Orders>
    {
        public Orders Get(string orderno);
        public void UpdateStatus(Orders order);
    }
}
