using Nesoft.Job.WMS.Common.Entity;
using Nesoft.Utility;
using Nesoft.Utility.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Nesoft.Job.WMS.SendAdjustToDataExchangeCenter
{
    public static class AdjustDA
    {
        /// <summary>
        /// 查询损益单
        /// </summary>
        /// <returns></returns>
        public static List<AdjustInfo> QueryAdjust()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("QueryAdjust");
            return cmd.ExecuteEntityList<AdjustInfo>();
            //List<AdjustInfo> list = new List<AdjustInfo>();
            //DataSet ds = cmd.ExecuteDataSet();

            //foreach (DataRow dr in ds.Tables[0].Rows)
            //{
            //    AdjustInfo adjustInfo = DataMapper.GetEntity<AdjustInfo>(dr);
            //    if (adjustInfo.Items == null) adjustInfo.Items = new List<AdjustItemInfo>();
            //    foreach (DataRow itemDR in ds.Tables[1].Rows)
            //    {
            //        int adjustSysNo = 0;
            //        int.TryParse(itemDR["AdjustSysNo"].ToString(), out adjustSysNo);
            //        if (adjustSysNo == adjustInfo.SysNo)
            //        {
            //            adjustInfo.Items.Add(DataMapper.GetEntity<AdjustItemInfo>(itemDR));
            //        }
            //    }
            //    list.Add(adjustInfo);
            //}

            //return list;
        }
        /// <summary>
        /// 更新损益单状态
        /// </summary>
        /// <param name="adjustID"></param>
        /// <returns></returns>
        public static bool UpdateAdjustStatus(int adjustID)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateAdjustStatus");
            cmd.SetParameterValue("@SysNo", adjustID);
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}
