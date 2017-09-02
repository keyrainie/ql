using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Nesoft.ECWeb.Entity.Payment;
using Nesoft.Utility;
using Nesoft.ECWeb.DataAccess.Shopping;

namespace Nesoft.ECWeb.Facade.Payment.Charge
{
    public class ChargeEasiPayOld : Charges
    {
        public override void SetRequestForm(ChargeContext context)
        {
            //不要改变顺序，按支付接口文档所给顺序设置值

            #region 单笔值计算
            decimal allCargoTotalPrice = 0m;
            string cargoDescript = "";
            decimal totalTariffAmount = 0m;
            decimal otherPrice = Math.Abs(context.SOInfo.Amount.PrepayAmt) * -1;
            int cargoTypeNum = 0;
            string cargoName = "";
            string cargoCode = "";
            string _HSCode = "";
            string cargoNum = "";
            string cargoUnitPrice = "";
            string cargoTotalPrice = "";
            string cargoTotalTax = "";

            foreach (var item in context.SOInfo.SOItemList)
            {
                cargoDescript += (string.IsNullOrEmpty(cargoDescript) ? "" : "；") + (item.ProductName.Replace("#", "").Replace("%", "").Replace("&", "").Replace("+", "") + "描述");
                totalTariffAmount += item.TariffAmt * item.Quantity;
                cargoTypeNum += item.Quantity;
                item.ProductName = item.ProductName.Replace("#", "").Replace("%", "").Replace("&", "").Replace("+", "");
                cargoName += (string.IsNullOrEmpty(cargoName) ? "" : "^") + item.ProductName;
                cargoCode += (string.IsNullOrEmpty(cargoCode) ? "" : "^") + item.EntryCode;
                _HSCode += (string.IsNullOrEmpty(_HSCode) ? "" : "^") + item.TariffCode;
                cargoNum += (string.IsNullOrEmpty(cargoNum) ? "" : "^") + item.Quantity.ToString();
                //折扣除不尽时，把多余的作为OtherPrice上送
                decimal currOtherPrice = Math.Abs(item.DiscountAmt) % item.Quantity;
                otherPrice += currOtherPrice * -1;
                //Item上是商品本身的价格，需要排除折扣
                decimal unitPrice = item.OriginalPrice - ((Math.Abs(item.DiscountAmt) - currOtherPrice) / item.Quantity);
                unitPrice = decimal.Parse(unitPrice.ToString("F2"));
                cargoUnitPrice += (string.IsNullOrEmpty(cargoUnitPrice) ? "" : "^") + unitPrice.ToString("F2");
                cargoTotalPrice += (string.IsNullOrEmpty(cargoTotalPrice) ? "" : "^") + (unitPrice * item.Quantity).ToString("F2");
                cargoTotalTax += (string.IsNullOrEmpty(cargoTotalTax) ? "" : "^") + (item.TariffAmt * item.Quantity).ToString("F2");
                otherPrice += Math.Abs(item.PromotionDiscount * item.Quantity) * -1;
                allCargoTotalPrice += unitPrice * item.Quantity;
            }
            //积分支付作为其他金额报关
            otherPrice += Math.Abs(context.SOInfo.Amount.PointPay * 1.00m / decimal.Parse(Nesoft.ECWeb.Entity.ConstValue.PointExhangeRate)) * -1;
            #endregion

            context.RequestForm["InputCharset"] = "1";
            context.RequestForm["Version"] = "v1.2";
            context.RequestForm["Language"] = "1";
            context.RequestForm["SignType"] = "1";
            context.RequestForm["PageUrl"] = BuildActionUrl(context.PaymentInfo.PaymentBase.BaseUrl, context.PaymentInfo.PaymentMode.PaymentCallbackUrl);
            context.RequestForm["BgUrl"] = BuildActionUrl(context.PaymentInfo.PaymentBase.BaseUrl, context.PaymentInfo.PaymentMode.PaymentBgCallbackUrl);
            context.RequestForm["ShowUrl"] = BuildActionUrl(context.PaymentInfo.PaymentBase.BaseUrl, context.PaymentInfo.PaymentMode.PaymentCallbackUrl);
            context.RequestForm["MerchantOrderId"] = context.SOInfo.SoSysNo.ToString();
            context.RequestForm["AssBillNo"] = "";
            context.RequestForm["TradeName"] = context.PaymentInfoMerchant.MerchantName;
            context.RequestForm["TradeCode"] = context.PaymentInfoMerchant.CustomConfigs["FirstMerchantCode"];
            context.RequestForm["SrcNcode"] = context.PaymentInfoMerchant.MerchantNO;
            context.RequestForm["OrderCommitTime"] = context.SOInfo.OrderDate.ToString("yyyyMMddHHmmss");
            //发件信息，从Appsetting配置中取
            //仓库编号
            string warehouseNumber = context.SOInfo.SOItemList != null && context.SOInfo.SOItemList.Count > 0 ? context.SOInfo.SOItemList[0].WarehouseNumber.Trim() : "";
            context.RequestForm["SenderName"] = AppSettingManager.GetSetting("PaySenderInfo", string.Format("Sender_{0}_SenderName", warehouseNumber));
            context.RequestForm["SenderTel"] = AppSettingManager.GetSetting("PaySenderInfo", string.Format("Sender_{0}_SenderTel", warehouseNumber));
            context.RequestForm["SenderCompanyName"] = AppSettingManager.GetSetting("PaySenderInfo", string.Format("Sender_{0}_SenderCompanyName", warehouseNumber));
            context.RequestForm["SenderAddr"] = AppSettingManager.GetSetting("PaySenderInfo", string.Format("Sender_{0}_SenderAddr", warehouseNumber));
            context.RequestForm["SenderZip"] = AppSettingManager.GetSetting("PaySenderInfo", string.Format("Sender_{0}_SenderZip", warehouseNumber));
            context.RequestForm["SenderCity"] = AppSettingManager.GetSetting("PaySenderInfo", string.Format("Sender_{0}_SenderCity", warehouseNumber));
            context.RequestForm["SenderProvince"] = AppSettingManager.GetSetting("PaySenderInfo", string.Format("Sender_{0}_SenderProvince", warehouseNumber));
            //发件地国家
            string countryCode = context.SOInfo.SOItemList[0].CountryCode;
            #region 三位国家代码适配
            countryCode = AppSettingManager.GetSetting("CountryCode", string.Format("CountryCode_{0}", countryCode));
            #endregion
            context.RequestForm["SenderCountry"] = countryCode;
            //订单商品信息简述
            cargoDescript = !string.IsNullOrEmpty(cargoDescript) && cargoDescript.Length > 256 ? cargoDescript.Substring(0, 256) : cargoDescript;//限制字符不能大于256
            context.RequestForm["CargoDescript"] = string.IsNullOrEmpty(cargoDescript) ? "无商品信息简述" : cargoDescript;
            //全部购买商品合计总价
            context.RequestForm["AllCargoTotalPrice"] = allCargoTotalPrice.ToString("F2");
            //税费
            context.RequestForm["AllCargoTotalTax"] = totalTariffAmount.ToString("F2");
            //物流运费
            context.RequestForm["ExpressPrice"] = context.SOInfo.Amount.ShipPrice.ToString("F2");
            //其他金额
            context.RequestForm["OtherPrice"] = otherPrice.ToString("F2");
            //支付总金额=全部商品合计总价+税费+物流运费+其他金额（其他金额为负数）-余额支付金额
            context.RequestForm["PayTotalPrice"] = (allCargoTotalPrice + totalTariffAmount + context.SOInfo.Amount.ShipPrice + otherPrice).ToString("F2");
            //付款币种
            context.RequestForm["PayCUR"] = "CNY";
            //收款币种，固定为港币，测试环境支持USD美元
            context.RequestForm["CrtCUR"] = context.PaymentInfoMerchant.CurCode;
            //收货地国家
            context.RequestForm["RecCountry"] = "中国";
            //收货地省/州
            context.RequestForm["RecProvince"] = string.IsNullOrWhiteSpace(context.SOInfo.ReceiveProvinceName) ? "无" : context.SOInfo.ReceiveProvinceName;
            //收货地城市
            context.RequestForm["RecCity"] = string.IsNullOrWhiteSpace(context.SOInfo.ReceiveCityName) ? "无" : context.SOInfo.ReceiveCityName;
            //收货地地址
            context.RequestForm["RecAddress"] = string.IsNullOrWhiteSpace(context.SOInfo.ReceiveAddress) ? "无" : context.SOInfo.ReceiveAddress;
            //收货地邮编
            context.RequestForm["RecZip"] = "";
            //购买商品总数量
            context.RequestForm["CargoTypeNum"] = cargoTypeNum.ToString();
            //单项购买商品名称
            context.RequestForm["CargoName"] = cargoName;
            //单项购买商品编号
            context.RequestForm["CargoCode"] = cargoCode;
            //单项购买商品税则号
            context.RequestForm["HSCode"] = _HSCode;
            //单项购买商品数量
            context.RequestForm["CargoNum"] = cargoNum;
            //单项购买商品单价
            context.RequestForm["CargoUnitPrice"] = cargoUnitPrice;
            //单项购买商品总价
            context.RequestForm["CargoTotalPrice"] = cargoTotalPrice;
            //单项购买商品行邮税总价
            context.RequestForm["CargoTotalTax"] = cargoTotalTax;
            //业务类型
            context.RequestForm["ServerType"] = context.SOInfo.SOItemList[0].CountryCode.ToUpper().Equals("CHN") ? "S02" : "S01";
            //扩展字段1
            context.RequestForm["Spt1"] = "";
            //扩展字段2
            context.RequestForm["Spt2"] = "";
            //签名
            context.RequestForm["SignMsg"] = SignData(context);
        }

