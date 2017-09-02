using ECCentral.BizEntity.Invoice.Refund;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Invoice.SqlDataAccess
{
    [VersionExport(typeof(IRefundPointDA))]
    public class RefundPointDA : IRefundPointDA
    {
        public int Insert(RefundPointInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertNetPayExtension");

            command.SetParameterValue("@OrderType", info.OrderType);
            command.SetParameterValue("@OrderSysNo", info.OrderSysNo);
            command.SetParameterValue("@SOSysNo", info.SOSysNo);
            command.SetParameterValue("@OrderAmt", info.OrderAmt);
            command.SetParameterValue("@PayAmt", info.PayAmt);
            command.SetParameterValue("@InUser", info.InUser);
            command.SetParameterValue("@EditUser", info.EditUser);
            command.SetParameterValue("@PayTypeSysNo", info.PayTypeSysNo);
            command.SetParameterValue("@ReferenceID", info.ReferenceID);
            command.SetParameterValue("@ExternalKey", info.ExternalKey);
            command.SetParameterValue("@Note", info.Note);
            command.SetParameterValue("@Status", info.RefundStatus);
            command.SetParameterValue("@ResponseContent", info.ResponseContent);
            command.SetParameterValue("@CompanyCode", info.CompanyCode);
            command.SetParameterValue("@LanguageCode", "zh-CN");
            command.SetParameterValue("@CurrencySysNo", info.CurrencySysNo);
            command.SetParameterValue("@StoreCompanyCode", "8601");
            command.SetParameterValue("@RefundLogSysNo", info.RefundLogSysNo);

            return Convert.ToInt32(command.ExecuteScalar());
        }

        public void Update(RefundPointInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateNetPayExtension");

            command.SetParameterValue("@SysNo", info.SysNo);
            command.SetParameterValue("@EditDate", info.EditDate);
            command.SetParameterValue("@EditUser", info.EditUser);
            command.SetParameterValue("@Status", info.RefundStatus);
            command.SetParameterValue("@ResponseContent", info.ResponseContent);

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新收款单
        /// </summary>
        /// <param name="info"></param>
        public void UpdateSOIncome(RefundPointInfo info)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateSoIncome");

            command.SetParameterValue("@ConfirmTime", info.EditDate);
            command.SetParameterValue("@ConfirmUserSysNo", info.CreateUserSysNo);
            command.SetParameterValue("@Status", 1);
            command.SetParameterValue("@OrderSysNo", info.OrderSysNo);
            command.SetParameterValue("@OrderType", (int)info.OrderType);
            command.SetParameterValue("@CompanyCode", info.CompanyCode);

            command.ExecuteNonQuery();
        }
    }
}
