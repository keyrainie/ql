using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;
using SendMailARAmtOfVIPCustomer.Biz.Entities;
using System.Data;
using SendMailARAmtOfVIPCustomer.Biz.Common;

namespace SendMailARAmtOfVIPCustomer.Biz.DataAccess
{
    public class SendMailARAmtOfVIPCustomerDA
    {
        /// <summary>
        /// 获取VIP客户应收未收款明细统计结果
        /// </summary>
        /// <returns></returns>
        public static List<ARAmtOfVIPCustomerEntity> GetARAmtOfVIPCustomerData()
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetARAmtOfVIPCustomer");
            command.AddInputParameter("@CompanyCode", DbType.AnsiStringFixedLength, JobConfig.CompanyCode);

            var result = command.ExecuteEntityList<ARAmtOfVIPCustomerEntity>();

            return result;
        }
    }
}
