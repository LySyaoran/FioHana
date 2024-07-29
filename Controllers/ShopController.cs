using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Do_An.Models;

namespace Do_An.Controllers
{
    public class ShopController : Controller
    {
        private nhom1ltwebEntities db = new nhom1ltwebEntities();
        // GET: Shop
        public ActionResult Shop(string sortOrder)
        {
            if (string.IsNullOrEmpty(sortOrder))
            {
                sortOrder = "default";
            }

            ViewBag.Tittle = "Shop";
            var products = db.products.AsQueryable();

            switch (sortOrder)
            {
                case "desc":
                    products = products.OrderByDescending(p => p.gia);
                    break;
                case "asc":
                    products = products.OrderBy(p => p.gia);
                    break;
                default:
                    products = products.OrderBy(p => p.id);
                    break;
            }

            return View(products.ToList());
        }

        public ActionResult PhanLoaiHoa(int id, string tieu_chi)
        {
            var products = new List<product>();

            if (tieu_chi.Equals("loai_hoa"))
            {
                products = db.products.Where(p => p.loai_hoa_id == id).ToList();
            }
            else if (tieu_chi.Equals("dip"))
            {
                products = db.products.Where(p => p.dip_phu_hop_id == id).ToList();
            }
            else
            {
                products = db.products.Where(p => p.kich_thuoc.Equals(tieu_chi)).ToList();
            }

            return View("Shop", products);
        }

        public ActionResult ShopSingle()
        {
            ViewBag.Tittle = "Shop Detail";
            return View();
        }
    }
}