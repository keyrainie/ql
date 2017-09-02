using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Service.Inventory.IDataAccess
{
    public interface IVirtualRequestDA
    {
        #region 虚库申请单维护

        /// <summary>
        /// 根据SysNO获取虚库申请单信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        VirtualRequestInfo GetVirtualRequestInfoBySysNo(int sysNo);

        /// <summary>
        /// 创建虚库申请单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        VirtualRequestInfo CreateVirtualRequest(VirtualRequestInfo entity);

        /// <summary>
        /// 更新虚库申请单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        VirtualRequestInfo UpdateVirtualRequestStatus(VirtualRequestInfo entity);

        #endregion 虚库申请单

        int CheckVirtualQty(int productSysNo, int quantity, int warehouseNumber, string companyCode);

        bool UpdateProductExtension(int virtualType, int itemSysNumber, string companyCode);

        bool Verify(VirtualRequestInfo entity);

        List<VirtualRequestInfo> ExistNeedCloseRequestByStockAndItem(int stockSysNumber, int itemSysNumber, string companyCode);

        int CloseRequest(int StVirtualRequestSysNo, VirtualRequestStatus Status, int IsAdjustVirtualQty, string CompanyCode);

        int Apply(VirtualRequestInfo entity);

        /// <summary>
        /// 查询其他单据中可以关闭的虚库数量
        /// </summary>
        /// <param name="stockSysNumber">仓库号</param>
        /// <param name="productSysNumber">商品编号</param>
        /// <param name="sysNo">单据编号</param>
        /// <param name="companyCode">公司编号</param>
        /// <returns></returns>
        int ExistNeedCloseVirtualQuantity(int stockSysNumber, int productSysNumber, int sysNo, string companyCode);
    }
}
