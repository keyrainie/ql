using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;
using IPP.OrderMgmt.JobV31.BusinessEntities.AutoAudit;

namespace IPP.OrderMgmt.JobV31.Dac.AutoAudit
{
    public class SOAutoAuditMaintainDA
    {
        public static int UpdateSO4PassAutoAudit(SOQueryEntity entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateSO4PassAutoAudit");

            command.SetParameterValue("@SysNo",entity.SystemNumber);
            command.SetParameterValue("@AuditUserSysNo", entity.AuditUserSysNo);
            command.SetParameterValue("@CompanyCode", entity.CompanyCode);

            return command.ExecuteNonQuery();
        }

        public static int UpdateSO4AuditUserInfo(int soSysNo, int userSysNo, string CompanyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateSO4AuditUserInfo");

            command.SetParameterValue("@SysNo", soSysNo);
            command.SetParameterValue("@AuditUserSysNo", userSysNo);
            command.SetParameterValue("@CompanyCode", CompanyCode);

            return command.ExecuteNonQuery();
        }

        public static int UpdateNetPay(SONetPayEntity entity, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateNetPayBySysNo");

            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@ApproveUserSysNo", entity.ApproveUserSysNo);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@CompanyCode", companyCode);

            return command.ExecuteNonQuery();
        }

        public static int UpdateCheckShippingAuditTypeBySysNo(int soSysNo, AppEnum.AuditType auditType, string autoAuditMemo, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateCheckShippingAuditTypeBySysNo");

            command.SetParameterValue("@SOSysNo", soSysNo);
            command.SetParameterValue("@AuditType", (int)auditType);
            command.SetParameterValue("@AutoAuditMemo", autoAuditMemo);
            command.SetParameterValue("@CompanyCode", companyCode);

            return command.ExecuteNonQuery();
        }
       
        internal static void CreateSOIncome(SOIncomeEntity soIncomeInfo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateSOIncome");

            command.SetParameterValue("@OrderType", soIncomeInfo.OrderType);
            command.SetParameterValue("@OrderSysNo", soIncomeInfo.OrderSysNo);
            command.SetParameterValue("@OrderAmt", soIncomeInfo.OrderAmt);
            command.SetParameterValue("@IncomeStyle", soIncomeInfo.IncomeStyle);
            command.SetParameterValue("@IncomeAmt", soIncomeInfo.IncomeAmt);
            command.SetParameterValue("@IncomeTime", soIncomeInfo.IncomeTime);
            command.SetParameterValue("@IncomeUserSysNo", soIncomeInfo.IncomeUserSysNo);
            command.SetParameterValue("@ConfirmTime", soIncomeInfo.ConfirmTime);
            command.SetParameterValue("@ConfirmUserSysNo", soIncomeInfo.ConfirmUserSysNo);
            command.SetParameterValue("@Note", soIncomeInfo.Note);
            command.SetParameterValue("@Status", soIncomeInfo.Status);
            command.SetParameterValue("@PrepayAmt", soIncomeInfo.PrepayAmt);
            command.SetParameterValue("@GiftCardPayAmt", soIncomeInfo.GiftCardPayAmt);

             command.ExecuteNonQuery();
        }
    }
}
