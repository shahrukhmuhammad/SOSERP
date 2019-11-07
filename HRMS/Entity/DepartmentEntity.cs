using AutoMapper;
using HRMS.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
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
                    return context.Departments.OrderBy(x=> x.Code).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Department GetDepartmentById(Guid Id)
        {
            try
            {
                using (context = new SOSHRMSEntities())
                {
                    return context.Departments.Where(x=> x.Id == Id).FirstOrDefault();
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


        #region Add/Update Employee
        public Guid? Create(Department model)
        {
            try
            {
                using (context = new SOSHRMSEntities())
                {
                    #region Save Department
                    context.Departments.Add(model);
                    context.SaveChanges();
                    #endregion
                    return model.Id;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Update(Department model)
        {
            try
            {
                using (context = new SOSHRMSEntities())
                {
                    #region Update Employee
                    var res = context.Departments.Where(x => x.Id == model.Id).FirstOrDefault();
                    if (res != null)
                    {
                        res.IsActive = model.IsActive;
                        res.Name = model.Name;
                        res.UpdatedOn = model.UpdatedOn;
                        res.UpdatedBy = model.UpdatedBy;
                        context.SaveChanges();
                    }
                    #endregion
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region Delete
        public bool Delete(Guid Id)
        {
            try
            {
                using (context = new SOSHRMSEntities())
                {
                    #region Update Employee
                    var res = context.Departments.Where(x => x.Id == Id).FirstOrDefault();
                    if (res != null)
                    {
                        context.Departments.Remove(res);
                        context.SaveChanges();
                    }
                    #endregion
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteMultiple(List<Guid> Ids)
        {
            try
            {
                using (context = new SOSHRMSEntities())
                {
                    #region Update Employee
                    var res = context.Departments.Where(x => Ids.Contains(x.Id)).ToList();
                    if (res != null)
                    {
                        context.Departments.RemoveRange(res);
                        context.SaveChanges();
                    }
                    #endregion
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion
    }
}
