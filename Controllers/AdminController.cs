using Microsoft.AspNetCore.Mvc;
using e_ticaret.Data;
using e_ticaret.Models;

namespace e_ticaret.Controllers
{
    public class AdminController : Controller
    {
        private bool IsAdmin()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null) return false;

            var user = UserRepository.GetById(int.Parse(userId));
            return user?.IsAdmin ?? false;
        }

        public IActionResult Index()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var orders = OrderRepository.GetAllOrders();
            var users = UserRepository.GetAll();
            var products = ProductRepository.GetAll();

            ViewBag.OrderCount = orders.Count;
            ViewBag.UserCount = users.Count;
            ViewBag.ProductCount = products.Count;
            ViewBag.TotalRevenue = orders.Sum(o => o.TotalAmount);

            // Son 5 sipariş
            ViewBag.RecentOrders = orders
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .Select(o => new
                {
                    o.Id,
                    o.OrderNumber,
                    o.TotalAmount,
                    o.OrderStatus,
                    UserFullName = UserRepository.GetById(int.Parse(o.UserId))?.FullName ?? "Bilinmiyor"
                });

            // Tüm ürünleri gönderelim
            ViewBag.AllProducts = products
                .OrderBy(p => p.StockQuantity);

            return View();
        }

        public IActionResult Orders()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            return View(OrderRepository.GetAllOrders());
        }

        public IActionResult OrderDetail(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var order = OrderRepository.GetOrderById(id);
            if (order == null) return NotFound();

            var user = UserRepository.GetById(int.Parse(order.UserId));
            ViewBag.UserFullName = user?.FullName ?? "Bilinmiyor";

            return View(order);
        }

        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var product = ProductRepository.GetById(id);
            if (product == null) return NotFound();

            return View(product);
        }

        [HttpPost]
        public IActionResult EditProduct(int id, [FromForm] Product product)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            product.Id = id; // ID'yi URL'den al
            ProductRepository.Update(product);
            TempData["Message"] = "Ürün başarıyla güncellendi.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateOrderStatus(int id, string status)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var order = OrderRepository.GetOrderById(id);
            if (order == null) return NotFound();

            order.OrderStatus = status;
            if (status == "Teslim Edildi")
            {
                order.DeliveryDate = DateTime.Now;
            }

            OrderRepository.UpdateOrder(order);
            TempData["Message"] = "Sipariş durumu güncellendi.";

            return RedirectToAction("OrderDetail", new { id });
        }

        public IActionResult AddProduct()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            return View(new Product());
        }

        [HttpPost]
        public IActionResult AddProduct(Product product)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            ProductRepository.Add(product);
            TempData["Message"] = "Ürün başarıyla eklendi.";
            return RedirectToAction("Index");
        }

        public IActionResult UpdateStock(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var product = ProductRepository.GetById(id);
            if (product == null) return NotFound();

            return View(product);
        }

        [HttpPost]
        public IActionResult UpdateStock(int id, int quantity)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            ProductRepository.UpdateStock(id, quantity);
            TempData["Message"] = "Stok başarıyla güncellendi.";
            return RedirectToAction("Index");
        }
    }
}
