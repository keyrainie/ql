using System;
using System.Data;
using ECCentral.BizEntity.Inventory;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Service.Inventory.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Text.RegularExpressions;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.Inventory.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IVirtualRequestQueryDA))]
    public class VirtualRequestQueryDA : IVirtualRequestQueryDA
    {
        private PagingInfoEntity PagingInfoToPagingInfoEntity(ECCentral.QueryFilter.Common.PagingInfo info)
        {
            PagingInfoEntity entity = new PagingInfoEntity();
            entity.SortField = info.SortBy;
            entity.MaximumRows = info.PageSize;
            entity.StartRowIndex = info.PageIndex * info.PageSize;
            return entity;
        }
   
        private string VirtualMemoSortFieldMapping(string sortField)
        {
            sortField = sortField == null ? null : sortField.Trim();
            if (!String.IsNullOrEmpty(sortField))
            {
                string[] tsort = sortField.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                switch (tsort[0].ToUpper())
                {
                    case "PRODUCTID":
                        tsort[0] = "b.ProductID";
                        break;
                    case "VIRTUALQUANTITY":
                        tsort[0] = "a.VirtualQty";
                        break;
                    case "VIRTUALTYPE":
                        tsort[0] = "a.VirtualType";
                        break;
                    case "CREATEUSERNAME":
                        tsort[0] = "c.DisplayName";
                        break;
                    case "CREATEDATE":
                        tsort[0] = "a.CreateTime";
                        break;                    
                }
                sortField = String.Join(" ", tsort);
            }
            return sortField;
        }

        /// <summary>
        /// 查询虚库申请单
        /// </summary>
        /// <returns></returns>
        public DataTable QueryVirtualRequest(VirtualRequestQueryFilter queryFilter, out int totalCount)
        {
            if (queryFilter == null)
            {
                totalCount = 0;
                return null;
            }
            DataTable dt = new DataTable();
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = queryFilter.PagingInfo.SortBy,
                StartRowIndex = queryFilter.PagingInfo.PageIndex * queryFilter.PagingInfo.PageSize,
                MaximumRows = queryFilter.PagingInfo.PageSize
            };
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("Inventory_QueryVirtualRequest");

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText,
                dataCommand, pagingInfo, "a.SysNo DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.CompanyCode",
                    DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, queryFilter.CompanyCode);
                if (queryFilter.RequestStatus.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.Status",
                        DbType.Int32, "@Status", QueryConditionOperatorType.Equal, (int)queryFilter.RequestStatus);
                }
                if (queryFilter.ProductSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.ProductSysNo",
                        DbType.Int32, "@ProductSysNo", QueryConditionOperatorType.Equal, queryFilter.ProductSysNo.Value);
                }
                if (queryFilter.SysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.SysNo",
        DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, queryFilter.SysNo.Value);
                }
                if (queryFilter.StockSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.StockSysNo",
    DbType.Int32, "@StockSysNo", QueryConditionOperatorType.Equal, queryFilter.StockSysNo.Value);

                }
                if (queryFilter.CreateUserSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.CreateUserSysNo",
    DbType.Int32, "@CreateUserSysNo", QueryConditionOperatorType.Equal, queryFilter.CreateUserSysNo.Value);
                }
                if (queryFilter.VirtualRequestType.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.VirtualType",
        DbType.Int32, "@VirtualType", QueryConditionOperatorType.Equal, (int)queryFilter.VirtualRequestType);

                }
                if (queryFilter.StartDate.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.CreateTime",
                        DbType.DateTime, "@DateFrom", QueryConditionOperatorType.MoreThanOrEqual, queryFilter.StartDate.Value);
                }
                if (queryFilter.EndDate.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.CreateTime",
                        DbType.DateTime, "@DateTo", QueryConditionOperatorType.LessThan, queryFilter.EndDate.Value.AddDays(1));
                }

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                EnumColumnList list = new EnumColumnList();
                list.Add("RequestStatus", typeof(VirtualRequestStatus));
                dt = dataCommand.ExecuteDataTable(list);
                if (null != dt && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["VirtualTypeString"] = CodeNamePairManager.GetName("Inventory", "VirtualRequestType", dr["VirtualType"].ToString());
                    }
                }
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
            }
            return dt;
        }

        /// <summary>
        /// 查询虚库日志
        /// </summary>
        /// <returns></returns>
        public DataTable QueryVirtualRequestMemo(VirtualRequestQueryFilter queryFilter, out int totalCount)
        {
            if (queryFilter == null)
            {
                totalCount = 0;
                return null;
            }
            DataTable dt = new DataTable();
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = VirtualMemoSortFieldMapping(queryFilter.PagingInfo.SortBy),
                StartRowIndex = queryFilter.PagingInfo.PageIndex * queryFilter.PagingInfo.PageSize,
                MaximumRows = queryFilter.PagingInfo.PageSize
            };
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("Inventory_QueryVirtualRequestMemo");

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText,
                dataCommand, pagingInfo, "a.SysNo DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.CompanyCode",
                  DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, queryFilter.CompanyCode);

                if (queryFilter.ProductSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.ProductSysNo",
                        DbType.Int32, "@ProductSysNo", QueryConditionOperatorType.Equal, queryFilter.ProductSysNo.Value);
                }

                if (queryFilter.StartDate.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.CreateTime",
                        DbType.DateTime, "@DateFrom", QueryConditionOperatorType.MoreThanOrEqual, queryFilter.StartDate.Value);
                }
                if (queryFilter.EndDate.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.CreateTime",
                        DbType.DateTime, "@DateTo", QueryConditionOperatorType.LessThan, queryFilter.EndDate.Value.AddDays(1));
                }

                if (queryFilter.CreateUserSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.CreateUserSysNo",
    DbType.Int32, "@CreateUserSysNo", QueryConditionOperatorType.Equal, queryFilter.CreateUserSysNo.Value);
                }
                if (queryFilter.VirtualRequestType.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.VirtualType",
        DbType.Int32, "@VirtualType", QueryConditionOperatorType.Equal, (int)queryFilter.VirtualRequestType);

                }

                if (queryFilter.StockSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.StockSysNo",
    DbType.Int32, "@StockSysNo", QueryConditionOperatorType.Equal, queryFilter.StockSysNo.Value);

                }

                if (!string.IsNullOrEmpty(queryFilter.Note))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.Note",
                    DbType.String, "@Note", QueryConditionOperatorType.Like, queryFilter.Note);
                }

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                dt = dataCommand.ExecuteDataTable();
                if (null != dt && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["VirtualTypeString"] = CodeNamePairManager.GetName("Inventory", "VirtualRequestType", dr["VirtualType"].ToString());
                    }
                }
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
            }
            return dt;
        }

        public DataTable QueryVirtualRequestCloseLog(VirtualRequestQueryFilter queryFilter, out int totalCount)
        {
            if (queryFilter == null)
            {
                totalCount = 0;
                return null;
            }
            DataTable dt = new DataTable();
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = queryFilter.PagingInfo.SortBy,
                StartRowIndex = queryFilter.PagingInfo.PageIndex * queryFilter.PagingInfo.PageSize,
                MaximumRows = queryFilter.PagingInfo.PageSize
            };

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("Inventory_QueryVirtualInventoryCloseLog");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, " SysNo DESC"))
            {
                if (queryFilter.SysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "VirtualRequestSysNo",
                    DbType.AnsiStringFixedLength, "@VirtualRequestSysNo", QueryConditionOperatorType.Equal, queryFilter.SysNo.Value.ToString());
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CompanyCode",
                DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, queryFilter.CompanyCode);
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                EnumColumnList list = new EnumColumnList();
                list.Add("ActionType", typeof(VirtualRequestActionType));
                list.Add("VirtualRequestStatus", typeof(VirtualRequestStatus));
                dt = dataCommand.ExecuteDataTable(list);
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
            }
            return dt;
        }

        public DataTable QueryProducts(VirtualRequestQueryProductsFilter filter, out int totalCount)
        {
            totalCount = 0;
            if (filter == null)
            {
                return null;
            }
            CustomDataCommand dataCommand = null;

            if (filter.StockSysNo.HasValue)
            {
                dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("Inventory_QueryVirtualItems");

            }
            else
            {
                dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("Inventory_QueryVirtualItemsWithInventory");
            }
            PagingInfoEntity pageEntity = PagingInfoToPagingInfoEntity(filter.PagingInfo);

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pageEntity, "a.SysNo DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.CompanyCode",
                    DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);

                sqlBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "v.StockType", DbType.AnsiStringFixedLength,
                  "@StockType", QueryConditionOperatorType.NotEqual, "MET");
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "v.StockType", DbType.AnsiStringFixedLength,
                  "@StockType", QueryConditionOperatorType.IsNull, DBNull.Value);
                sqlBuilder.ConditionConstructor.EndGroupCondition();


                sqlBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "v.ShippingType", DbType.AnsiStringFixedLength,
                  "@ShippingType", QueryConditionOperatorType.NotEqual, "MET");
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "v.ShippingType", DbType.AnsiStringFixedLength,
                  "@ShippingType", QueryConditionOperatorType.IsNull, DBNull.Value);
                sqlBuilder.ConditionConstructor.EndGroupCondition();


                sqlBuilder.ConditionConstructor.BeginGroupCondition(QueryConditionRelationType.AND);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "v.InvoiceType", DbType.AnsiStringFixedLength,
                  "@InvoiceType", QueryConditionOperatorType.NotEqual, "MET");
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "v.InvoiceType", DbType.AnsiStringFixedLength,
                  "@InvoiceType", QueryConditionOperatorType.IsNull, DBNull.Value);
                sqlBuilder.ConditionConstructor.EndGroupCondition();

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.ProductID",
                    DbType.String, "@ItemCode", QueryConditionOperatorType.Like, filter.ProductID);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.ProductName",
                    DbType.String, "@ItemName", QueryConditionOperatorType.Like, filter.ProductName);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.ProductType",
                    DbType.Int32, "@ItemType", QueryConditionOperatorType.Equal, GetPorductTypeEnumInt(filter.ProductType));
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.Status",
                    DbType.Int32, "@Status", filter.Operator == VirtualRequestQueryProductsFilter.OperationType.Equal ? QueryConditionOperatorType.Equal : QueryConditionOperatorType.NotEqual, (int?)filter.Status);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.PMUserSysNo",
                    DbType.Int32, "@PMSysNumber", QueryConditionOperatorType.Equal, filter.PMSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "b.stockSysNo",
                    DbType.Int32, "@WarehouseSysNumber", QueryConditionOperatorType.Equal, filter.StockSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "d.Category1Sysno",
                    DbType.Int32, "@Category1SysNumber", QueryConditionOperatorType.Equal, filter.Category1SysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "d.Category2Sysno",
                    DbType.Int32, "@Category2SysNumber", QueryConditionOperatorType.Equal, filter.Category2SysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "d.Category3Sysno",
                    DbType.Int32, "@Category3SysNumber", QueryConditionOperatorType.Equal, filter.Category3SysNo);
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();


                DataTable dt = dataCommand.ExecuteDataTable(new EnumColumnList() 
                { 
                    { "Status", typeof(ECCentral.BizEntity.IM.ProductStatus) }
                });

                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        public DataTable QueryVirtualInventoryInfoByStock(VirtualRequestQueryFilter queryFilter)
        {
            DataTable dt = new DataTable();
            if (queryFilter.ProductSysNo.HasValue)
            {
                var dataCommand = DataCommandManager.GetDataCommand("QueryInventoryByStock");

                dataCommand.SetParameterValue("@ItemSysNumber", queryFilter.ProductSysNo);

                dataCommand.SetParameterValue("@CompanyCode", queryFilter.CompanyCode);
                dt = dataCommand.ExecuteDataTable();
            }
            return dt;
        }

        public DataTable QueryVirtualInventoryLastVerifiedRequest(VirtualRequestQueryFilter queryFilter)
        {
            DataTable dt = new DataTable();
            if (queryFilter.ProductSysNo.HasValue)
            {
                DataCommand dataCommand = DataCommandManager.GetDataCommand("GetLastVerifiedRequest");

                dataCommand.SetParameterValue("@ProductSysNo", queryFilter.ProductSysNo.Value);
                dataCommand.SetParameterValue("@CompanyCode", queryFilter.CompanyCode);
                EnumColumnList list = new EnumColumnList();
                list.Add("Status", typeof(VirtualRequestStatus));
                dt = dataCommand.ExecuteDataTable(list);
                if (null != dt && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["VirtualTypeString"] = CodeNamePairManager.GetName("Inventory", "VirtualRequestType", dr["VirtualType"].ToString());
                    }
                }
            }
            return dt;
        }

        public DataTable QueryModifiedVirtualRequest(VirtualRequestQueryFilter queryCriteria, out int totalCount)
        {
            DataTable dt = new DataTable();
            if (queryCriteria == null)
            {
                totalCount = 0;
                return null;
            }
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                StartRowIndex = 0,
                MaximumRows = int.MaxValue
            };

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryVirtualRequest");

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText,
                dataCommand, null, "a.CreateTime DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.CompanyCode",
                    DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, queryCriteria.CompanyCode);
                if (queryCriteria.RequestStatus.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.Status",
              DbType.Int32, "@Status", QueryConditionOperatorType.Equal, (int)queryCriteria.RequestStatus);

                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.ProductSysNo",
                    DbType.Int32, "@ProductSysNo", QueryConditionOperatorType.Equal, queryCriteria.ProductSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.SysNo",
    DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, queryCriteria.SysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.StockSysNo",
