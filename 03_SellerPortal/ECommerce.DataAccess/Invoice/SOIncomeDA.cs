using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECommerce.Entity.Invoice;
using ECommerce.Enums;
using ECommerce.Utility.DataAccess;

namespace ECommerce.DataAccess.Invoice
{
    public class SOIncomeDA
    {
        /// <summary>
        /// 根据单据类型和单据编号取得有效的销售收款单
        /// </summary>
        /// <param name="soSysNo">单据系统编号</param>
        /// <param name="type">单据类型</param>
        /// <returns>符合条件的有效的收款单,如果没有找到符合条件的收款单则返回NULL</returns>
        public static SOIncomeInfo GetValidSOIncomeInfo(int soSysNo, SOIncomeOrderType type)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetValidSOIncomeInfo");
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@OrderType", type);
            return command.ExecuteEntity<SOIncomeInfo>();
        }

        public static SOIncomeInfo UpdateSOIncomeStatus(SOIncomeInfo soIncomeInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateSOIncomeStatus");
            command.SetParameterValue("@SysNo", soIncomeInfo.SysNo);
            command.SetParameterValue("@Status", soIncomeInfo.Status);
            command.SetParameterValue("@ConfirmTime", soIncomeInfo.ConfirmTime);
            command.SetParameterValue("@ConfirmUserSysNo", soIncomeInfo.ConfirmUserSysNo);
            command.ExecuteNonQuery();

            return soIncomeInfo;
        }

        /// <summary>
        /// 创建销售收款单
        /// </summary>
        /// <param name="entity">收款单信息</param>
        /// <returns>创建好的销售收款单</returns>
        public static int? CreateSOIncome(SOIncomeInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertSOIncome");
            command.SetParameterValue(entity);
            entity.SysNo = Convert.ToInt32(command.ExecuteScalar());
            return entity.SysNo;
        }

        /// <summary>
        /// 根据收款单系统编号加载收款单数据
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public static SOIncomeInfo LoadBySysNo(int sysNo)
        {
            var query = new SOIncomeQueryFilter()
            {
                SysNo = sysNo
            };

            var result = GetListByCriteria(query);
            if (result.Count > 0)
            {
                return result[0];
            }
            return null;
        }

        /// <summary>
        /// 根据查询条件取得收款单列表
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <returns></returns>
        public static List<SOIncomeInfo> GetListByCriteria(SOIncomeQueryFilter query)
        {
            List<SOIncomeInfo> result = new List<SOIncomeInfo>();
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetSOIncomeList");

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
               dataCommand.CommandText, dataCommand, (PagingInfoEntity)null, "SysNo"))
            {
                if (query.SysNo != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SysNo",
                   DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, query.SysNo.Value);
                }

                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CompanyCode",
                   DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, query.CompanyCode);

                if (!string.IsNullOrEmpty(query.OrderSysNo))
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "OrderSysNo",
                    DbType.Int32, "@OrderSysNo", QueryConditionOperatorType.Equal, int.Parse(query.OrderSysNo));
                }

                if (query.OrderType != null)
                {
                    sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "OrderType",
                    DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, (int)query.OrderType.Value);
                }

                if (query.InIncomeStatusList != null && query.InIncomeStatusList.Count > 0)
                {
                    sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "Status"
                            , QueryConditionOperatorType.In, string.Join(",", query.InIncomeStatusList.Select(p => ((int)p).ToString())));
                }

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                result = dataCommand.ExecuteEntityList<SOIncomeInfo>();
            }

            return result;
        }
    }
}
