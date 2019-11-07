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
    public class RegionEntity
    {
        private SOSHRMSEntities context;


        public List<Region> GetAllRegions()
        {
            try
            {
                using (context = new SOSHRMSEntities())
                {
                    return context.Regions.OrderBy(x=> x.Code).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Region GetRegionById(Guid Id)
        {
            try
            {
                using (context = new SOSHRMSEntities())
                {
                    return context.Regions.Where(x => x.RegionId == Id).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CustomSelectList> GetAllRegionsDropdown()
        {
            try
            {
                using (context = new SOSHRMSEntities())
                {
                    var ls = context.Regions.ToList();
                    return ls.Select(x => new CustomSelectList { Value = x.RegionId.ToString(), Text = x.Code + " - " + x.Name }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #region Add/Update Employee
        public Guid? Create(Region model)
        {
            try
            {
                using (context = new SOSHRMSEntities())
                {
                    #region Save Region
                    context.Regions.Add(model);
                    context.SaveChanges();
                    #endregion
                    return model.RegionId;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Update(Region model)
        {
            try
            {
                using (context = new SOSHRMSEntities())
                {
                    #region Update Employee
                    var res = context.Regions.Where(x => x.RegionId == model.RegionId).FirstOrDefault();
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
                    var res = context.Regions.Where(x => x.RegionId == Id).FirstOrDefault();
                    if (res != null)
                    {
                        context.Regions.Remove(res);
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
                    var res = context.Regions.Where(x => Ids.Contains(x.RegionId)).ToList();
                    if (res != null)
                    {
                        context.Regions.RemoveRange(res);
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
