using Supershop.Data.Entities;

namespace Supershop.Data
{
    public class MockRepository : IRepository
    {
        public void AddProduct(Product product)
        {
            throw new NotImplementedException();
        }

        public void DeleteProduct(Product product)
        {
            throw new NotImplementedException();
        }

        public Product? GetProduct(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Product> GetProducts()
        {
            return new List<Product>
            {
                new Product { Id = 1, Name = "Um", Price = 10 },
                new Product { Id = 2, Name = "Dois", Price = 20 },
                new Product { Id = 3, Name = "Três", Price = 30 },
                new Product { Id = 4, Name = "Quatro", Price = 40 },
                new Product { Id = 5, Name = "Cinco", Price = 50 },
            };
        }

        public bool ProductExists(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveAllAsync()
        {
            throw new NotImplementedException();
        }

        public void UpdateProduct(Product product)
        {
            throw new NotImplementedException();
        }
    }
}