DbType.Int32, "@StockSysNo", QueryConditionOperatorType.Equal, queryCriteria.StockSysNo);


                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.CreateUserSysNo",
DbType.Int32, "@CreateUserSysNo", QueryConditionOperatorType.Equal, queryCriteria.CreateUserSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.VirtualType",
    DbType.Int32, "@VirtualType", QueryConditionOperatorType.Equal, queryCriteria.VirtualRequestType);

                if (queryCriteria.StartDate.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.CreateTime",
                        DbType.DateTime, "@DateFrom", QueryConditionOperatorType.MoreThanOrEqual, queryCriteria.StartDate);
                }

                if (queryCriteria.EndDate.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.CreateTime",
                        DbType.DateTime, "@DateTo", QueryConditionOperatorType.LessThan, queryCriteria.EndDate.Value.AddDays(1));
                }

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                dataCommand.CommandText = dataCommand.CommandText.Replace("#BY#", "CreateDate DESC");
                dt = dataCommand.ExecuteDataTable(new EnumColumnList() 
                { 
                    { "RequestStatus", typeof(ECCentral.BizEntity.Inventory.VirtualRequestStatus) }
                });

                if (null != dt && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["VirtualTypeString"] = CodeNamePairManager.GetName("Inventory", "VirtualRequestType", dr["VirtualType"].ToString());
                    }
                }

                totalCount = dt.Rows.Count;
            }
            return dt;
        }

        private int? GetPorductTypeEnumInt(ProductType? type)
        {
            if (type.HasValue)
            {
                switch (type.Value)
                {
                    case ProductType.Normal:
                        return 0;

                    case ProductType.OpenBox:
                        return 1;

                    case ProductType.Bad:
                        return 2;

                    default:
                        return null;

                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 查询虚库日志创建者列表
        /// </summary>
        /// <returns></returns>
        public DataTable QueryVirtualRequestMemoCreateUserList(string companyCode, out int totalCount)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Inventory_QueryVirtualRequestMemoCreateUserList");
            dataCommand.SetParameterValue("@CompanyCode", companyCode);
            DataTable dt = dataCommand.ExecuteDataTable();
            totalCount = dt == null ? 0 : dt.Rows.Count;
            return dt;
        }

        /// <summary>
        /// 查询虚库申请单创建者列表
        /// </summary>
        /// <returns></returns>
        public DataTable QueryVirtualRequestCreateUserList(string companyCode, out int totalCount)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Inventory_QueryVirtualRequestCreateUserList");
            dataCommand.SetParameterValue("@CompanyCode", companyCode);
            DataTable dt = dataCommand.ExecuteDataTable();
            totalCount = dt == null ? 0 : dt.Rows.Count;
            return dt;
        }
    }
}
