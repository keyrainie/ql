using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ECommerce.Entity;
using ECommerce.Entity.Payment;
using ECommerce.Entity.Product;
using ECommerce.Entity.Shipping;
using ECommerce.Enums;

namespace ECommerce.SOPipeline.Impl
{
    public class OrderAmountCalculator : ICalculate
    {
        public void Calculate(ref OrderInfo order)
        {
            //初始化并取得所有的订单item
            var dicAllSubOrderItem = new Dictionary<string, List<OrderItem>>(0);
            InitAndGetAllOrderItems(order, out dicAllSubOrderItem);

            //计算运费
            CalculateShippingAmount(order);

            //计算手续费
            CalculateCommissionAmount(order);

            //计算促销折扣金额，主要是进行金额分摊
            CalculateDiscountAmount(order);

            //计算优惠券折扣，主要是进行金额分摊
            CalculateCouponAmount(order);

            //计算关税
            //CalculateTrariffAmount(order, dicAllSubOrderItem);

            //计算积分支付金额
            CalculatePointPayAmount(order);

            //计算网银积分支付金额
            //CalculateBankPointPayAmount(order);

            //计算礼品卡支付金额
            //CalculateGiftCardPayAmount(order);

            //计算余额支付
            CalculateBalancePayAmount(order);
        }

        /// <summary>
        /// 初始化计算
        /// </summary>
        /// <param name="order"></param>
        private void InitAndGetAllOrderItems(OrderInfo order, out Dictionary<string, List<OrderItem>> dicSubOrderItem)
        {
            dicSubOrderItem = new Dictionary<string, List<OrderItem>>();

            if (order.SubOrderList == null)
            {
                order.SubOrderList = new Dictionary<string, OrderInfo>();
            }

            OrderInfo subOrderInfo = null;
            List<OrderItem> subOrderItemList = null;

            foreach (var kvs in order.SubOrderList)
            {
                subOrderInfo = kvs.Value;
                subOrderInfo.PointPay = 0;
                subOrderInfo.PointPayAmount = 0m;
                subOrderInfo.BalancePayAmount = 0m;

                subOrderItemList = new List<OrderItem>();
                if (subOrderInfo.OrderItemGroupList != null)
                {
                    foreach (var itemGroup in subOrderInfo.OrderItemGroupList)
                    {
                        if (itemGroup.ProductItemList != null)
                        {
                            itemGroup.ProductItemList.ForEach(product =>
                            {
                                product["UnitDiscountAmt"] = 0m;
                                product["UnitCouponAmt"] = product["UnitCouponAmt"] == null ? 0m : product["UnitCouponAmt"];
                                subOrderItemList.Add(product);
                            });
                        }
                    }
                }
                if (subOrderInfo.GiftItemList != null)
                {
                    subOrderInfo.GiftItemList.ForEach(gift =>
                    {
                        gift["UnitDiscountAmt"] = 0m;
                        gift["UnitCouponAmt"] = 0m;
                        subOrderItemList.Add(gift);
                    });
                }
                if (subOrderInfo.AttachmentItemList != null)
                {
                    subOrderInfo.AttachmentItemList.ForEach(attachment =>
                    {
                        attachment["UnitDiscountAmt"] = 0m;
                        attachment["UnitCouponAmt"] = 0m;
                        subOrderItemList.Add(attachment);
                    });
                }

                dicSubOrderItem.Add(kvs.Key, subOrderItemList);
            }
        }

