using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.SO;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Invoice.SqlDataAccess
{
    [VersionExport(typeof(IPostPayDA))]
    public class PostPayDA : IPostPayDA
    {
        #region IPostPayDA Members

        public PostPayInfo Create(PostPayInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertPostPay");
            command.SetParameterValue<PostPayInfo>(entity);
            entity.SysNo = Convert.ToInt32(command.ExecuteScalar());

            return entity;
        }

        public List<PostPayInfo> GetListByConfirmedSOSysNo(int confirmedSOSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetPostPayListByConfirmedSOSysNo");
            command.SetParameterValue("@ConfirmedSOSysNo", confirmedSOSysNo);

            return command.ExecuteEntityList<PostPayInfo>();
        }

        public List<PostPayInfo> GetListBySOSysNo(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetPostPayListBySOSysNo");
            command.SetParameterValue("@SOSysNo", soSysNo);

            return command.ExecuteEntityList<PostPayInfo>();
        }

        /// <summary>
        /// 根据订单系统编号作废PostPay
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        public void AbandonBySOSysNo(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("AbandonPostPayBySOSysNo");
            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@Note", "Update--Record--InValid");

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 根据订单系统编号和PostPay状态取得PostPay列表
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <param name="status">PostPay状态，可以包含多种状态</param>
        /// <returns></returns>
        public List<PostPayInfo> GetListBySOSysNoAndStatus(int soSysNo, params PostPayStatus[] status)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetListBySOSysNoAndStatus");
            command.AddInputParameter("@SOSysNo", DbType.Int32, soSysNo);

            if (status != null && status.Length > 0)
            {
                string statusStr = "";
                for (int i = 0; i < status.Length; i++)
                {
                    statusStr += (int)status[i] + ((i < status.Length - 1) ? "," : "");
                }
                command.CommandText = command.CommandText.Replace("#where#", string.Format("AND Status IN ({0})", statusStr));
            }
            return command.ExecuteEntityList<PostPayInfo>();
        }

        /// <summary>
        /// 根据订单编号取得订单有效的PostPay
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <returns></returns>
        public PostPayInfo GetValidPostPayBySOSysNo(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetValidPostPayBySOSysNo");
            command.SetParameterValue("@SOSysNo", soSysNo);
            return command.ExecuteEntity<PostPayInfo>();
        }

        /// <summary>
        /// 取得已确认的多付金额
        /// </summary>
        /// <param name="confirmedSOSysNo">订单系统编号，多个编号之间用逗号（,）隔开</param>
        /// <returns></returns>
        public decimal GetRefundAmtByConfirmedSOSysNoList(string confirmedSOSysNo)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetRefundAmtByConfirmedSOSysNo");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, null, "OrderSysNo desc"))
            {
                sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "OrderSysNo",
                       QueryConditionOperatorType.In, confirmedSOSysNo);
                dataCommand.CommandText = sqlBuilder.BuildQuerySql();

                object obj = dataCommand.ExecuteScalar();
                if (obj == DBNull.Value)
                {
                    return 0;
                }
                return Convert.ToDecimal(dataCommand.ExecuteScalar());
            }
        }

        #endregion IPostPayDA Members

        #region [For SO Domain]

        public void UpdateStatusSplitForSO(PostPayInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdatePostPayStatusSplitForSO");
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@Note", "Update--Status--SplitSO");
            command.SetParameterValue("@SOSysNo", entity.SOSysNo);
            command.ExecuteNonQuery();
        }

        public void AbandonSplitForSO(SOBaseInfo master, List<SOBaseInfo> subList)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("AbandonSplitPostPayForSO");
            cmd.CommandText = cmd.CommandText.Replace("@SoSysNoList@", subList.ToListString("SysNo"));
            cmd.AddInputParameter("@MasterSoSysNo", DbType.AnsiStringFixedLength, master.SysNo);
            cmd.AddInputParameter("@CompanyCode", DbType.AnsiStringFixedLength, master.CompanyCode);
            cmd.ExecuteNonQuery();
        }

        #endregion [For SO Domain]
    }
}