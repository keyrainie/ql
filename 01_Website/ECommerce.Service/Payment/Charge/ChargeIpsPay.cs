using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Payment;

namespace ECommerce.Facade.Payment.Charge
{
    /// <summary>
    /// 环迅支付
    /// </summary>
    public class ChargeIPSPay : Charges
    {
        /// <summary>
        /// Sets the request form.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void SetRequestForm(ChargeContext context)
        {
            context.RequestForm["Mer_code"] = context.PaymentInfo.PaymentMode.BankCert;
            context.RequestForm["Billno"] = context.SOInfo.SoSysNo.ToString();
            context.RequestForm["Amount"] = context.SOInfo.RealPayAmt.ToString("######0.00");
            context.RequestForm["Date"] = context.SOInfo.OrderDate.ToString("yyyyMMdd");
            context.RequestForm["Currency_Type"] = "RMB";
            context.RequestForm["Gateway_Type "] = "01";
            context.RequestForm["Lang"] = "GB";
            context.RequestForm["Attach"] = context.SOInfo.Payment.PayTypeID.ToString();

            context.RequestForm["Merchanturl"] = BuildActionUrl(context.PaymentInfo.PaymentBase.BaseUrl,
                context.PaymentInfo.PaymentMode.PaymentCallbackUrl);
            context.RequestForm["ServerUrl"] = BuildActionUrl(context.PaymentInfo.PaymentBase.BaseUrl,
                context.PaymentInfo.PaymentMode.PaymentBgCallbackUrl);

            context.RequestForm["OrderEncodeType"] = "5";
            context.RequestForm["RetEncodeType"] = "17";
            context.RequestForm["Rettype"] = "1";

            if (context.PaymentInfo.PaymentMode.CustomConfigs != null
                && context.PaymentInfo.PaymentMode.CustomConfigs["DoCredit"] != null
                && context.PaymentInfo.PaymentMode.CustomConfigs["DoCredit"] == "1"
                && context.PaymentInfo.PaymentMode.CustomConfigs["Bankco"] != null)
            {
                context.RequestForm["DoCredit"] = context.PaymentInfo.PaymentMode.CustomConfigs["DoCredit"];
                context.RequestForm["Bankco"] = context.PaymentInfo.PaymentMode.CustomConfigs["Bankco"];
            }

            context.RequestForm["SignMD5"] = SignData(context);
        }

        /// <summary>
        /// Signs the data.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        private string SignData(ChargeContext context)
        {
            string result = string.Empty;

            Dictionary<string, object> signParams = new Dictionary<string, object>();

            signParams.Add("billno", context.RequestForm["Billno"]);
            signParams.Add("currencytype", context.RequestForm["Currency_Type"]);
            signParams.Add("amount", context.RequestForm["Amount"]);
            signParams.Add("date", context.RequestForm["Date"]);
            signParams.Add("orderencodetype", context.RequestForm["OrderEncodeType"]);

            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, object> item in signParams)
            {
                sb.AppendFormat("{0}{1}", item.Key, item.Value);
            }

            sb.Append(context.PaymentInfo.PaymentMode.BankCertKey);

            result = GetMD5(sb.ToString(), context.PaymentInfo.PaymentMode.CustomConfigs["encoding"]);

            return result;
        }

        /// <summary>
        /// Signs the data.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        private string SignData(CallbackContext context)
        {
            string result = string.Empty;

            Dictionary<string, object> signParams = new Dictionary<string, object>();

            signParams.Add("billno", context.ResponseForm["billno"]);
            signParams.Add("currencytype", context.ResponseForm["Currency_Type"]);
            signParams.Add("amount", context.ResponseForm["amount"]);
            signParams.Add("date", context.ResponseForm["date"]);
            signParams.Add("succ", context.ResponseForm["succ"]);
            signParams.Add("ipsbillno", context.ResponseForm["ipsbillno"]);
            signParams.Add("retencodetype", context.ResponseForm["retencodetype"]);

            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, object> item in signParams)
            {
                sb.AppendFormat("{0}{1}", item.Key, item.Value);
            }

            sb.Append(context.PaymentInfo.PaymentMode.BankCertKey);

            result = GetMD5(sb.ToString(), context.PaymentInfo.PaymentMode.CustomConfigs["encoding"]);

            return result;
        }

        /// <summary>
        /// Verifies the sign.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override bool VerifySign(CallbackContext context)
        {
            return context.ResponseForm["signature"] == SignData(context).ToLower();
        }

        /// <summary>
        /// Gets the pay result.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override bool GetPayResult(CallbackContext context)
        {
            return context.ResponseForm["succ"].Equals("Y");
        }

        /// <summary>
        /// Gets the so system no.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override int GetSOSysNo(CallbackContext context)
        {
            return int.Parse(context.ResponseForm["billno"]);
        }

        /// <summary>
        /// Gets the pay amount.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override decimal GetPayAmount(CallbackContext context)
        {
            return decimal.Parse(context.ResponseForm["amount"]);
        }

        /// <summary>
        /// Gets the serial number.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override string GetSerialNumber(CallbackContext context)
        {
            return context.ResponseForm["ipsbillno"];
        }

        /// <summary>
        /// Gets the pay process time.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override string GetPayProcessTime(CallbackContext context)
        {
            // 环迅文档《环迅IPS3.0系统接口手册V5.6》没有提供处理时间
            return base.GetPayProcessTime(context);
        }
    }
}
