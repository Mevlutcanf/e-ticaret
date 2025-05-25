using Microsoft.AspNetCore.Mvc;
using e_ticaret.Models;
using e_ticaret.Data;

namespace e_ticaret.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = UserRepository.GetByEmail(email);
            if (user != null && user.Password == password)
            {
                HttpContext.Session.SetString("UserId", user.Id.ToString());

                if (user.IsAdmin)
                    return RedirectToAction("Index", "Admin");

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Email veya şifre hatalı!";
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (UserRepository.Add(user))
            {
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Bu email adresi zaten kullanımda");
            return View(user);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
                return RedirectToAction("Login");

            var user = UserRepository.GetById(int.Parse(userId));
            return View(user);
        }

        [HttpPost]
        public IActionResult UpdateProfile(User user)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
                return RedirectToAction("Login");

            user.Id = int.Parse(userId);
            if (UserRepository.Update(user))
            {
                TempData["Message"] = "Profil bilgileriniz güncellendi.";
                return RedirectToAction("Profile");
            }

            return View("Profile", user);
        }
    }
}
