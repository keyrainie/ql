using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using ECommerce.DataAccess;
using ECommerce.DataAccess.Common;
using ECommerce.DataAccess.Member;
using ECommerce.DataAccess.Shopping;
using ECommerce.Entity;
using ECommerce.Entity.Common;
using ECommerce.Entity.Payment;
using ECommerce.Entity.Promotion;
using ECommerce.Entity.Shipping;
using ECommerce.Entity.Shopping;
using ECommerce.Enums;
using ECommerce.Facade.Product;
using ECommerce.Facade.Shipping;
using ECommerce.Facade.Shopping;
using ECommerce.SOPipeline;
using ECommerce.WebFramework.Mail;
using ECommerce.Utility;
using MemberInfo = ECommerce.Entity.Member.CustomerInfo;
using ECommerce.Entity.Product;
using ECommerce.SOPipeline.Impl;
using System.Threading;

namespace ECommerce.Facade.Shopping
{
    public static partial class ShoppingFacade
    {
        /// <summary>
        /// 构造checkout对象
        /// </summary>
        /// <param name="context">checkout数据上下文</param>
        /// <param name="shoppingCart">购物车数据</param>
        /// <param name="customerSysNo">当前登录用户编号</param>
        /// <returns>checkout对象</returns>
        public static CheckOutResult BuildCheckOut(CheckOutContext context, ShoppingCart shoppingCart, int customerSysNo)
        {
            CheckOutResult result = PreCheckAndBuild(context, shoppingCart, customerSysNo, -1, SOPipelineProcessor.BuildCheckOut);

            return result;
        }

        /// <summary>
        /// 提交checkout
        /// </summary>
        /// <param name="context">checkout数据上下文</param>
        /// <param name="shoppingCart">购物车数据</param>
        /// <param name="customerSysNo">当前登录用户编号</param>
        /// <param name="source">下单来源</param>
        /// <returns>提交checkout结果</returns>
        public static CheckOutResult SubmitCheckout(CheckOutContext context, ShoppingCart shoppingCart, int customerSysNo, SOSource orderSource)
        {
            CheckOutResult result = PreCheckAndBuild(context, shoppingCart, customerSysNo, (int)orderSource, SOPipelineProcessor.CreateSO);

            //订单创建发送邮件
            if (result.OrderProcessResult.HasSucceed)
                SalesOrderMailSuccessful(result.OrderProcessResult.ReturnData);

            return result;
        }

