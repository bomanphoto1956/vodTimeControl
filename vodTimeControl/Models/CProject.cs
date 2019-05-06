using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace vodTimeControl.Models
{
    public class CProject
    {
        public int projectID { get; set; }
        [Required(ErrorMessage = "Välj kund")]
        public int customerID { get; set; }
        [Required(ErrorMessage = "Mata in projektnamn")]
        public string projectName { get; set; }        
        public bool active { get; set; }

    }
}