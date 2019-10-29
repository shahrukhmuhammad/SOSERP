using System;
using System.Collections.Generic;
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
}