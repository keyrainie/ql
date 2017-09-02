using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;
using POASNMgmt.AutoCreateCollectionPayment.Compoents;
using POASNMgmt.AutoCreateCollectionPayment.Entities;
using System.Data;

namespace POASNMgmt.AutoCreateCollectionPayment.DataAccess
{
    internal sealed class VendorSettleDAL
    {
        public void SendEmail(string mailAddress, string mailSubject, string mailBody, int status,string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertToSendEmail");
            command.SetParameterValue("@MailAddress", mailAddress);
            command.SetParameterValue("@MailSubject", mailSubject);
            command.SetParameterValue("@MailBody", mailBody);
            command.SetParameterValue("@Staues", status);
            command.SetParameterValue("@CompanyCode", companyCode);

            command.ExecuteNonQuery();
        }

        public List<ConsginToAccLogEntity> GetConsginToAccLogList(List<int> payPeriodTypes)
        {
            if (payPeriodTypes == null || payPeriodTypes.Count == 0)
            {
                return new List<ConsginToAccLogEntity>();
            }

            var NewCosignAcctLogOnlineDate = GetNewCosignAcctLogOnlineDate();            

            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetConsginToAccLogList");

            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, null, "Consign.SysNo desc"))
            {
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "consign.[Status]", DbType.Int32, "@Status", QueryConditionOperatorType.Equal, 0);
                //builder.ConditionConstructor.AddInCondition<int>(QueryConditionRelationType.AND, "ex.SettlePeriodType", DbType.Int32, payPeriodTypes);
                builder.ConditionConstructor.AddInCondition<int>(QueryConditionRelationType.AND, "Vendor.PayPeriodType", DbType.Int32, payPeriodTypes);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "consign.CompanyCode", DbType.String, "@CompanyCode", QueryConditionOperatorType.Equal, GlobalSettings.CompanyCode);

                if (!string.IsNullOrEmpty(NewCosignAcctLogOnlineDate))
                {
                    builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "Consign.CreateTime ", DbType.DateTime, "@OnlineDate", QueryConditionOperatorType.LessThan, NewCosignAcctLogOnlineDate);
                }

                command.CommandText = builder.BuildQuerySql();

                return command.ExecuteEntityList<ConsginToAccLogEntity>();
            }
        }

        //public List<ConsginToAccLogEntity> GetConsginToAccLogList(List<int> payPeriodTypes, DateTime maxOrderEndData, int merchantSysNo)
        //{
        //    if (payPeriodTypes == null || payPeriodTypes.Count == 0)
        //    {
        //        return new List<ConsginToAccLogEntity>();
        //    }

        //    CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetGatherSettleList");
        //    command.CommandTimeout = 120;

        //    using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, null, "Consign.SysNo desc"))
        //    {
        //        builder.ConditionConstructor.AddInCondition<int>(QueryConditionRelationType.AND, "Vendor.PayPeriodType", DbType.Int32, payPeriodTypes);
        //        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "main.CompanyCode", DbType.String, "@CompanyCode", QueryConditionOperatorType.Equal, GlobalSettings.CompanyCode);
        //        builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "s.ReferenceType", DbType.String, "@ReferenceType", QueryConditionOperatorType.IsNull, DBNull.Value);

        //        if (merchantSysNo > 0)
        //        {
        //            builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "main.MerchantSysNo", DbType.Int32, "@MerchantSysNo", QueryConditionOperatorType.Equal, merchantSysNo);
        //        }

        //        command.CommandText = builder.BuildQuerySql();

        //        command.CommandText = command.CommandText.Replace("/*#RelaceWhere#*/", " Where OutOrRefundDateTime < '" + maxOrderEndData.ToString() + "'");

        //        return command.ExecuteEntityList<ConsginToAccLogEntity>();
        //    }
        //}

        public string GetNewCosignAcctLogOnlineDate()
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetNewCosignAcctLogOnlineDate");

            object obj = command.ExecuteScalar();
            if (obj != null)
            {
                return obj.ToString();
            }
            else
            {
                return null;
            }
        }

        //查找满足的结算规则
        public SettleRulesEntity GetRuleByConsginToAccLogSysNo(int consginToAccLogSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetRuleByConsginToAccLogSysNo");

            command.SetParameterValue("@ConsginToAccLogSysNo", consginToAccLogSysNo);

            return command.ExecuteEntity<SettleRulesEntity>();
        }
    }
}
