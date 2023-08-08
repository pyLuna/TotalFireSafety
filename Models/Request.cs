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
    
    public partial class Request
    {
        public System.Guid request_id { get; set; }
        public string request_type { get; set; }
        public string request_item { get; set; }
        public string request_item_quantity { get; set; }
        public System.DateTime request_date { get; set; }
        public Nullable<int> request_employee_id { get; set; }
        public string request_status { get; set; }
        public int request_type_id { get; set; }
        public string request_type_status { get; set; }
        public Nullable<int> request_proj_id { get; set; }
    
        public virtual Employee Employee { get; set; }
        public virtual Inventory Inventory { get; set; }
        public string formType { get; set; }

        public string FormattedDate
        {
            get { return request_date.ToString("MMMM dd, yyyy"); }
        }

        public string Id
        {
            get
            {
                string newID = "";
                if (request_type.ToLower() == "purchase")
                {
                    newID = "PUR" + request_type_id;
                }
                if (request_type.ToLower() == "deploy" || request_type.ToLower() == "deployment")
                {
                    newID = "DEP" + request_type_id;
                }
                if (request_type.ToLower() == "supply")
                {
                    newID = "SUP" + request_type_id;
                }
                return newID;
            }
        }
    }
}