USE [nhom1ltweb]
GO
/****** Object:  User [Nhom1LTWeb_SQLLogin_1]    Script Date: 8/9/2024 1:54:06 AM ******/
CREATE USER [Nhom1LTWeb_SQLLogin_1] FOR LOGIN [Nhom1LTWeb_SQLLogin_1] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [Nhom1LTWeb_SQLLogin_1]
GO
/****** Object:  Schema [Nhom1LTWeb_SQLLogin_1]    Script Date: 8/9/2024 1:54:07 AM ******/
CREATE SCHEMA [Nhom1LTWeb_SQLLogin_1]
GO
/****** Object:  Table [dbo].[products]    Script Date: 8/9/2024 1:54:07 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[products](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ten] [nvarchar](255) NOT NULL,
	[mo_ta] [nvarchar](max) NULL,
	[gia] [decimal](10, 2) NOT NULL,
	[hinh_anh] [nvarchar](255) NULL,
	[so_luong_ton_kho] [int] NULL,
	[ngay_nhap_kho] [date] NULL,
	[loai_hoa_id] [int] NULL,
	[dip_phu_hop_id] [int] NULL,
	[kich_thuoc] [nvarchar](50) NULL,
	[id_gg] [nvarchar](50) NULL,
	[mau_sac] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[order_details]    Script Date: 8/9/2024 1:54:08 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[order_details](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[don_hang_id] [int] NULL,
	[san_pham_id] [int] NULL,
	[so_luong] [int] NULL,
	[don_gia] [decimal](10, 2) NULL,
	[giam_gia] [decimal](18, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[ProductStatistics]    Script Date: 8/9/2024 1:54:08 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[ProductStatistics] AS
SELECT 
    p.id AS ProductID,
    p.ten AS ProductName,
    SUM(od.so_luong) AS TotalQuantitySold,
    SUM(od.so_luong * (od.don_gia - od.giam_gia)) AS TotalRevenue,
    p.so_luong_ton_kho AS QuantityInStock
FROM 
    products p
JOIN 
    order_details od ON p.id = od.san_pham_id
GROUP BY 
    p.id, p.ten, p.so_luong_ton_kho;
GO
/****** Object:  Table [dbo].[cart]    Script Date: 8/9/2024 1:54:08 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[cart](
	[id_user] [int] NOT NULL,
	[id_product] [int] NOT NULL,
	[soluong_sp] [int] NULL,
 CONSTRAINT [pk_cart] PRIMARY KEY CLUSTERED 
(
	[id_user] ASC,
	[id_product] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[flower_types]    Script Date: 8/9/2024 1:54:08 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[flower_types](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ten] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[gifts]    Script Date: 8/9/2024 1:54:08 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[gifts](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[order_id] [int] NULL,
	[nguoi_nhan_ten] [nvarchar](255) NULL,
	[nguoi_nhan_sdt] [nvarchar](20) NULL,
	[nguoi_nhan_dia_chi] [nvarchar](255) NULL,
	[loi_nhan] [text] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[occasions]    Script Date: 8/9/2024 1:54:08 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[occasions](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ten] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[orders]    Script Date: 8/9/2024 1:54:08 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[orders](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[khach_hang_id] [int] NULL,
	[ngay_dat_hang] [date] NULL,
	[ngay_giao_hang] [date] NULL,
	[tong_tien] [decimal](10, 2) NULL,
	[trang_thai] [nvarchar](50) NULL,
	[hinh_thuc_giao_hang] [nvarchar](50) NULL,
	[promotion_id] [int] NULL,
	[phuong_thuc_thanh_toan] [nvarchar](max) NULL,
	[dia_chi_nhan_hang] [nvarchar](max) NULL,
	[ho_ten] [nvarchar](max) NULL,
	[email] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[orders_payments]    Script Date: 8/9/2024 1:54:08 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[orders_payments](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NULL,
	[ten_phuong_thuc] [varchar](50) NULL,
	[mo_ta] [text] NULL,
	[stk] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[promotions]    Script Date: 8/9/2024 1:54:08 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[promotions](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ten] [nvarchar](255) NULL,
	[mo_ta] [nvarchar](max) NULL,
	[ngay_bat_dau] [date] NULL,
	[ngay_ket_thuc] [date] NULL,
	[code_promotions] [nvarchar](max) NULL,
	[value_promotions] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[suppliers]    Script Date: 8/9/2024 1:54:08 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[suppliers](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[san_pham_id] [int] NULL,
	[ten] [varchar](100) NULL,
	[dia_chi] [text] NULL,
	[so_dien_thoai] [varchar](20) NULL,
	[email] [varchar](100) NULL,
	[ghi_chu] [text] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[users]    Script Date: 8/9/2024 1:54:08 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[users](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ten] [nvarchar](255) NOT NULL,
	[dia_chi] [nvarchar](255) NULL,
	[so_dien_thoai] [nvarchar](20) NULL,
	[email] [nvarchar](255) NULL,
	[ghi_chu] [nvarchar](max) NULL,
	[ngay_sinh] [date] NULL,
	[id_google] [nvarchar](100) NULL,
	[id_facebook] [nvarchar](100) NULL,
	[vai_tro] [nvarchar](50) NULL,
	[password] [nvarchar](100) NOT NULL,
	[img_user] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[order_details] ADD  CONSTRAINT [DF_giam_gia]  DEFAULT ((0)) FOR [giam_gia]
GO
ALTER TABLE [dbo].[users] ADD  DEFAULT (CONVERT([varchar](32),hashbytes('MD5','passmau'),(2))) FOR [password]
GO
ALTER TABLE [dbo].[cart]  WITH CHECK ADD  CONSTRAINT [fk_cart_id_product] FOREIGN KEY([id_product])
REFERENCES [dbo].[products] ([id])
GO
ALTER TABLE [dbo].[cart] CHECK CONSTRAINT [fk_cart_id_product]
GO
ALTER TABLE [dbo].[cart]  WITH CHECK ADD  CONSTRAINT [fk_cart_id_user] FOREIGN KEY([id_user])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[cart] CHECK CONSTRAINT [fk_cart_id_user]
GO
ALTER TABLE [dbo].[gifts]  WITH CHECK ADD FOREIGN KEY([order_id])
REFERENCES [dbo].[orders] ([id])
GO
ALTER TABLE [dbo].[order_details]  WITH CHECK ADD FOREIGN KEY([don_hang_id])
REFERENCES [dbo].[orders] ([id])
GO
ALTER TABLE [dbo].[order_details]  WITH CHECK ADD FOREIGN KEY([san_pham_id])
REFERENCES [dbo].[products] ([id])
GO
ALTER TABLE [dbo].[orders]  WITH CHECK ADD FOREIGN KEY([khach_hang_id])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[orders]  WITH CHECK ADD FOREIGN KEY([promotion_id])
REFERENCES [dbo].[promotions] ([id])
GO
ALTER TABLE [dbo].[orders_payments]  WITH CHECK ADD FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([id])
GO
ALTER TABLE [dbo].[products]  WITH CHECK ADD FOREIGN KEY([dip_phu_hop_id])
REFERENCES [dbo].[occasions] ([id])
GO
ALTER TABLE [dbo].[products]  WITH CHECK ADD FOREIGN KEY([loai_hoa_id])
REFERENCES [dbo].[flower_types] ([id])
GO
ALTER TABLE [dbo].[suppliers]  WITH CHECK ADD FOREIGN KEY([san_pham_id])
REFERENCES [dbo].[products] ([id])
GO
ALTER TABLE [dbo].[users]  WITH CHECK ADD CHECK  (([vai_tro]='customer' OR [vai_tro]='admin'))
GO
