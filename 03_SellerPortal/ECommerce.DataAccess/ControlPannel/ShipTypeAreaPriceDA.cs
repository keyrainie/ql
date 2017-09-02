using ECommerce.Entity.Common;
using ECommerce.Entity.ControlPannel;
using ECommerce.Utility.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ECommerce.DataAccess.ControlPannel
{
    public class ShipTypeAreaPriceDA
    {
        /// <summary>
        /// 查询配送方式-地区-价格
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        public static QueryResult<ShipTypeAreaPriceInfoQueryResult> QueryShipTypeAreaPrice(ShipTypeAreaPriceQueryFilter queryFilter)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryShipTypeAreaPrice");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(command.CommandText, command, queryFilter, string.IsNullOrEmpty(queryFilter.SortFields) ? "stap.SysNo ASC" : queryFilter.SortFields))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "stap.MerchantSysNo", DbType.Int32, "@MerchantSysNo", QueryConditionOperatorType.Equal, queryFilter.MerchantSysNo);
                if (queryFilter.ShipTypeSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "stap.ShipTypeSysNo", DbType.String, "@ShipTypeSysNo", QueryConditionOperatorType.Equal, queryFilter.ShipTypeSysNo.Value);
                }
                if (queryFilter.DistrictSysNo.HasValue)
                {
                    using (GroupCondition g = new GroupCondition(sqlBuilder, QueryConditionRelationType.AND))
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.SysNO", DbType.String, "@SysNO", QueryConditionOperatorType.Equal, queryFilter.DistrictSysNo);
                    }
                }
                else if (queryFilter.CitySysNo.HasValue)
                {
                    using (GroupCondition g = new GroupCondition(sqlBuilder, QueryConditionRelationType.AND))
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.SysNo", DbType.String, "@SysNo", QueryConditionOperatorType.Equal, queryFilter.CitySysNo);
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "a.CitySysNo", DbType.String, "@CitySysNo", QueryConditionOperatorType.Equal, queryFilter.CitySysNo);

                    }
                }
                else if (queryFilter.ProvinceSysNo.HasValue)
                {
                    using (GroupCondition g = new GroupCondition(sqlBuilder, QueryConditionRelationType.AND))
                    {
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "a.SysNo", DbType.String, "@SysNo", QueryConditionOperatorType.Equal, queryFilter.ProvinceSysNo);
                        sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.OR, "a.ProvinceSysNo", DbType.String, "@ProvinceSysNo", QueryConditionOperatorType.Equal, queryFilter.ProvinceSysNo);
                    }
                }
                command.CommandText = sqlBuilder.BuildQuerySql();
                List<ShipTypeAreaPriceInfoQueryResult> resultList = command.ExecuteEntityList<ShipTypeAreaPriceInfoQueryResult>();
                int totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));

                return new QueryResult<ShipTypeAreaPriceInfoQueryResult>() { PageInfo = new PageInfo() { PageIndex = queryFilter.PageIndex, PageSize = queryFilter.PageSize, TotalCount = totalCount, SortBy = queryFilter.SortFields }, ResultList = resultList };
            }
        }
        /// <summary>
        /// 删除配送方式-地区-价格
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <param name="merchantSysNo"></param>
        /// <returns></returns>
        public static bool VoidShipTypeAreaPrice(List<int> sysNoList, int merchantSysNo)
        {
            if (sysNoList != null && sysNoList.Count > 0)
            {
                CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("ShipTypeAreaPrice_Void");
                cmd.CommandText = cmd.CommandText.Replace("@SysNo", String.Join(",", sysNoList));
                cmd.CommandText = cmd.CommandText.Replace("@MerchantSysNo", merchantSysNo.ToString());
                return cmd.ExecuteNonQuery() > 0;
            }
            return false;
        }
        /// <summary>
        /// 创建配送方式-地区-价格
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static ShipTypeAreaPriceInfo Create(ShipTypeAreaPriceInfo entity)
        {
            if (entity != null)
            {
                DataCommand cmd = DataCommandManager.GetDataCommand("ShipTypeAreaPrice_Create");
                cmd.SetParameterValue(entity);
                cmd.ExecuteNonQuery();
                entity.SysNo = (int?)cmd.GetParameterValue("@SysNO");
                return entity;
            }
            return null;
        }
        /// <summary>
        /// 更新配送方式-地区-价格
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool Edit(ShipTypeAreaPriceInfo entity)
        {
            if (entity != null)
            {
                DataCommand cmd = DataCommandManager.GetDataCommand("ShipTypeAreaPrice_Update");
                cmd.SetParameterValue(entity);
                return cmd.ExecuteNonQuery() > 0;

            }
            return false;
        }
        /// <summary>
        /// 加载配送方式-地区-价格
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="merchantSysNo"></param>
        /// <returns></returns>
        public static ShipTypeAreaPriceInfo LoadShipTypeAreaPrice(int sysNo, int merchantSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("LoadShipTypeAreaPrice");
            dc.SetParameterValue(@"SysNo", sysNo);
            dc.SetParameterValue(@"MerchantSysNo", merchantSysNo);
            return dc.ExecuteEntity<ShipTypeAreaPriceInfo>();
        }
    }
}
