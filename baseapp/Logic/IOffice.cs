using BaseApp.Entity;
using Insight.Database;
using System;
using System.Collections.Generic;

namespace BaseApp.Logic
{
    public interface IOffice
    {
        #region Office
        [Sql("Offices_GetById")]
        Office GetById(Guid Id);

        [Sql("Offices_GetByParentId")]
        List<Office> GetByParentId(Guid ParentId);

        [Sql("Offices_GetAll")]
        List<Office> GetAll();

        [Sql("Offices_GetAllByMainOffice")]
        IList<Office> GetAllByMainOffice();

        [Sql("Offices_GetBreadCrumb")]
        List<Office> GetBreadCrumb();

        [Sql("Offices_GetMaxCode")]
        string GetMaxCode();

        [Sql("Offices_Create")]
        Guid Create(Office model);

        [Sql("Offices_Update")]
        void Update(Office model);

        [Sql("Offices_Delete")]
        void Delete(Guid Id);

        [Sql("Offices_UpdateParentByOfficeId")]
        void UpdateParentByOfficeId(Guid Id, Guid ParentId);
        #endregion
    }
}
