--1.Tạo DATABASE
CREATE DATABASE QLCHVTTB2_65130239;
GO
USE QLCHVTTB2_65130239;
GO
-- 2. BẢNG NHANVIEN
CREATE TABLE NHANVIEN (
    MANV CHAR(10) PRIMARY KEY,
    TENNV NVARCHAR(40) NOT NULL,
    EMAIL NVARCHAR(100) UNIQUE,
    MATKHAU NVARCHAR(255) NOT NULL,
    VAITRO INT NOT NULL DEFAULT 1, -- 1:QL | 2:Duyet | 3:Giao | 0 : Nghỉ việc
    SDT CHAR(10) NOT NULL
);
-- 3. BẢNG KHACHHANG
CREATE TABLE KHACHHANG (
    MAKH CHAR(10) PRIMARY KEY,
    TENKH NVARCHAR(100) NOT NULL,
    SDT VARCHAR(15),
    EMAIL VARCHAR(100) UNIQUE,
    MATKHAU VARCHAR(255) NOT NULL,
    TRANGTHAI INT NOT NULL DEFAULT 1 -- 1:HoatDong | 2:HanChe
);
-- 4.BẢNG NHOMSANPHAM
CREATE TABLE NHOMSANPHAM (
    MANHOM CHAR(5) PRIMARY KEY,
    TENNHOM NVARCHAR(50) NOT NULL
);
-- 5.BẢNG SANPHAM
CREATE TABLE SANPHAM (
    MASP CHAR(10) PRIMARY KEY,
    TENSP NVARCHAR(100) NOT NULL,
    MANHOM CHAR(5) NOT NULL,
    DONVI NVARCHAR(20) NOT NULL,
    GIANHAP DECIMAL(10,2) NOT NULL,
    GIABAN DECIMAL(10,2) NOT NULL,
    SOLUONG INT NOT NULL,
    MANV CHAR(10) NOT NULL,
    MOTA NVARCHAR(500),
    HINHANH VARCHAR(255)
);
-- 6. TẠO BẢNG DONHANG
CREATE TABLE DONHANG (
    MADH CHAR(10) PRIMARY KEY,
    MAKH CHAR(10) NOT NULL,
    MANV_DUYET CHAR(10),
    MANV_GIAO CHAR(10),
    NGAYLAP SMALLDATETIME NOT NULL DEFAULT GETDATE(),
    TONGTIEN DECIMAL(10,2) NOT NULL DEFAULT 0,
    TRANGTHAI NVARCHAR(50) DEFAULT N'CHODUYET',
    PHIVANCHUYEN DECIMAL(10,2) DEFAULT 10000,
    DIACHIGIAO NVARCHAR(100)
);
-- 7. BẢNG CHI TIẾT ĐƠN HÀNG (CTDH)
CREATE TABLE CTDH (
    MADH CHAR(10) NOT NULL,
    MASP CHAR(10) NOT NULL,
    SOLUONG SMALLINT NOT NULL CHECK (SOLUONG > 0),
    DONGIA DECIMAL(10,2) NOT NULL CHECK (DONGIA >= 0),
    CONSTRAINT PK_CTDH PRIMARY KEY (MADH, MASP)
);
-- 8.BẢNG GIỎ HÀNG
CREATE TABLE GIOHANG (
    MAGH CHAR(10) PRIMARY KEY,        
    MAKH CHAR(10) NOT NULL UNIQUE
);
-- 9.BẢNG CHI TIẾT GIỎ HÀNG
CREATE TABLE CTGH (
    MAGH CHAR(10) NOT NULL,             
    MASP CHAR(10) NOT NULL,             
    SOLUONG INT NOT NULL CHECK (SOLUONG > 0),
    CONSTRAINT PK_CTGIOHANG PRIMARY KEY (MAGH, MASP)
);

--------------------------------------------------
-- THIẾT LẬP KHÓA NGOẠI (FOREIGN KEYS)
--------------------------------------------------

-- SANPHAM
ALTER TABLE SANPHAM
ADD CONSTRAINT FK_SP_NHOM FOREIGN KEY (MANHOM)
REFERENCES NHOMSANPHAM(MANHOM);

ALTER TABLE SANPHAM
ADD CONSTRAINT FK_SP_NV FOREIGN KEY (MANV)
REFERENCES NHANVIEN(MANV);

