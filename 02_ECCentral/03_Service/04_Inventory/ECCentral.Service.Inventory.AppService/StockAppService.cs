using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;
using ECCentral.Service.Inventory.BizProcessor;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Inventory.AppService
{
    [VersionExport(typeof(StockAppService))]
    public class StockAppService
    {
        private StockProcessor _stockProcessor;
        private StockProcessor StockProcessor
        {
            get
            {
                _stockProcessor = _stockProcessor ?? ObjectFactory<StockProcessor>.Instance;
                return _stockProcessor;
            }
        }

        #region 仓库(Warehouse)相关

        /// <summary>
        /// 创建仓库
        /// </summary>
        /// <param name="warehouseInfo"></param>
        /// <returns></returns>
        public virtual WarehouseInfo CreateWarehouse(WarehouseInfo warehouseInfo)
        {
            return StockProcessor.CreateWarehouse(warehouseInfo);
        }

        /// <summary>
        /// 更新仓库信息
        /// </summary>
        /// <param name="warehouseInfo"></param>
        /// <returns></returns>
        public virtual WarehouseInfo UpdateWarehouse(WarehouseInfo warehouseInfo)
        {
            return StockProcessor.UpdateWarehouse(warehouseInfo);
        }

        /// <summary>
        /// 获取仓库信息
        /// </summary>
        /// <param name="warehouseSysNo"></param>
        /// <returns></returns>
        public virtual WarehouseInfo GetWarehouseInfo(int warehouseSysNo)
        {
            return StockProcessor.GetWarehouseInfo(warehouseSysNo);
        }

        /// <summary>
        /// 获取仓库列表
        /// </summary>      
        /// <param name="companyCode">公司编号</param>  
        /// <returns>仓库列表</returns>
        public virtual List<WarehouseInfo> GetWarehouseList(string companyCode)
        {
            return StockProcessor.GetWarehouseListByCompanyCode(companyCode);
        }

        /// <summary>
        /// 获取本地仓库编码
        /// </summary>
        /// <param name="areaSysNo">地区编号</param>
        /// <param name="companyCode">公司编码</param>
        /// <returns>本地仓库编码</returns>
        public virtual string GetLocalWarehouseNumber(int areaSysNo, string companyCode)
        {
            return StockProcessor.GetlocalWH(areaSysNo, companyCode);
        }

        #endregion

        #region 渠道仓库(Stock)相关

        /// <summary>
        /// 创建渠道仓库
        /// </summary>
        /// <param name="stockInfo"></param>
        /// <returns></returns>
        public virtual StockInfo CreateStock(StockInfo stockInfo)
        {
            return StockProcessor.CreateStock(stockInfo);
        }

        /// <summary>
        /// 更新渠道仓库
        /// </summary>
        /// <param name="stockInfo"></param>
        /// <returns></returns>
        public virtual StockInfo UpdateStock(StockInfo stockInfo)
        {
            return StockProcessor.UpdateStock(stockInfo);
        }

        /// <summary>
        /// 获取渠道仓库信息
        /// </summary>
        /// <param name="stockSysNo"></param>
        /// <returns></returns>
        public virtual StockInfo GetStockInfo(int stockSysNo)
        {
            return StockProcessor.GetStockInfo(stockSysNo);
        }

        /// <summary>
        /// 获取渠道仓库列表
        /// </summary>
        /// <param name="stockSysNo"></param>
        /// <returns></returns>
        public virtual List<StockInfo> GetStockList(string webChannelID)
        {
            return StockProcessor.GetStockList(webChannelID);
        }

        #endregion

        #region 仓库所有者(WarehouseOwner)业务处理方法

        /// <summary>
        /// 创建仓库所有者
        /// </summary>
        /// <param name="warehouseOwnerInfo"></param>
        /// <returns></returns>
        public virtual WarehouseOwnerInfo CreateWarehouseOwner(WarehouseOwnerInfo warehouseOwnerInfo)
        {
            return StockProcessor.CreateWarehouseOwner(warehouseOwnerInfo);
        }

        /// <summary>
        /// 更新仓库所有者信息
        /// </summary>
        /// <param name="warehouseInfo"></param>
        /// <returns></returns>
        public virtual WarehouseOwnerInfo UpdateWarehouseOwner(WarehouseOwnerInfo warehouseOwnerInfo)
        {
            return StockProcessor.UpdateWarehouseOwner(warehouseOwnerInfo);
        }

        /// <summary>
        /// 获取仓库所有者信息
        /// </summary>
        /// <param name="warehouseSysNo"></param>
        /// <returns></returns>
        public virtual WarehouseOwnerInfo GetWarehouseOwnerInfo(int warehouseOwnerSysNo)
        {
            return StockProcessor.GetWarehouseOwnerInfo(warehouseOwnerSysNo);
        }
        /// <summary>
        /// 根据公司编号取得仓库所有者列表
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public List<WarehouseOwnerInfo> GetWarehouseOwnerInfoByCompanyCode(string companyCode)
        {
            return StockProcessor.GetWarehouseOwnerInfoByCompanyCode(companyCode);
        }
        #endregion
    }
}