        public override bool VerifySign(CallbackContext context)
        {
            string localSign = SignData(context);
            if (context.ResponseForm.AllKeys.Contains("SignMsg")
                && context.ResponseForm["SignMsg"].ToLower().Trim().Equals(localSign.ToLower().Trim()))
            {
                context.SOSysNo = int.Parse(context.ResponseForm["MerchantOrderId"]);
                return true;
            }

            Nesoft.Utility.Logger.WriteLog(string.Format("验签失败，东方支付签名为：{0}，本地签名：{1}！", context.ResponseForm["SignMsg"], localSign), "EasiPay", "VerifySign");
            return false;
        }

        public override bool GetPayResult(CallbackContext context)
        {
            return context.ResponseForm["PayResult"].Trim().Equals("10");
        }

        public override int GetSOSysNo(CallbackContext context)
        {
            return int.Parse(context.ResponseForm["MerchantOrderId"]);
        }

        public override decimal GetPayAmount(CallbackContext context)
        {
            return decimal.Parse(context.ResponseForm["PayAmount"]);
        }

        public override string GetSerialNumber(CallbackContext context)
        {
            return "0";
        }

        public override string GetPayProcessTime(CallbackContext context)
        {
            return DateTime.Now.ToString("yyyymmddHHMMss");
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

            string dataStr = string.Format("{0}{1}", builder.ToString().TrimEnd('&'), context.PaymentInfoMerchant.MerchantCertKey);
            string signStr = GetMD5(dataStr, context.PaymentInfoMerchant.Encoding).ToUpper();

            //Debug模式下记录相关信息至日志
            if (context.PaymentInfo.PaymentMode.Debug.Equals("1"))
            {
                string sourceData = BuildStringFromNameValueCollection(context.RequestForm);
                Nesoft.Utility.Logger.WriteLog(string.Format("原始值：{0}，签名明文：{1}，签名：{2}", sourceData, dataStr, signStr), "EasiPay", "PaySignData");
            }

            return signStr;
        }

