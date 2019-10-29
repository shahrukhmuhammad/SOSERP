using BaseApp.Entity;
using Insight.Database;
using System;
using System.Collections.Generic;

namespace BaseApp.Logic
{
    public interface IAppUser
    {
        [Sql("AppUsers_GetById")]
        AppUser GetUserById(Guid id);

        [Sql("AppUsers_ForceGetAll")]
        List<AppUser> ForceGetAll();

        [Sql("AppUsers_GetAll")]
        List<AppUser> GetAll();

        [Sql("AppUsers_GetAllByRoleId")]
        List<AppUser> GetAllByRoleId(Guid RoleId);

        [Sql("AppUsers_GetAllByOfficeId")]
        List<AppUser> GetAllByOfficeId(Guid OfficeId);

        [Sql("AppUsers_GetByPermission")]
        List<AppUser> GetByPermission(string Permission);

        [Sql("AppUsers_Login")]
        AppUser GetUserByLogin(string username, string password);

        [Sql("AppUsers_LoginEmployee")]
        AppUser GetEmployeeByLogin(string username, string password);

        [Sql("AppUsers_CheckDuplicateUsername")]
        AppUser CheckDuplicateUsername(Guid Id, string Username);

        [Sql("AppUsers_ChangePassword")]
        bool ChangePassword(Guid Id, string OldPassword, string NewPassword);

        [Sql("AppUsers_PasswordRecovery")]
        AppUser PasswordRecovery(string email, string Username);

        [Sql("AppUsers_UsernameRecoveryByEmail")]
        AppUser UsernameRecoveryByEmail(string Email, string FirstName, string MiddleName, string LastName);

        [Sql("AppUsers_UsernameRecoveryByQA")]
        AppUser UsernameRecoveryByQA(string Question, string Answer, string FirstName, string MiddleName, string LastName);

        [Sql("AppUsers_GetMaxCode")]
        string GetMaxCode();

        [Sql("AppUsers_Create")]
        Guid Create(AppUser model);

        [Sql("AppUsers_Update")]
        void Update(AppUser model);

        [Sql("AppUsers_UpdateQA")]
        void UpdateQA(Guid Id, string Question1, string Answer1, string Question2, string Answer2);

        [Sql("AppUsers_ChangeStatus")]
        void ChangeStatus(Guid Id);

        [Sql("AppUsers_ChangeTheme")]
        void ChangeTheme(Guid Id, string Theme);

        [Sql("AppUsers_Delete")]
        void Delete(Guid Id);

        [Sql("AppUsers_SelectAllByType")]
        IList<AppUser> SelectAllByType(AppUserEmploymentStatus EmployeeType);

        [Sql("AppUsers_UpdatePageLength")]
        void UpdatePageLength(Guid Id, string PageLength);

        #region Extras
        [Sql("AppUserExtras_GetByAppUserId")]
        List<AppUserExtra> GetExtrasByAppUserId(Guid AppUserId);

        [Sql("AppUserExtras_Create")]
        void CreateExtraField(AppUserExtra model);

        [Sql("AppUserExtras_DeleteByAppUserId")]
        void DeleteExtraByAppUserId(Guid AppUserId);

        [Sql("AppUserExtras_DeleteByExtraId")]
        void DeleteExtraByExtraId(Guid ExtraId);
        #endregion

        #region Employment
        [Sql("AppUserEmployment_GetByAppUserId")]
        AppUserEmployment GetEmploymentByAppUserId(Guid AppUserId);

        [Sql("AppUserEmployment_Create")]
        void CreateEmployment(AppUserEmployment model);

        [Sql("AppUserEmployment_Update")]
        void UpdateEmployment(AppUserEmployment model);

        [Sql("AppUserEmployment_UpdateAvailability")]
        void UpdateEmploymentAvailability(AppUserEmployment model);

        [Sql("AppUserEmployment_Delete")]
        void DeleteEmployment(Guid AppUserId);
        #endregion
    }
}
