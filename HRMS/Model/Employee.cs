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
    
    public partial class Employee
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Employee()
        {
            this.EmpArmyInformations = new HashSet<EmpArmyInformation>();
            this.EmpBankDetails = new HashSet<EmpBankDetail>();
            this.EmpEmergencyContacts = new HashSet<EmpEmergencyContact>();
            this.EmpFingerPrints = new HashSet<EmpFingerPrint>();
            this.EmpPoliticalInformations = new HashSet<EmpPoliticalInformation>();
            this.EmpReferences = new HashSet<EmpReference>();
            this.EmpRejoinHistories = new HashSet<EmpRejoinHistory>();
            this.EmpSalaryDetails = new HashSet<EmpSalaryDetail>();
            this.EmpTransferHistories = new HashSet<EmpTransferHistory>();
        }
    
        public System.Guid EmployeeId { get; set; }
        public System.Guid ProjectId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string CNIC { get; set; }
        public string FatherName { get; set; }
        public string FatherCNIC { get; set; }
        public string PermanentAddress { get; set; }
        public string CurrentAddress { get; set; }
        public string ProfilePicture { get; set; }
        public string Phone { get; set; }
        public string Region { get; set; }
        public string Center { get; set; }
        public string Supervisor { get; set; }
        public string Post { get; set; }
        public string MotherName { get; set; }
        public string Education { get; set; }
        public string BloodGroup { get; set; }
        public string IdentificationMarks { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string Gender { get; set; }
        public string MaritalStatus { get; set; }
        public string NextToKIN { get; set; }
        public string WifeName { get; set; }
        public Nullable<int> NoOfChilds { get; set; }
        public Nullable<int> NoOfBoys { get; set; }
        public Nullable<int> NoOfGirls { get; set; }
        public Nullable<byte> Status { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.Guid CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<System.Guid> UpdatedBy { get; set; }
        public Nullable<System.Guid> RegionId { get; set; }
        public Nullable<System.Guid> CenterId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmpArmyInformation> EmpArmyInformations { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmpBankDetail> EmpBankDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmpEmergencyContact> EmpEmergencyContacts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmpFingerPrint> EmpFingerPrints { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmpPoliticalInformation> EmpPoliticalInformations { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmpReference> EmpReferences { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmpRejoinHistory> EmpRejoinHistories { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmpSalaryDetail> EmpSalaryDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmpTransferHistory> EmpTransferHistories { get; set; }
    }
}
