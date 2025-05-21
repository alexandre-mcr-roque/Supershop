using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Supershop.Data;

namespace Supershop.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderRepository _orderRepository;

        public OrdersController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _orderRepository.GetOrderAsync(User.Identity!.Name!);
            return View(model);
        }
    }
}
