using ClosedXML.Excel;
using QLCHVT_65130239.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace QLCHVT_65130239.Controllers
{
    public class QuanLy_65130239Controller : Controller
    {
        // GET: QuanLy_65130239
        QLCHVTTB_65130239Entities db = new QLCHVTTB_65130239Entities();

        public ActionResult Index()
        {
            // Gán giả lập để test giao diện LayoutQuanLy
            Session["MaNV"] = "NV000";
            Session["TenNV"] = "Nguyễn Văn Q (Quản Lý)";
            Session["VaiTro"] = 1;

            ViewBag.TongDon = db.DONHANGs.Count();

            ViewBag.ChoDuyet = db.DONHANGs.Count(d => d.TRANGTHAI.Trim() == "CHODUYET");
            ViewBag.DaDuyet = db.DONHANGs.Count(d => d.TRANGTHAI.Trim() == "DADUYET");
            ViewBag.DangGiao = db.DONHANGs.Count(d => d.TRANGTHAI.Trim() == "DANGGIAO");
            ViewBag.HoanTat = db.DONHANGs.Count(d => d.TRANGTHAI.Trim() == "HOANTAT");
            ViewBag.DaHuy = db.DONHANGs.Count(d => d.TRANGTHAI.Trim() == "DAHUY");

            // NHÂN VIÊN
            ViewBag.TongNV = db.NHANVIENs.Count();
                ViewBag.NVDuyetDon = db.NHANVIENs.Count(n => n.VAITRO == 2);
                ViewBag.NVGiaoHang = db.NHANVIENs.Count(n => n.VAITRO == 3);

                // KHÁCH HÀNG
                ViewBag.TongKH = db.KHACHHANGs.Count();
                ViewBag.KHHoatDong = db.KHACHHANGs.Count(k => k.TRANGTHAI == 1);
                ViewBag.KHBiKhoa = db.KHACHHANGs.Count(k => k.TRANGTHAI != 1);

                // SẢN PHẨM
                ViewBag.TongSP = db.SANPHAMs.Count();
                ViewBag.SPHetHang = db.SANPHAMs.Count(s => s.SOLUONG <= 0);

                return View();
            
        }
        // 1. QUẢN LÝ SẢN PHẨM
        public ActionResult DanhSachSanPham(string searchString)
        {
            // Lấy query gốc từ database
            var products = db.SANPHAMs.AsQueryable();

            // Thực hiện lọc nếu người dùng có nhập từ khóa
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim().ToLower();
                products = products.Where(p => p.TENSP.ToLower().Contains(searchString)
                                            || p.MASP.ToLower().Contains(searchString));

                // Truyền từ khóa ngược lại View để hiển thị ở ô tìm kiếm
                ViewBag.CurrentSearch = searchString;
            }

            // Trả về danh sách (đã lọc hoặc chưa lọc)
            return View(products.ToList());
        }
        // GET: Hiển thị form thêm mới
        public ActionResult Create()
        {
            // Lấy danh sách nhóm sản phẩm để hiển thị trong DropdownList
            ViewBag.MANHOM = new SelectList(db.NHOMSANPHAMs, "MANHOM", "TENNHOM");
            return View();
        }

        // POST: Xử lý dữ liệu gửi từ form lên
        // POST: Xử lý dữ liệu gửi từ form lên
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MASP,TENSP,MANHOM,DONVI,GIANHAP,GIABAN,SOLUONG,MOTA")] SANPHAM sanpham, HttpPostedFileBase uploadHinh)
        {
            // --- PHẦN KIỂM TRA TRÙNG MÃ MỚI THÊM VÀO ---
            var isExist = db.SANPHAMs.Any(x => x.MASP == sanpham.MASP);
            if (isExist)
            {
                ModelState.AddModelError("MASP", "Mã sản phẩm này đã tồn tại. Vui lòng kiểm tra lại!");
            }
            // ------------------------------------------

            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Xử lý tải ảnh lên
                    if (uploadHinh != null && uploadHinh.ContentLength > 0)
                    {
                        // Lấy tên file
                        string fileName = Path.GetFileName(uploadHinh.FileName);
                        // Đường dẫn lưu file: ~/Images/tenfile.jpg
                        string path = Path.Combine(Server.MapPath("~/Images/"), fileName);

                        // Lưu file vào thư mục Images trên Server
                        uploadHinh.SaveAs(path);

                        // Lưu tên file vào database
                        sanpham.HINHANH = fileName;
                    }

                    // 2. Gán mặc định mã nhân viên đang quản lý (lấy từ Session)
                    sanpham.MANV = Session["MaNV"]?.ToString() ?? "NV000";

                    // 3. Lưu vào Database
                    db.SANPHAMs.Add(sanpham);
                    db.SaveChanges();

                    return RedirectToAction("DanhSachSanPham");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                }
            }

            // Nếu dữ liệu không hợp lệ hoặc trùng mã, load lại danh sách nhóm sản phẩm và trả về View
            ViewBag.MANHOM = new SelectList(db.NHOMSANPHAMs, "MANHOM", "TENNHOM", sanpham.MANHOM);
            return View(sanpham);
        }
        // 1. CHI TIẾT SẢN PHẨM
        public ActionResult ChiTietSanPham(string id)
        {
            var sanpham = db.SANPHAMs.Find(id);
            if (sanpham == null) return HttpNotFound();
            return View(sanpham);
        }

        // 2. SỬA SẢN PHẨM (GET: Hiển thị form)
        public ActionResult SuaSanPham(string id)
        {
            var sanpham = db.SANPHAMs.Find(id);
            if (sanpham == null) return HttpNotFound();

            ViewBag.MANHOM = new SelectList(db.NHOMSANPHAMs, "MANHOM", "TENNHOM", sanpham.MANHOM);
            return View(sanpham);
        }

        // 3. SỬA SẢN PHẨM (POST: Lưu thay đổi)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaSanPham([Bind(Include = "MASP,TENSP,MANHOM,DONVI,GIANHAP,GIABAN,SOLUONG,MOTA,HINHANH,MANV")] SANPHAM sanpham, HttpPostedFileBase uploadHinh)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (uploadHinh != null && uploadHinh.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(uploadHinh.FileName);
                        string path = Path.Combine(Server.MapPath("~/Images/"), fileName);
                        uploadHinh.SaveAs(path);
                        sanpham.HINHANH = fileName; // Cập nhật ảnh mới
                    }

                    db.Entry(sanpham).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("DanhSachSanPham");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi cập nhật: " + ex.Message);
                }
            }
            ViewBag.MANHOM = new SelectList(db.NHOMSANPHAMs, "MANHOM", "TENNHOM", sanpham.MANHOM);
            return View(sanpham);
        }
        // 2. QUẢN LÝ NHÂN VIÊN
        public ActionResult DanhSachNhanVien(string searchString)
        {
            // Lấy tất cả nhân viên
            var list = db.NHANVIENs.AsQueryable();

            // Lọc nếu có từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim().ToLower();
                list = list.Where(n => n.TENNV.ToLower().Contains(searchString)
                                    || n.MANV.ToLower().Contains(searchString));

                // Gửi từ khóa ngược lại View để hiển thị trong ô nhập
                ViewBag.CurrentSearch = searchString;
            }

            return View(list.ToList());
        }
        // 1. CHI TIẾT NHÂN VIÊN
        public ActionResult ChiTietNhanVien(string id)
        {
            var nv = db.NHANVIENs.Find(id.Trim());
            if (nv == null) return HttpNotFound();
            return View(nv);
        }

        // 2. THÊM NHÂN VIÊN (GET)
        public ActionResult ThemNhanVien()
        {
            return View();
        }

        // 2. THÊM NHÂN VIÊN (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemNhanVien(NHANVIEN nv)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng mã
                var check = db.NHANVIENs.Find(nv.MANV.Trim());
                if (check != null)
                {
                    ViewBag.Error = "Mã nhân viên này đã tồn tại!";
                    return View(nv);
                }
                db.NHANVIENs.Add(nv);
                db.SaveChanges();
                return RedirectToAction("DanhSachNhanVien");
            }
            return View(nv);
        }

        // 3. SỬA NHÂN VIÊN (GET)
        public ActionResult SuaNhanVien(string id)
        {
            var nv = db.NHANVIENs.Find(id.Trim());
            if (nv == null) return HttpNotFound();
            return View(nv);
        }

        // 3. SỬA NHÂN VIÊN (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaNhanVien(NHANVIEN nv)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nv).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DanhSachNhanVien");
            }
            return View(nv);
        }
        // 3. QUẢN LÝ KHÁCH HÀNG (Có chức năng Khóa/Mở khóa)
        public ActionResult DanhSachKhachHang(string searchString)
        {
            // Lấy toàn bộ danh sách khách hàng
            var khachHangs = db.KHACHHANGs.AsQueryable();

            // Nếu có nhập từ khóa tìm kiếm
            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();
                khachHangs = khachHangs.Where(kh => kh.TENKH.Contains(searchString) || kh.MAKH.Contains(searchString));
                ViewBag.SearchValue = searchString;
            }

            return View(khachHangs.ToList());
        }
        // 1. CHI TIẾT KHÁCH HÀNG
        public ActionResult ChiTietKhachHang(string id)
        {
            var kh = db.KHACHHANGs.Find(id.Trim());
            if (kh == null) return HttpNotFound();
            return View(kh);
        }

        // 2. CHỈNH SỬA KHÁCH HÀNG (GET)
        public ActionResult SuaKhachHang(string id)
        {
            var kh = db.KHACHHANGs.Find(id.Trim());
            if (kh == null) return HttpNotFound();
            return View(kh);
        }

        // 2. CHỈNH SỬA KHÁCH HÀNG (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaKhachHang(KHACHHANG kh)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kh).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DanhSachKhachHang");
            }
            return View(kh);
        }

        // 3. THAY ĐỔI TRẠNG THÁI NHANH (Hạn chế/Mở khóa)
        public ActionResult ThayDoiTrangThaiKH(string id)
        {
            var kh = db.KHACHHANGs.Find(id.Trim());
            if (kh != null)
            {
                // Nếu đang là 1 (Hoạt động) thì chuyển thành 2 (Hạn chế) và ngược lại
                kh.TRANGTHAI = (kh.TRANGTHAI == 1) ? 2 : 1;
                db.SaveChanges();
            }
            return RedirectToAction("DanhSachKhachHang");
        }
        // 4. QUẢN LÝ ĐƠN HÀNG
        // 4. QUẢN LÝ ĐƠN HÀNG
        public ActionResult DanhSachDonHang(string search)
        {
            // Sử dụng Include để lấy kèm thông tin Khách hàng
            var ds = db.DONHANGs.Include("KHACHHANG").AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                ds = ds.Where(x => x.MADH.Contains(search) || x.KHACHHANG.TENKH.Contains(search));
            }

            return View(ds.OrderByDescending(x => x.NGAYLAP).ToList());
        }

        // 1. XEM CHI TIẾT ĐƠN HÀNG
        public ActionResult ChiTietDonHang(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction("DanhSachDonHang");

            var idTrim = id.Trim();
            var donhang = db.DONHANGs
                .Include("KHACHHANG")
                .Include("CTDHs.SANPHAM")
                .Include("NHANVIEN")
                .Include("NHANVIEN1")
                .FirstOrDefault(x => x.MADH.Trim() == idTrim);

            if (donhang == null) return HttpNotFound();
            return View(donhang);
        }

        // 2. SỬA ĐƠN HÀNG (Giao diện)
        // GET: Sửa đơn hàng
        public ActionResult SuaDonHang(string id)
        {
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            var idTrim = id.Trim();
            var donhang = db.DONHANGs.FirstOrDefault(x => x.MADH.Trim() == idTrim);

            if (donhang == null) return HttpNotFound();

            // Lấy danh sách nhân viên để chọn
            // Bạn có thể lọc .Where(n => n.VAITRO == ...) nếu muốn tách riêng bộ phận
            ViewBag.MANV_DUYET = new SelectList(db.NHANVIENs, "MANV", "TENNV", donhang.MANV_DUYET);
            ViewBag.MANV_GIAO = new SelectList(db.NHANVIENs, "MANV", "TENNV", donhang.MANV_GIAO);

            return View(donhang);
        }

        // 3. SỬA ĐƠN HÀNG (Xử lý lưu)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaDonHang(DONHANG model)
        {
            var updateModel = db.DONHANGs.FirstOrDefault(x => x.MADH == model.MADH);
            if (updateModel != null)
            {
                updateModel.TRANGTHAI = model.TRANGTHAI;
                updateModel.MANV_DUYET = model.MANV_DUYET;
                updateModel.MANV_GIAO = model.MANV_GIAO;
                updateModel.DIACHIGIAO = model.DIACHIGIAO;
                updateModel.PHIVANCHUYEN = model.PHIVANCHUYEN;

                db.SaveChanges();
                return RedirectToAction("DanhSachDonHang");
            }
            return View(model);
        }




        public ActionResult ThongKe(DateTime? tuNgay, DateTime? denNgay, string kieu = "ngay")
        {
            // ================== 1. MẶC ĐỊNH NGÀY ==================
            if (!tuNgay.HasValue || !denNgay.HasValue)
            {
                tuNgay = DateTime.Today.AddDays(-7);
                denNgay = DateTime.Today;
            }

            // 🔴 FIX QUAN TRỌNG: cộng thêm 1 ngày để lấy trọn ngày cuối
            DateTime denNgayFix = denNgay.Value.AddDays(1);

            ViewBag.TuNgay = tuNgay.Value.ToString("yyyy-MM-dd");
            ViewBag.DenNgay = denNgay.Value.ToString("yyyy-MM-dd");
            ViewBag.Kieu = kieu;

            // ================== 2. QUERY GỐC ==================
            var query = db.CTDHs
                .Where(ct =>
                    ct.DONHANG.TRANGTHAI.Trim() == "HOANTAT" &&
                    ct.DONHANG.NGAYLAP >= tuNgay.Value &&
                    ct.DONHANG.NGAYLAP < denNgayFix   // ❗ DÙNG <, KHÔNG DÙNG <=
                );

            List<ThongKeModel> data;

            // ================== 3. THỐNG KÊ THEO NGÀY ==================
            if (kieu == "ngay")
            {
                data = query
                    .GroupBy(ct => DbFunctions.TruncateTime(ct.DONHANG.NGAYLAP))
                    .AsEnumerable()
                    .Select(g => new ThongKeModel
                    {
                        Nhan = g.Key.Value.ToString("dd/MM/yyyy"),
                        SoDonHang = g.Select(x => x.MADH).Distinct().Count(),
                        LoiNhuan = g.Sum(x =>
                            x.SOLUONG * (x.DONGIA - x.SANPHAM.GIANHAP)
                        )
                    })
                    .OrderBy(x => DateTime.ParseExact(x.Nhan, "dd/MM/yyyy", null))
                    .ToList();
            }

            // ================== 4. THỐNG KÊ THEO THÁNG ==================
            else if (kieu == "thang")
            {
                data = query
                    .GroupBy(ct => new
                    {
                        ct.DONHANG.NGAYLAP.Year,
                        ct.DONHANG.NGAYLAP.Month
                    })
                    .AsEnumerable()
                    .Select(g => new ThongKeModel
                    {
                        Nhan = $"{g.Key.Month:00}/{g.Key.Year}",
                        SoDonHang = g.Select(x => x.MADH).Distinct().Count(),
                        LoiNhuan = g.Sum(x =>
                            x.SOLUONG * (x.DONGIA - x.SANPHAM.GIANHAP)
                        )
                    })
                    .OrderBy(x => x.Nhan)
                    .ToList();
            }

            // ================== 5. THỐNG KÊ THEO NĂM ==================
            else
            {
                data = query
                    .GroupBy(ct => ct.DONHANG.NGAYLAP.Year)
                    .Select(g => new ThongKeModel
                    {
                        Nhan = g.Key.ToString(),
                        SoDonHang = g.Select(x => x.MADH).Distinct().Count(),
                        LoiNhuan = g.Sum(x =>
                            x.SOLUONG * (x.DONGIA - x.SANPHAM.GIANHAP)
                        )
                    })
                    .OrderBy(x => x.Nhan)
                    .ToList();
            }

            return View(data);
        }



        // ================== XUẤT EXCEL ==================
        public FileResult XuatExcel(DateTime tuNgay, DateTime denNgay)
        {
            var data = db.CTDHs
                .Where(ct =>
                    ct.DONHANG.TRANGTHAI.Trim() == "HOANTAT" &&
                    ct.DONHANG.NGAYLAP >= tuNgay &&
                    ct.DONHANG.NGAYLAP <= denNgay
                )
                .GroupBy(ct => DbFunctions.TruncateTime(ct.DONHANG.NGAYLAP))
                .Select(g => new
                {
                    Ngay = g.Key,
                    SoDon = g.Select(x => x.MADH).Distinct().Count(),
                    LoiNhuan = g.Sum(x => x.SOLUONG * (x.DONGIA - x.SANPHAM.GIANHAP))
                })
                .ToList();

            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("ThongKe");

                ws.Cell(1, 1).Value = "Ngày";
                ws.Cell(1, 2).Value = "Số đơn";
                ws.Cell(1, 3).Value = "Lợi nhuận";

                int row = 2;
                foreach (var item in data)
                {
                    ws.Cell(row, 1).Value = item.Ngay.HasValue
                        ? item.Ngay.Value.ToString("dd/MM/yyyy")
                        : "";

                    ws.Cell(row, 2).Value = item.SoDon;
                    ws.Cell(row, 3).Value = item.LoiNhuan;
                    row++;
                }

                // Format cho đẹp
                ws.Columns().AdjustToContents();
                ws.Column(3).Style.NumberFormat.Format = "#,##0";

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "ThongKe.xlsx"
                    );
                }
            }
        }

    }

}
