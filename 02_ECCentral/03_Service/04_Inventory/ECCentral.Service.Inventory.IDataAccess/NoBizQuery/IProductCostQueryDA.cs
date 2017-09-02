using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.Inventory;
using ECCentral.BizEntity.Inventory;
using ECCentral.QueryFilter.PO;

namespace ECCentral.Service.Inventory.IDataAccess.NoBizQuery
{
    public interface IProductCostQueryDA
    {
        /// <summary>
        /// 库存成本序列查询
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryProductCostInList(ProductCostQueryFilter queryFilter, out int totalCount);

        /// <summary>
        /// 查询商品入库成本明细
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        DataTable QueryAvaliableCostInList(CostChangeItemsQueryFilter queryFilter, out int totalCount);

        /// <summary>
        /// 预校验库存成本变更
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        bool PreCheckCostChange(int ccSysNo);
    }
}