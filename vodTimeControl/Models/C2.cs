//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace vodTimeControl.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class C2
    {
        public int subProjectID { get; set; }
        public int projectID { get; set; }
        public string subProjectName { get; set; }
    
        public virtual project project { get; set; }
    }
}
