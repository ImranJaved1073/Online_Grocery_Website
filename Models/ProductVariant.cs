using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models
{
    public class ProductVariant
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Product")]
        public int ProductID { get; set; }

        public string ProductCode { get; set; }

        //[ForeignKey("Size")]
        //public int SizeID { get; set; }

        public string Size {  get; set; }

        public string Color { get; set; }

        [Required]
        [Column(TypeName = "Nvarchar(max)")]
        public string VariantDescription { get; set; }

        [Required]
        public decimal SalePrice { get; set; }
        [Required]
        public decimal Price { get; set; }

        [Required]
        public bool InStock { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [Required]
        public int Quantity { get; set; }

        [NotMapped]
        public IFormFile Picture { get; set; }

        public string ImagePath { get; set; }

    }
}
