using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TotalFireSafety.Models
{
    using System.Collections.Generic;
    public class ProjectReportViewModel
    {
        public NewProject Project { get; set; }

        public NewReport SelectedReport { get; set; }

        public NewManpower SelectedManpower { get; set; }

        public NewProposal NewProposal { get; set; }

        public List<NewReport> Reports { get; set; }

        public List<NewManpower> Manpowers { get; set; }
        public List<Attendance> Attendances { get; set; }

        public List<ReportImage> ReportImage { get; set; }


    }
}