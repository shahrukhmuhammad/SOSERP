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
    public class DepartmentEntity
    {
        private SOSHRMSEntities context;


        public List<Department> GetAllDepartments()
        {
            try
            {
                using (context = new SOSHRMSEntities())
                {
                    return context.Departments.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Department> GetDepartmentById(Guid Id)
        {
            try
            {
                using (context = new SOSHRMSEntities())
                {
                    return context.Departments.Where(x=> x.Id == Id).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CustomSelectList> GetDepartmentDropdown(Guid? Id = null)
        {
            try
            {
                using (context = new SOSHRMSEntities())
                {
                    var ls = new List<Department>();
                    if (Id.HasValue)
                    {
                        ls = context.Departments.Where(x => x.Id == Id).ToList();
                        return ls.Select(x => new CustomSelectList { Value = x.Id.ToString(), Text = x.Code + " - " + x.Name }).ToList();
                    }
                    ls = context.Departments.ToList();
                    return ls.Select(x => new CustomSelectList { Value = x.Id.ToString(), Text = x.Code + " - " + x.Name }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
