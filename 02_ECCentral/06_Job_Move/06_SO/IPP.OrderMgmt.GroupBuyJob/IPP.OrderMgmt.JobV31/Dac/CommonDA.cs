using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;
using IPP.OrderMgmt.JobV31.BusinessEntities;

namespace IPP.OrderMgmt.JobV31.Dac
{
    public class CommonDA
    {
        public static int WriteProcessLog(int sysNumber,string type,string memo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("WriteProcessLog");

            command.SetParameterValue("@Number", sysNumber);
            command.SetParameterValue("@Type", type);
            command.SetParameterValue("@Memo", memo);

            return command.ExecuteNonQuery();
        }

        public static bool ExistsNetPay(int soSysNo,string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("ExistsNetPay");

            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@CompanyCode", companyCode);

            object o= command.ExecuteScalar();

            if (o == null)
                return false;
            else
                return true;
        }

        public static int ChangeGroupBuySettlement(int groupBuyingSysNo, string isSettlement, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("ChangeGroupBuySettlement");

            command.SetParameterValue("@GroupBuyingSysNo", groupBuyingSysNo);
            command.SetParameterValue("@IsSettlement",isSettlement);
            command.SetParameterValue("@CompanyCode",companyCode);

            return command.ExecuteNonQuery();

        }


        public static CustomerInfo GetCustomerBySysNo(int customerSysno, string companyCode)
        {
            CustomerInfo entity = null;
            DataCommand command = DataCommandManager.GetDataCommand("GetCustomerBySysNo");

            command.SetParameterValue("@CompanyCode", companyCode);
            command.SetParameterValue("@SysNo", customerSysno);

            entity = command.ExecuteEntity<CustomerInfo>();
            return entity;

        }

        public static int GetLowerLimitSellCount(int groupSysNo)
        {

            DataCommand command = DataCommandManager.GetDataCommand("GetLowerLimitSellCount");

            command.SetParameterValue("@GroupBuyingSysNo", groupSysNo);

            object o= command.ExecuteScalar();

            if(o==null)
                return 0;
            else
                return (int)o;
             

        }

    }
}
