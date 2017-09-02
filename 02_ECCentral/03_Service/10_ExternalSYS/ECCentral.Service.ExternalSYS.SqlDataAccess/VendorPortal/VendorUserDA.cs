using System;
using System.Data;
using System.Text.RegularExpressions;

using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.Service.ExternalSYS.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.ExternalSYS;
using System.Collections.Generic;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.ExternalSYS.SqlDataAccess
{
    [VersionExport(typeof(IVendorUserDA))]
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
                    string sortType = match.Groups["SortType"].Success ?
                        match.Groups["SortType"].Value : "DESC";
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
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(
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
                    LegacyEnumMapper.ConvertValidStatus(filter.UserStatus));

                if (filter.RoleSysNo.HasValue)
                {
                    builder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                    ConditionConstructor subQueryBuilder = builder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, null, QueryConditionOperatorType.Exist,
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
                var dt = command.ExecuteDataTable();
                EnumColumnList enumColList = new EnumColumnList();
                enumColList.Add("IsConsign", typeof(ECCentral.BizEntity.PO.VendorConsignFlag));
                enumColList.Add("Status", typeof(ECCentral.BizEntity.ExternalSYS.ValidStatus));
                enumColList.Add("VendorStatus", typeof(VendorStatus));
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
            dc.SetParameterValue("@Status", LegacyEnumMapper.ConvertValidStatus(entity.Status));
            dc.SetParameterValue("@InUser", entity.InUser);
            dc.SetParameterValue("@EditUser", entity.EditUser);
            dc.SetParameterValue("@CompanyCode", entity.CompanyCode);

            dc.ExecuteNonQuery();

            entity.SysNo = Convert.ToInt32(dc.GetParameterValue("@SysNo"));
            return entity;
        }

        public bool UpdateVendorUser(VendorUser entity)
        {
            DataCommand  dc = DataCommandManager.GetDataCommand("External_Update_VendorUser");
            ////2015.8.19为重置密码添加 John
            if (entity.Pwd != null)
            {
                dc = DataCommandManager.GetDataCommand("External_ResetPwd_VendorUser");
                dc.SetParameterValue("@Pwd", entity.Pwd);
                dc.SetParameterValue("@SysNo", entity.SysNo);
            }
            else
            {
                dc.SetParameterValue("@SysNo", entity.SysNo);
                dc.SetParameterValue("@VendorSysNo", entity.VendorSysNo);
                dc.SetParameterValue("@UserID", entity.UserID);
                dc.SetParameterValue("@UserName", entity.UserName);
                dc.SetParameterValue("@Email", entity.Email);
                dc.SetParameterValue("@Phone", entity.Phone);
                dc.SetParameterValue("@APIStatus", LegacyEnumMapper.ConvertValidStatus(entity.APIStatus));
                dc.SetParameterValue("@APIKey", entity.APIKey);
                dc.SetParameterValue("@Note", entity.Note);
                dc.SetParameterValue("@Status", LegacyEnumMapper.ConvertValidStatus(entity.Status));
                dc.SetParameterValue("@EditUser", entity.EditUser);
            }


            return dc.ExecuteNonQuery() > 0;
        }

        public void UpdateVendorUserStatus(List<int> sysNos, ECCentral.BizEntity.ExternalSYS.ValidStatus status, string editUser)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("External_Update_VendorUserStatus");
            dc.SetParameterValue("@Status", LegacyEnumMapper.ConvertValidStatus(status));
            dc.SetParameterValue("@EditUser", editUser);
            dc.ReplaceParameterValue("#SysNos#", string.Join(",", sysNos));
            dc.ExecuteNonQuery();
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

        public void InsertVendorUser_VendorExForUpdate(VendorUserMapping entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("External_InsertVendorUser_VendorExForUpdate");
            dc.SetParameterValue("@UserSysNo", entity.UserSysNo);
            dc.SetParameterValue("@ManufacturerSysNo", entity.ManufacturerSysNo);
            dc.SetParameterValue("@IsAuto", entity.IsAuto);

            dc.ExecuteNonQuery();
        }

        public int CountUserID(string userID, int sysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("External_Get_UserIDCount");
            dc.SetParameterValue("@SysNo", sysNo);
            dc.SetParameterValue("@UserID", userID);
            return (int)dc.ExecuteScalar();
        }

        public int CountVendorNum(int vendorSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("External_Get_VendorNumCount");
            dc.SetParameterValue("@VendorSysNo", vendorSysNo);
            return (int)dc.ExecuteScalar();
        }

        public VendorUser GetUserBySysNo(int sysNo)
        {
            var filter = new VendorUserQueryFilter();
            filter.PagingInfo = new ECCentral.QueryFilter.Common.PagingInfo();
            filter.PagingInfo.PageIndex = 0;
            filter.PagingInfo.PageSize = 1;
            filter.SysNo = sysNo;
            int dataCount = 0;
            var dt = UserQuery(filter, out dataCount);
            if (dataCount == 1)
            {
                return DataMapper.GetEntity<VendorUser>(dt.Rows[0], true, true, null);
            }
            return new VendorUser();
        }

        /// <summary>
        /// 查询VendorProdcut
        /// </summary>
        /// <returns></returns>
        public DataTable VendorProductQuery(VendorProductQueryFilter filter, out int TotalCount)
        {
            CustomDataCommand command = DataCommandManager.
            CreateCustomDataCommandFromConfig("External_Query_VendorProduct");
            using (DynamicQuerySqlBuilder sb = new DynamicQuerySqlBuilder(command.CommandText, command, HelpDA.ToPagingInfo(filter.PagingInfo), "a.SysNo DESC"))
            {
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.ProductType", DbType.Int32,
                    "@ProductType", QueryConditionOperatorType.Equal,
                    0);
                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.Status", DbType.Int32,
                    "@Status", QueryConditionOperatorType.MoreThanOrEqual,
                    0);

                sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.ManufacturerSysNo", DbType.Int32,
                    "@ManufacturerSysNo", QueryConditionOperatorType.Equal,
                    filter.VendorManufacturerSysNo);

                command.AddInputParameter("@UserSysNo", DbType.Int32, filter.UserSysNo);
                if (filter.C3SysNo.HasValue)
                {
                    sb.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.C3SysNo", DbType.Int32,
                    "@C3SysNo", QueryConditionOperatorType.Equal,
                    filter.C3SysNo);
                }
                else
                {
                    if (filter.C2SysNo.HasValue)
                    {
                        ConditionConstructor subQueryBuilder = sb.ConditionConstructor.AddSubQueryCondition(
                            QueryConditionRelationType.AND,
                            "a.C3SysNo",
                            QueryConditionOperatorType.In,
                            @"SELECT [Category3Sysno]  
                            FROM [OverseaContentManagement].[dbo].[V_CM_CategoryInfo] 
                            WHERE [Category2Sysno]=@C2SysNo");
                        command.AddInputParameter("@C2SysNo", DbType.Int32, filter.C2SysNo);
                    }
                }

                if (!filter.IsAuto)
                {
                    QueryConditionOperatorType ot = QueryConditionOperatorType.Exist;
                    if (!filter.IsMapping)
                    {
                        ot = QueryConditionOperatorType.NotExist;
                    }

                    sb.ConditionConstructor.AddSubQueryCondition(
                        QueryConditionRelationType.AND,
                        null,
                        ot,
                        @"SELECT TOP 1 1 
                        FROM [IPP3].[dbo].VendorUser_ProductMapping 
                        WHERE 
                            ProductSysNo=a.sysno 
                            AND UserSysNo=@UserSysNo 
                            AND VendorExSysNo=@VendorExSysNo");
                }

                sb.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "a.MerchantSysNo", 
                    DbType.Int32, 
                    "@MerchantSysNo", 
                    QueryConditionOperatorType.Equal, filter.VendorSysNo);

                command.AddInputParameter(
                    "@VendorManufacturerSysNo",
                    DbType.Int32, 
                    filter.ManufacturerSysNo);

                command.CommandText = sb.BuildQuerySql();
                DataTable dt = command.ExecuteDataTable();
                TotalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
                return dt;

            }
        }

        public int GetIsAuto(VendorProductQueryFilter filter)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("External_Get_IsAuto");
            dataCommand.SetParameterValue("@UserSysNo", filter.UserSysNo);
            dataCommand.SetParameterValue("@ManufacturerSysNo", filter.ManufacturerSysNo);
            object isAuto = dataCommand.ExecuteScalar();
            if (isAuto != null && isAuto != DBNull.Value)
            {
                return Convert.ToInt32(isAuto);
            }
            return 1;
        }

        public List<Vendor_ExInfo> QueryByStockShippingeInvoic(Vendor_ExInfo query)
        {
            var resultList = new List<Vendor_ExInfo>();
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("External_Query_ByStockShippingeInvoic");

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, new PagingInfoEntity(), "SysNo DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND, 
                    "VendorSysNo", 
                    DbType.Int32, 
                    "@VendorSysNo", 
                    QueryConditionOperatorType.Equal, 
                    query.VendorSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND, 
                    "StockType", 
                    DbType.AnsiStringFixedLength, 
                    "@StockType", 
                    QueryConditionOperatorType.Equal, 
                    query.StockType);
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND, 
                    "ShippingType", 
                    DbType.AnsiStringFixedLength, 
                    "@ShippingType", 
                    QueryConditionOperatorType.Equal, 
                    query.ShippingType);
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND, 
                    "InvoiceType", 
                    DbType.AnsiStringFixedLength, 
                    "@InvoiceType", 
                    QueryConditionOperatorType.Equal, 
                    query.InvoiceType);

                cmd.CommandText = sqlBuilder.BuildQuerySql();

                resultList = cmd.ExecuteEntityList<Vendor_ExInfo>();

                return resultList;
            }
        }

        public VendorProductList InsertVendorUser_ProductMappingAll(VendorProductList entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("External_Insert_VendorUser_ProductMappingAll");
            dc.SetParameterValue("@UserSysNo", entity.UserSysNo);
            dc.SetParameterValue("@ManufacturerSysNo", entity.ManufacturerSysNo);
            dc.SetParameterValue("@VendorManufacturerSysNo", entity.VendorManufacturerSysNo);
            dc.SetParameterValue("@C2SysNo", entity.C2SysNo);
            dc.SetParameterValue("@C3SysNo", entity.C3SysNo);
            dc.ExecuteNonQuery();
            return entity;
        }

        public int DeleteVendorUser_ProductMappingAll(VendorProductList entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("External_Delete_VendorUser_ProductMappingAll");
            dc.SetParameterValue("@UserSysNo", entity.UserSysNo);
            dc.SetParameterValue("@ManufacturerSysNo", entity.ManufacturerSysNo);
            return dc.ExecuteNonQuery();
        }

        public int UpdateVendorUser_VendorEx(VendorProductList entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("External_Update_VendorUser_VendorEx");
            dc.SetParameterValue("@UserSysNo", entity.UserSysNo);
            dc.SetParameterValue("@ManufacturerSysNo", entity.ManufacturerSysNo);
            dc.SetParameterValue("@IsAuto", entity.IsAuto);
            return dc.ExecuteNonQuery();
        }

        public int DeleteVendorUser_ProductMapping(VendorProductList entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("External_Delete_VendorUser_ProductMapping");
            dc.SetParameterValue("@UserSysNo", entity.UserSysNo);
            dc.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            return dc.ExecuteNonQuery();
        }

        public VendorProductList InsertVendorUser_ProductMapping(VendorProductList entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("External_Insert_VendorUser_ProductMapping");
            dc.SetParameterValue("@UserSysNo", entity.UserSysNo);
            dc.SetParameterValue("@ManufacturerSysNo", entity.ManufacturerSysNo);
            dc.SetParameterValue("@ProductSysNo", entity.ProductSysNo);

            dc.ExecuteNonQuery();
            return entity;
        }
    }
}
