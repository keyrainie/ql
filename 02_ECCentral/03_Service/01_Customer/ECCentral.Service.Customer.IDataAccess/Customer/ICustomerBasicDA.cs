using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.IDataAccess 
{
    public interface ICustomerBasicDA
    {
        bool IsExists(int customerSysNo);
        CustomerBasicInfo GetCustomerBasicInfoBySysNo(int customerSysNo);
        List<CustomerBasicInfo> GetCustomerBasicInfoBySysNoList(string sysNos);
        void CreateBasicInfo(CustomerBasicInfo entity);
        void UpdateBasicInfo(CustomerBasicInfo entity);
        void UpdatePassword(PasswordInfo info);
        List<CustomerBasicInfo> GetCustomerByCustomerIdList(string ids);
        List<CustomerBasicInfo> GetCustomerByEmailList(string emails);
        List<CustomerBasicInfo> GetSystemAccount(string webChannelID);
       CustomerBasicInfo CheckSameCellPhone(CustomerBasicInfo entity,string companyCode);
       CustomerBasicInfo GetCustomerBasicInfoByID(string customerID);
        List<CustomerBasicInfo> GetCustomerByIdentityCard(string identityCard);
    }
}