        private string SignData(CallbackContext context)
        {
            StringBuilder builder = new StringBuilder();
            int index = 0;
            foreach (var item in context.ResponseForm.AllKeys)
            {
                string value = context.ResponseForm[item];
                if (!item.Equals("SignMsg") && !string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(value.Trim()))
                {
                    builder.Append(item);
                    builder.Append("=");
                    builder.Append(System.Web.HttpUtility.UrlDecode(context.ResponseForm[item]));
                    if (index != context.ResponseForm.AllKeys.Length - 1)
                    {
                        builder.Append("&");
                    }

                }
                index = index + 1;
            }

            string dataStr = string.Format("{0}{1}", builder.ToString().TrimEnd('&'), context.PaymentInfoMerchant.MerchantCertKey);
            string signStr = GetMD5(dataStr, context.PaymentInfoMerchant.Encoding);

            //Debug模式下记录相关信息至日志
            if (context.PaymentInfo.PaymentMode.Debug.Equals("1"))
            {
                Nesoft.Utility.Logger.WriteLog(string.Format("签名明文：{0}，签名：{1}", dataStr, signStr), "EasiPay", "CallbackSignData");
            }

            return signStr;
        }
    }

    public class ChargeEasiPay : Charges
    {
        public override void SetRequestForm(ChargeContext context)
        {
            List<Dictionary<string, string>> detailList = new List<Dictionary<string, string>>();

            #region 1.单笔值计算
            decimal allCargoTotalPrice = 0m;
            decimal totalTariffAmount = 0m;
            decimal otherPrice = Math.Abs(context.SOInfo.Amount.PrepayAmt) * -1;
            foreach (var item in context.SOInfo.SOItemList)
            {
                totalTariffAmount += item.TariffAmt * item.Quantity;
                item.ProductName = item.ProductName.Replace("#", "").Replace("%", "").Replace("&", "").Replace("+", "");
                //折扣除不尽时，把多余的作为OtherPrice上送
                decimal currOtherPrice = Math.Abs(item.DiscountAmt) % item.Quantity;
                otherPrice += currOtherPrice * -1;
                //Item上是商品本身的价格，需要排除折扣
                decimal unitPrice = item.OriginalPrice - ((Math.Abs(item.DiscountAmt) - currOtherPrice) / item.Quantity);
                unitPrice = decimal.Parse(unitPrice.ToString("F2"));
                otherPrice += Math.Abs(item.PromotionDiscount * item.Quantity) * -1;
                allCargoTotalPrice += unitPrice * item.Quantity;
            }
            //积分支付作为其他金额报关
            otherPrice += Math.Abs(context.SOInfo.Amount.PointPay * 1.00m / decimal.Parse(Nesoft.ECWeb.Entity.ConstValue.PointExhangeRate)) * -1;
            #endregion
            
            #region 2.基本信息
            Dictionary<string, string> baseList = new Dictionary<string, string>();
            //订单号
            baseList["BILLNO"] = context.SOInfo.SoSysNo.ToString();
            //线上回调地址
            baseList["CALLBACKURL"] = BuildActionUrl(context.PaymentInfo.PaymentBase.BaseUrl, context.PaymentInfo.PaymentMode.PaymentCallbackUrl);
            //线下后台通知商户地址
            baseList["BGURL"] = BuildActionUrl(context.PaymentInfo.PaymentBase.BaseUrl, context.PaymentInfo.PaymentMode.PaymentBgCallbackUrl);
            //请求时间
            baseList["REQ_TIME"] = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            //一级商户代码
            baseList["SRC_NCODE"] = context.PaymentInfoMerchant.CBTSRC_NCode;
            //二级商户代码
            baseList["REC_NCODE"] = context.PaymentInfoMerchant.CBTREC_NCode;
            //支付业务
            baseList["PAY_BIZ"] = "30";
            //支付总金额
            baseList["PAY_AMOUNT"] = (allCargoTotalPrice + totalTariffAmount + context.SOInfo.Amount.ShipPrice + otherPrice).ToString("F2");
            //计价币种
            baseList["PAY_CURRENCY"] = "CNY";
            //贸易类型
            baseList["TRADE_TYPE"] = "010204";
            //商品总数
            baseList["CARGO_SUM"] = context.SOInfo.SOItemList.Count.ToString();
            //资金用途
            baseList["TXUSE"] = "10";
            //交易描述
            baseList["TRX_DESC"] = "";
            //订单链接地址
            baseList["BILL_LINK"] = "";
            baseList["SPT1"] = "";
            baseList["SPT2"] = "";
            baseList["SPT3"] = "";
            #endregion
            
            //其他金额可以抵扣货款和运费，先抵扣货款，再抵扣运费
            decimal productAmt = 0m;
            decimal shippingAmt = 0m;
            if (Math.Abs(otherPrice) > Math.Abs(allCargoTotalPrice))
            {
                productAmt = 0m;
                shippingAmt = context.SOInfo.Amount.ShipPrice - (Math.Abs(otherPrice) - Math.Abs(allCargoTotalPrice));
            }
            else
            {
                productAmt = Math.Abs(allCargoTotalPrice) -  Math.Abs(otherPrice);
                shippingAmt = context.SOInfo.Amount.ShipPrice;
            }

            #region 3.货款
            Dictionary<string, string> productFeeList = new Dictionary<string, string>();
            //子交易种类
            productFeeList["BILL_TYPE"] = "10";
            //订单时间
            productFeeList["BILL_DATE"] = context.SOInfo.OrderDate.ToString("yyyy-MM-ddTHH:mm:ss");
            //支付金额
            productFeeList["PAY_AMOUNT"] = productAmt.ToString("F2");
            //支付币种
            productFeeList["PAY_CURRENCY"] = context.PaymentInfoMerchant.PayCurrencyCode;
            //收款币种
            productFeeList["CRT_CURRENCY"] = context.PaymentInfoMerchant.CurCode;
            //是否跨境结算(外汇结算)标志
            productFeeList["BORDER_MARK"] = context.PaymentInfoMerchant.CurCode.Equals("CNY") ? "00" : "01";
            //收款方代码类型
            productFeeList["CRT_CODE_TYPE"] = "10";
            //收款方代码
            productFeeList["CRT_CODE"] = context.PaymentInfoMerchant.MerchantNO;
            //子交易描述
            productFeeList["BILL_DESC"] = "";
            productFeeList["SSPT1"] = "";
            productFeeList["SSPT2"] = "";
            detailList.Add(productFeeList);
            #endregion

            #region 4.税费
            Dictionary<string, string> taxFeeList = new Dictionary<string, string>();
            //子交易种类
            taxFeeList["BILL_TYPE"] = "30";
            //订单时间
            taxFeeList["BILL_DATE"] = context.SOInfo.OrderDate.ToString("yyyy-MM-ddTHH:mm:ss");
            //支付金额
            taxFeeList["PAY_AMOUNT"] = totalTariffAmount.ToString("F2");
            //支付币种
            taxFeeList["PAY_CURRENCY"] = context.PaymentInfoMerchant.PayCurrencyCode;
            //收款币种 *海关收款传人民币*
            taxFeeList["CRT_CURRENCY"] = "CNY";//context.PaymentInfoMerchant.CurCode;
            //是否跨境结算(外汇结算)标志
            taxFeeList["BORDER_MARK"] = "00";//context.PaymentInfoMerchant.CurCode.Equals("CNY") ? "00" : "01";
            //收款方代码类型
            taxFeeList["CRT_CODE_TYPE"] = "20";
            //收款方代码
            taxFeeList["CRT_CODE"] = context.SOInfo.CustomsCode;
            //子交易描述
            taxFeeList["BILL_DESC"] = "";
            taxFeeList["SSPT1"] = "";
            taxFeeList["SSPT2"] = "";
            detailList.Add(taxFeeList);
            #endregion

            #region 5.运费
            Dictionary<string, string> shippingFeeList = new Dictionary<string, string>();
            //子交易种类
            shippingFeeList["BILL_TYPE"] = "20";
            //订单时间
            shippingFeeList["BILL_DATE"] = context.SOInfo.OrderDate.ToString("yyyy-MM-ddTHH:mm:ss");
            //支付金额
            shippingFeeList["PAY_AMOUNT"] = shippingAmt.ToString("F2");
            //支付币种
            shippingFeeList["PAY_CURRENCY"] = context.PaymentInfoMerchant.PayCurrencyCode;
            //收款币种
            shippingFeeList["CRT_CURRENCY"] = context.PaymentInfoMerchant.CurCode;
            //是否跨境结算(外汇结算)标志
            shippingFeeList["BORDER_MARK"] = context.PaymentInfoMerchant.CurCode.Equals("CNY") ? "00" : "01";
            //收款方代码类型
            shippingFeeList["CRT_CODE_TYPE"] = "10";
            //收款方代码
            shippingFeeList["CRT_CODE"] = context.PaymentInfoMerchant.MerchantNO;
            //子交易描述
            shippingFeeList["BILL_DESC"] = "";
            shippingFeeList["SSPT1"] = "";
            shippingFeeList["SSPT2"] = "";
            detailList.Add(shippingFeeList);
            #endregion

            StringBuilder reqXml = new StringBuilder();
            reqXml.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?><EasipayB2CRequest><CnyPayRequest>");
            foreach (KeyValuePair<string, string> item in baseList)
            {
                reqXml.AppendFormat("<{0}><![CDATA[{1}]]></{0}>", item.Key, item.Value);
            }
            foreach (Dictionary<string, string> itemList in detailList)
            {
                reqXml.Append("<PayDetail>");
                foreach (KeyValuePair<string, string> item in itemList)
                {
                    reqXml.AppendFormat("<{0}><![CDATA[{1}]]></{0}>", item.Key, item.Value);
                }
                reqXml.Append("</PayDetail>");
            }
            reqXml.Append("</CnyPayRequest></EasipayB2CRequest>");
            string trxcontent = Base64Encode(reqXml.ToString());
            context.RequestForm["SENDER_CODE"] = context.PaymentInfoMerchant.MerchantNO;
            context.RequestForm["TRX_CONTENT"] = trxcontent;
            context.RequestForm["SIGNATURE"] = SignData(context);

            //Debug模式下记录相关信息至日志
            if (context.PaymentInfo.PaymentMode.Debug.Equals("1"))
                Nesoft.Utility.Logger.WriteLog(string.Format("请求xml：{0}", reqXml.ToString())
                    , "EasiPay", "PayXMLData");
        }

