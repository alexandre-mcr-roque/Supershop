using Microsoft.AspNetCore.Mvc.Rendering;
using Supershop.Data.Entities;

namespace Supershop.Data
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        IQueryable<Product> GetAllWithUsers();
        IEnumerable<SelectListItem> GetComboProducts();
    }
}
