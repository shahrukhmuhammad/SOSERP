using BaseApp.Entity;
using Insight.Database;
using System;
using System.Collections.Generic;

namespace BaseApp.Logic
{
    public interface IAppRole
    {
        [Sql("AppRoles_GetAll")]
        List<AppRole> GetAll();

        [Sql("AppRoles_GetById")]
        AppRole GetById(Guid Id);

        [Sql("AppRoles_GetMaxCode")]
        string GetMaxCode();

        [Sql("AppRoles_Create")]
        Guid Create(AppRole model);

        [Sql("AppRoles_Update")]
        void Update(AppRole model);

        [Sql("AppRoles_Delete")]
        void Delete(Guid Id);

        [Sql("AppRoles_GetPermissionsById")]
        string GetPermissionsById(Guid Id);

        #region AppUser Permissions

        [Sql("AppUserPermissions_Create")]
        Guid Create(AppUserPermissions model);

        [Sql("AppUserPermissions_IsAlreadyExist")]
        int IsAlreadyExist(Guid UserId);

        [Sql("AppUserPermissions_Delete")]
        void DeleteByUser(Guid UserId);

        [Sql("AppUserPermissions_GetByUser")]
        string GetCustomPermissions(Guid UserId);
        #endregion
    }
}
