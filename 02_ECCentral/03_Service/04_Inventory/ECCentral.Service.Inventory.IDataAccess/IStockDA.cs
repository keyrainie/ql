using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;
using ECCentral.QueryFilter.Inventory;

namespace ECCentral.Service.Inventory.IDataAccess
{
    public interface IStockDA
    {
        #region 渠道仓库

        /// <summary>
        /// 根据SysNo获取渠道仓库信息
        /// </summary>
        /// <param name="stockSysNo"></param>
        /// <returns></returns>
        StockInfo GetStockInfoBySysNo(int stockSysNo);

        /// <summary>
        /// 创建渠道仓库信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        StockInfo CreateStock(StockInfo entity);

        /// <summary>
        /// 更新渠道仓库信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        StockInfo UpdateStock(StockInfo entity);

        /// <summary>
        /// 获取渠道仓库列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        List<StockInfo> QueryStockList(StockQueryFilter request);
            
        /// <summary>
        /// 根据销售渠道编号获取渠道仓库列表
        /// </summary>
        /// <param name="webChannelID"></param>
        /// <returns>渠道仓库列表</returns>
        List<StockInfo> QueryStockListByWebChannelID(string webChannelID);

        List<StockInfo> QueryStockAll();

        List<StockInfo> QueryStockListByWebChannelIDAndMerchantSysNo(int? merchantSysNo, string webChannelID);

        #endregion       

    }
}
