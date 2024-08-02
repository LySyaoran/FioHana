using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Do_An.Models;

namespace Do_An.Controllers
{
    public class ShoppingCartController : Controller
    {
        private nhom1ltwebEntities db = new nhom1ltwebEntities();
        // GET: Shop
        public ActionResult Cart()
        {
            ViewBag.Tittle = "Cart";

            var customer = Session["UserId"];

            if (customer == null)
            {
                return RedirectToAction("Login", "Login");
            }

            int userId = (int)customer;

            var order = db.orders.SingleOrDefault(o => o.khach_hang_id == userId && o.trang_thai == "Chưa hoàn thiện");
            if (order == null)
            {
                order = new order
                {
                    khach_hang_id = userId,
                    ngay_dat_hang = DateTime.Now,
                    trang_thai = "Chưa hoàn thiện"
                };
                db.orders.Add(order);
                db.SaveChanges();
            }

            var orderDetail = db.order_details
                .Include(od => od.product)
                .Where(od => od.don_hang_id == order.id)
                .ToList();

            return View(orderDetail);
        }

        [HttpPost]
        public ActionResult AddToCart(int product_id, int soLuong)
        {
            var customer = Session["UserId"];

            if (customer == null)
            {
                return RedirectToAction("Login");
            }

            int userId = (int)customer;

            var order = db.orders.SingleOrDefault(o => o.khach_hang_id == userId && o.trang_thai == "Chưa hoàn thiện");
            if (order == null)
            {
                order = new order
                {
                    khach_hang_id = userId,
                    ngay_dat_hang = DateTime.Now,
                    trang_thai = "Chưa hoàn thiện"
                };
                db.orders.Add(order);
                db.SaveChanges();
            }

            var order_detail_co_san = db.order_details.SingleOrDefault(od => od.don_hang_id == order.id && od.san_pham_id == product_id);
            if (order_detail_co_san != null)
            {
                order_detail_co_san.so_luong += soLuong;
                db.Entry(order_detail_co_san).State = EntityState.Modified;
            }
            else
            {
                var orderDetail = new order_details
                {
                    don_hang_id = order.id,
                    san_pham_id = product_id,
                    so_luong = soLuong,
                    don_gia = db.products.Find(product_id).gia
                };
                db.order_details.Add(orderDetail);
            }

            db.SaveChanges();
            return RedirectToAction("Cart");
        }
    }
}