using System.Collections.Generic;
using TotalFireSafety.Models;

namespace TotalFireSafety.Controllers
{
    internal class ProjectReportsViewModel
    {
        public NewProject Projects { get; set; }
        public List<NewReport> Reports { get; set; }
    }
}