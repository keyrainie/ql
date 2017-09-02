using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.Invoice.Income;
using ECCentral.BizEntity.SO;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.Invoice.SqlDataAccess
{
    [VersionExport(typeof(ISOIncomeDA))]
    public class SOIncomeDA : ISOIncomeDA
    {
        #region ISOIncomeDA Members

        /// <summary>
        /// 创建销售收款单
        /// </summary>
        /// <param name="entity">收款单基本信息</param>
        /// <returns></returns>
        public SOIncomeInfo Create(SOIncomeInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertSOIncome");
            command.SetParameterValue(entity);
            entity.SysNo = Convert.ToInt32(command.ExecuteScalar());
            if (entity.SysNo != 0)
            {
                return LoadBySysNo(entity.SysNo.Value);
            }
            return entity;
        }

        /// <summary>
        /// 根据收款单系统编号加载收款单数据
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public SOIncomeInfo LoadBySysNo(int sysNo)
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
        /// 更新主单收款单金额
        /// </summary>
        /// <param name="baseInfo">订单基本信息</param>
        public void UpdateMasterSOAmt(SOBaseInfo baseInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateSOIncomeAmt");
            command.SetParameterValue("@PayAmount", baseInfo.ReceivableAmount);
            command.SetParameterValue("@OrderAmt", baseInfo.SOTotalAmount);
            command.SetParameterValue("@PointPayAmt", baseInfo.GainPoint);
            command.SetParameterValue("@PrePayAmt", baseInfo.PrepayAmount);
            command.SetParameterValue("@OrderSysNo", baseInfo.SysNo);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 根据查询条件取得收款单列表
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <returns></returns>
        List<SOIncomeInfo> GetListByCriteria(SOIncomeQueryFilter query)
        {
            List<SOIncomeInfo> result = new List<SOIncomeInfo>();
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetSOIncomeList");

            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
               dataCommand.CommandText, dataCommand, null, "SysNo"))
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

        public List<SOIncomeInfo> GetListByCriteria(int? sysNo, int? orderSysNo, SOIncomeOrderType? orderType, List<SOIncomeStatus> soIncomeStatus)
        {
            SOIncomeQueryFilter query = new SOIncomeQueryFilter();
            query.SysNo = sysNo;
            query.OrderSysNo = orderSysNo.HasValue ? orderSysNo.ToString() : null;
            query.OrderType = orderType;
            query.InIncomeStatusList = soIncomeStatus;
            return GetListByCriteria(query);
        }

        /// <summary>
        /// 根据单据类型和单据编号取得有效的销售收款单
        /// </summary>
        /// <param name="orderSysNo">单据系统编号</param>
        /// <param name="orderType">单据类型</param>
        /// <returns></returns>
        public SOIncomeInfo GetValid(int orderSysNo, SOIncomeOrderType orderType)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetValidSOIncomeInfo");
            command.SetParameterValue("@SOSysNo", orderSysNo);
            command.SetParameterValue("@OrderType", orderType);
            return command.ExecuteEntity<SOIncomeInfo>();
        }

        /// <summary>
        /// 根据单据类型和单据编号取得已经确认的销售收款单
        /// </summary>
        /// <param name="orderSysNo">单据系统编号</param>
        /// <param name="orderType">单据类型</param>
        /// <returns></returns>
        public SOIncomeInfo GetConfirmed(int orderSysNo, SOIncomeOrderType orderType)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetConfirmedSOIncomeInfo");
            command.SetParameterValue("@SOSysNo", orderSysNo);
            command.SetParameterValue("@OrderType", orderType);
            return command.ExecuteEntity<SOIncomeInfo>();
        }

        /// <summary>
        /// 根据系统编号列表取得收款单列表
        /// </summary>
        /// <param name="soSysNoList">订单系统编号列表</param>
        /// <returns></returns>
        public List<SOIncomeInfo> GetListBySOSysNoList(List<int> soSysNoList)
        {
            List<SOIncomeInfo> result = new List<SOIncomeInfo>();
            if (soSysNoList == null || soSysNoList.Count == 0)
            {
                return result;
            }

            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetSOIncomeList");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, null, "SysNo"))
            {
                sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("OrderType=1"));
                sqlBuilder.ConditionConstructor.AddCustomCondition(QueryConditionRelationType.AND, string.Format("OrderSysNo IN({0})", soSysNoList.ToListString()));
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                result = dataCommand.ExecuteEntityList<SOIncomeInfo>();
            }
            return result;
        }

        /// <summary>
        /// 设置凭证号
        /// </summary>
        /// <param name="sysNo">收款单系统编号</param>
        /// <param name="referenceID">凭证号</param>
        public void SetReferenceID(int sysNo, string referenceID)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SetSOIncomeReferenceID");
            command.SetParameterValue("@SysNo", sysNo);
            command.SetParameterValue("@ReferenceID", referenceID);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 设置收款单实收金额
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="incomeAmt"></param>
        public void SetIncomeAmount(int sysNo, decimal incomeAmt)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SetIncomeAmount");
            command.SetParameterValue("@SysNo", sysNo);
            command.SetParameterValue("@IncomeAmt", incomeAmt);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新收款单状态
        /// </summary>
        /// <param name="entity"></param>
        public void UpdateStatus(SOIncomeInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateSOIncomeStatus");
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@ExternalKey", entity.ExternalKey);
            if (entity.ConfirmUserSysNo.HasValue)
            {
                command.SetParameterValue("@ConfirmUserSysNo", entity.ConfirmUserSysNo);
            }
            else
            {
                command.SetParameterValueAsCurrentUserSysNo("@ConfirmUserSysNo");
            }
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新收款单状态为已处理，用于收款单自动确认时更新母单的收款状态为已处理
        /// </summary>
        /// <param name="entityList">收款单系统编号列表</param>
        /// <returns>此次更新的收款单列表</returns>
        public void UpdateToProcessedStatus(List<SOIncomeInfo> entityList)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("UpdateToProcessedStatus");
            dataCommand.CommandText.Replace("#SysNo#", entityList.ToListString("SysNo"));

            dataCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// 获取退款单编号
        /// </summary>
        /// <returns></returns>
        public List<int> GetSysNoListByRefund()
        {
            List<int> sysNoList = new List<int>();
            DataCommand command = DataCommandManager.GetDataCommand("SOIncome_GetSysNoListByRefund");
            DataTable dt = command.ExecuteDataTable();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    sysNoList.Add(int.Parse(dr[0].ToString()));
                }
            }
            return sysNoList;
        }

        #endregion ISOIncomeDA Members

        #region [For SO Domain]

        public void UpdateStatusSplitForSO(SOIncomeInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateSOIncomeStatusSplitForSO");
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@Status", entity.Status);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 取消拆分，将母单和子单的收款信息都作废
        /// </summary>
        /// <param name="master">母单</param>
        /// <param name="subList">子单列表</param>
        public void AbandonSplitForSO(SOBaseInfo master, List<SOBaseInfo> subList)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("AbandonSplitSOIncomeForSO");
            cmd.CommandText = cmd.CommandText.Replace("@SoSysNoList@", subList.ToListString("SysNo"));
            cmd.AddInputParameter("@MasterSoSysNo", DbType.AnsiStringFixedLength, master.SysNo);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 为PendingList生成销售收款单时需要调用，用来更新收款单单据金额
        /// </summary>
        /// <param name="soIncomeSysNo">销售-收款单系统编号</param>
        /// <param name="orderAmt">单据金额</param>
        public void UpdateOrderAmtForSO(int sysNo, decimal orderAmt)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateSOIncomeOrderAmtForSO");
            command.SetParameterValue("@SysNo", sysNo);
            command.SetParameterValue("@OrderAmt", orderAmt);
            command.ExecuteNonQuery();
        }

        #endregion [For SO Domain]



        public int GetROSOSysNO(int orderSysNo, int orderType)
        {
            int sosysNo = -1;
            DataCommand cmd = DataCommandManager.GetDataCommand("GetROSOSysNO");
            cmd.SetParameterValue("@OrderSysNo", orderSysNo);
            cmd.SetParameterValue("@OrderType", orderType);
            object result = cmd.ExecuteScalar();
            if (result != null)
            {
                int.TryParse(result.ToString(), out sosysNo);
            }
            return sosysNo;
        }

        public int GetSOIncomeBankInfoRefundPayType(int sosysNo)
        {
            int refundPayType = -1;
            DataCommand cmd = DataCommandManager.GetDataCommand("GetSOIncomeBankInfoRefundPayType");
            cmd.SetParameterValue("@SOSysNo", sosysNo);

            object result = cmd.ExecuteScalar();
            if (result != null)
            {
                int.TryParse(result.ToString(), out refundPayType);
            }
            return refundPayType;
        }

        public int GetRMAReundRefundPayType(int sosysNo)
        {
            int refundPayType = -1;
            DataCommand cmd = DataCommandManager.GetDataCommand("GetRMAReundRefundPayType");
            cmd.SetParameterValue("@SOSysNo", sosysNo);
            object result = cmd.ExecuteScalar();
            if (result != null)
            {
                int.TryParse(result.ToString(), out refundPayType);
            }
            return refundPayType;
        }

        public int GetSOIncomeBankInfoStatus(int sosysNo)
        {
            int status = 1;
            DataCommand cmd = DataCommandManager.GetDataCommand("GetSOIncomeBankStatus");
            cmd.SetParameterValue("@SOSysNo", sosysNo);

            object result = cmd.ExecuteScalar();
            if (result != null)
            {
                int.TryParse(result.ToString(), out status);

            }
            return status;
        }


        public decimal GetSOIncomeAmt(int sosysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetSOIncomeAmt");
            command.SetParameterValue("@SOSysNo", sosysNo);
            return command.ExecuteScalar<decimal>();
        }

        /// <summary>
        /// 通过订单号，获取订单的Pos信息（预付款、银行卡付款、现金付款）
        /// </summary>
        /// <param name="orderSysNo"></param>
        /// <returns></returns>
        public PosInfo GetPosInfoByOrderSysNo(int orderSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetPosInfoByOrderSysNo");
            command.SetParameterValue("@OrderSysNo", orderSysNo);
            var result = command.ExecuteEntity<PosInfo>();
            return result;
        }


        public SOFreightStatDetail LoadSOFreightConfirmBySysNo(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("LoadSOFreightConfirmBySysNo");
            command.SetParameterValue("@SysNo", sysNo);
            var result = command.ExecuteEntity<SOFreightStatDetail>();
            return result;
        }

        public void SOFreightConfirm(SOFreightStatDetail detail)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SOFreightConfirm");
            command.SetParameterValue("@SysNo", detail.SysNo);
            command.ExecuteNonQuery();
        }

        public void RealFreightConfirm(SOFreightStatDetail detail)
        {
            DataCommand command = DataCommandManager.GetDataCommand("RealFreightConfirm");
            command.SetParameterValue("@SysNo", detail.SysNo);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 获取所有需要对账的关务对接相关信息
        /// </summary>
        /// <returns></returns>
        public List<VendorCustomsInfo> QueryVendorCustomsInfo()
        {
            DataCommand command = DataCommandManager.GetDataCommand("SOIncome_QueryVendorCustomsInfo");
            return command.ExecuteEntityList<VendorCustomsInfo>();
        }
    }
}