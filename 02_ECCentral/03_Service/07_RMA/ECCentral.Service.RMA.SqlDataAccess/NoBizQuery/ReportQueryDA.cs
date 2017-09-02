using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.RMA.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using System.Data;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.QueryFilter.RMA;
using ECCentral.BizEntity.RMA;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.RMA.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IReportQueryDA))]
    public class ReportQueryDA : IReportQueryDA
    {

        #region IReportQueryDA Members

        public virtual DataTable QueryProductCardInventoryByProductSysNo(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("QueryProductCardInventory");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            return cmd.ExecuteDataTable();
        }

        public virtual DataTable QueryProductCardsByProductSysNo(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("QueryProductCards");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            DataTable dt = cmd.ExecuteDataTable();
            dt.Columns.Add("ThenQty", typeof(int));
            if (dt != null && dt.Rows.Count > 0)
            {
                // 针对一个已按 RecordTime 降序排列的货卡 List
                // 该货卡长度为 M 的话，第 N (0 < N <= M) 个货卡的 ThenQty 值就等于
                // 第 N + 1 个货卡的 ThenQty 与第 N 个货卡的 AffectQty 之和
                // 而第 M 个货卡的 ThenQty 则等于 0 与第 M 个货卡的 AffectQty 之和
                for (int then = 0, i = dt.Rows.Count - 1; i > -1; --i)
                {
                    then += Convert.ToInt32(dt.Rows[i]["AffectQty"]);
                    dt.Rows[i]["ThenQty"] = then;
                }
            }
            return dt;
        }

        public virtual DataTable QueryOutBoundNotReturn(OutBoundNotReturnQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PagingInfo.SortBy;
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryNotReturnPerformance");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, " ob.SysNo DESC "))
            {
                #region conditions

                if (filter.C3SysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "product.Category3SysNo",
                        DbType.Int32,
                        "@C3SysNo",
                        QueryConditionOperatorType.Equal,
                        filter.C3SysNo.Value
                    );
                }
                else if (filter.C2SysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "category.Category2Sysno",
                        DbType.Int32,
                        "@C2SysNo",
                        QueryConditionOperatorType.Equal,
                        filter.C2SysNo.Value
                    );
                }
                else if (filter.C1SysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND,
                        "category.Category1Sysno",
                        DbType.Int32,
                        "@C1SysNo",
                        QueryConditionOperatorType.Equal,
                        filter.C1SysNo.Value
                    );
                }
                if (filter.HasResponse.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddNullCheckCondition(
                        QueryConditionRelationType.AND,
                        "register.ResponseDesc",
                        filter.HasResponse.Value
                            ? QueryConditionOperatorType.IsNotNull : QueryConditionOperatorType.IsNull
                    );
                }
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "ob.OutTime",
                    DbType.DateTime,
                    "@OutTimeFrom",
                    QueryConditionOperatorType.MoreThanOrEqual,
                    filter.OutTimeFrom
                );
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "ob.OutTime",
                    DbType.DateTime,
                    "@OutTimeTo",
                    QueryConditionOperatorType.LessThan,
                    filter.OutTimeTo
                );
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "product.PMUserSysNo",
                    DbType.Int32,
                    "@PMUserSysNo",
                    QueryConditionOperatorType.Equal,
                    filter.PMUserSysNo
                );
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "product.SysNo",
                    DbType.Int32,
                    "@ProductSysNo",
                    QueryConditionOperatorType.Equal,
                    filter.ProductSysNo
                );
                if (filter.SendDays.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(
                        QueryConditionRelationType.AND, "DATEADD(DAY, @SendDays, ob.OutTime) < GETDATE()"
                    );
                    cmd.AddInputParameter("@SendDays", DbType.Int32, filter.SendDays.Value);
                }
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "so.SysNo",
                    DbType.Int32,
                    "@SOSysNo",
                    QueryConditionOperatorType.Equal,
                    filter.SOSysNo
                );
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "ob.VendorSysNo",
                    DbType.Int32,
                    "@VendorSysNo",
                    QueryConditionOperatorType.Equal,
                    filter.VendorSysNo
                );
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "register.RefundStatus",
                    DbType.Int32,
                    "@RefundStatus",
                    QueryConditionOperatorType.Equal,
                    filter.RefundStatus
                );
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "register.RevertStatus",
                    DbType.Int32,
                    "@RevertStatus",
                    QueryConditionOperatorType.Equal,
                    filter.RevertStatus
                );
                // only query Handling register
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "register.Status",
                    DbType.Int32,
                    "@RegisterStatus",
                    QueryConditionOperatorType.Equal,
                    1
                );
                // only query SendAlready outbund
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "register.OutBoundStatus",
                    DbType.Int32,
                    "@OutboundStatus",
                    QueryConditionOperatorType.Equal,
                    1
                );
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "register.CompanyCode",
                       DbType.String, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);
                #endregion

                cmd.CommandText = sqlBuilder.BuildQuerySql();

                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("RefundStatus", typeof(RMARefundStatus));
                enumList.Add("RevertStatus", typeof(RMARevertStatus));
                enumList.Add("Vendor_Status", typeof(VendorStatus));
                CodeNamePairColumnList codeList = new CodeNamePairColumnList();
                codeList.Add("PayPeriodType", "RMA", "VendorPayPeriodType");

                DataTable dt=cmd.ExecuteDataTable(enumList,codeList);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        /// <summary>
        /// NoBizQuery RMA商品库存
        /// </summary>
        /// <param name="filter">查询条件</param>
        /// <param name="totalCount">out 数据数量</param>
        /// <param name="totalMisCost">out 未税（增值税）成本</param>
        /// <returns>DataTable</returns>
        public virtual DataTable QueryRMAProductInventory(RMAInventoryQueryFilter filter, out int totalCount, out decimal totalMisCost)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PagingInfo.SortBy;
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryRMAProductsInventory");

            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, " p.SysNo "))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "R.Status",
                            DbType.Int32, "@Status", QueryConditionOperatorType.Equal, RMARequestStatus.Handling);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "RMA_Request.RecvTime",
                            DbType.DateTime, "@RecvTime", QueryConditionOperatorType.IsNotNull, DBNull.Value);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "P.SysNo",
                        DbType.Int32, "@ProductSysNo", QueryConditionOperatorType.Equal, filter.ProductSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "OwnBy",
                            DbType.Int32, "@OWnBy", QueryConditionOperatorType.Equal, filter.RMAOwnBy);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "LocationWarehouse",
                        DbType.AnsiStringFixedLength, "@LocationWarehouse", QueryConditionOperatorType.Equal, filter.LocationWarehouse);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "OwnByWarehouse",
                        DbType.AnsiStringFixedLength, "@OwnByWarehouse", QueryConditionOperatorType.Equal, filter.OwnByWarehouse);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "R.CompanyCode",
                        DbType.String, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                cmd.AddOutParameter("@TotleMisCost", DbType.String, 20);
                DataTable dt=cmd.ExecuteDataTable();
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                object temp = cmd.GetParameterValue("@TotleMisCost");
                decimal.TryParse(temp != null ? temp.ToString() : string.Empty, out totalMisCost);
                return dt;
            }
        }

        /// <summary>
        /// NoBizQuery RMA单件库存
        /// </summary>
        /// <param name="filter">查询条件</param>
        /// <param name="totalCount">out数据条数</param>
        /// <returns>DataTable</returns>
        public virtual DataTable QueryRMAItemInventory(RMAInventoryQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PagingInfo.SortBy;
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryGetRMAItemsInventory");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, " A.SysNo "))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.Status",
                            DbType.Int32, "@Status", QueryConditionOperatorType.Equal, RMARequestStatus.Handling);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "D.RecvTime",
                            DbType.DateTime, "@RecvTime", QueryConditionOperatorType.IsNotNull, DBNull.Value);


                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "B.SysNo",
                        DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, filter.ProductSysNo);


                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.OwnBy",
                        DbType.Int32, "@OwnBy", QueryConditionOperatorType.Equal, filter.RMAOwnBy);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.SysNo",
                        DbType.Int32, "@RMASysNo", QueryConditionOperatorType.Equal, filter.RMASysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.LocationWarehouse",
                        DbType.AnsiStringFixedLength, "@LocationWarehouse", QueryConditionOperatorType.Equal, filter.LocationWarehouse);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.OwnByWarehouse",
                        DbType.AnsiStringFixedLength, "@OwnByWarehouse", QueryConditionOperatorType.Equal, filter.OwnByWarehouse);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "A.CompanyCode",
                        DbType.String, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("OutBoundStatus", typeof(RMAOutBoundStatus));
                enumList.Add("RevertStatus", typeof(RMARevertStatus));
                enumList.Add("RefundStatus", typeof(RMARefundStatus));
                enumList.Add("NewProductStatus", typeof(RMANewProductStatus));
                enumList.Add("ReturnStatus", typeof(RMAReturnStatus));
                DataTable dt = cmd.ExecuteDataTable(enumList);
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }

        }

        #endregion
    }
}
