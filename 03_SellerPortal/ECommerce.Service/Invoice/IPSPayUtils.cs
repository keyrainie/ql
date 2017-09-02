using ECommerce.Entity.Invoice;
//using ECommerce.Service.IPSPayService;
using ECommerce.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ECommerce.Service.Invoice
{
    public class IPSPayUtils
    {
        public RefundResult Refund(RefundEntity refundEntity)
        {
            RefundResult result = new RefundResult();
            result.IsSync = true;
            NetPayInfo netpayInfo = NetPayService.GetValidNetPayBySOSysNo(refundEntity.SOSysNo);
            string partner = AppSettingManager.GetSetting("Invoice", "IPSPartner");
            string IPSSecurityKey = AppSettingManager.GetSetting("Invoice", "IPSSecurityKey");
            string sign = Sign(string.Format("{0}{1}{2}{3}{4}", partner, refundEntity.SOSysNo.ToString(), refundEntity.OrderDate.ToString("yyyyMMdd"), refundEntity.RefundAmt.ToString("f2"), IPSSecurityKey));
            //RefundMsg msg = new IPSPayService.ServiceSoapClient().Refund(partner, sign, refundEntity.OrderDate.ToString("yyyyMMdd"), refundEntity.SOSysNo.ToString(), (double)refundEntity.RefundAmt, "");
            //if (msg.ErrCode == "0000")
            //{
            //    if (msg.Sign == Sign(string.Format("{0}{1}{2}{3}{4}{5}{6}", partner, msg.TradeBillNo, msg.TradeTime, msg.RealRefundAmount.ToString("f2"), msg.CanRefundAmount.ToString("f2"), msg.RefundBillNo, IPSSecurityKey)))
            //    {
            //        result.Result = true;
            //    }
            //    else
            //    {
            //        result.Result = false;
            //        result.Message = "验签失败";
            //    }
            //}
            //else
            //{
            //    result.Result = false;
            //    result.Message = GetMessage(msg.ErrCode);
            //    //result.Message += refundEntity.SOSysNo.ToString() + " " + refundEntity.OrderDate.ToString("YYYYMMDD");
            //}

            return result;

        }
        /// <summary>
        /// 退款message
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        internal static string GetMessage(string code)
        {
            switch (code)
            {
                case "1001":
                    return "商户不存在";
                case "1002":
                    return "商户证书不存在";
                case "1003":
                    return "商户签名验证不通过";
                case "1004":
                    return "输入的交易有误";
                case "1005":
                    return "输入的交易时间有误";
                case "1006":
                    return "找不到这笔交易";
                case "1007":
                    return "不是您的交易";
                case "1008":
                    return "不是您的消费交易";
                case "1009":
                    return "退款金额过大";
                case "1010":
                    return "退款金额过低";
                case "1011":
                    return "退款失败，请联系环迅技术支持";
                case "1012":
                    return "无法找到IPS订单号";
                case "1013":
                    return "无法找到 BankTypeBankType";
                case "1021":
                    return "商户号不能为空";
                case "1022":
                    return "签名数据不能为空";
                case "1023":
                    return "商户时间不能为空";
                case "1024":
                    return "商户订单号不能为空";
                case "1025":
                    return "退款金额不能为空";
                case "1026":
                    return "商户号格式不正确";
                case "1027":
                    return "商户时间格式不正确 ";
                case "1028":
                    return "退款金额格式不正确";
                case "1029":
                    return "退款备注字符最多";
                case "1111":
                    return "未知错误，请联系环迅技术支持";
                default:
                    return "未知的错误代码";
            }
        }
        internal static string Sign(string prestr)
        {
            StringBuilder sb = new StringBuilder(32);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.Default.GetBytes(prestr));
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }

            return sb.ToString();
        }
    }
}
