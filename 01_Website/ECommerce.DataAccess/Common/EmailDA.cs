using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity;
using ECommerce.Entity.Common;
using ECommerce.Utility.DataAccess;

namespace ECommerce.DataAccess.Common
{
    public class EmailDA
    {
        /// <summary>
        /// 发送邮件，写到数据库
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool SendEmail(AsyncEmail item)
        {
            item.Priority = item.Priority ?? 0;
            item.LanguageCode = ConstValue.LanguageCode;
            item.CompanyCode = ConstValue.CompanyCode;
            item.StoreCompanyCode = ConstValue.StoreCompanyCode;
            DataCommand cmd = DataCommandManager.GetDataCommand("Email_SendEmail");
            cmd.SetParameterValue<AsyncEmail>(item);
            return cmd.ExecuteNonQuery() > 0 ? true : false;
        }
    }
}
