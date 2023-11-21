using BanPhimCanhCach.Models;

namespace BanPhimCanhCach.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentInformationModel model, string orderId, HttpContext context);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