        /// <summary>
        /// 计算运费
        /// </summary>
        /// <param name="order"></param>
        private void CalculateShippingAmount(OrderInfo order)
        {
            #region 【 step1 设置好各个子单的配送方式 】

            List<ShipTypeInfo> allShipTypeList = PipelineDA.GetSupportedShipTypeList(order.Contact.AddressAreaID,(PaymentCategory)order.PaymentCategory);

            List<int> stockSysNoList = order.SubOrderList.Values.Where(x => x["WarehouseNumber"] != null && (int)x["WarehouseNumber"] > 0)
                                                                .Select(x => Convert.ToInt32(x["WarehouseNumber"]))
                                                                .Distinct()
                                                                .ToList();
            List<SimpleStockInfo> stockList = PipelineDA.QueryStockInfoList(stockSysNoList);

            List<ShippingFeeQueryInfo> qryList = new List<ShippingFeeQueryInfo>();
            ShippingFeeQueryInfo qry = null;
            ShipTypeInfo curShipTypeInfo = null;
            //SimpleStockInfo curStockInfo = null;

            foreach (var kvs in order.SubOrderList)
            {
                OrderInfo subOrderInfo = kvs.Value;

                if (subOrderInfo["WarehouseNumber"] != null && (int)subOrderInfo["WarehouseNumber"] > 0)
                {
                    //curStockInfo = stockList.Find(x => x.SysNo == Convert.ToInt32(subOrderInfo["WarehouseNumber"]));
                    //判断是否是外贸型仓库，如果是外贸型仓库，配送方式由系统决定，否则由用户选择决定
                    //if (curStockInfo.StockType == 0 || curStockInfo.StockType == 1)
                    //{
                    //    curShipTypeInfo = PipelineDA.Pipeline_GetMerchantStockShippingType(Convert.ToInt32(kvs.Key.Split('|')[0]), Convert.ToInt32(subOrderInfo["WarehouseNumber"]));
                    //}
                    //else
                    //{
                    curShipTypeInfo = allShipTypeList.Find(x => x.ShipTypeSysNo.ToString() == subOrderInfo.ShipTypeID);
                    //}
                }
                if (curShipTypeInfo != null)
                {
                    subOrderInfo.ShipTypeID = curShipTypeInfo.ShipTypeID;
                    subOrderInfo["ShipTypeName"] = curShipTypeInfo.ShipTypeName;
                    subOrderInfo["ShipTypeDesc"] = string.Format("{1}出库（{0}）", curShipTypeInfo.ShipTypeName
                                                                                  , subOrderInfo["WarehouseName"]);

                    qry = new ShippingFeeQueryInfo();
                    qry.TransID = kvs.Key;
                    qry.SoAmount = subOrderInfo.TotalProductAmount;
                    qry.SoTotalWeight = subOrderInfo.TotalWeight;
                    qry.SOSingleMaxWeight = subOrderInfo.MaxWeight;
                    qry.AreaId = subOrderInfo.Contact.AddressAreaID;
                    qry.CustomerSysNo = subOrderInfo.Customer.SysNo;
                    qry.IsUseDiscount = 0;
                    qry.SubShipTypeList = curShipTypeInfo.ShipTypeSysNo.ToString();
                    qry.SellType = Convert.ToInt32(subOrderInfo["SellerType"]);
                    qry.MerchantSysNo = Convert.ToInt32(kvs.Key.Split('|')[0]);
                    qry.ShipTypeId = 0;
                    qryList.Add(qry);
                }
            }

            #endregion

            #region 【 开始计算运费 】

            List<ShippingInfo> shipFeeList = PipelineDA.GetAllShippingFee(qryList);
            ShippingInfo curShippingInfo = null;

            foreach (var kvs in order.SubOrderList)
            {
                curShippingInfo = shipFeeList.Find(x => x.TransID == kvs.Key && x.ShippingTypeID.ToString() == kvs.Value.ShipTypeID);
                if (curShippingInfo != null)
                {
                    kvs.Value.ShippingAmount = curShippingInfo.ShippingPrice; //+curShippingInfo.ShippingPackageFee; 
                }
            }

            order.ShippingAmount = order.SubOrderList.Sum(x => x.Value.ShippingAmount);

            #endregion
        }

        /// <summary>
        /// 计算积分支付金额
        /// </summary>
        /// <param name="order"></param>
        private void CalculatePointPayAmount(OrderInfo order)
        {
            //积分兑换比例
            decimal pointExhangeRate = 0m;
            decimal.TryParse(ConstValue.PointExhangeRate, out pointExhangeRate);

            //订单最大可使用积分
            //Modified by PoseidonTong 积分支付不能抵扣关税金额 at [2014-09-19 14:18:34] 
            order.MaxPointPay = (int)Math.Floor((order.SOAmount - order.TaxAmount - order.CouponAmount) * pointExhangeRate);

            //需要重新输入使用积分，主要考虑使用了积分后再使用优惠券，这时候之前
            //输入的积分必然超出最大可用积分，此时将使用积分清零，由用户重新输入要使用的积分
            if (order.PointPay > 0 && order.PointPay > order.MaxPointPay)
            {
                order.PointPay = 0;
            }

            //帐户可用积分不足，将订单使用积分修改为帐户可用积分
            if (order.Customer.AccountPoint < order.PointPay)
            {
                order.PointPay = 0;
                order.PointPayAmount = 0m;
                order.UsePointPayDesc = "积分余额不足，不能使用。";
                foreach (var kvs in order.SubOrderList)
                {
                    kvs.Value.PointPay = 0;
                    kvs.Value.PointPayAmount = 0m;
                }
            }
            else
            {
                var availablePointPay = order.PointPay;
                //Modified by PoseidonTong 积分支付不能抵扣关税金额 at [2014-09-19 14:18:34] 
                List<OrderInfo> subOrderList = order.SubOrderList.Where(x => (x.Value.SOAmount - x.Value.TaxAmount) > 0m)
                                                                .Select(kvs => kvs.Value)
                                                                .ToList();

                //按照订单金额降序排列，订单金额高的优先使用余额支付
                subOrderList.Sort((subOrder1, subOrder2) =>
                {
                    //Modified by PoseidonTong 积分支付不能抵扣关税金额 at [2014-09-19 14:18:34] 
                    return -1 * (subOrder1.SOAmount - subOrder1.TaxAmount).CompareTo((subOrder2.SOAmount - subOrder2.TaxAmount));
                });

                //分配订单积分支付金额
                foreach (var subOrder in subOrderList)
                {
                    if (availablePointPay > 0)
                    {
                        //Modified by PoseidonTong 积分支付不能抵扣关税金额 at [2014-09-19 14:18:34] 
                        var canPointPay = (int)Math.Floor((subOrder.SOAmount - subOrder.TaxAmount - subOrder.CouponAmount) * pointExhangeRate);

                        if (canPointPay > availablePointPay)
                        {
                            canPointPay = availablePointPay;
                        }
                        subOrder.PointPay = canPointPay;
                        availablePointPay -= canPointPay;
                    }
                    else
                    {
                        subOrder.PointPay = 0;
                    }
                    subOrder.PointPayAmount = decimal.Round(subOrder.PointPay / pointExhangeRate, 2, MidpointRounding.AwayFromZero);
                }
                //重新计算整单积分支付金额 
                order.PointPay = order.SubOrderList.Sum(x => x.Value.PointPay);
                order.PointPayAmount = decimal.Round(order.PointPay / pointExhangeRate, 2, MidpointRounding.AwayFromZero);
                order.UsePointPayDesc = null;
            }
        }

