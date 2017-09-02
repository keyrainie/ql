using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.PO;
using System.Data;

namespace ECCentral.Service.PO.IDataAccess
{
    /// <summary>
    /// 供应商退款
    /// </summary>
    public interface IVendorRefundDA
    {
        /// <summary>
        /// 加载供应商退款单信息
        /// </summary>
        /// <param name="vendorRefundSysNo"></param>
        /// <returns></returns>
        VendorRefundInfo LoadVendorRefundInfo(int vendorRefundSysNo);

        /// <summary>
        /// 记载供应商退款单商品
        /// </summary>
        /// <param name="vendorRefundSysNo"></param>
        /// <returns></returns>
        List<VendorRefundItemInfo> LoadVendorRefundItems(int vendorRefundSysNo);

        /// <summary>
        /// 更新供应商信息
        /// </summary>
        /// <param name="refundInfo"></param>
        /// <returns></returns>
        VendorRefundInfo UpdateVendorRefundInfo(VendorRefundInfo refundInfo);

        /// <summary>
        ///  获得Item的PM
        /// </summary>
        /// <param name="vendorRefundSysNo"></param>
        /// <returns></returns>
        List<int> GetPMUserSysNoByRMAVendorRefundSysNo(int vendorRefundSysNo);
    }
}
