﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class pdsTidRedLiveEntities : DbContext
    {
        public pdsTidRedLiveEntities()
            : base("name=pdsTidRedLiveEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C2> C2 { get; set; }
        public virtual DbSet<custBudget> custBudget { get; set; }
        public virtual DbSet<customer> customer { get; set; }
        public virtual DbSet<custType> custType { get; set; }
        public virtual DbSet<project> project { get; set; }
        public virtual DbSet<subProject2> subProject2 { get; set; }
        public virtual DbSet<templSubproj> templSubproj { get; set; }
        public virtual DbSet<timeTrackHead> timeTrackHead { get; set; }
        public virtual DbSet<userProject> userProject { get; set; }
        public virtual DbSet<userRole> userRole { get; set; }
        public virtual DbSet<userTbl> userTbl { get; set; }
        public virtual DbSet<reportPeriod> reportPeriod { get; set; }
        public virtual DbSet<timeTrackRow> timeTrackRow { get; set; }
    }
}
