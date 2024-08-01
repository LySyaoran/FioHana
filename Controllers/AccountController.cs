using Do_An.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Do_An.Controllers
{
    public class AccountController : Controller
    {
        [HttpPost]
        public JsonResult UploadImage()
        {
            HttpPostedFileBase file = Request.Files["file"];
            int userId = Convert.ToInt32(Session["UserID"]); // Giả sử ID người dùng lưu trong session

            if (file != null && file.ContentLength > 0)
            {
                var folderPath = Server.MapPath("~/Assets/img/Uploads");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(folderPath, fileName);
                file.SaveAs(path);

                // Cập nhật tên ảnh vào cơ sở dữ liệu
                using (var db = new nhom1ltwebEntities())
                {
                    var user = db.users.FirstOrDefault(u => u.id == userId);
                    if (user != null)
                    {
                        user.img_user = fileName;
                        db.SaveChanges();
                    }
                }

                return Json(new { success = true, imagePath = Url.Content("~/Assets/img/Uploads/" + fileName) });
            }

            return Json(new { success = false });
        }




        public ActionResult Logout()
        {
            // Xóa thông tin đăng nhập
            FormsAuthentication.SignOut();

            // Xóa session nếu có
            Session.Clear();
            Session.Abandon();

            // Chuyển hướng về trang Home
            return RedirectToAction("Home", "Home");
        }
    }
}