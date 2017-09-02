using System;
using System.Collections.Specialized;
using System.Linq;

using ECommerce.Utility;
using ECommerce.Entity.Payment;
using ECommerce.Facade.Payment.Charge;
using ECommerce.DataAccess.Shopping;
using ECommerce.Enums;
using ECommerce.Entity.Order;
using ECommerce.Entity.Common;
using ECommerce.Entity;
using ECommerce.DataAccess.Common;
using ECommerce.Entity.Member;
using ECommerce.DataAccess.Member;
using System.Configuration;
using ECommerce.WebFramework.Mail;
using ECommerce.Entity.Shopping;
using System.Collections.Generic;
using ECommerce.DataAccess.GroupBuying;
using System.Xml;
using System.Text;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using ECommerce.Facade.Common.RestClient;
using ECommerce.Facade.Member;
using ECommerce.SOPipeline.Impl;
using ECommerce.SOPipeline;

namespace ECommerce.Facade.Payment
{
    public class PaymentService
    {
        #region [ 支付请求 ]

        /// <summary>
        /// 支付
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public string Payment(int SOSysNo)
        {
            ChargeContext context = new ChargeContext();

            context.SOInfo = ShoppingOrderDA.PayGetCenterDBOrderInfoBySOSysNo(SOSysNo);
            if (context.SOInfo == null || context.SOInfo.SOItemList == null
                || context.SOInfo.SOItemList.Count == 0)
                throw new BusinessException("订单不存在！");
            if (context.SOInfo.Status != Enums.SOStatus.Original)
                throw new BusinessException("订单不是待支付状态！");
            var netPayInfo = ShoppingOrderDA.GetCenterDBNetpayBySOSysNo(SOSysNo);
            if (netPayInfo != null && netPayInfo.Status > (int)NetPayStatusType.Origin)
                throw new BusinessException("订单已支付！");

            if (context.SOInfo.Payment != null)
            {
                context.PaymentModeId = context.SOInfo.Payment.PayTypeID;
                CustomerFacade.UpdateCustomerLastOrderPayTypeID(context.SOInfo.CustomerSysNo, context.SOInfo.Payment.PayTypeID);
            }

            Charges charge = Charges.GetInstance(context);
            if (charge != null)
                charge.UpdateChargePayment(context);

            return charge != null ? charge.GetRequestContent(context) : string.Empty;
        }


        #endregion

        #region [ 支付回调 ]

