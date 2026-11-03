using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using QLCHVT_65130239.Models;
using System.Data.Entity;

namespace QLCHVT_65130239.Controllers
{
    public class GioHang_65130239Controller : Controller
    {
        private QLCHVTTB_65130239Entities db = new QLCHVTTB_65130239Entities();

        // Hàm cập nhật lại số lượng giỏ hàng vào Session
        private void UpdateCartCount(string magh)
        {
            int count = db.CTGHs.Where(c => c.MAGH == magh).Sum(c => (int?)c.SOLUONG) ?? 0;
            Session["CartCount"] = count;
        }
        // 1. THÊM VÀO GIỎ HÀNG → NHẢY TỚI GIỎ HÀNG
        public ActionResult ThemGioHang(string maSP, string url)
        {
            if (Session["MaKH"] == null)
            {
                // 🔥 GHI NHỚ SẢN PHẨM CẦN THÊM
                Session["PendingAddToCart"] = maSP;

                return RedirectToAction("Login", "TaiKhoan_65130239");
            }

            if (string.IsNullOrEmpty(maSP))
                return RedirectToAction("Index", "TrangChu_65130239");

            string maKH = Session["MaKH"].ToString().Trim();
            maSP = maSP.Trim();

            var sanPham = db.SANPHAMs.Find(maSP);
            if (sanPham == null)
                return RedirectToAction("Index", "TrangChu_65130239");

            // 1. Tìm hoặc tạo giỏ hàng
            var gioHang = db.GIOHANGs.FirstOrDefault(g => g.MAKH.Trim() == maKH);
            if (gioHang == null)
            {
                gioHang = new GIOHANG
                {
                    MAGH = "GH" + maKH.Replace(" ", ""),
                    MAKH = maKH
                };
                db.GIOHANGs.Add(gioHang);
                db.SaveChanges();
            }

            // 2. Thêm / cập nhật sản phẩm
            var chiTiet = db.CTGHs.FirstOrDefault(c => c.MAGH == gioHang.MAGH && c.MASP == maSP);
            if (chiTiet == null)
                db.CTGHs.Add(new CTGH { MAGH = gioHang.MAGH, MASP = maSP, SOLUONG = 1 });
            else
                chiTiet.SOLUONG++;

            db.SaveChanges();

            // 3. Cập nhật icon giỏ hàng
            UpdateCartCount(gioHang.MAGH);

            // 4. Thông báo thành công
            TempData["Success"] = "Đã thêm sản phẩm vào giỏ hàng!";

            // 5. LUÔN nhảy sang trang giỏ hàng
            return RedirectToAction("Index", "GioHang_65130239");
        }


        // 2. TRANG GIỎ HÀNG
        public ActionResult Index()
        {
            if (Session["MaKH"] == null) return RedirectToAction("Login", "TaiKhoan_65130239");

            string maKH = Session["MaKH"].ToString().Trim();
            var gioHang = db.GIOHANGs.FirstOrDefault(g => g.MAKH.Trim() == maKH);

            if (gioHang == null) return View(new List<CartItem>());

            var dsGioHang = db.CTGHs.Where(c => c.MAGH == gioHang.MAGH)
                .Select(c => new CartItem
                {
                    MASP = c.MASP,
                    TENSP = c.SANPHAM.TENSP,
                    HINHANH = c.SANPHAM.HINHANH,
                    GIABAN = (decimal)c.SANPHAM.GIABAN,
                    SOLUONG = (int)c.SOLUONG
                }).ToList();

            ViewBag.TongTien = dsGioHang.Sum(s => s.THANHTIEN);
            UpdateCartCount(gioHang.MAGH);

            return View(dsGioHang);
        }

