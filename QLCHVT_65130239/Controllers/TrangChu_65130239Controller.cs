using QLCHVT_65130239.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity; // Thêm dòng này để dùng .Include()

namespace QLCHVT_65130239.Controllers
{
    public class TrangChu_65130239Controller : Controller
    {
        private QLCHVTTB_65130239Entities db = new QLCHVTTB_65130239Entities();

        // 1. HIỂN THỊ TRANG CHỦ (Không thực hiện tìm kiếm ở đây nữa)
        public ActionResult Index()
        {
            // Lấy sản phẩm mới
            ViewBag.SanPhamMoi = db.SANPHAMs.OrderByDescending(s => s.MASP).Take(4).ToList();

            // Phân nhóm hiển thị tại trang chủ
            ViewBag.ThietBiDien = db.SANPHAMs.Where(n => n.MANHOM == "N01" || n.MANHOM == "N03").Take(8).ToList();
            ViewBag.ThietBiNuoc = db.SANPHAMs.Where(n => n.MANHOM == "N02" || n.MANHOM == "N05").Take(8).ToList();

            ViewBag.NhomSP = db.NHOMSANPHAMs.ToList();

            return View();
        }

        // 2. CHỨC NĂNG TÌM KIẾM RIÊNG BIỆT
        public ActionResult TimSanPham(string searchString)
        {
            // Sử dụng .Include để lấy được thông tin NHOMSANPHAM (tránh lỗi null ở View)
            var allProducts = db.SANPHAMs.Include(s => s.NHOMSANPHAM).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                allProducts = allProducts.Where(s => s.TENSP.Contains(searchString));
            }

            ViewBag.CurrentFilter = searchString; // Để hiển thị lại từ khóa trên ô tìm kiếm
            return View(allProducts.ToList());
        }

      

        // Action hiển thị chi tiết sản phẩm
        public ActionResult ChiTietSanPham(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index");
            }
            // Tìm sản phẩm và bao gồm luôn thông tin Nhóm để hiển thị tên danh mục
            var sanPham = db.SANPHAMs.Include(s => s.NHOMSANPHAM).FirstOrDefault(s => s.MASP.Trim() == id.Trim());

            if (sanPham == null)
            {
                return HttpNotFound();
            }

            // Lấy thêm danh sách sản phẩm cùng loại để gợi ý (nếu muốn)
            ViewBag.SanPhamLienQuan = db.SANPHAMs.Where(s => s.MANHOM == sanPham.MANHOM && s.MASP != sanPham.MASP).Take(4).ToList();

            return View(sanPham);
        }
        // GET: Chi tiết tài khoản khách hàng
        public ActionResult ChiTietTaiKhoan()
        {
            // 1. Kiểm tra nếu chưa đăng nhập thì chuyển hướng về trang Login
            if (Session["USER_ID"] == null)
            {
                return RedirectToAction("Login", "TaiKhoan_65130239");
            }

            // 2. Lấy ID người dùng từ Session
            // Lưu ý: Dựa vào code quản lý của bạn, MAKH có vẻ là kiểu String (ví dụ: KH001)
            string maKH = Session["USER_ID"].ToString();

            // 3. Tìm thông tin khách hàng trong DB
            var khachHang = db.KHACHHANGs.FirstOrDefault(x => x.MAKH.Trim() == maKH.Trim());

            if (khachHang == null)
            {
                return HttpNotFound();
            }

            // 4. Trả về View cùng với Model khách hàng
            return View(khachHang);
        }

    }
}