        /// <summary>
        /// 支付后台回调
        /// </summary>
        /// <param name="payTypeSysNo">支付方式系统编号</param>
        /// <param name="collection">POST数据集合</param>
        /// <returns></returns>
        public string PaymentCallback(int payTypeSysNo, NameValueCollection collection, out CallbackContext context)
        {
            context = new CallbackContext();
            try
            {
                #region [ 写系统Log ]

                ApplicationEventLog log = new ApplicationEventLog()
                {
                    Source = "B2C site pay",
                    EventType = 8,
                    HostName = "B2C",
                    EventTitle = "Pay callback",
                    EventMessage = Charges.BuildStringFromNameValueCollection(collection),
                    LanguageCode = ConstValue.LanguageCode,
                    CompanyCode = ConstValue.CompanyCode,
                    StoreCompanyCode = ConstValue.StoreCompanyCode
                };
                CommonDA.CreateApplicationEventLog(log);

                #endregion

                context.PaymentModeId = payTypeSysNo;
                context.ResponseForm = collection;

                Charges charge = Charges.GetInstance(context);
                context.SOSysNo = charge.GetSOSysNo(context);
                context.SOInfo = ShoppingOrderDA.PayGetOrderInfoBySOSysNo(context.SOSysNo);
                charge.UpdateCallbackPayment(context);

                //如果为泰隆网关支付，写入积分消费记录
                //if (payTypeSysNo == 201)
                //{
                //    CreateUserBankPointRecord(context);
                //}


                if (charge.VerifySign(context))
                {
                    #region [ 检查返回的订单实际支付金额与订单需要支付的金额是否相等，否则不予处理 ]

                    if (context.SOInfo == null)
                    {
                        return BuildPaymentCallbackReturnResult(payTypeSysNo, false);
                    }
                    if (!charge.GetPayAmount(context).ToString("F2").Equals(context.SOInfo.RealPayAmt.ToString("F2")))
                    {
                        ECommerce.Utility.Logger.WriteLog("订单实际支付金额与订单需要支付的金额不相等！", "PayCallback", "CheckPayAmount");
                        return BuildPaymentCallbackReturnResult(payTypeSysNo, false);
                    }

                    #endregion

                    #region [ 检查NetPay是否存在并且状态为>=0，是则已支付过 ]

                    NetpayInfo netPayInfo = ShoppingOrderDA.GetCenterDBNetpayBySOSysNo(context.SOSysNo);
                    if (netPayInfo != null && netPayInfo.Status > (int)NetPayStatusType.Origin)
                    {
                        ECommerce.Utility.Logger.WriteLog("订单已经支付！", "PayCallback", "CheckNetPay");
                        return BuildPaymentCallbackReturnResult(payTypeSysNo, true);
                    }
                    #endregion

                    if (charge.GetPayResult(context))
                    {
                        //支付成功
                        using (ITransaction scope = TransactionManager.Create())
                        {
                            #region 1.写Netpay
                            netPayInfo = new NetpayInfo()
                            {
                                SOSysNo = context.SOSysNo,
                                PayTypeSysNo = payTypeSysNo,
                                PayAmount = context.SOInfo.RealPayAmt,
                                Source = 0,
                                Status = NetPayStatusType.Verified,
                                CompanyCode = context.SOInfo.CompanyCode,
                                LanguageCode = context.SOInfo.LanguageCode,
                                CurrencySysNo = context.SOInfo.CurrencySysNo,
                                StoreCompanyCode = context.SOInfo.StoreCompanyCode,
                                OrderAmt = (context.SOInfo.SoAmt
                                - Math.Abs((context.SOInfo.Amount.PointPay * 1.00m) / decimal.Parse(ConstValue.PointExhangeRate))
                                - Math.Abs(context.SOInfo.PromotionAmt)
                                + Math.Abs(context.SOInfo.Amount.PayPrice)
                                + Math.Abs(context.SOInfo.Amount.ShipPrice)
                                + Math.Abs(context.SOInfo.Amount.PremiumAmt)
                                - Math.Abs(context.SOInfo.Amount.DiscountAmt)
                                + Math.Abs(context.SOInfo.TariffAmt)
                                ),
                                PrePayAmt = context.SOInfo.Amount.PrepayAmt,
                                PointPayAmt = context.SOInfo.PointPay * 1.00m / decimal.Parse(ConstValue.PointExhangeRate),
                                GiftCardPayAmt = context.SOInfo.Amount.GiftCardPay,
                                SerialNumber = charge.GetSerialNumber(context),
                                PayProcessTime = charge.GetPayProcessTime(context)
                            };
                            bool isTrue = ShoppingOrderDA.CreateNetpay(netPayInfo);
                            #endregion

                            if (!isTrue)
                            {
                                //解决前后台多次并发处理重复数据
                                ECommerce.Utility.Logger.WriteLog("订单已经支付！", "PayCallback", "CreateNetPay");
                                return BuildPaymentCallbackReturnResult(payTypeSysNo, false);
                            }
                            if (ConstValue.PaymentInventory)
                            {
                                List<OrderItem> subOrderItemList = new List<OrderItem>();
                                context.SOInfo.SOItemList.ForEach(Item =>
                                {
                                    OrderProductItem opitem = new OrderProductItem();
                                    opitem["Quantity"] = Item.Quantity;
                                    opitem.ProductSysNo = int.Parse(Item.ProductID);
                                    opitem.ProductName = Item.ProductName;
                                    opitem.WarehouseNumber = int.Parse(Item.WarehouseNumber);
                                    subOrderItemList.Add(opitem);
                                });
                                subOrderItemList.ForEach(Item =>
                                {
                                    //限时促销
                                    if (PipelineDA.CheckCountDownByProductSysNo(Item.ProductSysNo))
                                    {
                                        //扣减订单商品库存
                                        int inventoryType = PipelineDA.GetInventoryType(Item);
                                        var rowCount = PipelineDA.UpdateInventory(Item);
                                        if (rowCount != 1 && inventoryType != 1 && inventoryType != 3 && inventoryType != 5)
                                        {
                                            ECommerce.Utility.Logger.WriteLog("inventory: qty is not enough", "SOPipeline.CreateSOBasicPersister");
                                            throw new BusinessException(string.Format("商品【{0}】库存不足", Item.ProductName));
                                        }
                                        rowCount = PipelineDA.UpdateInventoryStock(Item);
                                        if (rowCount != 1 && inventoryType != 1 && inventoryType != 3 && inventoryType != 5)
                                        {
                                            ECommerce.Utility.Logger.WriteLog("inventory_stock: qty is not enough", "SOPipeline.CreateSOBasicPersister");
                                            throw new BusinessException(string.Format("商品【{0}】库存不足", Item.ProductName));
                                        }
                                    }
                                });
                            }


                            #region 2.发送支付成功邮件
                            //确认不需要发邮件
                            //CustomerInfo customer = CustomerDA.GetCustomerInfo(context.SOInfo.CustomerSysNo);
                            //if (!string.IsNullOrWhiteSpace(customer.Email))
                            //{
                            //    AsyncEmail email = new AsyncEmail();
                            //    email.MailAddress = customer.Email;
                            //    email.CustomerID = customer.CustomerID;
                            //    email.Status = (int)EmailStatus.NotSend;
                            //    string token = Guid.NewGuid().ToString("N");
                            //    email.ImgBaseUrl = ConfigurationManager.AppSettings["CDNWebDomain"].ToString();
                            //    email.SetNewTokenUrl = "/EmailVerifySucceed?token=" + token + "&sysno=" + customer.SysNo.ToString() + "&email=" + System.Web.HttpUtility.HtmlEncode(customer.Email);

                            //    string subject = string.Empty;
                            //    email.MailBody = MailHelper.GetMailTemplateBody("SalesOrderPaySuccessful", out subject);
                            //    email.MailSubject = subject.Replace("[SOSysNo]", context.SOInfo.SoSysNo.ToString());
                            //    //订单内容
                            //    email.MailBody = email.MailBody.Replace("[CustomerName]", customer.CustomerName);
                            //    email.MailBody = email.MailBody.Replace("[SOSysNo]", context.SOInfo.SoSysNo.ToString());
                            //    email.MailBody = email.MailBody.Replace("[PayAmount]", context.SOInfo.RealPayAmt.ToString("F2"));
                            //    email.MailBody = email.MailBody.Replace("[PayTypeName]", context.SOInfo.Payment.PayTypeName);
                            //    email.MailBody = email.MailBody.Replace("[OrderDate]", context.SOInfo.OrderDate.ToString());
                            //    email.MailBody = email.MailBody.Replace("[ReceiveName]", context.SOInfo.ReceiveName);
                            //    email.MailBody = email.MailBody.Replace("[SOMemo]", context.SOInfo.Memo);
                            //    email.MailBody = email.MailBody.Replace("[NowDate]", DateTime.Now.ToShortDateString());
                            //    EmailDA.SendEmail(email);
                            //}
                            #endregion

                            scope.Complete();
                        }

                        return BuildPaymentCallbackReturnResult(payTypeSysNo, true);
                    }
                    else
                    {
                        ECommerce.Utility.Logger.WriteLog("返回支付失败", "PayCallback", "CheckPayResult");
                        return BuildPaymentCallbackReturnResult(payTypeSysNo, false);
                    }
                }
                else
                {
                    //验签失败
                    return BuildPaymentCallbackReturnResult(payTypeSysNo, false);
                }
            }
            catch (Exception ex)
            {
                //系统异常，写日志
                ECommerce.Utility.Logger.WriteLog(string.Format("系统异常，异常信息：{0}！", ex.ToString()), "PayCallback", "OnlinePayBgCallbackFailure");
                return BuildPaymentCallbackReturnResult(payTypeSysNo, false);
            }
        }

