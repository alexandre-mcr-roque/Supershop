using Supershop.Data.Entities;
using Supershop.Models;

namespace Supershop.Data
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IQueryable<Order>?> GetOrderAsync(string userName);
        Task<IQueryable<OrderDetailTemp>?> GetOrderDetailsTempAsync(string userName);
        Task AddItemToOrderAsync(AddItemViewModel model, string userName);
        Task ModifyOrderDetailTempQuantityAsync(int id, double quantity);
        Task DeleteOrderDetailTempAsync(int id);
    }
}
