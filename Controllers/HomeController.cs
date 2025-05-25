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

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var products = ProductRepository.GetAll();
        return View(products);
    }

    public IActionResult Urunler()
    {
        var products = ProductRepository.GetAll();
        return View(products);
    }

    [HttpPost]
    public IActionResult SepeteEkle(int id)
    {
        if (HttpContext.Session.GetString("UserId") == null)
        {
            TempData["Message"] = "Sepete ürün eklemek için lütfen giriş yapın.";
            return RedirectToAction("Login", "Account");
        }

        var product = ProductRepository.GetAll().FirstOrDefault(x => x.Id == id);
        if (product == null || product.StockQuantity < 1)
            return RedirectToAction("Index");

        if (!ProductRepository.UpdateStock(id, 1))
            return RedirectToAction("Index");

        List<CartItem> cart = HttpContext.Session.GetObject<List<CartItem>>("cart") ?? new List<CartItem>();
        var cartItem = cart.FirstOrDefault(x => x.Product?.Id == id);
        if (cartItem != null)
            cartItem.Quantity++;
        else
            cart.Add(new CartItem { Product = product, Quantity = 1 });

        HttpContext.Session.SetObject("cart", cart);
        return RedirectToAction("Sepetim");
    }

    public IActionResult Sepetim()
    {
        var cart = HttpContext.Session.GetObject<List<CartItem>>("cart") ?? new List<CartItem>();
        return View(cart);
    }

    public IActionResult UrunDetay(int id)
    {
        var product = ProductRepository.GetAll().FirstOrDefault(x => x.Id == id);
        if (product == null)
            return RedirectToAction("Index");

        return View(product);
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
        var cart = HttpContext.Session.GetObject<List<CartItem>>("cart") ?? new List<CartItem>();
        return View(cart);
    }

    [HttpPost]
    public IActionResult OdemeYap(string cardName, string cardNumber, string expiryDate, string cvv,
                                string address, string city, string postalCode)
    {
        var cart = HttpContext.Session.GetObject<List<CartItem>>("cart");
        if (cart == null || !cart.Any())
            return RedirectToAction("Index");

        var userId = HttpContext.Session.GetString("UserId") ?? throw new InvalidOperationException("User not logged in");

        var order = new Order
        {
            UserId = userId,
            Address = address,
            City = city,
            PostalCode = postalCode,
            TotalAmount = cart.Sum(x => x.Product != null ? x.Product.Price * x.Quantity : 0),
            Items = cart
        };

        OrderRepository.AddOrder(order);
        HttpContext.Session.Remove("cart");

        return RedirectToAction("SiparisDurum", order);
    }

    public IActionResult SiparisDurum(Order order)
    {
        return View(order);
    }

    public IActionResult Siparislerim()
    {
        var userId = HttpContext.Session.GetString("UserId");
        if (userId == null)
            return RedirectToAction("Login", "Account");

        var orders = OrderRepository.GetUserOrders(userId);
        return View(orders);
    }
}
