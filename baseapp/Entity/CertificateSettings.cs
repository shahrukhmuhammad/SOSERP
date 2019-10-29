using Insight.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApp.Entity
{
    public abstract class CertificateSettingsRepository
    {
        [Sql("CertificateSettings_Select")]
        public abstract CertificateSettings Select(Guid Id);

        [Sql("CertificateSettings_SelectList")]
        public abstract IList<CertificateSettings> SelectList();

        [Sql("CertificateSettings_Insert")]
        public abstract Guid Insert(CertificateSettings model);

        [Sql("CertificateSettings_Update")]
        public abstract void Update(CertificateSettings model);

        [Sql("CertificateSettings_Delete")]
        public abstract void Delete(Guid Id);

        //public abstract System.Data.IDbConnection GetConnection();

        //public void CheckContactType(Contact model)
        //{
        //    var contactRepo = GetConnection().As<IContactRepository>();
        //    var contactType = contactRepo.Select(model.Id).ContactType;
        //    var certificateRepo = GetConnection().As<CertificationsRepository>();

        //    var sep = new string[] { "," };

        //    foreach (var c in SelectList())
        //    {
        //        //var getContactType = (ContactType)Enum.Parse(typeof(ContactType),c.AppliesTo.Split(sep, StringSplitOptions.RemoveEmptyEntries).ToString());
        //        if (c.AppliesTo.Split(sep, StringSplitOptions.RemoveEmptyEntries).Contains(contactType.ToString()))
        //        {
        //            var certificate = new Certifications
        //            {
        //                CertificateId = c.Id,
        //                UserId = model.Id,
        //                ExpiryDate = DateTime.UtcNow,
        //                Status = CertificateStatus.Pending
        //            };
        //            certificateRepo.Insert(certificate);
        //        }
        //    }
        //}
    }

    public class CertificateSettings
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ImpactType Impact { get; set; }
        public string AppliesTo { get; set; }
        public PerformAction PerformAction { get; set; }
        public bool IsExpiryDate { get; set; }
        public bool FullTime { get; set; }
        public bool PartTime { get; set; }
        public bool VaryingWeekly { get; set; }
        public bool Contractor { get; set; }
        public bool Freelance { get; set; }
    }
    public enum ImpactType
    {
        Compulsory = 1,
        Important = 2,
        GoodToHave = 3
    }

    public enum PerformAction
    {
        BlockUserAccess = 1, ShowAlert = 2, NoAction = 3
    }
}
