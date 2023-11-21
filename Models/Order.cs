using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanPhimCanhCach.Models
{
    [Table("Order")]
    public class Order
    {
        [Key]
        [Display(Name = "Mã đơn hàng")]
        public string Id { get; set; } // Unique identifier for the order
        public string UserId { get; set; } // Identifier for the user who placed the order
        [Display(Name = "Ngày đặt hàng")]
        public DateTime OrderDate { get; set; } // Date and time when the order was placed
        [Display(Name = "Tổng")]
        public int TotalAmount { get; set; } // Total amount of the order
        [Display(Name = "Trạng thái")]
        public string Status { get; set; } // Current status of the order (e.g., Pending, Shipped, Delivered)
        [Display(Name = "T.T. Thanh toán")]
        public bool IsPaid { get; set; }

        // Navigation property to link the order with its order items (line items)
        public ICollection<OrderItem> OrderItems { get; set; }
        [Required(ErrorMessage = "Tên không được bỏ trống")]
        [Display(Name = "Người mua")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Số điện thoại không được bỏ trống")]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get;set; }
        [Required(ErrorMessage = "Địa chỉ không được bỏ trống")]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }
        public string Email { get; set; }
        [Display(Name = "P.T. Thanh Toán")]
        public string PaymentMethod { get; set; }
        [Display(Name = "N.D. Thanh Toán")]
        public string Description { get; set; }


        public Order()
        {
            OrderItems = new List<OrderItem>();
        }
    }

    public static class OrderStatus
    {
        public const string Processing = "Đang xử lý";  // Order is being processed
        public const string Delivered = "Đã nhận hàng"; // Order has been delivered to the customer
        public const string Cancelled = "Đã hủy";        // Order has been canceled
    }

    public static class PaymentMethod
    {
        public const string VnPay = "VnPay";
        public const string Momo = "Momo";
        public const string COD = "COD";
    }
}