        /// <summary>
        /// 计算网银积分支付金额
        /// </summary>
        /// <param name="order"></param>
        private void CalculateBankPointPayAmount(OrderInfo order)
        {
            //积分兑换比例
            decimal bankPointExhangeRate = 0m;
            decimal.TryParse(ConstValue.BankPointExhangeRate, out bankPointExhangeRate);

            //订单最大可使用积分
            //Modified by PoseidonTong 积分支付不能抵扣关税金额 at [2014-09-19 14:18:34] 
            order.MaxPointPay = (int)Math.Floor((order.SOAmount - order.TaxAmount - order.CouponAmount) * bankPointExhangeRate);

            //需要重新输入使用积分，主要考虑使用了积分后再使用优惠券，这时候之前
            //输入的积分必然超出最大可用积分，此时将使用积分清零，由用户重新输入要使用的积分
            if (order.BankPointPay > 0 && order.BankPointPay > order.MaxPointPay)
            {
                order.BankPointPay = 0;
            }

            //帐户可用积分不足，将订单使用积分修改为帐户可用积分
            if (order.Customer.BankAccountPoint < order.BankPointPay)
            {
                order.BankPointPay = 0;
                order.BankPointPayAmount = 0m;
                order.UsePointPayDesc = "积分余额不足，不能使用。";
                foreach (var kvs in order.SubOrderList)
                {
                    kvs.Value.BankPointPay = 0;
                    kvs.Value.BankPointPayAmount = 0m;
                }
            }
            else
            {
                var availablePointPay = order.BankPointPay;
                //Modified by PoseidonTong 积分支付不能抵扣关税金额 at [2014-09-19 14:18:34] 
                List<OrderInfo> subOrderList = order.SubOrderList.Where(x => (x.Value.SOAmount - x.Value.TaxAmount) > 0m)
                                                                .Select(kvs => kvs.Value)
                                                                .ToList();

                //按照订单金额降序排列，订单金额高的优先使用余额支付
                subOrderList.Sort((subOrder1, subOrder2) =>
                {
                    //Modified by PoseidonTong 积分支付不能抵扣关税金额 at [2014-09-19 14:18:34] 
                    return -1 * (subOrder1.SOAmount - subOrder1.TaxAmount).CompareTo((subOrder2.SOAmount - subOrder2.TaxAmount));
                });

                //分配订单积分支付金额
                foreach (var subOrder in subOrderList)
                {
                    if (availablePointPay > 0)
                    {
                        //Modified by PoseidonTong 积分支付不能抵扣关税金额 at [2014-09-19 14:18:34] 
                        var canPointPay = (int)Math.Floor((subOrder.SOAmount - subOrder.TaxAmount - subOrder.CouponAmount) * bankPointExhangeRate);

                        if (canPointPay > availablePointPay)
                        {
                            canPointPay = availablePointPay;
                        }
                        subOrder.BankPointPay = canPointPay;
                        availablePointPay -= canPointPay;
                    }
                    else
                    {
                        subOrder.BankPointPay = 0;
                    }
                    subOrder.BankPointPayAmount = decimal.Round(subOrder.BankPointPay / bankPointExhangeRate, 2, MidpointRounding.AwayFromZero);
                }
                //重新计算整单积分支付金额 
                order.BankPointPay = order.SubOrderList.Sum(x => x.Value.BankPointPay);
                order.BankPointPayAmount = decimal.Round(order.BankPointPay / bankPointExhangeRate, 2, MidpointRounding.AwayFromZero);
                order.UsePointPayDesc = null;
            }
        }

