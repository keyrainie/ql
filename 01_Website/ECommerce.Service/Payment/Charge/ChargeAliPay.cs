using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Payment;

namespace ECommerce.Facade.Payment.Charge
{
    public class ChargeAliPay : Charges
    {
        public override void SetRequestForm(ChargeContext context)
        {
            //不要改变顺序
            context.RequestForm["_input_charset"] = context.PaymentInfoMerchant.Encoding;
            context.RequestForm["body"] = "";//商品描述 可空
            context.RequestForm["buyer_email"] = "";//买家支付宝账号 可空
            context.RequestForm["extra_common_param"] = context.PaymentModeId.ToString();
            context.RequestForm["it_b_pay"] = "";//超时时间 可空
            context.RequestForm["notify_url"] = BuildActionUrl(context.PaymentInfo.PaymentBase.BaseUrl, context.PaymentInfo.PaymentMode.PaymentBgCallbackUrl);//服务器异步通知页面路径 可空
            context.RequestForm["out_trade_no"] = context.SOInfo.SoSysNo.ToString();//商户网站唯一订单号 必填
            context.RequestForm["partner"] = context.PaymentInfoMerchant.MerchantCert;//合作者身份ID,以2008开头 必填
            context.RequestForm["payment_type"] = "1";//支付类型 必填
            context.RequestForm["paymethod"] = "";//默认支付方式,默认识别为余额支付(creditPay、directPay) 可空
            context.RequestForm["price"] = "";//商品单价 规则：price、quantity能代替total_fee。即存在total_fee，就不能存在price和quantity；存在price、quantity，就不能存在total_fee 可空
            context.RequestForm["quantity"] = ""; //可空
            context.RequestForm["return_url"] = BuildActionUrl(context.PaymentInfo.PaymentBase.BaseUrl, context.PaymentInfo.PaymentMode.PaymentCallbackUrl); //页面跳转同步通知地址 可空
            context.RequestForm["royalty_parameters"] = "";//分润账号集 可空
            context.RequestForm["royalty_type"] = "";//提成类型10 当传递了参数royalty_parameters时，提成类型参数不能为空 可空
            context.RequestForm["seller_email"] = context.PaymentInfoMerchant.MerchantNO;//卖家支付宝账号
            context.RequestForm["service"] = "create_direct_pay_by_user";//接口名称 必填
            context.RequestForm["show_url"] = "";//商品展示网址 可空
            context.RequestForm["subject"] = "SJSD_SO_" + context.SOInfo.SoSysNo.ToString();//商品名称 必填
            context.RequestForm["token"] = "";//快捷登录授权令牌 可空
            context.RequestForm["total_fee"] = context.SOInfo.RealPayAmt.ToString("F2");//交易金额 可空
            context.RequestForm["sign"] = SignData(context);//签名 必填
            context.RequestForm["sign_type"] = "MD5";//签名方式DSA、RSA、MD5 必填
        }

        public override bool VerifySign(CallbackContext context)
        {
            if (context != null && context.ResponseForm != null && context.ResponseForm.Count > 0)
            {
                string sign = string.Empty;
                string signType;
                if (context.ResponseForm.AllKeys.Contains("sign"))
                {
                    sign = context.ResponseForm["sign"];
                    //context.ResponseForm.Remove("sign");
                }
                if (context.ResponseForm.AllKeys.Contains("sign_type"))
                {
                    signType = context.ResponseForm["sign_type"];
                    //context.ResponseForm.Remove("sign_type");
                }
                string[] keys = context.ResponseForm.AllKeys;
                Array.Sort(keys);
                StringBuilder build = new StringBuilder();
                int index = 0;
                foreach (var item in keys)
                {
                    if (item == "sign" || item == "sign_type")
                    {
                        continue;
                    }
                    build.Append(item);
                    build.Append("=");
                    build.Append(context.ResponseForm[item]);
                    //还包括了本该剔除的sign和sign_type
                    if (index != keys.Length - 3)
                    {
                        build.Append("&");
                    }
                    index = index + 1;
                }

                build.Append(context.PaymentInfoMerchant.MerchantCertKey);
                if (sign == GetMD5(build.ToString(), context.PaymentInfoMerchant.Encoding))
                {
                    return true;
                }
            }
            return false;
        }

        public override bool GetPayResult(CallbackContext context)
        {
            return context.ResponseForm["trade_status"].Equals("TRADE_FINISHED") ||
                context.ResponseForm["trade_status"].Equals("TRADE_SUCCESS");
        }
        public override int GetSOSysNo(CallbackContext context)
        {
            return int.Parse(context.ResponseForm["out_trade_no"]);
        }

        public override decimal GetPayAmount(CallbackContext context)
        {
            return decimal.Parse(context.ResponseForm["total_fee"]);
        }

        public override string GetSerialNumber(CallbackContext context)
        {
            if (!string.IsNullOrWhiteSpace(context.ResponseForm["trade_no"]))
            {
                return context.ResponseForm["trade_no"];
            }
            return "";
        }

        private string SignData(ChargeContext context)
        {
            StringBuilder builder = new StringBuilder();
            int index = 0;
            foreach (var item in context.RequestForm.AllKeys)
            {
                string value = context.RequestForm[item];
                if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(value.Trim()))
                {
                    builder.Append(item);
                    builder.Append("=");
                    builder.Append(context.RequestForm[item]);
                    if (index != context.RequestForm.AllKeys.Length - 1)
                    {
                        builder.Append("&");
                    }

                }
                index = index + 1;
            }

            builder.Append(context.PaymentInfoMerchant.MerchantCertKey);


            string dataStr = builder.ToString();
            string signStr = GetMD5(dataStr, context.PaymentInfoMerchant.Encoding);

            //Debug模式下记录相关信息至日志
            if (context.PaymentInfo.PaymentMode.Debug.Equals("1"))
            {
                string sourceData = BuildStringFromNameValueCollection(context.RequestForm);
                ECommerce.Utility.Logger.WriteLog(string.Format("原始值：{0}，签名明文：{1}，签名：{2}", sourceData, dataStr, signStr), "AliPay", "PaySignData");
            }

            return signStr;
        }
    }
}
