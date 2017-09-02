using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.PO.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.PO.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IPurchaseOrderBasketQueryDA))]
    public class PurchaseOrderBasketQueryDA : IPurchaseOrderBasketQueryDA
    {

        public System.Data.DataTable QueryBasketList(QueryFilter.PO.PurchaseOrderBasketQueryFilter queryFilter, out int totalCount)
        {
            DataTable dt = new DataTable();
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryBasket");
            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = queryFilter.PageInfo.SortBy,
                StartRowIndex = queryFilter.PageInfo.PageIndex * queryFilter.PageInfo.PageSize,
                MaximumRows = queryFilter.PageInfo.PageSize
            };
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(
                command.CommandText, command, pagingInfo, "Basket.SysNo DESC"))
            {
                if (queryFilter != null)
                {
                    //by Jack.W.Wang  2012-12-6 CRL21776--------------------------BEGIN
                    if (!queryFilter.IsManagerPM.HasValue || !queryFilter.IsManagerPM.Value)
                    {
                        string tSubSqlTxt = @"Select 
									ProductLineSysNo 
                    FROM OverseaContentManagement.dbo.V_CM_ProductLine_PMs AS p " +
                "WHERE  PMUserSysNo=" + ServiceContext.Current.UserSysNo + " OR CHARINDEX(';'+CAST(" + ServiceContext.Current.UserSysNo + " AS VARCHAR(20))+';',';'+p.BackupPMSysNoList+';')>0";
                        builder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "line.ProductLineSysNo", QueryConditionOperatorType.In, tSubSqlTxt);
                    }
                    //by Jack.W.Wang  2012-12-6 CRL21776--------------------------END

                    //TODO:获取PM List;
                    #region 权限筛选
                    //var pmHasRight = AuthorizedPMs();
                    //var createUserSysNo = query.Condition.CreateUserSysNo;

                    //if (!string.IsNullOrEmpty(createUserSysNo))
                    //{
                    //    var createIsValid = pmHasRight.Exists(
                    //        x => x == Convert.ToInt32(createUserSysNo));
                    //    if (createIsValid)
                    //    {
                    //        builder.ConditionConstructor.AddCustomCondition(
                    //            QueryConditionRelationType.AND,
                    //            "Basket.CreateUserSysNo in(" + createUserSysNo + ")");
                    //    }
                    //    else
                    //    {
                    //        builder.ConditionConstructor.AddInCondition(
                    //            QueryConditionRelationType.AND,
                    //            "Basket.CreateUserSysNo",
                    //            DbType.Int32,
                    //            new List<int> { -999 });
                    //    }
                    //}
                    //else
                    //{
                    //    builder.ConditionConstructor.AddInCondition(
                    //        QueryConditionRelationType.AND,
                    //        "Basket.CreateUserSysNo",
                    //        DbType.Int32,
                    //        pmHasRight);
                    //}
                    #endregion

                    if (queryFilter.StockSysNo.HasValue)
                    {
                        string stockStr = queryFilter.StockSysNo.ToString();
                        //有中转仓
                        if (stockStr.Length > 2)
                        {
                            builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Basket.StockSysNo", DbType.Int32,
                                "@StockSysNo", QueryConditionOperatorType.Equal, Int32.Parse(stockStr.Substring(2)));
                            builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Basket.IsTransfer", DbType.Int32,
                                "@IsTransfer", QueryConditionOperatorType.Equal, 1);
                        }
                        else
                        {
                            builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Basket.StockSysNo", DbType.Int32, "@StockSysNo", QueryConditionOperatorType.Equal, queryFilter.StockSysNo);
                        }
                    }
                    if (!string.IsNullOrEmpty(queryFilter.CreateUserSysNo))
                    {
                        builder.ConditionConstructor.AddCondition(
                            QueryConditionRelationType.AND,
                            "Basket.CreateUserSysNo",
                            DbType.Int32,
                            "@CreateUserSysNo",
                            QueryConditionOperatorType.Equal,
                            Int32.Parse(queryFilter.CreateUserSysNo));
                    }

                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Basket.CompanyCode", System.Data.DbType.AnsiStringFixedLength,
    "@CompanyCode", QueryConditionOperatorType.Equal, queryFilter.CompanyCode);
                }

                command.CommandText = builder.BuildQuerySql();
            }
            EnumColumnList enumList = new EnumColumnList();
            enumList.Add("PaySettleCompany", typeof(PaySettleCompany));
            dt = command.ExecuteDataTable(enumList);
            totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
            return dt;
        }

    }
}
