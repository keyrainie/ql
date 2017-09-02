using System;
using System.Linq;
using System.Data;

using ECCentral.QueryFilter.MKT;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.MKT;
using System.Collections.Generic;

namespace ECCentral.Service.MKT.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IComboQueryDA))]
    public class ComboQueryDA : IComboQueryDA
    {
        #region IComboQueryDA Members

        public DataTable Query(ComboQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PagingInfo.SortBy;
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;
            if (!string.IsNullOrEmpty(pagingEntity.SortField) && pagingEntity.SortField.Contains( "DiscountAmt"))
            {
                pagingEntity.SortField = pagingEntity.SortField.Replace("DiscountAmt", "Isnull(Sum(si.[Quantity] * si.[Discount]),0)");
            }
            if (!string.IsNullOrEmpty(pagingEntity.SortField) && pagingEntity.SortField.Contains("PriceDiff"))
            {
                pagingEntity.SortField = pagingEntity.SortField.Replace("PriceDiff", "Isnull(Sum(pr.[CurrentPrice]*si.[Quantity] - pr.UnitCost*si.[Quantity]+si.[Quantity]* si.[Discount]),0)");
            }

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("QueryCombo");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "sm.[CreateTime] DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                   "sm.SysNo", DbType.Int32, "@SystemNumber", QueryConditionOperatorType.Equal, filter.SysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "sm.Status", DbType.Int32, "@Status", QueryConditionOperatorType.Equal, filter.Status);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "sm.CreateUserSysNo", DbType.Int32, "@PM", QueryConditionOperatorType.Equal, filter.PM);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sm.SaleRuleName", DbType.AnsiStringFixedLength, 
                    "@SaleRuleName", QueryConditionOperatorType.Equal, filter.SaleRuleName);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sm.SysNo", DbType.Int32,
                    "@SaleRuleName", QueryConditionOperatorType.Equal, filter.RulesSysNo);
                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                //    "p.ProductID", DbType.String, "@ProductID", QueryConditionOperatorType.Equal, filter.ProductID);

                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                //    "si.ProductSysNo", DbType.Int32, "@ProductSysNo", QueryConditionOperatorType.Equal, filter.ProductSysNo);

                if (filter.SysNoList != null && filter.SysNoList.Count > 0)
                {
                    List<object> list = new List<object>();
                    filter.SysNoList.ForEach(p =>
                    {
                        list.Add(p);
                    });
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "sm.SysNo", DbType.Int32, "@SysNoList", QueryConditionOperatorType.In, list);
                }

                //sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                //    "sm.CompanyCode", DbType.String, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);


                if (filter.MerchantSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                   QueryConditionRelationType.AND,
                   "V.SysNo",
                   DbType.Int32,
                   "@MerchantSysNo",
                   QueryConditionOperatorType.Equal,
                   filter.MerchantSysNo);
                }
                //如果大于 1 则查询sysno=MerchantSysNo的记录
                //if (filter.MerchantSysNo > 1)
                //{
                //    sqlBuilder.ConditionConstructor.AddCondition(
                //   QueryConditionRelationType.AND,
                //   "V.SysNo",
                //   DbType.Int32,
                //   "@MerchantSysNo",
                //   QueryConditionOperatorType.Equal,
                //   filter.MerchantSysNo);
                //    sqlBuilder.ConditionConstructor.AddCondition(
                //  QueryConditionRelationType.AND,
                //  "V.VendorType",
                //  DbType.Int32,
                //  "@VendorType",
                //  QueryConditionOperatorType.Equal,
                //  1);
                //}

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                   "isnull(sm.ReferenceType,1)", DbType.Int32, "@ReferenceType", QueryConditionOperatorType.Equal, 1);
                //if (filter.ProductSysNo > 0)
                //{
                //    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                //     "si.ProductSysNo", DbType.Int32, "@ProductSysNo", QueryConditionOperatorType.Equal, filter.ProductSysNo);
                //}
                cmd.CommandText = sqlBuilder.BuildQuerySql();

                //构造商品系统编号，商品编号查询参数
                string strWhere = "";
                if (filter.ProductSysNo.HasValue)
                {
                    strWhere += " and p.SysNo=@ProductSysNo ";
                    cmd.AddInputParameter("@ProductSysNo", DbType.Int32, filter.ProductSysNo);
                }
                if (!string.IsNullOrWhiteSpace(filter.ProductID))
                {
                    strWhere += " and p.ProductID=@ProductID";
                    cmd.AddInputParameter("@ProductID", DbType.AnsiString, filter.ProductID);
                }
                cmd.ReplaceParameterValue("#StrWhere_Product#", strWhere);

                DataTable dt = cmd.ExecuteDataTable(new EnumColumnList {
                    { "Status", typeof(ComboStatus)}                    
                });      
                
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));
                return dt;
            }       
        }

        #endregion
    }
}
