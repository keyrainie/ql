using ECCentral.QueryFilter.Inventory.Request;
using ECCentral.Service.Inventory.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Inventory.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IProductBatchQueryDA))]
    public class ProductBatchQuery: IProductBatchQueryDA
    {
        /// <summary>
        /// 查询商品批次信息
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public virtual DataTable QueryProductBatch(ProductBatchQueryFilter filter,out int totalCount)
        {
            PagingInfoEntity pagingEntity = new PagingInfoEntity();
            pagingEntity.SortField = "batch.BatchNumber";
            pagingEntity.MaximumRows = filter.PagingInfo.PageSize;
            pagingEntity.StartRowIndex = filter.PagingInfo.PageIndex * filter.PagingInfo.PageSize;

            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetProductBatchInfoByProductAndStock");

            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText,cmd, pagingEntity, "batch.BatchNumber"))
            {
                if (filter.ProductSysNo.HasValue && filter.ProductSysNo > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                                                                 , "p.SysNo"
                                                                 , DbType.Int32
                                                                 , "@ProductSysNo"
                                                                 , QueryConditionOperatorType.Equal
                                                                 , filter.ProductSysNo);
                }

                if (filter.StockSysNo.HasValue && filter.StockSysNo > 0)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND
                                                                 , "bStock.StockSysNo"
                                                                 , DbType.Int32
                                                                 , "@StockSysNo"
                                                                 , QueryConditionOperatorType.Equal
                                                                 , filter.StockSysNo);
                }

                cmd.CommandText = sqlBuilder.BuildQuerySql();

                var result = cmd.ExecuteDataTable();
                totalCount = Convert.ToInt32(cmd.GetParameterValue("@TotalCount"));

                return result;
            }
        }
    }
}   
