using ECommerce.Entity.ControlPannel;
using ECommerce.Utility.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECommerce.Enums;

namespace ECommerce.DataAccess.ControlPannel
{
    public class UserDA
    {
        /// <summary>
        /// 获取商家二级域名
        /// </summary>
        /// <param name="sellerSysNo"></param>
        /// <returns></returns>
        public static SecondDomainInfo LoadSecondDomain(int sellerSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Vendor_LoadSecondDomain");
            dc.SetParameterValue("@SysNo", sellerSysNo);
            return dc.ExecuteEntity<SecondDomainInfo>();
        }
        /// <summary>
        /// 设置商家二级域名
        /// </summary>
        /// <param name="sellerSysNo"></param>
        /// <param name="secondDomain"></param>
        /// <returns></returns>
        public static int SetSecondDomain(SecondDomainInfo secondDomain)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Vendor_SetSecondDomain");
            dc.SetParameterValue(secondDomain);
            return dc.ExecuteScalar<int>();
        }

        public static DataTable UserQuery(UserQueryFilter filter, out int dataCount)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("Query_Vendor_User");
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, filter, !string.IsNullOrWhiteSpace(filter.SortFields) ? filter.SortFields : "a.SysNo DESC"))
            {


                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.UserName", DbType.String,
                    "@UserName", QueryConditionOperatorType.LeftLike,
                    filter.UserName);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.UserID", DbType.String,
                    "@UserID", QueryConditionOperatorType.LeftLike,
                    filter.UserID);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.Status", DbType.AnsiStringFixedLength,
                    "@Status", QueryConditionOperatorType.Equal, filter.UserStatus);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode",
                    QueryConditionOperatorType.Equal,
                        filter.CompanyCode);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
               "a.VendorSysNo", DbType.AnsiStringFixedLength, "@ManufacturerSysNo",
               QueryConditionOperatorType.Equal,
                   filter.ManufacturerSysNo);
                if (!string.IsNullOrEmpty(filter.SerialNum))
                {
                    string[] serialNum = filter.SerialNum.Split('-');

                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                    "a.VendorSysNo", DbType.Int32,
                                    "@VendorSysNo", QueryConditionOperatorType.Equal,
                                    serialNum[0]);
                    if (serialNum.Length > 1)
                    {
                        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                                        "a.UserNum", DbType.Int32,
                                        "@UserNum", QueryConditionOperatorType.Equal,
                                        Convert.ToInt32(serialNum[1]).ToString());
                    }
                }

                command.CommandText = builder.BuildQuerySql();
                var dt = command.ExecuteDataTable();
                EnumColumnList enumColList = new EnumColumnList();
                enumColList.Add("Status", typeof(UserStatus));
                command.ConvertEnumColumn(dt, enumColList);
                dataCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
                return dt;
            }
        }



        public static DataTable QueryRole(RoleQueryFilter filter, out int dataCount)
        {
            CustomDataCommand command = DataCommandManager.
           CreateCustomDataCommandFromConfig("Query_Vendor_Role");
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(
            command.CommandText, command, filter, !string.IsNullOrWhiteSpace(filter.SortFields) ? filter.SortFields : "SysNo DESC"))
            {


                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "RoleName", DbType.String,
                    "@RoleName", QueryConditionOperatorType.LeftLike,
                    filter.RoleName);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "Status", DbType.AnsiStringFixedLength,
                    "@Status", QueryConditionOperatorType.Equal,
                    filter.Status);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "ManufacturerSysNo", DbType.AnsiStringFixedLength,
                    "@ManufacturerSysNo", QueryConditionOperatorType.Equal,
                    filter.ManufacturerSysNo);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "VendorSysNo", DbType.AnsiStringFixedLength,
                    "@VendorSysNo", QueryConditionOperatorType.Equal,
                    filter.VendorSysNo);

                command.CommandText = builder.BuildQuerySql();
                var dt = command.ExecuteDataTable("Status", typeof(RoleStatus));
                dataCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        public static List<PrivilegeInfo> GetPrivilegeListByRoleSysNo(int roleSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Get_PrivilegeListByRoleSysNo");
            dc.SetParameterValue("@RoleSysNo", roleSysNo);
            return dc.ExecuteEntityList<PrivilegeInfo>();
        }

        public static RoleInfo CreateRole(RoleInfo info)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Insert_Role");
            dc.SetParameterValue<RoleInfo>(info);
            dc.ExecuteNonQuery();
            info.SysNo = Convert.ToInt32(dc.GetParameterValue("@SysNo"));
            return info;
        }


        public static void UpdateRole(RoleInfo info)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Update_Role");
            dc.SetParameterValue<RoleInfo>(info);
            dc.ExecuteNonQuery();
        }

        public static List<PrivilegeInfo> GetPrivilegeList()
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Get_PrivilegeList");
            List<PrivilegeInfo> result = dc.ExecuteEntityList<PrivilegeInfo>();
            return result;

        }

        public static void UpdateRoleStatus(RoleInfo item)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Update_Role_Status");
            dc.SetParameterValue<RoleInfo>(item);
            dc.ExecuteNonQuery();
        }

        public static void InsertRolePrivilege(int roleSysNo, int privilegeSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Insert_RolePrivilege");
            dc.SetParameterValue("@RoleSysNo", roleSysNo);
            dc.SetParameterValue("@PrivilegeSysNo", privilegeSysNo);
            dc.ExecuteNonQuery();
        }

        public static int DeleteRolePrivilege(int roleSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Delete_RolePrivilege");
            dc.SetParameterValue("@RoleSysNo", roleSysNo);
            return dc.ExecuteNonQuery();
        }

        public static int RoleNameIsExist(string roleName, int sysNo, int VendorSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("RoleNameIsExist");
            dc.SetParameterValue("@RoleName", roleName);
            dc.SetParameterValue("@SysNo", sysNo);
            dc.SetParameterValue("@VendorSysNo", VendorSysNo);
            return (int)dc.ExecuteScalar();
        }



        public static RoleInfo GetRoleInfo(int roleSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetRoleBySysNo");
            dc.SetParameterValue("@SysNo", roleSysNo);
            return dc.ExecuteEntity<RoleInfo>();
        }

        public static void InsertVendorUser(UserInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Insert_VendorUser");
            dc.SetParameterValue("@SysNo", entity.SysNo);
            dc.SetParameterValue<UserInfo>(entity);
            dc.ExecuteNonQuery();
            entity.SysNo = Convert.ToInt32(dc.GetParameterValue("@SysNo"));
        }

        public static void UpdateVendorUser(UserInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Update_VendorUser");
            dc.SetParameterValue<UserInfo>(entity);
            dc.ExecuteNonQuery();
        }

        public static void UpdateVendorUserStatus(List<int> sysNos, UserStatus status, string editUser)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Update_VendorUserStatus");
            dc.SetParameterValue("@Status", status);
            dc.SetParameterValue("@EditUser", editUser);
            dc.ReplaceParameterValue("#SysNos#", string.Join(",", sysNos));
            dc.ExecuteNonQuery();
        }

        public static int CountUserID(string userID, int sysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Get_UserIDCount");
            dc.SetParameterValue("@SysNo", sysNo);
            dc.SetParameterValue("@UserID", userID);
            return (int)dc.ExecuteScalar();
        }

        public static int CountVendorNum(int vendorSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Get_VendorNumCount");
            dc.SetParameterValue("@VendorSysNo", vendorSysNo);
            return (int)dc.ExecuteScalar();
        }

        public static UserInfo GetUserBySysNo(int sysNo, int sellersysno)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Get_VendorUser");
            dc.SetParameterValue("@UserSysNo", sysNo);
            dc.SetParameterValue("@VendorSysNo", sellersysno);
            return dc.ExecuteEntity<UserInfo>();
        }

        public static UsersRoleInfo InsertVendorUser_User_Role(UsersRoleInfo entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Insert_VendorUser_Role");
            dc.SetParameterValue("@UserSysNo", entity.UserSysNo);
            dc.SetParameterValue("@RoleSysNo", entity.RoleSysNo);
            dc.ExecuteNonQuery();
            return entity;
        }
        public static int DeleteVendorUser_User_Role(int UserSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Delete_VendorUser_RoleByUserSysNo");
            dc.SetParameterValue("@UserSysNo", UserSysNo);
            return dc.ExecuteNonQuery();
        }

        public static List<UsersRoleInfo> GetRolesByUserSysNo(int sysno)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("VendorUser_GetRolesByUserSysNo");
            dc.SetParameterValue("@UserSysNo", sysno);
            return dc.ExecuteEntityList<UsersRoleInfo>();
        }

        public static void UpdateUserPassword(UserInfo user)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateUserPassword");
            dc.SetParameterValue<UserInfo>(user);
            dc.ExecuteNonQuery();
        }
    }
}
