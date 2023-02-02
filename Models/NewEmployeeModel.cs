using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace TotalFireSafety.Models
{
    public class NewEmployeeModel
    {
        public int emp_no { get; set; }
        public string emp_name { get; set; }
        public string emp_hiredDate { get; set; }
        public long emp_contact { get; set; }
        public string emp_position { get; set; }
        
        public string IsActive { get; set; }
        public string IsLocked { get; set; }


       
    }

  
}