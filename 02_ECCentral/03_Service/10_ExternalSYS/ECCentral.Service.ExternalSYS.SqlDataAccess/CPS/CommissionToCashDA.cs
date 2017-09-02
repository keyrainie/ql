using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.ExternalSYS.IDataAccess;
using ECCentral.Service.Utility;
using System.Data;
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.ExternalSYS.SqlDataAccess
{
    [VersionExport(typeof(ICommissionToCashDA))]
   public class CommissionToCashDA : ICommissionToCashDA
    {
        /// <summary>
        /// 根据query获取兑现申请信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetCommissionToCashByQuery(CommissionToCashQueryFilter query, out int TotalCount)
        {
            var cmd = DataCommandManager.CreateCustomDataCommandFromConfig("GetCommissionToCashByQuery");
            var pagingInfo = new PagingInfoEntity
            {
                SortField = query.PageInfo.SortBy,
                StartRowIndex = query.PageInfo.PageIndex * query.PageInfo.PageSize,
                MaximumRows = query.PageInfo.PageSize
            };
            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd.CommandText, cmd, pagingInfo, "cashRecord.SysNo desc"))
            {
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "userInfo.CustomerID",
                  DbType.String, "@CustomerID", QueryConditionOperatorType.LeftLike, query.CustomerID);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "cashRecord.InDate",
                    DbType.DateTime, "@RequestDateFrom", QueryConditionOperatorType.MoreThanOrEqual, query.ApplicationDateFrom);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "cashRecord.InDate",
                    DbType.DateTime, "@RequestDateTo", QueryConditionOperatorType.LessThanOrEqual, query.ApplicationDateTo);
                sqlBuilder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "cashRecord.Status",
                    DbType.AnsiStringFixedLength, "@Status", QueryConditionOperatorType.Equal, query.Status);

                cmd.CommandText = sqlBuilder.BuildQuerySql();
            }
            DataTable dt = new DataTable();
            dt = cmd.ExecuteDataTable(8, typeof(ToCashStatus));
            TotalCount = (int)cmd.GetParameterValue("@TotalCount");
            return dt;
        }

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="info"></param>
        public void AuditCommisonToCash(CommissionToCashInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("AuditCommisonToCash");
            cmd.SetParameterValue("@SysNo", info.SysNo);
            cmd.SetParameterValue("@PayAmt", info.NewPayAmt);
            cmd.SetParameterValue("@Bonus", info.Bonus);
            cmd.SetParameterValue("@ConfirmUser", info.User.UserName);
            cmd.SetParameterValue("@Memo", info.Memo);
            cmd.ExecuteNonQuery();

        }
        /// <summary>
        /// 更新实际支付金额
        /// </summary>
        /// <param name="info"></param>
        public void UpdateCommissionToCashPayAmt(CommissionToCashInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateCommissionToCashPayAmt");
            cmd.SetParameterValue("@SysNo", info.SysNo);
            cmd.SetParameterValue("@PayAmt", info.NewPayAmt);
            cmd.SetParameterValue("@EditUser", info.User.UserName);
            cmd.ExecuteNonQuery();
        }
        /// <summary>
        /// 确认支付
        /// </summary>
        /// <param name="info"></param>
        public void ConfirmCommisonToCash(CommissionToCashInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ConfirmCommisonToCash");
            cmd.SetParameterValue("@SysNo",info.SysNo);
            cmd.SetParameterValue("@UserSysNo", ServiceContext.Current.UserSysNo);
            cmd.SetParameterValue("@ConfirmToCashAmt", info.ConfirmToCashAmt);
            cmd.SetParameterValue("@AfterTaxAmt", info.AfterTaxAmt);
            cmd.SetParameterValue("@PayUser", info.User.UserName);
            cmd.SetParameterValue("@PayAmt", info.NewPayAmt);
            cmd.SetParameterValue("@Memo", info.Memo);
            cmd.ExecuteNonQuery();
        }

        public void Insert(CommissionToCashInfo commissionToCashInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertCommissionToCashInfo");
            cmd.SetParameterValue("@UserSysNo",commissionToCashInfo.CPSUserInfo.SysNo);
            cmd.SetParameterValue("@Status", commissionToCashInfo.Status);
            cmd.SetParameterValue("@ToCashAmt", commissionToCashInfo.ToCashAmt);
            cmd.SetParameterValue("@AfterTaxAmt", commissionToCashInfo.AfterTaxAmt);
            cmd.SetParameterValue("@BankCode", commissionToCashInfo.BankCode);
            cmd.SetParameterValue("@BankName", commissionToCashInfo.BankName);
            cmd.SetParameterValue("@BranchBank", commissionToCashInfo.BranchBank);
            cmd.SetParameterValue("@BankCardNumber", commissionToCashInfo.BankCardNumber);
            cmd.SetParameterValue("@ReceivableName", commissionToCashInfo.ReceivableName);
            cmd.SetParameterValue("@IsHasInvoice", commissionToCashInfo.CanProvideInvoice);
            cmd.SetParameterValue("@InUser", commissionToCashInfo.CPSUserInfo.SysNo);
            cmd.ExecuteNonQuery();
        }
    }
}