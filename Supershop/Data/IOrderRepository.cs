using Supershop.Data.Entities;
using Supershop.Models;

namespace Supershop.Data
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IQueryable<Order>?> GetOrderAsync(string userName);
        Task<IQueryable<OrderDetailTemp>?> GetDetailsTempAsync(string userName);
        Task AddItemToOrderAsync(AddItemViewModel model, string userName);
        Task ModifyOrderDetailTempQuantityAsync(int id, double quantity);
    }
}
