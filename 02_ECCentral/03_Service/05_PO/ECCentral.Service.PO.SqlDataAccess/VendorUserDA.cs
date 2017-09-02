using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.BizEntity.PO;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.Service.PO.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ValidStatus = ECCentral.BizEntity.ExternalSYS.ValidStatus;

namespace ECCentral.Service.PO.SqlDataAccess
{
    [VersionExport(typeof (IVendorUserDA))]
    public class VendorUserDA : IVendorUserDA
    {
        public DataTable UserQuery(VendorUserQueryFilter filter, out int dataCount)
        {
            if (filter.PagingInfo.SortBy != null)
            {
                string sortCondition = filter.PagingInfo.SortBy.Trim();

                Match match = Regex.Match(sortCondition, @"^(?<SortColumn>[\S]+)(?:\s+(?<SortType>ASC|DESC))?$", RegexOptions.IgnoreCase);
                if (match.Groups["SortColumn"].Success)
                {
                    string sortColumn = match.Groups["SortColumn"].Value;
                    string sortType = match.Groups["SortType"].Success
                        ? match.Groups["SortType"].Value
                        : "DESC";

                    #region switch

                    switch (sortColumn)
                    {
                        case "SysNo":
                            filter.PagingInfo.SortBy =
                                String.Format("{0} {1}", "a.SysNo", sortType);
                            break;
                        case "RoleName":
                            filter.PagingInfo.SortBy =
                                String.Format("{0} {1}", "a.RoleName", sortType);
                            break;
                        case "Status":
                            filter.PagingInfo.SortBy =
                                String.Format("{0} {1}", "a.Status", sortType);
                            break;
                        case "InUser":
                            filter.PagingInfo.SortBy =
                                String.Format("{0} {1}", "a.InUser", sortType);
                            break;
                        case "InDate":
                            filter.PagingInfo.SortBy =
                                String.Format("{0} {1}", "a.InDate", sortType);
                            break;
                        case "EditUser":
                            filter.PagingInfo.SortBy =
                                String.Format("{0} {1}", "a.EditUser", sortType);
                            break;
                        case "EditDate":
                            filter.PagingInfo.SortBy =
                                String.Format("{0} {1}", "a.EditDate", sortType);
                            break;

                        case "VendorSysNo":
                            filter.PagingInfo.SortBy =
                                String.Format("{0} {1}", "a.VendorSysNo", sortType);
                            break;
                        case "VendorName":
                            filter.PagingInfo.SortBy =
                                String.Format("{0} {1}", "b.VendorName", sortType);
                            break;
                        case "VendorStatus":
                            filter.PagingInfo.SortBy =
                                String.Format("{0} {1}", "b.Status", sortType);
                            break;
                        case "Rank":
                            filter.PagingInfo.SortBy =
                                String.Format("{0} {1}", "b.Rank", sortType);
                            break;
                        case "IsConsign":
                            filter.PagingInfo.SortBy =
                                String.Format("{0} {1}", "b.IsConsign", sortType);
                            break;
                    }

                    #endregion
                }
            }
            CustomDataCommand command = DataCommandManager.
                CreateCustomDataCommandFromConfig("External_Query_Vendor");
            using (var builder = new DynamicQuerySqlBuilder(
                command.CommandText, command, HelpDA.ToPagingInfo(filter.PagingInfo), "a.SysNo DESC"))
            {
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.SysNo", DbType.Int32,
                    "@SysNo", QueryConditionOperatorType.Equal,
                    filter.SysNo);

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
                    "@Status", QueryConditionOperatorType.Equal,
                    ((char?) filter.UserStatus).ToString());

