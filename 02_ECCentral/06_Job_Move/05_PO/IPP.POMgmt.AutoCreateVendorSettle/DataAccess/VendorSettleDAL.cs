using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;
using POASNMgmt.AutoCreateVendorSettle.Compoents;
using POASNMgmt.AutoCreateVendorSettle.Entities;
using System.Data;

namespace POASNMgmt.AutoCreateVendorSettle.DataAccess
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
                

            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetConsginToAccLogList");

            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(command.CommandText, command, null, "Consign.SysNo desc"))
            {
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "consign.[Status]", DbType.Int32, "@Status", QueryConditionOperatorType.Equal, 0);
                builder.ConditionConstructor.AddInCondition<int>(QueryConditionRelationType.AND, "ex.SettlePeriodType", DbType.Int32, payPeriodTypes);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "consign.CompanyCode", DbType.String, "@CompanyCode", QueryConditionOperatorType.Equal, GlobalSettings.CompanyCode);
                var lastdate = DateTime.Today.AddMonths(-1);
                //欧亚强制为自然月
                DateTime begin = new DateTime(lastdate.Year,lastdate.Month,1);
                DateTime End = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "consign.[CreateTime]", DbType.DateTime, "@CreateTimeFrom", QueryConditionOperatorType.MoreThanOrEqual, begin);
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "consign.[CreateTime]", DbType.DateTime, "@CreateTimeTo", QueryConditionOperatorType.LessThan, End);

                command.CommandText = builder.BuildQuerySql();

                return command.ExecuteEntityList<ConsginToAccLogEntity>();
            }
        }
        
        //查找满足的结算规则
        public SettleRulesEntity GetRuleByConsginToAccLogSysNo(int consginToAccLogSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetRuleByConsginToAccLogSysNo");

            command.SetParameterValue("@ConsginToAccLogSysNo", consginToAccLogSysNo);

            return command.ExecuteEntity<SettleRulesEntity>();
        }

        //查找满足的结算规则
        public VendorDeductEntity GetVendorDeductInfo(int VendorSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetVendorDeductInfo");

            command.SetParameterValue("@VendorSysNo", VendorSysNo);

            return command.ExecuteEntity<VendorDeductEntity>();
        }
    }
}
