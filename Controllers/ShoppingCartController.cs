using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Do_An.Filters;
using Do_An.Models;

namespace Do_An.Controllers
{
    public class ShoppingCartController : Controller
    {
        private nhom1ltwebEntities db = new nhom1ltwebEntities();
        // GET: Shop
        [AuthorizeUser]
        public ActionResult Cart()
        {
            int userId = (int)Session["UserId"];
            var cartItems = db.carts.Include(c => c.product)
                                    .Where(c => c.id_user == userId)
                                    .ToList();

            return View(cartItems);
        }

        [HttpPost]
        public ActionResult AddToCart(int userId, int productId, int quantity)
        {
            try
            {
                // Tạo đối tượng cart mới
                var cartItem = new cart
                {
                    id_user = userId,
                    id_product = productId,
                    soluong_sp = quantity
                };

                // Kiểm tra xem sản phẩm đã tồn tại trong giỏ hàng của user chưa
                var existingCartItem = db.carts.FirstOrDefault(c => c.id_user == userId && c.id_product == productId);
                if (existingCartItem != null)
                {
                    // Nếu tồn tại thì cập nhật số lượng
                    existingCartItem.soluong_sp += quantity;
                }
                else
                {
                    // Nếu không tồn tại thì thêm mới
                    db.carts.Add(cartItem);
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult AddToCart(int id)
        {
            try
            {
                int userId = (int)Session["UserId"];
                var existingCartItem = db.carts.FirstOrDefault(c => c.id_user == userId && c.id_product == id);
                if (existingCartItem != null)
                {
                    existingCartItem.soluong_sp += 1; // Tăng số lượng sản phẩm thêm 1
                }
                else
                {
                    var product = db.products.Find(id);
                    if (product != null)
                    {
                        var cartItem = new cart
                        {
                            id_user = userId,
                            id_product = id,
                            soluong_sp = 1
                        };
                        db.carts.Add(cartItem);
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Cart", "ShoppingCart"); // Chuyển hướng đến trang giỏ hàng
            }
            catch (Exception ex)
            {
                // Xử lý lỗi (có thể log lỗi và thông báo cho người dùng)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }



        [HttpPost]
        public ActionResult DeleteFromCart(int productId)
        {
            try
            {
                int userId = (int)Session["UserId"];
                var cartItem = db.carts.FirstOrDefault(c => c.id_user == userId && c.id_product == productId);
                if (cartItem != null)
                {
                    db.carts.Remove(cartItem);
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Item not found in cart." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult UpdateCart(int productId, int quantity)
        {
            try
            {
                int userId = (int)Session["UserId"];
                var cartItem = db.carts.FirstOrDefault(c => c.id_user == userId && c.id_product == productId);
                if (cartItem != null)
                {
                    cartItem.soluong_sp = quantity;
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Item not found in cart." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult SaveSelectedItems(List<SelectedItem> items)
        {
            int userId = (int)Session["UserId"];

            // Chuyển đổi danh sách mặt hàng đã chọn thành đối tượng cart
            var cartItems = items.Select(item => new cart
            {
                id_user = userId,
                id_product = item.productId,
                soluong_sp = item.quantity
            }).ToList();

            // Lưu các mặt hàng đã chọn vào session
            Session["SelectedCartItems"] = cartItems;

            return Json(new { success = true });
        }
    }
}