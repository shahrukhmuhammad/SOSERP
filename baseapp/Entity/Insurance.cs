using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApp.Entity
{
    public class Insurance
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public InsuranceType InsuranceType { get; set; }
        public string InsurancePlan { get; set; }
        public string PolicyNumber { get; set; }
        public string GroupInsuranceNumber { get; set; }
        public int Coverage { get; set; }
        public int Premium { get; set; }
        public decimal PercentageByEmployee { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool FullTime { get; set; }
        public bool PartTime { get; set; }
        public bool VaryingWeekly { get; set; }
        public bool Contractor { get; set; }
        public bool PerDm { get; set; }
        public bool Freelance { get; set; }
    }

    public enum InsuranceType
    {
        WorkersCompensation = 1, Disability = 2, HealthInsurance = 3, LifeInsurance = 4, Other = 5
    }
}
