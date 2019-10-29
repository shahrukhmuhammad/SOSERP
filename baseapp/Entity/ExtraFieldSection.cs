using System;
using System.Collections.Generic;

namespace BaseApp.Entity
{
    public class ExtraFieldSection
    {
        public Guid Id { get; set; }
        public string Module { get; set; }
        public Guid? ParentId { get; set; }
        public string Section { get; set; }
        public string FieldType { get; set; }
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public bool IsSystem { get; set; }

        public virtual List<ExtraFieldSection> Fields { get; set; }
    }
}
