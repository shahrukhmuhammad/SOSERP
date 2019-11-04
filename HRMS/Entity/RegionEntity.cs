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
                    return context.Regions.ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Region> GetRegionById(Guid Id)
        {
            try
            {
                using (context = new SOSHRMSEntities())
                {
                    return context.Regions.Where(x => x.RegionId == Id).ToList();
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

    }
}
