using System;
using System.Collections.Generic;

namespace BaseApp.Entity
{
    public class AppUser
    {
        public Guid Id { get; set; }
        public AppUserType Type { get; set; }
        public AppUserEmploymentStatus EmploymentStatus { get; set; }
        public Guid RoleId { get; set; }
        public Guid? OfficeId { get; set; }
        public string Code { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName { get { return string.Format("{0} {1} {2}", FirstName, MiddleName, LastName); } }
        public AppUserGender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string HomePhone { get; set; }
        public string WorkPhone { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }


        public string Username { get; set; }
        public string Password { get; set; }
        public AppUserStatus Status { get; set; }
        public DateTime LastLogin { get; set; }
        public string Theme { get; set; }
        public string PageLength { get; set; }

        public string Question1 { get; set; }
        public string Answer1 { get; set; }
        public string Question2 { get; set; }
        public string Answer2 { get; set; }


        public Guid UpdatedByUserId { get; set; }
        public Guid CreatedByUserId { get; set; }

        public DateTime UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }

        public virtual Office Office { get; set; }
        public virtual AppRole Role { get; set; }

        public virtual List<AppUserExtra> Extras { get; set; }
        public virtual AppUserEmployment Employment { get; set; }
    }

    public class AppUserPermissions
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public string Permissions { get; set; }

        public string[] PermissionsList
        {
            get
            {
                if (string.IsNullOrEmpty(Permissions))
                {
                    return null;
                }
                else
                {
                    return Permissions.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                }
            }
        }
    }

    public enum AppUserGender { Male = 1, Female = 2, Other = 3 }

    public enum AppUserStatus { Invited = 1, UnVerified = 2, Active = 3, Suspended = 4 }

    //public enum AppUserEmploymentStatus { FullTime = 1, PartTime = 2, Contract = 3, Temporary = 4, Internee = 5, Trainee = 6 }
    public enum AppUserEmploymentStatus
    {
        FullTime = 1, PartTime = 2, VaryingWeekly = 3, Contractor = 4, Freelance = 5
    }

    public enum AppUserType { Administrator = 1, Manager = 2, Employee = 3 }

    #region Extras
    public class AppUserExtra
    {
        public Guid AppUserId { get; set; }
        public Guid ExtraId { get; set; }
        public string Value { get; set; }

        public virtual ExtraFieldSection Extra { get; set; }
    }
    #endregion

    #region Employment
    public class AppUserEmployment
    {
        public Guid AppUserId { get; set; }
        public DateTime StartingDate { get; set; }
        public DateTime TermEnding { get; set; }
        public decimal PayRate { get; set; }
        public decimal OvertimeRate { get; set; }
        public AppUserSalaryCycle SalaryCycle { get; set; }

        #region Availability
        public bool Monday { get; set; }
        public TimeSpan MondayStart { get; set; }
        public TimeSpan MondayEnd { get; set; }

        public bool Tuesday { get; set; }
        public TimeSpan TuesdayStart { get; set; }
        public TimeSpan TuesdayEnd { get; set; }

        public bool Wednesday { get; set; }
        public TimeSpan WednesdayStart { get; set; }
        public TimeSpan WednesdayEnd { get; set; }

        public bool Thursday { get; set; }
        public TimeSpan ThursdayStart { get; set; }
        public TimeSpan ThursdayEnd { get; set; }

        public bool Friday { get; set; }
        public TimeSpan FridayStart { get; set; }
        public TimeSpan FridayEnd { get; set; }

        public bool Saturday { get; set; }
        public TimeSpan SaturdayStart { get; set; }
        public TimeSpan SaturdayEnd { get; set; }

        public bool Sunday { get; set; }
        public TimeSpan SundayStart { get; set; }
        public TimeSpan SundayEnd { get; set; }
        #endregion
    }
    public enum AppUserSalaryCycle
    {
        Weekly = 1,
        BiWeekly = 2,
        SemiWeekly = 3,
        EveryFourWeeks = 4,
        Monthly = 5
    }
    #endregion
}
