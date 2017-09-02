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
    [VersionExport(typeof(IStockDA))]
    public class StockDA : IStockDA
    {
        /// <summary>
        /// 生成一个新的仓库编号
        /// </summary>
        /// <returns></returns>
        public int NewStockSysNo()
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_Insert_NewStockSysNo");
            return dc.ExecuteScalar<int>();

        }
        #region 渠道仓库

        /// <summary>
        /// 根据SysNo获取渠道仓库信息
        /// </summary>
        /// <param name="stockSysNo"></param>
        /// <returns></returns>
        [Caching(new string[] { "stockSysNo" }, ExpiryType = ExpirationType.SlidingTime, ExpireTime = "02:00:00")]
        public virtual StockInfo GetStockInfoBySysNo(int stockSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_GetStockInfoBySysNo");
            dc.SetParameterValue("@SysNo", stockSysNo);
            return dc.ExecuteEntity<StockInfo>();
        }

        /// <summary>
        /// 根据渠道仓库编号获取渠道仓库信息
        /// </summary>
        /// <param name="stockSysNo"></param>
        /// <returns></returns>
        public virtual StockInfo GetStockInfoByStockID(string stockID)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Inventory_GetStockInfoByID");
            dc.SetParameterValue("@StockID", stockID);
            return dc.ExecuteEntity<StockInfo>();
        }

        /// <summary>
        /// 创建渠道仓库信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual StockInfo CreateStock(StockInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Inventory_CreateStock");
            entity.SysNo = NewStockSysNo();
            command.SetParameterValue(entity);
            command.ExecuteNonQuery();
            return entity;
        }

        /// <summary>
        /// 更新渠道仓库信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual StockInfo UpdateStock(StockInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Inventory_UpdateStockInfo");
            command.SetParameterValue(entity);
            command.ExecuteNonQuery();
            return entity;
        }


        /// <summary>
        /// 获取渠道仓库列表
        /// </summary>
        /// <param name="CompanyCode"></param>
        /// <returns>渠道仓库列表</returns>
        public virtual List<StockInfo> QueryStockList(StockQueryFilter request)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_QueryStockList");
            cmd.SetParameterValue("@WebChannelSysNo", request.WebChannelID);
            cmd.SetParameterValue("@WarehouseSysNo", request.WarehouseSysNo);
            cmd.SetParameterValue("@CompanyCode", request.CompanyCode);

            using (IDataReader reader = cmd.ExecuteDataReader())
            {
                return DataMapper.GetEntityList<StockInfo, List<StockInfo>>(reader);
            }
        }
        /// <summary>
        /// 获取所有渠道仓库列表
        /// </summary>
        /// <param name="CompanyCode"></param>
        /// <returns>渠道仓库列表</returns>
        public virtual List<StockInfo> QueryStockAll()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_QueryStockAll");            

            using (IDataReader reader = cmd.ExecuteDataReader())
            {
                return DataMapper.GetEntityList<StockInfo, List<StockInfo>>(reader);
            }
        }
        /// <summary>
        /// 根据销售渠道编号获取渠道仓库列表
        /// </summary>
        /// <param name="webChannelID"></param>
        /// <returns>渠道仓库列表</returns>
        public virtual List<StockInfo> QueryStockListByWebChannelID(string webChannelID)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_QueryStockListByWebChannelID");
            cmd.SetParameterValue("@WebChannelID", webChannelID);
            cmd.SetParameterValue("@CompanyCode", "8601");//[Mark][Alan.X.Luo 硬编码]
            using (IDataReader reader = cmd.ExecuteDataReader())
            {
                return DataMapper.GetEntityList<StockInfo, List<StockInfo>>(reader);
            }
        }

        /// <summary>
        /// 根据销售渠道编号获取渠道仓库列表
        /// </summary>
        /// <param name="merchantSysNo">商家</param>
        /// <param name="webChannelID"></param>
        /// <returns>渠道仓库列表</returns>
        public virtual List<StockInfo> QueryStockListByWebChannelIDAndMerchantSysNo(int? merchantSysNo,string webChannelID)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_QueryStockListByMerchantSysNo");
            cmd.SetParameterValue("@MerchantSysNo", merchantSysNo);
            cmd.SetParameterValue("@WebChannelID", webChannelID);
            cmd.SetParameterValue("@CompanyCode", "8601");//[Mark][Alan.X.Luo 硬编码]
            using (IDataReader reader = cmd.ExecuteDataReader())
            {
                return DataMapper.GetEntityList<StockInfo, List<StockInfo>>(reader);
            }
        }
        #endregion

    }
}
