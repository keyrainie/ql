using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Newegg.Oversea.Framework.DataAccess;
using MktToolMgmt.PromotionCustomerLogApp.Entities;

namespace MktToolMgmt.PromotionCustomerLogApp.DA
{
    public static class LimitCustomerDA
    {
        public static List<CustomerResultEntity> GetLimitCustomer(SettingCustomerEntity setting)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Promotion_GetLimitCustomer");
            command.SetParameterValue("@CompanyCode", ConstValues.CompanyCode);
            command.SetParameterValue("@PromotionSysNo", setting.PromotionSysNo);
            List<CustomerResultEntity> customerList = command.ExecuteEntityList<CustomerResultEntity>();         
            return customerList;
        }
    }
}
