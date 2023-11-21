using Microsoft.AspNetCore.Mvc;
using BanPhimCanhCach.Models;
using BanPhimCanhCach.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BanPhimCanhCach.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("/Admin/Order/{action}")]
    [Authorize(Roles = Roles.Administrator)]
    public class OrderController : Controller
    {
        private IOrderRepository _orderRepository;
        [TempData]
        public string StatusMessage { get; set; }
        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        // GET: Admin/Order
        public async Task<IActionResult> Index(string searchString, string searchFilter)
        {
              return View(await _orderRepository.GetOrder(searchString, searchFilter));
        }

        // GET: Admin/Order/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _orderRepository.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Admin/Order/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _orderRepository.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }
            await _orderRepository.DeleteOrder(id);
            StatusMessage = "Đã xóa đơn hàng";

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> SetNotificationStatus(int id)
        {
            var notification = await _orderRepository.GetNotification(id);
            notification.Viewed = true;
            await _orderRepository.Save();
            return LocalRedirect(notification.Url);
        }

        public async Task<IActionResult> UpdateOrderStatus(string id, string status)
        {
            await _orderRepository.UpdateOrderStatus(id, status);
            StatusMessage = "Đã cập nhật trạng thái đơn hàng";
            return RedirectToAction("Details", new { id = id });
        }

        public async Task<IActionResult> UpdatePaidStatus(string id, bool isPaid)
        {
            await _orderRepository.UpdatePaidStatus(id, isPaid);
            StatusMessage = "Đã cập nhật trạng thái thanh toán";
            return RedirectToAction("Details", new { id = id });
        }

        public async Task<IActionResult> Notification()
        {
            var records = await _orderRepository.GetNotificationByPage(1);
            return View(records);
        }
        public async Task<IActionResult> LoadMoreNotification(int page)
        {
            var records = await _orderRepository.GetNotificationByPage(page);
            return PartialView("_notificationPartial", records);
        }

        public async Task<IActionResult> ClearNotification()
        {
            await _orderRepository.DeleteNotification();
            return RedirectToAction("Notification");
        }
    }
}
