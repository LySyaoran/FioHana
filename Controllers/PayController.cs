using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Do_An.Models;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using VNPAY_CS_ASPX;

namespace Do_An.Controllers
{
    public class PayController : Controller
    {
        private nhom1ltwebEntities db = new nhom1ltwebEntities();
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
            Session["FinalAmount"] = totalAmount + shippingFee;
            Session["TotalAmount"] = totalAmount;

            return View(selectedItems);
        }


        [HttpPost]
        public ActionResult ApplyPromotion(string promoCode)
        {
            using (var db = new nhom1ltwebEntities())
            {
                var promotion = db.promotions.FirstOrDefault(p => p.code_promotions == promoCode);
                Session["PromotionID"] = promotion?.id;
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
                Session["FinalAmount"] = totalAmount - discountAmount + shippingFee;
                Session["DiscountAmount"] = discountAmount;
                Session["TotalAmount"] = totalAmount;
                ViewBag.DiscountAmount = discountAmount;
                ViewBag.PromotionCode = promoCode;

                return View("Pay", selectedItems);
            }
        }

        [HttpPost]
        public ActionResult SubmitOrder(string firstName, string lastName, string email, string address, string cityName, string districtName, string wardName, string paymentMethod)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Kết hợp các phần của địa chỉ
            string fullAddress = $"{address}, {wardName}, {districtName}, {cityName}";
            Session["address"] = $"{address}";
            Session["ward"] = $"{wardName}";
            Session["district"] = $"{districtName}";
            Session["city"] = $"{cityName}";

            var order = new order
            {
                khach_hang_id = (int)Session["UserID"],
                ho_ten = $"{firstName} {lastName}",
                email = email,
                dia_chi_nhan_hang = fullAddress,
                phuong_thuc_thanh_toan1 = paymentMethod,
                promotion_id = Session["PromotionID"] != null ? (int?)Session["PromotionID"] : null,
                tong_tien = (decimal)Session["FinalAmount"],
                ngay_dat_hang = DateTime.Now,
                trang_thai = "Chưa giao hàng",
            };

            Session["DateOrder"] = order.ngay_dat_hang;
            Session["PaymentMethod"] = order.phuong_thuc_thanh_toan1;
            Session["Name"] = order.ho_ten;

            // Thêm order vào DbContext và lưu vào cơ sở dữ liệu
            db.orders.Add(order);
            db.SaveChanges();

            Session["OrderID"] = order.id;

