using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECommerce.Entity.Invoice;
using ECommerce.Entity.SO;
using ECommerce.Enums;
using ECommerce.Utility;
using ECommerce.Utility.DataAccess;

namespace ECommerce.DataAccess.Invoice
{
    public class SOIncomeRefundDA
    {
        /// <summary>
        /// 获取退款原因列表
        /// </summary>
        /// <returns>退款原因列表</returns>
        public static List<Entity.Common.CodeNamePair> GetRefundReasons()
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetRefundReasons");
            var list = command.ExecuteEntityList<Entity.Common.CodeNamePair>();
            list.ForEach(p => p.Name = p.Name.Trim());
            return list;
        }

        /// <summary>
        /// 根据退款单号获取退款单信息
        /// </summary>
        /// <param name="SysNo"></param>
        /// <param name="CompanyCode"></param>
        /// <returns></returns>
        public static SOIncomeRefundInfo GetSOIncomeRefundByID(int SysNo, string CompanyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetSOIncomeRefundByID");
            command.SetParameterValue("@SysNo", SysNo);
            command.SetParameterValue("@CompanyCode", CompanyCode);
            return command.ExecuteEntity<SOIncomeRefundInfo>();
        }

        /// <summary>
        /// 创建退款单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int Create(SOIncomeRefundInfo input)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateAuditRefund");
            cmd.SetParameterValue<SOIncomeRefundInfo>(input);

            int sysNo = Convert.ToInt32(cmd.ExecuteScalar());
            return sysNo;
        }


        /// <summary>
        /// 根据退款单编号取得退款信息
        /// </summary>
        /// <param name="sysNo">退款单系统编号</param>
        /// <returns></returns>
        public static SOIncomeRefundInfo LoadBySysNo(int sysNo)
        {
            var list = GetListByCriteria(new SOIncomeRefundInfo()
            {
                SysNo = sysNo
            });
            if (list != null && list.Count > 0)
            {
                return list[0];
            }
            return null;
        }

        /// <summary>
        /// 根据查询条件取得退款单列表
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <returns></returns>
        public static List<SOIncomeRefundInfo> GetListByCriteria(SOIncomeRefundInfo query)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetSOIncomeRefundList");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, (PagingInfoEntity)null, "SysNo DESC"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "OrderType", DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, query.OrderType);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Status", DbType.Int32, "@Status", QueryConditionOperatorType.Equal, query.Status);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SysNo", DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, query.SysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "OrderSysNo", DbType.Int32, "@OrderSysNo", QueryConditionOperatorType.Equal, query.OrderSysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, query.CompanyCode);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
            }

            return cmd.ExecuteEntityList<SOIncomeRefundInfo>();
        }

        public static SOIncomeRefundInfo GetValidSOIncomeRefundInfo(int soSysNo, SOIncomeOrderType type)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetValidSOIncomeRefundInfo");
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@OrderType", type);
            return command.ExecuteEntity<SOIncomeRefundInfo>();
        }

        public static SOIncomeRefundInfo UpdateSOIncomeRefundStatus(SOIncomeRefundInfo soIncomeRefundInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateSOIncomeRefundStatus");
            command.SetParameterValue("@SysNo", soIncomeRefundInfo.SysNo);
            command.SetParameterValue("@Status", soIncomeRefundInfo.Status);
            command.SetParameterValue("@AuditUserSysNo", soIncomeRefundInfo.AuditUserSysNo);
            command.SetParameterValue("@AuditTime", soIncomeRefundInfo.AuditTime);
            command.SetParameterValue("@EditTime", soIncomeRefundInfo.EditDate);
            command.SetParameterValue("@EditUserSysNo", soIncomeRefundInfo.EditUserSysNo);
            command.ExecuteNonQuery();

            return soIncomeRefundInfo;
        }
    }
}
