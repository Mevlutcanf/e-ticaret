using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using e_ticaret.Data;
using e_ticaret.Models;

namespace e_ticaret.Controllers.Admin
{
    [Route("admin/[controller]")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        [HttpPost("update-status")]
        public async Task<IActionResult> UpdateStatus([FromBody] OrderStatusUpdateModel model)
        {
            var order = await _context.Orders.FindAsync(model.OrderId);
            if (order == null)
            {
                return NotFound();
            }

            order.OrderStatus = model.Status;
            if (model.Status == "kargoya_verildi" && string.IsNullOrEmpty(order.TrackingNumber))
            {
                order.TrackingNumber = GenerateTrackingNumber();
                order.EstimatedDeliveryDate = DateTime.Now.AddDays(3);
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        private string GenerateTrackingNumber()
        {
            return $"TR{DateTime.Now:yyyyMMdd}{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
        }
    }

    public class OrderStatusUpdateModel
    {
        public int OrderId { get; set; }
        public string Status { get; set; } = string.Empty;
    }
} 