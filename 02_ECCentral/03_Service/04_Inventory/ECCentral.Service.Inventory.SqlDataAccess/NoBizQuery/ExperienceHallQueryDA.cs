using ECCentral.BizEntity.Inventory;
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
    [VersionExport(typeof(IExperienceHallQueryDA))]
    class ExperienceHallQueryDA : IExperienceHallQueryDA
    {
        /// <summary>
        /// 体验厅库存查询
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable QueryExperienceHallInventory(QueryFilter.Inventory.ExperienceHallInventoryInfoQueryFilter queryFilter, out int totalCount)
        {
            var dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("Inventory_QueryExperienceHallInventory");

            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = queryFilter.PagingInfo.SortBy,
                StartRowIndex = queryFilter.PagingInfo.PageIndex * queryFilter.PagingInfo.PageSize,
                MaximumRows = queryFilter.PagingInfo.PageSize
            };

            using (var sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "Inventory_Exp.SysNo "))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Inventory_Exp.CompanyCode",
                    DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, queryFilter.CompanyCode);

                if (queryFilter.ProductSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Inventory_Exp.ProductSysNo",
                        DbType.Int32, "@ProductSysNo", QueryConditionOperatorType.Equal,
                        queryFilter.ProductSysNo.Value);
                }

                if (queryFilter.C3SysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c3.SysNo",
                        DbType.Int32, "@C3SysNo", QueryConditionOperatorType.Equal,
                        queryFilter.C3SysNo.Value);
                }
                if (queryFilter.C2SysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c2.SysNo",
                        DbType.Int32, "@C2SysNo", QueryConditionOperatorType.Equal,
                        queryFilter.C2SysNo.Value);
                }
                if (queryFilter.C1SysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "c1.SysNo",
                        DbType.Int32, "@C1SysNo", QueryConditionOperatorType.Equal,
                        queryFilter.C1SysNo.Value);
                }

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                var dt = dataCommand.ExecuteDataTable();
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));

                return dt;
            }
        }



        public DataTable QueryExperienceHallAllocateOrder(QueryFilter.Inventory.ExperienceHallAllocateOrderQueryFilter queryFilter, out int totalCount)
        {
            var dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("QueryExperienceHallAllocateOrder");

            PagingInfoEntity pagingInfo = new PagingInfoEntity()
            {
                SortField = queryFilter.PagingInfo.SortBy,
                StartRowIndex = queryFilter.PagingInfo.PageIndex * queryFilter.PagingInfo.PageSize,
                MaximumRows = queryFilter.PagingInfo.PageSize
            };

            using (var sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, pagingInfo, "SysNo desc"))
            {
                //商品
                if (queryFilter.ProductSysNo.HasValue)
                {
                    string sqlStr = "SELECT AllocateSysNo FROM ipp3.dbo.St_AllocateItem WHERE ProductSysNo = " + queryFilter.ProductSysNo.Value.ToString() + "";
                    sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "SysNo", QueryConditionOperatorType.In, sqlStr);
                }
                //单据编号
                if (queryFilter.SysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SysNo",
                        DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal,
                        queryFilter.SysNo.Value);
                }
                //状态
                if (queryFilter.ExperienceHallStatus.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Status",
                       DbType.Int32, "@Status", QueryConditionOperatorType.Equal,
                       queryFilter.ExperienceHallStatus.Value);
                }
                //调拨性质
                if (queryFilter.AllocateType.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "AllocateType",
                       DbType.Int32, "@AllocateType", QueryConditionOperatorType.Equal,
                       queryFilter.AllocateType.Value);
                }
                //时间 DataFrom DataTo
                if (queryFilter.DateFrom.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "InDate",
                        DbType.Date, "@InDateFrom", QueryConditionOperatorType.MoreThan,
                        queryFilter.DateFrom.Value);
                }

                if (queryFilter.DateTo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "InDate",
                        DbType.Date, "@InDateTo", QueryConditionOperatorType.LessThan,
                        queryFilter.DateTo.Value);
                }

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                EnumColumnList enumList = new EnumColumnList();
                enumList.Add("Status", typeof(ExperienceHallStatus));
                enumList.Add("AllocateType", typeof(AllocateType));
                var dt = dataCommand.ExecuteDataTable(enumList);
                totalCount = Convert.ToInt32(dataCommand.GetParameterValue("@TotalCount"));

                return dt;
            }
        }
    }
}
