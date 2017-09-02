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
    [VersionExport(typeof(IPostIncomeDA))]
    public class PostIncomeDA : IPostIncomeDA
    {
        #region IPostIncomeDA Members

        /// <summary>
        /// 创建电汇邮局收款单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public PostIncomeInfo Create(PostIncomeInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertPostIncome");
            command.SetParameterValue(entity);
            entity.SysNo = Convert.ToInt32(command.ExecuteScalar());

            return LoadBySysNo(entity.SysNo.Value);
        }

        /// <summary>
        /// 更新电汇邮局收款单
        /// </summary>
        /// <param name="entity"></param>
        public void Update(PostIncomeInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdatePostIncome");
            command.SetParameterValue(entity);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 处理电汇邮局收款单
        /// </summary>
        /// <param name="entity"></param>
        public void Handle(PostIncomeInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("HandlePostIncome");
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@HandleStatus", entity.HandleStatus);
            command.SetParameterValue("@CSNotes", entity.CSNotes);
            command.SetParameterValueAsCurrentUserSysNo("@HandleUserSysNo");
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新电汇邮局收款单状态
        /// 用于收款单的确认、取消确认、作废、取消作废操作
        /// </summary>
        /// <param name="sysNo">电汇邮局收款单系统编号</param>
        /// <param name="status">要更新到的目标状态</param>
        public void UpdateConfirmStatus(int sysNo, PostIncomeStatus status)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateConfirmStatus");
            command.SetParameterValue("@SysNo", sysNo);
            command.SetParameterValue("@ConfirmStatus", status);
            command.SetParameterValueAsCurrentUserSysNo("@ConfirmUserSysNo");
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 根据电汇邮局收款单系统编号加载收款单信息
        /// </summary>
        /// <param name="postIncomeSysNo"></param>
        /// <returns></returns>
        public PostIncomeInfo LoadBySysNo(int postIncomeSysNo)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetPostIncomeList");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, null, "SysNo desc"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "SysNo",
                       DbType.Int32, "@SysNo", QueryConditionOperatorType.Equal, postIncomeSysNo);

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                var postIncomeInfo = dataCommand.ExecuteEntity<PostIncomeInfo>();
                if (postIncomeInfo != null)
                {
                    postIncomeInfo.ConfirmInfoList = GetConfirmListByPostIncomeSysNo(postIncomeInfo.SysNo.Value);
                }
                return postIncomeInfo;
            }
        }

        /// <summary>
        /// 根据订单系统编号取得已和订单关联的PostIncome列表
        /// </summary>
        /// <param name="confirmedSOSysNo">订单系统编号字符串，多个订单编号之间用逗号（,）隔开</param>
        /// <returns>满足条件的PostIncome列表</returns>
        public List<PostIncomeInfo> GetListByConfirmedSOSysNo(string confirmedSOSysNo)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetPostIncomeList");
            using (DynamicQuerySqlBuilder sqlBuilder = new DynamicQuerySqlBuilder(
                dataCommand.CommandText, dataCommand, null, "SysNo desc"))
            {
                string confirmedSOSysNoStr = string.Join(",", confirmedSOSysNo);

                sqlBuilder.ConditionConstructor.AddSubQueryCondition(QueryConditionRelationType.AND, "SysNo",
            QueryConditionOperatorType.In, string.Format(@"SELECT a.PostIncomeSysNo FROM
         	        [OverseaInvoiceReceiptManagement].[dbo].[PostIncomeConfirm] a WITH(NOLOCK)
         	        WHERE a.ConfirmedSoSysNo IN ({0}) AND a.Status <> 'C'", confirmedSOSysNoStr));

                dataCommand.CommandText = sqlBuilder.BuildQuerySql();
                return dataCommand.ExecuteEntityList<PostIncomeInfo>();
            }
        }

        /// <summary>
        /// 通过电汇邮局收款单系统编号取得订单确认关联信息列表
        /// </summary>
        /// <param name="postIncomeSysNo">电汇邮局收款单系统编号</param>
        /// <returns></returns>
        public List<PostIncomeConfirmInfo> GetConfirmListByPostIncomeSysNo(int postIncomeSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("GetPostIncomeConfirmeListByPostIncomeSysNo");
            dataCommand.SetParameterValue("@PostIncomeSysNo", postIncomeSysNo);

            return dataCommand.ExecuteEntityList<PostIncomeConfirmInfo>();
        }

        /// <summary>
        /// 通过订单系统编号(多个订单系统编号通过逗号分隔)取得订单确认关联信息列表
        /// </summary>
        /// <param name="SOSysNos">订单系统编号(多个订单系统编号通过逗号分隔)</param>
        /// <returns></returns>
        public List<PostIncomeConfirmInfo> GetConfirmedListBySOSysNo(string SOSysNos)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("GetPostIncomeConfirmedListBySONo");
            dataCommand.ReplaceParameterValue("#ConfirmedSoSysNo", SOSysNos);
            return dataCommand.ExecuteEntityList<PostIncomeConfirmInfo>();
        }

        /// <summary>
        /// 通过通过电汇邮局收款单系统编号更新订单关联信息状态
        /// </summary>
        /// <param name="postIncomeSysNo">通过电汇邮局收款单系统编号</param>
        /// <param name="status">需要更新到的状态</param>
        public void UpdatePostIncomeConfirmStatus(int postIncomeSysNo, PostIncomeConfirmStatus status)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdatePostIncomeConfirm");
            command.SetParameterValue("@SysNo", postIncomeSysNo);
            command.SetParameterValue("@Status", status);
            command.SetParameterValueAsCurrentUserAcct("@EditUser");
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 创建订单确认关联信息
        /// </summary>
        /// <param name="entity"></param>
        public void CreatePostIncomeConfirm(PostIncomeConfirmInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertPostIncomeConfirm");
            command.SetParameterValue(entity);
            command.ExecuteNonQuery();
        }

        #endregion IPostIncomeDA Members

        #region [For SO Domain]

        public void AbandonSplitForSO(PostIncomeInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("AbandonPostIncomeSplitForSO");
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@ConfirmStatus", entity.ConfirmStatus);
            command.SetParameterValue("@ConfirmUserSysNo", entity.ConfirmUserSysNo);
            command.SetParameterValue("@HandleStatus", entity.HandleStatus);
            command.SetParameterValue("@HandleUserSysNo", entity.HandleUserSysNo);

            command.ExecuteNonQuery();
        }

        #endregion [For SO Domain]
    }
}