using System;
using System.Collections.Generic;

namespace BaseApp.Entity
{
    public class Office
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public Guid ContactId { get; set; }
        public string ContactName { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }

        public string Currency { get; set; }

        public int CountEmployees { get; set; }
        public int CountSubOffices { get; set; }

        #region Time Zone
        public string TimeZone { get; set; }
        public string TimeZoneTitle { get; set; }
        public string FullTimeZone { get { return string.Format("{0}_{1}", TimeZone, TimeZoneTitle); } }
        #endregion


        public DateTime UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid UpdatedByUserId { get; set; }
        public Guid CreatedByUserId { get; set; }
        public bool IsMainOffice { get; set; }
        public string PhoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public OfficeType OfficeType { get; set; }

        public virtual AppUser Contact { get; set; }
        public virtual Office Parent { get; set; }
        public virtual List<Office> Children { get; set; }
    }

    public enum OfficeType
    {
        Branch = 1, Department = 2
    }
}
