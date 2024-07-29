using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Do_An.Models;

namespace Do_An.Controllers
{
    public class AboutController : Controller
    {
        // GET: About
        public ActionResult About()
        {
            ViewBag.Tittle = "About";
            return View();
        }
        public ActionResult Promotion()
        {
            ViewBag.Tittle = "Promotion";
            return View();
        }
    }
}