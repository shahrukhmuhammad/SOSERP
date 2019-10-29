using System;
using System.Collections.Generic;

namespace CRM.Entity
{
    #region Contact
    public class Contact
    {
        public Guid Id { get; set; }
        public ContactTargetedFor TargetedFor { get; set; }
        public ContactType ContactType { get; set; }
        public ContactStatus Status { get; set; }
        public ContactSource Source { get; set; }
        public string OtherSource { get; set; }
        public string Code { get; set; }
        public string Username { get; set; }
        public ContactEmailType PrimaryEmailType { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public ContactPhoneType PrimaryPhoneType { get; set; }
        public string Phone { get; set; }


        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName { get { return string.Format("{0} {1} {2}", FirstName, MiddleName, LastName); } }

        
        public string Fax { get; set; }
        public string Website { get; set; }
        public string SkypeID { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Description { get; set; }

        public ContactGender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }


        public bool SubscribedForNewsletter { get; set; }
        public bool SubscribedForUpdates { get; set; }


        public DateTime UpdatedOn { get; set; }
        public Guid UpdatedByUserId { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedByUserId { get; set; }
        public TitlePrefix TitlePrefix { get; set; }
        public TitleSuffix TitleSuffix { get; set; }
        public List<string> ContactTypes { get; set; }

        public virtual List<ContactEmail> MoreEmails { get; set; }
        public virtual List<ContactPhone> MorePhones { get; set; }
        public virtual List<ContactAddress> MoreAddresses { get; set; }
        public virtual ContactCompany Company { get; set; }

        public virtual List<CrmContactType> SelectedContactTypes { get; set; }
    }

    public enum ContactStatus
    {
        //NoPurchase = 1,
        //InProgress = 2,
        //Purchase = 3,
        //Registered = 4,
        //NoPurchaseHistory = 5,
        
        Invited = 1,
        Inactive = 2,
        Active = 3,
        SignedUp = 4,
        PendingApproval = 5,
        Suspended = 6
    }

    public enum ContactGender
    {
        Male = 1,
        Female = 2,
        Other = 3
    }
    public enum ContactType
    {
        Lead = 1,
        Pipeline = 2,
        Customer = 3,
        Contact = 4,
        Trader = 5,
        DropShipper = 6,
        TraderDropShipper = 7,
        Supplier = 8,
        General = 9,
    }
    public enum ContactSource
    {
        Website = 1,
        Campaign = 2,
        Referred = 3,
        Exhibition = 4,
        SearchEngine = 5,
        PersonalContact = 6,
        Advertisement = 7,
        ColdCall = 8,
        Partner = 9,
        PublicRelations = 10,
        Other = 11
    }
    public enum ContactTargetedFor
    {
        TenderPurchase = 1,
        TenderListing = 2
    }

    public enum TitlePrefix
    {
        Mr = 2, Mrs = 1, Miss = 3, Ms = 4 
    }

    public enum TitleSuffix
    {
        Jr = 1, Sr = 2
    }
    #endregion

    #region Contact Email
    public class ContactEmail
    {
        public Guid ContactId { get; set; }
        public ContactEmailType Type { get; set; }
        public string Email { get; set; }
        public bool IsPrimary { get; set; }

        public virtual Contact Contact { get; set; }
    }
    public enum ContactEmailType
    {
        Home = 1,
        Work = 2,
        Other = 3
    }
    #endregion

    #region Contact Phones
    public class ContactPhone
    {
        public Guid ContactId { get; set; }
        public ContactPhoneType Type { get; set; }
        public string Phone { get; set; }
        public bool IsPrimary { get; set; }

        public virtual Contact Contact { get; set; }
    }
    public enum ContactPhoneType
    {
        Home = 1,
        Work = 2,
        Other = 3
    }
    #endregion

    #region Contact Addresses
    public class ContactAddress
    {
        public Guid ContactId { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public bool IsPrimary { get; set; }

        public virtual Contact Contact { get; set; }
    }
    #endregion
    #region Contact Company
    public class ContactCompany
    {
        public Guid Id { get; set; }
        public Guid ContactId { get; set; }
        public ContactCompanyType Type { get; set; }
        public string Title { get; set; }
        public string Industry { get; set; }
        public string NoOfEmployees { get; set; }
        public double AnnualRevenue { get; set; }
        public string ResponsiblePerson { get; set; }
        public string Currency { get; set; }
        public string Fax { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string NINumber { get; set; }
        public decimal CompanyPercentage { get; set; }

        public virtual Contact Contact { get; set; }
    }
    public enum ContactCompanyType
    {
        Private = 1,
        Public = 2,
        Government = 3,
        Shop = 4,
        MarketTrader = 5,
        OnlineStore = 6,
        Other = 7
    }
    public class CrmContactType
    {
        public Guid Id { get; set; }
        public ContactType ContactType { get; set; }
        public Guid UserId { get; set; }
    }
    #endregion
}
