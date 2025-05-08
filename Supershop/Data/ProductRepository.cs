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
    }
}
