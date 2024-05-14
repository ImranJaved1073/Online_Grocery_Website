using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace Ecommerce.Models
{
    public class Images
    {
        [Key]
        public int ImageID { get; set; }

        [ForeignKey("Product")]
        public int ProductID { get; set; }

        [ForeignKey("Color")]
        public int ColorID { get; set; }

        [MaxLength(255)]
        public string? ImageUrl { get; set; }

        [MaxLength(255)]
        public string? ImageName { get; set; }

        // Navigation properties
        public virtual Product? Product { get; set; }
        public virtual Color Color { get; set; }
    }
}
