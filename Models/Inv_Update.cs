//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TotalFireSafety.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Inv_Update
    {
        public System.Guid update_id { get; set; }
        public string update_item_id { get; set; }
        public string update_quantity { get; set; }
        public Nullable<System.DateTime> update_date { get; set; }
        public string update_type { get; set; }
    
        public virtual Inventory Inventory { get; set; }
        public string FormattedDate
        {
            get { return update_date?.ToString("MMMM dd, yyyy"); }
        }
    }
}
