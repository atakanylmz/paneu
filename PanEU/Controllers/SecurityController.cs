using PanEU.Models.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace PanEU.Controllers
{
    public class SecurityController : Controller
    {
        PaneuDBEntities PaneuDBEntities = new PaneuDBEntities();
        // GET: Security
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User user)
        {
            var foundUser = PaneuDBEntities.User.FirstOrDefault(u => u.UserName == user.UserName && u.Password == user.Password);
            if (foundUser != null)
            {
                FormsAuthentication.SetAuthCookie(user.UserName, false);
                return RedirectToAction("Index","Stores");
            }
            else
            {
                ViewBag.Message = "Invalid Username or Password!";
                return View();
            }
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return View("Login");
        }

    }
}