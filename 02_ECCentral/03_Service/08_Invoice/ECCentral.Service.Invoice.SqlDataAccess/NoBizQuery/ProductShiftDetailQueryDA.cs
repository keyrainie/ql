using System;
using System.Data;
using System.Linq;

using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Invoice.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Collections.Generic;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Service.Invoice.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IProductShiftDetailQueryDA))]
    public class ProductShiftDetailQueryDA : IProductShiftDetailQueryDA
    {
        private PagingInfoEntity ToPagingInfo(PagingInfo pagingInfo)
        {
            if (pagingInfo == null)
            {
                pagingInfo = new PagingInfo();
                pagingInfo.PageIndex = 0;
                pagingInfo.PageSize = 10;
            }

            return new PagingInfoEntity()
            {
                SortField = pagingInfo.SortBy,
                StartRowIndex = pagingInfo.PageIndex * pagingInfo.PageSize,
                MaximumRows = pagingInfo.PageSize
            };
        }

        public List<ProductShiftDetail> Query(ProductShiftDetailReportQueryFilter filter, out int totalCount
                                                ,ref ProductShiftDetailAmtInfo outAmt,ref ProductShiftDetailAmtInfo inAmt
                                                ,ref ProductShiftDetail needManualItem)
        {
            totalCount = 0;
            List<ProductShiftDetail> result = null;

            if (filter.IsCheckCompany || filter.IsCheckDetail)
            {
                if (filter.IsCheckDetail)
                {
                    result = QueryProductShiftDetailCompanyInfos(filter, out totalCount);

                    if (totalCount > 0)
                    {
                        outAmt = QueryCompanyAmtCountInfo(filter)[0];
                    }
                }
                else
                {
                    List<ProductShiftDetailAmtInfo> listAmtInfo = null;
                    ProductShiftDetail productAdjest = null;
                    result = QueryCompany(filter,ref listAmtInfo,ref productAdjest, out totalCount);
                    if (result.Count > 0 || (listAmtInfo != null && listAmtInfo.Count > 0))
                    {
                        outAmt = listAmtInfo.Find(item => item.CountType.Value == 1);
                        inAmt = listAmtInfo.Find(item => item.CountType.Value == -1);

                        if (result.Count > 0)
                        {
                            var alertItem = result.Find(x => { return x.NeedManual == true; });
                            if (alertItem != null || productAdjest != null)
                            {
                                if (alertItem != null)
                                {
                                    needManualItem = alertItem;
                                }
                                else
                                {
                                    needManualItem = productAdjest;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                result = QueryProductShiftDetail(filter, out totalCount);
            }

            return result;
        }

        List<ProductShiftDetail> QueryProductShiftDetail(ProductShiftDetailReportQueryFilter query, out int totalCount)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryProductShiftDetail");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, ToPagingInfo(query.PagingInfo), "stItem.ShiftSysNo asc"))
            {
                sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "1=1");

                if (!string.IsNullOrEmpty(query.GoldenTaxNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                       "stItem.GoldenTaxNo",
                     DbType.String,
                     "@GoldenTaxNo",
                     QueryConditionOperatorType.Equal,
                     query.GoldenTaxNo);
                }
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "st.StockSysNoA",
                    DbType.Int32, "@StockSysNoA", QueryConditionOperatorType.Equal, query.StockSysNoA);

                if (!string.IsNullOrEmpty(query.StockSysNoB.ToString())) //dropdrow取下来的是？ all/全部
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "st.StockSysNoB",
                       DbType.Int32, "@StockSysNoB", QueryConditionOperatorType.Equal, query.StockSysNoB);
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "st.OutTime",
                    DbType.DateTime, "@OutTimeStart", QueryConditionOperatorType.MoreThanOrEqual, query.OutTimeStart);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "st.OutTime",
                     DbType.DateTime, "@OutTimeEnd", QueryConditionOperatorType.LessThanOrEqual, query.OutTimeEnd);

                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "st.CompanyCode",
                    DbType.AnsiStringFixedLength,
                    "@CompanyCode",
                    QueryConditionOperatorType.Equal,
                    query.CompanyCode);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                var dt = dataCommand.ExecuteEntityList<ProductShiftDetail>();
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        List<ProductShiftDetail> QueryProductShiftDetailCompanyInfos(ProductShiftDetailReportQueryFilter query, out int totalCount)
        {

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryProductShiftDetailCompanyInfos");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, ToPagingInfo(query.PagingInfo), "shiftDetail.SysNo desc"))
            {
                sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "1=1");

                if (!string.IsNullOrEmpty(query.GoldenTaxNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                       "shiftDetail.TaxNO",
                     DbType.String,
                     "@GoldenTaxNo",
                     QueryConditionOperatorType.Equal,
                     query.GoldenTaxNo);
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "shiftDetail.OutCompany",
                     DbType.String, "@OutCompany", QueryConditionOperatorType.Equal, query.OutCompany);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "shiftDetail.InCompany",
                                    DbType.String, "@InCompany", QueryConditionOperatorType.Equal, query.EnterCompany);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "shift.OutTime",
                    DbType.DateTime, "@OutTimeStart", QueryConditionOperatorType.MoreThanOrEqual, query.OutTimeStart);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "shift.OutTime",
                     DbType.DateTime, "@OutTimeEnd", QueryConditionOperatorType.LessThanOrEqual, query.OutTimeEnd);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "shiftDetail.CompanyCode",
                    DbType.StringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, query.CompanyCode);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                var dt = dataCommand.ExecuteEntityList <ProductShiftDetail>();
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                return dt;
            }
        }

        List<ProductShiftDetailAmtInfo> QueryCompanyAmtCountInfo(ProductShiftDetailReportQueryFilter query)
        {
            if (query.IsCheckDetail)
            {
                CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryProductShiftDetailCompanyInfosCountInfo");
                using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                    dataCommand.CommandText, dataCommand, ToPagingInfo(query.PagingInfo), "shiftDetail.SysNo asc"))
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "1=1");

                    if (!string.IsNullOrEmpty(query.GoldenTaxNo))
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                           "shiftDetail.TaxNO",
                         DbType.String,
                         "@GoldenTaxNo",
                         QueryConditionOperatorType.Equal,
                         query.GoldenTaxNo);
                    }

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "shiftDetail.OutCompany",
                         DbType.String, "@OutCompany", QueryConditionOperatorType.Equal, query.OutCompany);

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "shiftDetail.InCompany",
                                        DbType.String, "@InCompany", QueryConditionOperatorType.Equal, query.EnterCompany);

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "shift.OutTime",
                        DbType.DateTime, "@OutTimeStart", QueryConditionOperatorType.MoreThanOrEqual, query.OutTimeStart);

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "shift.OutTime",
                         DbType.DateTime, "@OutTimeEnd", QueryConditionOperatorType.LessThanOrEqual, query.OutTimeEnd);

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "shift.CompanyCode",
                    DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, query.CompanyCode);

                    dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                    return dataCommand.ExecuteEntityList<ProductShiftDetailAmtInfo>();
                }
            }
            else
            {

                CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryProductShiftDetailCompanyAmt");
                using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                    dataCommand.CommandText, dataCommand, ToPagingInfo(query.PagingInfo), "ShiftQty desc"))
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "1=1");

                    dataCommand.AddInputParameter("@SapCoCodeFrom", DbType.String, query.OutCompany);
                    dataCommand.AddInputParameter("@SapCoCodeTo", DbType.String, query.EnterCompany);

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "st.OutTime",
                        DbType.DateTime, "@OutTimeStart", QueryConditionOperatorType.MoreThanOrEqual, query.OutTimeStart);

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "st.OutTime",
                         DbType.DateTime, "@OutTimeEnd", QueryConditionOperatorType.LessThanOrEqual, query.OutTimeEnd);

                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, " stItem.GoldenTaxNo is null ");

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "st.CompanyCode",
                        DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, query.CompanyCode);

                    dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                    return dataCommand.ExecuteEntityList<ProductShiftDetailAmtInfo>();
                }
            }
        }

        private List<ProductShiftDetail> QueryCompany(ProductShiftDetailReportQueryFilter filter,ref List<ProductShiftDetailAmtInfo> listAmtInfo,ref ProductShiftDetail productAdjest, out int totalCount)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryProductShiftDetailCompany");
            List<ProductShiftDetail> result = new List<ProductShiftDetail>();

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, ToPagingInfo(filter.PagingInfo), "ShiftQty desc"))
            {
                sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "1=1");

                if (!string.IsNullOrEmpty(filter.GoldenTaxNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                       "stItem.GoldenTaxNo",
                     DbType.String,
                     "@GoldenTaxNo",
                     QueryConditionOperatorType.Equal,
                     filter.GoldenTaxNo);
                }

                dataCommand.AddInputParameter("@SapCoCodeFrom", DbType.String, filter.OutCompany);

                dataCommand.AddInputParameter("@SapCoCodeTo", DbType.String, filter.EnterCompany);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "st.OutTime",
                    DbType.DateTime, "@OutTimeStart", QueryConditionOperatorType.MoreThanOrEqual, filter.OutTimeStart);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "st.OutTime",
                     DbType.DateTime, "@OutTimeEnd", QueryConditionOperatorType.LessThanOrEqual, filter.OutTimeEnd);

                sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, " stItem.GoldenTaxNo is null ");

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "st.CompanyCode",
                    DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, filter.CompanyCode);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                var transferBefore = dataCommand.ExecuteEntityList<ProductShiftDetail>();

                listAmtInfo = QueryCompanyAmtCountInfo(filter);
                var transferResult = CollideOutAmt(listAmtInfo, transferBefore);
                totalCount = transferResult == null ? 0 : transferResult.Count;

                var rowIndex = filter.PagingInfo.PageIndex.Value * filter.PagingInfo.PageSize.Value;
                var pageSize = filter.PagingInfo.PageSize.Value;
                result = transferResult == null ? null : transferResult.Skip(rowIndex).Take(pageSize).ToList();

                //加调整项/////////////////////////////////
                productAdjest = transferResult.Find(x => { return x.NeedManual == true; });
                /////////////////////////////////////////////////////

                List<ShiftSysnoProduct> listAtoB = GetStockAToStockB(filter, GetListString(transferResult));
                for (int i = 0; i < transferResult.Count; i++)
                {
                    List<ShiftSysnoProduct> listShiftAtoB = listAtoB.Where(item => item.ProductSysNo == transferResult[i].ProductSysNo).ToList();
                    if (transferResult[i].ShiftQty.HasValue && transferResult[i].ShiftQty > 0)
                    {
                        if (listShiftAtoB.Count > 0)
                        {
                            transferResult[i].ShiftSysNo = listShiftAtoB[0].ShiftSysNo;
                            transferResult[i].OutTime = listShiftAtoB[0].OutTime;
                            transferResult[i].StockNameB = listShiftAtoB[0].StockNameB;
                        }
                    }
                }

                return result;
            }
        }

        private List<ShiftSysnoProduct> GetStockAToStockB(ProductShiftDetailReportQueryFilter query, string prodtuctSysnos)
        {
            List<ShiftSysnoProduct> result = null;
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryProductShiftDetailCompanyProductInfoStackAToStackB");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
               dataCommand.CommandText, dataCommand, ToPagingInfo(query.PagingInfo), "ProductSysNo asc"))
            {
                sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, "1=1");

                dataCommand.AddInputParameter("@SapCoCodeFrom", DbType.String, query.OutCompany);
                dataCommand.AddInputParameter("@SapCoCodeTo", DbType.String, query.EnterCompany);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "st.OutTime",
                    DbType.DateTime, "@OutTimeStart", QueryConditionOperatorType.MoreThanOrEqual, query.OutTimeStart);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "st.OutTime",
                     DbType.DateTime, "@OutTimeEnd", QueryConditionOperatorType.LessThanOrEqual, query.OutTimeEnd);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "st.CompanyCode",
                     DbType.String, "@CompanyCode", QueryConditionOperatorType.LessThanOrEqual, query.CompanyCode);

                sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, " stItem.GoldenTaxNo is null ");

                if (prodtuctSysnos.Length < 1000)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, " stItem.ProductSysNo in  (" + prodtuctSysnos + ") ");
                }
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                result = dataCommand.ExecuteEntityList<ShiftSysnoProduct>();
            }

            return result;
        }

        private string GetListString(List<ProductShiftDetail> transferResult)
        {
            string strProduct = "-9999999";
            foreach (var productShift in transferResult)
            {
                if (productShift.ProductSysNo.HasValue)
                {
                    strProduct += "," + productShift.ProductSysNo.Value.ToString();
                }
            }
            return strProduct;
        }

        private List<ProductShiftDetail> CollideOutAmt(List<ProductShiftDetailAmtInfo> info, List<ProductShiftDetail> transferBefore)
        {
            var transferResult = new List<ProductShiftDetail>();
            var inAmtInfo = info.Where(item => item.CountType.Value == -1).FirstOrDefault();
            decimal inAmt;

            if (inAmtInfo != null &&
                inAmtInfo.AmtProductCost.HasValue &&
                inAmtInfo.AmtProductCost > 0)
            {
                inAmt = inAmtInfo.AmtProductCost ?? 0M;
                for (int i = 0; i < transferBefore.Count; i++)
                {
                    if (inAmt >= transferBefore[i].AmtProductCost)
                    {
                        inAmt = inAmt - transferBefore[i].AmtProductCost ?? 0;
                    }
                    else
                    {
                        transferResult = transferBefore.Skip(i).ToList();
                        var findout = transferResult
                            .FindAll(x => { return x.UnitCost > 0 && ((x.AmtProductCost - inAmt) / x.UnitCost > 1); })
                            .OrderByDescending(x => x.AmtProductCost)
                            .FirstOrDefault();
                        if (findout != null
                            && findout.UnitCost.HasValue
                            && findout.UnitCost > 0)
                        {
                            findout.AmtProductCost = findout.AmtProductCost - inAmt;
                            findout.ShiftQty = (int)(findout.AmtProductCost / findout.UnitCost);
                            //优化公式：（（列表中当前总金额/数量）-单位成本）/单位成本
                            findout.AjustRate = findout.AmtProductCost / (findout.ShiftQty * findout.UnitCost) - 1;
                        }
                        else
                        {
                            transferResult.Add(new ProductShiftDetail
                            {
                                AmtProductCost = -inAmt,
                                NeedManual = true
                            });
                        }
                        break;
                    }
                }
            }
            else
            {
                transferResult = transferBefore;
            }
            return transferResult;
        }
    }
}
