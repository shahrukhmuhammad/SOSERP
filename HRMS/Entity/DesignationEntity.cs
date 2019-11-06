using AutoMapper;
using HRMS.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS
{
    public class DesignationEntity
    {
        private SOSHRMSEntities context;
        public List<Designation> GetAllDesignations()
        {
            try
            {
                using (context = new SOSHRMSEntities())
                {
                    return context.Designations.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Designation> GetDesignationById(Guid Id)
        {
            try
            {
                using (context = new SOSHRMSEntities())
                {
                    return context.Designations.Where(x=> x.Id == Id).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CustomSelectList> GetDesignationDropdown(Guid? Id = null)
        {
            try
            {
                using (context = new SOSHRMSEntities())
                {
                    var ls = new List<Designation>();
                    if (Id.HasValue)
                    {
                        ls = context.Designations.Where(x => x.DepartmentId == Id).ToList();
                        return ls.Select(x => new CustomSelectList { Value = x.Id.ToString(), Text = x.Code + " - " + x.Title }).ToList();
                    }
                    ls = context.Designations.ToList();
                    return ls.Select(x => new CustomSelectList { Value = x.Id.ToString(), Text = x.Code + " - " + x.Title }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