        public static CheckOutResult GetPayAndShipTypeList(CheckOutContext context, int customerSysNo, ShoppingCart shoppingCart)
        {

            CheckOutResult result = new CheckOutResult();

            CheckOutContext newCheckoutContenxt = new CheckOutContext();
            if (context != null)
            {
                newCheckoutContenxt = context.Clone();
            }

            MemberInfo memberInfo = CustomerDA.GetCustomerInfo(customerSysNo);

            //取得用户选择的收货地址信息
            var custShippingAddrResult = GetCustomerShippingAddressList(newCheckoutContenxt, customerSysNo);
            result.SelShippingAddress = custShippingAddrResult.SelShippingAddress;

            #region 支付类别选择
            result.PaymentCategoryList = GetAllPaymentCategoryList();
            //优先取context指定的支付类型
            //其次取配送地址指定的支付类型
            if (!result.PaymentCategoryList.Exists(x =>
            {
                if (x.Key.ToString() == newCheckoutContenxt.PaymentCategoryID
                    || x.Key == result.SelShippingAddress.PaymentCategoryID)
                {
                    result.SelPaymentCategoryID = x.Key;
                    return true;
                }
                return false;
            }))
            {
                //都没有就取第一条支付类型
                result.SelPaymentCategoryID = result.PaymentCategoryList.First().Key;
            }
            #endregion



            #region 配送方式选择
            //step1 取得配送地址支持的所有配送方式
            List<ShipTypeInfo> ShipTypeInfoList = new List<ShipTypeInfo>();
            List<ShipTypeInfo> shipTypeList = new List<ShipTypeInfo>();
            foreach (ShoppingItemGroup ShoppingItemGroup in shoppingCart.ShoppingItemGroupList)
            {
                foreach (ShoppingItem ShoppingItem in ShoppingItemGroup.ShoppingItemList)
                {
                    ProductBasicInfo basicInfo = ProductFacade.GetProductBasicInfoBySysNo(ShoppingItem.ProductSysNo);
                    List<ShipTypeInfo> ShipTypeNew = ShipTypeFacade.Checkout_GetStockShippingType(basicInfo.VendorSysno);
                    if (ShipTypeNew.Count > 0)
                    {
                        if (ShipTypeInfoList.Count <= 0)
                        {
                            ShipTypeInfoList.AddRange(ShipTypeNew);
                            shipTypeList.AddRange(ShipTypeNew);
                        }
                        else
                        {
                            shipTypeList = new List<ShipTypeInfo>();
                            for (int i = 0; i < ShipTypeInfoList.Count; i++)
                            {
                                for (int j = 0; j < ShipTypeNew.Count; j++)
                                {
                                    if (ShipTypeInfoList[i].ShipTypeName == ShipTypeNew[j].ShipTypeName)
                                    {
                                        shipTypeList.Add(ShipTypeInfoList[i]);
                                    }
                                }
                            }
                            if (shipTypeList.Count <= 0)
                            {
                                result.ErrorMessages.Add("不同商家的商品，没有相同的配送方式，请分开下单！");
                                break;
                            }
                            else
                            {
                                ShipTypeInfoList = shipTypeList;
                            }
                        }
                    }
                    else
                    {
                        string error = string.Format("商品【{0}】没有对应配送方式，暂时无法为您配送！", basicInfo.ProductName);
                        result.ErrorMessages.Add(error);
                        result.ShipTypeList = null;
                        break;
                    }
                }
                if (result.ErrorMessages.Count > 0)
                {
                    break;
                }
            }
            //var shipTypeList = ShipTypeFacade.GetSupportedShipTypeList(result.SelShippingAddress.ReceiveAreaSysNo, null);
            //step2 如果不存在支持货到付款的配送方式， 则移除掉货到付款支付类别
            if (shipTypeList.Count(x => x.IsPayWhenRecv) <= 0)
            {
                result.PaymentCategoryList = result.PaymentCategoryList.FindAll(x => x.Key == (int)PaymentCategory.OnlinePay);
                result.SelPaymentCategoryID = (int)PaymentCategory.OnlinePay;
            }
            //step3 如果选择的是货到付款，则移除掉不支持货到付款的配送方式
            if (result.SelPaymentCategoryID == (int)PaymentCategory.PayWhenRecv)
            {
                result.ShipTypeList = shipTypeList.Where(x => x.IsPayWhenRecv).ToList();
                //step4 移除掉不支持货到付款的配送方式后没有可用的配送方式时，系统自动选择在线支付
                if (result.ShipTypeList.Count <= 0)
                {
                    result.ShipTypeList = shipTypeList;
                    result.PaymentCategoryList = result.PaymentCategoryList.FindAll(x => x.Key == (int)PaymentCategory.OnlinePay);
                    result.SelPaymentCategoryID = (int)PaymentCategory.OnlinePay;
                }
            }
            else
            {
                result.ShipTypeList = shipTypeList;
            }

            //优先取context指定的配送方式
            result.SelShipType = result.ShipTypeList.Find(x => x.ShipTypeSysNo.ToString() == newCheckoutContenxt.ShipTypeID);
            //其次取配送地址指定的配送方式
            if (result.SelShipType == null && result.SelShippingAddress != null)
            {
                result.SelShipType = result.ShipTypeList.Find(x => x.ShipTypeSysNo == result.SelShippingAddress.ShipTypeSysNo);
            }
            //都没有就取第一条配送方式
            if (result.SelShipType == null && result.ShipTypeList.Count > 0)
            {
                result.SelShipType = result.ShipTypeList.First();
            }
            result.ShipTypeList = EnsureNotNullObject(result.ShipTypeList);
            result.SelShipType = EnsureNotNullObject(result.SelShipType);
            #endregion

            #region 支付方式选择
            result.PayTypeList = GetAllPayTypeList();
            if (result.SelPaymentCategoryID == (int)PaymentCategory.PayWhenRecv)
            {
                result.PayTypeList = result.PayTypeList.FindAll(x => x.IsPayWhenRecv == 1);
            }
            //优先取用户上次下单使用的支付方式
            result.SelPayType = result.PayTypeList.Find(x => x.PayTypeID == memberInfo.ExtendInfo.LastPayTypeSysNo);
            if (result.SelPayType == null && result.PayTypeList.Count > 0)
            {
                result.SelPayType = result.PayTypeList.First();
            }
            if (result.SelPayType != null && result.PayTypeList.Count > 0)
            {
                var cateId = result.SelPayType.IsPayWhenRecv == 1 ? (int)PaymentCategory.PayWhenRecv : (int)PaymentCategory.OnlinePay;
                var isPayWhenRecvValue = result.SelPaymentCategoryID == (int)PaymentCategory.PayWhenRecv ? 1 : 0;
                //如果上次下单用户使用的支付方式类型跟本次下单选择的支付类型不一致
                //则选择符合当前选择的支付类型的第一个支付方式
                if (cateId != result.SelPaymentCategoryID)
                {
                    result.SelPayType = result.PayTypeList.Where(x => x.IsPayWhenRecv == isPayWhenRecvValue).First();
                }

                if (context != null && context.PayTypeID.HasValue)
                {
                    result.SelPayType = result.PayTypeList.Where(x => x.PayTypeID == context.PayTypeID.Value).First();
                }
            }
            result.PayTypeList = EnsureNotNullObject(result.PayTypeList);
            result.SelPayType = EnsureNotNullObject(result.SelPayType);
            #endregion

            return result;
        }

        //public static CheckOutResult GetPayAndShipTypeList(CheckOutContext context, int customerSysNo)
        //{
        //    CheckOutResult result = new CheckOutResult();

        //    CheckOutContext newCheckoutContenxt = new CheckOutContext();
        //    if (context != null)
        //    {
        //        newCheckoutContenxt = context.Clone();
        //    }

        //    MemberInfo memberInfo = CustomerDA.GetCustomerInfo(customerSysNo);

        //    //取得用户选择的收货地址信息
        //    var custShippingAddrResult = GetCustomerShippingAddressList(newCheckoutContenxt, customerSysNo);
        //    result.SelShippingAddress = custShippingAddrResult.SelShippingAddress;

        //    #region 支付类别选择
        //    result.PaymentCategoryList = GetAllPaymentCategoryList();
        //    //优先取context指定的支付类型
        //    //其次取配送地址指定的支付类型
        //    if (!result.PaymentCategoryList.Exists(x =>
        //    {
        //        if (x.Key.ToString() == newCheckoutContenxt.PaymentCategoryID
        //            || x.Key == result.SelShippingAddress.PaymentCategoryID)
        //        {
        //            result.SelPaymentCategoryID = x.Key;
        //            return true;
        //        }
        //        return false;
        //    }))
        //    {
        //        //都没有就取第一条支付类型
        //        result.SelPaymentCategoryID = result.PaymentCategoryList.First().Key;
        //    }
        //    #endregion

