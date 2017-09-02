using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.PO;

namespace ECCentral.Service.PO.IDataAccess.NoBizQuery
{
    public interface IVendorQueryDA
    {
        DataTable QueryVendorList(VendorQueryFilter queryFilter, out int totalCount);

        /// <summary>
        ///  供应商应付款低于应收时系统添加控制
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        DataTable QueryVendorPayBalanceByVendorSysNo(int vendorSysNo);

        /// <summary>
        /// 获取Vendor下可以锁定的PM列表
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        DataTable QueryCanLockPMListByVendorSysNo(VendorQueryFilter queryFilter);

        /// <summary>
        /// 获取已经锁定的PM列表
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        DataTable QueryVendorPMHoldInfoByVendorSysNo(VendorQueryFilter queryFilter);

        /// <summary>
        /// 查询商家页面类型
        /// </summary>
        /// <returns></returns>
        DataTable QueryStorePageType();

        DataTable QueryStorePageInfo(StorePageQueryFilter filter,out int totalCount);

        void DeleteStorePageInfo(int SysNo);

        void CheckStorePageInfo(int SysNo, int Status);
    }
}
