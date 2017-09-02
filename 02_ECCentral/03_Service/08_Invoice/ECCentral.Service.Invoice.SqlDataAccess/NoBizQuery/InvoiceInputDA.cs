using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Invoice.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IInvoiceInputQueryDA))]
    public class InvoiceInputDA : IInvoiceInputQueryDA
    {
        public DataTable QueryInvoiceInput(InvoiceInputQueryFilter query, out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = query.PagingInfo.SortBy;
            pagingEntity.MaximumRows = query.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = query.PagingInfo.PageIndex * query.PagingInfo.PageSize;

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetInvoiceMasterByQuery");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingEntity, "M.DocNo DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND
                    , "M.CompanyCode"
                    , DbType.AnsiStringFixedLength
                    , "@CompanyCode"
                    , QueryConditionOperatorType.Equal
                    , query.CompanyCode);

                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND
                    , "M.DocNo"
                    , DbType.Int32
                    , "@DocNo"
                    , QueryConditionOperatorType.Equal
                    , query.DocNo);

                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND
                    , "M.VendorSysNo"
                    , DbType.Int32
                    , "@VendorSysNo"
                    , QueryConditionOperatorType.Equal
                    , query.VendorSysNo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                    , "M.InDate"
                    , DbType.DateTime
                    , "@CreateDateFrom"
                    , QueryConditionOperatorType.MoreThanOrEqual
                    , query.CreateDateFrom);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                    , "M.InDate"
                    , DbType.DateTime
                    , "@CreateDateTo"
                    , QueryConditionOperatorType.LessThanOrEqual
                    , query.CreateDateTo);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                    , "M.ConfirmDate"
                    , DbType.DateTime
                    , "@ConfirmDateFrom"
                    , QueryConditionOperatorType.MoreThanOrEqual
                    , query.ConfirmDateFrom);

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                    , "M.ConfirmDate"
                    , DbType.DateTime
                    , "@ConfirmDateTo"
                    , QueryConditionOperatorType.LessThanOrEqual
                    , query.ConfirmDateTo);

                if (!string.IsNullOrEmpty(query.POList))
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(
                                       QueryConditionRelationType.AND
                                       , string.Format(@"Exists(SELECT TOP 1 1 
                                                         FROM [OverseaInvoiceReceiptManagement].[dbo].[APInvoice_PO_Item] P WITH(NOLOCK) 
                                                         WHERE P.PoNo IN({0}) AND P.DocNo = M.DocNo )"
                                       , query.POList.Replace(".", ",")));
                }

                if (!string.IsNullOrEmpty(query.APList))
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(
                                       QueryConditionRelationType.AND
                                       , string.Format(@"Exists(SELECT TOP 1 1 
                                                                FROM [OverseaInvoiceReceiptManagement].[dbo].[APInvoice_Invo_Item] A WITH(NOLOCK) 
                                                                WHERE A.InvoiceNo IN({0}) AND A.DocNo = M.DocNo)"
                                       , query.APList.Replace(".", ",")));
                }


                if (query.HasDiff.HasValue)
                {
                    if (query.HasDiff.Value)
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                            , "M.DiffTaxAmt"
                            , DbType.Decimal
                            , "@DiffTaxAmt"
                            , QueryConditionOperatorType.NotEqual
                            , 0m);

                    }
                    else
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                            , "M.DiffTaxAmt"
                            , DbType.Decimal
                            , "@DiffTaxAmt"
                            , QueryConditionOperatorType.Equal
                            , 0m);
                    }
                }

                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND
                    , "M.Status"
                    , DbType.Int32
                    , "@Status"
                    , QueryConditionOperatorType.Equal
                    , query.Status);

                if (query.ComeFrom.HasValue)
                {
                    if (query.ComeFrom.Value == 0)
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(
                            QueryConditionRelationType.AND
                            , "M.VendorPortalSysNo"
                            , DbType.Int32
                            , "@VendorPortalSysNo"
                            , QueryConditionOperatorType.IsNull
                            , query.ComeFrom);
                    }
                    else
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(
                            QueryConditionRelationType.AND
                            , "M.VendorPortalSysNo"
                            , DbType.Int32
                            , "@VendorPortalSysNo"
                            , QueryConditionOperatorType.IsNotNull
                            , query.ComeFrom);
                    }
                }

                if (query.PaySettleCompany.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                        QueryConditionRelationType.AND
                        , "V.PaySettleCompany"
                        , DbType.Int32
                        , "@PaySettleCompany"
                        , QueryConditionOperatorType.Equal
                        , query.PaySettleCompany.Value);
                }

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("Status", typeof(APInvoiceMasterStatus));
                enumList.Add("SapImportedStatus", typeof(SapImportedStatus));
                enumList.Add("DiffTaxTreatmentType", typeof(InvoiceDiffType));
                DataTable dt = dataCommand.ExecuteDataTable(enumList);
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                return dt;

            }

        }

        /// <summary>
        /// 获取付款结算公司
        /// </summary>
        /// <param name="poNo"></param>
        /// <param name="orderType"></param>
        /// <param name="batchNumber"></param>
        /// <returns></returns>
        public virtual int GetPaySettleCompany(int vendorSysNo)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetPaySettleCompany");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                command.CommandText, command, null, "Sysno"
                ))
            {
                sqlBuilder.ConditionConstructor.AddCondition(
                    QueryConditionRelationType.AND,
                    "VendorSysNo",
                    DbType.Int32,
                    "@VendorSysNo",
                    QueryConditionOperatorType.Equal, vendorSysNo);

                command.CommandText = sqlBuilder.BuildQuerySql();
                object obj = command.ExecuteScalar();
                if (obj == DBNull.Value)
                {
                    return 0;
                }
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
    }
}
