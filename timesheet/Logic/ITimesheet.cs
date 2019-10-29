using Insight.Database;
using System;
using System.Collections.Generic;

namespace Timesheet.Logic
{
    public interface ITimesheet
    {
        [Sql("Timesheets_GetByReferenceId")]
        List<Entity.Timesheet> GetByReferenceId(Guid AppUserId);

        [Sql("Timesheets_GetById")]
        Entity.Timesheet GetById(Guid Id);

        [Sql("Timesheets_Create")]
        Guid Create(Entity.Timesheet model);

        [Sql("Timesheets_Update")]
        void Update(Entity.Timesheet model);

        [Sql("Timesheets_DeleteByReferenceId")]
        void DeleteByReferenceId(Guid ReferenceId);

        [Sql("Timesheets_Delete")]
        void Delete(Guid Id);
    }
}
