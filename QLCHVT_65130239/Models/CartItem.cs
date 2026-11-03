using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLCHVT_65130239.Models
{
    public class CartItem
    {
        
            public string MASP { get; set; }
            public string TENSP { get; set; }
            public string HINHANH { get; set; }
            public decimal GIABAN { get; set; }
            public int SOLUONG { get; set; }
            public decimal THANHTIEN => SOLUONG * GIABAN;
        
    }
}