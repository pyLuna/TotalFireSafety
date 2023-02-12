namespace TotalFireSafety.Models
{
    public class NewEmployeeModel
    {
        public int emp_no { get; set; }
        public string emp_name { get; set; }
        public string emp_hiredDate { get; set; }
        public long emp_contact { get; set; }
        public string emp_position { get; set; }

        public virtual Credential Credential { get; set; }
        public virtual Role Role { get; set; }
        public virtual Status Status { get; set; }

    }


}