                if (filter.RoleSysNo.HasValue)
                {
                    builder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                    ConditionConstructor subQueryBuilder = builder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, null,
                        QueryConditionOperatorType.Exist,
                        @"SELECT TOP 1 1 
            FROM (SELECT 
                                    UserSysNo
                                FROM [IPP3].[dbo].[VendorUser_User_Role]  WITH(NOLOCK)
                                WHERE 
                                    RoleSysNo=@RoleSysNo
                        ) AS RESULT 
            WHERE 
                RESULT.[UserSysNo]=a.[SysNo]");

                    builder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.OR, null, QueryConditionOperatorType.Exist,
                        @"SELECT TOP 1 1 
            FROM (SELECT 
                                    UserSysNo
                                FROM [IPP3].[dbo].[VendorUser_RoleMapping]  WITH(NOLOCK)
                                WHERE 
                                    RoleSysNo=@RoleSysNo
                        ) AS RESULT 
            WHERE 
                RESULT.[UserSysNo]=a.[SysNo]");
                    command.AddInputParameter("@RoleSysNo", DbType.Int32, filter.RoleSysNo);
                    builder.ConditionConstructor.EndGroupCondition();
                }

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.VendorSysNo", DbType.Int32,
                    "@VendorSysNo", QueryConditionOperatorType.Equal,
                    filter.VendorSysNo);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "b.VendorName", DbType.String,
                    "@VendorName", QueryConditionOperatorType.Like,
                    filter.VendorName);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "b.ExpiredDate", DbType.DateTime,
                    "@ExpiredDateFrom", QueryConditionOperatorType.MoreThanOrEqual,
                    filter.ExpiredDateFrom);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "b.ExpiredDate", DbType.DateTime,
                    "@ExpiredDateTo", QueryConditionOperatorType.LessThanOrEqual,
                    filter.ExpiredDateTo);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "b.Contact", DbType.String,
                    "@Contact", QueryConditionOperatorType.LeftLike,
                    filter.Contact);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "b.Phone", DbType.String,
                    "@Phone", QueryConditionOperatorType.LeftLike,
                    filter.Phone);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "b.Address", DbType.String,
                    "@Address", QueryConditionOperatorType.LeftLike,
                    filter.Address);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "b.Status", DbType.Int32,
                    "@VendorStatus", QueryConditionOperatorType.Equal,
                    filter.VendorStatus);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "b.Rank", DbType.String,
                    "@Rank", QueryConditionOperatorType.Equal,
                    filter.Rank);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "b.IsConsign", DbType.Int32,
                    "@IsConsign", QueryConditionOperatorType.Equal,
                    filter.ConsignType);

                if (filter.ManufacturerSysNo.HasValue
                    || (!string.IsNullOrEmpty(filter.AgentLevel)
                        && filter.AgentLevel.Trim() != "")
                    || filter.C1SysNo.HasValue)
                {
                    string subCond = @"SELECT distinct [VendorSysNo]
                                   FROM dbo.Vendor_Manufacturer  WITH(NOLOCK) ";

                    string strWhere = string.Empty;

                    //代理品牌 二级类 三级类 代理级别 
                    if (filter.ManufacturerSysNo.HasValue)
                    {
                        strWhere += "ManufacturerSysNo=@ManufacturerSysNo";
                        command.AddInputParameter("@ManufacturerSysNo",
                            DbType.Int32, filter.ManufacturerSysNo);
                    }
                    if (!string.IsNullOrEmpty(filter.AgentLevel)
                        && filter.AgentLevel.Trim() != "")
                    {
                        if (strWhere != string.Empty)
                        {
                            strWhere += " AND ";
                        }
                        strWhere += "AgentLevel=@AgentLevel";
                        command.AddInputParameter("@AgentLevel",
                            DbType.String, filter.AgentLevel);
                    }

                    if (filter.C3SysNo.HasValue)
                    {
                        if (strWhere != string.Empty)
                        {
                            strWhere += " AND ";
                        }
                        strWhere += "C3SysNo=@C3SysNo";
                        command.AddInputParameter("@C3SysNo",
                            DbType.Int32, filter.C3SysNo);
                    }
                    else
                    {
                        if (filter.C2SysNo.HasValue)
                        {
                            if (strWhere != string.Empty)
                            {
                                strWhere += " AND ";
                            }
                            strWhere += "C2SysNo=@C2SysNo";
                            command.AddInputParameter("@C2SysNo",
                                DbType.Int32, filter.C2SysNo);
                        }
                        else
                        {
                            if (filter.C1SysNo.HasValue)
                            {
                                if (strWhere != string.Empty)
                                {
                                    strWhere += " AND ";
                                }
                                strWhere += @"C2SysNo in (SELECT 
                [Category2Sysno]  
            FROM [OverseaContentManagement].[dbo].[V_CM_CategoryInfo]  WITH(NOLOCK)
            WHERE 
                [Category1Sysno]=@C1SysNo)";
                                command.AddInputParameter("@C1SysNo",
                                    DbType.Int32, filter.C1SysNo);
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(strWhere))
                    {
                        subCond += "WHERE status=0 and " + strWhere;
                    }
                    builder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "b.SysNo",
                        QueryConditionOperatorType.In, subCond);
                }

                command.CommandText = builder.BuildQuerySql();
                DataTable dt = command.ExecuteDataTable();
                var enumColList = new EnumColumnList();
                enumColList.Add("IsConsign", typeof (VendorConsignFlag));
                enumColList.Add("Status", typeof (ValidStatus));
                enumColList.Add("VendorStatus", typeof (VendorStatus));
                command.ConvertEnumColumn(dt, enumColList);
                dataCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        public VendorUser InsertVendorUser(VendorUser entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("External_Insert_VendorUser");
            dc.SetParameterValue("@SysNo", entity.SysNo);
            dc.SetParameterValue("@VendorSysNo", entity.VendorSysNo);
            dc.SetParameterValue("@UserID", entity.UserID);
            dc.SetParameterValue("@UserNum", entity.UserNum);
            dc.SetParameterValue("@UserName", entity.UserName);
            dc.SetParameterValue("@Pwd", entity.Pwd);
            dc.SetParameterValue("@Email", entity.Email);
            dc.SetParameterValue("@Phone", entity.Phone);

            dc.SetParameterValue("@Note", entity.Note);
            dc.SetParameterValue("@Status", ((char?) entity.Status).ToString());
            dc.SetParameterValue("@InUser", entity.InUser);
            dc.SetParameterValue("@EditUser", entity.EditUser);
            dc.SetParameterValue("@CompanyCode", entity.CompanyCode);

            dc.ExecuteNonQuery();

            entity.SysNo = Convert.ToInt32(dc.GetParameterValue("@SysNo"));
            return entity;
        }


        public void InsertVendorUserVendorEx(VendorUserMapping entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("External_Insert_VendorUserEx");
            dc.SetParameterValue("@UserSysNo", entity.UserSysNo);
            dc.SetParameterValue("@ManufacturerSysNo", entity.ManufacturerSysNo);
            dc.SetParameterValue("@IsAuto", entity.IsAuto);
            dc.SetParameterValue("@VendorSysNo", entity.VendorSysNo);

            dc.ExecuteNonQuery();
        }

        public Role CreateRole(Role entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("External_Insert_Role");
            dc.SetParameterValue("@SysNo", entity.SysNo);
            dc.SetParameterValue("@RoleName", entity.RoleName);
            dc.SetParameterValue("@Status", entity.Status);
            dc.SetParameterValue("@InUser", entity.InUser);

            dc.SetParameterValue("@EditUser", entity.EditUser);

            dc.SetParameterValue("@CompanyCode", entity.CompanyCode);
            dc.SetParameterValue("@VendorSysNo", entity.VendorSysNo);

            dc.ExecuteNonQuery();

            entity.SysNo = Convert.ToInt32(dc.GetParameterValue("@SysNo"));
            return entity;
        }

        public List<PrivilegeEntity> GetPrivilegeList()
        {
            DataCommand dc = DataCommandManager.GetDataCommand("External_Get_PrivilegeList");

            List<PrivilegeEntity> result = dc.ExecuteEntityList<PrivilegeEntity>();
            return result;
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

        public bool IsCreatedVendorUser(int vendorSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("IsCreatedVendorUser");
            dc.SetParameterValue("@VendorSysNo", vendorSysNo);
            object vendorUserSysNo = dc.ExecuteScalar();
            return vendorUserSysNo != null && vendorUserSysNo != DBNull.Value;
        }
    }
}