using Product.API.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Product.API.Model
{
    [Table("Products", Schema = "public")]
    public class Product
    {
        [Key]
        [Column("ProductID")]
        public int ProductID { get; set; }

        [Required(ErrorMessage = Messages.RequiredProductName)]
        [MinLength(2, ErrorMessage = Messages.ProductNameLength), MaxLength(200, ErrorMessage = Messages.ProductNameLength)]
        [Column("ProductName")]
        public string ProductName { get; set; }

        [MaxLength(500, ErrorMessage = Messages.ProductDescriptionLength)]
        [Column("ProductDescription")]
        public string ProductDescription { get; set; }

        [Column("Price")]
        [Required(ErrorMessage = Messages.RequiredProductPrice)]
        public decimal Price { get; set; }
    }
}
