using Insight.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseApp.Entity
{
    public abstract class CertificationsRepository
    {
        [Sql("Certifications_SelectListByUserId")]
        public abstract IList<Certifications> SelectListByUserId(Guid UserId);

        [Sql("Certifications_SelectList")]
        public abstract IList<Certifications> SelectList();

        [Sql("Certifications_SelectListByStatus")]
        public abstract IList<Certifications> SelectListByStatus(CertificateStatus Status);

        [Sql("Certifications_Select")]
        public abstract Certifications Select(Guid Id);

        [Sql("Certifications_Insert")]
        public abstract Guid Insert(Certifications model);

        [Sql("Certifications_UpdateStatus")]
        public abstract void UpdateStatus(Certifications model);

        [Sql("Certifications_Update")]
        public abstract void Update(Certifications model);

        [Sql("Certifications_DeleteByUserIdnCertificateId")]
        public abstract void DeleteByUserIdnCertificateId(Guid UserId, Guid CertificateId);

        [Sql("Certifications_DeleteByCertificateId")]
        public abstract void DeleteByCertificateId(Guid CertificateId);

        [Sql("Certifications_CheckExpired")]
        public abstract bool CheckExpired(Guid UserId);

        //public abstract System.Data.IDbConnection GetConnection();

        //public void CheckCertificates(Guid id)
        //{
        //    var notify = GetConnection().As<IAppNotificationRepository>();
        //    var certificatesList = SelectListByUserId(id);

        //    foreach (var c in certificatesList)
        //    {
        //        if (c.ExpiryDate <= DateTime.UtcNow)
        //        {
        //            var cert = new Certifications();
        //            cert.Id = c.Id;
        //            cert.Status = CertificateStatus.Rejected;
        //            cert.Remarks = c.Remarks;
        //            UpdateStatus(cert);

        //            notify.Insert("Your " + c.Title + " certificate has been expired.", "/secure/certificates", id);

        //            notify.Insert(c.Username + " certificate has been expired.", "/secure/certificates/list", ContactType.AccountManager);
        //            notify.Insert(c.Username + " certificate has been expired.", "/secure/certificates/list", ContactType.Administrator);
        //        }

        //        if (c.ExpiryDate < DateTime.UtcNow.AddDays(7) && c.ExpiryDate > DateTime.UtcNow)
        //        {
        //            notify.Insert("Your " + c.Title + " certificate will expire soon.", "/secure/certificates", id);
        //        }
        //    }

        //}
    }
    public class Certifications
    {
        public Guid Id { get; set; }
        public Guid CertificateId { get; set; }
        public ImpactType Impact { get; set; }
        public string AppliesTo { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Title { get; set; }
        public string Number { get; set; }
        public string State { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Attachments { get; set; }
        public CertificateStatus Status { get; set; }
        public string Remarks { get; set; }
    }
    public enum CertificateStatus
    {
        Approved = 1,
        Pending = 2,
        Rejected = 3
    }
}
