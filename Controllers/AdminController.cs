using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Do_An.Models;

namespace Do_An.Controllers
{
    [AdminAuthorize]
    public class AdminController : Controller
    {
        private nhom1ltwebEntities db = new nhom1ltwebEntities();

        // GET: Admin/Dashboard
        public ActionResult AdminDashboard()
        {
            ViewBag.TotalUsers = db.users.Count();
            ViewBag.TotalAdmins = db.users.Count(u => u.vai_tro.ToLower() == "admin");
            ViewBag.RecentUsers = db.users.OrderByDescending(u => u.ngay_sinh).Take(5).ToList();
            // Thêm dữ liệu cho biểu đồ nếu cần
            return View();
        }

       
        
        // GET: Admin/UserList
        public ActionResult UserList()
        {
            var users = db.users.ToList();
            return View(users);
        }

        // GET: Admin/EditUser/5
        public ActionResult EditUser(int id)
        {
            var user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Admin/EditUser/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(user userModel)
        {
            if (ModelState.IsValid)
            {
                var user = db.users.Find(userModel.id);
                if (user != null)
                {
                    user.ten = userModel.ten;
                    user.email = userModel.email;
                    user.dia_chi = userModel.dia_chi;
                    user.so_dien_thoai = userModel.so_dien_thoai;
                    user.vai_tro = userModel.vai_tro;
                    user.ghi_chu = userModel.ghi_chu;
                    user.ngay_sinh = userModel.ngay_sinh;
                    // Không cập nhật mật khẩu ở đây vì nó đã được mã hóa

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "User information updated successfully.";
                    return RedirectToAction("UserList");
                }
            }
            return View(userModel);
        }

        // GET: Admin/DeleteUser/5
        public ActionResult DeleteUser(int id)
        {
            var user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Admin/DeleteUser/5
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUserConfirmed(int id)
        {
            var user = db.users.Find(id);
            if (user != null)
            {
                db.users.Remove(user);
                db.SaveChanges();
                TempData["SuccessMessage"] = "User deleted successfully.";
            }
            return RedirectToAction("UserList");
        }


        public ActionResult ProductList()
        {
            var products = db.products.ToList();
            return View(products);
        }

        // GET: Admin/EditProduct/5
        public ActionResult EditProduct(int id)
        {
            var product = db.products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Admin/EditProduct/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProduct(product productModel)
        {
            if (ModelState.IsValid)
            {
                var product = db.products.Find(productModel.id);
                if (product != null)
                {
                    product.ten = productModel.ten;
                    product.mo_ta = productModel.mo_ta;
                    product.gia = productModel.gia;
                    product.hinh_anh = productModel.hinh_anh;
                    product.so_luong_ton_kho = productModel.so_luong_ton_kho;
                    product.ngay_nhap_kho = productModel.ngay_nhap_kho;
                    product.loai_hoa_id = productModel.loai_hoa_id;
                    product.dip_phu_hop_id = productModel.dip_phu_hop_id;
                    product.kich_thuoc = productModel.kich_thuoc;
                    product.id_gg = productModel.id_gg;
                    product.mau_sac = productModel.mau_sac;

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Product information updated successfully.";
                    return RedirectToAction("ProductList");
                }
            }
            return View(productModel);
        }

        // GET: Admin/AddProduct
        public ActionResult AddProduct()
        {
            ViewBag.Title = "Add Product";
            return View();
        }

        // POST: Admin/AddProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddProduct(product productModel)
        {
            if (ModelState.IsValid)
            {
                db.products.Add(productModel);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Product added successfully.";
                return RedirectToAction("ProductList");
            }
            return View(productModel);
        }


        // GET: Admin/DeleteProduct/5
        public ActionResult DeleteProduct(int id)
        {
            var product = db.products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Admin/DeleteProduct/5
        [HttpPost, ActionName("DeleteProduct")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteProductConfirmed(int id)
        {
            var product = db.products.Find(id);
            if (product != null)
            {
                db.products.Remove(product);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Product deleted successfully.";
            }
            return RedirectToAction("ProductList");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
            
    // Custom attribute để kiểm tra quyền admin
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAdmin = httpContext.Session["IsAdmin"];
            return isAdmin != null && (bool)isAdmin;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("~/Login/Login");
        }
    }
}
