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

        [Required]
        [Column(TypeName = "Nvarchar(max)")]
        public string Description { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [NotMapped]
        public string  CategoryName { get; set; }

        [NotMapped]
        public string BrandName { get; set; }
    }

}
