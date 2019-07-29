using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using UserLoginWithTwiloo.Classes;
using UserLoginWithTwiloo.Models;

namespace UserLoginWithTwiloo.Controllers
{
    public class HomeController : Controller
    {
        DatabaseSpExec _db;
        IConfiguration _configuration;
        public HomeController(DatabaseSpExec db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(User user)
        {
            var registerId = _db.Register(user);
            string code = CodeGenerator.GetCode();
            var verId = _db.CreateVerification(new Verification { Code = code });
            HttpContext.Response.Cookies.Append("verId", verId.ToString());
            HttpContext.Response.Cookies.Append("userId", registerId.ToString());
            return RedirectToAction("Verify");
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(User user)
        {
            var userId = _db.Login(user.Username, user.Password);
            if (userId != 0) // means not found
            {
                string code = CodeGenerator.GetCode();
                var verId = _db.CreateVerification(new Verification { Code = code });
                HttpContext.Response.Cookies.Append("verId", verId.ToString());
                HttpContext.Response.Cookies.Append("userId", userId.ToString());
                var userFromDb = _db.GetUser(userId);
                string accountSid = _configuration["Twiloo:accountSid"].ToString();
                string authToken = _configuration["Twiloo:authToken"].ToString();
                string twilooPhoneNumber = _configuration["Twiloo:phoneNumber"].ToString();
                TwilioClient.Init(accountSid, authToken);
                var message = MessageResource.Create(
                    body: "Verification Code is Below " + Environment.NewLine + code,
                    from: new Twilio.Types.PhoneNumber(twilooPhoneNumber),
                    to: new Twilio.Types.PhoneNumber(userFromDb.Phone)
                );
                return RedirectToAction("Verify");
            }
            else
            {
                return View();
            }

        }

        [HttpGet]
        public IActionResult Verify()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Verify(Verification ver)
        {
            var verId = HttpContext.Request.Cookies.FirstOrDefault(x => x.Key == "verId").Value;
            HttpContext.Response.Cookies.Delete("verId");
            if (ver.Code == _db.GetVerificationCode(int.Parse(verId)))
            {
                ViewData["success"] = "<script>alert('Welcome To Site')</script>";
                return RedirectToAction("Index");
            }
            HttpContext.Response.Cookies.Delete("userId");
            return RedirectToAction("Login");

        }
    }
}