        //    #region 配送方式选择
        //    //step1 取得配送地址支持的所有配送方式
        //    var shipTypeList = ShipTypeFacade.GetSupportedShipTypeList(result.SelShippingAddress.ReceiveAreaSysNo, null);
        //    //step2 如果不存在支持货到付款的配送方式， 则移除掉货到付款支付类别
        //    if (shipTypeList.Count(x => x.IsPayWhenRecv) <= 0)
        //    {
        //        result.PaymentCategoryList = result.PaymentCategoryList.FindAll(x => x.Key == (int)PaymentCategory.OnlinePay);
        //        result.SelPaymentCategoryID = (int)PaymentCategory.OnlinePay;
        //    }
        //    //step3 如果选择的是货到付款，则移除掉不支持货到付款的配送方式
        //    if (result.SelPaymentCategoryID == (int)PaymentCategory.PayWhenRecv)
        //    {
        //        result.ShipTypeList = shipTypeList.Where(x => x.IsPayWhenRecv).ToList();
        //        //step4 移除掉不支持货到付款的配送方式后没有可用的配送方式时，系统自动选择在线支付
        //        if (result.ShipTypeList.Count <= 0)
        //        {
        //            result.ShipTypeList = shipTypeList;
        //            result.PaymentCategoryList= result.PaymentCategoryList.FindAll(x =>x.Key==(int)PaymentCategory.OnlinePay);
        //            result.SelPaymentCategoryID=(int)PaymentCategory.OnlinePay;
        //        }
        //    }
        //    else
        //    {
        //        result.ShipTypeList = shipTypeList;
        //    }

        //    //优先取context指定的配送方式
        //    result.SelShipType = result.ShipTypeList.Find(x => x.ShipTypeSysNo.ToString() == newCheckoutContenxt.ShipTypeID);
        //    //其次取配送地址指定的配送方式
        //    if (result.SelShipType == null && result.SelShippingAddress != null)
        //    {
        //        result.SelShipType = result.ShipTypeList.Find(x => x.ShipTypeSysNo == result.SelShippingAddress.ShipTypeSysNo);
        //    }
        //    //都没有就取第一条配送方式
        //    if (result.SelShipType == null && result.ShipTypeList.Count > 0)
        //    {
        //        result.SelShipType = result.ShipTypeList.First();
        //    }
        //    result.ShipTypeList = EnsureNotNullObject(result.ShipTypeList);
        //    result.SelShipType = EnsureNotNullObject(result.SelShipType);
        //    #endregion

        //    #region 支付方式选择
        //    result.PayTypeList = GetAllPayTypeList();
        //    if (result.SelPaymentCategoryID == (int)PaymentCategory.PayWhenRecv)
        //    {
        //        result.PayTypeList = result.PayTypeList.FindAll(x => x.IsPayWhenRecv == 1);
        //    }
        //    //优先取用户上次下单使用的支付方式
        //    result.SelPayType = result.PayTypeList.Find(x => x.PayTypeID == memberInfo.ExtendInfo.LastPayTypeSysNo);
        //    if (result.SelPayType == null && result.PayTypeList.Count > 0)
        //    {
        //        result.SelPayType = result.PayTypeList.First();
        //    }
        //    if (result.SelPayType != null && result.PayTypeList.Count > 0)
        //    {
        //        var cateId = result.SelPayType.IsPayWhenRecv == 1 ? (int)PaymentCategory.PayWhenRecv : (int)PaymentCategory.OnlinePay;
        //        var isPayWhenRecvValue = result.SelPaymentCategoryID == (int)PaymentCategory.PayWhenRecv ? 1 : 0;
        //        //如果上次下单用户使用的支付方式类型跟本次下单选择的支付类型不一致
        //        //则选择符合当前选择的支付类型的第一个支付方式
        //        if (cateId != result.SelPaymentCategoryID)
        //        {
        //            result.SelPayType = result.PayTypeList.Where(x => x.IsPayWhenRecv == isPayWhenRecvValue).First();
        //        }
        //    }
        //    result.PayTypeList = EnsureNotNullObject(result.PayTypeList);
        //    result.SelPayType = EnsureNotNullObject(result.SelPayType);
        //    #endregion

        //    return result;
        //}

        public static CheckOutResult GetCustomerShippingAddressList(CheckOutContext context, int customerSysNo)
        {
            CheckOutResult result = new CheckOutResult();
            ShippingContactInfo curShippingAddress = null;
            List<ShippingContactInfo> customerShippingAddressList = CustomerShippingAddresssFacade.GetCustomerShippingAddressList(customerSysNo);
            if (customerShippingAddressList != null && customerShippingAddressList.Count > 0)
            {
                if (context == null)
                {
                    curShippingAddress = customerShippingAddressList.Find(x => x.IsDefault);
                }
                else
                {
                    curShippingAddress = customerShippingAddressList.Find(x => x.SysNo == context.ShippingAddressID);
                }
                if (curShippingAddress == null)
                {
                    curShippingAddress = customerShippingAddressList.First();
                }
            }
            result.ShippingAddressList = EnsureNotNullObject(customerShippingAddressList);
            result.SelShippingAddress = EnsureNotNullObject(curShippingAddress);

            return result;
        }

