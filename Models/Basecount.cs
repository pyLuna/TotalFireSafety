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
    
    public partial class Basecount
    {
        public System.Guid bc_id { get; set; }
        public string bc_itemCode { get; set; }
        public System.DateTime bc_date { get; set; }
        public string bc_count { get; set; }
    
        public virtual Inventory Inventory { get; set; }
    }
}