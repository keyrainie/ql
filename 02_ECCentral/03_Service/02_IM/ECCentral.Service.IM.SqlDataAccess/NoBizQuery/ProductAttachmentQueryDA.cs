using System;
using System.Data;
using ECCentral.QueryFilter.IM;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.IM.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IProductAttachmentQueryDA))]
    public class ProductAttachmentQueryDA : IProductAttachmentQueryDA
    {
        /// <summary>
        /// 查询商品附件
        /// </summary>
        /// <returns></returns>
        public virtual DataTable QueryProductAttachment(ProductAttachmentQueryFilter queryCriteria, out int totalCount)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryProductAttachment");
            var pagingInfo = new PagingInfoEntity
            {
                SortField = queryCriteria.PagingInfo.SortBy,
                StartRowIndex = queryCriteria.PagingInfo.PageIndex * queryCriteria.PagingInfo.PageSize,
                MaximumRows = queryCriteria.PagingInfo.PageSize
            };

            using (var sqlBuilder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, pagingInfo, "P.ProductSysNo DESC"))
            {
                if (!String.IsNullOrEmpty(queryCriteria.ProductID))
                {
                    dataCommand.AddInputParameter("@ProductID", DbType.String, queryCriteria.ProductID);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "P.ProductID",
                        DbType.String, "@ProductID",
                        QueryConditionOperatorType.Equal,
                        queryCriteria.ProductID);
                }
                if (!String.IsNullOrEmpty(queryCriteria.ProductName))
                {
                    dataCommand.AddInputParameter("@ProductName", DbType.String, queryCriteria.ProductName);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "P.ProductName",
                        DbType.String, "@ProductName",
                        QueryConditionOperatorType.Like,
                        queryCriteria.ProductName);
                }
                if (!String.IsNullOrEmpty(queryCriteria.AttachmentID))
                {
                    dataCommand.AddInputParameter("@AttachmentID", DbType.String, queryCriteria.AttachmentID);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "P.AttachmentSysNo",
                        DbType.String, "@AttachmentID",
                        QueryConditionOperatorType.Like,
                        queryCriteria.AttachmentID);
                }
                if (!String.IsNullOrEmpty(queryCriteria.AttachmentName))
                {
                    dataCommand.AddInputParameter("@AttachmentName", DbType.String, queryCriteria.AttachmentName);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "P.AttachmentName",
                        DbType.String, "@AttachmentName",
                        QueryConditionOperatorType.Like,
                        queryCriteria.AttachmentName);
                }
                if (!String.IsNullOrEmpty(queryCriteria.EditUser))
                {
                    dataCommand.AddInputParameter("@EditUser", DbType.String, queryCriteria.EditUser);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                        "P.InUser",
                        DbType.String, "@EditUser",
                        QueryConditionOperatorType.Like,
                        queryCriteria.EditUser);
                }
                //sqlBuilder.ConditionConstructor.AddBetweenCondition(QueryConditionRelationType.AND, "P.InDate", DbType.DateTime, "@InDate", QueryConditionOperatorType.LessThanOrEqual, QueryConditionOperatorType.MoreThanOrEqual, queryCriteria.InDateEnd, queryCriteria.InDateStart);
                if (queryCriteria.InDateStart != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                                        QueryConditionRelationType.AND,
                                        "P.InDate",
                                        DbType.DateTime,
                                        "@InDateStart",
                                        QueryConditionOperatorType.MoreThanOrEqual,
                                        queryCriteria.InDateStart
                                   );
                }
                if (queryCriteria.InDateEnd != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(
                                  QueryConditionRelationType.AND,
                                  "P.InDate",
                                  DbType.DateTime,
                                  "@InDateEnd",
                                  QueryConditionOperatorType.LessThan,
                                  queryCriteria.InDateEnd
                             );
                }


                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("ProductStatus", typeof(ECCentral.BizEntity.IM.ProductStatus));

                DataTable dt = dataCommand.ExecuteDataTable(enumList);
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                return dt;
            }
        }
    }
}
