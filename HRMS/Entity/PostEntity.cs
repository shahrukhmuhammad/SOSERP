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
    public class PostEntity
    {
        private SOSHRMSEntities context;


        public List<Center> GetAllCenters()
        {
            try
            {
                using (context = new SOSHRMSEntities())
                {
                    return context.Centers.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Center> GetCentersById(Guid Id)
        {
            try
            {
                using (context = new SOSHRMSEntities())
                {
                    return context.Centers.Where(x=> x.CenterId == Id).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CustomSelectList> GetCentersDropdown(Guid? RegionId = null)
        {
            try
            {
                using (context = new SOSHRMSEntities())
                {
                    var ls = new List<Center>();
                    if (RegionId.HasValue)
                    {
                        ls = context.Centers.Where(x => x.RegionId == RegionId).ToList();
                        return ls.Select(x => new CustomSelectList { Value = x.CenterId.ToString(), Text = x.Name }).ToList();
                    }
                    ls = context.Centers.ToList();
                    return ls.Select(x => new CustomSelectList { Value = x.CenterId.ToString(), Text = x.Name }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
