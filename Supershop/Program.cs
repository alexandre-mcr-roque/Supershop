using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Supershop.Data;
using Supershop.Data.Entities;
using Supershop.Helpers;

namespace Supershop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddIdentity<User, IdentityRole>(cfg =>
            {
                cfg.User.RequireUniqueEmail = true;
                cfg.Password.RequireDigit = false;
                cfg.Password.RequiredUniqueChars = 0;
                cfg.Password.RequireLowercase = false;
                cfg.Password.RequireUppercase = false;
                cfg.Password.RequireNonAlphanumeric = false;
                cfg.Password.RequiredLength = 6;
            })
                .AddEntityFrameworkStores<DataContext>();

            builder.Services.AddDbContext<DataContext>(cfg =>
            {
                cfg.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddTransient<SeedDb>();
            builder.Services.AddScoped<IUserHelper, UserHelper>();
            builder.Services.AddScoped<IBlobHelper, BlobHelper>();
            builder.Services.AddScoped<IConverterHelper, ConverterHelper>();

            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<ICountryRepository, CountryRepository>();

            builder.Services.ConfigureApplicationCookie(cfg =>
            {
                cfg.LoginPath = "/Account/NotAuthorized";   // Force AccessDenied
                cfg.AccessDeniedPath = "/Account/NotAuthorized";
            });

            var app = builder.Build();
            RunSeeding(app);

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Errors/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/error/{0}");

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }

        private static void RunSeeding(WebApplication app)
        {
            var scopeFactory = app.Services.GetService<IServiceScopeFactory>()!;
            using (var scope = scopeFactory.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetService<SeedDb>()!;
                seeder.SeedAsync().Wait();
            }
        }
    }
}
