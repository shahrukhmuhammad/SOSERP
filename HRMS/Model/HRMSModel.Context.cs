﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HRMS.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class SOSHRMSEntities : DbContext
    {
        public SOSHRMSEntities()
            : base("name=SOSHRMSEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Center> Centers { get; set; }
        public virtual DbSet<EmpArmyInformation> EmpArmyInformations { get; set; }
        public virtual DbSet<EmpBankDetail> EmpBankDetails { get; set; }
        public virtual DbSet<EmpDocument> EmpDocuments { get; set; }
        public virtual DbSet<EmpEmergencyContact> EmpEmergencyContacts { get; set; }
        public virtual DbSet<EmpFingerPrint> EmpFingerPrints { get; set; }
        public virtual DbSet<EmpPoliticalInformation> EmpPoliticalInformations { get; set; }
        public virtual DbSet<EmpReference> EmpReferences { get; set; }
        public virtual DbSet<EmpRejoinHistory> EmpRejoinHistories { get; set; }
        public virtual DbSet<EmpSalaryDetail> EmpSalaryDetails { get; set; }
        public virtual DbSet<EmpTransferHistory> EmpTransferHistories { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Segment> Segments { get; set; }
        public virtual DbSet<Designation> Designations { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<HRM_Vew_Employee> HRM_Vew_Employee { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Region> Regions { get; set; }
        public virtual DbSet<City> Cities { get; set; }
    
        public virtual ObjectResult<string> SP_Employee_GetMaxCode()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("SP_Employee_GetMaxCode");
        }
    }
}
