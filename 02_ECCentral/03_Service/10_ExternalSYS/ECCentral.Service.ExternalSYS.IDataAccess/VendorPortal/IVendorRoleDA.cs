using System.Data;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.BizEntity.ExternalSYS;
using System.Collections.Generic;

namespace ECCentral.Service.ExternalSYS.IDataAccess
{
    public interface IVendorRoleDA
    {
        /// <summary>
        /// 角色查询
        /// </summary>
        /// <param name="filter">过滤条件</param>
        /// <param name="dataCount">查询总数</param>
        /// <returns>数据集合</returns>
        DataTable RoleQuery(VendorRoleQueryFilter filter, out int dataCount);

        Role CreateRole(Role entity);

        Role UpdateRole(Role entity);

        RolePrivilege CreateRolePrivilege(RolePrivilege entity);

        int DeleteRolePrivilege(int roleSysNo);

        int RoleNameIsExist(string roleName, int sysNo);

        List<PrivilegeEntity> GetPrivilegeList();

        List<PrivilegeEntity> GetPrivilegeListByRoleSysNo(int roleSysNo);

        /// <summary>
        /// 根据厂家编号和用户编号获取角色列表
        /// </summary>
        /// <param name="ManufacturerSysNo">厂家编号</param>
        /// <param name="UserSysNo">用户编号</param>
        /// <returns></returns>
        List<VendorUserMapping> GetRoleListByVendorEx(int ManufacturerSysNo, int UserSysNo);

        /// <summary>
        /// 根据用户编号获取角色列表
        /// </summary>
        /// <param name="userSysNo">用户编号</param>
        /// <returns></returns>
        List<VendorUserMapping> GetRoleListByUserSysNo(int userSysNo);

        VendorUserRole InsertVendorUser_User_Role(VendorUserRole entity);

        VendorUserRole InsertVendorUser_RoleMapping(VendorUserRole entity);

        int DeleteVendorUser_User_Role(int UserSysNo);

        int DeleteVendorUser_RoleMapping(VendorUserRole entity);
    }
}
