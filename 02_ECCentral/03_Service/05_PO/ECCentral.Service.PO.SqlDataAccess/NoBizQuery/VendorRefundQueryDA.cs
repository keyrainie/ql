using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.PO.IDataAccess.NoBizQuery;
using ECCentral.QueryFilter.PO;
using System.Data;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.PO;
using ECCentral.Service.Utility;
using System.Text.RegularExpressions;

namespace ECCentral.Service.PO.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IVendorRefundQueryDA))]
    public class VendorRefundQueryDA : IVendorRefundQueryDA
    {
        #region IVendorRefundQueryDA Members

        public DataTable QueryRMARefundList(VendorRMARefundQueryFilter queryFilter, out int totalCount)
        {
            DataTable dt = new DataTable();
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = queryFilter.PageInfo.SortBy,
                StartRowIndex = queryFilter.PageInfo.PageIndex * queryFilter.PageInfo.PageSize,
                MaximumRows = queryFilter.PageInfo.PageSize
            };
            CustomDataCommand command = null;

            command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryVendorRefundMaster");

            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, pagingInfo, "vrm.[SysNo] DESC"))
            {

                if (queryFilter.CreateDateFrom.HasValue)
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vrm.CreateTime",
                    DbType.DateTime, "@CreateTime", QueryConditionOperatorType.MoreThanOrEqual, queryFilter.CreateDateFrom.Value);
                }

                if (queryFilter.CreateDateTo.HasValue)
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vrm.CreateTime",
                    DbType.DateTime, "@CreateTimeTo", QueryConditionOperatorType.LessThan, queryFilter.CreateDateTo.Value.AddDays(1));
                }

                if (queryFilter.VendorSysNo.HasValue)
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vrm.VendorSysNo",
                    DbType.Int32, "@VendorSysNo", QueryConditionOperatorType.Equal, queryFilter.VendorSysNo.Value);
                }

                if (!string.IsNullOrEmpty(queryFilter.VendorRefundSysNo))
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vrm.SysNo",
                    DbType.String, "@SysNo", QueryConditionOperatorType.Equal, queryFilter.VendorRefundSysNo);
                }


                if (queryFilter.Status.HasValue)
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vrm.Status",
                    DbType.Int32, "@Status", QueryConditionOperatorType.Equal, (int)queryFilter.Status.Value);
                }

                if (queryFilter.PayType.HasValue)
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vrm.PayType",
                    DbType.Int32, "@PayType", QueryConditionOperatorType.Equal, (int)queryFilter.PayType.Value);
                }

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "vrm.CompanyCode",
                    DbType.Int32, "@CompanyCode", QueryConditionOperatorType.Equal, queryFilter.CompanyCode);

                System.Text.StringBuilder strSql = new System.Text.StringBuilder();
                //TODO:PM List
                if (queryFilter.PMSysNo.HasValue)
                {
                    strSql.Append(" AND p.PMUserSysNo = " + queryFilter.PMSysNo.Value);
                }
                //else
                //{
                //    if (condition.Condition.PMSysNoList.Count > 0)
                //    {
                //        strSql.Append(" AND p.PMUserSysNo IN ( ");
                //        for (int i = 0; i < condition.Condition.PMSysNoList.Count; i++)
                //        {
                //            if (i == 0)
                //            {
                //                strSql.Append(condition.Condition.PMSysNoList[i].ToString());
                //            }
                //            else
                //            {
                //                strSql.Append(" , " + condition.Condition.PMSysNoList[i].ToString());
                //            }
                //        }
                //        strSql.Append(" ) ");
                //    }
                //}

                if (queryFilter.ProductSysNo.HasValue)
                {
                    strSql.Append(" AND p.SysNo = " + queryFilter.ProductSysNo.Value);
                }

                if (!string.IsNullOrEmpty(queryFilter.RMARegisterSysNo))
                {
                    strSql.Append(" AND item.RegisterSysNo = " + queryFilter.RMARegisterSysNo);
                }

                command.CommandText = builder.BuildQuerySql().Replace("AppendSql", strSql.ToString());

                EnumColumnList enumColumnList = new EnumColumnList();
                enumColumnList.Add("Status", typeof(VendorRefundStatus));
                enumColumnList.Add("PayType", typeof(VendorRefundPayType));
                dt = command.ExecuteDataTable(enumColumnList);
                totalCount = (int)command.GetParameterValue("@TotalCount");

                return dt;
            }
        }

        #endregion
    }
}
