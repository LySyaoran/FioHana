using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Do_An.Models;

namespace Do_An.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Home()
        {
            ViewBag.Tittle = "FioHana Shop";
            return View();
        }
        public ActionResult Report()
        {
            ViewBag.Tittle = "Report";
            return View();
        }
    }
}