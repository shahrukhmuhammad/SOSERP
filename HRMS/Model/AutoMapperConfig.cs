using AutoMapper;
using HRMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS
{
    public class AutoMapperConfig
    {
        public static void Intialization()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<HRM_Vew_Employee, Employee>();
                cfg.CreateMap<HRM_Vew_Employee, EmpArmyInformation>();
                cfg.CreateMap<HRM_Vew_Employee, EmpBankDetail>();
                cfg.CreateMap<HRM_Vew_Employee, EmpEmergencyContact>();
                cfg.CreateMap<HRM_Vew_Employee, EmpFingerPrint>();
                cfg.CreateMap<HRM_Vew_Employee, EmpPoliticalInformation>();
                cfg.CreateMap<HRM_Vew_Employee, EmpReference>();
                cfg.CreateMap<HRM_Vew_Employee, EmpRejoinHistory>();
                cfg.CreateMap<HRM_Vew_Employee, EmpSalaryDetail>();
                cfg.CreateMap<HRM_Vew_Employee, EmpTransferHistory>();

            });
        }
    }
}