        private static CheckOutResult PreCheckAndBuild(CheckOutContext context, ShoppingCart shoppingCart, int customerSysNo, int orderSource
                                                    , Func<OrderInfo, OrderPipelineProcessResult> action)
        {
            CheckOutResult result = new CheckOutResult();

            MemberInfo memberInfo = CustomerDA.GetCustomerInfo(customerSysNo);

            CheckOutContext newCheckoutContenxt = new CheckOutContext();
            if (context != null)
            {
                newCheckoutContenxt = context.Clone();
            }

            CustomerInfo customerInfo = new CustomerInfo()
            {
                AccountBalance = memberInfo.ValidPrepayAmt,
                AccountPoint = memberInfo.ValidScore,
                CustomerRank = (int)memberInfo.CustomerRank,
                ID = memberInfo.CustomerID,
                SysNo = memberInfo.SysNo,
                Name = memberInfo.CustomerName,
                IsEmailConfirmed = memberInfo.IsEmailConfirmed,
                IsPhoneValided = memberInfo.IsPhoneValided,
                CellPhone = memberInfo.CellPhone,
            };
            result.Customer = customerInfo;
            //用户个人实名认证信息
            result.CustomerAuthenticationInfo = CustomerDA.GetCustomerAuthenticationInfo(customerSysNo);

            //用户购物发票信息
            result.CustomerInvoiceInfo = CustomerDA.GetCustomerInvoiceInfo(customerSysNo);
            if (result.CustomerInvoiceInfo == null)
            {
                result.CustomerInvoiceInfo = new Entity.Member.CustomerInvoiceInfo() { CustomerSysNo = customerSysNo, InvoiceTitle = customerInfo.Name };
            }

            //收货地址
            var custShippingAddressListResult = GetCustomerShippingAddressList(context, customerSysNo);
            result.ShippingAddressList = custShippingAddressListResult.ShippingAddressList;
            result.SelShippingAddress = custShippingAddressListResult.SelShippingAddress;

            //支付方式&配送方式
            var payAndShipTypeResult = GetPayAndShipTypeList(context, customerSysNo, shoppingCart);
            result.PaymentCategoryList = payAndShipTypeResult.PaymentCategoryList;
            result.SelPaymentCategoryID = payAndShipTypeResult.SelPaymentCategoryID;
            result.PayTypeList = payAndShipTypeResult.PayTypeList;
            result.SelPayType = payAndShipTypeResult.SelPayType;
            result.ShipTypeList = payAndShipTypeResult.ShipTypeList;
            result.SelShipType = payAndShipTypeResult.SelShipType;

            //根据CheckOutContext 进一步构造shoppingCartResult.ReturnData对象
            OrderInfo preOrderInfo = SOPipelineProcessor.Convert2OrderInfo(shoppingCart);
            preOrderInfo.Customer = customerInfo;
            preOrderInfo.PayTypeID = result.SelPayType.PayTypeID.ToString();
            preOrderInfo.ShipTypeID = result.SelShipType.ShipTypeSysNo.ToString();
            preOrderInfo.Memo = newCheckoutContenxt.OrderMemo;
            preOrderInfo.CouponCode = newCheckoutContenxt.PromotionCode;
            preOrderInfo.ChannelID = shoppingCart.ChannelID;
            preOrderInfo.LanguageCode = shoppingCart.LanguageCode;
            preOrderInfo.OrderSource = orderSource;
            preOrderInfo.VirualGroupBuyOrderTel = context != null ? context.VirualGroupBuyOrderTel : "";

            preOrderInfo.Contact = new ContactInfo()
            {
                AddressAreaID = result.SelShippingAddress.ReceiveAreaSysNo,
                //AddressAreaID = result.SelShippingAddress.ReceiveAreaCitySysNo,
                AddressTitle = result.SelShippingAddress.AddressTitle,
                AddressDetail = result.SelShippingAddress.ReceiveAddress,
                MobilePhone = result.SelShippingAddress.ReceiveCellPhone,
                Phone = result.SelShippingAddress.ReceivePhone,
                Name = result.SelShippingAddress.ReceiveName,
                ZipCode = result.SelShippingAddress.ReceiveZip,
                ID = result.SelShippingAddress.SysNo,
            };
            //使用余额进行支付，给订单的余额支付金额赋值，在SOPipline中会对订单的余额支付金额重新进行计算
            if (newCheckoutContenxt.IsUsedPrePay > 0)
            {
                preOrderInfo.BalancePayAmount = customerInfo.AccountBalance;
            }
            //积分
            preOrderInfo.PointPay = newCheckoutContenxt.PointPay;
            //礼品卡
            if (newCheckoutContenxt.GiftCardList != null && newCheckoutContenxt.GiftCardList.Count > 0)
            {
                preOrderInfo.GiftCardList = new List<GiftCardInfo>();
                foreach (var giftCardContext in newCheckoutContenxt.GiftCardList)
                {
                    if (!string.IsNullOrWhiteSpace(giftCardContext.Crypto))
                    {
                        giftCardContext.Password = ExtractGiftCardPassword(giftCardContext.Password, customerSysNo);
                    }
                    GiftCardInfo giftCardInfo = new GiftCardInfo()
                    {
                        Code = giftCardContext.Code,
                        Password = giftCardContext.Password
                    };
                    giftCardInfo["Crypto"] = giftCardContext.Crypto;
                    preOrderInfo.GiftCardList.Add(giftCardInfo);
                }
            }
            //购物发票，1表示要开发票
            if (newCheckoutContenxt.NeedInvoice == 1)
            {
                preOrderInfo.Receipt = new ReceiptInfo() { PersonalInvoiceTitle = result.CustomerInvoiceInfo.InvoiceTitle };
            }
            //执行真正的action操作
            OrderPipelineProcessResult checkOutResult = action(preOrderInfo);
            SetCheckoutResult(checkOutResult, result, newCheckoutContenxt);

            return result;
        }

