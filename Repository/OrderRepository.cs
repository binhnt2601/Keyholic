using BanPhimCanhCach.AppData;
using BanPhimCanhCach.Models;
using MailKit.Search;
using Microsoft.EntityFrameworkCore;

namespace BanPhimCanhCach.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly IProductRepository _productRepository;

        public OrderRepository(AppDbContext appDbContext, IProductRepository productRepository)
        {
            _appDbContext = appDbContext;
            _productRepository = productRepository;
        }

        public async Task AddOrder(Order order)
        {
            await _appDbContext.Orders.AddAsync(order);
        }

        public async Task AddOrderItem(OrderItem orderItem)
        {
            await _appDbContext.OrderItems.AddAsync(orderItem);
        }


        public async Task Save()
        {
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<List<Order>> GetOrder(string searchString, string searchFilter)
        {
            IEnumerable<Order> orders;
            switch(searchFilter)
            {
                case "Id":
                    orders = _appDbContext.Orders.Where(o => o.Id == searchString);
                    break;
              
                case "Name":
                    orders = _appDbContext.Orders.Where(o => o.Name == searchString);
                    break;

                case "PhoneNumber":
                    orders = _appDbContext.Orders.Where(o => o.PhoneNumber == searchString);
                    break;

                case "Email":
                    orders = _appDbContext.Orders.Where(o => o.Email == searchString);
                    break;

                default:
                    return await _appDbContext.Orders.OrderByDescending(o => o.OrderDate).ToListAsync();

                
            }
            return orders.OrderByDescending(o => o.OrderDate).ToList();
        }

        public async Task UpdateOrderStatus(string orderId, string status)
        {
            var order = await GetOrderById(orderId); 
     
            if(status == OrderStatus.Delivered)
            {
                order.Status = status;
            }
            else
            {
                order.Status = OrderStatus.Cancelled;
                foreach (var orderItem in order.OrderItems)
                {
                    await _productRepository.UpdateProductQuantity(orderItem.ProductId, orderItem.Quantity);
                }
            }   
            await Save();
        }

        public async Task DeleteOrder(string orderId)
        {
            var order = await GetOrderById(orderId);
            if(order != null && order.Status == OrderStatus.Cancelled)
            {
                _appDbContext.Orders.Remove(order);
                await Save();
            }

        }

        public async Task<Order> GetOrderById(string id)
        {
            var order = await _appDbContext.Orders
                        .Where(o => o.Id == id)
                        .Include(o => o.OrderItems)
                            .ThenInclude(oi => oi.Product)
                        .FirstOrDefaultAsync();
            return order;
        }

        public async Task<bool> OrderExist(string orderId)
        {
            return await _appDbContext.Orders.AnyAsync(e => e.Id == orderId);
        }

        public async Task<List<Order>> GetUserOrder(string userId, string orderStatus)
        {
            var userOrder = new List<Order>();
            if(!String.IsNullOrEmpty(orderStatus))
            {
               userOrder = await _appDbContext.Orders
                                .Where(o => o.UserId == userId && o.Status == orderStatus)
                                .OrderByDescending(o => o.OrderDate)
                                .ToListAsync();
            }
            else
            {
                userOrder = await _appDbContext.Orders
                                .Where(o => o.UserId == userId)
                                .OrderByDescending(o => o.OrderDate)
                                .ToListAsync();
            }
            return userOrder;          
        }

        public async Task<Order> GetUserOrderInfo(string userId, string orderId)
        {
            var order = await _appDbContext.Orders
                      .Where(o => o.Id == orderId)
                      .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                      .FirstOrDefaultAsync();
            return order;
        }

        public async Task AddNotification(Notification notification)
        {
            await _appDbContext.Notifications.AddAsync(notification);
        }

        public async Task<List<Notification>> GetAllNotification(int? quantity)
        {                     
            if(quantity == null) {
                return await _appDbContext.Notifications.ToListAsync();
            }
            return await _appDbContext.Notifications.OrderByDescending(n => n.CreatedDate).Take((int)quantity).ToListAsync();
        }

        public async Task<Notification> GetNotification(int id)
        {
            return await _appDbContext.Notifications.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<int> NotificationCount()
        {
            return await _appDbContext.Notifications.Where(n => n.Viewed == false).CountAsync();
        }

        public async Task UpdatePaidStatus(string orderId, bool isPaid)
        {
            var order = await GetOrderById(orderId);
            if(order != null && order.PaymentMethod == PaymentMethod.COD)
            {
                order.IsPaid = isPaid;
            }
            await Save();
        }

        public async Task<List<Notification>> GetNotificationByPage(int page)
        {
            return await _appDbContext.Notifications.OrderByDescending(n => n.CreatedDate).Skip(5 * (page-1)).Take(5).ToListAsync();
        }

        public async Task DeleteNotification()
        {
            var query = "Truncate Table Notifications";
            await _appDbContext.Database.ExecuteSqlRawAsync(query);
        }
    }
}
