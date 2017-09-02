using Nesoft.Job.WMS.Common.Entity;
using Nesoft.Utility;
using Nesoft.Utility.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Nesoft.Job.WMS.SendPOToDataExchangeCenter
{
    public static class PODA
    {
        /// <summary>
        /// 查询入库单信息
        /// </summary>
        /// <returns></returns>
        public static List<POInfo> QueryPO()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("QueryPO");
            List<POInfo> list = new List<POInfo>();
            DataSet ds = cmd.ExecuteDataSet();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                POInfo poInfo = DataMapper.GetEntity<POInfo>(dr);
                if (poInfo.Items == null) poInfo.Items = new List<POItemInfo>();
                foreach (DataRow itemDR in ds.Tables[1].Rows)
                {
                    if (itemDR["POID"].ToString() == poInfo.BillNo)
                    {
                        poInfo.Items.Add(DataMapper.GetEntity<POItemInfo>(itemDR));
                    }
                }
                list.Add(poInfo);
            }

            return list;
        }
        /// <summary>
        /// 更新入库单状态
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public static bool UpdatePOStatus(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdatePOStatus");
            cmd.SetParameterValue("@SysNo", sysNo);
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}
