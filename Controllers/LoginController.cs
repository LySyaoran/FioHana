using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Do_An.Models;
using System.Security.Cryptography;
using System.Text;

namespace Do_An.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        [HttpGet]
        public ActionResult Login()
        {
            ViewBag.Tittle = "Login";
            return View();
        }

        [HttpPost]
        public ActionResult Login(user a)
        {
            using (var db = new nhom1ltwebEntities())
            {
                string hashedPassword = GetMd5Hash(a.password);
                var user = db.users.FirstOrDefault(u => u.email == a.email && u.password == hashedPassword);
                if (user != null)
                {
                    Session["UserId"] = user.id;
                    return RedirectToAction("Success", new { ID = user.id });
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password");
                    return View();
                }
            }
        }

        [HttpGet]
        public ActionResult Signup()
        {
            ViewBag.Tittle = "Signup";
            return View();
        }

        [HttpPost]
        public ActionResult Signup(user a)
        {
            if (ModelState.IsValid)
            {
                using (var db = new nhom1ltwebEntities())
                {
                    a.password = GetMd5Hash(a.password);
                    db.users.Add(a);
                    db.SaveChanges();
                }
                return RedirectToAction("Login");
            }
            else
            {
                return View();
            }
        }

        public ActionResult Success(int ID)
        {
            ViewBag.Tittle = "Manager Account";
            using (var db = new nhom1ltwebEntities())
            {
                var user = db.users.FirstOrDefault(u => u.id == ID);
                if (user != null)
                {
                    ViewBag.ImagePath = Url.Content("~/Assets/img/Uploads/" + user.img_user);
                    return View(user);
                }
            }
            return RedirectToAction("Error", "Home");
        }


        // Hàm mã hóa MD5
        private string GetMd5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}