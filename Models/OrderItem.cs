using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanPhimCanhCach.Models
{
    [Table("OrderItem")]
    public class OrderItem
    {
        [Key]
        public int Id { get; set; } // Unique identifier for the order item
        [ForeignKey("Product")]
        public int ProductId { get; set; } // Identifier for the product associated with the order item
        [Display(Name = "Tên")]
        public string ProductName { get; set; } // Name of the product
        [Display(Name = "Phân loại")]
        public string VariantName { get; set; }
        [Display(Name = "Đơn giá")]
        public int UnitPrice { get; set; } // Price per unit of the product
        [Display(Name = "Số lượng")]
        public int Quantity { get; set; } // Quantity of the product in the order

        [ForeignKey("Order")]
        public string OrderId { get; set; }
        public Order Order { get; set; }
        public ProductVariant Product { get; set; }
    }
}
