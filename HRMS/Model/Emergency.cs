using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS
{
    public class Emergency
    {
        //[Key]
        public Guid EmergencyId { get; set; }
        public string EmergencyAddress { get; set; }
        public string EmergencyPhone { get; set; }
        public string EmergencyCell { get; set; }
        //[ForeignKey("Employee")]
        public Guid EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