        /// <summary>
        /// 计算礼品卡支付金额
        /// </summary>
        /// <param name="order"></param>
        private void CalculateGiftCardPayAmount(OrderInfo order)
        {
            if (order.GiftCardList == null || order.GiftCardList.Count <= 0)
            {
                foreach (KeyValuePair<string, OrderInfo> kvs in order.SubOrderList)
                {
                    kvs.Value.GiftCardPayAmount = 0m;
                }
            }
            else
            {
                PreCheckCalculateGiftCardPay(order);
                if (order.GiftCardList == null || order.GiftCardList.Count <= 0)
                {
                    foreach (KeyValuePair<string, OrderInfo> kvs in order.SubOrderList)
                    {
                        kvs.Value.GiftCardPayAmount = 0m;
                    }
                }
                else
                {
                    order.GiftCardList.ForEach(giftCardInfo =>
                    {
                        giftCardInfo["ActAvailableAmount"] = giftCardInfo.AvailableAmount;
                    });
                    //优先使用快过期的礼品卡
                    order.GiftCardList.Sort((giftCard1, giftCard2) =>
                    {
                        var diffDate1 = giftCard1.ValidEndDate.Subtract(DateTime.Today);
                        var diffDate2 = giftCard2.ValidEndDate.Subtract(DateTime.Today);

                        return diffDate1.CompareTo(diffDate2);
                    });

                    List<OrderInfo> subPreOrderInfoList = order.SubOrderList.Select(kvs => kvs.Value)
                                      .Where(x => x.SOAmount - x.TaxAmount > 0m).ToList();

                    //按照订单金额降序排列，订单金额高的优先使用礼品卡支付
                    subPreOrderInfoList.Sort((subOrder1, subOrder2) =>
                    {
                        return -1 * (subOrder1.SOAmount - subOrder1.TaxAmount).CompareTo((subOrder2.SOAmount - subOrder2.TaxAmount));
                    });

                    foreach (OrderInfo subPreOrderInfo in subPreOrderInfoList)
                    {
                        subPreOrderInfo.GiftCardList = new List<GiftCardInfo>();

                        //计算总计可以使用礼品卡支付的金额，需要扣除关税，关税不能使用礼品卡抵扣 
                        decimal totalCanUseGiftPayAmount = subPreOrderInfo.SOAmount - subPreOrderInfo.TaxAmount
                                                            - subPreOrderInfo.PointPayAmount - subPreOrderInfo.CouponAmount;

                        //从礼品卡列表中选出可用余额大于0的礼品卡
                        var availableGiftCardList = order.GiftCardList.Where(g => g.AvailableAmount > 0).ToList();
                        foreach (GiftCardInfo giftCardInfo in availableGiftCardList)
                        {
                            //该子单已经使用礼品卡付清，结束本轮循环，计算下一张子单
                            if (totalCanUseGiftPayAmount <= 0)
                            {
                                break;
                            }
                            GiftCardInfo clonedGiftCardInfo = (GiftCardInfo)giftCardInfo.Clone();
                            //该张礼品卡可用余额小于登录总共可用于礼品卡支付的金额，那么这张礼品卡将全额支付
                            //否则该张礼品卡使用部分金额来支付剩余待支付金额
                            if (giftCardInfo.AvailableAmount <= totalCanUseGiftPayAmount)
                            {
                                clonedGiftCardInfo.UseAmount = giftCardInfo.AvailableAmount;
                            }
                            else
                            {
                                clonedGiftCardInfo.UseAmount = totalCanUseGiftPayAmount;
                            }
                            //扣减该张礼品卡的可用余额，更新该张礼品卡的使用金额
                            giftCardInfo.AvailableAmount -= clonedGiftCardInfo.UseAmount;
                            giftCardInfo.UseAmount += clonedGiftCardInfo.UseAmount;

                            totalCanUseGiftPayAmount -= clonedGiftCardInfo.UseAmount;
                            subPreOrderInfo.GiftCardList.Add(clonedGiftCardInfo);
                        }
                        //计算子单使用礼品卡抵扣金额
                        subPreOrderInfo.GiftCardPayAmount = subPreOrderInfo.GiftCardList.Sum(g => g.UseAmount);
                    }
                }
            }
            //重新计算整单礼品卡支付金额 
            order.GiftCardPayAmount = order.SubOrderList.Sum(x => x.Value.GiftCardPayAmount);
        }

