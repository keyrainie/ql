using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.IDataAccess
{

    public interface ICustomerInfoDA
    {
        CustomerInfo CreateDetailInfo(CustomerInfo customer);

        void UpdateDetailInfo(CustomerInfo customer);

        void UpdateCustomerStatus(int customerSysNo, CustomerStatus status);

        void UpdateExperience(int customerSysNo, decimal totalSO);

        void AdjustOrderedAmount(int customerSysNo, decimal orderedAmount);

        void UpdateIsBadCustomer(int customerSysNo, bool isBadUser);

        void UpdateAvatarStatus(int CustomerSysNo, AvtarShowStatus AvtarImageStatus);

        CustomerInfo GetCustomerBySysNo(int customerSysNo);
 
        void InsertCustomerInfoOperateLog(CustomerOperateLog entity);

        void InsertExperienceLog(CustomerExperienceLog entity);

        List<CustomerInfo> GetMalevolenceCustomerList(string companyCode);

        void CancelConfirmEmail(int customerSysNo, bool isEmailConfirmed);

        void CancelConfirmPhone(int customerSysNo, bool cellPhoneConfirmed);
    }

  

}