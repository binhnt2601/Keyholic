using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanPhimCanhCach.Models
{
    [Table("Product")]
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name="Tên")]
        public string Name { get; set; }
        [StringLength(1000)]
        [Required]
        [Display(Name = "Mô tả")]
        public string Description { get; set; }
        [Display(Name = "Sản phẩm hot")]
        public bool IsHot { get; set; } = false;
        [Display(Name = "Lượt xem")]
        public int ViewCount { get; set; } = 0;
        [Display(Name = "Ngày thêm")]
        public DateTime CreatedAt { get; set; }
        [Display(Name = "Ngày cập nhật")]
        public DateTime UpdatedAt { get; set;}
        public List<ProductImage> ProductImages { get; set; }
        public List<ProductVariant> ProductVariants { get; set; }

    }

    [Table("ProductImage")]
    public class ProductImage
    {
        [Key]
        public int Id { get; set; }
        public string ImgSrc { get; set; } = "/images/default.jpg";
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public Product Product { get; set; }

    }
}