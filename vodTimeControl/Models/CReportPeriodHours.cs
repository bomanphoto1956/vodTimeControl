using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vodTimeControl.Models
{
    public class CReportPeriodHours
    {

        public string customerName { get; set; }
        public string projectName { get; set; }
        public string subProjectName { get; set; }
        public string tDate { get; set; }
        public Decimal hours { get; set; }
        public string note { get; set; }
        
    }
}