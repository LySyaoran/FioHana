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

        public ActionResult PhanLoaiHoa(int id, string tieu_chi, string sortOrder)
        {
            if (string.IsNullOrEmpty(sortOrder))
            {
                sortOrder = "default";
            }

            ViewBag.Tittle = "Shop";
            var products = db.products.AsQueryable();

            if (tieu_chi.Equals("loai_hoa"))
            {
                products = products.Where(p => p.loai_hoa_id == id);
            }
            else if (tieu_chi.Equals("dip"))
            {
                products = db.products.Where(p => p.dip_phu_hop_id == id);
            }
            else
            {
                products = db.products.Where(p => p.kich_thuoc.Equals(tieu_chi));
            }

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

            
            return View("Shop", products);
        }

        public ActionResult ShopSingle(int id)
        {
            var product = db.products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            var occasion = db.occasions.Find(product.dip_phu_hop_id).ten;
            var relatedProducts = db.products
                .Where(p => p.loai_hoa_id == product.loai_hoa_id && p.id != product.id)
                .ToList();

            var viewModel = new ShopSingleViewModel
            {
                Product = product,
                RelatedProducts = relatedProducts,
                Occasion = occasion
            };

            return View(viewModel);
        }



        public ActionResult Product_Search(string search)
        {
            ViewBag.Tittle = "Product_Search";
            var products = db.products
                .Where(p => p.ten.ToLower().Contains(search.ToLower()))
                .ToList();
            return View("Product_Search", products);
        }
    }
}