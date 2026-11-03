using QLCHVT_65130239.Models;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;



namespace QLCHVT_65130239.Controllers
{
    public class TaiKhoan_65130239Controller : Controller
    {
        private QLCHVTTB_65130239Entities db = new QLCHVTTB_65130239Entities();

        // ================= ĐĂNG NHẬP =================
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string Email, string MatKhau, string returnUrl)
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(MatKhau))
            {
                ViewBag.Error = "Vui lòng điền đầy đủ thông tin!";
                return View();
            }

            // LẤY DỮ LIỆU TỪ INPUT VÀ XOÁ KHOẢNG TRẮNG
            string emailLogin = Email.Trim();
            string plainPass = MatKhau.Trim();
            string hashPass = HashPassword(plainPass); // Mật khẩu đã băm

            // 1. KIỂM TRA BẢNG NHANVIEN
            // Thêm .Trim() vào các trường trong Database để tránh lỗi kiểu dữ liệu CHAR
            var nv = db.NHANVIENs.AsEnumerable().FirstOrDefault(x =>
                x.EMAIL.Trim().Equals(emailLogin, StringComparison.OrdinalIgnoreCase) &&
                (x.MATKHAU.Trim() == hashPass || x.MATKHAU.Trim() == plainPass)); // Cho phép cả pass băm hoặc pass thuần để test

            if (nv != null)
            {
                Session["MaNV"] = nv.MANV.Trim();
                Session["USER_ID"] = nv.MANV.Trim(); // Đồng bộ với trang chủ
                Session["TenUser"] = nv.TENNV;
                Session["VaiTro"] = nv.VAITRO;

                if (nv.VAITRO == 1) return RedirectToAction("Index", "QuanLy_65130239");
                if (nv.VAITRO == 2) return RedirectToAction("DuyetDonIndex", "NhanVien_65130239");
                if (nv.VAITRO == 3) return RedirectToAction("GiaoHangIndex", "NhanVien_65130239");
            }

            // 2. KIỂM TRA BẢNG KHACHHANG
            var kh = db.KHACHHANGs.AsEnumerable().FirstOrDefault(x =>
                x.EMAIL.Trim().Equals(emailLogin, StringComparison.OrdinalIgnoreCase) &&
                (x.MATKHAU.Trim() == hashPass || x.MATKHAU.Trim() == plainPass));

            if (kh != null)
            {
                if (kh.TRANGTHAI == 2)
                {
                    ViewBag.Error = "Tài khoản của bạn đang bị tạm khóa!";
                    return View();
                }

                // ĐỒNG BỘ SESSION VỚI TRANG CHỦ ĐỂ GIỎ HÀNG HOẠT ĐỘNG
                Session["MaKH"] = kh.MAKH.Trim();
                Session["USER_ID"] = kh.MAKH.Trim(); // TRANG CHỦ DÙNG "USER_ID"
                Session["TenUser"] = kh.TENKH;
                Session["VaiTro"] = 0;

                if (!string.IsNullOrEmpty(returnUrl)) return Redirect(returnUrl);
                return RedirectToAction("Index", "TrangChu_65130239");
            }

            ViewBag.Error = "Email hoặc mật khẩu không chính xác!";
            return View();
        }

        // ================= ĐĂNG KÝ =================
        [HttpGet]
        public ActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(KHACHHANG model, string XacNhanMatKhau)
        {
            if (db.KHACHHANGs.Any(x => x.EMAIL == model.EMAIL))
            {
                ViewBag.Error = "Email này đã được đăng ký!";
                return View(model);
            }

            if (model.MATKHAU != XacNhanMatKhau)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp!";
                return View(model);
            }

            // Tạo mã KH tự động (KH011, KH012...)
            var lastKH = db.KHACHHANGs.OrderByDescending(x => x.MAKH).FirstOrDefault();
            int nextId = (lastKH != null) ? int.Parse(lastKH.MAKH.Substring(2)) + 1 : 1;
            model.MAKH = "KH" + nextId.ToString("D3");

            model.MATKHAU = HashPassword(model.MATKHAU);
            model.TRANGTHAI = 1; // Mặc định Hoạt động

            try
            {
                db.KHACHHANGs.Add(model);
                db.SaveChanges();
                return RedirectToAction("Login");
            }
            catch
            {
                ViewBag.Error = "Có lỗi xảy ra, vui lòng thử lại!";
                return View(model);
            }
        }

        // ================= ĐĂNG XUẤT =================
        public ActionResult Logout()
        {
            Session.Clear(); // Xóa sạch các biến Session
            Session.Abandon(); // Kết thúc phiên làm việc
            return RedirectToAction("Index", "TrangChu_65130239");
        }

        // Hàm băm mật khẩu
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes) builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        void SendEmail(string toEmail, string otp)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("yourgmail@gmail.com");
            mail.To.Add(toEmail);
            mail.Subject = "Mã xác nhận đặt lại mật khẩu";
            mail.Body = $"Mã xác nhận của bạn là: {otp}";
            mail.IsBodyHtml = false;

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new 
            NetworkCredential("binh.pt.65cntt@ntu.edu.vn", "ymju cirn krgh rwfe");
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }
        [HttpPost]
        public ActionResult SendOTP(string email)
        {
            var user = db.KHACHHANGs.FirstOrDefault(x => x.EMAIL == email);
            if (user == null)
            {
                ViewBag.Error = "Email không tồn tại!";
                return View("ForgotPassword");
            }

            string otp = new Random().Next(100000, 999999).ToString();

            Session["OTP"] = otp;
            Session["OTP_EMAIL"] = email;
            Session["OTP_EXPIRE"] = DateTime.Now.AddMinutes(5);

            SendEmail(email, otp);

            return RedirectToAction("VerifyOTP");
        }
        [HttpGet]
        public ActionResult VerifyOTP()
        {
            return View();
        }

        [HttpPost]
        public ActionResult VerifyOTP(string otp, string newPassword, string confirmPassword)
        {
            if (Session["OTP"] == null || Session["OTP_EXPIRE"] == null)
            {
                ViewBag.Error = "Phiên xác nhận không hợp lệ!";
                return View();
            }

            if (DateTime.Now > (DateTime)Session["OTP_EXPIRE"])
            {
                ViewBag.Error = "Mã OTP đã hết hạn!";
                return View();
            }

            if (otp != Session["OTP"].ToString())
            {
                ViewBag.Error = "Mã OTP không đúng!";
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp!";
                return View();
            }

            string email = Session["OTP_EMAIL"].ToString();
            var user = db.KHACHHANGs.FirstOrDefault(x => x.EMAIL == email);

            if (user == null)
            {
                ViewBag.Error = "Không tìm thấy tài khoản!";
                return View();
            }

            // ✅ HASH MẬT KHẨU
            user.MATKHAU = HashPassword(newPassword);
            db.SaveChanges();

            Session.Remove("OTP");
            Session.Remove("OTP_EMAIL");
            Session.Remove("OTP_EXPIRE");

            TempData["Success"] = "Đổi mật khẩu thành công! Vui lòng đăng nhập lại.";
            return RedirectToAction("Login");
        }


        
    }
}