-- DONHANG
ALTER TABLE DONHANG
ADD CONSTRAINT FK_DH_KH FOREIGN KEY (MAKH)
REFERENCES KHACHHANG(MAKH);

ALTER TABLE DONHANG
ADD CONSTRAINT FK_DH_NV_DUYET FOREIGN KEY (MANV_DUYET)
REFERENCES NHANVIEN(MANV);

ALTER TABLE DONHANG
ADD CONSTRAINT FK_DH_NV_GIAO FOREIGN KEY (MANV_GIAO)
REFERENCES NHANVIEN(MANV);

-- CTDH
ALTER TABLE CTDH
ADD CONSTRAINT FK_CTDH_DH FOREIGN KEY (MADH)
REFERENCES DONHANG(MADH)
ON DELETE CASCADE;

ALTER TABLE CTDH
ADD CONSTRAINT FK_CTDH_SP FOREIGN KEY (MASP)
REFERENCES SANPHAM(MASP);

-- GIOHANG
ALTER TABLE GIOHANG
ADD CONSTRAINT FK_GH_KH FOREIGN KEY (MAKH)
REFERENCES KHACHHANG(MAKH);

-- CTGH
ALTER TABLE CTGH
ADD CONSTRAINT FK_CTGH_GH FOREIGN KEY (MAGH)
REFERENCES GIOHANG(MAGH)
ON DELETE CASCADE;

ALTER TABLE CTGH
ADD CONSTRAINT FK_CTGH_SP FOREIGN KEY (MASP)
REFERENCES SANPHAM(MASP);


---------------------------------------------
-------------------- Thêm dữ liệu -----------
---------------------------------------------
INSERT INTO NHANVIEN (MANV, TENNV, EMAIL, MATKHAU, VAITRO, SDT)
VALUES
-- Quản lý
('NV000', N'Nguyễn Văn Q', 'quanly@example.com', '123456', 1, '0909000001'),

-- Nhân viên duyệt đơn
('NV001', N'Trần Thị A', 'banhang1@example.com', '123456', 2, '0909000002'),
('NV002', N'Lê Văn B', 'banhang2@example.com', '123456', 2, '0909000003'),
('NV003', N'Phạm Thị C', 'banhang3@example.com', '123456', 2, '0909000004'),

-- Nhân viên giao hàng
('NV004', N'Hoàng Văn D', 'giaohang1@example.com', '123456', 3, '0909000005'),
('NV005', N'Ngô Thị E', 'giaohang2@example.com', '123456', 3, '0909000006'),
('NV006', N'Vũ Văn F', 'giaohang3@example.com', '123456', 3, '0909000007');
-- 18 khách HoatDong
INSERT INTO KHACHHANG (MAKH, TENKH, SDT, EMAIL, MATKHAU, TRANGTHAI)
VALUES
('KH001', N'Nguyễn Văn A', '0909000001', 'a.nguyen@example.com', '123456', 1),
('KH002', N'Trần Thị B', '0909000002', 'b.tran@example.com', '123456', 1),
('KH003', N'Lê Văn C', '0909000003', 'c.le@example.com', '123456', 1),
('KH004', N'Phạm Thị D', '0909000004', 'd.pham@example.com', '123456', 1),
('KH005', N'Hoàng Văn E', '0909000005', 'e.hoang@example.com', '123456', 1),
('KH006', N'Ngô Thị F', '0909000006', 'f.ngo@example.com', '123456', 1),
('KH007', N'Vũ Văn G', '0909000007', 'g.vu@example.com', '123456', 1),
('KH008', N'Phan Thị H', '0909000008', 'h.phan@example.com', '123456', 1),
('KH009', N'Bùi Văn I', '0909000009', 'i.bui@example.com', '123456', 1),
('KH010', N'Dương Thị J', '0909000010', 'j.duong@example.com', '123456', 1),
-- 2 khách bị Hạn Chế
('KH019', N'Nguyễn Văn S', '0909000019', 's.nguyen@example.com', '123456', 2),
('KH020', N'Trần Thị T', '0909000020', 't.tran@example.com', '123456', 2);
INSERT INTO NHOMSANPHAM VALUES
('N01', N'Dây cáp điện Cadivi'),
('N02', N'Thiết bị nước PPR'),
('N03', N'Điện Quang - Đồ gia dụng'),
('N04', N'Thiết bị điện Sino'),
('N05', N'Ống nước Bình Minh'),
('N06', N'Bóng đèn & Vật tư điện');
INSERT INTO SANPHAM
(MASP, TENSP, MANHOM, DONVI, GIANHAP, GIABAN, SOLUONG, MANV, MOTA, HINHANH)
VALUES
-- Nhóm 1: Dây cáp điện Cadivi
('SP01', N'Dây cáp điện đôi Vcmd 2x0.5 Cadivi', 'N01', N'Mét', 4500, 6000, 1000, 'NV000',
 N'Dây cáp điện đôi Cadivi chất lượng cao, dùng cho công trình dân dụng và công nghiệp.', 'sp01.jpg'),