            // Chuyển hướng người dùng đến trang xác nhận đơn hàng hoặc trang khác
            if (order.phuong_thuc_thanh_toan1 == "vnpay")
            {
                return RedirectToAction("UrlPayment", new { orderCode = order.id });
            }
            else
            {
                if (order.phuong_thuc_thanh_toan1 == "cod")
                {
                    return RedirectToAction("OrderConfirmation", new { orderCode = order.id });
                }
                else
                {
                    return RedirectToAction("OrderConfirmation", new { orderCode = order.id });
                }
            }
        }

        public ActionResult UrlPayment(int orderCode)
        {
            // Lấy thông tin cấu hình từ appSettings
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"];
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"];
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"];
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];

            var order1 = db.orders.FirstOrDefault(x => x.id == orderCode);
            var price = (long)(order1.tong_tien * 100);

            // Tạo dữ liệu yêu cầu cho VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", price.ToString());
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn"); // Bạn có thể thay đổi theo nhu cầu
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + DateTime.Now.Ticks);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString());
            string ExpireDate = DateTime.Now.AddMinutes(30).ToString("yyyyMMddHHmmss");
            vnpay.AddRequestData("vnp_ExpireDate", ExpireDate);


            // Tạo URL thanh toán
            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return Redirect(paymentUrl);
        }

        public ActionResult VnpayReturn()
        {
            var vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];
            var vnpayData = Request.QueryString;
            VnPayLibrary vnpay = new VnPayLibrary();

            foreach (string s in vnpayData)
            {
                if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(s, vnpayData[s]);
                }
            }

            long orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
            long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
            string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
            string vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
            string TerminalID = Request.QueryString["vnp_TmnCode"];
            long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
            string bankCode = Request.QueryString["vnp_BankCode"];

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);

            if (checkSignature)
            {
                if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                {
                    // Thanh toán thành công
                    ViewBag.Message = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";

                    // Cập nhật trạng thái đơn hàng trong cơ sở dữ liệu
                    var order = db.orders.FirstOrDefault(o => o.id == orderId);

                    if (order != null)
                    {
                        order.trang_thai = "Paid"; // Hoặc trạng thái tương ứng của bạn
                        db.SaveChanges();
                    }

                    var selectedItems = Session["SelectedCartItems"] as List<cart>;
                    foreach (var item in selectedItems)
                    {
                        if (item == null)
                        {
                            Console.WriteLine("Item is null");
                        }
                        else
                        {
                            // Ensure product is loaded
                            if (item.product == null)
                            {
                                item.product = db.products.FirstOrDefault(p => p.id == item.id_product);
                            }

                            if (item.product != null)
                            {
                                var orderDetail = new order_details
                                {
                                    don_hang_id = (int)Session["OrderID"],
                                    san_pham_id = item.id_product,
                                    so_luong = item.soluong_sp,
                                    don_gia = (decimal)(item.product.gia * item.soluong_sp),
                                    giam_gia = 0m
                                };
                                db.order_details.Add(orderDetail);

                                // Cập nhật số lượng tồn kho
                                var change_quantity = db.products.FirstOrDefault(p => p.id == item.id_product);
                                change_quantity.so_luong_ton_kho -= item.soluong_sp;

                                // Xóa sản phẩm khỏi giỏ hàng
                                var cartItem = db.carts.FirstOrDefault(c => c.id_user == item.id_user && c.id_product == item.id_product);
                                if (cartItem != null)
                                {
                                    db.carts.Remove(cartItem);
                                }

                                db.SaveChanges();
                            }
                            else
                            {
                                Console.WriteLine("Product is null for item with id_product: " + item.id_product);
                            }
                        }
                    }

                }
                else
                {
                    // Thanh toán không thành công
                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý. Mã lỗi: " + vnp_ResponseCode;
                }

                ViewBag.OrderId = orderId.ToString();
                ViewBag.VnpayTranId = vnpayTranId.ToString();
                ViewBag.Amount = vnp_Amount.ToString();
                ViewBag.BankCode = bankCode;
                ViewBag.TerminalID = TerminalID;
            }
            else
            {
                ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
                ViewBag.OrderId = "N/A";
                ViewBag.VnpayTranId = "N/A";
                ViewBag.Amount = "N/A";
                ViewBag.BankCode = "N/A";
                ViewBag.TerminalID = "N/A";
            }

            return View();
        }

        public ActionResult OrderConfirmation()
        {
            var selectedItems = Session["SelectedCartItems"] as List<cart>;
            foreach (var item in selectedItems)
            {
                if (item == null)
                {
                    Console.WriteLine("Item is null");
                }
                else
                {
                    // Ensure product is loaded
                    if (item.product == null)
                    {
                        item.product = db.products.FirstOrDefault(p => p.id == item.id_product);
                    }

                    if (item.product != null)
                    {
                        var orderDetail = new order_details
                        {
                            don_hang_id = (int)Session["OrderID"],
                            san_pham_id = item.id_product,
                            so_luong = item.soluong_sp,
                            don_gia = (decimal)(item.product.gia * item.soluong_sp),
                            giam_gia = 0m
                        };
                        db.order_details.Add(orderDetail);

                        // Cập nhật số lượng tồn kho
                        var change_quantity = db.products.FirstOrDefault(p => p.id == item.id_product);
                        change_quantity.so_luong_ton_kho -= item.soluong_sp;

                        // Xóa sản phẩm khỏi giỏ hàng
                        var cartItem = db.carts.FirstOrDefault(c => c.id_user == item.id_user && c.id_product == item.id_product);
                        if (cartItem != null)
                        {
                            db.carts.Remove(cartItem);
                        }

                        db.SaveChanges();
                    }
                    else
                    {
                        Console.WriteLine("Product is null for item with id_product: " + item.id_product);
                    }
                }
            }

            return View();
        }
    }
}