        // 3. CẬP NHẬT SỐ LƯỢNG
        public ActionResult CapNhatSoLuong(string id, int soLuong)
        {
            string maKH = Session["MaKH"]?.ToString().Trim();
            var item = db.CTGHs.FirstOrDefault(c => c.GIOHANG.MAKH.Trim() == maKH && c.MASP.Trim() == id.Trim());

            if (item != null)
            {
                if (soLuong > 0) item.SOLUONG = soLuong;
                else db.CTGHs.Remove(item);
                db.SaveChanges();
                UpdateCartCount(item.MAGH);
            }
            return RedirectToAction("Index");
        }

        // 4. XÓA SẢN PHẨM
        public ActionResult XoaGioHang(string maSP)
        {
            string maKH = Session["MaKH"]?.ToString().Trim();
            var item = db.CTGHs.FirstOrDefault(c => c.GIOHANG.MAKH.Trim() == maKH && c.MASP.Trim() == maSP.Trim());

            if (item != null)
            {
                string magh = item.MAGH;
                db.CTGHs.Remove(item);
                db.SaveChanges();
                UpdateCartCount(magh);
            }
            return RedirectToAction("Index");
        }

        // 5. ĐẶT HÀNG
        [HttpPost]
        public ActionResult DatHang(string diaChiGiao)
        {
            if (Session["MaKH"] == null) return RedirectToAction("Login", "TaiKhoan_65130239");

            string maKH = Session["MaKH"].ToString().Trim();
            var dsTrongGio = db.CTGHs.Where(c => c.GIOHANG.MAKH.Trim() == maKH).ToList();
            if (!dsTrongGio.Any()) return RedirectToAction("Index");

            try
            {
                // Thêm giây (ss) và 3 số ngẫu nhiên để đảm bảo duy nhất
                Random rd = new Random();
                string maDH = "DH" + DateTime.Now.ToString("ddHHmmss");
                var dh = new DONHANG
                {
                    MADH = maDH,
                    MAKH = maKH,
                    NGAYLAP = DateTime.Now,
                    DIACHIGIAO = diaChiGiao,
                    TRANGTHAI = "CHODUYET",
                    PHIVANCHUYEN = 10000,
                    TONGTIEN = dsTrongGio.Sum(s => s.SOLUONG * s.SANPHAM.GIABAN) + 10000
                };
                db.DONHANGs.Add(dh);

                foreach (var item in dsTrongGio)
                {
                    db.CTDHs.Add(new CTDH
                    {
                        MADH = maDH,
                        MASP = item.MASP,
                        SOLUONG = (short)item.SOLUONG,
                        DONGIA = item.SANPHAM.GIABAN
                    });
                    db.CTGHs.Remove(item); // Xóa khỏi giỏ sau khi đặt
                }

                db.SaveChanges();
                Session["CartCount"] = 0;
                TempData["Success"] = "🎉 Đặt hàng thành công! Đơn hàng của bạn đang chờ duyệt.";
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                return Content("Lỗi: " + ex.Message);
            }
        }
        public ActionResult TheoDoiDonHang()
        {
            if (Session["MaKH"] == null)
                return RedirectToAction("Login", "TaiKhoan_65130239");

            string maKH = Session["MaKH"].ToString().Trim();

            var dsDonHang = db.DONHANGs
                .Where(d => d.MAKH.Trim() == maKH)
                .OrderByDescending(d => d.NGAYLAP)
                .ToList();

            return View(dsDonHang);
        }
        public ActionResult ChiTietDonHang(string id)
        {
            if (Session["MaKH"] == null)
                return RedirectToAction("Login", "TaiKhoan_65130239");

            if (string.IsNullOrEmpty(id)) return RedirectToAction("TheoDoiDonHang");

            string maKH = Session["MaKH"].ToString().Trim();
            string maDH = id.Trim();

            // Tìm đơn hàng khớp với mã đơn và phải thuộc về khách hàng đang đăng nhập (để bảo mật)
            var donHang = db.DONHANGs.FirstOrDefault(d => d.MADH == maDH && d.MAKH == maKH);

            if (donHang == null)
            {
                return HttpNotFound("Không tìm thấy đơn hàng này hoặc bạn không có quyền xem.");
            }

            return View(donHang);
        }
    }
}