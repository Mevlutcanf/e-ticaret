using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using e_ticaret.Data;
using e_ticaret.Models;
using e_ticaret.ViewModels;
using System.Linq;

namespace e_ticaret.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("admin")]
    public class AdminController : Controller
    {
        private readonly ProductRepository _productRepository;
        private readonly OrderRepository _orderRepository;
        private readonly UserRepository _userRepository;

        public AdminController(
            ProductRepository productRepository,
            OrderRepository orderRepository,
            UserRepository userRepository)
        {
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var viewModel = new AdminDashboardViewModel
            {
                Products = _productRepository.GetAll(),
                RecentOrders = _orderRepository.GetAllOrders().Take(5).ToList(),
                UserCount = _userRepository.GetAll().Count,
                OrderCount = _orderRepository.GetAll().Count
            };

            return View(viewModel);
        }

        [HttpGet("products")]
        public IActionResult Products()
        {
            var products = _productRepository.GetAll();
            return View(products);
        }

        [HttpGet("products/{id}")]
        public IActionResult ProductDetails(int id)
        {
            var product = _productRepository.GetById(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost("products")]
        public IActionResult AddProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _productRepository.Add(product);
            return Ok(product);
        }

        [HttpPut("products/{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] Product product)
        {
            if (id != product.Id)
                return BadRequest();

            _productRepository.Update(product);
            return Ok(product);
        }

        [HttpDelete("products/{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = _productRepository.GetById(id);
            if (product == null)
                return NotFound();

            _productRepository.Delete(id);
            return Ok();
        }

        [HttpGet("orders")]
        public IActionResult Orders()
        {
            var orders = _orderRepository.GetAllOrders();
            return View(orders);
        }

        [HttpGet("orders/{id}")]
        public IActionResult OrderDetail(int id)
        {
            var order = _orderRepository.GetOrderById(id);
            if (order == null)
                return NotFound();

            return View(order);
        }

        [HttpGet("users")]
        public IActionResult Users()
        {
            var users = _userRepository.GetAll();
            return View(users);
        }

        [HttpPost("users/{id}/toggle-status")]
        public IActionResult ToggleUserStatus(int id)
        {
            var user = _userRepository.GetById(id);
            if (user == null)
                return NotFound();

            user.IsActive = !user.IsActive;
            _userRepository.Update(user);
            return Ok(new { isActive = user.IsActive });
        }
    }
} 