        /// <summary>
        /// 检查客户端传来的礼品卡数据，并过滤不合法的数据
        /// </summary>
        /// <param name="order"></param>
        private void PreCheckCalculateGiftCardPay(OrderInfo order)
        {
            //去掉卡号重复的礼品卡
            order.GiftCardList = order.GiftCardList.Distinct(new GiftCardInfoEqualityComparer()).ToList();

            //查询得到礼品卡信息
            List<string> giftCardCodeList = order.GiftCardList.Select(g => SafeTrim(g.Code)).ToList();
            List<GiftCardInfo> giftCardInfoList = PipelineDA.QueryGiftCardInfoList(giftCardCodeList);

            List<string> invalidGiftCardCodeList = new List<string>();
            StringBuilder giftCardErrorDesc = new StringBuilder();

            for (var index = order.GiftCardList.Count - 1; index >= 0; index--)
            {
                if (string.IsNullOrWhiteSpace(order.GiftCardList[index].Code))
                {
                    giftCardErrorDesc.AppendLine(LanguageHelper.GetText("礼品卡卡号不能为空！请重新输入正确的卡号。", order.LanguageCode));
                    invalidGiftCardCodeList.Add(order.GiftCardList[index].Code);
                }
                else
                {
                    var giftCardInfo = giftCardInfoList.Find(g => String.Equals(SafeTrim(g.Code), SafeTrim(order.GiftCardList[index].Code), StringComparison.InvariantCultureIgnoreCase));
                    if (giftCardInfo == null)
                    {
                        giftCardErrorDesc.AppendLine(LanguageHelper.GetText(string.Format("礼品卡【{0}】密码输入错误！请重新输入正确的密码。", order.GiftCardList[index].Code), order.LanguageCode));
                        invalidGiftCardCodeList.Add(order.GiftCardList[index].Code);
                    }
                    else
                    {
                        giftCardInfo["Crypto"] = "1";
                        if (giftCardInfo.BindingCustomerSysNo > 0)
                        {
                            if (giftCardInfo.BindingCustomerSysNo != order.Customer.SysNo)
                            {
                                giftCardErrorDesc.AppendLine(LanguageHelper.GetText(string.Format("您输入的礼品卡【{0}】只能由其绑定的客户使用！", giftCardInfo.Code), order.LanguageCode));
                                invalidGiftCardCodeList.Add(giftCardInfo.Code);
                                continue;
                            }
                        }
                        else
                        {
                            string inputPassword = order.GiftCardList[index].Password;
                            if (order.GiftCardList[index].HasProperty("Crypto") && Convert.ToString(order.GiftCardList[index]["Crypto"]) == "1")
                            {
                                //用户篡改客户端数据导致无法解密，按密码错误处理
                                try { inputPassword = ECommerce.Utility.CryptoManager.GetCrypto(ECommerce.Utility.CryptoAlgorithm.DES).Decrypt(SafeTrim(inputPassword)); }
                                catch { inputPassword = String.Empty; }
                            }
                            string decryptedPwd = ECommerce.Utility.CryptoManager.GetCrypto(ECommerce.Utility.CryptoAlgorithm.DES).Decrypt(SafeTrim(giftCardInfo.Password));
                            bool passwordValid = String.Equals(decryptedPwd, SafeTrim(inputPassword), StringComparison.InvariantCultureIgnoreCase);

                            if (!passwordValid)
                            {
                                giftCardErrorDesc.AppendLine(LanguageHelper.GetText(string.Format("礼品卡【{0}】密码输入错误！请重新输入正确的密码。", giftCardInfo.Code), order.LanguageCode));
                                invalidGiftCardCodeList.Add(giftCardInfo.Code);
                                continue;
                            }
                        }
                        if (giftCardInfo.Status == "D")
                        {
                            giftCardErrorDesc.AppendLine(LanguageHelper.GetText(string.Format("您输入的礼品卡【{0}】还未激活！", giftCardInfo.Code), order.LanguageCode));
                            invalidGiftCardCodeList.Add(giftCardInfo.Code);
                            continue;
                        }
                        if (giftCardInfo.IsLock)
                        {
                            giftCardErrorDesc.AppendLine(LanguageHelper.GetText(string.Format("您输入的礼品卡【{0}】已被锁定，暂时无法使用！", giftCardInfo.Code), order.LanguageCode));
                            invalidGiftCardCodeList.Add(giftCardInfo.Code);
                            continue;
                        }
                        if (giftCardInfo.ValidBeginDate > DateTime.Today || giftCardInfo.ValidEndDate < DateTime.Today)
                        {
                            giftCardErrorDesc.AppendLine(LanguageHelper.GetText(string.Format("您输入的礼品卡【{0}】未生效或已过期！", giftCardInfo.Code), order.LanguageCode));
                            invalidGiftCardCodeList.Add(giftCardInfo.Code);
                            continue;
                        }
                        if (giftCardInfo.AvailableAmount <= 0m)
                        {
                            giftCardErrorDesc.AppendLine(LanguageHelper.GetText(string.Format("您输入的礼品卡【{0}】余额不足！", giftCardInfo.Code), order.LanguageCode));
                            invalidGiftCardCodeList.Add(giftCardInfo.Code);
                            continue;
                        }
                        order.GiftCardList[index] = giftCardInfo;
                    }
                }
            }

            //去掉没有通过验证的礼品卡
            order.GiftCardList.RemoveAll(g => invalidGiftCardCodeList.Any(giftCardCode => string.IsNullOrWhiteSpace(g.Code) ||
                                                                String.Equals(SafeTrim(g.Code), SafeTrim(giftCardCode), StringComparison.InvariantCultureIgnoreCase)));

            order.GiftCardErrorDesc = giftCardErrorDesc.ToString();
        }

