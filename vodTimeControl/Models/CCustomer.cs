using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace vodTimeControl.Models
{
    public class CCustomer
    {

        public int customerID { get; set; }
        [Required(ErrorMessage = "Välj kundtyp")]
        public int custTypeID { get; set; }
        [Required(ErrorMessage = "Ange kundnamn")]
        public string custName { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string zipCode { get; set; }
        public string email { get; set; }
        public string city { get; set; }
        public bool active { get; set; }

        public string custTypeName { get; set; }
        public bool toBeInvoiced { get; set; }
    }
}