using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using QLCHVT_65130239.Models;
using System.Data.Entity;

namespace QLCHVT_65130239.Controllers
{
    public class NhanVien_65130239Controller : Controller
    {
        private QLCHVTTB_65130239Entities db = new QLCHVTTB_65130239Entities();

        private string GetMaNV() => Session["MaNV"]?.ToString() ?? "NV001";

        // ============================================================
        // PHẦN 1: DÀNH CHO NHÂN VIÊN DUYỆT ĐƠN (VAITRO = 2)
        // ============================================================

        public ActionResult DuyetDonIndex()
        {
            // Giả lập Session (Thực tế sẽ làm ở trang Login)
            Session["MaNV"] = "NV001";
            Session["TenNV"] = "Trần Thị A (Duyệt Đơn)";
            Session["VaiTro"] = 2;

            var model = db.DONHANGs.Include(d => d.KHACHHANG).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult DuyetDon_XacNhan(string id)
        {
            var idTrim = id.Trim();
            var dh = db.DONHANGs.FirstOrDefault(x => x.MADH.Trim() == idTrim);
            if (dh != null && dh.TRANGTHAI.Trim() == "CHODUYET")
            {
                dh.TRANGTHAI = "DADUYET"; // Đã duyệt, chờ shipper nhận
                dh.MANV_DUYET = GetMaNV();
                db.Entry(dh).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("DuyetDonIndex");
        }

        [HttpPost]
        public ActionResult DuyetDon_TuChoi(string id)
        {
            var idTrim = id.Trim();
            var dh = db.DONHANGs.FirstOrDefault(x => x.MADH.Trim() == idTrim);
            if (dh != null)
            {
                dh.TRANGTHAI = "DAHUY";
                dh.MANV_DUYET = GetMaNV();
                db.Entry(dh).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("DuyetDonIndex");
        }

       
        // ============================================================
        // PHẦN 2: DÀNH CHO NHÂN VIÊN GIAO HÀNG (VAITRO = 3)
        // ============================================================

        public ActionResult GiaoHangIndex()
        {
            // Giữ nguyên phần Session của bạn
            Session["MaNV"] = "NV004";
            Session["TenNV"] = "Hoàng Văn D (Shipper)";
            Session["VaiTro"] = 3;

            string maShipper = GetMaNV();

            // SỬA TẠI ĐÂY: Thêm .Trim() cho d.TRANGTHAI
            var model = db.DONHANGs
                .Include(d => d.KHACHHANG)
                .Where(d => d.TRANGTHAI.Trim() == "DADUYET" || d.MANV_GIAO.Trim() == maShipper)
                .OrderByDescending(d => d.NGAYLAP)
                .ToList();

            return View(model);
        }

        // BƯỚC 2: Shipper nhấn "Nhận giao" -> Chuyển sang DANGGIAO
        [HttpPost]
        public ActionResult GiaoHang_NhanDon(string id)
        {
            var idTrim = id.Trim();
            var dh = db.DONHANGs.FirstOrDefault(x => x.MADH.Trim() == idTrim);

            if (dh != null && dh.TRANGTHAI.Trim() == "DADUYET")
            {
                dh.TRANGTHAI = "DANGGIAO";
                dh.MANV_GIAO = GetMaNV();
                db.Entry(dh).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("GiaoHangIndex");
        }

        // BƯỚC 3: Shipper xác nhận Thành công hoặc Thất bại
        [HttpPost]
        public ActionResult GiaoHang_KetThuc(string id, string ketqua)
        {
            var idTrim = id.Trim();
            var dh = db.DONHANGs.FirstOrDefault(x => x.MADH.Trim() == idTrim);

            if (dh != null && dh.TRANGTHAI.Trim() == "DANGGIAO")
            {
                // Đổi từ HOANTAN thành HOANTAT
                dh.TRANGTHAI = (ketqua == "THANHCONG") ? "HOANTAT" : "DAHUY";
                db.Entry(dh).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("GiaoHangIndex");
        }

        // ============================================================
        // AJAX - XEM CHI TIẾT ĐƠN HÀNG (DÙNG CHO MODAL)
        // ============================================================
        public ActionResult XemChiTietDonHang(string id)
        {
            if (string.IsNullOrEmpty(id))
                return Content("Không có mã đơn hàng");

            var idTrim = id.Trim();

            var donhang = db.DONHANGs
                .Include(d => d.KHACHHANG)
                .Include(d => d.CTDHs.Select(ct => ct.SANPHAM))
                .FirstOrDefault(d => d.MADH.Trim() == idTrim);

            if (donhang == null)
                return Content("Không tìm thấy đơn hàng");

            return PartialView("_ChiTietDonHang", donhang);
        }

    }
}