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
    public class ProjectEntity
    {
        private SOSHRMSEntities context;


        public List<Project> GetAllProjects()
        {
            try
            {
                using (context = new SOSHRMSEntities())
                {
                    return context.Projects.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Project GetProjectById(Guid Id)
        {
            try
            {
                using (context = new SOSHRMSEntities())
                {
                    return context.Projects.Where(x => x.Id == Id).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CustomSelectList> GetAllProjectsDropdown()
        {
            try
            {
                using (context = new SOSHRMSEntities())
                {
                    var ls = context.Projects.ToList();
                    return ls.Select(x => new CustomSelectList { Value = x.Id.ToString(), Text = x.Id + " - " + x.Name }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #region Add/Update Employee
        public Guid? Create(Project model)
        {
            try
            {
                using (context = new SOSHRMSEntities())
                {
                    #region Save Project
                    context.Projects.Add(model);
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

        public bool Update(Project model)
        {
            try
            {
                using (context = new SOSHRMSEntities())
                {
                    #region Update Employee
                    var res = context.Projects.Where(x => x.Id == model.Id).FirstOrDefault();
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
                    var res = context.Projects.Where(x => x.Id == Id).FirstOrDefault();
                    if (res != null)
                    {
                        context.Projects.Remove(res);
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
                    var res = context.Projects.Where(x => Ids.Contains(x.Id)).ToList();
                    if (res != null)
                    {
                        context.Projects.RemoveRange(res);
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
