# Hệ thống quản lý cửa hàng vật tư điện nước

## 1. Giới thiệu

**Hệ thống quản lý cửa hàng vật tư điện nước** là ứng dụng web được xây dựng nhằm hỗ trợ hoạt động quản lý và bán hàng tại cửa hàng vật tư điện nước.

Hệ thống cung cấp các chức năng phục vụ khách hàng, nhân viên và người quản lý, giúp việc quản lý sản phẩm, khách hàng, đơn hàng và hoạt động kinh doanh thuận tiện hơn.

## 2. Công nghệ sử dụng

- **Ngôn ngữ lập trình:** C#
- **Framework:** ASP.NET MVC
- **Nền tảng:** .NET Framework 4.7.2
- **Cơ sở dữ liệu:** Microsoft SQL Server
- **ORM:** Entity Framework 6
- **Giao diện:** HTML, CSS, JavaScript, Bootstrap, jQuery
- **Công cụ phát triển:** Visual Studio

## 3. Chức năng chính

### Khách hàng
- Đăng ký và đăng nhập tài khoản.
- Xác thực OTP.
- Quên mật khẩu.
- Xem và tìm kiếm sản phẩm.
- Xem chi tiết sản phẩm.
- Quản lý giỏ hàng.
- Theo dõi đơn hàng.
- Quản lý thông tin tài khoản.

### Nhân viên
- Xem và duyệt đơn hàng.
- Xem chi tiết đơn hàng.
- Quản lý hoạt động giao hàng.

### Quản lý
- Quản lý sản phẩm.
- Quản lý khách hàng.
- Quản lý nhân viên.
- Quản lý đơn hàng.
- Xem thống kê và tổng quan đơn hàng.

## 4. Cấu trúc dự án

```text
QLCHVT_65130239/
│
├── App_Start/       # Cấu hình ứng dụng và định tuyến
├── Content/         # CSS, Bootstrap
├── Controllers/     # Các Controller xử lý nghiệp vụ
├── Images/          # Hình ảnh sản phẩm và giao diện
├── Models/          # Các Model và dữ liệu Entity Framework
├── Properties/      # Thông tin cấu hình dự án
├── Scripts/         # JavaScript, jQuery, Bootstrap
├── Views/           # Các giao diện Razor
├── Web.config       # Cấu hình ứng dụng
└── QLCHVT_65130239.csproj
```

## 5. Cơ sở dữ liệu

Dự án sử dụng Microsoft SQL Server.

Các file SQL đi kèm:
- `QLCHVTTB_65130239.sql`
- `QuanLyVatTuDienNuoc-65130239.sql`

## 6. Hướng dẫn cài đặt

### Yêu cầu môi trường

- Visual Studio có hỗ trợ ASP.NET MVC.
- .NET Framework 4.7.2.
- Microsoft SQL Server.
- SQL Server Management Studio (SSMS).

### Các bước thực hiện

1. Clone repository:

   ```bash
   git clone https://github.com/thanhbinh-io/QLCHVT-65130239.git
   ```

2. Mở file `QLCHVT_65130239.sln` bằng Visual Studio.

3. Khôi phục các NuGet packages nếu Visual Studio chưa tự động thực hiện.

4. Tạo cơ sở dữ liệu trên SQL Server bằng file SQL phù hợp.

5. Mở file `Web.config` và điều chỉnh connection string theo SQL Server trên máy tính.

6. Build solution và chạy ứng dụng bằng Visual Studio.

## 7. Tác giả

**Phạm Thanh Bình**

GitHub: [thanhbinh-io](https://github.com/thanhbinh-io)

## 8. Repository

[QLCHVT-65130239](https://github.com/thanhbinh-io/QLCHVT-65130239)
