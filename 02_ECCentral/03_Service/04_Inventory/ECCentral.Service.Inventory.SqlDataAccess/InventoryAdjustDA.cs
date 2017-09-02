using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;
using ECCentral.QueryFilter.Inventory;

using ECCentral.Service.Inventory.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Inventory.SqlDataAccess
{
    [VersionExport(typeof(IInventoryAdjustDA))]
    public class InventoryAdjustDA : IInventoryAdjustDA
    {
        #region IInventoryAdjustDA Members
        
        #region 商品批号相关方法

        /// <summary>
        /// 根据单据编号 获取批号信息表信息
        /// </summary>
        /// <param name="DocumentNumber"></param>
        /// <returns></returns>
        public List<InventoryBatchDetailsInfo> GetBatchDetailsInfoEntityListByNumber(int DocumentNumber)
        {
            var command = DataCommandManager.GetDataCommand("GetBatchDetailsInfoEntityListByNumber");
            command.SetParameterValue("@Number", DocumentNumber);
            return command.ExecuteEntityList<InventoryBatchDetailsInfo>();
        }

        /// <summary>
        /// 单据作废 取消作废 出库时执行此方法
        /// </summary>
        /// <param name="paramXml"></param>
        /// <returns></returns>  
        public int AdjustBatchNumberInventory(string paramXml)
        {
            DataCommand command = DataCommandManager.GetDataCommand("AdjustBatchNumberInventory");
            command.SetParameterValue("@Msg", paramXml);
            return command.ExecuteNonQuery();
        }

        public void UpdateSTBInfo(List<InventoryBatchDetailsInfo> model, int documentNumber, string strType, string action)
        {            
            string sqlCommandString = "UpdateQuantityOfSTB";
            if (strType == "LD" && action == "Return")//借货单归还 需要更新ReturnQty 和 Quantity
            {
                sqlCommandString = "ReturnItemUpdateQuantityOfSTB";
            }
            var dataCommand = DataCommandManager.GetDataCommand(sqlCommandString);
            
            if (model != null && model.Count > 0)
            {
                foreach (var item in model)
                {
                    dataCommand.SetParameterValue("@BatchNumber", item.BatchNumber);
                    dataCommand.SetParameterValue("@Type", strType);
                    dataCommand.SetParameterValue("@ProductSysNo", item.ProductSysNo);
                    dataCommand.SetParameterValue("@Quantity", item.Quantity);
                    dataCommand.SetParameterValue("@ReturnQty", item.ReturnQty);
                    dataCommand.SetParameterValue("@StockSysNo", item.StockSysNo);
                    dataCommand.SetParameterValueAsCurrentUserAcct("@InUser");
                    dataCommand.SetParameterValue("@Number", documentNumber);
                    dataCommand.ExecuteNonQuery();                    
                }
            }            
        }

        /// <summary>
        /// 在ST表中删除 单据中已删除的批次商品
        /// </summary>
        /// <param name="productSysNo">商品ID</param>
        /// <param name="documentNumber">单据编号</param>
        /// <returns></returns>
        public void DeleteBatchItemOfSTB(int productSysNo, int documentNumber)
        {
            var dataCommand = DataCommandManager.GetDataCommand("DeleteBatchItemOfSTB");
            dataCommand.SetParameterValue("@ProductSysNo", productSysNo);
            dataCommand.SetParameterValue("@Number",documentNumber);
            dataCommand.ExecuteNonQuery();
        }

        public void SourceUpdateSTBInfo(List<InventoryBatchDetailsInfo> model, int documentNumber, string strType)
        {
            var dataCommand = DataCommandManager.GetDataCommand("SourceUpdateSTBInfo");            
            if (model != null && model.Count > 0)
            {
                foreach (var item in model)
                {
                    dataCommand.SetParameterValue("@BatchNumber", item.BatchNumber);
                    dataCommand.SetParameterValue("@Type", strType);
                    dataCommand.SetParameterValue("@ProductSysNo", item.ProductSysNo);
                    dataCommand.SetParameterValue("@Quantity", item.Quantity);
                    dataCommand.SetParameterValue("@ReturnQty", item.ReturnQty);
                    dataCommand.SetParameterValue("@StockSysNo", item.StockSysNo);
                    dataCommand.SetParameterValueAsCurrentUserAcct("@InUser");
                    dataCommand.SetParameterValue("@Number", documentNumber);
                    dataCommand.ExecuteNonQuery();                                            
                }
            }            
        }

        /// <summary>
        /// 转换单目标商品 创建或更新（ 去分 源商品和目标商品的标志位 ：  目标商品 值存于 ReturnQty 字段中  源商品 值存于 Quantity字段中）
        /// </summary>
        /// <param name="model">目标商品</param>
        /// <param name="InUser"></param>
        /// <param name="DocumentNumber"></param>
        /// <param name="Type"></param>
        /// <param name="Action"></param>
        /// <returns></returns>
        public void TargetUpdateSTBInfo(List<InventoryBatchDetailsInfo> model, int documentNumber, string strType)
        {
            var dataCommand = DataCommandManager.GetDataCommand("TargetUpdateSTBInfo");            
            if (model != null && model.Count > 0)
            {
                foreach (var item in model)
                {
                    dataCommand.SetParameterValue("@BatchNumber", item.BatchNumber);
                    dataCommand.SetParameterValue("@Type", strType);
                    dataCommand.SetParameterValue("@ProductSysNo", item.ProductSysNo);
                    dataCommand.SetParameterValue("@Quantity", item.Quantity);
                    dataCommand.SetParameterValue("@ReturnQty", item.ReturnQty);
                    dataCommand.SetParameterValue("@StockSysNo", item.StockSysNo);
                    dataCommand.SetParameterValueAsCurrentUserAcct("@InUser");
                    dataCommand.SetParameterValue("@Number", Convert.ToInt32(documentNumber));
                    dataCommand.ExecuteNonQuery();                    
                }
            }            
        }

        /// <summary>
        /// 在ST表中删除 单据中已删除的批次商品
        /// </summary>
        /// <param name="ProductID">商品ID</param>
        /// <param name="documentNumber">单据编号</param>
        /// <returns></returns>
        public void DeleteAllBatchItem(int documentNumber, string deleteType)
        {
            string sqlCommandString = "DeleteAllBatchItem";
            if (!string.IsNullOrEmpty(deleteType))
            {
                sqlCommandString = deleteType;
            }
            var dataCommand = DataCommandManager.GetDataCommand(sqlCommandString);
            dataCommand.SetParameterValue("@Number", documentNumber);
            dataCommand.ExecuteNonQuery();
        }
        #endregion 商品批号相关

        #endregion
    }
}
