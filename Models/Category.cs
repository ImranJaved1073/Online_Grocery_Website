﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? CategoryCode { get; set; }

        [Required]
        public string? CategoryName { get; set; }

        public int? ParentCategoryID { get; set; }

        [Required]
        public string? CategoryDescription { get; set; }

        public IFormFile? CategoryImg { get; set; }

        public string? ImgPath { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        [ForeignKey("ParentCategoryID")]
        public Category? ParentCategory { get; set; }
    }
}
