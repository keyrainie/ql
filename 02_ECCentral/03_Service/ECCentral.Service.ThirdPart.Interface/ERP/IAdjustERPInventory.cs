using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.Service.ThirdPart.Interface
{
    /// <summary>
    /// 库存调整接口
    /// </summary>
    public interface IAdjustERPInventory
    {

        /// <summary>
        /// 调整商品ERP库存
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        string AdjustERPInventory(ERPInventoryAdjustInfo adjustInfo);

        /// <summary>
        /// 查询商品ERP库存
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        ERPItemInventoryInfo GetERPItemInventoryInfoByProductSysNo(int productSysNo);

        /// <summary>
        /// 查询商品ERP库存
        /// </summary>
        /// <param name="erpProductId"></param>
        /// <returns></returns>
        ERPItemInventoryInfo GetERPItemInventoryInfoByERPProductId(int erpProductId);
    }
}
