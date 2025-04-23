using Supershop.Data.Entities;

namespace Supershop.Data
{
    public class SeedDb
    {
        private readonly DataContext _ctx;
        private Random _random;

        public SeedDb(DataContext ctx)
        {
            _ctx = ctx;
            _random = new Random();
        }

        public async Task SeedAsync()
        {
            await _ctx.Database.EnsureCreatedAsync();

            if (!_ctx.Products.Any())
            {
                AddProduct("iPhone X");
                AddProduct("Magic Mouse");
                AddProduct("iWatch Series 4");
                AddProduct("iPad Mini");
                await _ctx.SaveChangesAsync();
            }
        }

        private void AddProduct(string name)
        {
            _ctx.Products.Add(new Product
            {
                Name = name,
                Price = _random.Next(1000),
                IsAvailable = true,
                Stock = _random.Next(200)
            });
        }
    }
}
