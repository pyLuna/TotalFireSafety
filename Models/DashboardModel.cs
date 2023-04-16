using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TotalFireSafety.Models
{
    public class DashboardModel
    {
        public int users { get; set; }
        public int active_users { get; set; }
        public int purchase { get; set; }
        public int rec_purchase { get; set; }
        public int deployment { get; set; }
        public int rec_deployment { get; set; }
        public int supply { get; set; }
        public int rec_supply { get; set; }
    }
}