        public override bool VerifySign(CallbackContext context)
        {
            string localSign = SignData(context);
            if (context.ResponseForm.AllKeys.Contains("SIGNATURE")
                && context.ResponseForm["SIGNATURE"].ToLower().Trim().Equals(localSign.ToLower().Trim()))
            {
                return true;
            }

            Nesoft.Utility.Logger.WriteLog(string.Format("验签失败，东方支付签名为：{0}，本地签名：{1}！", context.ResponseForm["SignMsg"], localSign), "EasiPay", "VerifySign");
            return false;
        }

        public override bool GetPayResult(CallbackContext context)
        {
            return GetResXml(context).SelectSingleNode("EasipayB2CResponse/ResData/RTN_CODE").InnerText.Equals("000000");
        }

        public override int GetSOSysNo(CallbackContext context)
        {
            return int.Parse(GetResXml(context).SelectSingleNode("EasipayB2CResponse/ResData/BILL_NO").InnerText);
        }

        public override decimal GetPayAmount(CallbackContext context)
        {
            return decimal.Parse(GetResXml(context).SelectSingleNode("EasipayB2CResponse/ResData/PAY_AMOUNT").InnerText);
        }

        public override string GetSerialNumber(CallbackContext context)
        {
            return string.Format("P{0}", GetResXml(context).SelectSingleNode("EasipayB2CResponse/ResData/TRX_SERNO").InnerText);
        }

