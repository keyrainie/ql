using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.BizEntity.ExternalSYS;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.ThirdPart.SqlDataAccess
{
    public class ERPItemInfoDA
    {
        /// <summary>
        /// 记录库存调整日志
        /// </summary>
        /// <param name="adjustInfo"></param>
        /// <returns></returns>
        public void AddERPInventoryAdjustLog(ERPInventoryAdjustInfo adjustInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ERP_AddERPInventoryAdjustLog");
            foreach (ERPItemInventoryInfo item in adjustInfo.AdjustItemList)
            {
                cmd.SetParameterValue("@ProductSysNo", item.ProductSysNo);
                cmd.SetParameterValue("@ERPProductID", item.ERPProductId);
                cmd.SetParameterValue("@B2CSalesQty", item.B2CSalesQuantity);
                cmd.SetParameterValue("@HQQty", item.HQQuantity);
                cmd.SetParameterValue("@DeptQty", item.DeptQuantity);
                cmd.SetParameterValue("@OrderType", adjustInfo.OrderType);
                cmd.SetParameterValue("@OrderSysNo", adjustInfo.OrderSysNo);
                cmd.SetParameterValue("@Memo", adjustInfo.Memo);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 库存调整
        /// </summary>
        /// <param name="adjustInfo"></param>
        /// <returns></returns>
        public void AdjustERPItemInventory(ERPInventoryAdjustInfo adjustInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ERP_AdjustERPItemInventory");
            foreach (ERPItemInventoryInfo item in adjustInfo.AdjustItemList)
            {
                cmd.SetParameterValue("@ProductSysNo", item.ProductSysNo);
                cmd.SetParameterValue("@ERPProductId", item.ERPProductId);
                cmd.SetParameterValue("@B2CSalesQty", item.B2CSalesQuantity);
                cmd.SetParameterValue("@HQQty", item.HQQuantity);
                cmd.SetParameterValue("@DeptQty", item.DeptQuantity);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 库存查询
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public ERPItemInventoryInfo GetERPItemInventoryByProductSysNo(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ERP_GetERPItemInventoryByProductSysNo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            return cmd.ExecuteEntity<ERPItemInventoryInfo>();
        }

        /// <summary>
        /// 库存查询
        /// </summary>
        /// <param name="erpProductID"></param>
        /// <returns></returns>
        public ERPItemInventoryInfo GetERPItemInventoryByERPProductID(int erpProductID)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ERP_GetERPItemInventoryByERPProductID");
            cmd.SetParameterValue("@EPRProductID", erpProductID);
            return cmd.ExecuteEntity<ERPItemInventoryInfo>();
        }
    }
}
