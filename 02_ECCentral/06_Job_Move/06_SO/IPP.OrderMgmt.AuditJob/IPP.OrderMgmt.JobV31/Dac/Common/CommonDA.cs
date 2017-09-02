using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;
using IPP.OrderMgmt.JobV31.BusinessEntities.FPCheck;
using IPP.OrderMgmt.JobV31.BusinessEntities.Common;
using IPP.OrderMgmt.JobV31.BusinessEntities.AutoAudit;

namespace IPP.OrderMgmt.JobV31.Dac.Common
{
    public class CommonDA
    {
        public static bool IsTelPhoneCheck(int customerSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("IsTelPhoneCheck");
            command.SetParameterValue("@CustomerSysNo", customerSysNo);

            object result = command.ExecuteScalar();
            if (result != null)
            {
                return true;
            }
            return false;
        }

        public static bool IsNewCustomer(int customerSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("IsNewCustomer");
            command.SetParameterValue("@CustomerSysNo", customerSysNo);

            object result = command.ExecuteScalar();
            if (result == null)
            {
                return true;
            }
            return false;
        }

        public static string GetlocalWHByAreaSysNo(int areaSysNo, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetlocalWHByAreaSysNo");

            command.SetParameterValue("@AreaSysNo", areaSysNo);
            command.SetParameterValue("@CompanyCode", companyCode);

            object obj = command.ExecuteScalar();
            if (obj != null)
            {
                return obj.ToString();
            }
            return null;
        }

        public static List<CustomerEntity> GetMalevolenceCustomers(string CompanyCode)
        {
            List<CustomerEntity> result;

            DataCommand command = DataCommandManager.GetDataCommand("GetMalevolenceCustomers");
            command.SetParameterValue("@CompanyCode", CompanyCode);

            result = command.ExecuteEntityList<CustomerEntity>();

            result = (result == null) ? new List<CustomerEntity>() : result;

            return result;
        }


        public static List<PayTypeEntity> GetPayTypeList(string CompanyCode)
        {
            List<PayTypeEntity> result;

            DataCommand command = DataCommandManager.GetDataCommand("GetPayTypeList");
            command.SetParameterValue("@CompanyCode", CompanyCode);

            result = command.ExecuteEntityList<PayTypeEntity>();

            result = (result == null) ? new List<PayTypeEntity>() : result;

            return result;
        }

        public static List<SMSTypeInfo> GetSMSTypeInfoList()
        {
            List<SMSTypeInfo> result;

            DataCommand command = DataCommandManager.GetDataCommand("GetSMSTypeInfoList");

            result = command.ExecuteEntityList<SMSTypeInfo>();

            result = (result == null) ? new List<SMSTypeInfo>() : result;

            return result;
        }


        public static void SendSMS(SMSInfo tmpSMSInfo)
        {

            DataCommand command = DataCommandManager.GetDataCommand("InsertSMS");

            command.SetParameterValue("@CellNumber", tmpSMSInfo.CellNumber);
            command.SetParameterValue("@SMSContent", tmpSMSInfo.SMSContent);
            command.SetParameterValue("@Priority", tmpSMSInfo.Priority);
            command.SetParameterValue("@RetryCount", tmpSMSInfo.RetryCount);
            command.SetParameterValue("@CreateTime", tmpSMSInfo.CreateTime);
            command.SetParameterValue("@HandleTime", tmpSMSInfo.HandleTime);
            command.SetParameterValue("@Status", tmpSMSInfo.Status);
            command.SetParameterValue("@CreateUserSysNo", tmpSMSInfo.CreateUserSysNo);

            command.ExecuteNonQuery();
        }

        public static CustomerEntity GetCustomerInfoBySysNo(int sysno)
        {
            CustomerEntity result;

            DataCommand command = DataCommandManager.GetDataCommand("GetCustomerInfoBySysNo");

            command.SetParameterValue("@SysNo", sysno);

            result = command.ExecuteEntity<CustomerEntity>();

            return result;
        }

        public static void SendMail(EmailInfo tmpMailInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertEmail");

            command.SetParameterValue("@MailAddress", tmpMailInfo.MailAddress);
            command.SetParameterValue("@MailSubject", tmpMailInfo.MailSubject);
            command.SetParameterValue("@MailBody", tmpMailInfo.MailBody);
            command.SetParameterValue("@Status", tmpMailInfo.Status);
            command.SetParameterValue("@CCMailAddress", tmpMailInfo.CCMailAddress);
            command.SetParameterValue("@BCMailAddress", tmpMailInfo.BCMailAddress);
            command.SetParameterValue("@MailFrom", tmpMailInfo.MailFrom);
            command.SetParameterValue("@MailSenderName", tmpMailInfo.MailSenderName);

            command.ExecuteNonQuery();
        }

        public static bool ExistsNotLocalWH(int soSysNo, string localWH, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("ExistsNotLocalWH");

            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@LocalWH", localWH);
            command.SetParameterValue("@CompanyCode", companyCode);

            int result = command.ExecuteScalar<int>();
            if (result > 0)
            {
                return true;
            }
            return false;
        }

        public static void UpdateLocalWHMark(int soSysNo, string localWH, int targetStatus)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateLocalWHMark");

            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@LocalWH", localWH);
            command.SetParameterValue("@TargetStatus", targetStatus);

            command.ExecuteNonQuery();
        }


        public static List<ShipTypeInfo> GetShipTypeList(string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetShipTypeInfo");

            command.SetParameterValue("@CompanyCode",companyCode);

            return command.ExecuteEntityList<ShipTypeInfo>();
 
        }
    }
}
