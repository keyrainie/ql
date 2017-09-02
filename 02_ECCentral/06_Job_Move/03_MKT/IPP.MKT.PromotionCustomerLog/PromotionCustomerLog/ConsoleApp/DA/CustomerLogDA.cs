using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;
using MktToolMgmt.PromotionCustomerLogApp.Entities;

namespace MktToolMgmt.PromotionCustomerLogApp.DA
{
    public static class CustomerLogDA
    {
        private static void SetCommand(DataCommand command, CustomerLogEntity entity)
        {
            command.SetParameterValue("ActionName", entity.ActionName);
            command.SetParameterValue("CustomerSysNo", entity.CustomerSysNo);
            command.SetParameterValue("GetPromotionDate", entity.GetPromotionDate);
            command.SetParameterValue("IsBinding", entity.IsBinding);
            command.SetParameterValue("PromotionAmountType", entity.PromotionAmountType);
            command.SetParameterValue("PromotionCode", entity.PromotionCode);
            command.SetParameterValue("PromotionSysNo", entity.PromotionSysNo);
            command.SetParameterValue("SoSysNo", entity.SoSysNo);
            command.SetParameterValue("Status", entity.Status);
            command.SetParameterValue("CompanyCode", ConstValues.CompanyCode);
            command.SetParameterValue("StoreCompanyCode", ConstValues.StoreCompanyCode);
            command.SetParameterValue("LanguageCode", ConstValues.LanguageCode);
            command.SetParameterValue("SysNo", entity.SysNo);
        }

        public static void BatchInsert(CustomerLogEntity log, List<PromotionCodeEntity> codes)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Promotion_InsertCustomerLog");
            SetCommand(command, log);
            foreach (PromotionCodeEntity code in codes)
            {
                command.SetParameterValue("PromotionAmountType", Convert.ToInt32(code.PromotionValue));
                command.SetParameterValue("PromotionCode", code.PromotionCode);
                for (int i = 0; i < code.CustomerTotalCount; i++)
                {
                    command.ExecuteNonQuery();
                }
            }
        }

    }
}
