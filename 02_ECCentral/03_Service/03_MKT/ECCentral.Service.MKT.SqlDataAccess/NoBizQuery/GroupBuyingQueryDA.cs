using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.SO;

namespace ECCentral.Service.MKT.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IGroupBuyingQueryDA))]
    public class GroupBuyingQueryDA : IGroupBuyingQueryDA
    {
        public DataSet Query(GroupBuyingQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PagingInfo.SortBy;
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetProductGroupBuyingList");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "M.SysNo DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "M.SysNo", DbType.Int32, "@SysNo",
                    QueryConditionOperatorType.Equal, filter.SysNo);

                if (filter.GroupBuyingCategorySysNo.HasValue && filter.GroupBuyingCategorySysNo.Value > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "M.GroupBuyingCategorySysNo", DbType.Int32, "@GroupBuyingCategorySysNo",
                        QueryConditionOperatorType.Equal, filter.GroupBuyingCategorySysNo.Value);
                }

                if (filter.GroupBuyingAreaSysNo > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "M.GroupBuyingAreaSysNo", DbType.Int32, "@GroupBuyingAreaSysNo",
                        QueryConditionOperatorType.Equal, filter.GroupBuyingAreaSysNo);
                }

                if (filter.GroupBuyingVendorSysNo > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "M.VendorSysNo", DbType.Int32, "@VendorSysNo",
                        QueryConditionOperatorType.Equal, filter.GroupBuyingVendorSysNo);
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "P.C1SysNo", DbType.Int32, "@C1SysNo",
                    QueryConditionOperatorType.Equal, filter.C1SysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "P.C2SysNo", DbType.Int32, "@C2SysNo",
                    QueryConditionOperatorType.Equal, filter.C2SysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "P.C3SysNo", DbType.Int32, "@C3SysNo",
                    QueryConditionOperatorType.Equal, filter.C3SysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "P.ProductID", DbType.String, "@ProductID",
                    QueryConditionOperatorType.Equal, filter.ProductID);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "P.SysNo", DbType.Int32, "@ProductSysNo",
                    QueryConditionOperatorType.Equal, filter.ProductSysNo);

                #region 时间过滤
                if (filter.InDateTo != null) { filter.InDateTo = (filter.InDateTo == filter.InDateFrom ? filter.InDateTo.Value.AddDays(1) : filter.InDateTo); }
                if (filter.BeginDateTo != null) { filter.BeginDateTo = (filter.BeginDateTo == filter.BeginDateFrom ? filter.BeginDateTo.Value.AddDays(1) : filter.BeginDateTo); }
                if (filter.EndDateTo != null) { filter.EndDateTo = (filter.EndDateTo == filter.EndDateFrom ? filter.EndDateTo.Value.AddDays(1) : filter.EndDateTo); }
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "M.InDate", DbType.DateTime, "@InDateFrom",
                     QueryConditionOperatorType.MoreThanOrEqual,
                     filter.InDateFrom);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "M.InDate", DbType.DateTime, "@InDateTo",
                     QueryConditionOperatorType.LessThan,
                     filter.InDateTo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "M.BeginDate", DbType.DateTime, "@BeginDateFrom",
                     QueryConditionOperatorType.MoreThanOrEqual,
                     filter.BeginDateFrom);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "M.BeginDate", DbType.DateTime, "@BeginDateTo",
                     QueryConditionOperatorType.LessThan,
                     filter.BeginDateTo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "M.EndDate", DbType.DateTime, "@EndDateFrom",
                     QueryConditionOperatorType.MoreThanOrEqual,
                     filter.EndDateFrom);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "M.EndDate", DbType.DateTime, "@EndDateTo",
                     QueryConditionOperatorType.LessThan,
                     filter.EndDateTo);
                #endregion

                sqlBuilder.ConditionConstructor.AddCondition(
                   QueryConditionRelationType.AND,
                   "M.Status",
                   DbType.AnsiStringFixedLength,
                   "@Status",
                   QueryConditionOperatorType.Equal,
                 filter.Status);

                sqlBuilder.ConditionConstructor.AddCondition(
                   QueryConditionRelationType.AND,
                   "c.CategoryType",
                   DbType.Int32,
                   "@CategoryType",
                   QueryConditionOperatorType.Equal,
                 filter.CategoryType);

                sqlBuilder.ConditionConstructor.AddCondition(
                   QueryConditionRelationType.AND,
                   "M.CompanyCode",
                   DbType.AnsiStringFixedLength,
                   "@CompanyCode",
                   QueryConditionOperatorType.Equal,
                 filter.CompanyCode);

                //TODO:添加渠道过滤条件

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                var ds = cmd.ExecuteDataSet();

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    if (row["VendorSysNo"] != DBNull.Value && Convert.ToInt32(row["VendorSysNO"].ToString()) == 0)
                    {
                        row["VendorName"] = "泰隆优选";
                    }
                }
                //cmd.ExecuteDataTable();

                cmd.ConvertEnumColumn(ds.Tables[0], new EnumColumnList { { "Status", typeof(GroupBuyingStatus) }, { "GroupBuyingCategoryType", typeof(GroupBuyingCategoryType) } });
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return ds;
            }
        }

        public DataTable Query(int productSysNo)
        {
            GroupBuyingQueryFilter filter = new GroupBuyingQueryFilter()
            {
                ProductSysNo = productSysNo
            };

            int count = 0;
            return Query(filter, out count).Tables[0];
        }

        public DataTable QueryFeedback(GroupBuyingFeedbackQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PagingInfo.SortBy;
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryGroupBuyingFeedback");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "SysNo DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "Type", DbType.Int32, "@FeedbackType",
                    QueryConditionOperatorType.Equal, filter.FeedbackType);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "CreateDate", DbType.DateTime, "@CreateDateFrom",
                    QueryConditionOperatorType.MoreThanOrEqual, filter.CreateDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "CreateDate", DbType.DateTime, "@CreateDateTo",
                    QueryConditionOperatorType.LessThanOrEqual, filter.CreateDateTo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "Status", DbType.Int32, "@Status",
                    QueryConditionOperatorType.Equal, filter.Status);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "EditDate", DbType.DateTime, "@ReadDateFrom",
                    QueryConditionOperatorType.MoreThanOrEqual, filter.ReadDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "EditDate", DbType.DateTime, "@ReadDateTo",
                    QueryConditionOperatorType.LessThanOrEqual, filter.ReadDateTo);

                sqlBuilder.ConditionConstructor.AddCondition(
                  QueryConditionRelationType.AND,
                  "CompanyCode",
                  DbType.AnsiStringFixedLength,
                  "@CompanyCode",
                  QueryConditionOperatorType.Equal,
                filter.CompanyCode);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                var cnList = new CodeNamePairColumnList();
                cnList.Add("Type", "MKT", "GroupBuyingFeedBackType");
                var dt = cmd.ExecuteDataTable(new EnumColumnList { { "Status", typeof(GroupBuyingFeedbackStatus) } }, cnList);

                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        public DataTable QueryBusinessCooperation(BusinessCooperationQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PagingInfo.SortBy;
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryBusinessCooperation");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "m.SysNo DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "m.Type", DbType.Int32, "@Type",
                    QueryConditionOperatorType.Equal, filter.GroupBuyingType);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "m.VendorName", DbType.String, "@VendorName",
                    QueryConditionOperatorType.Like, filter.VendorName);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                   "m.ContactTel", DbType.String, "@ContactTel",
                   QueryConditionOperatorType.Like, filter.Telephone);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "m.AreaSysNo", DbType.String, "@AreaSysNo",
                    QueryConditionOperatorType.Equal, filter.AreaSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "m.CreateDate", DbType.DateTime, "@CreateDateFrom",
                    QueryConditionOperatorType.MoreThanOrEqual, filter.CreateDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "m.CreateDate", DbType.DateTime, "@CreateDateTo",
                    QueryConditionOperatorType.LessThanOrEqual, filter.CreateDateTo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "m.Status", DbType.Int32, "@Status",
                    QueryConditionOperatorType.Equal, filter.Status);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "m.EditDate", DbType.DateTime, "@HandleDateFrom",
                    QueryConditionOperatorType.MoreThanOrEqual, filter.HandleDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "m.EditDate", DbType.DateTime, "@HandleDateTo",
                    QueryConditionOperatorType.LessThanOrEqual, filter.HandleDateTo);

                sqlBuilder.ConditionConstructor.AddCondition(
                  QueryConditionRelationType.AND,
                  "m.CompanyCode",
                  DbType.AnsiStringFixedLength,
                  "@CompanyCode",
                  QueryConditionOperatorType.Equal,
                filter.CompanyCode);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                var cnList = new CodeNamePairColumnList();
                cnList.Add("Type", "MKT", "GroupBuyingTypeList");
                var dt = cmd.ExecuteDataTable(new EnumColumnList { { "Status", typeof(BusinessCooperationStatus) } }, cnList);

                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        public DataTable QuerySettlement(GroupBuyingSettlementQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PagingInfo.SortBy;
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryGroupBuyingSettlement");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "m.SysNo DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "m.SysNo", DbType.String, "@SysNo",
                    QueryConditionOperatorType.Equal, filter.SysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "m.CreateDate", DbType.DateTime, "@CreateDateFrom",
                    QueryConditionOperatorType.MoreThanOrEqual, filter.CreateDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "m.CreateDate", DbType.DateTime, "@CreateDateTo",
                    QueryConditionOperatorType.LessThanOrEqual, filter.CreateDateTo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                   "m.SettleDate", DbType.DateTime, "@SettleDateFrom",
                   QueryConditionOperatorType.MoreThanOrEqual, filter.SettleDateDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "m.SettleDate", DbType.DateTime, "@SettleDateTo",
                    QueryConditionOperatorType.LessThanOrEqual, filter.SettleDateDateTo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "m.Status", DbType.Int32, "@Status",
                    QueryConditionOperatorType.Equal, filter.Status);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "FPM.Status", DbType.Int32, "@PayItemStatus",
                    QueryConditionOperatorType.Equal, filter.PayItemStatus);
                sqlBuilder.ConditionConstructor.AddCondition(
                  QueryConditionRelationType.AND,
                  "m.CompanyCode",
                  DbType.AnsiStringFixedLength,
                  "@CompanyCode",
                  QueryConditionOperatorType.Equal,
                filter.CompanyCode);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                var dt = cmd.ExecuteDataTable(new EnumColumnList 
                { 
                    { "Status", typeof(SettlementBillStatus) },
                    {"PayItemStatus",typeof(PayItemStatus)}
                });

                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        public DataTable QueryGroupBuyingTicket(GroupBuyingTicketQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PagingInfo.SortBy;
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryGroupBuyingTicket");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "m.SysNo DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "g.GroupBuyingTitle", DbType.String, "@GroupBuyingTitle",
                    QueryConditionOperatorType.Like, filter.GroupBuyingTitle);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                  "s.Name", DbType.String, "@UsedStoreName",
                  QueryConditionOperatorType.Like, filter.UsedStoreName);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                 "m.TicketID", DbType.String, "@TicketID",
                 QueryConditionOperatorType.Equal, filter.TicketID);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "m.CreateDate", DbType.DateTime, "@CreateDateFrom",
                    QueryConditionOperatorType.MoreThanOrEqual, filter.CreateDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "m.CreateDate", DbType.DateTime, "@CreateDateTo",
                    QueryConditionOperatorType.LessThanOrEqual, filter.CreateDateTo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                   "m.UsedDate", DbType.DateTime, "@UsedDateFrom",
                   QueryConditionOperatorType.MoreThanOrEqual, filter.UsedDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "m.UsedDate", DbType.DateTime, "@UsedDateTo",
                    QueryConditionOperatorType.LessThanOrEqual, filter.UsedDateTo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "m.Status", DbType.Int32, "@Status",
                    QueryConditionOperatorType.Equal, filter.Status);

                sqlBuilder.ConditionConstructor.AddCondition(
                  QueryConditionRelationType.AND,
                  "m.CompanyCode",
                  DbType.AnsiStringFixedLength,
                  "@CompanyCode",
                  QueryConditionOperatorType.Equal,
                filter.CompanyCode);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                var dt = cmd.ExecuteDataTable(new EnumColumnList { { "Status", typeof(GroupBuyingTicketStatus) } });

                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        public DataTable LoadGroupBuyingSettlementItemBySettleSysNo(int settlementSysNo)
        {
            var cmd = DataCommandManager.GetDataCommand("LoadGroupBuyingSettlementItemBySettleSysNo");
            cmd.SetParameterValue("SettlementSysNo", settlementSysNo);
            return cmd.ExecuteDataTable();
        }

        public DataTable LoadTicketByGroupBuyingSysNo(int groupBuyingSysNo)
        {
            var cmd = DataCommandManager.GetDataCommand("LoadTicketByGroupBuyingSysNo");
            cmd.SetParameterValue("GroupBuyingSysNo", groupBuyingSysNo);
            return cmd.ExecuteDataTable();
        }
    }
}
