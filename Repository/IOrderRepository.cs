using BanPhimCanhCach.Models;

namespace BanPhimCanhCach.Repository
{
    public interface IOrderRepository
    {
        Task AddOrder(Order order);
        Task AddOrderItem(OrderItem orderItem);
        Task UpdateOrderStatus(string orderId, string status);
        Task UpdatePaidStatus(string orderId, bool isPaid);
        Task Save();
        Task<List<Order>> GetOrder(string searchString, string searchFilter);
        Task<Order> GetOrderById(string id);
        Task DeleteOrder(string orderId);
        Task<List<Order>> GetUserOrder(string userId, string orderStatus);
        Task<Order> GetUserOrderInfo(string userId, string orderId);
        Task AddNotification(Notification notification);
        Task<List<Notification>> GetAllNotification(int? quantity);
        Task<Notification> GetNotification(int id);
        Task<List<Notification>> GetNotificationByPage(int page);
        Task<int> NotificationCount();
        Task DeleteNotification();
    }
}
