//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Do_An.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public order()
        {
            this.order_details = new HashSet<order_details>();
            this.gifts = new HashSet<gift>();
        }
    
        public int id { get; set; }
        public Nullable<int> khach_hang_id { get; set; }
        public Nullable<System.DateTime> ngay_dat_hang { get; set; }
        public Nullable<System.DateTime> ngay_giao_hang { get; set; }
        public Nullable<decimal> tong_tien { get; set; }
        public string trang_thai { get; set; }
        public string hinh_thuc_giao_hang { get; set; }
        public Nullable<int> promotion_id { get; set; }
        public string phuong_thuc_thanh_toan1 { get; set; }
        public string dia_chi_nhan_hang { get; set; }
        public string ho_ten { get; set; }
        public string email { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<order_details> order_details { get; set; }
        public virtual user user { get; set; }
        public virtual promotion promotion { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<gift> gifts { get; set; }
    }
}
