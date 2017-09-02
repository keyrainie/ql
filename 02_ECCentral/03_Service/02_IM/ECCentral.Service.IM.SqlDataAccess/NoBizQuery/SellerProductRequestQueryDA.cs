//************************************************************************
// 用户名				泰隆优选
// 系统名				商家商品管理
// 子系统名		        商家商品管理NoBizQuery查询接口实现
// 作成者				Kevin
// 改版日				2012.6.8
// 改版内容				新建
//************************************************************************

using System;
using System.Data;
using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.PO;
using System.Collections.Generic;

namespace ECCentral.Service.IM.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(ISellerProductRequestQueryDA))]
    class SellerProductRequestQueryDA : ISellerProductRequestQueryDA
    {
        public DataTable QuerySellerProductRequest(SellerProductRequestQueryFilter queryCriteria, out int totalCount)
        {
            var dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QuerySellerProductRequest");
            var pagingInfo = new PagingInfoEntity
            {
                SortField = queryCriteria.PagingInfo.SortBy,
                StartRowIndex = queryCriteria.PagingInfo.PageIndex * queryCriteria.PagingInfo.PageSize,
                MaximumRows = queryCriteria.PagingInfo.PageSize
            };
            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "A.SysNo DESC"))
            {
                if (queryCriteria.C1SysNo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "E.C1Sysno",
                        DbType.Int32, "@C1SysNo",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.C1SysNo.Value);
                }

                if (queryCriteria.C2SysNo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "E.Sysno",
                        DbType.Int32, "@C2SysNo",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.C2SysNo.Value);
                }

                if (queryCriteria.C3SysNo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "D.Sysno",
                        DbType.Int32, "@C3SysNo",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.C3SysNo.Value);
                }

                if (queryCriteria.ProductSysNo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "B.SysNo",
                        DbType.Int32, "@ProductSysNo",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.ProductSysNo.Value);
                }

                if (!string.IsNullOrEmpty(queryCriteria.ProductID))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "B.ProductID",
                        DbType.String, "@ProductID",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.ProductID);
                }

                if (queryCriteria.RequestStartDate != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "A.InDate",
                        DbType.DateTime, "@RequestStartDate",
                        QueryConditionOperatorType.MoreThanOrEqual,
                        queryCriteria.RequestStartDate.Value);
                }

                if (queryCriteria.RequestEndDate != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "A.InDate",
                        DbType.DateTime, "@RequestEndDate",
                        QueryConditionOperatorType.LessThan,
                        queryCriteria.RequestEndDate.Value);
                }

                if (!string.IsNullOrEmpty(queryCriteria.ProductName))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "A.ProductName ",
                        DbType.String, "@ProductName",
                        QueryConditionOperatorType.Like,
                        queryCriteria.ProductName);
                }

                if (!string.IsNullOrEmpty(queryCriteria.Auditor))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "A.Auditor",
                        DbType.String, "@Auditor",
                        QueryConditionOperatorType.Like,
                        queryCriteria.Auditor);
                }

                if (!string.IsNullOrEmpty(queryCriteria.EditUser))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "A.EditUser",
                        DbType.String, "@EditUser",
                        QueryConditionOperatorType.Like,
                        queryCriteria.EditUser);
                }

                if (!string.IsNullOrEmpty(queryCriteria.CommonSKUNumber))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "A.CommonSKUNumber",
                        DbType.String, "@CommonSKUNumber",
                        QueryConditionOperatorType.Like,
                        queryCriteria.CommonSKUNumber);
                }


                if (!queryCriteria.Status.ToString().Equals("0"))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                      "A.Status",
                      DbType.String, "@Status",
                      QueryConditionOperatorType.Equal,
                      queryCriteria.Status);
                }

                if (!queryCriteria.Type.ToString().Equals("0"))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                      "A.Type",
                      DbType.String, "@Type",
                      QueryConditionOperatorType.Equal,
                      queryCriteria.Type);
                }
                //else
                //{
                //      sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                //      "A.Type",
                //      DbType.String, "@Type",
                //      QueryConditionOperatorType.In,
                //      new List<object> { SellerProductRequestType.NewCreated, SellerProductRequestType.ParameterUpdate });
                //}

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                var enumList = new EnumColumnList { { "Status", typeof(SellerProductRequestStatus) }
                                                  , { "Type", typeof(SellerProductRequestType) }
                                                  , { "IsTakePictures", typeof(SellerProductRequestTakePictures) }
                                                  , { "IsConsign", typeof(VendorConsignFlag) }
                                                  , { "IsOfferInvoice",typeof(SellerProductRequestOfferInvoice)}};

                DataTable dt = dataCommand.ExecuteDataTable(enumList);

                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                return dt;
            }

        }
    }
}
