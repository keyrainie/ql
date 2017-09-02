using Nesoft.Job.WMS.Common.Entity;
using Nesoft.Utility;
using Nesoft.Utility.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Nesoft.Job.WMS.SendSOToDataExchangeCenter
{
    public static class SODA
    {
        /// <summary>
        /// 查询订单
        /// </summary>
        /// <returns></returns>
        public static List<SOInfo> QuerySO()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("QuerySO");
            List<SOInfo> list = new List<SOInfo>();
            DataSet ds = cmd.ExecuteDataSet();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                SOInfo soInfo = DataMapper.GetEntity<SOInfo>(dr);
                if (soInfo.OrderItems == null) soInfo.OrderItems = new List<SOItemInfo>();
                foreach (DataRow itemDR in ds.Tables[1].Rows)
                {
                    int soSysNo = 0;
                    int.TryParse(itemDR["SOSysNo"].ToString(), out soSysNo);
                    if (soSysNo == soInfo.SysNo)
                    {
                        soInfo.OrderItems.Add(DataMapper.GetEntity<SOItemInfo>(itemDR));
                    }
                }
                list.Add(soInfo);
            }

            return list;
        }
        /// <summary>
        /// 更新订单出关状态
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public static bool UpdateSOStatus(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateSOCustomsStatus");
            cmd.SetParameterValue("@SysNo", sysNo);
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}
