using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Do_An.Models;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Net;

namespace Do_An.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        [HttpGet]
        public ActionResult Login()
        {
            ViewBag.Tittle = "Login";
            return View();
        }

        [HttpPost]
        public ActionResult Login(user a)
        {
            using (var db = new nhom1ltwebEntities())
            {
                string hashedPassword = GetMd5Hash(a.password);
                var user = db.users.FirstOrDefault(u => u.email == a.email && u.password == hashedPassword);
                if (user != null)
                {
                    Session["UserId"] = user.id;

                    // Kiểm tra vai trò của user
                    if (IsAdmin(user.id))
                    {
                        Session["IsAdmin"] = true;
                        return RedirectToAction("AdminDashboard", "Admin", new { area = "Admin"});
                    }
                    else
                    {
                        Session["IsAdmin"] = false;
                        return RedirectToAction("Success", new { ID = user.id, a.password });
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password");
                    return View();
                }
            }
        }

        private readonly string clientId = "406209051907-5ab3r694h4mshbinrtqp4jtstsdiqa3l.apps.googleusercontent.com";
        private readonly string clientSecret = "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";
        private readonly string redirectUri = "https://localhost:44314/Login/GoogleCallback";

        [RequireHttps]
        public ActionResult LoginGoogle()
        {
            var state = Guid.NewGuid().ToString();
            var authorizeUrl = string.Format(
                "https://accounts.google.com/o/oauth2/auth?client_id={0}&redirect_uri={1}&response_type=code&scope=email profile&state={2}",
                clientId,
                redirectUri,
                state
            );
            return Redirect(authorizeUrl);
        }


        [RequireHttps]
        public ActionResult GoogleCallback(string code, string state)
        {
            System.Diagnostics.Debug.WriteLine("Code: " + code);
            System.Diagnostics.Debug.WriteLine("State: " + state);

            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(state))
            {
                return RedirectToAction("Login", "Login");
            }

            var tokenUrl = "https://accounts.google.com/o/oauth2/token";
            var userInfoUrl = "https://www.googleapis.com/oauth2/v2/userinfo";

            var parameters = new Dictionary<string, string>
    {
        {"code", code},
        {"client_id", clientId},
        {"client_secret", clientSecret},
        {"redirect_uri", redirectUri},
        {"grant_type", "authorization_code"}
    };

            var tokenResponse = HttpPost(tokenUrl, parameters);
            if (tokenResponse == null || !tokenResponse.ContainsKey("access_token"))
            {
                return RedirectToAction("Login", "Login");
            }

            var accessToken = tokenResponse["access_token"];

            var userInfoResponse = HttpGet(userInfoUrl, accessToken);
            if (userInfoResponse == null || !userInfoResponse.ContainsKey("email"))
            {
                return RedirectToAction("Login", "Login");
            }

            var email = userInfoResponse["email"];
            var name = userInfoResponse["name"];

            using (var db = new nhom1ltwebEntities())
            {
                var user = db.users.FirstOrDefault(u => u.email == email);

                if (user == null)
                {
                    // Tạo mới user nếu chưa tồn tại
                    user = new user
                    {
                        email = email,
                        ten = name,
                        password = GetMd5Hash(Guid.NewGuid().ToString()) // Mã hóa password bằng một chuỗi ngẫu nhiên
                    };
                    db.users.Add(user);
                    db.SaveChanges();
                }

                // Lưu thông tin người dùng vào session
                Session["UserId"] = user.id;
                Session["Email"] = user.email;
                Session["Name"] = user.ten;

                // Kiểm tra vai trò của user
                if (IsAdmin(user.id))
                {
                    Session["IsAdmin"] = true;
                    return RedirectToAction("AdminDashboard", "Admin", new { area = "Admin" });
                }
                else
                {
                    Session["IsAdmin"] = false;
                    return RedirectToAction("Success", new { ID = user.id, password = user.password });
                }
            }
        }


        private Dictionary<string, string> HttpPost(string url, Dictionary<string, string> parameters)
        {
            using (var client = new WebClient())
            {
                var nameValueCollection = new NameValueCollection();
                foreach (var parameter in parameters)
                {
                    nameValueCollection.Add(parameter.Key, parameter.Value);
                }

                var response = client.UploadValues(url, "POST", nameValueCollection);
                var result = Encoding.ASCII.GetString(response);
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
            }
        }

        private Dictionary<string, string> HttpGet(string url, string accessToken)
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("Authorization", "Bearer " + accessToken);
                var response = client.DownloadString(url);
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(response);
            }
        }


        private bool IsAdmin(int userId)
        {
            using (var db = new nhom1ltwebEntities())
            {
                var user = db.users.Find(userId);
                if (user != null)
                {
                    // Giả sử 'vai_tro' là một trường trong bảng users
                    // và giá trị "admin" đại diện cho vai trò admin
                    return string.Equals(user.vai_tro, "admin", StringComparison.OrdinalIgnoreCase);
                }
                return false;
            }
        }
        [HttpGet]
        public ActionResult Signup()
        {
            ViewBag.Tittle = "Signup";
            return View();
        }

        [HttpPost]
        public ActionResult Signup(user a)
        {
            if (ModelState.IsValid)
            {
                using (var db = new nhom1ltwebEntities())
                {
                    a.password = GetMd5Hash(a.password);
                    db.users.Add(a);
                    db.SaveChanges();
                }
                return RedirectToAction("Login");
            }
            else
            {
                return View();
            }
        }

        public ActionResult Success(int ID, string password)
        {
            ViewBag.Tittle = "Manager Account";
            using (var db = new nhom1ltwebEntities())
            {
                var user = db.users.FirstOrDefault(u => u.id == ID);
                if (user != null)
                {
                    ViewBag.ImagePath = Url.Content("~/Assets/img/Uploads/" + user.img_user);
                    ViewBag.Password = password;
                    return View(user);
                }
            }
            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        public ActionResult EditProfile(user model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var db = new nhom1ltwebEntities())
                    {
                        var user = db.users.Find(model.id);

                        if (user != null)
                        {
                            // Update user information
                            user.ten = model.ten;
                            user.dia_chi = model.dia_chi;
                            user.so_dien_thoai = model.so_dien_thoai;
                            user.email = model.email;
                            if (!string.IsNullOrEmpty(model.password))
                            {
                                user.password = GetMd5Hash(model.password);
                            }
                            user.ghi_chu = model.ghi_chu;
                            user.ngay_sinh = model.ngay_sinh;

                            // Save changes
                            db.SaveChanges();

                            //// Optionally, add a success message
                            TempData["SuccessMessage"] = "Thông tin tài khoản đã được cập nhật.";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "User not found.";
                            return RedirectToAction("Login", "Home");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error updating profile: " + ex.Message);
                    TempData["ErrorMessage"] = "An error occurred while updating the profile.";
                    return RedirectToAction("Error", "Home");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid input data.";
                return RedirectToAction("Error", "Home");
            }

            return RedirectToAction("Success", new { ID = model.id, password = model.password });
        }


        // Hàm mã hóa MD5
        private string GetMd5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

    }
}