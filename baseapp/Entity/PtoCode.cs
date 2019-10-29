using System;
using System.Collections.Generic;

namespace BaseApp.Entity
{
    public class PtoCode
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public bool IsAssignedToAll { get; set; }

        public virtual List<PtoCodeAssignee> Assignees { get; set; }
    }
    public class PtoCodeAssignee
    {
        public Guid Id { get; set; }
        public Guid PtoId { get; set; }
        public Guid AppUserId { get; set; }

        public virtual PtoCode PtoCode { get; set; }
        public virtual AppUser AppUser { get; set; }
    }
}
