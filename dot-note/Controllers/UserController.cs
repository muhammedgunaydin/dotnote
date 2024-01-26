using dot_note.DbContext;
using dot_note.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace dot_note.Controllers
{
    public class UserController : Controller
    {
        private readonly MongoDbContext _context;

        public UserController(MongoDbContext context)
        {
            _context = context;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(User user)
        {
            var existingUser = _context.Users.Find(u => u.Email == user.Email).FirstOrDefault();
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "This email address is already in use.");
            }

            if (ModelState.IsValid)
            {
                _context.Users.InsertOne(user);
                return RedirectToAction("Login", "User");
            }

            TempData["ErrorMessage"] = "There was an error in the registration process.";

            return View(user);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users.Find(i => i.Email == email && i.Password == password).FirstOrDefault();

            if (user != null)
            {
                HttpContext.Session.SetString("UserId", user.Id);
                Console.WriteLine(user.Id);
                TempData["Message"] = "Login successfull.";
                return RedirectToAction("Index", "note");
            }
            TempData["Error"] = "Invalid email or password";
            return View(user);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserId");

            return RedirectToAction("Login", "User");
        }
    }
}
