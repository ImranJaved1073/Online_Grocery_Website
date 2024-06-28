using Ecommerce.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Ecommerce.Models.Product> Product { get; set; } = default!;
        public DbSet<Ecommerce.Models.Category> Category { get; set; } = default!;
        public DbSet<Ecommerce.Models.Brand> Brand { get; set; } = default!;
        public DbSet<Ecommerce.Models.Unit> Unit { get; set; } = default!;
        public DbSet<Ecommerce.Models.Orders> Order { get; set; } = default!;
        public DbSet<Ecommerce.Models.OrderDetail> OrderDetail { get; set; } = default!;
    }

}
