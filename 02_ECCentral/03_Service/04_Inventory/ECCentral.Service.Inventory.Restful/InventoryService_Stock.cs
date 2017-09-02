using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

using ECCentral.BizEntity.Inventory;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Service.Inventory.AppService;
using ECCentral.Service.Inventory.IDataAccess;
using ECCentral.Service.Inventory.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.Inventory.Restful
{
    public partial class InventoryService
    {
        #region Query

        /// <summary>
        /// 查询渠道仓库列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Stock/QueryStock", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryStock(StockQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<IStockQueryDA>.Instance.QueryStock(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }
        [WebInvoke(UriTemplate = "/Stock/QueryStockAll", Method = "GET")]
        [DataTableSerializeOperationBehavior]
        public virtual List<StockInfo> QueryStockAll()
        {
            return ObjectFactory<IStockDA>.Instance.QueryStockAll();
        }
        /// <summary>
        /// 查询渠道仓库列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Stock/QueryStockList", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public List<StockInfo> QueryStockList(StockQueryFilter request)
        {
            return ObjectFactory<IStockDA>.Instance.QueryStockList(request);
        }

        /// <summary>
        /// 查询渠道仓库列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Stock/QueryStockListByWebChannelID", Method = "POST")]
        public List<StockInfo> QueryStockListByWebChannelID(string webChannelID)
        {
            return ObjectFactory<IStockDA>.Instance.QueryStockListByWebChannelID(webChannelID);
        }


        /// <summary>
        /// 查询仓库列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Stock/QueryWarehouse", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryWarehouse(WarehouseQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<IWarehouseQueryDA>.Instance.QueryWarehouse(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 查询仓库所有者
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Stock/QueryWarehouseOwner", Method = "POST")]
        [DataTableSerializeOperationBehavior]
        public QueryResult QueryWarehouseOwner(WarehouseOwnerQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<IWarehouseOwnerQueryDA>.Instance.QueryWarehouseOwner(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        #endregion Query

        private StockAppService _stockAppService;
        private StockAppService StockAppService
        {
            get
            {
                _stockAppService = _stockAppService ?? ObjectFactory<StockAppService>.Instance;
                return _stockAppService;
            }
        }

        /// <summary>
        /// 查询渠道仓库列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Stock/QueryStockListByChannelAndMerchant", Method = "POST")]
        public List<StockInfo> QueryStockListByWebChannelIDAndMerchantSysNo(StockQuerySimpleFilter filter)
        {
            return ObjectFactory<IStockDA>.Instance.QueryStockListByWebChannelIDAndMerchantSysNo(filter.MerchantSysNo, filter.WebChannelID);
        }

        #region 仓库(Warehouse)相关

        /// <summary>
        /// 创建仓库
        /// </summary>
        /// <param name="warehouseInfo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Stock/WH/Create", Method = "POST")]
        public virtual WarehouseInfo CreateWarehouse(WarehouseInfo warehouseInfo)
        {
            return StockAppService.CreateWarehouse(warehouseInfo);
        }

        /// <summary>
        /// 更新仓库信息
        /// </summary>
        /// <param name="warehouseInfo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Stock/WH/Update", Method = "POST")]
        public virtual WarehouseInfo UpdateWarehouse(WarehouseInfo warehouseInfo)
        {
            return StockAppService.UpdateWarehouse(warehouseInfo);
        }

        /// <summary>
        /// 获取仓库信息
        /// </summary>
        /// <param name="warehouseSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Stock/WH/Get/{warehouseSysNo}", Method = "GET")]
        public virtual WarehouseInfo GetWarehouseInfo(string warehouseSysNo)
        {
            int sysNo = int.TryParse(warehouseSysNo, out sysNo) ? sysNo : 0;
            return StockAppService.GetWarehouseInfo(sysNo);
        }


        /// <summary>
        /// 查询仓库列表
        /// </summary> 
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Stock/WH/GetByCompanyCode", Method = "POST")]
        public virtual List<WarehouseInfo> GetWarehouseList(string companyCode)
        {
            return StockAppService.GetWarehouseList(companyCode);
        }

        #endregion

        #region 渠道仓库(Stock)相关

        /// <summary>
        /// 创建渠道仓库
        /// </summary>
        /// <param name="stockInfo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Stock/Create", Method = "POST")]
        public virtual StockInfo CreateStock(StockInfo stockInfo)
        {
            return StockAppService.CreateStock(stockInfo);
        }

        /// <summary>
        /// 更新渠道仓库
        /// </summary>
        /// <param name="stockInfo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Stock/Update", Method = "POST")]
        public virtual StockInfo UpdateStock(StockInfo stockInfo)
        {
            return StockAppService.UpdateStock(stockInfo);
        }

        /// <summary>
        /// 获取渠道仓库信息
        /// </summary>
        /// <param name="stockSysNo"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/Stock/Get/{stockSysNo}", Method = "GET")]
        public virtual StockInfo GetStockInfo(string stockSysNo)
        {
            int sysNo = int.TryParse(stockSysNo, out sysNo) ? sysNo : 0;
            return StockAppService.GetStockInfo(sysNo);
        }
        #endregion

        #region 仓库所有者 相关
        [WebInvoke(UriTemplate = "/Stock/WarehouseOwner/GetByCompanyCode", Method = "POST")]
        public virtual List<WarehouseOwnerInfo> GetWarehouseOwnerInfoListByCompanyCode(string companyCode)
        {
            return StockAppService.GetWarehouseOwnerInfoByCompanyCode(companyCode);
        }

        [WebInvoke(UriTemplate = "/Stock/WarehouseOwner/Get/{OwnerSysNo}", Method = "GET")]
        public virtual WarehouseOwnerInfo GetWarehouseOwnerInfoBySysNo(string ownerSysNo)
        {
            int sysNo = int.TryParse(ownerSysNo, out sysNo) ? sysNo : 0;
            return StockAppService.GetWarehouseOwnerInfo(sysNo);
        }

        [WebInvoke(UriTemplate = "/Stock/WarehouseOwner/Update", Method = "POST")]
        public virtual WarehouseOwnerInfo UpdateWarehouseOwnerInfo(WarehouseOwnerInfo entityToUpdate)
        {   
            return StockAppService.UpdateWarehouseOwner(entityToUpdate);
        }

        [WebInvoke(UriTemplate = "/Stock/WarehouseOwner/Create", Method = "POST")]
        public virtual WarehouseOwnerInfo CreateWarehouseOwnerInfo(WarehouseOwnerInfo entityToCreate)
        {
            return StockAppService.CreateWarehouseOwner(entityToCreate);
        }
        #endregion
    }
}