        /// <summary>
        /// 创建泰隆银行网银积分消费记录
        /// </summary>
        /// <param name="context"></param>
        private static void CreateUserBankPointRecord(CallbackContext context)
        {
            try
            {
                TLPoint point = new TLPoint();
                point.Point = 0;
                point.SoSysNO = context.SOSysNo;
                point.InUser = "WebSite";
                bool result = PaymentDA.CreateUseBankPointRecord(point);
                Logger.WriteLog("写入积分消费记录结果:" + result, "PaymentCallback", "CreateUseBankPointRecord");
            }
            catch (Exception ex)
            {
                Logger.WriteLog("写入积分消费记录异常:" + ex.Message, "PaymentCallback", "CreateUseBankPointRecord");
            }
        }

        /// <summary>
        /// 支付返回信息 Check
        /// Called before [show pay callback].
        /// </summary>
        /// <param name="payTypeSysNo">The pay type system no.</param>
        /// <param name="collection">The collection.</param>
        /// <exception cref="ECommerce.Utility.BusinessException">
        /// 订单不存在！
        /// or
        /// 订单实际支付金额与订单需要支付的金额不相等！
        /// or
        /// 支付请求签名验证失败！
        /// </exception>
        public void OnlineShowPayCallbackPreCheck(int payTypeSysNo, NameValueCollection collection)
        {
            var context = new CallbackContext();
            try
            {
                context.PaymentModeId = payTypeSysNo;
                context.ResponseForm = collection;

                Charges charge = Charges.GetInstance(context);
                context.SOSysNo = charge.GetSOSysNo(context);
                context.SOInfo = ShoppingOrderDA.PayGetOrderInfoBySOSysNo(context.SOSysNo);

                if (charge.VerifySign(context))
                {
                    //验签成功
                    #region 检查返回的订单实际支付金额与订单需要支付的金额是否相等
                    if (context.SOInfo == null)
                    {
                        throw new BusinessException("订单不存在！");
                    }
                    if (!charge.GetPayAmount(context).ToString("F2").Equals(context.SOInfo.RealPayAmt.ToString("F2")))
                    {
                        throw new BusinessException("订单实际支付金额与订单需要支付的金额不相等！");
                    }
                    #endregion

                    if (!charge.GetPayResult(context))
                    {
                        throw new BusinessException("支付失败，請完成支付！");
                    }
                }
                else
                {
                    //验签失败
                    throw new BusinessException("支付请求签名验证失败！");
                }
            }
            catch (Exception ex)
            {
                ECommerce.Utility.Logger.WriteLog(string.Format("系统异常，异常信息：{0}！", ex.ToString()), "OnlineShowPayCallbackPreCheck", "OnlineShowPayCallbackPreCheckFailure");

                if (ex is BusinessException)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 构造在线支付回调返回结果
        /// </summary>
        /// <param name="payTypeSysNo">支付方式系统编号</param>
        /// <param name="bResult">回调处理结果</param>
        /// <returns></returns>
        private string BuildPaymentCallbackReturnResult(int payTypeSysNo, bool bResult)
        {
            string result = "";

            //switch (payTypeSysNo)
            //{
            //    case 111:
            //    case 112:
            //    case 113:
            //    case 114:
            //    case 115:
            result = bResult ? "SUCCESS" : "FAILURE";
            //        break;
            //}

            return result;
        }

        #endregion

        #region [ 支付回调扩展 ]

        #region [支付宝支付]

        /// <summary>
        /// 支付宝后台回调
        /// </summary>
        /// <param name="payTypeSysNo"></param>
        /// <param name="collection"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public string AliPayCallback(int payTypeSysNo, NameValueCollection collection, out CallbackContext context)
        {
            context = new CallbackContext();
            try
            {
                #region [ 写系统Log ]

                ApplicationEventLog log = new ApplicationEventLog()
                {
                    Source = "B2C site pay",
                    EventType = 8,
                    HostName = "B2C",
                    EventTitle = "Pay callback",
                    EventMessage = Charges.BuildStringFromNameValueCollection(collection),
                    LanguageCode = ConstValue.LanguageCode,
                    CompanyCode = ConstValue.CompanyCode,
                    StoreCompanyCode = ConstValue.StoreCompanyCode
                };
                CommonDA.CreateApplicationEventLog(log);

                #endregion

                context.PaymentModeId = payTypeSysNo;
                context.ResponseForm = collection;

                Charges charge = Charges.GetInstance(context);
                context.SOSysNo = charge.GetSOSysNo(context);
                context.SOInfo = ShoppingOrderDA.PayGetOrderInfoBySOSysNo(context.SOSysNo);
                charge.UpdateCallbackPayment(context);


                #region [ 检查返回的订单实际支付金额与订单需要支付的金额是否相等，否则不予处理 ]

                if (context.SOInfo == null)
                {
                    return BuildPaymentCallbackReturnResult(payTypeSysNo, false);
                }
                if (!charge.GetPayAmount(context).ToString("F2").Equals(context.SOInfo.RealPayAmt.ToString("F2")))
                {
                    ECommerce.Utility.Logger.WriteLog("订单实际支付金额与订单需要支付的金额不相等！", "PayCallback", "CheckPayAmount");
                    return BuildPaymentCallbackReturnResult(payTypeSysNo, false);
                }

                #endregion

                #region [ 检查NetPay是否存在并且状态为>=0，是则已支付过 ]

                NetpayInfo netPayInfo = ShoppingOrderDA.GetCenterDBNetpayBySOSysNo(context.SOSysNo);
                if (netPayInfo != null && netPayInfo.Status > (int)NetPayStatusType.Origin)
                {
                    ECommerce.Utility.Logger.WriteLog("订单已经支付！", "PayCallback", "CheckNetPay");
                    return BuildPaymentCallbackReturnResult(payTypeSysNo, true);
                }
                #endregion
                ECommerce.Utility.Logger.WriteLog(context.ResponseForm["trade_status"], "AliPay", "FrontPay");
                if (charge.GetPayResult(context))
                {
                    //支付成功
                    using (ITransaction scope = TransactionManager.Create())
                    {
                        #region 1.写Netpay
                        netPayInfo = new NetpayInfo()
                        {
                            SOSysNo = context.SOSysNo,
                            PayTypeSysNo = payTypeSysNo,
                            PayAmount = context.SOInfo.RealPayAmt,
                            Source = 0,
                            Status = NetPayStatusType.Verified,
                            CompanyCode = context.SOInfo.CompanyCode,
                            LanguageCode = context.SOInfo.LanguageCode,
                            CurrencySysNo = context.SOInfo.CurrencySysNo,
                            StoreCompanyCode = context.SOInfo.StoreCompanyCode,
                            OrderAmt = (context.SOInfo.SoAmt
                            - Math.Abs((context.SOInfo.Amount.PointPay * 1.00m) / decimal.Parse(ConstValue.PointExhangeRate))
                            - Math.Abs(context.SOInfo.PromotionAmt)
                            + Math.Abs(context.SOInfo.Amount.PayPrice)
                            + Math.Abs(context.SOInfo.Amount.ShipPrice)
                            + Math.Abs(context.SOInfo.Amount.PremiumAmt)
                            - Math.Abs(context.SOInfo.Amount.DiscountAmt)
                            + Math.Abs(context.SOInfo.TariffAmt)
                            ),
                            PrePayAmt = context.SOInfo.Amount.PrepayAmt,
                            PointPayAmt = context.SOInfo.PointPay * 1.00m / decimal.Parse(ConstValue.PointExhangeRate),
                            GiftCardPayAmt = context.SOInfo.Amount.GiftCardPay,
                            SerialNumber = charge.GetSerialNumber(context),
                            PayProcessTime = charge.GetPayProcessTime(context)
                        };
                        bool isTrue = ShoppingOrderDA.CreateNetpay(netPayInfo);
                        ECommerce.Utility.Logger.WriteLog("成功", "AliPay", "FrontPay");
                        #endregion

                        if (!isTrue)
                        {
                            //解决前后台多次并发处理重复数据
                            ECommerce.Utility.Logger.WriteLog("订单已经支付！", "PayCallback", "CreateNetPay");
                            return BuildPaymentCallbackReturnResult(payTypeSysNo, false);
                        }
                        #region 促销活动里的商品是否支持付款后扣减在线库存【支持：true或不支持：false】
                        if (ConstValue.PaymentInventory)
                        {
                            List<OrderItem> subOrderItemList = new List<OrderItem>();
                            context.SOInfo.SOItemList.ForEach(Item =>
                            {
                                OrderProductItem opitem = new OrderProductItem();
                                opitem["Quantity"] = Item.Quantity;
                                opitem.ProductSysNo = int.Parse(Item.ProductID);
                                opitem.ProductName = Item.ProductName;
                                opitem.WarehouseNumber = int.Parse(Item.WarehouseNumber);
                                subOrderItemList.Add(opitem);
                            });
                            subOrderItemList.ForEach(Item =>
                            {
                                //限时促销
                                if (PipelineDA.CheckCountDownByProductSysNo(Item.ProductSysNo))
                                {
                                    //扣减订单商品库存
                                    int inventoryType = PipelineDA.GetInventoryType(Item);
                                    var rowCount = PipelineDA.UpdateInventory(Item);
                                    if (rowCount != 1 && inventoryType != 1 && inventoryType != 3 && inventoryType != 5)
                                    {
                                        ECommerce.Utility.Logger.WriteLog("inventory: qty is not enough", "SOPipeline.CreateSOBasicPersister");
                                        throw new BusinessException(string.Format("商品【{0}】库存不足", Item.ProductName));
                                    }
                                    rowCount = PipelineDA.UpdateInventoryStock(Item);
                                    if (rowCount != 1 && inventoryType != 1 && inventoryType != 3 && inventoryType != 5)
                                    {
                                        ECommerce.Utility.Logger.WriteLog("inventory_stock: qty is not enough", "SOPipeline.CreateSOBasicPersister");
                                        throw new BusinessException(string.Format("商品【{0}】库存不足", Item.ProductName));
                                    }
                                }
                            });
                        }
                        #endregion

                        #region 2.发送支付成功邮件
                        //确认不需要发邮件
                        //CustomerInfo customer = CustomerDA.GetCustomerInfo(context.SOInfo.CustomerSysNo);
                        //if (!string.IsNullOrWhiteSpace(customer.Email))
                        //{
                        //    AsyncEmail email = new AsyncEmail();
                        //    email.MailAddress = customer.Email;
                        //    email.CustomerID = customer.CustomerID;
                        //    email.Status = (int)EmailStatus.NotSend;
                        //    string token = Guid.NewGuid().ToString("N");
                        //    email.ImgBaseUrl = ConfigurationManager.AppSettings["CDNWebDomain"].ToString();
                        //    email.SetNewTokenUrl = "/EmailVerifySucceed?token=" + token + "&sysno=" + customer.SysNo.ToString() + "&email=" + System.Web.HttpUtility.HtmlEncode(customer.Email);

                        //    string subject = string.Empty;
                        //    email.MailBody = MailHelper.GetMailTemplateBody("SalesOrderPaySuccessful", out subject);
                        //    email.MailSubject = subject.Replace("[SOSysNo]", context.SOInfo.SoSysNo.ToString());
                        //    //订单内容
                        //    email.MailBody = email.MailBody.Replace("[CustomerName]", customer.CustomerName);
                        //    email.MailBody = email.MailBody.Replace("[SOSysNo]", context.SOInfo.SoSysNo.ToString());
                        //    email.MailBody = email.MailBody.Replace("[PayAmount]", context.SOInfo.RealPayAmt.ToString("F2"));
                        //    email.MailBody = email.MailBody.Replace("[PayTypeName]", context.SOInfo.Payment.PayTypeName);
                        //    email.MailBody = email.MailBody.Replace("[OrderDate]", context.SOInfo.OrderDate.ToString());
                        //    email.MailBody = email.MailBody.Replace("[ReceiveName]", context.SOInfo.ReceiveName);
                        //    email.MailBody = email.MailBody.Replace("[SOMemo]", context.SOInfo.Memo);
                        //    email.MailBody = email.MailBody.Replace("[NowDate]", DateTime.Now.ToShortDateString());
                        //    EmailDA.SendEmail(email);
                        //}
                        #endregion

                        scope.Complete();
                    }

                    return BuildPaymentCallbackReturnResult(payTypeSysNo, true);
                }
                else
                {
                    ECommerce.Utility.Logger.WriteLog("返回支付失败", "PayCallback", "CheckPayResult");
                    return BuildPaymentCallbackReturnResult(payTypeSysNo, false);
                }
            }
            catch (Exception ex)
            {
                //系统异常，写日志
                ECommerce.Utility.Logger.WriteLog(string.Format("系统异常，异常信息：{0}！", ex.ToString()), "PayCallback", "OnlinePayBgCallbackFailure");
                return BuildPaymentCallbackReturnResult(payTypeSysNo, false);
            }
        }

        /// <summary>
        /// 同步Check订单(1000订单不存在，2000订单不是待支付状态，3000订单已支付，4000通过Check)
        /// </summary>
        /// <param name="SOSysNo">订单系统编号</param>
        /// <returns>1000订单不存在，2000订单不是待支付状态，3000订单已支付，4000通过Check</returns>
        public int AliPayCheck(int SOSysNo)
        {
            ChargeContext context = new ChargeContext();
            //支付时获取SO详细信息，从CenterDB获取
            context.SOInfo = ShoppingOrderDA.PayGetCenterDBOrderInfoBySOSysNo(SOSysNo);
            if (context.SOInfo == null || context.SOInfo.SOItemList == null || context.SOInfo.SOItemList.Count == 0)
            {
                //throw new BusinessException("订单不存在！");
                return 1000;
            }
            //if (context.SOInfo.Status != Enums.SOStatus.Original)
            //{
            //    //throw new BusinessException("订单不是待支付状态！");
            //    return 2000;
            //}
            //根据订单号查询Netpay
            var netPayInfo = ShoppingOrderDA.GetCenterDBNetpayBySOSysNo(SOSysNo);
            if (netPayInfo != null && netPayInfo.Status > (int)NetPayStatusType.Origin)
            {
                //throw new BusinessException("订单已支付！");
                return 3000;
            }
            return 4000;
        }

        #endregion


        #region [东方支付/财付通退款后台回调 ]

        /// <summary>
        /// 东方支付退款后台回调
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public string EasiPayRefundBack(NameValueCollection collection)
        {
            try
            {
                #region 写系统Log
                ApplicationEventLog log = new ApplicationEventLog()
                {
                    Source = "B2C site refund",
                    EventType = 8,
                    HostName = "B2C",
                    EventTitle = "Refund callback",
                    EventMessage = Charges.BuildStringFromNameValueCollection(collection),
                    LanguageCode = ConstValue.LanguageCode,
                    CompanyCode = ConstValue.CompanyCode,
                    StoreCompanyCode = ConstValue.StoreCompanyCode
                };
                CommonDA.CreateApplicationEventLog(log);
                #endregion

                CallbackContext context = new CallbackContext();
                context.PaymentModeId = 111;
                context.ResponseForm = collection;

                Charges charge = Charges.GetInstance(context);
                charge.UpdateCallbackPayment(context);
                if (charge.VerifySign(context))
                {
                    string trxContent = Charges.Base64Decode(context.ResponseForm["TRX_CONTENT"]);
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(trxContent);
                    string code = xmlDoc.SelectSingleNode("EasipayB2CResponse/ResData/RTN_CODE").InnerText;
                    if (code.Equals("000000"))
                    {
                        string serno = xmlDoc.SelectSingleNode("EasipayB2CResponse/ResData/REFTRX_SERNO").InnerText;
                        string refundStatus = xmlDoc.SelectSingleNode("EasipayB2CResponse/ResData/REFUND_STATE").InnerText;
                        bool bRefundStatus = refundStatus.Equals("S") ? true : false;
                        Refund(serno, bRefundStatus);
                    }
                    return BuildPaymentCallbackReturnResult(111, true);
                }

                ECommerce.Utility.Logger.WriteLog("返回退款回调失败", "RefundCallback", "CheckRefundResult");
                return BuildPaymentCallbackReturnResult(111, false);
            }
            catch (Exception ex)
            {
                //系统异常，写日志
                ECommerce.Utility.Logger.WriteLog(string.Format("系统异常，异常信息：{0}！", ex.ToString()), "RefundCallback", "RefundBgCallbackFailure");
                return BuildPaymentCallbackReturnResult(111, false);
            }
        }

        /// <summary>
        /// 财付通退款后台回调
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public string TenPayRefundBack(NameValueCollection collection)
        {
            try
            {
                #region 写系统Log
                ApplicationEventLog log = new ApplicationEventLog()
                {
                    Source = "B2C site refund",
                    EventType = 8,
                    HostName = "B2C",
                    EventTitle = "TenPay Refund callback",
                    EventMessage = Charges.BuildStringFromNameValueCollection(collection),
                    LanguageCode = ConstValue.LanguageCode,
                    CompanyCode = ConstValue.CompanyCode,
                    StoreCompanyCode = ConstValue.StoreCompanyCode
                };
                CommonDA.CreateApplicationEventLog(log);
                #endregion

                CallbackContext context = new CallbackContext();
                context.PaymentModeId = 114;
                context.ResponseForm = collection;

                Charges charge = Charges.GetInstance(context);
                charge.UpdateCallbackPayment(context);
                if (charge.VerifySign(context))
                {
                    bool bRefundStatus = false;
                    string serno = context.ResponseForm["refund_id"];
                    string code = context.ResponseForm["refund_status"];
                    if (code.Equals("4") || code.Equals("10"))
                        bRefundStatus = true;
                    var status = bRefundStatus ? SOIncomeStatus.Confirmed : SOIncomeStatus.ProcessingFailed;
                    ShoppingOrderDA.Refund(serno, status);
                    return BuildPaymentCallbackReturnResult(114, true);
                }

                ECommerce.Utility.Logger.WriteLog("返回退款回调失败", "RefundCallback", "CheckRefundResult");
                return BuildPaymentCallbackReturnResult(114, false);
            }
            catch (Exception ex)
            {
                //系统异常，写日志
                ECommerce.Utility.Logger.WriteLog(string.Format("系统异常，异常信息：{0}！", ex.ToString()), "RefundCallback", "RefundBgCallbackFailure");
                return BuildPaymentCallbackReturnResult(111, false);
            }
        }

        /// <summary>
        /// 网关退款回调
        /// </summary>
        /// <param name="externalKey">退款流水号</param>
        /// <param name="isTrue">是否成功</param>
        /// <returns></returns>
        public void Refund(string externalKey, bool isTrue)
        {
            //更新收款单状态
            var status = isTrue ? SOIncomeStatus.Confirmed : SOIncomeStatus.ProcessingFailed;
            ShoppingOrderDA.Refund("R" + externalKey, status);
        }

        #endregion

        #region [ 东方支付通关后台回调 ]

        public string EasiPaySODeclareBack(NameValueCollection collection)
        {
            try
            {
                #region 写系统Log
                ApplicationEventLog log = new ApplicationEventLog()
                {
                    Source = "B2C site SODeclare",
                    EventType = 8,
                    HostName = "B2C",
                    EventTitle = "SODeclare callback",
                    EventMessage = Charges.BuildStringFromNameValueCollection(collection),
                    LanguageCode = ConstValue.LanguageCode,
                    CompanyCode = ConstValue.CompanyCode,
                    StoreCompanyCode = ConstValue.StoreCompanyCode
                };
                CommonDA.CreateApplicationEventLog(log);
                #endregion

                EasiPaySODeclareBackInfo backResult = SerializationUtility.JsonDeserialize<EasiPaySODeclareBackInfo>(collection["EData"]);
                int SOSysNo = int.Parse(backResult.merchantOrderId);
                int status = 0;
                var customsInfo = ShoppingOrderDA.LoadVendorCustomsInfo(SOSysNo);

                #region 验证签名
                if (!EasiPaySODeclareBackVerifySign(collection["EData"], customsInfo.CBTSODeclareSecretKey, collection["SignMsg"]))
                {
                    throw new Exception("订单申报回调，验证签名失败！" + Charges.BuildStringFromNameValueCollection(collection));
                }
                #endregion

                bool bHandleResult = true;
                var client = new Common.RestClient.RestClient(ConstValue.ECCServiceBaseUrl, ConstValue.LanguageCode);
                ECommerce.Facade.Common.RestClient.RestServiceError error;
                string serviceUrl = "";

                if (backResult.status.Equals("1"))
                {
                    //成功
                    status = 10;
                    serviceUrl = "/SOService/SO/UpdateSOStatusToReported";
                    client.Update(serviceUrl, backResult.merchantOrderId, out error);
                    if (error != null)
                    {
                        bHandleResult = false;
                        StringBuilder sb = new StringBuilder();
                        error.Faults.ForEach(e => sb.AppendLine(e.ErrorDescription));

                        if (error.Faults.All(e => e.IsBusinessException))
                        {
                            return sb.ToString();
                        }
                        ECommerce.Utility.Logger.WriteLog(sb.ToString(), "SODeclareCallback", "SODeclareBgCallbackUpdateFailure");
                    }
                    if (ShoppingOrderDA.GetOrderTradeType(int.Parse(backResult.merchantOrderId)) == TradeType.DirectMail)
                    {
                        //如果是直邮订单，直接完成通关操作
                        serviceUrl = "/SOService/SO/BatchOperationUpdateSOStatusToCustomsPass";
                        client.Update(serviceUrl, new List<int> { int.Parse(backResult.merchantOrderId) }, out error);
                        if (error != null)
                        {
                            bHandleResult = false;
                            StringBuilder sb = new StringBuilder();
                            error.Faults.ForEach(e => sb.AppendLine(e.ErrorDescription));

                            if (error.Faults.All(e => e.IsBusinessException))
                            {
                                return sb.ToString();
                            }
                            ECommerce.Utility.Logger.WriteLog(sb.ToString(), "SODeclareCallback", "SODeclareBgCallbackUpdateFailure");
                        }
                    }
                }
                else
                {
                    //失败
                    status = -10;
                    object obj = new object();
                    serviceUrl = "/SOService/SO/UpdateSOStatusToReject";
                    client.Update(serviceUrl, backResult.merchantOrderId, out obj, out error);
                    if (error != null)
                    {
                        bHandleResult = false;
                        StringBuilder sb = new StringBuilder();
                        error.Faults.ForEach(e => sb.AppendLine(e.ErrorDescription));

                        if (error.Faults.All(e => e.IsBusinessException))
                        {
                            return sb.ToString();
                        }
                        ECommerce.Utility.Logger.WriteLog(sb.ToString(), "SODeclareCallback", "SODeclareBgCallbackUpdateFailure");
                    }
                }
                if (bHandleResult)
                {
                    ShoppingOrderDA.UpdateDeclareRecordsStatus(SOSysNo, status);
                }

                return BuildPaymentCallbackReturnResult(111, bHandleResult);
            }
            catch (Exception ex)
            {
                //系统异常，写日志
                ECommerce.Utility.Logger.WriteLog(string.Format("系统异常，异常信息：{0}！", ex.ToString()), "SODeclareCallback", "SODeclareBgCallbackFailure");
                return BuildPaymentCallbackReturnResult(111, false);
            }
        }

        public bool EasiPaySODeclareBackVerifySign(string reqValue, string secretKey, string sign)
        {
            string sourceSignValue = "{0}{1}";
            sourceSignValue = string.Format(sourceSignValue, reqValue, secretKey);

            return GetMD5(sourceSignValue).ToUpper().Equals(sign);
        }

        public static string GetMD5(string s)
        {
            return GetMD5(s, "utf-8");
        }

        public static string GetMD5(string s, string inputCharset)
        {
            byte[] buffer = new MD5CryptoServiceProvider().ComputeHash(Encoding.GetEncoding(inputCharset).GetBytes(s));
            StringBuilder builder = new StringBuilder(0x20);
            for (int i = 0; i < buffer.Length; i++)
            {
                builder.Append(buffer[i].ToString("x").PadLeft(2, '0'));
            }
            return builder.ToString();
        }

        [Serializable]
        [DataContract]
        public class EasiPaySODeclareBackInfo
        {
            [DataMember]
            public string version { get; set; }
            [DataMember]
            public string commitTime { get; set; }
            [DataMember]
            public string serialNumber { get; set; }
            [DataMember]
            public string merchantOrderId { get; set; }
            [DataMember]
            public string status { get; set; }
            [DataMember]
            public string statusMsg { get; set; }
        }

        #endregion

        #region [ 东方支付商品申报回调 ]
        /// <summary>
        /// 东方支付商品申报回调
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public string EasiPayDeclareProductBack(NameValueCollection collection)
        {
            try
            {
                #region 写系统Log
                ApplicationEventLog log = new ApplicationEventLog()
                {
                    Source = "B2C site ProductDeclare",
                    EventType = 8,
                    HostName = "B2C",
                    EventTitle = "ProductDeclare callback",
                    EventMessage = Charges.BuildStringFromNameValueCollection(collection),
                    LanguageCode = ConstValue.LanguageCode,
                    CompanyCode = ConstValue.CompanyCode,
                    StoreCompanyCode = ConstValue.StoreCompanyCode
                };
                CommonDA.CreateApplicationEventLog(log);
                #endregion

                EasiPayProductDeclareBackInfo backResult = SerializationUtility.JsonDeserialize<EasiPayProductDeclareBackInfo>(collection["EData"]);
                var customsInfo = ShoppingOrderDA.LoadVendorCustomsInfoByProduct(backResult.cargoes.FirstOrDefault().cargoCode);

                #region 验证签名
                if (!EasiPaySODeclareBackVerifySign(collection["EData"], customsInfo.CBTProductDeclareSecretKey, collection["SignMsg"]))
                {
                    throw new Exception("商品申报回调，验证签名失败！" + Charges.BuildStringFromNameValueCollection(collection));
                }
                #endregion

                bool bHandleResult = true;
                var client = new Common.RestClient.RestClient(ConstValue.ECCServiceBaseUrl, ConstValue.LanguageCode);
                RestServiceError error;
                object obj = new object();
                string serviceUrl = "/SOService/SO/Job/DeclareProductCallBack";
                client.Query(serviceUrl, backResult.cargoes.ToJsonString(), out obj, out error);
                if (error != null)
                {
                    bHandleResult = false;
                    StringBuilder sb = new StringBuilder();
                    error.Faults.ForEach(e => sb.AppendLine(e.ErrorDescription));

                    if (error.Faults.All(e => e.IsBusinessException))
                    {
                        return sb.ToString();
                    }
                    ECommerce.Utility.Logger.WriteLog(sb.ToString(), "ProductDeclareCallback", "ProductDeclareCallbackUpdateFailure");
                }
                return BuildPaymentCallbackReturnResult(111, bHandleResult);
            }
            catch (Exception ex)
            {
                //系统异常，写日志
                ECommerce.Utility.Logger.WriteLog(string.Format("系统异常，异常信息：{0}！", ex.ToString()), "ProductDeclareCallback", "ProductDeclareBgCallbackFailure");
                return BuildPaymentCallbackReturnResult(111, false);
            }
        }
        /// <summary>
        /// 商品备案返回结果
        /// </summary>
        [Serializable]
        [DataContract]
        public class EasiPayProductDeclareBackInfo
        {
            public EasiPayProductDeclareBackInfo()
            {
                this.cargoes = new List<EasiPayProductDeclareBackItemInfo>();
            }
            /// <summary>
            /// 网关版本
            /// </summary>
            [DataMember]
            public string version { get; set; }
            /// <summary>
            /// 提交时间
            /// </summary>
            [DataMember]
            public string commitTime { get; set; }
            /// <summary>
            /// 流水号
            /// </summary>
            [DataMember]
            public string serialNumber { get; set; }
            /// <summary>
            /// 业务参数：Cargoes集合信息，一次最多20条商品信息
            /// </summary>
            [DataMember]
            public List<EasiPayProductDeclareBackItemInfo> cargoes { get; set; }
        }
        /// <summary>
        /// 商品备案结果详细信息
        /// </summary>
        [Serializable]
        [DataContract]
        public class EasiPayProductDeclareBackItemInfo
        {
            /// <summary>
            /// 商品编号
            /// </summary>
            [DataMember]
            public string cargoCode { get; set; }
            /// <summary>
            /// 申报备案号
            /// </summary>
            [DataMember]
            public string declaraNo { get; set; }
            /// <summary>
            /// 税则号
            /// </summary>
            [DataMember]
            public string cargoCodeTS { get; set; }
            /// <summary>
            /// 税率
            /// </summary>
            [DataMember]
            public decimal cargoRate { get; set; }
            /// <summary>
            /// 审核状态
            /// 1：成功
            /// 2：失败
            /// </summary>
            [DataMember]
            public string status { get; set; }
            /// <summary>
            /// 审核意见
            /// </summary>
            [DataMember]
            public string statusMsg { get; set; }
            [DataMember]
            public string effectStartTime { get; set; }
            [DataMember]
            public string effectEndTime { get; set; }
        }
        #endregion

        #endregion

        #region [ 其他 ]
        /// <summary>
        /// Updates the type of the order pay.
        /// </summary>
        /// <param name="soSysNo">The so system no.</param>
        /// <param name="payTypeSysNo">The pay type system no.</param>
        public static void UpdateOrderPayType(int soSysNo, int payTypeSysNo)
        {
            PaymentDA.UpdateOrderPayType(soSysNo, payTypeSysNo);
        }

        /// <summary>
        /// Gets the center database netpay by so system no.
        /// </summary>
        /// <param name="soSysNo">The so system no.</param>
        /// <returns></returns>
        public static NetpayInfo GetCenterDBNetpayBySOSysNo(int soSysNo)
        {
            return ShoppingOrderDA.GetCenterDBNetpayBySOSysNo(soSysNo);
        }
        #endregion
    }
}
