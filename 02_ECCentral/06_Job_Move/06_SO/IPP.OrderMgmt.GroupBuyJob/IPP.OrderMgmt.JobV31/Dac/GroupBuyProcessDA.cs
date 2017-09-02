using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.OrderMgmt.JobV31.BusinessEntities;
using Newegg.Oversea.Framework.DataAccess;
using System.Data;

namespace IPP.OrderMgmt.JobV31.Dac
{
    public class GroupBuyProcessDA
    {


        /// <summary>
        /// 获取团购商品信息
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static List<GroupBuyItemEntity> GetGroupBuyItems(int groupBuySysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetGroupBuyItems");

            command.SetParameterValue("@ReferenceSysNo", groupBuySysNo);

            return command.ExecuteEntityList<GroupBuyItemEntity>();
        }



        public static bool IsItemAllSettled(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("IsItemAllSettled");

            command.SetParameterValue("@SOSysNo", soSysNo);

            object o= command.ExecuteScalar();

            if (o == null||Convert.IsDBNull(o))
                return true;
            else
                return false;
        }


        public static int UpdateItemSettlementStatus(int sysNo, string settlementStatus)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateItemSettlementStatus");

            command.SetParameterValue("@SysNo", sysNo);
            command.SetParameterValue("@SettlementStatus", settlementStatus);

            return command.ExecuteNonQuery();
 
        }


        public static bool IsPartlyFail(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("IsPartlyFail");

            command.SetParameterValue("@SOSysNo", soSysNo);

            object o= command.ExecuteScalar();

            if (o == null||Convert.IsDBNull(o))
                return false;
            else
                return true;
        }

        public static bool IsOnlyHaveFailItem(int soSysNo,int productSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("IsOnlyHaveFailItem");

            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@ProductSysNo", productSysNo);

            object o = command.ExecuteScalar();

            if (o == null || Convert.IsDBNull(o))
                return false;
            else
                return true;
        }

        public static List<int> GetSOSysNosNeedMark()
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetSOSysNosNeedMark");

            IDataReader reader = command.ExecuteDataReader();

            List<int> result = new List<int>();
            IDataReader dr = command.ExecuteDataReader();
            while (dr.Read())
            {
                result.Add(Convert.ToInt32(dr[0]));
            }
            dr.Close();
            return result;
 
        }


        public static List<GroupBuyItemEntity> GetGroupBuyItemBySOSysNo(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetGroupBuyItemBySOSysNo");

            command.SetParameterValue("@SOSysNo", soSysNo);

            return command.ExecuteEntityList<GroupBuyItemEntity>();

        }

        
    }
}
