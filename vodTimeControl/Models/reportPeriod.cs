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
    
    public partial class reportPeriod
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public reportPeriod()
        {
            this.timeTrackRow = new HashSet<timeTrackRow>();
        }
    
        public int reportPeriodId { get; set; }
        public System.DateTime fromDate { get; set; }
        public System.DateTime toDate { get; set; }
        public int regUserId { get; set; }
        public System.DateTime regDate { get; set; }
    
        public virtual userTbl userTbl { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<timeTrackRow> timeTrackRow { get; set; }
    }
}
