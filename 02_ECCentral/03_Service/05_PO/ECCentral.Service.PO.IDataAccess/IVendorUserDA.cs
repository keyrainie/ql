using System.Collections.Generic;
using System.Data;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.QueryFilter.ExternalSYS;

namespace ECCentral.Service.PO.IDataAccess
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
        /// 添加商家账号
        /// </summary>
        /// <param name="entity">商家账号实体</param>
        /// <returns>返回添加后的实体</returns>
        VendorUser InsertVendorUser(VendorUser entity);

        /// <summary>
        /// 添加Mapping表
        /// </summary>
        /// <param name="vendorUserMapping">映射表</param>
        void InsertVendorUserVendorEx(VendorUserMapping vendorUserMapping);

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Role CreateRole(Role entity);

        /// <summary>
        /// 获取所有授权定义
        /// </summary>
        /// <returns></returns>
        List<PrivilegeEntity> GetPrivilegeList();

        /// <summary>
        /// 为角色创添加授权
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        RolePrivilege CreateRolePrivilege(RolePrivilege entity);

        /// <summary>
        /// 添加角色Mapping
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        VendorUserRole InsertVendorUser_RoleMapping(VendorUserRole entity);

        /// <summary>
        /// 添加角色授权
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        VendorUserRole InsertVendorUser_User_Role(VendorUserRole entity);

        /// <summary>
        /// 是否已经创建了商家用户
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        bool IsCreatedVendorUser(int vendorSysNo);
    }
}