        /// <summary>
        /// 计算余额支付金额
        /// </summary>
        /// <param name="order"></param>
        private void CalculateBalancePayAmount(OrderInfo order)
        {
            //帐户余额不足，将所有子单的余额支付金额清零
            //Modified by PoseidonTong 余额支付不能抵扣关税金额 at [2014-09-19 14:25:27]
            if (order.Customer.AccountBalance < order.BalancePayAmount ||
                order.SOAmount - order.TaxAmount - order.PointPayAmount - order.CouponAmount - order.GiftCardPayAmount <= 0)
            {
                foreach (KeyValuePair<string, OrderInfo> sub in order.SubOrderList)
                {
                    sub.Value.BalancePayAmount = 0m;
                }
            }
            else
            {
                decimal availableBalancePayAmount = order.BalancePayAmount;
                //Modified by PoseidonTong 余额支付不能抵扣关税金额 at [2014-09-19 14:25:27]
                List<OrderInfo> subOrderList = order.SubOrderList.Where(x => (x.Value.SOAmount - x.Value.TaxAmount) > 0m)
                                                                .Select(kvs => kvs.Value)
                                                                .ToList();

                //按照订单金额降序排列，订单金额高的优先使用余额支付
                subOrderList.Sort((subOrder1, subOrder2) =>
                {
                    return -1 * (subOrder1.SOAmount - subOrder1.TaxAmount).CompareTo((subOrder2.SOAmount - subOrder2.TaxAmount));
                });

                //分配订单余额支付金额
                foreach (var subOrder in subOrderList)
                {
                    if (availableBalancePayAmount > 0)
                    {
                        //Modified by PoseidonTong 余额支付不能抵扣关税金额 at [2014-09-19 14:25:27]
                        var canBalancePayAmount = subOrder.SOAmount - subOrder.TaxAmount - subOrder.PointPayAmount
                                                    - subOrder.CouponAmount - subOrder.GiftCardPayAmount;

                        if (canBalancePayAmount > availableBalancePayAmount)
                        {
                            canBalancePayAmount = availableBalancePayAmount;
                        }
                        subOrder.BalancePayAmount = canBalancePayAmount;
                        availableBalancePayAmount -= canBalancePayAmount;
                    }
                    else
                    {
                        subOrder.BalancePayAmount = 0m;
                    }
                }
            }

            //重新计算整单余额支付金额 
            order.BalancePayAmount = order.SubOrderList.Sum(x => x.Value.BalancePayAmount);
        }

        /// <summary>
        /// 计算手续费
        /// </summary>
        /// <param name="order"></param>
        private void CalculateCommissionAmount(OrderInfo order)
        {
            //先计算子单的手续费
            PayTypeInfo payTypeInfo = PipelineDA.GetPayTypeBySysNo(order.PayTypeID);
            if (payTypeInfo != null)
            {
                order.PayTypeName = payTypeInfo.PayTypeName;
            }
            //循环计算每个子单的手续费
            foreach (KeyValuePair<string, OrderInfo> sub in order.SubOrderList)
            {
                payTypeInfo = PipelineDA.GetPayTypeBySysNo(sub.Value.PayTypeID);
                if (payTypeInfo != null)
                {
                    sub.Value.CommissionAmount = decimal.Round(payTypeInfo.PayRate * sub.Value.CashPayAmount, 2, MidpointRounding.AwayFromZero);
                    sub.Value.PayTypeName = payTypeInfo.PayTypeName;
                }
            }

            order.CommissionAmount = order.SubOrderList.Sum(subOrder => subOrder.Value.CommissionAmount);
        }

        /// <summary>
        /// 计算关税
        /// </summary>
        /// <param name="order"></param>
        private void CalculateTrariffAmount(OrderInfo order, Dictionary<string, List<OrderItem>> dicSubOrderItem)
        {
            OrderInfo subOrderInfo;
            List<ProductEntryInfo> allProductEntryInfoList = null;

            List<int> allItemSysNoList = dicSubOrderItem.SelectMany(kvs => kvs.Value.Select(item => item.ProductSysNo))
                                                          .Distinct()
                                                          .ToList();

            if (allItemSysNoList.Count > 0)
            {
                allProductEntryInfoList = PipelineDA.GetProductEntryInfo(allItemSysNoList);
            }

            decimal totalTrariffAmountSum = 0m;
            decimal subOrderTrariffAmountSum = 0m;

            if (allProductEntryInfoList != null && allProductEntryInfoList.Count > 0)
            {
                foreach (var kvs in order.SubOrderList)
                {
                    subOrderInfo = kvs.Value;
                    subOrderTrariffAmountSum = 0m;

                    //循环计算子单中每个item的关税
                    foreach (var item in dicSubOrderItem[kvs.Key])
                    {
                        CalculateOrderItemTrariffAmount(item, order, subOrderInfo, allProductEntryInfoList);

                        subOrderTrariffAmountSum += item.UnitTaxFee * item.UnitQuantity;
                    }

                    //单笔订单关税金额小于50元的免征关税
                    if (subOrderTrariffAmountSum <= 50m)
                    {
                        subOrderTrariffAmountSum = 0m;
                        dicSubOrderItem[kvs.Key].ForEach(item =>
                        {
                            //满足关税免征条件，给出免征提示，并记录下原始关税计算金额
                            item["Tariff-Free-Flag"] = "1";
                            item.UnitTaxFee = 0m;
                        });
                    }
                    kvs.Value.TaxAmount = subOrderTrariffAmountSum;
                    totalTrariffAmountSum += subOrderTrariffAmountSum;
                }
            }

            order.TaxAmount = totalTrariffAmountSum;
        }