('SP02', N'Dây cáp điện đơn CV 10.0 Cadivi', 'N01', N'Mét', 38000, 46000, 500, 'NV000',
 N'Dây cáp điện 1 lõi 10mm², chịu tải điện tốt.', 'sp02.jpg'),
('SP03', N'Dây cáp điện đơn CV 6.0 Cadivi', 'N01', N'Mét', 22000, 28000, 800, 'NV000',
 N'Dây cáp điện 1 lõi 6mm², phù hợp cho hộ gia đình.', 'sp03.jpg'),
('SP04', N'Dây cáp điện đơn CV 4.0 Cadivi', 'N01', N'Mét', 15000, 19000, 1000, 'NV000',
 N'Dây cáp điện 1 lõi 4mm², sử dụng cho các thiết bị điện nhỏ.', 'sp04.jpg'),
('SP05', N'Dây cáp điện đơn CV 2.5 Cadivi', 'N01', N'Mét', 9000, 12500, 2000, 'NV000',
 N'Dây cáp điện 1 lõi 2.5mm², dùng cho chiếu sáng dân dụng.', 'sp05.jpg'),

-- Nhóm 2: Thiết bị nước PPR
('SP06', N'Nối 32 răng trong 34 PPR-UV', 'N02', N'Cái', 60000, 75000, 50, 'NV000',
 N'Nối ống PPR chịu UV, phi 32.', 'sp06.jpg'),
('SP07', N'Nối 32 răng ngoài 34 PPR-UV', 'N02', N'Cái', 65000, 81000, 50, 'NV000',
 N'Nối ống PPR phi 32, đầu ngoài có ren.', 'sp07.jpg'),
('SP08', N'Nối phi 32 PPR-UV nước nóng', 'N02', N'Cái', 5000, 8000, 200, 'NV000',
 N'Nối PPR chịu nhiệt, phi 32.', 'sp08.jpg'),
('SP09', N'Tê phi 32 PPR-UV nước nóng', 'N02', N'Cái', 11000, 15000, 150, 'NV000',
 N'Tê 3 ngã cho hệ thống ống PPR phi 32.', 'sp09.jpg'),
('SP10', N'Van xoay phi 32 PPR-UV', 'N02', N'Cái', 55000, 75000, 20, 'NV000',
 N'Van xoay điều khiển dòng chảy, phi 32.', 'sp10.jpg'),

-- Nhóm 3: Điện Quang - Đồ gia dụng
('SP11', N'Phích cắm công suất lớn 4000W', 'N03', N'Cái', 10000, 15000, 100, 'NV000',
 N'Phích cắm điện công suất lớn, an toàn khi dùng.', 'sp11.jpg'),
('SP12', N'Ấm đun nước siêu tốc Điện Quang', 'N03', N'Cái', 135000, 180000, 30, 'NV000',
 N'Ấm siêu tốc 1.5L, nấu sôi nhanh.', 'sp12.jpg'),
('SP13', N'Phích cắm thông minh Apollo ĐQ', 'N03', N'Cái', 150000, 195000, 25, 'NV000',
 N'Phích cắm thông minh có bảo vệ quá tải.', 'sp13.jpg'),
('SP14', N'Ổ cắm điện Quang ECO 4 lỗ 2m', 'N03', N'Cái', 85000, 110000, 40, 'NV000',
 N'Ổ cắm 4 lỗ dài 2m, bọc nhựa chịu nhiệt.', 'sp14.jpg'),
('SP15', N'Bộ vợt muỗi Điện Quang', 'N03', N'Cái', 100000, 130000, 60, 'NV000',
 N'Bộ vợt muỗi tiện dụng, pin sạc nhanh.', 'sp15.jpg'),

