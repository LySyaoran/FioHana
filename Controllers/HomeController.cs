using Do_An.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Do_An.Controllers
{
    public class HomeController : Controller
    {
        private nhom1ltwebEntities db = new nhom1ltwebEntities();

        // GET: Home
        public ActionResult Home()
        {
            ViewBag.Tittle = "FioHana Shop";
            var products = db.products.Take(3).ToList(); // Lấy tất cả sản phẩm từ database
            return View(products);
        }
        public ActionResult Report()
        {
            ViewBag.Tittle = "Report";
            return View();
        }
    }
}