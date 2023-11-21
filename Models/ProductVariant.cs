using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanPhimCanhCach.Models
{
    [Table("ProductVariant")]
    public class ProductVariant
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Phân loại")]
        public string Name { get; set; }
        [Range(0, int.MaxValue)]
        [Display(Name = "Giá gốc")]
        public int OriginalPrice { get; set; }
        [Range(0, int.MaxValue)]
        [Display(Name = "Giá hiện tại")]
        public int CurrentPrice { get; set; }
        [Range(0, int.MaxValue)]
        [Display(Name = "Số lượng")]
        public int Quantity { get; set; }
        public string ImgSrc { get; set; }
        [ForeignKey("Product")]
        public int? ProductId { get; set; }
        [Display(Name = "Tên Sản phẩm")]
        public string ProductName { get; set; }
        public Product Product { get; set; }
    }

    public class ProductVariantModel
    {
        public string Name { get; set; }
        [Range(0, double.MaxValue)]
        public int Price { get; set; }
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
        public string ImgSrc { get; set; }
        public int ProductId { get; set; }
    }
}
