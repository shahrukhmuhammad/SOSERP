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
    public class EmployeeEntity
    {
        private SOSHRMSEntities context;


        public List<HRM_Vew_Employee> GetAllEmployees()
        {
            try
            {
                using (context = new SOSHRMSEntities())
                {
                    var ls = context.HRM_Vew_Employee.ToList();
                    return ls;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CheckDuplicateEmail(Guid Id, string CNIC)
        {
            try
            {
                using (context = new SOSHRMSEntities())
                {
                    var res = context.Employees.Where(x=> x.CNIC.Trim() == CNIC.Trim()).FirstOrDefault();
                    if (res != null)
                    {
                        return res.EmployeeId == Id ? true : false;
                    }
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<CustomSelectList> GetEmployeeDropdown(Guid? Id = null)
        {
            try
            {
                using (context = new SOSHRMSEntities())
                {
                    var ls = new List<Employee>();
                    if (Id.HasValue)
                    {
                        ls = context.Employees.Where(x => x.PostId == Id).ToList();
                        return ls.Select(x => new CustomSelectList { Value = x.EmployeeId.ToString(), Text = x.Name }).ToList();
                    }
                    ls = context.Employees.ToList();
                    return ls.Select(x => new CustomSelectList { Value = x.EmployeeId.ToString(), Text = x.Code + " " + x.Name }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public string GetNextEmployeeCode()
        //{
        //    try
        //    {
        //        using (context = new SOSHRMSEntities())
        //        {
        //            return Convert.ToString(context.SP_Employee_GetMaxCode());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public string GetNextEmployeeCode()
        {
            try
            {
                using (context = new SOSHRMSEntities())
                {
                    return context.Database.SqlQuery<string>("exec SP_Employee_GetMaxCode").FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                //new Logger().LogError(ex);
                return null;
            }
        }
        public HRM_Vew_Employee GetUserById(Guid Id)
        {
            try
            {
                using (context = new SOSHRMSEntities())
                {
                    return context.HRM_Vew_Employee.Where(x => x.EmployeeId == Id).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EmpReference> GetReferencesByEmpId(Guid EmployeeId)
        {
            try
            {
                using (context = new SOSHRMSEntities())
                {
                    return context.EmpReferences.Where(x => x.EmployeeId == EmployeeId).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public Employee 


        #region Add/Update Employee
        public Guid? Create(HRM_Vew_Employee model)
        {
            try
            {
                using (context = new SOSHRMSEntities())
                {
                    //var userDto = Mapper.Map<HRM_Vew_Employee>(context.Employees);

                    #region Employee Master
                    var employeeModel = Mapper.Map<Employee>(model);
                    employeeModel.EmployeeId = Guid.NewGuid();
                    employeeModel.SegmentId = new Guid("2C4C0573-070F-499F-B800-22ED5356D5F7"); //For Guarding
                    context.Employees.Add(employeeModel);
                    context.SaveChanges();
                    #endregion

                    #region ArmyInformation Details
                    var armyObject = Mapper.Map<EmpArmyInformation>(model);
                    armyObject.EmployeeId = employeeModel.EmployeeId;
                    AddOrUpdateArmyInformation(armyObject);
                    #endregion

                    #region BankDetails
                    var bankObject = Mapper.Map<EmpBankDetail>(model);
                    bankObject.EmployeeId = employeeModel.EmployeeId;
                    AddOrUpdateBankDetails(bankObject);
                    #endregion

                    #region Emergency Contact Details
                    var emergencyObject = Mapper.Map<EmpEmergencyContact>(model);
                    emergencyObject.EmployeeId = employeeModel.EmployeeId;
                    AddOrUpdateEmergencyDetails(emergencyObject);
                    #endregion

                    #region Finger Print Details

                    var fingerPrintObject = Mapper.Map<EmpFingerPrint>(model);
                    fingerPrintObject.EmployeeId = employeeModel.EmployeeId;
                    AddOrUpdateFingerPrints(fingerPrintObject);

                    #endregion

                    #region Political Information
                    var politicalObject = Mapper.Map<EmpPoliticalInformation>(model);
                    politicalObject.EmployeeId = employeeModel.EmployeeId;
                    AddOrUpdatePoliticalInfo(politicalObject);
                    #endregion

                    #region Reference Information

                    //var referenceObject = Mapper.Map<EmpReference>(model);
                    //referenceObject.EmployeeId = employeeModel.EmployeeId;
                    //AddOrUpdateEmpReference(referenceObject);

                    //if (!string.IsNullOrEmpty(model.RefName) || !string.IsNullOrEmpty(model.RefCNIC))
                    //{
                    //    var referenceObject = Mapper.Map<EmpReference>(model);
                    //    referenceObject.ReferenceId = Guid.NewGuid();
                    //    referenceObject.EmployeeId = employeeModel.EmployeeId;
                    //    context.EmpReferences.Add(referenceObject);
                    //    context.SaveChanges();
                    //}

                    #endregion

                    #region Rejoin Information
                    var rejoinObject = Mapper.Map<EmpRejoinHistory>(model);
                    rejoinObject.EmployeeId = employeeModel.EmployeeId;
                    AddOrUpdateRejoinHistory(rejoinObject);

                    //if (model.PreviousAppointmentDate != null || model.PreviousTerminateDate != null)
                    //{
                    //    var rejoinObject = Mapper.Map<EmpRejoinHistory>(model);
                    //    rejoinObject.RejoinHistoryId = Guid.NewGuid();
                    //    rejoinObject.EmployeeId = employeeModel.EmployeeId;
                    //    context.EmpRejoinHistories.Add(rejoinObject);
                    //    context.SaveChanges();
                    //}
                    #endregion

                    #region Salary Details Information

                    var empSalaryObject = Mapper.Map<EmpSalaryDetail>(model);
                    empSalaryObject.EmployeeId = employeeModel.EmployeeId;
                    AddOrUpdateSalaryDetail(empSalaryObject);

                    //if (model.BasicSalary != null || model.DutyBouns != null)
                    //{
                    //    var empSalaryObject = Mapper.Map<EmpSalaryDetail>(model);
                    //    empSalaryObject.SalaryDetailsId = Guid.NewGuid();
                    //    empSalaryObject.EmployeeId = employeeModel.EmployeeId;
                    //    context.EmpSalaryDetails.Add(empSalaryObject);
                    //    context.SaveChanges();
                    //}
                    #endregion

                    #region Transfer History Information

                    var transferObject = Mapper.Map<EmpTransferHistory>(model);
                    transferObject.EmployeeId = employeeModel.EmployeeId;
                    AddOrUpdateTransferHistory(transferObject);

                    //if (model.OTSegment != null || model.NTSegment != null)
                    //{
                    //    var transferObject = Mapper.Map<EmpTransferHistory>(model);
                    //    transferObject.TransferHistoryId = Guid.NewGuid();
                    //    transferObject.EmployeeId = employeeModel.EmployeeId;
                    //    context.EmpTransferHistories.Add(transferObject);
                    //    context.SaveChanges();
                    //}
                    #endregion

                    return employeeModel.EmployeeId; //this.context.Employees.Include(x => x.EmpBankDetails).Where(x => x.EmployeeId == Id).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Update(HRM_Vew_Employee model)
        {
            try
            {
                using (context = new SOSHRMSEntities())
                {
                    #region Employee Master
                    var updatedEmployee = Mapper.Map<Employee>(model);
                    var dbEmployee = context.Employees.Where(x => x.EmployeeId == model.EmployeeId).FirstOrDefault();
                    if (dbEmployee != null)
                    {
                        updatedEmployee.CreatedBy = dbEmployee.CreatedBy;
                        updatedEmployee.CreatedOn = dbEmployee.CreatedOn;
                        context.Entry(dbEmployee).CurrentValues.SetValues(updatedEmployee);
                        context.SaveChanges();
                    }
                    #endregion

                    #region BankDetails
                    var dbBankDetails = Mapper.Map<EmpBankDetail>(model);
                    AddOrUpdateBankDetails(dbBankDetails);
                    #endregion

                    #region Emergency Contact Details
                    var energencyUpdated = Mapper.Map<EmpEmergencyContact>(model);
                    AddOrUpdateEmergencyDetails(energencyUpdated);
                    #endregion

                    #region Finger Print Details
                    var fingerPrintObject = Mapper.Map<EmpFingerPrint>(model);
                    AddOrUpdateFingerPrints(fingerPrintObject);
                    #endregion

                    #region Political Information
                    var politicalObject = Mapper.Map<EmpPoliticalInformation>(model);
                    AddOrUpdatePoliticalInfo(politicalObject);
                    #endregion

                    #region Reference Information
                    //var referenceObject = Mapper.Map<EmpReference>(model);
                    //AddOrUpdateEmpReference(referenceObject);
                    #endregion

                    #region Rejoin Information
                    var rejoinObject = Mapper.Map<EmpRejoinHistory>(model);
                    AddOrUpdateRejoinHistory(rejoinObject);
                    #endregion

                    #region Salary Details Information
                    var empSalaryObject = Mapper.Map<EmpSalaryDetail>(model);
                    AddOrUpdateSalaryDetail(empSalaryObject);
                    #endregion

                    #region Transfer History Information
                    var transferObject = Mapper.Map<EmpTransferHistory>(model);
                    AddOrUpdateTransferHistory(transferObject);
                    #endregion

                    return true;
                }
            }
            catch (Exception ex)
            {
                //throw ex;
                return false;
            }
        }
        #endregion

        #region ArmyInformation Details
        public void AddOrUpdateArmyInformation(EmpArmyInformation armyObject)
        {
            try
            {

                var dbArmyInfo = context.EmpArmyInformations.Where(x => x.EmployeeId == armyObject.EmployeeId).FirstOrDefault();

                if (dbArmyInfo != null)
                {
                    //Update
                    armyObject.ArmyInformationId = dbArmyInfo.ArmyInformationId;
                    armyObject.CreatedBy = dbArmyInfo.CreatedBy;
                    armyObject.CreatedOn = dbArmyInfo.CreatedOn;
                    context.Entry(dbArmyInfo).CurrentValues.SetValues(armyObject);
                    context.SaveChanges();
                }
                else
                {
                    //Add
                    if (armyObject.JoiningDate != null || !string.IsNullOrEmpty(armyObject.UnitName)
                        || !string.IsNullOrEmpty(armyObject.Rank))
                    {
                        armyObject.ArmyInformationId = Guid.NewGuid();
                        armyObject.CreatedBy = armyObject.CreatedBy != null ? armyObject.CreatedBy : dbArmyInfo.CreatedBy;
                        armyObject.CreatedOn = armyObject.CreatedOn != null ? armyObject.CreatedOn : dbArmyInfo.CreatedOn;
                        context.EmpArmyInformations.Add(armyObject);
                        context.SaveChanges();
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Add/Update Bank Details
        public void AddOrUpdateBankDetails(EmpBankDetail bankObject)
        {
            try
            {
                var dbBankInfo = context.EmpBankDetails.Where(x => x.EmployeeId == bankObject.EmployeeId).FirstOrDefault();
                if (dbBankInfo != null)
                {
                    //Update
                    bankObject.BankDetailsId = dbBankInfo.BankDetailsId;
                    bankObject.CreatedBy = dbBankInfo.CreatedBy;
                    bankObject.CreatedOn = dbBankInfo.CreatedOn;
                    context.Entry(dbBankInfo).CurrentValues.SetValues(bankObject);
                    context.SaveChanges();
                }
                else
                {
                    //Add
                    if (!string.IsNullOrEmpty(bankObject.BankName) && !string.IsNullOrEmpty(bankObject.AccountNo))
                    {
                        bankObject.BankDetailsId = Guid.NewGuid();
                        bankObject.CreatedBy = bankObject.CreatedBy != null ? bankObject.CreatedBy : dbBankInfo.CreatedBy;
                        bankObject.CreatedOn = bankObject.CreatedOn != null ? bankObject.CreatedOn : dbBankInfo.CreatedOn;
                        context.EmpBankDetails.Add(bankObject);
                        context.SaveChanges();
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Add/Update Emergency Contact Details
        public void AddOrUpdateEmergencyDetails(EmpEmergencyContact emergencyObject)
        {
            try
            {
                var dbEmergencyInfo = context.EmpEmergencyContacts.Where(x => x.EmployeeId == emergencyObject.EmployeeId).FirstOrDefault();
                if (dbEmergencyInfo != null)
                {
                    //Update
                    emergencyObject.EmergencyContactId = dbEmergencyInfo.EmergencyContactId;
                    emergencyObject.CreatedBy = dbEmergencyInfo.CreatedBy;
                    emergencyObject.CreatedOn = dbEmergencyInfo.CreatedOn;
                    context.Entry(dbEmergencyInfo).CurrentValues.SetValues(emergencyObject);
                    context.SaveChanges();
                }
                else
                {
                    //Add
                    if (!string.IsNullOrEmpty(emergencyObject.EmergencyCell) || !string.IsNullOrEmpty(emergencyObject.EmergencyPhone))
                    {
                        emergencyObject.EmergencyContactId = Guid.NewGuid();
                        emergencyObject.CreatedBy = emergencyObject.CreatedBy != null ? emergencyObject.CreatedBy : dbEmergencyInfo.CreatedBy;
                        emergencyObject.CreatedOn = emergencyObject.CreatedOn != null ? emergencyObject.CreatedOn : dbEmergencyInfo.CreatedOn;
                        context.EmpEmergencyContacts.Add(emergencyObject);
                        context.SaveChanges();
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Add/Update Finger Print Details
        public void AddOrUpdateFingerPrints(EmpFingerPrint fingerPrintObject)
        {
            try
            {
                var dbFingerInfo = context.EmpFingerPrints.Where(x => x.EmployeeId == fingerPrintObject.EmployeeId).FirstOrDefault();
                if (dbFingerInfo != null)
                {
                    //Update
                    fingerPrintObject.CreatedBy = dbFingerInfo.CreatedBy;
                    fingerPrintObject.CreatedOn = dbFingerInfo.CreatedOn;
                    context.Entry(dbFingerInfo).CurrentValues.SetValues(fingerPrintObject);
                    context.SaveChanges();
                }
                else
                {
                    //Add
                    if (!string.IsNullOrEmpty(fingerPrintObject.LeftThumb) || !string.IsNullOrEmpty(fingerPrintObject.RightThumb))
                    {
                        fingerPrintObject.FingerPrintId = Guid.NewGuid();
                        fingerPrintObject.CreatedBy = fingerPrintObject.CreatedBy != null ? fingerPrintObject.CreatedBy : dbFingerInfo.CreatedBy;
                        fingerPrintObject.CreatedOn = fingerPrintObject.CreatedOn != null ? fingerPrintObject.CreatedOn : dbFingerInfo.CreatedOn;
                        context.EmpFingerPrints.Add(fingerPrintObject);
                        context.SaveChanges();
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Add/Update Political Details
        public void AddOrUpdatePoliticalInfo(EmpPoliticalInformation politicalObject)
        {
            try
            {
                var dbpoliticalInfo = context.EmpPoliticalInformations.Where(x => x.EmployeeId == politicalObject.EmployeeId).FirstOrDefault();
                if (dbpoliticalInfo != null)
                {
                    //Update
                    politicalObject.PoliticalInformationId = dbpoliticalInfo.PoliticalInformationId;
                    politicalObject.CreatedOn = dbpoliticalInfo.CreatedOn;
                    politicalObject.CreatedBy = dbpoliticalInfo.CreatedBy;
                    context.Entry(dbpoliticalInfo).CurrentValues.SetValues(politicalObject);
                    context.SaveChanges();
                }
                else
                {
                    //Add
                    if (!string.IsNullOrEmpty(politicalObject.PoliceStation) || !string.IsNullOrEmpty(politicalObject.VoteNo))
                    {
                        politicalObject.PoliticalInformationId = Guid.NewGuid();
                        politicalObject.CreatedOn = politicalObject.CreatedOn != null ? politicalObject.CreatedOn : dbpoliticalInfo.CreatedOn;
                        politicalObject.CreatedBy = politicalObject.CreatedBy != null ? politicalObject.CreatedBy : dbpoliticalInfo.CreatedBy;
                        context.EmpPoliticalInformations.Add(politicalObject);
                        context.SaveChanges();
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Add/Update Rejoin History
        public void AddOrUpdateRejoinHistory(EmpRejoinHistory rejoinHistoryObject)
        {
            try
            {
                var dbRejoinHistories = context.EmpRejoinHistories.Where(x => x.EmployeeId == rejoinHistoryObject.EmployeeId).FirstOrDefault();
                if (dbRejoinHistories != null)
                {
                    //Update
                    rejoinHistoryObject.RejoinHistoryId = dbRejoinHistories.RejoinHistoryId;
                    rejoinHistoryObject.CreatedOn = dbRejoinHistories.CreatedOn;
                    rejoinHistoryObject.CreatedBy = dbRejoinHistories.CreatedBy;
                    context.Entry(dbRejoinHistories).CurrentValues.SetValues(rejoinHistoryObject);
                    context.SaveChanges();
                }
                else
                {
                    //Add
                    if (rejoinHistoryObject.PreviousAppointmentDate != null || rejoinHistoryObject.PreviousTerminateDate != null)
                    {
                        rejoinHistoryObject.RejoinHistoryId = Guid.NewGuid();
                        rejoinHistoryObject.CreatedOn = rejoinHistoryObject.CreatedOn != null ? rejoinHistoryObject.CreatedOn : dbRejoinHistories.CreatedOn;
                        rejoinHistoryObject.CreatedBy = rejoinHistoryObject.CreatedBy != null ? rejoinHistoryObject.CreatedBy : dbRejoinHistories.CreatedBy;
                        context.EmpRejoinHistories.Add(rejoinHistoryObject);
                        context.SaveChanges();
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Add/Update Reference Details
        //public void AddOrUpdateEmpReference(EmpReference referenceObject)
        //{
        //    try
        //    {
        //        var dbReferenceInfo = context.EmpReferences.Where(x => x.EmployeeId == referenceObject.EmployeeId).FirstOrDefault();
        //        if (dbReferenceInfo != null)
        //        {
        //            //Update
        //            referenceObject.ReferenceId = dbReferenceInfo.ReferenceId;
        //            referenceObject.CreatedOn = dbReferenceInfo.CreatedOn;
        //            referenceObject.CreatedBy = dbReferenceInfo.CreatedBy;
        //            context.Entry(dbReferenceInfo).CurrentValues.SetValues(referenceObject);
        //            context.SaveChanges();
        //        }
        //        else
        //        {
        //            //Add
        //            if (!string.IsNullOrEmpty(referenceObject.RefName) || !string.IsNullOrEmpty(referenceObject.RefCNIC))
        //            {
        //                referenceObject.ReferenceId = Guid.NewGuid();
        //                referenceObject.CreatedOn = referenceObject.CreatedOn != null ? referenceObject.CreatedOn : dbReferenceInfo.CreatedOn;
        //                referenceObject.CreatedBy = referenceObject.CreatedBy != null ? referenceObject.CreatedBy : dbReferenceInfo.CreatedBy;
        //                context.EmpReferences.Add(referenceObject);
        //                context.SaveChanges();
        //            }
        //        }

        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        #endregion

        #region Add/Update Salary Details
        public void AddOrUpdateSalaryDetail(EmpSalaryDetail salaryObject)
        {
            try
            {
                var dbEmpSalaryDetails = context.EmpSalaryDetails.Where(x => x.EmployeeId == salaryObject.EmployeeId).FirstOrDefault();
                if (dbEmpSalaryDetails != null)
                {
                    //Update
                    salaryObject.SalaryDetailsId = dbEmpSalaryDetails.SalaryDetailsId;
                    salaryObject.CreatedOn = dbEmpSalaryDetails.CreatedOn;
                    salaryObject.CreatedOn = dbEmpSalaryDetails.CreatedOn;
                    context.Entry(dbEmpSalaryDetails).CurrentValues.SetValues(salaryObject);
                    context.SaveChanges();
                }
                else
                {
                    //Add
                    if (salaryObject.BasicSalary != null || salaryObject.DutyBouns != null)
                    {
                        salaryObject.SalaryDetailsId = Guid.NewGuid();
                        salaryObject.CreatedOn = salaryObject.CreatedOn != null ? salaryObject.CreatedOn : salaryObject.UpdatedOn.Value;
                        salaryObject.CreatedBy = salaryObject.CreatedBy != null ? salaryObject.CreatedBy : salaryObject.UpdatedBy.Value;
                        context.EmpSalaryDetails.Add(salaryObject);
                        context.SaveChanges();
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Add/Update Transfer Details
        public void AddOrUpdateTransferHistory(EmpTransferHistory transferObject)
        {
            try
            {
                var dbEmpSalaryDetails = context.EmpTransferHistories.Where(x => x.EmployeeId == transferObject.EmployeeId).FirstOrDefault();
                if (dbEmpSalaryDetails != null)
                {
                    //Update
                    transferObject.TransferHistoryId = dbEmpSalaryDetails.TransferHistoryId;
                    transferObject.CreatedOn = dbEmpSalaryDetails.CreatedOn;
                    transferObject.CreatedBy = dbEmpSalaryDetails.CreatedBy;
                    context.Entry(dbEmpSalaryDetails).CurrentValues.SetValues(transferObject);
                    context.SaveChanges();
                }
                else
                {
                    //Add
                    if (transferObject.OTSegment != null || transferObject.NTSegment != null)
                    {
                        transferObject.TransferHistoryId = Guid.NewGuid();
                        transferObject.CreatedOn = transferObject.CreatedOn != null ? transferObject.CreatedOn : transferObject.UpdatedOn.Value;
                        transferObject.CreatedBy = transferObject.CreatedBy != null ? transferObject.CreatedBy : transferObject.UpdatedBy.Value;
                        context.EmpTransferHistories.Add(transferObject);
                        context.SaveChanges();
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Add/Update References
        public void AddOrUpdateEmpReference(List<EmpReference> referenceObject)
        {
            try
            {
                using (context = new SOSHRMSEntities())
                {
                    foreach (var item in referenceObject)
                    {
                        var resObj = context.EmpReferences.Where(x => x.ReferenceId == item.ReferenceId).FirstOrDefault();
                        if (resObj != null)
                        {
                            //Update
                            //resObj.ReferenceId = item.ReferenceId;
                            item.CreatedOn = resObj.CreatedOn;
                            item.CreatedBy = resObj.CreatedBy;
                            context.Entry(resObj).CurrentValues.SetValues(item);
                            context.SaveChanges();
                        }
                        else
                        {
                            //item.ReferenceId = Guid.NewGuid();
                            context.EmpReferences.Add(item);
                            context.SaveChanges();
                        }

                    }
                    //context.EmpReferences.AddRange(referenceObject);
                    //context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Documents
        public List<EmpDocument> GetDocumentsByEmpId(Guid Id)
        {
            try
            {
                using (context = new SOSHRMSEntities())
                {
                    var ls = context.EmpDocuments.Where(x => x.EmployeeId == Id).ToList();
                    //foreach (var item in ls)
                    //{
                    //    item.ContentType = 
                    //}
                    return ls;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public EmpDocument GetDocumentById(Guid Id)
        {
            try
            {
                using (context = new SOSHRMSEntities())
                {
                    return context.EmpDocuments.Where(x => x.DocumentId == Id).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SaveDocs(List<EmpDocument> lsDocs)
        {
            try
            {
                using (context = new SOSHRMSEntities())
                {
                    context.EmpDocuments.AddRange(lsDocs);
                    context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion



    }

    public enum ProfileStatus { Draft = 1, Done = 2, Review = 3, Completed = 4 }
    public enum EmployeeStatus { Active = 1, Suspended = 2, Terminated = 3 }
    public enum Gender
    {
        Male = 1,
        Female = 2,
        Other = 3
    }

    public enum MaritalStatus
    {
        Single = 1,
        Married = 2
    }
    public enum AddressType
    {
        Default,
        Home,
        Work,
        Other,
        Permanent,
        HeadOffice,
    }
    public enum DoumentType
    {
        DischargeBook,
        EducationCertificate,
        PoliceVerification,
        SendForPoliceAttestion,
        NadraAttested,
        IdentityCardPension,
        PositionBook,
        CNICFrontCopy,
        CNICBackCopy
    }
    public enum EmployeeType
    {
        MANAGEMENT = 1,
        CREW = 2
    }
}
