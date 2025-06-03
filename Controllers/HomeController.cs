using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using e_ticaret.Models;
using e_ticaret.Data;
using System.Collections.Generic;
using System.Linq;
using e_ticaret.Extensions;
using Microsoft.AspNetCore.Http;

namespace e_ticaret.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ProductRepository _productRepository;
    private readonly OrderRepository _orderRepository;
    private readonly UserRepository _userRepository;

    public HomeController(
        ILogger<HomeController> logger,
        ProductRepository productRepository,
        OrderRepository orderRepository,
        UserRepository userRepository)
    {
        _logger = logger;
        _productRepository = productRepository;
        _orderRepository = orderRepository;
        _userRepository = userRepository;
    }

    public IActionResult Index()
    {
        try
        {
            var products = _productRepository.GetAll();
            if (!products.Any())
            {
                _logger.LogWarning("No products found in the database.");
            }
            return View(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching products");
            return View(new List<Product>());
        }
    }

    public IActionResult Urunler()
    {
        var products = _productRepository.GetAll();
        return View(products);
    }

    public IActionResult UrunDetay(int id)
    {
        var product = _productRepository.GetById(id);
        if (product == null)
        {
            TempData["Error"] = "Ürün bulunamadı.";
            return RedirectToAction("Index");
        }

        return View("ProductDetails", product);
    }

    [HttpPost]
    public IActionResult AddToCart(int productId, int quantity)
    {
        if (HttpContext.Session.GetString("UserId") == null)
        {
            TempData["Error"] = "Sepete ürün eklemek için lütfen giriş yapın.";
            return RedirectToAction("Login", "Account");
        }

        var product = _productRepository.GetById(productId);
        if (product == null || product.StockQuantity < quantity)
        {
            TempData["Error"] = "Ürün stokta yok veya yeterli stok bulunmuyor.";
            return RedirectToAction("UrunDetay", new { id = productId });
        }

        List<CartItem> cart = HttpContext.Session.GetObject<List<CartItem>>("cart") ?? new List<CartItem>();
        var cartItem = cart.FirstOrDefault(x => x.ProductId == productId);
        
        if (cartItem != null)
        {
            if (cartItem.Quantity + quantity > product.StockQuantity)
            {
                TempData["Error"] = "Stok miktarını aşamazsınız.";
                return RedirectToAction("UrunDetay", new { id = productId });
            }
            cartItem.Quantity += quantity;
        }
        else
        {
            cart.Add(new CartItem 
            { 
                Product = product, 
                Quantity = quantity, 
                ProductId = productId 
            });
        }

        HttpContext.Session.SetObject("cart", cart);
        TempData["Success"] = "Ürün sepete eklendi.";
        return RedirectToAction("Sepetim");
    }

    public IActionResult Sepetim()
    {
       

        var cart = HttpContext.Session.GetObject<List<CartItem>>("cart") ?? new List<CartItem>();
        
        // Sepetteki ürünlerin güncel bilgilerini al
        foreach (var item in cart)
        {
            item.Product = _productRepository.GetById(item.ProductId);
        }
        
        return View(cart);
    }

    [HttpPost]
    public IActionResult RemoveFromCart(int productId)
    {
        var cart = HttpContext.Session.GetObject<List<CartItem>>("cart");
        if (cart != null)
        {
            cart.RemoveAll(x => x.ProductId == productId);
            HttpContext.Session.SetObject("cart", cart);
        }
        return RedirectToAction("Sepetim");
    }

    [HttpPost]
    public IActionResult UpdateCartQuantity(int productId, int quantity)
    {
        var cart = HttpContext.Session.GetObject<List<CartItem>>("cart");
        if (cart != null)
        {
            var item = cart.FirstOrDefault(x => x.ProductId == productId);
            if (item != null)
            {
                var product = _productRepository.GetById(productId);
                if (product != null && quantity <= product.StockQuantity)
                {
                    item.Quantity = quantity;
                    HttpContext.Session.SetObject("cart", cart);
                    return Json(new { success = true });
                }
            }
        }
        return Json(new { success = false });
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult OdemeYap()
    {
        if (HttpContext.Session.GetString("UserId") == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var cart = HttpContext.Session.GetObject<List<CartItem>>("cart");
        if (cart == null || !cart.Any())
        {
            return RedirectToAction("Sepetim");
        }

        return View(cart);
    }

    [HttpPost]
    public IActionResult OdemeYap(string cardName, string cardNumber, string expiryDate, string cvv,
                                string address, string city, string postalCode)
    {
        var cart = HttpContext.Session.GetObject<List<CartItem>>("cart");
        if (cart == null || !cart.Any())
        {
            TempData["Error"] = "Sepetiniz boş.";
            return RedirectToAction("Sepetim");
        }

        var userIdStr = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var user = _userRepository.GetById(userId);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Stok kontrolü
        foreach (var item in cart)
        {
            var product = _productRepository.GetById(item.ProductId);
            if (product == null || product.StockQuantity < item.Quantity)
            {
                TempData["Error"] = $"{item.Product?.Name} için yeterli stok bulunmuyor.";
                return RedirectToAction("Sepetim");
            }
        }

        var order = new Order
        {
            UserId = userId,
            OrderDate = DateTime.Now,
            OrderStatus = "Hazırlanıyor",
            ShippingAddress = address,
            BillingAddress = $"{address}, {city} {postalCode}",
            PaymentMethod = "Kredi Kartı",
            Address = address,
            City = city,
            PostalCode = postalCode,
            Total = cart.Sum(x => x.Product != null ? x.Product.Price * x.Quantity : 0),
            TotalAmount = cart.Sum(x => x.Product != null ? x.Product.Price * x.Quantity : 0),
            Items = cart.Where(x => x.Product != null).Select(item => new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = item.Product?.Price ?? 0,
                ProductName = item.Product?.Name ?? "Ürün Bulunamadı"
            }).ToList()
        };

        // Stokları güncelle
        foreach (var item in cart)
        {
            _productRepository.DecrementStock(item.ProductId, item.Quantity);
        }

        _orderRepository.AddOrder(order);
        HttpContext.Session.Remove("cart");
        TempData["Success"] = "Siparişiniz başarıyla oluşturuldu.";

        return RedirectToAction("SiparisDurum", new { id = order.Id });
    }

    public IActionResult SiparisDurum(int id)
    {
        var order = _orderRepository.GetOrderById(id);
        if (order == null)
        {
            TempData["Error"] = "Sipariş bulunamadı.";
            return RedirectToAction("Index");
        }
        return View(order);
    }

    public IActionResult Siparislerim()
    {
        var userIdStr = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var orders = _orderRepository.GetUserOrders(userId);
        return View(orders);
    }

    [HttpGet]
    public IActionResult HemenAl(int productId, int quantity = 1)
    {
        if (HttpContext.Session.GetString("UserId") == null)
        {
            return Json(new { success = false, message = "Satın almak için lütfen giriş yapın." });
        }

        var product = _productRepository.GetById(productId);
        if (product == null || product.StockQuantity < quantity)
        {
            return Json(new { success = false, message = "Ürün stokta yok veya yeterli stok bulunmuyor." });
        }

        // Clear existing cart and create a new one with just this item
        var cartItem = new CartItem
        {
            Product = product,
            ProductId = productId,
            Quantity = quantity
        };

        var cart = new List<CartItem> { cartItem };
        HttpContext.Session.SetObject("cart", cart);

        return Json(new { success = true });
    }
}
