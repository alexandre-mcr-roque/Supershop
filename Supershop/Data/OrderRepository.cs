﻿using Microsoft.AspNetCore.Connections.Features;
using Microsoft.EntityFrameworkCore;
using Supershop.Data.Entities;
using Supershop.Helpers;
using Supershop.Models;

namespace Supershop.Data
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly IUserHelper _userHelper;

        public OrderRepository(DataContext context, IUserHelper userHelper) : base(context)
        {
            _userHelper = userHelper;
        }

        public async Task AddItemToOrderAsync(AddItemViewModel model, string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);
            if (user == null)
            {
                return;
            }

            var product = await _context.Products.FindAsync(model.ProductId);
            if (product == null)
            {
                return;
            }

            var orderDetailTemp = await _context.OrderDetailsTemp
                .Where(odt => odt.User == user && odt.Product == product)
                .FirstOrDefaultAsync();
            if (orderDetailTemp == null)
            {
                orderDetailTemp = new OrderDetailTemp
                {
                    Price = product.Price,
                    Product = product,
                    Quantity = model.Quantity,
                    User = user
                };
                _context.OrderDetailsTemp.Add(orderDetailTemp);
            }
            else
            {
                orderDetailTemp.Quantity += model.Quantity;
                if (orderDetailTemp.Quantity <= 0) return;
                _context.OrderDetailsTemp.Update(orderDetailTemp);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<IQueryable<OrderDetailTemp>?> GetOrderDetailsTempAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);
            if (user == null)
            {
                return null;
            }

            return _context.OrderDetailsTemp
                .Include(odt => odt.Product)
                .Where(odt => odt.User == user)
                .OrderBy(odt => odt.Product.Name);
        }

        public async Task<IQueryable<Order>?> GetOrderAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);
            if (user == null)
            {
                return null;
            }

            if (await _userHelper.IsUserInRoleAsync(user, "Admin"))
            {
                return _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                    .OrderByDescending(o => o.OrderDate);
            }

            return _context.Orders
                    .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                    .Where(o => o.User == user)
                    .OrderByDescending(o => o.OrderDate);
        }

        public async Task ModifyOrderDetailTempQuantityAsync(int id, double quantity)
        {
            var orderDetailTemp = await _context.OrderDetailsTemp.FindAsync(id);
            if (orderDetailTemp == null)
            {
                return;
            }

            orderDetailTemp.Quantity += quantity;
            if (orderDetailTemp.Quantity > 0)
            {
                _context.OrderDetailsTemp.Update(orderDetailTemp);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteOrderDetailTempAsync(int id)
        {
            var orderDetailTemp = await _context.OrderDetailsTemp.FindAsync(id);
            if (orderDetailTemp == null)
            {
                return;
            }

            _context.OrderDetailsTemp.Remove(orderDetailTemp);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ConfirmOrderAsync(string userName)
        {
            var user = await _userHelper.GetUserByEmailAsync(userName);
            if (user == null)
            {
                return false;
            }

            var orderTmp = await _context.OrderDetailsTemp
                .Include(odt => odt.Product)
                .Where(odt => odt.User == user)
                .ToListAsync();
            if (orderTmp == null || orderTmp.Count == 0)
            {
                return false;
            }

            var details = orderTmp.Select(odt => new OrderDetail
            {
                Price = odt.Price,
                Product = odt.Product,
                Quantity = odt.Quantity
            }).ToList();

            var order = new Order
            {
                OrderDate = DateTime.UtcNow,
                User = user,
                Items = details
            };

            _context.OrderDetailsTemp.RemoveRange(orderTmp);    // changes will be saved after calling CreateAsync()
            await CreateAsync(order);
            return true;
        }

        public async Task DeliverOrder(DeliveryViewModel model)
        {
            var order = await _context.Orders.FindAsync(model.Id);
            if (order == null)
            {
                return;
            }

            order.DeliveryDate = model.DeliveryDate;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task<Order?> GetOrderAsync(int id)
        {
            return await _context.Orders.FindAsync(id);
        }
    }
}