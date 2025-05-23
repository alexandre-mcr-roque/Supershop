using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Supershop.Data.Entities;
using Supershop.Helpers;

namespace Supershop.Data
{
    public class SeedDb
    {
        private readonly DataContext _ctx;
        private readonly IUserHelper _userHelper;
        private Random _random;

        public SeedDb(DataContext ctx, IUserHelper userHelper)
        {
            _ctx = ctx;
            _userHelper = userHelper;
            _random = new Random();
        }

        public async Task SeedAsync()
        {
            await _ctx.Database.MigrateAsync();

            await _userHelper.CheckRoleAsync("Admin");
            await _userHelper.CheckRoleAsync("Customer");

            if (!_ctx.Countries.Any())
            {
                var cities = new List<City>
                {
                    new City { Name = "Lisbon" },
                    new City { Name = "Porto" },
                    new City { Name = "Faro" }
                };

                _ctx.Countries.Add(new Country
                {
                    Name = "Portugal",
                    Cities = cities
                });
                await _ctx.SaveChangesAsync();
            }

            var user = await _userHelper.GetUserByEmailAsync("alexandre.mcr.roque@gmail.com");
            if (user == null)
            {
                user = new User
                {
                    FirstName = "Alexandre",
                    LastName = "Roque",
                    Email = "alexandre.mcr.roque@gmail.com",
                    UserName = "alexandre.mcr.roque@gmail.com",
                    PhoneNumber = "123123123",
                    Address = "Real City 23",
                    City = _ctx.Cities.FirstOrDefault()
                };

                var result = await _userHelper.AddUserAsync(user, "123456");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }
                await _userHelper.AddUserToRoleAsync(user, "Admin");
            }
            bool isInRole = await _userHelper.IsUserInRoleAsync(user, "Admin");
            if (!isInRole)
            {
                await _userHelper.AddUserToRoleAsync(user, "Admin");
            }

            if (!_ctx.Products.Any())
            {
                AddProduct("iPhone X", user);
                AddProduct("Magic Mouse", user);
                AddProduct("iWatch Series 4", user);
                AddProduct("iPad Mini", user);
                await _ctx.SaveChangesAsync();
            }
        }

        private void AddProduct(string name, User user)
        {
            _ctx.Products.Add(new Product
            {
                Name = name,
                Price = _random.Next(1000),
                IsAvailable = true,
                Stock = _random.Next(200),
                User = user
            });
        }
    }
}
