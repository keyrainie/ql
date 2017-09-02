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
    [VersionExport(typeof(IPayItemDA))]
    public class PayItemDA : IPayItemDA
    {
        #region IPayItemDA Members

        public PayItemInfo Create(PayItemInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertPayItem");
            command.SetParameterValue(entity);

            entity = Load(Convert.ToInt32(command.ExecuteScalar()));
            //建立应付款和付款单的关联关系
            CreatePayEx(entity);

            return entity;
        }

        public void CreatePayEx(PayItemInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertPayEx");
            command.SetParameterValue("@PaySysNo", entity.PaySysNo);
            command.SetParameterValue("@PayItemSysNo", entity.SysNo);
            command.SetParameterValue("@PayedAmt", entity.PayAmt);
            command.SetParameterValue("@Status", 'A');
            command.SetParameterValue("@CompanyCode", entity.CompanyCode);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新付款单
        /// </summary>
        /// <param name="entity"></param>
        public PayItemInfo Update(PayItemInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdatePayItem");
            command.SetParameterValue("@PayAmt", entity.PayAmt);
            command.SetParameterValue("@EstimatePayTime", entity.EstimatePayTime);
            command.SetParameterValue("@ReferenceID", entity.ReferenceID);
            command.SetParameterValue("@Note", entity.Note);
            command.SetParameterValue("@SysNo", entity.SysNo);

            command.ExecuteNonQuery();

            return Load(entity.SysNo.Value);
        }

        /// <summary>
        /// 设置付款单凭证号
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public PayItemInfo SetReferenceID(int sysNo, string referenceID)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdatePayItemReferenceID");

            command.SetParameterValue("@SysNo", sysNo);
            command.SetParameterValue("@ReferenceID", referenceID);

            command.ExecuteNonQuery();
            return Load(sysNo);
        }

        /// <summary>
        /// 根据付款单编号加载付款单
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public PayItemInfo Load(int sysNo)
        {
            var result = GetListByCriteria(new PayItemInfo
            {
                SysNo = sysNo
            });
            if (result.Count > 0)
            {
                return result[0];
            }
            return null;
        }

        /// <summary>
        /// 根据查询条件加载付款单列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<PayItemInfo> GetListByCriteria(PayItemInfo query)
        {
            int? orderType = null;
            if (query.OrderType.HasValue)
            {
                orderType = (int)query.OrderType;
            }
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetPayItemList");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, null, "SysNo desc"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PayStyle",
                  DbType.Int32, "@PayStyle", QueryConditionOperatorType.Equal, query.PayStyle);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "PaySysNo",
                 DbType.Int32, "@PaySysNo", QueryConditionOperatorType.Equal, query.PaySysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Status",
                 DbType.Int32, "@Status", QueryConditionOperatorType.Equal, query.Status);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SysNo",
                 DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, query.SysNo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "CompanyCode",
                 DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, query.CompanyCode);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "OrderType",
               DbType.Int32, "@OrderType", QueryConditionOperatorType.Equal, orderType);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "OrderSysNo",
               DbType.Int32, "@OrderSysNo", QueryConditionOperatorType.Equal, query.OrderSysNo);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                var result = dataCommand.ExecuteEntityList<PayItemInfo>();
                return result;
            }
        }

        /// <summary>
        /// 对于一个PO单对应的付款状态为FullPay的应付款，如果这个PO单对应的应付款有存在付款状态为Origin的付款单，
        /// 系统自动将这些付款单的付款状态从Origin置为Abandon
        /// </summary>
        public void SetAbandonOfFullPay(PayItemInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SetPayItemAbandonOfFullPay");

            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@OriginStatus", PayItemStatus.Origin);
            command.SetParameterValue("@AbandonStatus", PayItemStatus.Abandon);
            command.SetParameterValue("@OrderSysNo", entity.OrderSysNo);
            //command.SetParameterValue("@BatchNumber", entity.BatchNo);

            command.ExecuteNonQuery();
        }

        public PayItemInfo UpdateStatus(PayItemInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdatePayItemStatus");

            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@BankGLAccount", string.IsNullOrEmpty(entity.BankGLAccount) ? null : entity.BankGLAccount);
            if (string.IsNullOrEmpty(entity.Note))
            {
                entity.Note = " ";
            }
            command.SetParameterValue("@Note", entity.Note);
            //command.SetParameterValueAsCurrentUserSysNo("@PayUserSysNo");
            command.SetParameterValue("@PayUserSysNo", entity.PayUserSysNo);
            command.ExecuteNonQuery();

            return Load(entity.SysNo.Value);
        }

        public int GetSystemUserSysNo()
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetSystemUserSysNo");
            return Convert.ToInt32(command.ExecuteScalar());
        }

        /// <summary>
        /// 是否是最后一个未作废付款单
        /// </summary>
        /// <param name="payItemInfo"></param>
        /// <returns></returns>
        public bool IsLastUnAbandon(PayItemInfo payItemInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("IsLastUnAbandonPayItem");
            command.SetParameterValue("@PaySysNo", payItemInfo.PaySysNo);
            command.SetParameterValue("@SysNo", payItemInfo.SysNo);
            return command.ExecuteScalar<int>() == 0;
        }

        /// <summary>
        /// 更新付款单状态和编辑人
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public PayItemInfo UpdateStatusAndEditUser(PayItemInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdatePayItemStatusAndEditUser");

            //command.SetParameterValue("@EditUser", entity.EditUserSysNo);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValueAsCurrentUserSysNo("@EditUser");
            command.ExecuteNonQuery();

            return Load(entity.SysNo.Value);
        }

        #endregion IPayItemDA Members

        #region For PO Domain

        /// <summary>
        /// 根据付款单状态查询PO单、代销结算单、代收结算单的付款单列表
        /// </summary>
        /// <param name="status">付款单状态</param>
        /// <returns></returns>
        public List<PayItemInfo> GetListByStatus(PayItemStatus status)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetPayItemListByStatus");
            command.SetParameterValue("@PayItemStatus", status);

            return command.ExecuteEntityList<PayItemInfo>();
        }

        public void UpdateAvailableAmt(PayableOrderType orderType, int orderSysNo, decimal availableAmt)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdatePayItemAvailableAmt");
            command.SetParameterValue("@OrderType", orderType);
            command.SetParameterValue("@OrderSysNo", orderSysNo);
            command.SetParameterValue("@AvailableAmt", availableAmt);
            command.ExecuteNonQuery();
        }

        public bool IsAdvanced(PayableOrderType orderType, int orderSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("PayItemIsAdvanced");
            command.SetParameterValue("@OrderType", orderType);
            command.SetParameterValue("@OrderSysNo", orderSysNo);
            object obj = command.ExecuteScalar();
            if (obj == null || obj == DBNull.Value)
            {
                return false;
            }
            return Convert.ToInt32(obj) > 0;
        }

        public List<PayItemInfo> LockOrUnLockBySysNoList(List<int> payItemSysNoList, PayItemStatus status)
        {
            string selectInStr = " (-1) ";

            if (payItemSysNoList != null && payItemSysNoList.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(" (");
                foreach (int sysNo in payItemSysNoList)
                {
                    sb.AppendFormat("{0},", sysNo);
                }
                selectInStr = sb.ToString();
                if (selectInStr.EndsWith(","))
                {
                    selectInStr = selectInStr.Remove(selectInStr.Length - 1);
                }
                selectInStr += " )";
            }

            DataCommand command = DataCommandManager.GetDataCommand("LockOrUnLockPayItemBySysNoList");
            command.SetParameterValue("@Status", status);
            command.SetParameterValue("@EditUser", ServiceContext.Current.UserSysNo);
            command.ReplaceParameterValue("#SysNoList", selectInStr);

            return command.ExecuteEntityList<PayItemInfo>();
        }

        public void InsertPayItemInfo(PayItemInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertFinancePayAndItem");

            cmd.SetParameterValue("@OrderSysNo", info.OrderSysNo);
            cmd.SetParameterValue("@PrePayItemSysNo", info.SysNo);
            cmd.SetParameterValue("@CreateUserSysNo", info.CreateUserSysNo);
            cmd.SetParameterValue("@PayAmt", -1 * Math.Abs(info.AvailableAmt.HasValue ? info.AvailableAmt.Value : 0M));
            cmd.SetParameterValue("@CompanyCode", info.CompanyCode);
            cmd.SetParameterValue("@LanguageCode", "zh-CN");
            cmd.SetParameterValue("@StoreCompanyCode", info.CompanyCode);

            cmd.ExecuteNonQuery();
        }

        public PayItemInfo GetPayItemInfoByPOSysNo(int poSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetFinacePayItemByPOSysNo");

            cmd.SetParameterValue("@OrderSysNo", poSysNo);

            return cmd.ExecuteEntity<PayItemInfo>();
        }

        #endregion For PO Domain
    }
}