using System.Data;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.BizEntity.ExternalSYS;
using System.Collections.Generic;

namespace ECCentral.Service.ExternalSYS.IDataAccess
{
    public interface IVendorUserDA
    {
        /// <summary>
        /// 账号查询
        /// </summary>
        /// <param name="filter">过滤条件</param>
        /// <param name="dataCount">查询总数</param>
        /// <returns>数据集合</returns>
        DataTable UserQuery(VendorUserQueryFilter filter, out int dataCount);

        /// <summary>
        /// 添加供应商账号
        /// </summary>
        /// <param name="entity">供应商账号实体</param>
        /// <returns>返回添加后的实体</returns>
        VendorUser InsertVendorUser(VendorUser entity);

        /// <summary>
        /// 修改供应商账号
        /// </summary>
        /// <param name="entity">供应商账号实体</param>
        /// <returns>修改成功返回真，否则返回假</returns>
        bool UpdateVendorUser(VendorUser entity);

        /// <summary>
        /// 更新供应商账号状态
        /// </summary>
        /// <param name="sysNos">待更新的编号</param>
        /// <param name="status">更新状态</param>
        /// <param name="editUser">更新人</param>
        void UpdateVendorUserStatus(List<int> sysNos, ValidStatus status, string editUser);

        /// <summary>
        /// 统计当前用户名的账户数
        /// </summary>
        /// <param name="userID">用户名</param>
        /// <param name="sysNo">当前编号</param>
        /// <returns>账户数</returns>
        int CountUserID(string userID, int sysNo);

        /// <summary>
        /// 统计当前供应商的账户数
        /// </summary>
        /// <param name="vendorSysNo">供应商编号</param>
        /// <returns>账户数</returns>
        int CountVendorNum(int vendorSysNo);

        /// <summary>
        /// 添加Mapping表
        /// </summary>
        /// <param name="vendorUserMapping">映射表</param>
        void InsertVendorUserVendorEx(VendorUserMapping vendorUserMapping);

        /// <summary>
        /// 更新Mapping表
        /// </summary>
        /// <param name="vendorUserMapping"></param>
        void InsertVendorUser_VendorExForUpdate(VendorUserMapping vendorUserMapping);

        /// <summary>
        /// 根据编号获取对象
        /// </summary>
        /// <param name="sysNo">编号</param>
        /// <returns>对象</returns>
        VendorUser GetUserBySysNo(int sysNo);

        /// <summary>
        /// 查询VendorProduct
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        DataTable VendorProductQuery(VendorProductQueryFilter filter,out int TotalCount);

        int GetIsAuto(VendorProductQueryFilter filter);

        VendorProductList InsertVendorUser_ProductMappingAll(VendorProductList entity);

        List<Vendor_ExInfo> QueryByStockShippingeInvoic(Vendor_ExInfo query);

        int DeleteVendorUser_ProductMappingAll(VendorProductList entity);

        int UpdateVendorUser_VendorEx(VendorProductList entity);

        int DeleteVendorUser_ProductMapping(VendorProductList entity);

        VendorProductList InsertVendorUser_ProductMapping(VendorProductList entity);
    }
}
