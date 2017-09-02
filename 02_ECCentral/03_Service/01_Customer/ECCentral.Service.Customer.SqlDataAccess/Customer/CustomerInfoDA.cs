using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.Customer;
using System.Data;

namespace ECCentral.Service.Customer.SqlDataAccess
{
    [VersionExport(typeof(ICustomerInfoDA))]
    public class CustomerInfoDA : ICustomerInfoDA
    {
        #region ICustomerInfoDA Members

        public virtual CustomerInfo CreateDetailInfo(CustomerInfo customer)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateCustomerDetailInfo");
            cmd.SetParameterValue<CustomerInfo>(customer);
            cmd.ExecuteNonQuery();
            return customer;
        }

        public virtual void UpdateDetailInfo(CustomerInfo customer)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateCustomerDetailInfo");
            cmd.SetParameterValue<CustomerInfo>(customer);
            cmd.ExecuteNonQuery();
        }

        public virtual void UpdateCustomerStatus(int customerSysNo, CustomerStatus status)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateCustomerStatus");
            cmd.SetParameterValue("@SysNo", customerSysNo);
            cmd.SetParameterValue("@Status", status);
            cmd.ExecuteNonQuery();
        }

        public void CancelConfirmEmail(int customerSysNo, bool isEmailConfirmed)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CancelConfirmCustomerEmail");
            cmd.SetParameterValue("@SysNo", customerSysNo);
            cmd.SetParameterValue("@IsEmailConfirmed", isEmailConfirmed);
            cmd.ExecuteNonQuery();
        }

        public void CancelConfirmPhone(int customerSysNo, bool cellPhoneConfirmed)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CancelConfirmCustomerPhone");
            cmd.SetParameterValue("@SysNo", customerSysNo);
            cmd.SetParameterValue("@Status", cellPhoneConfirmed);
            cmd.ExecuteNonQuery();
        }

        public virtual void UpdateExperience(int customerSysNo, decimal totalSO)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateTotalSOMoney");
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);
            cmd.SetParameterValue("@TotalSOMoney", totalSO);
            cmd.ExecuteNonQuery();
        }


        public virtual void AdjustOrderedAmount(int customerSysNo, decimal orderedAmount)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateConfirmedTotalAmt");
            cmd.SetParameterValue("@SysNo", customerSysNo);
            cmd.SetParameterValue("@ConfirmedTotalAmt", orderedAmount);
            cmd.ExecuteNonQuery();
        }

        public virtual void UpdateIsBadCustomer(int customerSysNo, bool isBadUser)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateIsBadCustomer");
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);
            cmd.SetParameterValue("@IsBadCustomer", isBadUser);
            cmd.ExecuteNonQuery();
        }

        public virtual void UpdateAvatarStatus(int CustomerSysNo, AvtarShowStatus AvtarImageStatus)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateAvtarImageStatus");
            cmd.SetParameterValue("@CustomerSysno", CustomerSysNo);
            cmd.SetParameterValue("@AvtarImageStatus", AvtarImageStatus);
            cmd.ExecuteNonQuery();
        }

        public virtual CustomerInfo GetCustomerBySysNo(int customerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCustomerBySysNo");
            cmd.SetParameterValue("@SysNo", customerSysNo);
            return cmd.ExecuteEntity<CustomerInfo>();
        }

        public virtual void InsertCustomerInfoOperateLog(CustomerOperateLog entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertCustomerInfoOperateLog");
            cmd.SetParameterValue<CustomerOperateLog>(entity);
            cmd.ExecuteNonQuery();
        }

        public virtual void InsertExperienceLog(CustomerExperienceLog entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertExperienceLog");
            cmd.SetParameterValue<CustomerExperienceLog>(entity);
            cmd.ExecuteNonQuery();
        }

        #endregion

        #region ICustomerInfoDA Members

        /// <summary>
        /// 获取所有恶意用户
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public virtual List<CustomerInfo> GetMalevolenceCustomerList(string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetMalevolenceCustomerList");
            cmd.SetParameterValue("@CompanyCode", companyCode);
            return cmd.ExecuteEntityList<CustomerInfo>();
        }

        #endregion
    }
}
