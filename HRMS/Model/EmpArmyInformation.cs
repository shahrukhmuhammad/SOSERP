//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class EmpArmyInformation
    {
        public System.Guid ArmyInformationId { get; set; }
        public System.Guid EmployeeId { get; set; }
        public Nullable<System.DateTime> JoiningDate { get; set; }
        public string UnitName { get; set; }
        public string ForceNo { get; set; }
        public string LastUnit { get; set; }
        public string LastCenter { get; set; }
        public Nullable<int> ServicePeriod { get; set; }
        public string RecordCenter { get; set; }
        public Nullable<System.DateTime> DischargeDate { get; set; }
        public string Rank { get; set; }
        public string Trade { get; set; }
        public string ForceType { get; set; }
        public string PreviousForces { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.Guid CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<System.Guid> UpdatedBy { get; set; }
    
        public virtual Employee Employee { get; set; }
    }
}