        public override string GetPayProcessTime(CallbackContext context)
        {
            return GetResXml(context).SelectSingleNode("EasipayB2CResponse/ResData/RDO_TIME").InnerText;
        }

        public override void UpdateChargePayment(ChargeContext context)
        {
            UpdatePayment(context.SOInfo.SoSysNo, context.PaymentInfo.PaymentMode.MerchantList);
        }

        public override void UpdateCallbackPayment(CallbackContext context)
        {
            UpdatePayment(context.SOSysNo, context.PaymentInfo.PaymentMode.MerchantList);
        }

        private  void UpdatePayment(int soSysNo,  List<PaymentModeMerchant> paymentModeMerchants)
        {
            var customsInfo = ShoppingOrderDA.LoadVendorCustomsInfo(soSysNo);
            PaymentModeMerchant payInfo = paymentModeMerchants.FirstOrDefault(p => p.MerchantSysNo == customsInfo.MerchantSysNo);
            if (payInfo == null)
            {
                payInfo = new PaymentModeMerchant();
                payInfo.MerchantSysNo = customsInfo.MerchantSysNo;
                payInfo.Encoding = "utf-8";
                paymentModeMerchants.Add(payInfo);
            }
            payInfo.MerchantNO = customsInfo.CBTMerchantCode;
            payInfo.PayCurrencyCode = customsInfo.PayCurrencyCode;
            payInfo.MerchantCertKey = customsInfo.EasiPaySecretKey;
            payInfo.CBTREC_NCode = customsInfo.CBTREC_NCode;
            payInfo.CBTSRC_NCode = customsInfo.CBTSRC_NCode;
            payInfo.CurCode = customsInfo.ReceiveCurrencyCode;
            payInfo.MerchantName = customsInfo.CBTMerchantName;
        }

