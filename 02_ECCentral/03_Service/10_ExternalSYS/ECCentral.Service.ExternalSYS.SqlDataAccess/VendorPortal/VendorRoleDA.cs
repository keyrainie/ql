using System;
using System.Data;
using System.Text.RegularExpressions;

using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.Service.ExternalSYS.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.ExternalSYS;
using System.Collections.Generic;



namespace ECCentral.Service.ExternalSYS.SqlDataAccess
{
    [VersionExport(typeof(IVendorRoleDA))]
    public class VendorRoleDA : IVendorRoleDA
    {

        #region IVendorRoleDA Members

        public DataTable RoleQuery(VendorRoleQueryFilter filter, out int dataCount)
        {
            if (filter.PagingInfo != null && filter.PagingInfo.SortBy != null)
            {
                string sortCondition = filter.PagingInfo.SortBy.Trim();

                Match match = Regex.Match(sortCondition, @"^(?<SortColumn>[\S]+)(?:\s+(?<SortType>ASC|DESC))?$", RegexOptions.IgnoreCase);
                if (match.Groups["SortColumn"].Success)
                {
                    string sortColumn = match.Groups["SortColumn"].Value;
                    string sortType = match.Groups["SortType"].Success ?
                        match.Groups["SortType"].Value : "DESC";
                    #region switch
                    switch (sortColumn)
                    {
                        case "SysNo":
                            filter.PagingInfo.SortBy =
                                String.Format("{0} {1}", "SysNo", sortType);
                            break;
                        case "RoleName":
                            filter.PagingInfo.SortBy =
                                String.Format("{0} {1}", "RoleName", sortType);
                            break;
                        case "Status":
                            filter.PagingInfo.SortBy =
                                String.Format("{0} {1}", "Status", sortType);
                            break;
                        case "InUser":
                            filter.PagingInfo.SortBy =
                                String.Format("{0} {1}", "InUser", sortType);
                            break;
                        case "InDate":
                            filter.PagingInfo.SortBy =
                                String.Format("{0} {1}", "InDate", sortType);
                            break;
                        case "EditUser":
                            filter.PagingInfo.SortBy =
                                String.Format("{0} {1}", "EditUser", sortType);
                            break;
                        case "EditDate":
                            filter.PagingInfo.SortBy =
                                String.Format("{0} {1}", "EditDate", sortType);
                            break;
                    }
                    #endregion
                }
            }
            CustomDataCommand command = DataCommandManager.
            CreateCustomDataCommandFromConfig("ExternalSYS_Query_VendorRole");
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(
            command.CommandText, command, HelpDA.ToPagingInfo(filter.PagingInfo), "SysNo DESC"))
            {
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "SysNo", DbType.Int32,
                    "@SysNo", QueryConditionOperatorType.Equal,
                    filter.RoleSysNo);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "RoleName", DbType.String,
                    "@RoleName", QueryConditionOperatorType.LeftLike,
                    filter.RoleName);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "Status", DbType.AnsiStringFixedLength,
                    "@Status", QueryConditionOperatorType.Equal,
                    filter.Status);
                if (!string.IsNullOrEmpty(filter.PrivilegeName))
                {
                    ConditionConstructor subQueryBuilder = builder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, null, QueryConditionOperatorType.Exist,
                    @"SELECT 1 
            FROM (
                    SELECT 
                        [RoleSysNo] 
                    FROM [IPP3].[dbo].[VendorUser_Role_Privilege]  WITH(NOLOCK)
                    WHERE 
                        [PrivilegeSysNo] IN ( 
                                SELECT 
                                    [SysNo]      
                                FROM [IPP3].[dbo].[VendorUser_Privilege]  WITH(NOLOCK)
                                WHERE 
                                    [Memo] LIKE @PrivilegeName
                            )
                 ) AS RESULT 
            WHERE 
                RESULT.[RoleSysNo]=[VendorUser_Role].[SysNo]");
                    command.AddInputParameter("@PrivilegeName", DbType.String,
                        filter.PrivilegeName + "%");
                }

                command.CommandText = builder.BuildQuerySql();
                var dt = command.ExecuteDataTable("Status", typeof(ValidStatus));

                dataCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        #endregion

        public Role CreateRole(Role entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("External_Insert_Role");
            dc.SetParameterValue("@SysNo", entity.SysNo);
            dc.SetParameterValue("@RoleName", entity.RoleName);
            dc.SetParameterValue("@Status", entity.Status);
            dc.SetParameterValue("@InUser", entity.InUser);

            dc.SetParameterValue("@EditUser", entity.EditUser);

            dc.SetParameterValue("@CompanyCode", entity.CompanyCode);

            dc.ExecuteNonQuery();

            entity.SysNo = Convert.ToInt32(dc.GetParameterValue("@SysNo"));
            return entity;
        }

        public Role UpdateRole(Role entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("External_Update_Role");
            dc.SetParameterValue("@SysNo", entity.SysNo);
            dc.SetParameterValue("@RoleName", entity.RoleName);
            dc.SetParameterValue("@Status", entity.Status);

            dc.SetParameterValue("@EditUser", entity.EditUser);

            dc.SetParameterValue("@CompanyCode", entity.CompanyCode);

            dc.ExecuteNonQuery();

            return entity;
        }

        public RolePrivilege CreateRolePrivilege(RolePrivilege entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("External_Insert_RolePrivilege");
            dc.SetParameterValue("@SysNo", entity.SysNo);
            dc.SetParameterValue("@RoleSysNo", entity.RoleSysNo);
            dc.SetParameterValue("@PrivilegeSysNo", entity.PrivilegeSysNo);

            dc.ExecuteNonQuery();

            entity.SysNo = Convert.ToInt32(dc.GetParameterValue("@SysNo"));
            return entity;
        }

        public int DeleteRolePrivilege(int roleSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("External_Delete_RolePrivilege");
            dc.SetParameterValue("@RoleSysNo", roleSysNo);

            return dc.ExecuteNonQuery();
        }

        public int RoleNameIsExist(string roleName, int sysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("External_RoleNameIsExist");
            dc.SetParameterValue("@RoleName", roleName);
            dc.SetParameterValue("@SysNo", sysNo);
            return (int)dc.ExecuteScalar();
        }

        [Caching(ExpiryType = ExpirationType.SlidingTime,ExpireTime="00:20:00")]
        public List<PrivilegeEntity> GetPrivilegeList()
        {
            DataCommand dc =
                DataCommandManager.GetDataCommand("External_Get_PrivilegeList");

            List<PrivilegeEntity> result = dc.ExecuteEntityList<PrivilegeEntity>();
            return result;
        }

        public List<PrivilegeEntity> GetPrivilegeListByRoleSysNo(int roleSysNo)
        {
            DataCommand dc =
                DataCommandManager.GetDataCommand("External_Get_PrivilegeListByRoleSysNo");

            dc.SetParameterValue("@RoleSysNo", roleSysNo);

            return dc.ExecuteEntityList<PrivilegeEntity>();
        }

        /// <summary>
        /// 根据厂家编号和用户编号获取角色信息
        /// </summary>
        /// <param name="ManufacturerSysNo"></param>
        /// <param name="UserSysNo"></param>
        /// <returns></returns>
        public List<VendorUserMapping> GetRoleListByVendorEx(int ManufacturerSysNo, int UserSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("External_Get_RoleListByVendorEx");
            dc.SetParameterValue("@ManufacturerSysNo", ManufacturerSysNo);
            dc.SetParameterValue("@UserSysNo", UserSysNo);
            return dc.ExecuteEntityList<VendorUserMapping>();
        }

        /// <summary>
        /// 根据用户编号获取角色信息
        /// </summary>
        /// <param name="userSysNo"></param>
        /// <returns></returns>
        public List<VendorUserMapping> GetRoleListByUserSysNo(int userSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("External_Get_RoleListByUserSysNo");
            dc.SetParameterValue("@UserSysNo", userSysNo);
            return dc.ExecuteEntityList<VendorUserMapping>();
        }

        /// <summary>
        /// 删除角色Mapping
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int DeleteVendorUser_RoleMapping(VendorUserRole entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("External_Delete_VendorUser_RoleMapping");
            dc.SetParameterValue("@UserSysNo", entity.UserSysNo);
            dc.SetParameterValue("@ManufacturerSysNo", entity.ManufacturerSysNo);
            return dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 取消角色授权
        /// </summary>
        /// <param name="UserSysNo"></param>
        /// <returns></returns>
        public int DeleteVendorUser_User_Role(int UserSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("External_Delete_VendorUser_RoleByUserSysNo");
            dc.SetParameterValue("@UserSysNo", UserSysNo);

            return dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 添加角色Mapping
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public VendorUserRole InsertVendorUser_RoleMapping(VendorUserRole entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("External_Insert_VendorUser_RoleMapping");
            dc.SetParameterValue("@UserSysNo", entity.UserSysNo);
            dc.SetParameterValue("@ManufacturerSysNo", entity.ManufacturerSysNo);
            dc.SetParameterValue("@RoleSysNo", entity.RoleSysNo);

            dc.ExecuteNonQuery();

            return entity;
        }
        /// <summary>
        /// 添加角色授权
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public VendorUserRole InsertVendorUser_User_Role(VendorUserRole entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("External_Insert_VendorUser_Role");
            dc.SetParameterValue("@UserSysNo", entity.UserSysNo);
            dc.SetParameterValue("@RoleSysNo", entity.RoleSysNo);

            dc.ExecuteNonQuery();

            return entity;
        }

    }
}
