using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;
using MktToolMgmt.PromotionCustomerLogApp.Entities;
using System.Configuration;

namespace MktToolMgmt.PromotionCustomerLogApp.DA
{
    public static class SettingCustomerDA
    {
        static string CompanyCode = ConfigurationManager.AppSettings["CompanyCode"];
        public static List<SettingCustomerEntity> GetAvailable()
        {
            DataCommand command = DataCommandManager.GetDataCommand("Promotion_GetAvailableSettingCustomer");
            command.SetParameterValue("CompanyCode", ConstValues.CompanyCode);
            return command.ExecuteEntityList<SettingCustomerEntity>();
        }

        public static int UpdateStatus(SettingCustomerEntity entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Promotion_SetSettingCustomerStatus");
            command.SetParameterValue("EditUser", ConstValues.EditUser);
            command.SetParameterValue("Status", entity.Status);
            command.SetParameterValue("SysNo", entity.SysNo);
            command.SetParameterValue("CompanyCode", ConstValues.CompanyCode);
            return command.ExecuteNonQuery();
        }

        public static int SendMailInfo(MailEntity entity)
        {
            DataCommand command = null;
            if (entity.IsInternal)
            {
                command = DataCommandManager.GetDataCommand("SendMailInternal");
            }
            else
            {
                command = DataCommandManager.GetDataCommand("SendMailInfo");
            }
            command.SetParameterValue("MailAddress", entity.MailAddress);//ConstValues.ToMailAddress);
            command.SetParameterValue("CCMailAddress", entity.CCMailAddress);//ConstValues.CcMailAddress);
            command.SetParameterValue("MailSubject", entity.MailSubject);
            command.SetParameterValue("MailBody", entity.MailBody);
            command.SetParameterValue("CompanyCode", ConstValues.CompanyCode);
            command.SetParameterValue("StoreCompanyCode", ConstValues.StoreCompanyCode);
            command.SetParameterValue("LanguageCode", ConstValues.LanguageCode);
            return command.ExecuteNonQuery();
        }

    }
}