        private void CalculateOrderItemTrariffAmount(OrderItem orderItem, OrderInfo mstOrderInfo,
            OrderInfo subOrderInfo, List<ProductEntryInfo> allProductEntryInfoList)
        {
            var curProductEntryInfo = allProductEntryInfoList.Find(x => x.ProductSysNo == orderItem.ProductSysNo);

            if (curProductEntryInfo != null)
            {
                //计算关税时，需要扣除每个item上的促销折扣和优惠券折扣
                orderItem.UnitTaxFee = (orderItem.UnitSalePrice - (decimal)orderItem["UnitDiscountAmt"]
                    //- (decimal)orderItem["UnitCouponAmt"]

                                       ) * curProductEntryInfo.TariffRate;

                //保留两位小数
                orderItem.UnitTaxFee = decimal.Round(orderItem.UnitTaxFee, 2, MidpointRounding.AwayFromZero);
                //原始计算所得关税金额
               // orderItem["Tariff-Original-Amt"] = orderItem.UnitTaxFee;
                //填充其他附加信息
                //orderItem["TariffCode"] = curProductEntryInfo.TariffCode;
                //orderItem["TariffRate"] = curProductEntryInfo.TariffRate;
                //orderItem["EntryRecord"] = curProductEntryInfo.EntryCode;
            }
        }

