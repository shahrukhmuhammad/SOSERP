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
    
    public partial class EmpReference
    {
        public System.Guid ReferenceId { get; set; }
        public System.Guid EmployeeId { get; set; }
        public string RefName { get; set; }
        public string RefAddress { get; set; }
        public string RefAddressType { get; set; }
        public string RefPhoto { get; set; }
        public string RefState { get; set; }
        public string RefCountry { get; set; }
        public string RefCNIC { get; set; }
        public string RefProfession { get; set; }
        public string RefPhone { get; set; }
        public string RefMobile { get; set; }
        public string RefEmail { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.Guid CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<System.Guid> UpdatedBy { get; set; }
    
        public virtual Employee Employee { get; set; }
    }
}
