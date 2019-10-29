using CRM.Entity;
using Insight.Database;
using System;
using System.Collections.Generic;

namespace CRM.Logic
{
    public interface IContact
    {
        #region Contact
        [Sql("CrmContacts_GetAll")]
        List<Contact> GetAll();

        [Sql("CrmContacts_GetById")]
        Contact GetById(Guid Id);

        [Sql("CrmContacts_GetMaxCode")]
        string GetMaxCode();

        [Sql("CrmContacts_Create")]
        Guid Create(Contact model);

        [Sql("CrmContacts_Update")]
        void Update(Contact model);

        [Sql("CrmContacts_Verify")]
        void VerifyContact(Guid Id);

        [Sql("CrmContacts_LoginByContactType")]
        Contact Login(string Email, string Password);

        [Sql("CrmContacts_ChangePassword")]
        bool ChangePassword(Guid Id, string OldPassword, string NewPassword);

        [Sql("CrmContacts_ChangeType")]
        void ChangeContactType(Guid Id, ContactType Type, Guid UpdatedByUserId);

        [Sql("CrmContacts_ChangeStatus")]
        void ChangeStatus(Guid Id, ContactStatus Status, Guid UpdatedByUserId);

        [Sql("CrmContacts_CheckDuplicateUsername")]
        Contact CheckDuplicateUsername(Guid Id, string Username);

        [Sql("CrmContacts_CheckDuplicateEmail")]
        Contact CheckDuplicateEmail(Guid Id, string Email);

        [Sql("CrmContacts_Delete")]
        void Delete(Guid Id);

        [Sql("CrmContacts_Login")]
        Contact GetUserByLogin(string email, string password);

        [Sql("CrmContactsAppUsers_GetAll")]
        List<Contact> GetAllUsers();
        #endregion

        #region Contact Emails
        [Sql("CrmContactEmails_GetAll")]
        List<ContactEmail> GetAllEmails(Guid ContactId);

        [Sql("CrmContactEmails_Create")]
        void CreateMoreEmail(ContactEmail model);

        [Sql("CrmContactEmails_Delete")]
        void DeleteMoreEmails(Guid ContactId);

        #endregion

        #region Contact Phones
        [Sql("CrmContactPhones_GetAll")]
        List<ContactPhone> GetAllPhones(Guid ContactId);

        [Sql("CrmContactPhones_Create")]
        void CreateMorePhone(ContactPhone model);

        [Sql("CrmContactPhones_Delete")]
        void DeleteMorePhones(Guid ContactId);
        #endregion

        #region Compnay

        [Sql("CrmContactCompanies_Create")]
        void CreateCompany(ContactCompany model);

        [Sql("CrmContactCompanies_Update")]
        void UpdateCompany(ContactCompany model);

        [Sql("CrmContactCompanies_GetByContactId")]
        ContactCompany GetByContactId(Guid ContactId);

        #endregion

        #region ContactTypes
        [Sql("CrmContactTypes_GetByContact")]
        List<CrmContactType> GetByContact(Guid UserId);

        [Sql("CrmContactTypes_Delete")]
        void DeleteTypes(Guid UserId);

        [Sql("CrmContactTypes_Create")]
        Guid CreateTypes(ContactType contactType, Guid UserId);
        #endregion

        #region More Addresses
        [Sql("CrmContactAddresses_GetAll")]
        List<ContactAddress> GetAllAddresses(Guid ContactId);

        [Sql("CrmContactAddresses_Create")]
        void CreateMoreAddress(ContactAddress model);

        [Sql("CrmContactAddresses_Delete")]
        void DeleteMoreAddress(Guid ContactId);

        #endregion
    }
}