        /// <summary>
        /// 支付签名
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string SignData(ChargeContext context)
        {
            string sourceSignValue = "{0}^{2}^{3}^{1}";
            string prefixCertKey = context.PaymentInfoMerchant.MerchantCertKey.Substring(0, 64);
            string suffixCertKey = context.PaymentInfoMerchant.MerchantCertKey.Substring(64, 64);
            sourceSignValue = string.Format(sourceSignValue, prefixCertKey, suffixCertKey, context.RequestForm["SENDER_CODE"], context.RequestForm["TRX_CONTENT"]);
            string signValue = GetMD5(sourceSignValue, context.PaymentInfoMerchant.Encoding).ToUpper();
            //Debug模式下记录相关信息至日志
            if (context.PaymentInfo.PaymentMode.Debug.Equals("1"))
                Nesoft.Utility.Logger.WriteLog(string.Format("签名明文：{0}，签名：{1}", sourceSignValue, signValue)
                    , "EasiPay", "PaySignData");
            return signValue;
        }

        /// <summary>
        /// 回调签名
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string SignData(CallbackContext context)
        {
            string sourceSignValue = "{0}^{2}^{1}";
            string prefixCertKey = context.PaymentInfoMerchant.MerchantCertKey.Substring(0, 64);
            string suffixCertKey = context.PaymentInfoMerchant.MerchantCertKey.Substring(64, 64);
            sourceSignValue = string.Format(sourceSignValue, prefixCertKey, suffixCertKey, context.ResponseForm["TRX_CONTENT"]);
            string signValue = GetMD5(sourceSignValue, context.PaymentInfoMerchant.Encoding).ToUpper();
            //Debug模式下记录相关信息至日志
            if (context.PaymentInfo.PaymentMode.Debug.Equals("1"))
                Nesoft.Utility.Logger.WriteLog(string.Format("签名明文：{0}，签名：{1}", sourceSignValue, signValue)
                    , "EasiPay", "PaySignData");
            return signValue;
        }

        /// <summary>
        /// 获取返回的业务XML数据
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private XmlDocument GetResXml(CallbackContext context)
        {
            string trxContent = Base64Decode(context.ResponseForm["TRX_CONTENT"]);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(trxContent);
            return xmlDoc;
        }
    }
}
