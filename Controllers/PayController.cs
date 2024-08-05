using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Do_An.Models;

namespace Do_An.Controllers
{
    public class PayController : Controller
    {
        // GET: Pay
        public ActionResult Pay()
        {
            // Lấy các mặt hàng giỏ hàng đã chọn từ session
            var selectedItems = Session["SelectedCartItems"] as List<cart>;

            // Nếu không có sản phẩm nào, chuyển hướng về trang giỏ hàng
            if (selectedItems == null || !selectedItems.Any())
            {
                return RedirectToAction("Cart", "ShoppingCart");
            }

            var count = 0;

            using (var db = new nhom1ltwebEntities())
            {
                // Load full product information from the database if not already loaded
                foreach (var item in selectedItems)
                {
                    if (item.product == null)
                    {
                        item.product = db.products.FirstOrDefault(p => p.id == item.id_product);
                    }
                    count++;
                }
            }

            // Tính tổng tiền hàng
            var totalAmount = selectedItems.Sum(item => item.soluong_sp * item.product.gia);
            ViewBag.TotalAmount = totalAmount;
            ViewBag.Count = count;

            decimal shippingFee = 32000m;
            ViewBag.ShippingFee = shippingFee;

            ViewBag.FinalAmount = totalAmount + shippingFee;

            return View(selectedItems);
        }

        [HttpPost]
        public ActionResult ApplyPromotion(string promoCode)
        {
            using (var db = new nhom1ltwebEntities())
            {
                var promotion = db.promotions.FirstOrDefault(p => p.code_promotions == promoCode);
                if (promotion == null)
                {
                    TempData["PromotionError"] = "Mã giảm giá không hợp lệ.";
                    return RedirectToAction("Pay");
                }

                var selectedItems = Session["SelectedCartItems"] as List<cart>;
                if (selectedItems == null || !selectedItems.Any())
                {
                    return RedirectToAction("Cart", "ShoppingCart");
                }

                var totalAmount = selectedItems.Sum(item => item.soluong_sp * item.product.gia);

                if (totalAmount == null)
                {
                    TempData["PromotionError"] = "Không thể tính tổng tiền hàng.";
                    return RedirectToAction("Pay");
                }

                decimal discountAmount = 0m;

                if (promotion.value_promotions.EndsWith("%"))
                {
                    var percentage = int.Parse(promotion.value_promotions.TrimEnd('%'));
                    discountAmount = (totalAmount.Value * percentage) / 100;
                }
                else
                {
                    discountAmount = decimal.Parse(promotion.value_promotions);
                }

                decimal shippingFee = 32000m;
                ViewBag.ShippingFee = shippingFee;

                ViewBag.TotalAmount = totalAmount;
                ViewBag.FinalAmount = totalAmount - discountAmount + shippingFee;
                ViewBag.DiscountAmount = discountAmount;
                ViewBag.PromotionCode = promoCode;

                return View("Pay", selectedItems);
            }
        }

    }
}