using System;
using System.Collections.Generic;
using System.Configuration;
using IPP.ECommerceMgmt.SendAmbassadorPoints.BusinessEntities;
using IPP.ECommerceMgmt.SendAmbassadorPoints.Utilities;
using Newegg.Oversea.Framework.DataAccess;

namespace IPP.ECommerceMgmt.SendAmbassadorPoints.DA
{
    public class SendAmbassadorPointsDA
    {
        public static List<SOMasterEntity> GetSO(int pointLogType, DateTime currentDate, int startNumber)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Agent_GetOrder" + pointLogType);
            cmd.SetParameterValue("@CompanyCode", AppConfigHelper.CompanyCode);
            cmd.SetParameterValue("@FromDate", currentDate);
            cmd.SetParameterValue("@Top", AppConfigHelper.PerTopCount);
            cmd.SetParameterValue("@StartNumber", startNumber);
            cmd.SetParameterValue("@TestAgentSysNo", AppConfigHelper.TestAgentSysNo);
            cmd.SetParameterValue("@TestSOSysNo", AppConfigHelper.TestSOSysNo);
            return cmd.ExecuteEntityList<SOMasterEntity>();
        }

        public static List<RmaRefundEntity> GetRefundBySO(int soId)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Agent_GetRefundBySO");
            dc.SetParameterValue("@CompanyCode", AppConfigHelper.CompanyCode);
            dc.SetParameterValue("@SoSysNo", soId);
            return dc.ExecuteEntityList<RmaRefundEntity>();
        }

        public static List<SOMasterEntity> GetInValidSubSO(int soId)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Agent_GetInvalidSO");
            dc.SetParameterValue("@CompanyCode", AppConfigHelper.CompanyCode);
            dc.SetParameterValue("@SoSysNo", soId);
            return dc.ExecuteEntityList<SOMasterEntity>();
        }

        public static SOMasterEntity GetGroupBuyingSO(int soId)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Agent_GetGroupBuyingSO");
            dc.SetParameterValue("@CompanyCode", AppConfigHelper.CompanyCode);
            dc.SetParameterValue("@SoSysNo", soId);
            return dc.ExecuteEntity<SOMasterEntity>();
        }

        public static SOMasterEntity GetGroupBuyingSubSO(int soId)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Agent_GetGroupBuyingSubSO");
            dc.SetParameterValue("@CompanyCode", AppConfigHelper.CompanyCode);
            dc.SetParameterValue("@SoSysNo", soId);
            return dc.ExecuteEntity<SOMasterEntity>();
        }

        public static int GetPointAmountByCustomer(int customerId, DateTime fromDate, DateTime toDate,int pointType)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Agent_GetPointAmountByCustomer");
            dc.SetParameterValue("@CustomerSysNo", customerId);
            dc.SetParameterValue("@PointType", pointType);
            dc.SetParameterValue("@FromDate", fromDate);
            dc.SetParameterValue("@ToDate", toDate);
            dc.SetParameterValue("@CompanyCode", AppConfigHelper.CompanyCode);
            return dc.ExecuteScalar<int>();
        }

        public static void SendMailAboutExceptionInfo(string ErrorMsg, string CompanyCode)
        {
            string MailAddress = Convert.ToString(ConfigurationManager.AppSettings["MailAddress"]);
            string CCMailAddress = Convert.ToString(ConfigurationManager.AppSettings["CCMailAddress"]);
            string MailSubject = DateTime.Now + " IPP-ECommerceMgmt-SendAmbassadorPoints(新蛋大使加积分) 运行时异常";

            DataCommand command = DataCommandManager.GetDataCommand("SendMailInfo");
            command.SetParameterValue("@MailAddress", MailAddress);
            command.SetParameterValue("@CCMailAddress", CCMailAddress);
            command.SetParameterValue("@MailSubject", MailSubject);
            command.SetParameterValue("@MailBody", ErrorMsg);
            command.SetParameterValue("@CompanyCode", CompanyCode);
            command.SetParameterValue("@LanguageCode", ConfigurationManager.AppSettings["LanguageCode"]);

            command.ExecuteNonQuery();
        }

        public static List<SOMasterEntity> GetAllSubSO(int soId)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("Agent_GetAllSubSO");
            dc.SetParameterValue("@CompanyCode", AppConfigHelper.CompanyCode);
            dc.SetParameterValue("@SoSysNo", soId);
            return dc.ExecuteEntityList<SOMasterEntity>();
        }
    }
}
