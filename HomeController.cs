using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LoginReg.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace LoginReg.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
     
        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            HttpContext.Session.Clear();
            return View();
        }

        [HttpGet("Success")]
        public IActionResult Success()
        {
            int? SessionUser = HttpContext.Session.GetInt32("SessionUser");
            if(SessionUser != null){
                List<User> AllUsers = dbContext.Users.OrderByDescending(u => u.UserId).ToList();
                ViewBag.Users=AllUsers;
                return View();
            }
            return View("Index");
            
        }

        [HttpPost("")]
        public IActionResult Register(User newUser)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email is already in use!");
                    return View("Index");
                }
                PasswordHasher<User> hasher = new PasswordHasher<User>();
                newUser.Password = hasher.HashPassword(newUser, newUser.Password);
                dbContext.Add(newUser);
                dbContext.SaveChanges();

                HttpContext.Session.SetInt32("SessionUser", newUser.UserId);

                return RedirectToAction("Success");
            }
            return View("Index");
        }

        [HttpGet("login")]
        public IActionResult LoginGet()
        {
            return View("Login");
        }

        [HttpPost("login")]
        public IActionResult Login(LoginUser getUser)
        {
            if(ModelState.IsValid)
            {
                User userInDb = dbContext.Users.FirstOrDefault(u => u.Email == getUser.Email);
                if(userInDb == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email");
                    System.Console.WriteLine("XXXXXXXXXXXXXXXXXX LOGIN FAILED XXXXXXXXXXXXXXXXXXXXXXXXX");
                    return View("login");
                }

                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(getUser, userInDb.Password, getUser.Password);
                if(result == 0)
                {
                    ModelState.AddModelError("Password", "Invalid Password");
                    return View();
                }

                HttpContext.Session.SetInt32("SessionUser", userInDb.UserId);
                System.Console.WriteLine("******************** LOGIN SUCCESS ***********************");
                return RedirectToAction("Success");
            }
            System.Console.WriteLine("OOOOOOOOOOOOOOOOOOOOOO LOGIN INVALID OOOOOOOOOOOOOOOOOOOOOOO");
            return View();
            
        }

        [HttpGet("{UserId}")]
        public IActionResult GetOne(int userId)
        {
            int? SessionUser = HttpContext.Session.GetInt32("SessionUser");
            if(SessionUser != null)
            {
                User user = dbContext.Users.FirstOrDefault(u => u.UserId == userId);
                return View("Info", user);
            }
            return View("Index");
            
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
    }
}
