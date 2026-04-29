using Ghtk.Repository.Abstractions.Entities;

namespace Ghtk.Repository.Abstractions
{
    public interface IOrderRepository
    {
        Task<bool> CancelOrderAsync(string id, string partnerId, int status);
        Task CreateOrderAsync(OrderEntity orderEntity);
        Task<OrderEntity> GetOrderByIdAsync(string id, string partnerId);
    }
}