-- Nhóm 4: Thiết bị điện Sino
('SP16', N'Ba ổ cắm 2 chấu 16A Sino', 'N04', N'Cái', 32000, 46000, 100, 'NV000',
 N'Ổ cắm Sino 2 chấu 16A.', 'sp16.jpg'),
('SP17', N'Công tắc 1 chiều Sino', 'N04', N'Cái', 6000, 9000, 300, 'NV000',
 N'Công tắc 1 chiều, chất liệu bền.', 'sp17.jpg'),
('SP18', N'Mặt 1 lỗ S181/X Sino', 'N04', N'Cái', 8000, 11000, 200, 'NV000',
 N'Mặt 1 lỗ Sino S181/X.', 'sp18.jpg'),
('SP19', N'Mặt 3 lỗ S183/X Sino', 'N04', N'Cái', 8000, 11000, 200, 'NV000',
 N'Mặt 3 lỗ Sino S183/X.', 'sp19.jpg'),
('SP20', N'Hai ổ cắm 2 chấu 16A với 1 lỗ', 'N04', N'Cái', 28000, 37000, 100, 'NV000',
 N'Bộ 2 ổ cắm 16A với 1 lỗ.', 'sp20.jpg'),

-- Nhóm 5: Ống nước Bình Minh
('SP21', N'Van xoay 32 nước nóng PPR Bình Minh', 'N05', N'Cái', 180000, 236000, 15, 'NV000',
 N'Van xoay chịu nhiệt, phi 32.', 'sp21.jpg'),
('SP22', N'Bịt trơn 25 nước nóng PPR Bình Minh', 'N05', N'Cái', 3000, 5000, 500, 'NV000',
 N'Bịt trơn phi 25, chất liệu PPR.', 'sp22.jpg'),
('SP23', N'Ống tránh 25 nước nóng PPR Bình Minh', 'N05', N'Cái', 14000, 20000, 40, 'NV000',
 N'Ống tránh phi 25, chịu nhiệt.', 'sp23.jpg'),
('SP24', N'Nối 32 nước nóng PPR Bình Minh', 'N05', N'Cái', 4000, 6000, 300, 'NV000',
 N'Nối ống PPR phi 32.', 'sp24.jpg'),
('SP25', N'Van xoay 25 nước nóng PPR Bình Minh', 'N05', N'Cái', 120000, 166000, 20, 'NV000',
 N'Van xoay phi 25, chất liệu PPR.', 'sp25.jpg'),

-- Nhóm 6: Bóng đèn & vật tư điện
('SP26', N'Bóng đèn tủ lạnh E16-15W', 'N06', N'Cái', 10000, 15000, 50, 'NV000',
 N'Bóng đèn tủ lạnh 15W E16.', 'sp26.jpg'),
('SP27', N'Bóng đèn tủ lạnh E14-15W', 'N06', N'Cái', 9000, 13000, 50, 'NV000',
 N'Bóng đèn tủ lạnh 15W E14.', 'sp27.jpg'),
('SP28', N'Bóng đèn học để bàn 11W chữ U', 'N06', N'Cái', 22000, 30000, 40, 'NV000',
 N'Bóng đèn học chữ U 11W, tiết kiệm điện.', 'sp28.jpg'),
('SP29', N'Cặp ron ống thủy tinh phi 47', 'N06', N'Cặp', 18000, 25000, 100, 'NV000',
 N'Cặp ron ống thủy tinh phi 47.', 'sp29.jpg'),
('SP30', N'Bộ ron và chén đỡ ống phi 58', 'N06', N'Bộ', 25000, 32000, 80, 'NV000',
 N'Bộ ron và chén đỡ ống phi 58.', 'sp30.jpg');

