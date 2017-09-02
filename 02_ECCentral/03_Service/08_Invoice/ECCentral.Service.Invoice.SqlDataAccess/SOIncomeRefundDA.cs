using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Invoice.SqlDataAccess
{
    [VersionExport(typeof(ISOIncomeRefundDA))]
    public class SOIncomeRefundDA : ISOIncomeRefundDA
    {
        #region ISaleIncomeRefundDA Members

        /// <summary>
        /// 创建退款单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual SOIncomeRefundInfo Create(SOIncomeRefundInfo input)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateAuditRefund");
            cmd.SetParameterValue<SOIncomeRefundInfo>(input);

            int sysNo = Convert.ToInt32(cmd.ExecuteScalar());
            return LoadBySysNo(sysNo);
        }

        /// <summary>
        /// 更新退款单
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Update(SOIncomeRefundInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpateSOIncomeRefund");
            cmd.SetParameterValue(entity);

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 根据查询条件取得退款单列表
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <returns></returns>
        public virtual List<SOIncomeRefundInfo> GetListByCriteria(SOIncomeRefundInfo query)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetSOIncomeRefundList");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, null, "SysNo DESC"))
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

        /// <summary>
        /// 根据退款单编号取得退款信息
        /// </summary>
        /// <param name="sysNo">退款单系统编号</param>
        /// <returns></returns>
        public virtual SOIncomeRefundInfo LoadBySysNo(int sysNo)
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
        /// 创建Alipay退款信息
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <param name="companyCode"></param>
        public virtual void CreateAliPayRefund(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateAliPayRefund");
            command.SetParameterValue("@SoSysNo", soSysNo);
            command.SetParameterValueAsCurrentUserAcct("@InUser");

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新销售退款单状态
        /// </summary>
        /// <param name="sysNo">销售退款单系统编号</param>
        /// <param name="userSysNo">引起状态改变的操作人系统编号，如果不是审核动作可以填null</param>
        /// <param name="status">销售退款单的目标状态</param>
        /// <param name="auditTime">审核时间，如果不是审核动作可以填null</param>
        public bool UpdateStatus(int sysNo, int? userSysNo, RefundStatus status, DateTime? auditTime)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateSOIncomeRefundStatus");
            command.SetParameterValue("@SysNo", sysNo);
            command.SetParameterValue("@AuditUserSysNo", userSysNo);
            command.SetParameterValue("@AuditTime", auditTime);
            command.SetParameterValue("@Status", status);

            return command.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// 更新销售退款单状态和财务备注
        /// </summary>
        /// <param name="sysNo">销售退款单系统编号</param>
        /// <param name="userSysNo">引起状态改变的操作人系统编号，如果不是审核动作可以填null</param>
        /// <param name="status">销售退款单的目标状态</param>
        /// <param name="auditTime">审核时间，如果不是审核动作可以填null</param>
        /// <param name="finNote">财务备注</param>
        public bool UpdateStatusAndFinNote(int sysNo, int? userSysNo, RefundStatus status, DateTime? auditTime, string finNote)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateSOIncomeRefundStatusAndFinNote");
            command.SetParameterValue("@SysNo", sysNo);
            command.SetParameterValue("@AuditUserSysNo", userSysNo);
            command.SetParameterValue("@AuditTime", auditTime);
            command.SetParameterValue("@Status", status);
            command.SetParameterValue("@FinNote", finNote);

            return command.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// 根据退款单号获取退款单信息
        /// </summary>
        /// <param name="SysNo"></param>
        /// <param name="CompanyCode"></param>
        /// <returns></returns>
        public SOIncomeRefundInfo GetSOIncomeRefundByID(int SysNo, string CompanyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetSOIncomeRefundByID");
            command.SetParameterValue("@SysNo", SysNo);
            command.SetParameterValue("@CompanyCode", CompanyCode);
            return command.ExecuteEntity<SOIncomeRefundInfo>();
        }

        /// <summary>
        /// 获取对款记录数
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public int GetRefundOrder(int soSysNo, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetRefundOrder");
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@CompanyCode", companyCode);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        /// <summary>
        /// 获取最后一次退款的payment
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public decimal GetPayAmountBeHis(int soSysNo, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetPayAmountBeHis");
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@CompanyCode", companyCode);
            return Convert.ToDecimal(command.ExecuteScalar());
        }

        #endregion ISaleIncomeRefundDA Members
    }
}