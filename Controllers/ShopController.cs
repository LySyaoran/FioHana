using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Do_An.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Shop()
        {
            ViewBag.Tittle = "Shop";
            return View();
        }
        public ActionResult ShopSingle()
        {
            ViewBag.Tittle = "Shop Detail";
            return View();
        }
    }
}