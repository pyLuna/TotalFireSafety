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
    
    public partial class Employee
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Employee()
        {
            this.Requests = new HashSet<Request>();
        }
    
        public int emp_no { get; set; }
        public string emp_name { get; set; }
        public Nullable<System.DateTime> emp_hiredDate { get; set; }
        public long emp_contact { get; set; }
        public string emp_position { get; set; }
    
        public virtual Credential Credential { get; set; }
        public virtual Role Role { get; set; }
        public virtual Status Status { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Request> Requests { get; set; }

        public string formType { get; set; }

        public string FormattedDate
        {
            get { return emp_hiredDate.HasValue ? emp_hiredDate.Value.ToString("MMMM dd, yyyy") : ""; }
        }
    }
}
