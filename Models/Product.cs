using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace Ecommerce.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ProductCode { get; set; }

        [Required]
        public string Name { get; set; }

        public string Category { get; set; }

        public string Color {  get; set; }
        
        public string Size { get; set; }

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


    }
}
