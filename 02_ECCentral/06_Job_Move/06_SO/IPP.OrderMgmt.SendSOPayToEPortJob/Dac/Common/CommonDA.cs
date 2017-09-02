using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using SendSOPayToEPortJob.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPP.OrderMgmt.SendSOPayToEPortJob.Dac.Common
{
    public class CommonDA
    {
        /// <summary>
        /// 获取某个电子口岸的所有符合的订单
        /// </summary>
        /// <param name="ePortSysNo">电子口岸编号</param>
        /// <returns></returns>
        public static List<int> GetOrderSysNoList(int ePortSysNo)
        {
            List<int> result = new List<int>();
            DataCommand command = DataCommandManager.GetDataCommand("GetOrderSysNoList");
            command.SetParameterValue("@ePortSysNo", ePortSysNo);
            DataSet dsResult = command.ExecuteDataSet();
            if (dsResult != null && dsResult.Tables.Count > 0)
            {
                DataTable reviesTable = dsResult.Tables[0];
                if (reviesTable.Rows != null && reviesTable.Rows.Count > 0)
                {
                    foreach (DataRow dr in reviesTable.Rows)
                    {
                        result.Add(int.Parse(dr[0].ToString()));
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 获取SO详细信息
        /// </summary>
        /// <param name="sosysno">SO#</param>
        /// <returns>订单详细信息</returns>
        public static OrderInfo GetCenterSODetailInfo(int soSysNo)
        {
            OrderInfo orderInfo = null;
            DataCommand command = DataCommandManager.GetDataCommand("GetOrderInfoList");
            command.SetParameterValue("@SOID", soSysNo);

            DataSet result = command.ExecuteDataSet();

            if (result != null && result.Tables.Count > 0)
            {
                DataTable masterTable = result.Tables[0];

                if (masterTable.Rows != null && masterTable.Rows.Count > 0)
                {
                    orderInfo = DataMapper.GetEntity<OrderInfo>(masterTable.Rows[0]);
                }
                if (result.Tables != null && result.Tables.Count > 1)
                {
                    DataTable itemTable = result.Tables[1];
                    if (itemTable.Rows != null && itemTable.Rows.Count > 0 && orderInfo != null)
                    {
                        orderInfo.SOItemList = DataMapper.GetEntityList<SOItemInfo, List<SOItemInfo>>(itemTable.Rows);
                    }
                }
                if (result.Tables != null && result.Tables.Count > 1 && result.Tables[2].Rows.Count > 0)
                {
                    DataTable dtDetail = result.Tables[3];
                    List<ECCentral.BizEntity.SO.SOPromotionInfo> promotionList = DataMapper.GetEntityList<ECCentral.BizEntity.SO.SOPromotionInfo, List<ECCentral.BizEntity.SO.SOPromotionInfo>>(result.Tables[2].Rows,
                    (dr, p) =>
                    {
                        p.PromotionType = ECCentral.BizEntity.SO.SOPromotionType.Combo;
                        dtDetail.DefaultView.RowFilter = String.Format("PromotionSysNo={0}", p.PromotionSysNo);
                        p.SOPromotionDetails = DataMapper.GetEntityList<ECCentral.BizEntity.SO.SOPromotionDetailInfo, List<ECCentral.BizEntity.SO.SOPromotionDetailInfo>>(dtDetail.DefaultView.ToTable().Rows);
                    });
                    if (promotionList.Count > 0)
                    {
                        orderInfo.SOPromotions = promotionList;
                    }
                }
            }

            return orderInfo;
        }

        /// <summary>
        /// 推送成功后修改订单状态
        /// </summary>
        /// <param name="soSysNo"></param>
        public static void UpdateOrderStatus(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateOrderStatus");
            command.SetParameterValue("@SysNo", soSysNo);
            command.ExecuteNonQuery();
        }
    }
}
