using System.ComponentModel.DataAnnotations;

namespace BanPhimCanhCach.Models
{
    public class CreateProductModel
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Tên")]
        public string Name { get; set; }
        [StringLength(1000)]
        [Required]
        [Display(Name = "Mô tả")]
        public string Description { get; set; }
        [Display(Name = "Sản phẩm hot")]
        public bool IsHot { get; set; } = false;
        public List<ProductImage> ProductImages { get; set; }
        public List<ProductVariant> ProductVariants { get; set; }
    }
}
