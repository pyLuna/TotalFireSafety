﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TotalFireSafety.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class nwTFSEntity : DbContext
    {
        public nwTFSEntity()
            : base("name=nwTFSEntity")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Attendance> Attendances { get; set; }
        public virtual DbSet<Basecount> Basecounts { get; set; }
        public virtual DbSet<Credential> Credentials { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Inv_Update> Inv_Update { get; set; }
        public virtual DbSet<Inventory> Inventories { get; set; }
        public virtual DbSet<NewManpower> NewManpowers { get; set; }
        public virtual DbSet<NewProject> NewProjects { get; set; }
        public virtual DbSet<NewReport> NewReports { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<ReportImage> ReportImages { get; set; }
        public virtual DbSet<Request> Requests { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Status> Status { get; set; }
        public virtual DbSet<NewProposal> NewProposals { get; set; }
        public virtual DbSet<Proposal> Proposals { get; set; }
    }
}
