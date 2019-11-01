using HRMS.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebApp.Areas.HRMS
{
    public class EmployeePostedFiles
    {
        public HttpPostedFileBase ProfilePhoto { get; set; }
        public HttpPostedFileBase Reference1Photo { get; set; }
        public HttpPostedFileBase Reference2Photo { get; set; }
        public HttpPostedFileBase LeftThumb { get; set; }
        public HttpPostedFileBase LeftIndexFinger { get; set; }
        public HttpPostedFileBase LeftMiddleFinger { get; set; }
        public HttpPostedFileBase LeftRingFinger { get; set; }
        public HttpPostedFileBase LeftBabyFinger { get; set; }
        public HttpPostedFileBase RightThumb { get; set; }
        public HttpPostedFileBase RightIndexFinger { get; set; }
        public HttpPostedFileBase RightMiddleFinger { get; set; }
        public HttpPostedFileBase RightRingFinger { get; set; }
        public HttpPostedFileBase RightBabyFinger { get; set; }
    }

    public class DocumentPostedFiles
    {
        public HttpPostedFileBase DischargeBook { get; set; }
        public HttpPostedFileBase EducationCertificate { get; set; }
        public HttpPostedFileBase PoliceVerification { get; set; }
        public HttpPostedFileBase SendForPoliceAttestion { get; set; }
        public HttpPostedFileBase NadraAttested { get; set; }
        public HttpPostedFileBase IdentityCardPension { get; set; }
        public HttpPostedFileBase PositionBook { get; set; }
        public HttpPostedFileBase CNICFrontCopy { get; set; }
        public HttpPostedFileBase CNICBackCopy { get; set; }
    }
}