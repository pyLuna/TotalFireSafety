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
    
    public partial class Manpower
    {
        public System.Guid man_id { get; set; }
        public int man_emp_no { get; set; }
        public string man_name { get; set; }
        public string man_postition { get; set; }
    
        public virtual Employee Employee { get; set; }
    }
}
