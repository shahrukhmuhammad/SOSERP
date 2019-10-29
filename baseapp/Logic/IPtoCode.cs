using System;
using System.Collections.Generic;
using Insight.Database;
using BaseApp.Entity;

namespace BaseApp.Logic
{
    public interface IPtoCode
    {
        #region PTO Code
        [Sql("PtoCodes_GetAll")]
        List<PtoCode> GetAll();

        [Sql("PtoCodes_GetById")]
        PtoCode GetById(Guid Id);

        [Sql("PtoCodes_Create")]
        Guid Create(PtoCode model);

        [Sql("PtoCodes_Update")]
        void Update(PtoCode model);

        [Sql("PtoCodes_UpdateAssignedToAll")]
        void UpdateAssignedToAll(Guid Id);

        [Sql("PtoCodes_Delete")]
        void Delete(Guid Id);
        #endregion

        #region PTO Code Assignee
        [Sql("PtoCodeAssignees_GetById")]
        PtoCodeAssignee GetAssigneeById(Guid Id);

        [Sql("PtoCodeAssignees_GetByAppUserId")]
        List<PtoCodeAssignee> GetAssigneesByAppUserId(Guid AppUserId);

        [Sql("PtoCodeAssignees_GetByPtoId")]
        List<PtoCodeAssignee> GetAssigneesByPtoId(Guid PtoId);

        [Sql("PtoCodeAssignees_Create")]
        Guid CreateAssignee(PtoCodeAssignee model);

        [Sql("PtoCodeAssignees_Update")]
        void UpdateAssignee(PtoCodeAssignee model);

        [Sql("PtoCodeAssignees_DeleteByAppUserId")]
        void DeleteAssigneesByAppUserId(Guid AppUserId);

        [Sql("PtoCodeAssignees_DeleteByPtoId")]
        void DeleteAssigneesByPtoId(Guid PtoId);

        [Sql("PtoCodeAssignees_Delete")]
        void DeleteAssignee(Guid Id);
        #endregion
    }
}
