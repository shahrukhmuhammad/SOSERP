using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApp.Entity
{
    public class TaxTypes
    {
        public Guid Id { get; set; }
        public string Taxname { get; set; }
        //public TaxType Taxtype { get; set; }
        public decimal PercentageDeduction { get; set; }
        public bool FullTime { get; set; }
        public bool PartTime { get; set; }
        public bool VaryingWeekly { get; set; }
        public bool Contractor { get; set; }
        public bool PerDm { get; set; }
        public bool Freelance { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }

    public enum TaxType
    {

    }
}
