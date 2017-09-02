using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Serialization;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.SO.SqlDataAccess
{
    public partial class SODA : ISODA
    {
        #region Job 联通合约机相关
        /// <summary>
        /// 取得待审核的联通结算订单编号
        /// </summary>
        /// <returns></returns>
        public List<int> GetStatusIsOriginalBuyMobileSettlementSOSysNo()
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_BuyMobileSettlementNeedAuditSysNo");
            DataTable dt = command.ExecuteDataTable();
            List<int> soSysNoList = new List<int>();
            if (dt != null)
            {
                using (dt)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr[0] != DBNull.Value)
                        {
                            soSysNoList.Add((int)dr[0]);
                        }
                    }
                } 
            }
            return soSysNoList;
        }



        /// <summary>
        /// 取得状态为已完成的联通合约机订单编号
        /// </summary>
        /// <returns></returns>
        public List<int> GetStatusIsCompleteUnicomFreeBuySOSysNo()
        {
            DataCommand command = DataCommandManager.GetDataCommand("SO_Get_UnicomFreeBuySOBySysNo");
            DataTable dt = command.ExecuteDataTable();
            List<int> soSysNoList = new List<int>();
            if (dt != null)
            {
                using (dt)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr[0] != DBNull.Value)
                        {
                            soSysNoList.Add((int)dr[0]);
                        }
                    }
                }
            }
            return soSysNoList;
        }
        #endregion
         
    }
}

