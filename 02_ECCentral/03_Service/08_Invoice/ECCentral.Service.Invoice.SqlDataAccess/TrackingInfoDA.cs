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
    [VersionExport(typeof(ITrackingInfoDA))]
    public class TrackingInfoDA : ITrackingInfoDA
    {
        #region ITrackingInfoDA Members

        public TrackingInfo LoadTrackingInfoByOrderSysNo(int orderSysNo, SOIncomeOrderType orderType)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("QueryTrackingInfoByOrderSysNo");
            cmd.SetParameterValue("@OrderSysNo", orderSysNo);
            cmd.SetParameterValue("@OrderType", orderType);

            return cmd.ExecuteEntity<TrackingInfo>();
        }

        public void UpdateTrackingInfoStatus(TrackingInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateTrackingInfoStatus");
            cmd.SetParameterValue("@SysNo", entity.SysNo);
            cmd.SetParameterValue("@Status", entity.Status);
            cmd.SetParameterValueAsCurrentUserAcct("@EditUser");

            cmd.ExecuteNonQuery();
        }

        public bool ExistsTrackingInfo(int orderSysNo, SOIncomeOrderType orderType)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ExistsTrackingInfo");

            cmd.SetParameterValue("@OrderSysNo", orderSysNo);
            cmd.SetParameterValue("@OrderType", orderType);

            return cmd.ExecuteScalar<Int32>() > 0;
        }

        public TrackingInfo CreateTrackingInfo(TrackingInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateTrackingInfo");
            cmd.SetParameterValue(entity);

            entity.SysNo = Convert.ToInt32(cmd.ExecuteNonQuery());
            return entity;
        }

        public List<ResponsibleUserInfo> GetAllResponsibleUsers(string companyCode)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetAllResponsibleUsers");
            cmd.SetParameterValue("@CompanyCode", companyCode);

            return cmd.ExecuteEntityList<ResponsibleUserInfo>();
        }

        /// <summary>
        /// 取得收款单应收金额
        /// </summary>
        /// <param name="orderSysNo"></param>
        /// <param name="orderType"></param>
        /// <returns></returns>
        public decimal GetIncomeAmt(int orderSysNo, SOIncomeOrderType orderType)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetIncomeAmt");

            cmd.SetParameterValue("@OrderSysNo", orderSysNo);
            cmd.SetParameterValue("@OrderType", orderType);

            return Convert.ToDecimal(cmd.ExecuteScalar());
        }

        /// <summary>
        /// 根据跟踪单系统编号取得跟踪单信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public TrackingInfo LoadTrackingInfoBySysNo(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetTrackingInfoBySysNo");
            cmd.SetParameterValue("@SysNo", sysNo);

            return cmd.ExecuteEntity<TrackingInfo>();
        }

        /// <summary>
        /// 更新跟踪单信息
        /// </summary>
        /// <param name="entity"></param>
        public void UpdateTrackingInfo(TrackingInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateTrackingInfo");
            cmd.SetParameterValue(entity);

            cmd.ExecuteNonQuery();
        }

        public List<string> GetEmailAddressByResponsibleUserName(string userName)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetEmailAddressByResponsibleUserName");
            cmd.SetParameterValue("@ResponsibleUserName", userName);
            cmd.SetParameterValue("@CompanyCode", "8601"); //TODO:暂时写成8601

            return cmd.ExecuteFirstColumn<string>();
        }

        public string GetEmailAddressByUserSysNo(int userSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetEmailAddressByUserSysNo");
            cmd.SetParameterValue("@UserSysNo", userSysNo);

            return cmd.ExecuteScalar<string>();
        }

        #endregion ITrackingInfoDA Members

        #region ITrackingInfoDA Members

        public ResponsibleUserInfo CreateResponsibleUser(ResponsibleUserInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateResponsibleUser");
            cmd.SetParameterValue(entity);

            entity.SysNo = cmd.ExecuteScalar<int>();
            return LoadResponsibleUserBySysNo(entity.SysNo.Value);
        }

        public ResponsibleUserInfo LoadResponsibleUserBySysNo(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetResponsibleUserBySysNo");
            cmd.SetParameterValue("@SysNo", sysNo);

            return cmd.ExecuteEntity<ResponsibleUserInfo>();
        }

        public List<TrackingInfo> GetNotClosedTrackingInfoBelongToCertainUser(string responsibleUserName)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetNotClosedTrackingInfo");
            cmd.SetParameterValue("@ResponsibleUserName", responsibleUserName);

            return cmd.ExecuteEntityList<TrackingInfo>();
        }

        public void UpdateResponsibleUser(ResponsibleUserInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateResponsibleUser");
            cmd.SetParameterValue(entity);

            cmd.ExecuteNonQuery();
        }

        public void AbandonResponsibleUser(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("AbandonResponsibleUser");
            cmd.SetParameterValue("@SysNo", sysNo);
            cmd.SetParameterValueAsCurrentUserAcct("@EditUser");

            cmd.ExecuteNonQuery();
        }

        public void UpdateTrackingInfoResponsibleUserName(TrackingInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateTrackingInfoResponsibleUserName");

            cmd.SetParameterValue("@SysNo", entity.SysNo);
            cmd.SetParameterValue("@ResponsibleUserName", entity.ResponsibleUserName);
            cmd.SetParameterValueAsCurrentUserAcct("@EditUser");

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 修改跟踪单损失类型
        /// </summary>
        /// <param name="entity"></param>
        public void UpdateTrackingInfoLossType(TrackingInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateTrackingInfoLossType");

            cmd.SetParameterValue("@SysNo", entity.SysNo);
            cmd.SetParameterValue("@LossType", entity.LossType);
            cmd.SetParameterValueAsCurrentUserAcct("@EditUser");

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 根据条件取得责任人信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ResponsibleUserInfo GetExistedResponsibleUser(ResponsibleUserInfo entity)
        {
            CustomDataCommand dataCommand = DataCommandManager.CreateCustomDataCommandFromConfig("ExistedResponsibleUser");
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(dataCommand.CommandText, dataCommand, null, "sr.SysNo desc"))
            {
                //特殊模式责任人
                var isSpecialMode = (!entity.PayTypeSysNo.HasValue && !entity.ShipTypeSysNo.HasValue && !entity.CustomerSysNo.HasValue);
                if (!isSpecialMode)
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sr.IsPayWhenRecv", DbType.Int32, "@IncomeStyle", QueryConditionOperatorType.Equal, entity.IncomeStyle);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sr.PayTypeSysNo", DbType.Int32, "@PayTypeSysNo", QueryConditionOperatorType.Equal, entity.PayTypeSysNo);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sr.ShipTypeSysNo", DbType.Int32, "@ShipTypeSysNo", QueryConditionOperatorType.Equal, entity.ShipTypeSysNo);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sr.CustomerSysNo", DbType.Int32, "@CustomerSysNo", QueryConditionOperatorType.Equal, entity.CustomerSysNo);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sr.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, entity.CompanyCode);
                }
                else
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sr.IsPayWhenRecv", DbType.Int32, "@IsPayWhenRecv", QueryConditionOperatorType.Equal, entity.IncomeStyle);
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "sr.CompanyCode", DbType.AnsiStringFixedLength, "@CompanyCode", QueryConditionOperatorType.Equal, entity.CompanyCode);
                    builder.ConditionConstructor.AddNullCheckCondition(QueryConditionRelationType.AND, "sr.PayTypeSysNo", QueryConditionOperatorType.IsNull);
                    builder.ConditionConstructor.AddNullCheckCondition(QueryConditionRelationType.AND, "sr.ShipTypeSysNo", QueryConditionOperatorType.IsNull);
                    builder.ConditionConstructor.AddNullCheckCondition(QueryConditionRelationType.AND, "sr.CustomerSysNo", QueryConditionOperatorType.IsNull);
                }

                dataCommand.CommandText = builder.BuildQuerySql();
                var result = dataCommand.ExecuteScalar();
                if (!(result is DBNull || result == null))
                {
                    return LoadResponsibleUserBySysNo(Convert.ToInt32(result));
                }
                return null;
            }
        }

        #endregion ITrackingInfoDA Members
    }
}