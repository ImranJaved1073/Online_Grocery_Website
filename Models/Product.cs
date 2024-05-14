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
        public int ProductID { get; set; }

        [Required]
        [MaxLength(255)]
        public string ProductCode { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [ForeignKey("Category")]
        public int CategoryID { get; set; }

        [ForeignKey("Brand")]
        public int BrandID { get; set; }

        [ForeignKey("Color")]
        public int ColorID { get; set; }

        [ForeignKey("Size")]
        public int SizeID { get; set; }

        [Required]
        public int Quantity { get; set; }

        public decimal? RegularPrice { get; set; }

        [Required]
        public decimal SalePrice { get; set; }

        public string ProductDescription { get; set; }

        [Required]
        public bool InStock { get; set; }

        [NotMapped]
        public IFormFile Picture { get; set; }

        public string ImageUrl { get; set; }
        public string ImageName { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual Category? Category { get; set; }
        public virtual Brand? Brand { get; set; }
        //public virtual Color Color { get; set; }
        public virtual Size Size { get; set; }
    }

}
