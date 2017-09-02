using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;
using MktToolMgmt.PromotionCustomerLogApp.Entities;

namespace MktToolMgmt.PromotionCustomerLogApp.DA
{
    public static class PromotionCodeDA
    {
        public static List<PromotionCodeEntity> QueryMulti(SettingCustomerEntity query)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Promotion_QueryMultiPromotionCode");
            command.SetParameterValue("PromotionSysNo", query.PromotionSysNo);
            command.SetParameterValue("CompanyCode", ConstValues.CompanyCode);
            command.SetParameterValue("Top", query.CustomerCount);
            return command.ExecuteEntityList<PromotionCodeEntity>();
        }

        public static List<PromotionCodeEntity> QuerySingle(SettingCustomerEntity query)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Promotion_QuerySinglePromotionCode");
            command.SetParameterValue("PromotionSysNo", query.PromotionSysNo);
            command.SetParameterValue("CompanyCode", ConstValues.CompanyCode);
            return command.ExecuteEntityList<PromotionCodeEntity>();
        }
    }
}
