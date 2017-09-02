using ECommerce.Entity.Common;
using ECommerce.Entity.ControlPannel;
using ECommerce.Enums;
using ECommerce.Utility.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ECommerce.DataAccess.ControlPannel
{
    public class StockDA
    {
        /// <summary>
        /// 查询商家的仓库列表
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        public static QueryResult<StockInfoQueryRestult> QueryStock(StockQueryFilter queryFilter)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryStock");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(command.CommandText, command, queryFilter, string.IsNullOrEmpty(queryFilter.SortFields) ? "s.SysNo ASC" : queryFilter.SortFields))
            {
                if (!string.IsNullOrEmpty(queryFilter.StockID))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "s.StockID", DbType.String, "@StockID", QueryConditionOperatorType.Like, queryFilter.StockID);
                }
                if (queryFilter.SysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "s.SysNo", DbType.String, "@SysNo", QueryConditionOperatorType.Equal, queryFilter.SysNo.Value);
                }
                if (queryFilter.Status.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "s.Status", DbType.String, "@Status", QueryConditionOperatorType.Equal, queryFilter.Status.Value);
                }
                if (queryFilter.StockType.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "s.StockType", DbType.String, "@StockType", QueryConditionOperatorType.Equal, queryFilter.StockType.Value);
                }
                if (queryFilter.ContainKJT)
                {
                    sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND,
                        string.Format("(s.MerchantSysNo = 1 OR s.MerchantSysNo = {0})", queryFilter.MerchantSysNo));
                }
                else
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "s.MerchantSysNo", DbType.Int32, "@MerchantSysNo", QueryConditionOperatorType.Equal, queryFilter.MerchantSysNo);
                }
                if (!string.IsNullOrEmpty(queryFilter.StockName))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "s.StockName", DbType.String, "@StockName", QueryConditionOperatorType.Like, queryFilter.StockName);
                }
                command.CommandText = sqlBuilder.BuildQuerySql();
                List<StockInfoQueryRestult> resultList = command.ExecuteEntityList<StockInfoQueryRestult>();
                int totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));

                return new QueryResult<StockInfoQueryRestult>() { PageInfo = new PageInfo() { PageIndex = queryFilter.PageIndex, PageSize = queryFilter.PageSize, TotalCount = totalCount, SortBy = queryFilter.SortFields }, ResultList = resultList };
            }
        }
        /// <summary>
        /// 删除某一个商家的一个仓库
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="merchantSysNo"></param>
        /// <returns></returns>
        public static bool DelStock(int sysNo, int merchantSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DelStock");
            cmd.SetParameterValue("@SysNo", sysNo);
            cmd.SetParameterValue("@MerchantSysNo", merchantSysNo);
            return cmd.ExecuteNonQuery() > 0 ? true : false;
        }

        /// <summary>
        /// 生成一个新的仓库编号
        /// </summary>
        /// <returns></returns>
        private static int NewStockSysNo()
        {
            DataCommand dc = DataCommandManager.GetDataCommand("NewStockSysNo");
            return dc.ExecuteScalar<int>();

        }
        /// <summary>
        /// 创建仓库信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static StockInfo Create(StockInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateStock");
            entity.SysNo = NewStockSysNo();
            command.SetParameterValue(entity);
            command.ExecuteNonQuery();
            return entity;
        }
        /// <summary>
        /// 编辑仓库信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool Edit(StockInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("EditStock");
            command.SetParameterValue(entity);
            return command.ExecuteNonQuery() > 0;
        }
        /// <summary>
        /// 加载仓库信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public static StockInfo LoadStock(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("LoadStock");
            command.SetParameterValue("@SysNo", sysNo);
            return command.ExecuteEntity<StockInfo>();
        }

        public static DataTable QueryStockShipType(StockShipTypeQueryFilter filter, out int count)
        {
            CustomDataCommand command = DataCommandManager.
             CreateCustomDataCommandFromConfig("Query_Stock_QueryStockShipType");
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(
            command.CommandText, command, filter, !string.IsNullOrWhiteSpace(filter.SortFields) ? filter.SortFields : "ss.SysNo DESC"))
            {
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "ss.SellerSysNo", DbType.String,
                    "@SellerSysNo", QueryConditionOperatorType.Equal,
                    filter.SellerSysNo);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "ss.StockSysNo", DbType.AnsiStringFixedLength,
                    "@StockSysNo", QueryConditionOperatorType.Equal,
                    filter.StockSysNo);

                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND,
                    "ss.ShipTypeSysNo", DbType.AnsiStringFixedLength,
                    "@ShipTypeSysNo", QueryConditionOperatorType.Equal,
                    filter.ShipTypeSysNo);

                command.CommandText = builder.BuildQuerySql();
                count = Convert.ToInt32(command.GetParameterValue("@TotalCount"));
                return command.ExecuteDataTable();
            }
        }


        public static StockShipTypeInfo GetStockShipTypeInfo(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetStockShipTypeInfo");
            command.SetParameterValue("@SysNo", sysNo);
            return command.ExecuteEntity<StockShipTypeInfo>();
        }
        /// <summary>
        /// 新增仓库-配送方式
        /// </summary>
        /// <param name="info"></param>
        public static void CreateStockShipType(StockShipTypeInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateStockShipType");
            command.SetParameterValue(info);
            command.ExecuteNonQuery();
        }
        /// <summary>
        /// 维护仓库-配送方式
        /// </summary>
        /// <param name="info"></param>
        public static void UpdateStockShipType(StockShipTypeInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateStockShipType");
            command.SetParameterValue(info);
            command.ExecuteNonQuery();
        }

    }
}