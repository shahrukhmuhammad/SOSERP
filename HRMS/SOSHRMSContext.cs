using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace HRMS
{
    public partial class SOSHRMSContext : DbContext
    {
        public SOSHRMSContext() : base("name=HRMS_Connection")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Configure Student &StudentAddress entity
            modelBuilder.Entity<Employee>()
                        .HasOptional(s => s.Emergency) // Mark Address property optional in Student entity
                        .WithRequired(ad => ad.Employee); // mark Student property as required in StudentAddress entity. Cannot save StudentAddress without Student
        }

        public DbSet<Employee> Employee { get; set; }
    }
}
