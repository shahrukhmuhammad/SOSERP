using BaseApp.Entity;
using System;
using System.Collections.Generic;

namespace Timesheet.Entity
{
    public class Timesheet
    {
        public Guid Id { get; set; }
        public Guid ReferenceId { get; set; }
        public DateTime Dated { get; set; }
        public TimeSpan ClockIn { get; set; }
        public TimeSpan ClockOut { get; set; }
        public string Task { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedByUserId { get; set; }

        public virtual AppUser AppUser { get; set; }
        public virtual AppUser CreatedByUser { get; set; }
    }
}