-- Hóa đơn 1
INSERT INTO DONHANG
(MADH, MAKH, MANV_DUYET, MANV_GIAO, NGAYLAP, TONGTIEN, TRANGTHAI, PHIVANCHUYEN, DIACHIGIAO)
VALUES
('DH101', 'KH001', NULL, NULL, GETDATE(), 0, N'CHODUYET', 30000, N'12 Trần Phú, Nha Trang');
INSERT INTO CTDH VALUES
('DH101', 'SP01', 50, 6000),
('DH101', 'SP05', 100, 12500),
('DH101', 'SP11', 2, 15000);
-- Hóa đơn 2
INSERT INTO DONHANG
(MADH, MAKH, MANV_DUYET, MANV_GIAO, NGAYLAP, TONGTIEN, TRANGTHAI, PHIVANCHUYEN, DIACHIGIAO)
VALUES
('DH102', 'KH002', NULL, NULL, GETDATE(), 0, N'CHODUYET', 25000, N'45 Lê Lợi, Nha Trang');
INSERT INTO CTDH VALUES
('DH102', 'SP12', 1, 180000),
('DH102', 'SP14', 2, 110000);
-- Hóa đơn 3
INSERT INTO DONHANG
(MADH, MAKH, MANV_DUYET, MANV_GIAO, NGAYLAP, TONGTIEN, TRANGTHAI, PHIVANCHUYEN, DIACHIGIAO)
VALUES
('DH103', 'KH003', NULL, NULL, GETDATE(), 0, N'CHODUYET', 20000, N'78 Thống Nhất, Nha Trang');
INSERT INTO CTDH VALUES
('DH103', 'SP06', 2, 75000),
('DH103', 'SP09', 3, 15000),
('DH103', 'SP10', 1, 75000);
-- Hóa đơn 4
INSERT INTO DONHANG
(MADH, MAKH, MANV_DUYET, MANV_GIAO, NGAYLAP, TONGTIEN, TRANGTHAI, PHIVANCHUYEN, DIACHIGIAO)
VALUES
('DH104', 'KH004', NULL, NULL, GETDATE(), 0, N'CHODUYET', 30000, N'102 Lê Hồng Phong, Nha Trang');
INSERT INTO CTDH VALUES
('DH104', 'SP16', 5, 46000),
('DH104', 'SP17', 10, 9000);
-- Hóa đơn 5
INSERT INTO DONHANG
(MADH, MAKH, MANV_DUYET, MANV_GIAO, NGAYLAP, TONGTIEN, TRANGTHAI, PHIVANCHUYEN, DIACHIGIAO)
VALUES
('DH105', 'KH005', NULL, NULL, GETDATE(), 0, N'CHODUYET', 25000, N'25 Nguyễn Thiện Thuật, Nha Trang');
INSERT INTO CTDH VALUES
('DH105', 'SP21', 1, 236000),
('DH105', 'SP25', 1, 166000);
-- ================== CẬP NHẬT TỔNG TIỀN ==================
UPDATE DONHANG
SET TONGTIEN = (
    SELECT SUM(CT.SOLUONG * CT.DONGIA)
    FROM CTDH CT
    WHERE CT.MADH = DONHANG.MADH
) + PHIVANCHUYEN
WHERE MADH IN ('DH101','DH102','DH103','DH104','DH105');

-- ================== 2 GIỎ HÀNG (HÓA ĐƠN TẠM) ==================

-- Giỏ hàng 1
INSERT INTO GIOHANG VALUES ('GH01', 'KH006');
INSERT INTO CTGH VALUES
('GH01', 'SP02', 20),
('GH01', 'SP07', 3);
-- Giỏ hàng 2
INSERT INTO GIOHANG VALUES ('GH02', 'KH007');
INSERT INTO CTGH VALUES
('GH02', 'SP18', 5),
('GH02', 'SP19', 4),
('GH02', 'SP26', 2);

--Thêm đơn hàng vào 
INSERT INTO DONHANG
(MADH, MAKH, MANV_DUYET, MANV_GIAO, NGAYLAP, TONGTIEN, TRANGTHAI, PHIVANCHUYEN, DIACHIGIAO)
VALUES
-- ===== CHỜ DUYỆT (GETDATE) =====
('DH106','KH006',NULL,NULL,GETDATE(),0,N'CHODUYET',25000,N'10 Lý Thánh Tôn'),
('DH107','KH007',NULL,NULL,GETDATE(),0,N'CHODUYET',30000,N'22 Phan Chu Trinh'),
('DH108','KH008',NULL,NULL,GETDATE(),0,N'CHODUYET',20000,N'5 Nguyễn Huệ'),
('DH121','KH001',NULL,NULL,GETDATE(),0,N'CHODUYET',20000,N'19 Lê Thành Phương'),
('DH125','KH005',NULL,NULL,GETDATE(),0,N'CHODUYET',25000,N'99 Lê Lợi'),
('DH129','KH009',NULL,NULL,GETDATE(),0,N'CHODUYET',30000,N'74 Lê Văn Tám'),

