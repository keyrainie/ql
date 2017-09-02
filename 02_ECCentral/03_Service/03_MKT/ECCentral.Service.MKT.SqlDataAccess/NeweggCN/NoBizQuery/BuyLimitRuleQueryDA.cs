using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.Service.MKT.SqlDataAccess.NeweggCN.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IBuyLimitRuleQueryDA))]
    public class BuyLimitRuleQueryDA : IBuyLimitRuleQueryDA
    {
        #region IBuyLimitRuleQueryDA Members

        public DataTable Query(BuyLimitRuleQueryFilter filter, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = filter.PagingInfo.SortBy;
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;

            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("NewPromotion_BuyLimitRule_Query");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingEntity, "SysNo DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.LimitType", DbType.Int32, "@LimitType",
                    QueryConditionOperatorType.Equal, filter.LimitType);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.Status", DbType.Int32, "@Status",
                    QueryConditionOperatorType.Equal, filter.Status);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "b.SysNo", DbType.Int32, "@ComboSysNo",
                    QueryConditionOperatorType.Equal, filter.ComboSysNo); 
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                     "b.SaleRuleName", DbType.String, "@ComboName",
                     QueryConditionOperatorType.Like, filter.ComboName);

                if (filter.ProductSysNo.HasValue || !string.IsNullOrEmpty(filter.ProductID))
                {
                    string strWhere = "";
                    if (filter.ProductSysNo.HasValue)
                    {
                        
                        strWhere += "bbb.SysNo=@ProductSysNo ";
                        cmd.AddInputParameter("@ProductSysNo", DbType.Int32, filter.ProductSysNo.Value);
                    }
  
                    if (!string.IsNullOrEmpty(filter.ProductID))
                    {
                        string prepend = strWhere.Length > 0 ? " AND " : "";
                        strWhere += prepend + " bbb.ProductID=@ProductID ";
                        cmd.AddInputParameter("@ProductID", DbType.String, filter.ProductID);
                    }

                    //组商品限购规则查询
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, @"c.SysNo IN (select pp.SysNo from [OverseaContentManagement].[dbo].[ProductCommonInfo] cc
inner join IPP3.dbo.Product pp
on cc.SysNo=pp.ProductCommonInfoSysno
where cc.ProductGroupSysno IN(
	select ProductGroupSysno from [OverseaContentManagement].[dbo].[ProductCommonInfo] ccc
	inner join [IPP3].[dbo].[Product] bbb on bbb.ProductCommonInfoSysno=ccc.SysNo
	where #StrWhere#
)
)".Replace("#StrWhere#", strWhere));
                    
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.BeginDate", DbType.DateTime, "@BeginDate",
                    QueryConditionOperatorType.LessThanOrEqual, filter.BeginDate);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.EndDate", DbType.DateTime, "@EndDate",
                    QueryConditionOperatorType.MoreThanOrEqual, filter.EndDate);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "a.CompanyCode", DbType.String, "@CompanyCode",
                    QueryConditionOperatorType.Equal, filter.CompanyCode);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
                EnumColumnList enumConfig = new EnumColumnList();
                enumConfig.Add("Status", typeof(LimitStatus));
                enumConfig.Add("LimitType", typeof(LimitType));

                var dt = cmd.ExecuteDataTable(enumConfig);
                int.TryParse(cmd.GetParameterValue("@TotalCount").ToString(), out totalCount);

                return dt;
            }
        }

        #endregion
    }
}