        /// <summary>
        /// 计算促销折扣金额，主要是进行金额分摊
        /// </summary>
        /// <param name="order"></param>
        private void CalculateDiscountAmount(OrderInfo order)
        {
            if (order.DiscountDetailList == null || order.DiscountDetailList.Count <= 0) return;

            decimal unitDiscountAmt = 0m;

            foreach (var kvs in order.SubOrderList)
            {
                //初始化子单上的折扣列表，通过计算，将折扣信息分摊到每个子单上 
                kvs.Value.DiscountDetailList = new List<OrderItemDiscountInfo>();

                //计算该item总计购买数量，需要遍历主商品列表
                if (kvs.Value.OrderItemGroupList != null)
                {
                    foreach (var itemGroup in kvs.Value.OrderItemGroupList)
                    {
                        if (itemGroup.ProductItemList != null)
                        {
                            itemGroup.ProductItemList.ForEach(orderItem =>
                            {
                                //暂时还没有考虑整单减免的情况。

                                //从订单的折扣明细表找到当前item的折扣信息
                                var discountInfoList = order.DiscountDetailList.FindAll(x => x.ProductSysNo == orderItem.ProductSysNo);
                                //先考虑和主商品相关的促销折扣
                                if (discountInfoList != null)
                                {
                                    //计算item总数量
                                    int totalItemBuyQuantity = 0;
                                    //只考虑主商品折扣
                                    if (order.OrderItemGroupList != null)
                                    {
                                        //计算该主商品购买的总数量
                                        order.OrderItemGroupList.ForEach(g =>
                                                                            {
                                                                                totalItemBuyQuantity += g.ProductItemList.Where(x => x.ProductSysNo == orderItem.ProductSysNo)
                                                                                                                         .Sum(x => x.UnitQuantity * g.Quantity);
                                                                            });
                                    }

                                    //计算item总折扣
                                    decimal totalDiscountAmt = 0m;
                                    foreach (var discountInfo in discountInfoList)
                                    {
                                        //将折扣信息加入到子单中, 按照item平均折扣重新计算UnitDiscount，并将Quantity填为1 
                                        OrderItemDiscountInfo clonedDiscountInfo = (OrderItemDiscountInfo)discountInfo.Clone();
                                        clonedDiscountInfo.Quantity = 1;
                                        clonedDiscountInfo.UnitDiscount = (discountInfo.UnitDiscount * discountInfo.Quantity)
                                                                                                     * orderItem.UnitQuantity / totalItemBuyQuantity;
                                        kvs.Value.DiscountDetailList.Add(clonedDiscountInfo);

                                        //计算单个item的平均折扣，同一个商品A如果既存在与套餐C中，又单独购买过，那么discountInfo中记录的是套餐中该商品享受的折扣
                                        //而单独购买的A不享受折扣，这里需要计算的是单个item的平均折扣，计算的时候需要用item的总折扣除以item的总数量
                                        totalDiscountAmt += (discountInfo.UnitDiscount * discountInfo.Quantity);
                                    }

                                    //计算item平均折扣金额，折扣金额为正数
                                    if (totalItemBuyQuantity > 0)
                                    {
                                        unitDiscountAmt = totalDiscountAmt / totalItemBuyQuantity;
                                        orderItem["UnitDiscountAmt"] = unitDiscountAmt;
                                    }
                                }
                            });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 计算优惠券折扣，主要是进行金额分摊
        /// </summary>
        /// <param name="order"></param>
        private void CalculateCouponAmount(OrderInfo order)
        {
            if (order.CouponAmount <= 0m)
            {
                if (order.SubOrderList != null)
                {
                    foreach (var kvs in order.SubOrderList)
                    {
                        order.CouponAmount += kvs.Value.MerchantCouponAmount;
                        kvs.Value.CouponAmount = kvs.Value.OrderItemGroupList.Sum(g =>
                                                                                        g.ProductItemList.Sum(item => (decimal)item["UnitCouponAmt"] * item.UnitQuantity)
                                                                                 );
                    }
                }
                return;
            }

            decimal totalCouponAmount = order.CouponAmount;
            decimal totalRealSaleAmt = order.SubOrderList.Sum(kvs => kvs.Value.TotalProductAmount - kvs.Value.TotalDiscountAmount);

            //清除掉所有子单上的优惠券使用记录（通过子单克隆继承下来）
            foreach (var kvs in order.SubOrderList)
            {
                //kvs.Value.CouponCode = string.Empty;
                //kvs.Value.CouponCodeSysNo = null;
                //kvs.Value.CouponSysNo = null;
                kvs.Value.CouponAmount = 0m;
            }

            //根据订单商品的总金额对子单进行重新分组，排除掉商品金额为0的订单
            var subOrders = order.SubOrderList.Where(x => x.Value.TotalProductAmount > 0m)
                                                .ToDictionary(k => k.Key, v =>
                                                                                {
                                                                                    if (v.Value.OrderItemGroupList == null)
                                                                                    {
                                                                                        v.Value.OrderItemGroupList = new List<OrderItemGroup>();
                                                                                    }
                                                                                    return v.Value;
                                                                                });

            //根据订单商品的总金额对子单商品进行重新分组，只取主商品
            var subOrderProduct = order.SubOrderList.Where(x => x.Value.TotalProductAmount > 0m)
                                                .Select(x =>
                                                        new
                                                        {
                                                            Key = x.Key,
                                                            Value = x.Value.OrderItemGroupList.SelectMany(y => y.ProductItemList).ToList()
                                                        })
                                                .ToDictionary(k => k.Key, v => v.Value);


            //计算每个子单分摊的优惠券折扣，不能对计算结果截取小数位数，否则会丢失精度
            //每个子单的优惠券分摊金额，取两位小数，最后一个订单用总金额来减
            int subOrderIndex = 0;
            decimal applyedCouponAmt = 0m;
            foreach (var kvs in subOrders)
            {
                //每个子单分摊金额计算需要扣除掉促销折扣部分
                var subOrderRealAmt = kvs.Value.TotalProductAmount - kvs.Value.TotalDiscountAmount;
                decimal subOrderRealCouponAmt = 0m;
                if ((subOrderIndex++) < subOrders.Count - 1)
                {
                    subOrderRealCouponAmt = subOrderRealAmt * (totalCouponAmount / totalRealSaleAmt);
                    subOrderRealCouponAmt = decimal.Round(subOrderRealCouponAmt, 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    subOrderRealCouponAmt = totalCouponAmount - applyedCouponAmt;
                }

                if (subOrderRealAmt - subOrderRealCouponAmt < 0m)
                {
                    subOrderRealCouponAmt = subOrderRealAmt;
                }
                kvs.Value.CouponAmount = subOrderRealCouponAmt;
                applyedCouponAmt += subOrderRealCouponAmt;
            }

            //计算子单下每个Item分摊的优惠券折扣，不能对计算结果截取小数位数，否则会丢失精度
            foreach (var kvs in subOrderProduct)
            {
                var subOrder = subOrders[kvs.Key];

                for (int index = 0; index < kvs.Value.Count; index++)
                {
                    OrderItem item = kvs.Value[index];

                    //每个Item分摊的优惠券折扣需要扣除掉促销折扣部分
                    decimal realItemPrice = item.UnitSalePrice - (decimal)item["UnitDiscountAmt"];
                    decimal unitCouponAmt = (subOrder.CouponAmount * realItemPrice)
                                           / (subOrder.TotalProductAmount - subOrder.TotalDiscountAmount);

                    if (realItemPrice - unitCouponAmt < 0m)
                    {
                        unitCouponAmt = realItemPrice;
                    }
                    //包含先均摊的店铺优惠券金额
                    item["UnitCouponAmt"] = decimal.Parse(item["UnitCouponAmt"].ToString()) + unitCouponAmt;
                }
            }

            decimal realCounponAmt = 0;
            //重新计算真实的优惠券折扣金额
            foreach (var kvs in subOrders)
            {
                kvs.Value.CouponCode = order.CouponCode;
                kvs.Value.CouponCodeSysNo = order.CouponCodeSysNo;
                kvs.Value.CouponSysNo = order.CouponSysNo;
                kvs.Value.CouponAmount = kvs.Value.OrderItemGroupList.Sum(g =>
                                                                                g.ProductItemList.Sum(item => (decimal)item["UnitCouponAmt"] * item.UnitQuantity)
                                                                         );

                realCounponAmt += kvs.Value.CouponAmount;// +kvs.Value.MerchantCouponAmount;
            }
            order.CouponAmount = realCounponAmt;
        }

        private string SafeTrim(string originalString, params char[] trimChars)
        {
            if (string.IsNullOrWhiteSpace(originalString))
            {
                return originalString;
            }
            return originalString.Trim(trimChars);
        }
    }
}