-- ===== ĐANG GIAO (GETDATE) =====
('DH109','KH009','NV001','NV004',GETDATE(),0,N'DANGGIAO',30000,N'18 Trường Chinh'),
('DH110','KH010','NV002','NV005',GETDATE(),0,N'DANGGIAO',25000,N'9 Pasteur'),
('DH111','KH001','NV003','NV006',GETDATE(),0,N'DANGGIAO',20000,N'45 Hùng Vương'),
('DH112','KH002','NV001','NV004',GETDATE(),0,N'DANGGIAO',30000,N'77 Quang Trung'),
('DH122','KH002','NV001','NV004',GETDATE(),0,N'DANGGIAO',25000,N'81 Võ Thị Sáu'),
('DH126','KH006','NV001','NV006',GETDATE(),0,N'DANGGIAO',30000,N'41 Nguyễn Thị Minh Khai'),

-- ===== HOÀN THÀNH (2024–2026) =====
('DH113','KH003','NV002','NV005','2024-08-30',0,N'HOANTHANH',25000,N'12 Trần Nhật Duật'),

('DH114','KH004','NV003','NV006','2025-01-12',0,N'HOANTHANH',30000,N'90 Lê Duẩn'),
('DH115','KH005','NV001','NV004','2025-02-08',0,N'HOANTHANH',20000,N'33 Hai Bà Trưng'),
('DH116','KH006','NV002','NV005','2025-03-19',0,N'HOANTHANH',25000,N'66 Nguyễn Trãi'),
('DH117','KH007','NV003','NV006','2025-04-25',0,N'HOANTHANH',30000,N'14 Trần Phú'),

('DH123','KH003','NV002','NV005','2026-02-18',0,N'HOANTHANH',30000,N'62 Trần Quý Cáp'),
('DH127','KH007','NV002','NV004','2026-06-11',0,N'HOANTHANH',20000,N'58 Hoàng Hoa Thám'),
('DH130','KH010','NV001','NV005','2026-09-12',0,N'HOANTHANH',25000,N'29 Nguyễn Công Trứ'),

-- ===== ĐÃ HỦY (2024–2026) =====
('DH118','KH008','NV001',NULL,'2025-05-17',0,N'DAHUY',20000,N'88 Yersin'),
('DH119','KH009','NV002',NULL,'2025-06-09',0,N'DAHUY',25000,N'51 Hoàng Diệu'),
('DH120','KH010','NV003',NULL,'2025-07-21',0,N'DAHUY',30000,N'27 Nguyễn Khuyến'),

('DH124','KH004','NV003',NULL,'2026-03-07',0,N'DAHUY',20000,N'7 Phạm Văn Đồng'),
('DH128','KH008','NV003',NULL,'2026-07-05',0,N'DAHUY',25000,N'16 Hà Huy Tập');


INSERT INTO CTDH (MADH, MASP, SOLUONG, DONGIA) VALUES
-- ===== CHỜ DUYỆT =====
('DH106','SP01',30,6000),
('DH107','SP05',40,12500),
('DH108','SP11',2,15000),
('DH121','SP02',10,46000),
('DH125','SP27',3,13000),
('DH129','SP03',20,28000),

-- ===== ĐANG GIAO =====
('DH109','SP12',1,180000),
('DH110','SP14',2,110000),
('DH111','SP16',3,46000),
('DH112','SP17',10,9000),
('DH122','SP07',2,81000),
('DH126','SP28',2,30000),

-- ===== HOÀN THÀNH =====
('DH113','SP21',1,236000),
('DH114','SP25',1,166000),
('DH115','SP06',2,75000),
('DH116','SP09',4,15000),
('DH117','SP10',1,75000),
('DH123','SP08',5,8000),
('DH127','SP29',1,25000),
('DH130','SP04',20,19000),

-- ===== ĐÃ HỦY =====
('DH118','SP18',5,11000),
('DH119','SP19',3,11000),
('DH120','SP26',2,15000),
('DH124','SP20',2,37000),
('DH128','SP30',1,32000);
-- ================== CẬP NHẬT TỔNG TIỀN TẤT CẢ ĐƠN ==================
UPDATE DONHANG
SET TONGTIEN = ISNULL((
    SELECT SUM(CT.SOLUONG * CT.DONGIA)
    FROM CTDH CT
    WHERE CT.MADH = DONHANG.MADH
), 0) + PHIVANCHUYEN;
