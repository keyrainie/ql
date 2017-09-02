using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using ECCentral.Service.Utility;
using ECCentral.Service.Inventory.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Inventory;
using ECCentral.BizEntity.Inventory;
using ECCentral.QueryFilter.PO;

namespace ECCentral.Service.Inventory.SqlDataAccess.NoBizQuery
{
    [VersionExport(typeof(IProductCostQueryDA))]
    public class ProductCostQueryDA : IProductCostQueryDA
    {
        #region IProductCostQueryDA Members

        public virtual DataTable QueryProductCostInList(ProductCostQueryFilter queryFilter, out int totalCount)
        {
            DataTable dt = new DataTable();
            if (queryFilter == null)
            {
                totalCount = 0;
                return null;
            }
            var dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryProductCostInList");

            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = queryFilter.PagingInfo.SortBy,
                StartRowIndex = queryFilter.PagingInfo.PageIndex * queryFilter.PagingInfo.PageSize,
                MaximumRows = queryFilter.PagingInfo.PageSize
            };

            using (var sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "c.[Priority],c.SysNo"))
            {
                if (queryFilter.IsAvailableInventory)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c.LeftQuantity",
                        DbType.Int32, "@LeftQuantity", QueryConditionOperatorType.MoreThan, 0);
                }
                else
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c.LeftQuantity",
                        DbType.Int32, "@LeftQuantity", QueryConditionOperatorType.MoreThanOrEqual, 0);
                }

                if (queryFilter.ProductSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c.ProductSysNo",
                        DbType.Int32, "@ProductSysNo", QueryConditionOperatorType.Equal,
                        queryFilter.ProductSysNo.Value);

                    dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                    dt = dataCommand.ExecuteDataTable();
                    totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
                }
                else
                {
                    totalCount = 0;
                }
            }

            return dt;
        }

        /// <summary>
        /// 查询商品入库成本明细
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        public virtual DataTable QueryAvaliableCostInList(CostChangeItemsQueryFilter queryFilter, out int totalCount)
        {
            DataTable dt = new DataTable();
            if (queryFilter == null)
            {
                totalCount = 0;
                return null;
            }
            var dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryAvaliableCostInList");

            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = queryFilter.PageInfo.SortBy,
                StartRowIndex = queryFilter.PageInfo.PageIndex * queryFilter.PageInfo.PageSize,
                MaximumRows = queryFilter.PageInfo.PageSize
            };

            using (var sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "c.[Priority],c.SysNo"))
            {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c.BillType",
                        DbType.Int32, "@BillType", QueryConditionOperatorType.Equal, (int)ECCentral.BizEntity.Inventory.InventoryAdjustContractInfo.CostBillType.PO);
                    if (queryFilter.ProductSysNo > 0)
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c.ProductSysNo",
                            DbType.Int32, "@ProductSysNo", QueryConditionOperatorType.Equal, queryFilter.ProductSysNo);
                    }
                    else
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "0",
                            DbType.Int32, "@ProductSysNo", QueryConditionOperatorType.Equal, queryFilter.ProductSysNo);
                    }

                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pm.PMSysNo",
                        DbType.Int32, "@PMSysNo", QueryConditionOperatorType.Equal, queryFilter.PMSysNo);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "pm.VendorSysNo",
                        DbType.Int32, "@VendorSysNo", QueryConditionOperatorType.Equal, queryFilter.VendorSysNo);
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c.LeftQuantity-c.LockQuantity",
                        DbType.Int32, "@AvaliableQty", QueryConditionOperatorType.MoreThan, 0);

                    dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                    dt = dataCommand.ExecuteDataTable();
                    totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));
            }

            return dt;
        }

        /// <summary>
        /// 预校验库存成本变更
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        public virtual bool PreCheckCostChange(int ccSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("PreCheckCostChange");
            cmd.SetParameterValue("@CostInSysNo", ccSysNo);
            cmd.SetParameterValue("@BillType", (int)ECCentral.BizEntity.Inventory.InventoryAdjustContractInfo.CostBillType.PO);
            return cmd.ExecuteScalar()==null;
        }
        #endregion

        #region 私有方法
        #endregion 私有方法
    }
}
