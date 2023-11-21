using BanPhimCanhCach.Models;
using BanPhimCanhCach.Repository;
using BanPhimCanhCach.Services;
using MailKit;
using MailKit.Search;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

namespace BanPhimCanhCach.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IEmailSender _emailSender;
        private readonly IVnPayService _vnPayService;

        public CartController(IProductRepository productRepository, IOrderRepository orderRepository, IEmailSender emailSender, IVnPayService vnPayService)
        {
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _emailSender = emailSender;
            _vnPayService = vnPayService;
        }

        // Hiện thị giỏ hàng
        [Route("/cart")]
        public IActionResult Cart()
        {
            var cartItems = GetCartItems();
            ViewBag.Amount = cartItems.Sum(i => i.quantity * i.product.CurrentPrice);
            return View(cartItems);
        }

        public async Task<bool> AddToCart(int id, int quantity)
        {
            var product = await _productRepository.GetProductVariant(id);
            if(product == null)
            {
                return false;
            }
            // Xử lý đưa vào Cart ...
            var cart = GetCartItems();
            var cartitem = cart.Find(p => p.product.Id == id);
            if (cartitem != null)
            {
                cartitem.quantity += quantity;
                if(cartitem.quantity > product.Quantity) {
                    cartitem.quantity = product.Quantity;
                }
                // Đã tồn tại, tăng thêm 1     
            }
            else
            {
                cart.Add(new CartItem() { quantity = quantity, product = product });
                //  Thêm mới
            }

            // Lưu cart vào Session
            SaveCartSession(cart);
            return true;
        }

        public const string CARTKEY = "BpccCart";

        // Lấy cart từ Session (danh sách CartItem)
        List<CartItem> GetCartItems()
        {
            var session = HttpContext.Session;
            string jsoncart = session.GetString(CARTKEY);
            if (jsoncart != null)
            {
                return JsonConvert.DeserializeObject<List<CartItem>>(jsoncart);
            }
            return new List<CartItem>();
        }

        // Xóa cart khỏi session
        public void ClearCart()
        {
            var session = HttpContext.Session;
            session.Remove(CARTKEY);
        }

        // Lưu Cart (Danh sách CartItem) vào session
        void SaveCartSession(List<CartItem> ls)
        {
            var session = HttpContext.Session;
            string jsoncart = JsonConvert.SerializeObject(ls);
            session.SetString(CARTKEY, jsoncart);
        }

        [HttpPost]
        public void UpdateCart(int productid, int quantity)
        {
            // Cập nhật Cart thay đổi số lượng quantity ...
            var cart = GetCartItems();
            var cartitem = cart.Find(p => p.product.ProductId == productid);
            if (cartitem != null)
            {
                cartitem.quantity = quantity;
            }
            SaveCartSession(cart);
        }

        public IActionResult RemoveCartItem(int productId)
        {
            var cart = GetCartItems();
            var cartitem = cart.Find(p => p.product.Id == productId);
            if (cartitem != null)
            {
                cart.Remove(cartitem);
            }
            SaveCartSession(cart);
            return RedirectToAction("Cart");
        }
        public IActionResult Checkout()
        {
            var cartItems = GetCartItems();
            if(cartItems.Count() == 0)
            {
                return NotFound();
            }
            ViewBag.CartItem = cartItems;
            return View();
        }

        [NonAction]
        public async Task<string> CreateNewOrder(PaymentInformationModel model)
        {
            var id = GenerateRandomString();
            var order = new Order()
            {
                Id = id,
                Status = OrderStatus.Processing,
                OrderDate = DateTime.Now.ToLocalTime(),
                Name = model.Name,
                Address = model.Address,
                TotalAmount = model.Amount,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Description = model.Description,
                IsPaid = false,
                UserId = model.UserId,
                PaymentMethod = model.PaymentMethod
            };

            await _orderRepository.AddOrder(order);
            await _orderRepository.Save();
            return id;
        }
        [NonAction]
        private string GenerateRandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            StringBuilder stringBuilder = new StringBuilder(10);

            for (int i = 0; i < 10; i++)
            {
                int index = random.Next(chars.Length);
                stringBuilder.Append(chars[index]);
            }

            return stringBuilder.ToString();
        }

        public string CreatePaymentUrl(PaymentInformationModel model, string orderId)
        {
            var url = _vnPayService.CreatePaymentUrl(model, orderId, HttpContext);

            return url;
        }

        public IActionResult PaymentCallback()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            return RedirectToAction("CheckOutResult", response);
        }

        [HttpPost]
        public async Task<bool> COD(PaymentInformationModel model)
        {
            var orderId = await CreateNewOrder(model);
            await _orderRepository.Save();
            var result = await AddOrderItem(orderId);
            if(!result)
            {
                return false;
            }
            await AddNotification(orderId);
            await _orderRepository.Save();
            ClearCart();
            if (model.Email != string.Empty)
            {
                var content = "<p>Đơn hàng của bạn đã được xác nhận.</p>\r\n<h3>Cảm ơn bạn đã mua hàng tại Keyholic</h3>";
                await _emailSender.SendEmailAsync(model.Email, $"Xác nhận đơn hàng {orderId}", content);
            };
            return true;
        }
        public async Task<IActionResult> CheckoutResult(PaymentResponseModel model)
        {
            var order = await _orderRepository.GetOrderById(model.OrderId);
            if (model.VnPayResponseCode == "00")
            {          
                order.IsPaid = true;
                order.Status = OrderStatus.Processing;
                await AddNotification(order.Id);
                await _orderRepository.Save();
                ClearCart();
                if (order.Email != string.Empty)
                {
                    var content = "<p>Đơn hàng của bạn đã được xác nhận.</p>\r\n<h3>Cảm ơn bạn đã mua hàng tại BPCC</h3>";
                    await _emailSender.SendEmailAsync(order.Email, $"Xác nhận đơn hàng {order.Id}", content);
                }
                ViewData["Amount"] = order.TotalAmount;
                return View(model);
            }
            else
            {
                await _orderRepository.UpdateOrderStatus(model.OrderId, OrderStatus.Cancelled);
                await _orderRepository.Save();
                return View(model);
            }
        }
        public async Task<string> VnPay(PaymentInformationModel model)
        {
            var orderId = await CreateNewOrder(model);
            await _orderRepository.Save();
            var result = await AddOrderItem(orderId);
            if(!result)
            {
                await _orderRepository.DeleteOrder(orderId);
                await _orderRepository.Save();
                return "0";
            }
            await _orderRepository.Save();
            var url = CreatePaymentUrl(model, orderId);
            return url;
        }
        [NonAction]
        public async Task<bool> AddOrderItem(string orderId)
        {
            foreach (var item in GetCartItems())
            {
                var orderItem = new OrderItem()
                {
                    OrderId = orderId,
                    ProductId = item.product.Id,
                    Quantity = item.quantity,
                    UnitPrice = item.product.CurrentPrice,
                    ProductName = item.product.ProductName,
                    VariantName = item.product.Name
                };
                await _orderRepository.AddOrderItem(orderItem);
                var result = await _productRepository.UpdateProductQuantity(item.product.Id, -item.quantity);
                if(result == false)
                {
                    return false;
                }      
            }
            return true;
        }
        [NonAction]
        public async Task AddNotification(string orderId)
        {
            var notify = new Notification()
            {
                Title = $"Đơn hàng mới: {orderId}",
                Url = Url.Action("Details", "Order", new { area = "Admin", id = orderId }),
                Viewed = false,
                CreatedDate = DateTime.Now.ToLocalTime(),
            };
            await _orderRepository.AddNotification(notify);
        }
    }
}
