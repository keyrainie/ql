using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System;
using System.Data;
using System.Text;
using ECCentral.QueryFilter.SO;
using System.Collections.Generic;

namespace ECCentral.Service.SO.SqlDataAccess
{
    [VersionExport(typeof(ISOKFCDA))]
    public class SOKFCDA : ISOKFCDA
    {
        public int InsertKnowFrandCustomer(KnownFraudCustomer entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Insert_KnowFrandCustomer");

            command.SetParameterValue("@BrowseInfo", entity.BrowseInfo);
            command.SetParameterValue("@CustomerSysNo", entity.CustomerSysNo);
            command.SetParameterValue("@EmailAddress", entity.EmailAddress);
            command.SetParameterValue("@FraudType", entity.KFCType);
            command.SetParameterValue("@IPAddress", entity.IPAddress);
            command.SetParameterValue("@InDate", entity.CreateDate);
            command.SetParameterValue("@InUser", entity.CreateUserName);
            command.SetParameterValue("@MobilePhone", entity.MobilePhone);
            command.SetParameterValue("@ShippingAddress", entity.ShippingAddress);
            command.SetParameterValue("@ShippingContact", entity.ShippingContact);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@Telephone", entity.Telephone);
            command.SetParameterValue("@CompanyCode", entity.CompanyCode);
            int result = command.ExecuteNonQuery();
            return result;       
        }

        public KnownFraudCustomer GetKFCByCustomerSysNo(int customSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_KFCByCustomerSysNo");
            command.SetParameterValue("@CustomerSysNo", customSysNo);
            return command.ExecuteEntity<KnownFraudCustomer>();
        }

        public List<KnownFraudCustomer> GetKFCByIPAndTel(string ipAddress, string mobilePhone, string telephone, string companyCode)
        {
            if (string.IsNullOrEmpty(mobilePhone))
            {
                mobilePhone = null;
            }

            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_KFCByTelPhone");

            command.SetParameterValue("@IPAddress", ipAddress);
            command.SetParameterValue("@MobilePhone", mobilePhone);
            command.SetParameterValue("@Telephone", telephone);
            command.SetParameterValue("@CompanyCode", companyCode);

            return command.ExecuteEntityList<KnownFraudCustomer>();
        }

        public void UpdateKnowFrandCustomerStatus(KnownFraudCustomer entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Update_KnowFrandCustomerStatus");

            command.SetParameterValue("@CustomerSysNo", entity.CustomerSysNo);
            command.SetParameterValue("@FraudType", entity.KFCType);
            command.SetParameterValue("@EditDate", entity.LastEditDate);
            command.SetParameterValue("@EditUser", entity.LastEditUserName);

            command.ExecuteNonQuery();
        }
    }
}
