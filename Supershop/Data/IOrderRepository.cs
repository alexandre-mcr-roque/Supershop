using Supershop.Data.Entities;

namespace Supershop.Data
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IQueryable<Order>?> GetOrderAsync(string userName);
    }
}
