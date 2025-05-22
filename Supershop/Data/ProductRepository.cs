using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Supershop.Data.Entities;

namespace Supershop.Data
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(DataContext context) : base(context)
        { }

        public IQueryable<Product> GetAllWithUsers()
        {
            return GetAll().Include(p => p.User);
        }

        public IEnumerable<SelectListItem> GetComboProducts()
        {
            var list = _context.Products.Select(p => new SelectListItem
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }).ToList();
            
            list.Insert(0, new SelectListItem
            {
                Text = "(Select a product)",
                Value = "0"
            });
            return list;
        }
    }
}
