using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Ecommerce.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "Nvarchar(100)")]
        public string Name { get; set; }

        [ForeignKey("Category")]
        public int CategoryID { get; set; }

        [ForeignKey("Brand")]
        public int BrandID { get; set; }

        [ForeignKey("Unit")]
        public int UnitID { get; set; }

        [Required]
        [Column(TypeName = "Nvarchar(max)")]
        public string Description { get; set; }

        public string ProductCode { get; set; }

        public decimal Weight { get; set; }

        [Required]
        public decimal SalePrice { get; set; }
        [Required]
        public decimal Price { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [Required]
        public int Quantity { get; set; }

        [NotMapped]
        public IFormFile Picture { get; set; }

        public string ImagePath { get; set; }

        [NotMapped]
        public string  CategoryName { get; set; }

        [NotMapped]
        public string BrandName { get; set; }

        [NotMapped]
        public string UnitName { get; set; }

        public string GetUnitName(int id)
        {
            IRepository<Unit> unitRepository = new GenericRepository<Unit>(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=newDb;Integrated Security=True;Trust Server Certificate=True");
            UnitName = unitRepository.Get(id).Name;
            return UnitName;
        }
    }

}
