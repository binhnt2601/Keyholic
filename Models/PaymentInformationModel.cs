using System.ComponentModel.DataAnnotations;

namespace BanPhimCanhCach.Models
{
    public class PaymentInformationModel
    {
        [Display(Name = "Tổng")]
        [Required]
        public int Amount { get; set; }

        [Required(ErrorMessage = "Tên không được bỏ trống")]
        [Display(Name = "Người đặt")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được bỏ trống")]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được bỏ trống")]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }
        public string Email { get; set; }
        [Display(Name = "P.T. Thanh Toán")]
        [Required]
        public string PaymentMethod { get; set; }
        [Display(Name = "N.D. Thanh Toán")]
        public string Description { get; set; }
        public string UserId { get; set; }
    }
}