        private static void SetCheckoutResult(OrderPipelineProcessResult orderProcessResult, CheckOutResult checkoutResult, CheckOutContext context)
        {
            checkoutResult.OrderProcessResult = orderProcessResult;
            checkoutResult.HasSucceed = orderProcessResult.HasSucceed;
            checkoutResult.ShoppingItemParam = context.ShoppingItemParam;
            if (orderProcessResult.ErrorMessages != null)
            {
                checkoutResult.ErrorMessages = orderProcessResult.ErrorMessages.SelectMany(msg =>
                {
                    if (!String.IsNullOrWhiteSpace(msg))
                    {
                        return msg.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    }
                    return new List<String>(0);
                })
                .ToList();
                checkoutResult.ErrorMessages.RemoveAll(x => String.IsNullOrWhiteSpace(x));
            }
            if (orderProcessResult != null && orderProcessResult.ReturnData != null)
            {
                //优惠券使用情况
                checkoutResult.ApplyCouponCode = orderProcessResult.ReturnData.CouponCode;
                checkoutResult.ApplyCouponName = orderProcessResult.ReturnData.CouponName;
                checkoutResult.ApplyedCouponDesc = orderProcessResult.ReturnData.CouponErrorDesc;
                //积分使用情况
                checkoutResult.UsePointPay = orderProcessResult.ReturnData.PointPay;
                checkoutResult.MaxPointPay = orderProcessResult.ReturnData.MaxPointPay;
                checkoutResult.UsePointPayDesc = orderProcessResult.ReturnData.UsePointPayDesc;
                //礼品卡使用情况
                checkoutResult.ApplyedGiftCardDesc = orderProcessResult.ReturnData.GiftCardErrorDesc;
                checkoutResult.BindingGiftCardList = orderProcessResult.ReturnData.BindingGiftCardList;
                if (orderProcessResult.ReturnData.GiftCardList != null)
                {
                    checkoutResult.ApplyedGiftCardList = orderProcessResult.ReturnData.GiftCardList.Select(g =>
                    {
                        GiftCardInfo giftCardInfo = (GiftCardInfo)g.Clone();
                        giftCardInfo.Password = ConfuseGiftCardPassword(g.Password, orderProcessResult.ReturnData.Customer.SysNo);
                        return giftCardInfo;
                    }).ToList();
                }
            }
        }

        private static Random randomSeed = new Random();

        private static string ConfuseGiftCardPassword(string password, int customerSysNo)
        {
            char[] arrayPwd = (password + customerSysNo).ToCharArray();

            int random = 0;
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < arrayPwd.Length; i++)
            {
                random = randomSeed.Next(65, 90);
                sb.Append((char)random);
                sb.Append(arrayPwd[i]);
            }

            random = randomSeed.Next(65, 90);
            sb.Append((char)random);

            string randomPwd = sb.ToString();
            return CryptoManager.GetCrypto(CryptoAlgorithm.DES).Encrypt(randomPwd);
        }

