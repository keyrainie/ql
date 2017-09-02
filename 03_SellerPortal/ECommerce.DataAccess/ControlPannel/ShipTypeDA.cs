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
    public class ShipTypeDA
    {
        /// <summary>
        /// 查询商家的配送方式
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        public static QueryResult<ShipTypeInfoQueryResult> QueryShipType(ShipTypeQueryFilter queryFilter)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryShipType");
            using (var sqlBuilder = new DynamicQuerySqlBuilder(command.CommandText, command, queryFilter, string.IsNullOrEmpty(queryFilter.SortFields) ? "st.SysNo ASC" : queryFilter.SortFields))
            {
                if (queryFilter.MerchantSysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "st.MerchantSysNo", DbType.Int32, "@MerchantSysNo", QueryConditionOperatorType.Equal, queryFilter.MerchantSysNo);
                }
                if (!string.IsNullOrEmpty(queryFilter.ShipTypeID))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "st.ShipTypeID", DbType.String, "@ShipTypeID", QueryConditionOperatorType.Like, queryFilter.ShipTypeID);
                }
                if (queryFilter.SysNo.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "st.SysNo", DbType.String, "@SysNo", QueryConditionOperatorType.Equal, queryFilter.SysNo.Value);
                }
                if (queryFilter.IsOnlineShow.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "st.IsOnlineShow", DbType.String, "@IsOnlineShow", QueryConditionOperatorType.Equal, queryFilter.IsOnlineShow.Value);
                }
                if (queryFilter.IsWithPackFee.HasValue)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "st.IsWithPackFee", DbType.String, "@IsWithPackFee", QueryConditionOperatorType.Equal, queryFilter.IsWithPackFee.Value);
                }
                if (!string.IsNullOrEmpty(queryFilter.ShipTypeName))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "st.ShipTypeName", DbType.String, "@ShipTypeName", QueryConditionOperatorType.Like, queryFilter.ShipTypeName);
                }
                command.CommandText = sqlBuilder.BuildQuerySql();
                List<ShipTypeInfoQueryResult> resultList = command.ExecuteEntityList<ShipTypeInfoQueryResult>();
                int totalCount = Convert.ToInt32(command.GetParameterValue("@TotalCount"));

                return new QueryResult<ShipTypeInfoQueryResult>() { PageInfo = new PageInfo() { PageIndex = queryFilter.PageIndex, PageSize = queryFilter.PageSize, TotalCount = totalCount, SortBy = queryFilter.SortFields }, ResultList = resultList };
            }
        }
        /// <summary>
        /// 删除某一个商家的一个配送方式
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public static bool DelShipType(int sysNo, int merchantSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DelShipType");
            cmd.SetParameterValue("@SysNo", sysNo);
            cmd.SetParameterValue("@MerchantSysNo", merchantSysNo);
            return cmd.ExecuteNonQuery() > 0 ? true : false;
        }

        /// <summary>
        /// 检查创建配送方式是否存在
        /// </summary>
        /// <param name="item"></param>
        public static bool GetShipTypeforCreate(ShipTypeInfo item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ShipType_GetShipTypeforCreate");
            cmd.SetParameterValue<ShipTypeInfo>(item);
            var data = cmd.ExecuteEntity<ShipTypeInfo>();
            return data != null;
        }

        /// <summary>
        /// 生成一个新的配送方式编号
        /// </summary>
        /// <returns></returns>
        private static int NewShipTypeSysNo()
        {
            DataCommand dc = DataCommandManager.GetDataCommand("NewShipTypeSysNo");
            return dc.ExecuteScalar<int>();

        }
        /// <summary>
        /// 创建配送方式信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static ShipTypeInfo Create(ShipTypeInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateShipType");
            entity.SysNo = NewShipTypeSysNo();
            entity.ShipTypeID = entity.SysNo.ToString();
            command.SetParameterValue(entity);
            command.ExecuteNonQuery();
            return entity;
        }
        /// <summary>
        /// 编辑配送方式信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool Edit(ShipTypeInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("EditShipType");
            command.SetParameterValue(entity);
            return command.ExecuteNonQuery() > 0;
        }
        /// <summary>
        /// 加载配送方式信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public static ShipTypeInfo LoadShipType(int sysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("LoadShipType");
            dc.SetParameterValue(@"SysNo", sysNo);
            return dc.ExecuteEntity<ShipTypeInfo>();
        }
    }
}
