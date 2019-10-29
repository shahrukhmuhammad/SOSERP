using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApp.Entity
{
    public class BenefitType
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public double Amount { get; set; }
        public FrequencyType Frequency { get; set; }
        public bool FullTime { get; set; }
        public bool PartTime { get; set; }
        public bool VaryingWeekly { get; set; }
        public bool Contractor { get; set; }
        //public bool PerDm { get; set; }
        public bool Freelance { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

    }

    public enum FrequencyType
    {
        Yearly = 1, SemiYearly = 2, Quarter = 3, Monthly = 4, BiWeekly = 5
    }

    public class BonusType
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public FrequencyType Frequency { get; set; }
        public decimal BonusPercentage { get; set; }
        public bool FullTime { get; set; }
        public bool PartTime { get; set; }
        public bool VaryingWeekly { get; set; }
        public bool Contractor { get; set; }
        //public bool PerDm { get; set; }
        public bool Freelance { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