        private static string ExtractGiftCardPassword(string randomizePassword, int customerSysNo)
        {
            if (string.IsNullOrWhiteSpace(randomizePassword))
            {
                return string.Empty;
            }

            string randomEncryptPwd = CryptoManager.GetCrypto(CryptoAlgorithm.DES).Decrypt(randomizePassword);

            char[] arrayPwd = randomEncryptPwd.ToCharArray();
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < arrayPwd.Length; i++)
            {
                if (i % 2 == 1)
                {
                    sb.Append(arrayPwd[i]);
                }
            }
            string fixedPassword = sb.ToString();
            string extractCustomerSysNo = fixedPassword.Substring(fixedPassword.Length - customerSysNo.ToString().Length);
            int sysno;
            if (!int.TryParse(extractCustomerSysNo, out sysno) || sysno != customerSysNo)
            {
                return string.Empty;
            }
            return fixedPassword.Substring(0, fixedPassword.Length - customerSysNo.ToString().Length);
        }

        private static List<KeyValuePair<int, string>> m_allPaymentCategoryList = null;
        private static List<KeyValuePair<int, string>> GetAllPaymentCategoryList()
        {
            if (m_allPaymentCategoryList == null)
            {
                m_allPaymentCategoryList = new List<KeyValuePair<int, string>>();
                var dic = EnumHelper.GetDescriptions<PaymentCategory>();
                foreach (var item in dic)
                {
                    m_allPaymentCategoryList.Add(new KeyValuePair<int, string>((int)item.Key, item.Value));
                }
            }
            return m_allPaymentCategoryList;
        }

        public static List<PayTypeInfo> GetAllPayTypeList()
        {
            string cacheKey = "Payment_GetAllPayTypeList";
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (List<PayTypeInfo>)HttpRuntime.Cache[cacheKey];
            }
            List<PayTypeInfo> payTypeList = PaymentDA.GetAllPayTypeList();

            var cachedPayTypeList = SerializationUtility.DeepClone(payTypeList);
            HttpRuntime.Cache.Insert(cacheKey, cachedPayTypeList, null, DateTime.Now.AddSeconds(CacheTime.Longest), Cache.NoSlidingExpiration);

            return payTypeList;
        }

        private static CheckOutResult BuildFailedCheckOutResult(string errorMsg)
        {
            CheckOutResult result = new CheckOutResult()
            {
                PayTypeList = new List<PayTypeInfo>(),
                ShippingAddressList = new List<ShippingContactInfo>()
            };

            result.HasSucceed = false;
            result.ErrorMessages.Add(errorMsg);

            return result;
        }

        public static void SalesOrderMailSuccessful(OrderInfo order)
        {
            foreach (var subOrder in order.SubOrderList.Values)
            {
                if (subOrder.SOType == (int)SOType.VirualGroupBuy)
                    continue;
                ECommerce.Entity.Member.CustomerInfo customer = CustomerDA.GetCustomerByID(subOrder.Customer.ID);

                if (string.IsNullOrEmpty(customer.Email))
                {
                    return;
                }

                AsyncEmail email = new AsyncEmail();
                email.MailAddress = customer.Email;
                email.CustomerID = customer.CustomerID;
                email.Status = (int)EmailStatus.NotSend;
                string token = Guid.NewGuid().ToString("N");
                email.ImgBaseUrl = ConfigurationManager.AppSettings["CDNWebDomain"].ToString();
                email.SetNewTokenUrl = "/EmailVerifySucceed?token=" + token + "&sysno=" + customer.SysNo.ToString() + "&email=" + System.Web.HttpUtility.HtmlEncode(customer.Email);

                string subject = string.Empty;
                email.MailBody = MailHelper.GetMailTemplateBody("SalesOrderMailSuccessful", out subject);
                email.MailSubject = subject.Replace("[SOSysNo]", subOrder.ID.ToString());

                //订单内容
                email.MailBody = email.MailBody.Replace("[SOSysNo]", subOrder.ID.ToString());
                email.MailBody = email.MailBody.Replace("[OrderTime]", subOrder.InDate.ToString("yyyy年MM月dd日 HH时mm分"));

                email.MailBody = email.MailBody.Replace("[CustomerID]", subOrder.Customer.ID);
                email.MailBody = email.MailBody.Replace("[CustomerName]", subOrder.Customer.Name);
                email.MailBody = email.MailBody.Replace("[ContactName]", subOrder.Contact.Name);
                email.MailBody = email.MailBody.Replace("[ContactMobilePhone]", subOrder.Contact.MobilePhone);
                email.MailBody = email.MailBody.Replace("[ContactPhone]", subOrder.Contact.Phone);

                var area = CommonFacade.GetArea(subOrder.Contact.AddressAreaID);
                string address = string.Format("{0} {1} {2} {3}", area.ProvinceName, area.CityName, area.DistrictName, subOrder.Contact.AddressDetail);
                email.MailBody = email.MailBody.Replace("[ContactAddress]", address);
                email.MailBody = email.MailBody.Replace("[ContactZipCode]", subOrder.Contact.ZipCode);

                email.MailBody = email.MailBody.Replace("[ShipTypeName]", (string)subOrder["ShipTypeName"]);
                email.MailBody = email.MailBody.Replace("[PayTypeName]", subOrder.PayTypeName);
                email.MailBody = email.MailBody.Replace("[CashPayAmountWithTax]", (subOrder.CashPayAmount + subOrder.TaxAmount).ToString("F2"));
                email.MailBody = email.MailBody.Replace("[TotalWeight]", (((decimal)subOrder.TotalWeight) / 1000).ToString("F3"));
                email.MailBody = email.MailBody.Replace("[ShippingAmount]", subOrder.ShippingAmount.ToString("F2"));
                email.MailBody = email.MailBody.Replace("[TaxAmount]", subOrder.TaxAmount.ToString("F2"));
                email.MailBody = email.MailBody.Replace("[CouponAmount]", subOrder.CouponAmount.ToString("F2"));
                email.MailBody = email.MailBody.Replace("[GiftCardPayAmount]", subOrder.GiftCardPayAmount.ToString("F2"));
                email.MailBody = email.MailBody.Replace("[BalancePayAmount]", subOrder.BalancePayAmount.ToString("F2"));
                email.MailBody = email.MailBody.Replace("[TotalDiscountAmount]", subOrder.TotalDiscountAmount.ToString("F2"));
                email.MailBody = email.MailBody.Replace("[PointPayAmount]", subOrder.PointPayAmount.ToString("F2"));
                email.MailBody = email.MailBody.Replace("[SOAmount]", subOrder.SOAmount.ToString("F2"));
                email.MailBody = email.MailBody.Replace("[SOMemo]", subOrder.Memo);
                email.MailBody = email.MailBody.Replace("[SendTime]", DateTime.Now.ToString("yyyy-MM-dd"));
                email.MailBody = email.MailBody.Replace("[Year]", DateTime.Now.Year.ToString());

                string domain = ConfigurationManager.AppSettings["WebDomain"].ToString();

                StringBuilder sb = new StringBuilder();

                if (subOrder.OrderItemGroupList != null)
                {
                    subOrder.OrderItemGroupList.ForEach(groupItem =>
                    {
                        groupItem.ProductItemList.ForEach(item =>
                        {
                            sb.Append("<tr style=\"background:#fff;\">");
                            sb.AppendFormat("<td align=\"center\" valign=\"middle\" style=\"padding-left:5px; border-left:1px solid #eeeeee; border-top:1px solid #eeeeee;\"><a href=\"{0}\"><img src=\"{1}\" width=\"50\" height=\"50\" border=\"0\" /></a></td>", domain + "/product/detail/" + item.ProductSysNo, ProductFacade.BuildProductImage(ImageSize.P60, item.DefaultImage));
                            sb.AppendFormat("<td align=\"center\" style=\"padding-left:5px; border-left:1px solid #eeeeee; border-top:1px solid #eeeeee;\"><p>{0}</p></td>", item.ProductID);
                            sb.AppendFormat("<td valign=\"middle\" style=\"padding-left:5px; border-left:1px solid #eeeeee; border-top:1px solid #eeeeee; border-right:1px solid #eeeeee;\"><a href=\"{0}\" style=\"color:#ff6600; text-decoration:none; line-height:18px;\">{1}</a></td>", domain + "/product/detail/" + item.ProductSysNo, item.ProductName);
                            sb.AppendFormat("<td align=\"center\" style=\"border-right:1px solid #eeeeee; border-top:1px solid #eeeeee;\">&yen;{0}</td>", item.UnitSalePrice.ToString("F2"));
                            sb.AppendFormat("<td align=\"center\" style=\"border-right:1px solid #eeeeee; border-top:1px solid #eeeeee;\">{0}</td>", item.UnitQuantity);
                            sb.AppendFormat("<td align=\"center\" style=\"border-right:1px solid #eeeeee; border-top:1px solid #eeeeee;\">&yen;{0}</td>", item.TotalSalePrice.ToString("F2"));
                            sb.Append("</tr>");
                        });
                    });
                }
                if (subOrder.GiftItemList != null)
                {
                    var mergedSaleGiftList = new List<OrderGiftItem>();
                    subOrder.GiftItemList.ForEach(gift =>
                    {
                        if (!mergedSaleGiftList.Exists(g =>
                        {
                            if (g.ProductSysNo == gift.ProductSysNo)
                            {
                                g.UnitQuantity += gift.UnitQuantity;
                                return true;
                            }
                            return false;
                        }))
                        {
                            mergedSaleGiftList.Add(gift);
                        }
                    });
                    foreach (var item in mergedSaleGiftList)
                    {
                        sb.Append("<tr style=\"background:#FFF4F2;\">");
                        sb.AppendFormat("<td style=\"padding-left:5px; border-left:1px solid #eeeeee; border-top:1px solid #eeeeee;\">&nbsp;</td>");
                        sb.AppendFormat("<td style=\"padding-left:5px; border-left:1px solid #eeeeee; border-top:1px solid #eeeeee;\">&nbsp;</td>");
                        sb.AppendFormat("<td valign=\"middle\" style=\"padding-left:5px; border-left:1px solid #eeeeee; border-top:1px solid #eeeeee; border-right:1px solid #eeeeee;line-height:18px;\"><span style=\"color:#ff6600; padding-right:8px;\">[赠品]</span>{0}</td>", item.ProductName);
                        sb.AppendFormat("<td align=\"center\" style=\"border-right:1px solid #eeeeee; border-top:1px solid #eeeeee;\">&yen;{0}</td>", item.UnitSalePrice.ToString("F2"));
                        sb.AppendFormat("<td align=\"center\" style=\"border-right:1px solid #eeeeee; border-top:1px solid #eeeeee;\">{0}</td>", item.UnitQuantity);
                        sb.AppendFormat("<td align=\"center\" style=\"border-right:1px solid #eeeeee; border-top:1px solid #eeeeee;\">&yen;{0}</td>", item.TotalSalePrice.ToString("F2"));
                        sb.Append("</tr>");
                    }
                }
                if (subOrder.AttachmentItemList != null)
                {
                    foreach (var item in subOrder.AttachmentItemList)
                    {
                        sb.Append("<tr style=\"background:#FFF4F2;\">");
                        sb.AppendFormat("<td style=\"padding-left:5px; border-left:1px solid #eeeeee; border-top:1px solid #eeeeee;\">&nbsp;</td>");
                        sb.AppendFormat("<td style=\"padding-left:5px; border-left:1px solid #eeeeee; border-top:1px solid #eeeeee;\">&nbsp;</td>");
                        sb.AppendFormat("<td valign=\"middle\" style=\"padding-left:5px; border-left:1px solid #eeeeee; border-top:1px solid #eeeeee; border-right:1px solid #eeeeee;line-height:18px;\"><span style=\"color:#ff6600; padding-right:8px;\">[附件]</span>{0}</td>", item.ProductName);
                        sb.AppendFormat("<td align=\"center\" style=\"border-right:1px solid #eeeeee; border-top:1px solid #eeeeee;\">&yen;{0}</td>", item.UnitSalePrice.ToString("F2"));
                        sb.AppendFormat("<td align=\"center\" style=\"border-right:1px solid #eeeeee; border-top:1px solid #eeeeee;\">{0}</td>", item.UnitQuantity);
                        sb.AppendFormat("<td align=\"center\" style=\"border-right:1px solid #eeeeee; border-top:1px solid #eeeeee;\">&yen;{0}</td>", item.TotalSalePrice.ToString("F2"));
                        sb.Append("</tr>");
                    }
                }
                if (subOrder.TotalRewardedPoint >= 0)
                {
                    sb.Append("<tr style=\"background:#fff;\">");
                    sb.AppendFormat("<td colspan=\"6\" style=\"border-bottom:1px solid #eeeeee; border-top:1px solid #eeeeee; height:41px; text-align:center; font-weight:bold;\" align=\"center\">本单交易成功后您可以获得积分：<strong style=\"color:#c40000; font-size:12px; font-family:microsoft yahei;\">{0}</strong></td>", subOrder.TotalRewardedPoint);
                    sb.Append("</tr>");
                }

                email.MailBody = email.MailBody.Replace("[OrderContent]", sb.ToString()).Replace("[WebBaseUrl]", domain).Replace("[ImgBaseUrl]", email.ImgBaseUrl);
                EmailDA.SendEmail(email);
            }
        }

        private static List<T> EnsureNotNullObject<T>(List<T> list)
        {
            if (list == null)
            {
                list = new List<T>(0);
            }
            return list;
        }

        private static T EnsureNotNullObject<T>(T obj)
            where T : class, new()
        {
            if (obj == null)
            {
                obj = new T();
            }
            return obj;
        }
        public static List<ECommerce.Entity.Member.CustomerCouponInfo> GetCustomerPlatformCouponCode(CheckOutResult model)
        {
            int customerID = model.Customer.SysNo;
            var item = model.OrderProcessResult.ReturnData.OrderItemGroupList.FirstOrDefault();
            int merchantSysNo = item == null ? 1 : item.MerchantSysNo;
            return ECommerce.SOPipeline.Impl.CouponCalculator.GetCustomerPlatformCouponCode(customerID, merchantSysNo);
        }
        public static List<ECommerce.Entity.Member.CustomerCouponInfo> GetCustomerPlatformCouponCode(int UserSysNo, int merchantSysNo)
        {
            return ECommerce.SOPipeline.Impl.CouponCalculator.GetCustomerPlatformCouponCode(UserSysNo, merchantSysNo);
        }

        /// <summary>
        /// 获取当前用户可参与的优惠券活动（当商家不确定或者为自营商品时获取平台优惠券活动，第三方商家是获取当前用户在该商家可参与的优惠券活动）
        /// </summary>
        /// Create by John.E.Kang 2015.11.03
        /// <param name="UserSysNo">用户编码</param>
        /// <param name="MerchantSysNo">商家编码</param>
        /// <returns></returns>
        public static List<CouponInfo> GetCouponList(int UserSysNo, int MerchantSysNo)
        {
            int RemainingNumber = 0;
            List<CouponInfo> couponList_pre = new List<CouponInfo>();
            List<CouponInfo> couponList = new List<CouponInfo>();
            try
            {
                couponList_pre = PromotionDA.GetCouponList(UserSysNo, MerchantSysNo);

                foreach (var item in couponList_pre)
                {
                    CouponInfo couponinfo = PromotionDA.GetCouponInfo(item.SysNo);
                    if (couponinfo != null)
                    {
                        item.SaleRulesList = couponinfo.SaleRulesList;
                        item.AssignCustomerList = couponinfo.AssignCustomerList;
                        item.DiscountRuleList = couponinfo.DiscountRuleList;
                        item.SaleRulesEx = couponinfo.SaleRulesEx;
                        //当为百分比折扣是显示前台显示折扣价
                        if (couponinfo.DiscountRuleList[0].DiscountType == CouponDiscountType.OrderAmountPercentage)
                        {
                            item.DiscountRuleList[0].Value = 1 - couponinfo.DiscountRuleList[0].Value;
                        }
                    }
                    //活动已参与次数
                    int UsedTotalNumber = PromotionDA.GetCodeNumberByCouponNumber(item.SysNo);
                    //校验用户是否可领取优惠券
                    bool receivable = false;
                    if (couponinfo.SaleRulesEx.CustomerMaxFrequency.HasValue)
                    {
                        receivable = PromotionDA.CheckUserAreadyGetCode(UserSysNo, item.SysNo, (int)couponinfo.SaleRulesEx.CustomerMaxFrequency, out RemainingNumber);
                    }
                    if (!receivable && !(item.SaleRulesEx.MaxFrequency.HasValue
                        && UsedTotalNumber >= item.SaleRulesEx.MaxFrequency))
                    {
                        couponList.Add(item);
                    }
                }

                return couponList;
            }
            catch
            {
                couponList = null;
                return couponList;
            }
        }

        public static string UserGetCouponCode(int UserSysNo, int CouponSysNo, out string couponCodestr)
        {
            couponCodestr = "";
            int RemainingNumber = 0;
            CouponInfo coupon = new CouponInfo();
            coupon = PromotionDA.GetCouponInfo(CouponSysNo);
            if (coupon == null)
            {
                return "优惠券活动异常，请联系客服！";
            }
            CouponCode couponCode = new CouponCode();
            CouponCodeCustomerLog couponCodeCustomLog = new CouponCodeCustomerLog();
            try
            {
                //校验用户是否已领取优惠券

                int CouponNumber = PromotionDA.GetCodeNumberByCouponNumber(coupon.SysNo);

                if (coupon.SaleRulesEx.MaxFrequency.HasValue && CouponNumber >= coupon.SaleRulesEx.MaxFrequency)
                {
                    return "活动优惠券已被领完，谢谢参与！";
                }
                if (coupon.SaleRulesEx.CustomerMaxFrequency.HasValue && PromotionDA.CheckUserAreadyGetCode(UserSysNo, CouponSysNo, (int)coupon.SaleRulesEx.CustomerMaxFrequency, out RemainingNumber))
                {
                    return "您领取次数已完，谢谢参与！";
                }
                string GenerateCode = GenerateRandomCode(10);
                couponCode.Code = GenerateCode;
                //领取优惠券为通用优惠券
                couponCode.CodeType = "C";
                couponCode.CouponSysNo = coupon.SysNo;
                couponCode.CustomerMaxFrequency = 1;
                couponCode.EndDate = coupon.EndDate;
                couponCode.TotalCount = 1;
                couponCode.BeginDate = coupon.BeginDate;
                int couponCodeSysNo = PromotionDA.InsertCouponCode(UserSysNo, couponCode);
                if (couponCodeSysNo <= 0)
                {
                    return "无法领取优惠券，请联系客服！";
                }

                couponCodeCustomLog.CouponCode = GenerateCode;
                couponCodeCustomLog.CouponSysNo = coupon.SysNo;
                couponCodeCustomLog.CustomerSysNo = UserSysNo;
                couponCodeCustomLog.UserCodeType = "L";
                if (PromotionDA.InsertCustomerLog(couponCodeCustomLog))
                {
                    couponCodestr = GenerateCode;
                    return "您已成功领取优惠券，号码：" + GenerateCode;
                    //if (RemainingNumber>0)
                    //{
                    //    RemainingNumber--;
                    //    return "您已成功领取优惠券，号码：" + GenerateCode + "。\n你还有" + RemainingNumber+"次领取机会！";
                    //}
                    //else
                    //{
                    //   return "您已成功领取优惠券，号码：" + GenerateCode;
                    //}

                }
                else
                {
                    return "无法领取优惠券，请联系客服！";
                }
            }
            catch
            {
                return "领取优惠券异常，请联系客服！";
            }
        }
        //获取优惠券
        private static string GenerateRandomCode(int length)
        {
            Thread.Sleep(10);
            const string template = "234679ACDEFGHJKLMNPQRTUVWXYZ";
            int success = 0;
            Random random = new Random();
            random.Next();
            string CouponCode = string.Empty;
            while (success == 0)
            {
                StringBuilder builder = new StringBuilder();
                int maxRandom = template.Length - 1;
                for (int i = 0; i < length; i++)
                {
                    builder.Append(template[random.Next(maxRandom)]);
                }
                CouponCode = builder.ToString();
                if (!PromotionDA.CheckExistCode(CouponCode, null))
                {
                    success = 1;
                }
            }
            return CouponCode;
        }
    }
}
