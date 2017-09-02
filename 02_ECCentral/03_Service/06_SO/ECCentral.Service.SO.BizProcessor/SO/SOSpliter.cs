using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.Service.Utility;
using System.Text;

namespace ECCentral.Service.SO.BizProcessor
{
    /// <summary>
    /// 作废订单
    /// 属性 Parameter 说明:
    /// Parameter[0] : bool , 是否自动拆分订单,默认为true；
    /// </summary>
    [VersionExport(typeof(SOAction), new string[] { "General", "Split" })]
    public class SOSpliter : SOAction
    {
        #region Parameter参数
        /* Parameter说明:
         * Parameter[0] : bool , 是否自动拆分订单
         *
         *
         */
        /// <summary>
        /// 是否强制审核,由 Parameter[0] 传入,默认为false.
        /// </summary>
        protected bool IsAutoSplit
        {
            get
            {
                return GetParameterByIndex<bool>(0);
            }
        }
        #endregion

        private ISODA _soDA;
        protected ISODA SODA
        {
            get
            {
                _soDA = _soDA ?? ObjectFactory<ISODA>.Instance;
                return _soDA;
            }
        }

        /// <summary>
        /// 当前订单编号
        /// </summary>
        protected int SOSysNo
        {
            get
            {
                return CurrentSO.SysNo.Value;
            }
        }

        /// <summary>
        /// 使用的优惠券列表，现在一个订单只能使用一张优惠券，所以列表中只有一条数据，方便以后（以前有人提过一个订单可以使用多张优惠券）可以使用多张优惠券所以用列表表示
        /// </summary>
        private List<SOItemInfo> CouponList
        {
            get
            {
                return CurrentSO.Items.FindAll(item =>
                {
                    return item.ProductType == SOProductType.Coupon;
                });
            }
        }
        private SOIncomeInfo _currentSOIncome;
        SOIncomeInfo CurrentSOIncome
        {
            get
            {
                _currentSOIncome = _currentSOIncome ?? ExternalDomainBroker.GetValidSOIncomeInfo(SOSysNo, SOIncomeOrderType.SO);
                return _currentSOIncome;
            }
        }

        private bool? _isPostShipTypeSysNo;
        protected bool IsPostShip
        {
            get
            {
                _isPostShipTypeSysNo = _isPostShipTypeSysNo ?? CurrentSO.ShippingInfo.ShipTypeSysNo == ExternalDomainBroker.ChinaPostShipTypeID(CurrentSO.CompanyCode) && CurrentSO.ShippingInfo.PostInfo != null;
                return _isPostShipTypeSysNo.Value;
            }
        }

        private SOHolder _holder;
        /// <summary>
        /// 当前订单的锁定操作
        /// </summary>
        private SOHolder Holder
        {
            get
            {
                _holder = _holder ?? ObjectFactory<SOHolder>.Instance;
                _holder.CurrentSO = CurrentSO;
                return _holder;
            }
        }
        public List<SOInfo> SubSOList
        {
            get;
            set;
        }

        public override void Do()
        {
            Split();
        }

        private BizEntity.Common.PayType _currentSOPayType;
        /// <summary>
        /// 当前订单的支付类型
        /// </summary>
        private BizEntity.Common.PayType CurrentSOPayType
        {
            get
            {
                _currentSOPayType = _currentSOPayType ?? ExternalDomainBroker.GetPayTypeBySysNo(CurrentSO.BaseInfo.PayTypeSysNo.Value);
                return _currentSOPayType;
            }
        }
        /// <summary>
        /// 计算现金支付,需要先计算总金额，Promotion，积分。
        /// soInfo.SOMaster.CashPay
        /// </summary>
        /// <param name="soInfo"></param>
        public void CalcCashPay(SOInfo soInfo)
        {
            soInfo.BaseInfo.CouponAmount = UtilityHelper.ToMoney(soInfo.Items.Sum(item => item.CouponAmount));
            soInfo.BaseInfo.PointPayAmount = UtilityHelper.ToMoney(Convert.ToDecimal(soInfo.BaseInfo.PointPay) / ExternalDomainBroker.GetPointToMoneyRatio());
        }
        private void Precheck()
        {
            if (CurrentSO.BaseInfo.HoldStatus == SOHoldStatus.WebHold)
            {
                BizExceptionHelper.Throw("SO_Split_WebHold");
            }

            if (CurrentSO.BaseInfo.SplitType != SOSplitType.Force && CurrentSO.BaseInfo.SplitType != SOSplitType.Customer)
            {
                BizExceptionHelper.Throw("SO_Split_NotAllow");
            }

            switch (CurrentSO.BaseInfo.Status.Value)
            {
                case SOStatus.Origin:
                    break;
                case SOStatus.Split:
                    BizExceptionHelper.Throw("SO_Split_SOStatus_Splited");
                    break;
                case SOStatus.Abandon:
                    BizExceptionHelper.Throw("SO_Split_SOStatus_Abandon");
                    break;
                default:
                    BizExceptionHelper.Throw("SO_Split_SOStatus_NotOriginal");
                    break;
            }

            if (CurrentSO.BaseInfo.SOType == SOType.GroupBuy)
            {
                if (CurrentSO.BaseInfo.SettlementStatus == SettlementStatus.PlanFail)
                {
                    BizExceptionHelper.Throw("SO_Split_GroupBuy_PlanFail");
                }
                else if (CurrentSO.BaseInfo.SettlementStatus == SettlementStatus.Fail)
                {
                    BizExceptionHelper.Throw("SO_Split_GroupBuy_Fail");
                }
                else if (CurrentSO.BaseInfo.SettlementStatus != SettlementStatus.Success)
                {
                    BizExceptionHelper.Throw("SO_Split_GroupBuy_NotSuccess");
                }
            }

            //检验支付方式
            if (!CurrentSO.BaseInfo.PayWhenReceived.Value && CurrentSOIncome == null)
            {
                BizExceptionHelper.Throw("SO_Split_NotExistSOIncome");
            }
        }

        private SOInfo CreateSubSO()
        {
            SOInfo subSOInfo = new SOInfo();
            subSOInfo.BaseInfo = SerializationUtility.DeepClone<SOBaseInfo>(CurrentSO.BaseInfo);
            subSOInfo.ClientInfo = SerializationUtility.DeepClone<SOClientInfo>(CurrentSO.ClientInfo);
            subSOInfo.FPInfo = SerializationUtility.DeepClone<SOFPInfo>(CurrentSO.FPInfo);
            //添加增值税发票信息到每个仓库
            subSOInfo.InvoiceInfo = SerializationUtility.DeepClone<SOInvoiceInfo>(CurrentSO.InvoiceInfo);
            subSOInfo.ReceiverInfo = SerializationUtility.DeepClone<SOReceiverInfo>(CurrentSO.ReceiverInfo);
            subSOInfo.ShippingInfo = SerializationUtility.DeepClone<SOShippingInfo>(CurrentSO.ShippingInfo);

            subSOInfo.CompanyCode = CurrentSO.BaseInfo.CompanyCode;
            subSOInfo.Merchant = CurrentSO.Merchant;
            subSOInfo.WebChannel = CurrentSO.WebChannel;
            //提前生成子单号
            int subSOSysNo = SODA.NewSOSysNo();
            subSOInfo.SysNo = subSOSysNo;
            subSOInfo.BaseInfo.SOID = subSOSysNo.ToString();
            subSOInfo.BaseInfo.CreateTime = DateTime.Now;
            subSOInfo.BaseInfo.SplitType = SOSplitType.SubSO;
            subSOInfo.BaseInfo.SOSplitMaster = SOSysNo;
            subSOInfo.ShippingInfo.IsCombine = null;
            return subSOInfo;
        }

        /// <summary>
        /// 传入参数为已经拆分好的子订单
        /// </summary>
        protected event Action<SOInfo> SubSOAssign;

        public SOSpliter()
        {
        }

        public virtual void Split()
        {
            SubSOList = new List<SOInfo>();
            //  1.  效验是否满足基本拆分条件
            Precheck();

            //拆当前订单
            SplitCurrentSO();

            // 子订单分摊计算
            CalculateSubSO(SubSOList);

            #region 保存订单信息到数据库

            // 保存订单信息到数据库 
            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                #region 保存主订单信息
                SOStatusChangeInfo statusChangeInfo = new SOStatusChangeInfo
                {
                    ChangeTime = DateTime.Now,
                    OldStatus = CurrentSO.BaseInfo.Status.Value,
                    OperatorSysNo = ServiceContext.Current.UserSysNo,
                    OperatorType = SOOperatorType.User,
                    Status = SOStatus.Split,
                    SOSysNo = CurrentSO.SysNo
                };
                //更新主单状态
                if (!SODA.UpdateSOStatusToSplit(statusChangeInfo))
                    BizExceptionHelper.Throw("SO_Split_SOStatus_NotOriginal");
                //更新主单标记为已拆分
                CurrentSO.BaseInfo.Status = SOStatus.Split;
                #endregion

                //插入子单数据到数据库
                foreach (SOInfo subSOInfo in SubSOList)
                {
                    SaveSubSO(subSOInfo);
                }
                #region 调整订单相关日志
                //调整礼品卡使用日志
                List<SOBaseInfo> subSOBaseInfoList = SubSOList.Select<SOInfo, SOBaseInfo>(subSOInfo => subSOInfo.BaseInfo).ToList();
                if (CurrentSO.SOGiftCardList.Count > 0)
                {
                    Dictionary<int, List<ECCentral.BizEntity.IM.GiftCardRedeemLog>> subSOGiftCardDictionary = new Dictionary<int, List<ECCentral.BizEntity.IM.GiftCardRedeemLog>>();
                    SubSOList.ForEach(subSOInfo =>
                    {
                        subSOGiftCardDictionary.Add(subSOInfo.SysNo.Value, subSOInfo.SOGiftCardList);
                    });
                    ExternalDomainBroker.SplitSOGiftCard(CurrentSO.BaseInfo, subSOGiftCardDictionary);
                }

                //调整积分
                //调整余额支付
                //作废主单积分日志
                if (CurrentSO.BaseInfo.PointPay > 0)
                {
                    ExternalDomainBroker.SplitSOPoint(CurrentSO.BaseInfo, subSOBaseInfoList);
                }

                //作废主单余额日志
                if (CurrentSO.BaseInfo.PrepayAmount > 0)
                {
                    ExternalDomainBroker.AdjustPrePay(new BizEntity.Customer.CustomerPrepayLog
                    {
                        AdjustAmount = CurrentSO.BaseInfo.PrepayAmount,
                        PrepayType = ECCentral.BizEntity.Customer.PrepayType.SOPay,
                        SOSysNo = SOSysNo,
                        Note = "拆分订单，作废主订单余额支付",
                        CustomerSysNo = CurrentSO.BaseInfo.CustomerSysNo
                    });

                    SubSOList.ForEach(subSOInfo =>
                    {
                        if (subSOInfo.BaseInfo.PrepayAmount.Value != 0M)
                        {
                            ExternalDomainBroker.AdjustPrePay(new BizEntity.Customer.CustomerPrepayLog
                            {
                                AdjustAmount = -subSOInfo.BaseInfo.PrepayAmount,
                                PrepayType = ECCentral.BizEntity.Customer.PrepayType.SOPay,
                                SOSysNo = SOSysNo,
                                Note = "拆分订单，子订单使用余额支付",
                                CustomerSysNo= subSOInfo.BaseInfo.CustomerSysNo
                            });
                        }
                    });
                }

                //如果是款到发货订单，拆分NetPay
                if (!CurrentSO.BaseInfo.PayWhenReceived.Value)
                {
                    ExternalDomainBroker.SplitSOIncome(CurrentSO.BaseInfo, subSOBaseInfoList);
                }
                #endregion
                scope.Complete();
            }
            #endregion
            //给客户发拆分邮件
            ObjectFactory<SOSendMessageProcessor>.Instance.SendSplitSOEmail(CurrentSO, SubSOList);

            if (IsAutoSplit)
            {
                foreach (SOInfo subSO in SubSOList)
                {
                    SOAction soAction = SOActionFactory.Create(new SOCommandInfo { Command = SOCommand.Audit, SOInfo = subSO });
                    if (soAction is SOAudit)
                    {
                        (soAction as SOAudit).SendMessage();
                    }
                }
            }
            //记录增票更改日志记录
            if (CurrentSO.InvoiceInfo.IsVAT.Value)
            {
                WriteSOChangeVATLog(SubSOList);
            }

            //记日志
            WriteLog();
        }
        /// <summary>
        /// 拆分当前订单
        /// </summary>
        private void SplitCurrentSO()
        {
            #region 拆分订单
            List<SOItemInfo> couponList = new List<SOItemInfo>();//订单使用的优惠券
            List<SOItemInfo> extendWarrentyList = new List<SOItemInfo>(); //订单的所有延保
            List<int> stockSysNoList = new List<int>();
            CurrentSO.Items.ForEach(item =>
            {
                switch (item.ProductType.Value)
                {
                    case SOProductType.Product:
                    case SOProductType.Gift:
                    case SOProductType.Award:
                    case SOProductType.Accessory:
                    case SOProductType.SelfGift:
                        {
                            if (!stockSysNoList.Exists(stockSysNo => { return stockSysNo == item.StockSysNo && item.StockSysNo != null; }))
                            {
                                stockSysNoList.Add(item.StockSysNo.Value);
                            }
                            break;
                        }
                    case SOProductType.Coupon:
                        couponList.Add(item);
                        break;
                    case SOProductType.ExtendWarranty:
                        extendWarrentyList.Add(item);
                        break;
                }
            });

            if (stockSysNoList.Count < 2)
            {
                BizExceptionHelper.Throw("SO_Split_NotSplit");
                return;
            }

            foreach (int stockSysNo in stockSysNoList)
            {
                SOInfo soInfo = CreateSubSO();

                //取得子订单的商品
                soInfo.Items = SerializationUtility.DeepClone<List<SOItemInfo>>(CurrentSO.Items.FindAll(item => item.StockSysNo == stockSysNo));

                //修改ItemsExtenstion
                if(soInfo.Items!=null && soInfo.Items.Count>0)
                {
                    foreach (var item in soInfo.Items)
                    {
                        if(item.ItemExtList!=null && item.ItemExtList.Count>0)
                            item.ItemExtList.ForEach(x => { x.SOSysNo = soInfo.SysNo.Value; });

                    }

                }


                //添加优惠券到每个仓库
                if (couponList != null && couponList.Count > 0)
                {
                    soInfo.Items.AddRange(SerializationUtility.DeepClone(couponList));
                }

                //延保跟随主商品
                extendWarrentyList.ForEach(item =>
                {
                    if (soInfo.Items.Exists(o => o.ProductSysNo == int.Parse(item.MasterProductSysNo)))
                    {
                        soInfo.Items.Add(SerializationUtility.DeepClone<SOItemInfo>(item));
                    }
                });

                //计算总金额
                soInfo.BaseInfo.SOAmount = UtilityHelper.ToMoney(soInfo.Items.Sum<SOItemInfo>(item =>
                {
                    if (item.ProductType == SOProductType.Product || item.ProductType == SOProductType.ExtendWarranty)
                    {
                        return item.OriginalPrice.Value * item.Quantity.Value;
                    }
                    return 0;
                }));
                //soInfo.ShippingInfo.DeliveryFrequency = SetDeliveryType(soInfo);//设置订单的配置频率
                SetSOCCheckShippingSettlementStatus(soInfo);
                SubSOList.Add(soInfo);
            }
            #endregion
        }

        /// <summary>
        /// 分摊子订单
        /// </summary>
        /// <param name="subSOList"></param>
        private void CalculateSubSO(List<SOInfo> subSOList)
        {
            // 按金额升序排列，保证分摊的最后订单的金额最大，减少分摊误差。
            subSOList.Sort((o1, o2) => { return o1.BaseInfo.SOAmount.Value.CompareTo(o2.BaseInfo.SOAmount.Value); });

            decimal masterSOTotalPrice = CurrentSO.BaseInfo.PromotionAmount.Value; //计算金额基数
            decimal masterSOTotalWeight = 0;  //计算重量基数
            CurrentSO.Items.ForEach(item =>
            {
                masterSOTotalPrice += item.Quantity.Value * item.OriginalPrice.Value;
                masterSOTotalWeight += item.Quantity.Value * item.Weight.Value;
            });
            #region 分摊主的费用到子订单

            int t_PointPay = CurrentSO.BaseInfo.PointPay.Value;
            decimal t_ShipPrice = CurrentSO.BaseInfo.ShipPrice.Value;
            decimal t_PayPrice = CurrentSO.BaseInfo.PayPrice.Value;
            decimal t_PremiumAmt = CurrentSO.BaseInfo.PremiumAmount.Value;
            decimal t_PrepayAmt = CurrentSO.BaseInfo.PrepayAmount.Value;
            decimal t_PromotionValue = CurrentSO.BaseInfo.CouponAmount.Value;
            decimal t_GiftCardPay = CurrentSO.BaseInfo.GiftCardPay.Value;

            decimal? t_OriginShipPrice = CurrentSO.ShippingInfo.OriginShipPrice ?? 0;
            decimal? t_PackageFee = CurrentSO.ShippingInfo.PackageFee ?? 0;
            decimal? t_RegisteredFee = CurrentSO.ShippingInfo.RegisteredFee ?? 0;
            decimal? t_ShippingFee = CurrentSO.ShippingInfo.ShippingFee ?? 0;
            decimal? t_Weight3PL = CurrentSO.ShippingInfo.Weight3PL ?? 0;
            decimal? t_WeightSO = CurrentSO.ShippingInfo.Weight ?? 0;

            int subSOIndex = 0;
            foreach (SOInfo soInfo in subSOList)
            {
                bool isLastSubSO = ++subSOIndex == subSOList.Count;
                decimal subSOWeight = 0; //订单总重量
                decimal subSOTotalPrice = 0;//订单去优惠券折扣后的总价

                soInfo.BaseInfo.PromotionAmount = 0;
                soInfo.BaseInfo.GainPoint = 0;
                soInfo.BaseInfo.CouponAmount = 0;

                soInfo.Items.ForEach(item =>
                {
                    item.SOSysNo = soInfo.SysNo;
                    soInfo.BaseInfo.PromotionAmount += item.PromotionAmount.Value;//计算销售规则拆扣
                    soInfo.BaseInfo.GainPoint += item.GainPoint;//计算获得的积分
                    soInfo.BaseInfo.CouponAmount += item.CouponAmount; //计算优惠券折扣

                    subSOWeight += item.Weight.Value * item.Quantity.Value;
                    //价格引用Price，而不是OriginalPrice，因为总价中不包含优惠券折扣。
                    subSOTotalPrice += item.Quantity.Value * item.Price.Value + item.PromotionAmount.Value;
                });

                soInfo.ShippingInfo.Weight = subSOWeight;

                #region 分摊订单费用
                decimal weightRate = subSOWeight / (masterSOTotalWeight <= 0 ? 1 : masterSOTotalWeight); //重量分摊比例
                decimal priceRate = subSOTotalPrice / (masterSOTotalPrice <= 0 ? 1 : masterSOTotalPrice); //金额分摊比例
              
                if (!isLastSubSO) //  不是最后一个子订单
                {
                    //根据重量来分摊运费
                    soInfo.BaseInfo.ShipPrice = UtilityHelper.ToMoney(CurrentSO.BaseInfo.ShipPrice.Value * weightRate);
                    t_ShipPrice -= soInfo.BaseInfo.ShipPrice.Value;

                    //根据价格来分摊积分支付
                    soInfo.BaseInfo.PointPay = (int)(Math.Round(CurrentSO.BaseInfo.PointPay.Value * priceRate));
                    t_PointPay -= soInfo.BaseInfo.PointPay.Value;
                    soInfo.BaseInfo.PointPayAmount = Convert.ToDecimal(soInfo.BaseInfo.PointPay) / ExternalDomainBroker.GetPointToMoneyRatio(); //计算积分支付

                    //根据价格来分摊手续费
                    soInfo.BaseInfo.PayPrice = UtilityHelper.ToMoney(CurrentSO.BaseInfo.PayPrice.Value * priceRate);
                    t_PayPrice -= soInfo.BaseInfo.PayPrice.Value;

                    //根据价格来分摊积保价费
                    soInfo.BaseInfo.PremiumAmount = UtilityHelper.ToMoney(CurrentSO.BaseInfo.PremiumAmount.Value * priceRate);
                    t_PremiumAmt -= soInfo.BaseInfo.PremiumAmount.Value;

                    
                    //余额支付的分摊比例
                    decimal priceRate_Prepay = soInfo.BaseInfo.SOTotalAmount / (CurrentSO.BaseInfo.SOTotalAmount <= 0 ? 1 : CurrentSO.BaseInfo.SOTotalAmount);
                    //根据商品总价分摊余额支付
                    soInfo.BaseInfo.PrepayAmount = UtilityHelper.ToMoney(CurrentSO.BaseInfo.PrepayAmount.Value * priceRate_Prepay);//分摊余额支付
                    t_PrepayAmt -= soInfo.BaseInfo.PrepayAmount.Value;
                    //根据商品总价分摊礼品卡支付
                    soInfo.BaseInfo.GiftCardPay = UtilityHelper.ToMoney(CurrentSO.BaseInfo.GiftCardPay.Value * priceRate_Prepay);//分摊礼品支付
                    t_GiftCardPay -= soInfo.BaseInfo.GiftCardPay.Value;
                }
                else //  最后一个子订单
                {
                    soInfo.BaseInfo.ShipPrice = UtilityHelper.ToMoney(t_ShipPrice);
                    soInfo.BaseInfo.PointPay = t_PointPay;
                    soInfo.BaseInfo.PointPayAmount = Convert.ToDecimal(soInfo.BaseInfo.PointPay) / ExternalDomainBroker.GetPointToMoneyRatio(); //计算积分支付
                    soInfo.BaseInfo.PayPrice = UtilityHelper.ToMoney(t_PayPrice);
                    soInfo.BaseInfo.PremiumAmount = UtilityHelper.ToMoney(t_PremiumAmt);

                    //余额支付的分摊比例
                    decimal priceRate_Prepay = soInfo.BaseInfo.SOTotalAmount / (CurrentSO.BaseInfo.SOTotalAmount <= 0 ? 1 : CurrentSO.BaseInfo.SOTotalAmount);
                    soInfo.BaseInfo.PrepayAmount = UtilityHelper.ToMoney(t_PrepayAmt);
                    soInfo.BaseInfo.GiftCardPay = UtilityHelper.ToMoney(t_GiftCardPay);
                }

                if (CurrentSO.SOGiftCardList.Count > 0)
                {
                    decimal itemGiftCardPay = soInfo.BaseInfo.GiftCardPay.Value;
                    foreach (ECCentral.BizEntity.IM.GiftCardRedeemLog giftCard in CurrentSO.SOGiftCardList)
                    {
                        if (itemGiftCardPay <= 0) break;
                        if (giftCard.Amount <= 0) continue;
                        if (giftCard.Amount >= itemGiftCardPay)
                        {
                            giftCard.Amount -= itemGiftCardPay;
                            soInfo.SOGiftCardList.Add(new ECCentral.BizEntity.IM.GiftCardRedeemLog
                            {
                                Code = giftCard.Code,
                                Amount = itemGiftCardPay
                            });
                            itemGiftCardPay = 0;
                            break;
                        }
                        else
                        {
                            itemGiftCardPay -= giftCard.Amount.Value;
                            soInfo.SOGiftCardList.Add(new ECCentral.BizEntity.IM.GiftCardRedeemLog
                            {
                                Code = giftCard.Code,
                                Amount = giftCard.Amount
                            });
                            giftCard.Amount = 0;
                        }
                        if (itemGiftCardPay == 0M)
                        {
                            break;
                        }
                    }
                }


                if (IsAutoSplit)
                {
                    soInfo.BaseInfo.Status = SOStatus.WaitingOutStock;//拆分后子单为待出库状态
                }
                else
                {
                    soInfo.BaseInfo.Status = SOStatus.Origin;//拆分后子单为待审核状态
                }
                //拆单后需要重新计算每个子单是否是大件
                soInfo.BaseInfo.IsLarge = SOCommon.ValidateIsLarge(soInfo.ShippingInfo.Weight.Value); // 是否大件商品

                if (!isLastSubSO)
                {
                    soInfo.ShippingInfo.OriginShipPrice = UtilityHelper.ToMoney((CurrentSO.ShippingInfo.OriginShipPrice ?? 0) * weightRate);
                    t_OriginShipPrice -= soInfo.ShippingInfo.OriginShipPrice;

                    soInfo.ShippingInfo.PackageFee = UtilityHelper.ToMoney((CurrentSO.ShippingInfo.PackageFee ?? 0) * weightRate);
                    t_PackageFee -= soInfo.ShippingInfo.PackageFee;

                    soInfo.ShippingInfo.RegisteredFee = UtilityHelper.ToMoney((CurrentSO.ShippingInfo.RegisteredFee ?? 0) * weightRate);
                    t_RegisteredFee -= soInfo.ShippingInfo.RegisteredFee;

                    soInfo.ShippingInfo.ShippingFee = UtilityHelper.ToMoney((CurrentSO.ShippingInfo.ShippingFee ?? 0) * weightRate);
                    t_ShippingFee -= soInfo.ShippingInfo.ShippingFee;

                    soInfo.ShippingInfo.Weight3PL = (int)((CurrentSO.ShippingInfo.Weight3PL ?? 0) * weightRate);
                    t_Weight3PL -= soInfo.ShippingInfo.Weight3PL;

                    soInfo.ShippingInfo.Weight = (int)((CurrentSO.ShippingInfo.Weight ?? 0) * weightRate);
                    t_WeightSO -= soInfo.ShippingInfo.Weight;
                }
                else
                {
                    soInfo.ShippingInfo.OriginShipPrice = t_OriginShipPrice;
                    soInfo.ShippingInfo.PackageFee = t_PackageFee;
                    soInfo.ShippingInfo.RegisteredFee = t_RegisteredFee;
                    soInfo.ShippingInfo.ShippingFee = t_ShippingFee;
                    soInfo.ShippingInfo.Weight3PL = t_Weight3PL;
                    soInfo.ShippingInfo.Weight = t_WeightSO;
                }
                #endregion

                AssignSubSOPromotion(soInfo);
                AssignSubSOInvoice(soInfo);
                if (SubSOAssign != null)
                {
                    SubSOAssign(soInfo);
                }

                List<ItemGrossProfitInfo> gorsses = new List<ItemGrossProfitInfo>();
                foreach (ItemGrossProfitInfo gross in CurrentSO.ItemGrossProfitList)
                {
                    foreach (SOItemInfo item in soInfo.Items)
                    {
                        if (gross.ProductSysNo == item.ProductSysNo)
                        {
                            ItemGrossProfitInfo subgross = SerializationUtility.DeepClone(gross);
                            subgross.SOSysNo = soInfo.SysNo.Value;
                            soInfo.ItemGrossProfitList.Add(subgross);
                        }
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// 子订单促销活动分摊
        /// </summary>
        /// <param name="masterSOInfo"></param>
        /// <param name="subSOInfo"></param>
        private void AssignSubSOPromotion(SOInfo subSOInfo)
        {
            #region 拆分订单的促销活动.
            //拆分订单的促销活动.
            if (CurrentSO.SOPromotions != null && CurrentSO.SOPromotions.Count > 0)
            {
                CurrentSO.SOPromotions.ForEach(promotion =>
                {
                    SOPromotionInfo promotionInfo = SerializationUtility.DeepClone(promotion);

                    promotionInfo.SOSysNo = subSOInfo.SysNo;
                    switch (promotion.PromotionType)
                    {
                        case SOPromotionType.Combo:
                            break;
                        case SOPromotionType.Coupon:
                            if (promotionInfo.SOPromotionDetails != null && promotionInfo.SOPromotionDetails.Count > 0)
                            {
                                //移出主商品不在子订单中的促销明细
                                promotionInfo.SOPromotionDetails.RemoveAll(d =>
                                {
                                    return !subSOInfo.Items.Exists(subSOItem =>
                                    {
                                        return subSOItem.ProductSysNo == d.MasterProductSysNo;
                                    });
                                });
                                //计算子订单的总折扣
                                promotionInfo.DiscountAmount = promotionInfo.SOPromotionDetails.Sum<SOPromotionDetailInfo>(d => d.DiscountAmount);
                            }
                            if (promotionInfo.DiscountAmount == 0)
                            {
                                promotionInfo = null;
                            }
                            break;
                        case SOPromotionType.SelfGift:
                            //子订单中是否包含有赠品,没有就将促销设置为null.
                            if (!promotionInfo.GiftList.Exists(g =>
                            {
                                return subSOInfo.Items.Exists(item =>
                                {
                                    return item.ProductType == SOProductType.SelfGift && g.ProductSysNo == item.ProductSysNo;
                                });
                            }))
                            {
                                promotionInfo = null;
                            }
                            break;
                        case SOPromotionType.VendorGift:
                        case SOPromotionType.Accessory:
                            //子订单是否包含附件或厂商赠品,如果没有就设置为null
                            if (promotionInfo.SOPromotionDetails != null && promotionInfo.GiftList.Count > 0)
                            {
                                //移出不在子订单中的附件
                                promotionInfo.GiftList.RemoveAll(g =>
                                {
                                    return !subSOInfo.Items.Exists(subSOItem =>
                                    {
                                        return promotion.PromotionType == SOPromotionType.Accessory ?
                                            (subSOItem.ProductSysNo == g.ProductSysNo && subSOItem.ProductType == SOProductType.Accessory) :
                                            (subSOItem.ProductSysNo == g.ProductSysNo && subSOItem.ProductType == SOProductType.Gift);
                                    });
                                });
                                ////移出附件不在子订单中的附件明细
                                //if (promotionInfo.GiftList.Count < 1)
                                //{
                                //    promotionInfo.SOPromotionDetails.RemoveAll(d =>
                                //    {
                                //        d.GiftList.RemoveAll(dg =>
                                //        {
                                //            return !promotionInfo.GiftList.Exists(g =>
                                //            {
                                //                return g.ProductSysNo == dg.ProductSysNo;
                                //            });
                                //        });
                                //        return d.GiftList.Count == 0;
                                //    });
                                //}
                            }
                            if (promotionInfo.GiftList.Count == 0)
                            {
                                promotionInfo = null;
                            }
                            break;
                    }
                    if (promotionInfo != null)
                    {
                        subSOInfo.SOPromotions.Add(promotionInfo);
                    }
                });
            }
            #endregion

        }

        /// <summary>
        /// 子订单发票计算
        /// </summary>
        /// <param name="masterSOInfo"></param>
        /// <param name="subSOInfo"></param>
        private void AssignSubSOInvoice(SOInfo subSOInfo)
        {
            //如果子单金额小于等于1，并且开具了增值税发票，则将增票改为普票并记录修改日志
            if (subSOInfo.InvoiceInfo.IsVAT.HasValue && subSOInfo.InvoiceInfo.IsVAT.Value)
            {
                decimal invoiceAmt = SOCommon.CalculateInvoiceAmount(
                    subSOInfo.BaseInfo.CashPay,
                    subSOInfo.BaseInfo.PremiumAmount.Value,
                    subSOInfo.BaseInfo.ShipPrice.Value,
                    subSOInfo.BaseInfo.PayPrice.Value,
                    subSOInfo.BaseInfo.PromotionAmount.Value,
                    subSOInfo.BaseInfo.GiftCardPay.Value,
                    subSOInfo.BaseInfo.PayWhenReceived.Value);
                if (invoiceAmt <= 1M)
                {
                    subSOInfo.InvoiceInfo.IsVAT = false;
                }
            }
        }

        /// <summary>
        /// 保存单个子订单
        /// </summary>
        /// <param name="subSOInfo"></param>
        protected virtual void SaveSubSO(SOInfo subSOInfo)
        {
            if (IsAutoSplit)
            {
                subSOInfo.StatusChangeInfoList = SerializationUtility.DeepClone(CurrentSO.StatusChangeInfoList);
            }

            //保存订单主信息
            SODA.InsertSOMainInfo(subSOInfo);

            //邮政自提
            if (IsPostShip)
            {
                subSOInfo.ShippingInfo.PostInfo.SOSysNo = subSOInfo.SysNo;
                SODA.InsertChinaPost(subSOInfo.ShippingInfo.PostInfo);
            }
            //添加增值税发票信息
            if (subSOInfo.InvoiceInfo != null && subSOInfo.InvoiceInfo.IsVAT.Value && subSOInfo.InvoiceInfo.VATInvoiceInfo != null)
            {
                SODA.UpdateSOVATInvoice(subSOInfo.InvoiceInfo.VATInvoiceInfo);
            }

            SODA.InsertSOItemInfo(subSOInfo);

            SODA.InsertSOCheckShippingInfo(subSOInfo);
            //处理赠品信息 2011-8-29
            foreach (SOPromotionInfo promotionInfo in subSOInfo.SOPromotions)
            {
                SODA.InsertSOPromotionInfo(promotionInfo, subSOInfo.CompanyCode);
            }

            //处理毛利分配信息
            foreach (ItemGrossProfitInfo gross in subSOInfo.ItemGrossProfitList)
            {
                SODA.InsertSOItemGossProfit(gross);
            }
        }

        private void WriteSOChangeVATLog(List<SOInfo> subSOList)
        {
            if (subSOList != null && subSOList.Count > 0)
            {
                foreach (SOInfo subSO in subSOList)
                {
                    if (!subSO.InvoiceInfo.IsVAT.Value)
                    {
                        ECCentral.BizEntity.SO.SOInvoiceChangeLogInfo invoiceChanageLog = new SOInvoiceChangeLogInfo
                        {
                            SOSysNo = subSO.SysNo.Value,
                            ChangeTime = DateTime.Now,
                            ChangeType = InvoiceChangeType.VATToGeneral,
                            CompanyCode = subSO.CouponCode,
                            UserSysNo = ServiceContext.Current.UserSysNo,
                            Note = ResourceHelper.Get("Res_SO_Invoice_Change", subSO.InvoiceInfo.VATInvoiceInfo.TaxNumber, subSO.InvoiceInfo.InvoiceNo)
                        };
                        ObjectFactory<ISOLogDA>.Instance.InsertSOInvoiceChangeLogInfo(invoiceChanageLog);
                    }
                }
            }
        }

        /// <summary>
        /// 重新计算团购状态
        /// </summary>
        /// <param name="soInfo"></param>
        private void SetSOCCheckShippingSettlementStatus(SOInfo soInfo)
        {
            if (soInfo.BaseInfo.SettlementStatus == SettlementStatus.PlanFail)
            {
                foreach (SOItemInfo item in soInfo.Items)
                {
                    if (!item.ReferenceSysNo.HasValue || item.ReferenceSysNo.Value == 0)
                    {
                        continue;
                    } //如果检测到有团购失败的商品，则不更改状态
                    else if (item.SettlementStatus == SettlementStatus.Fail || item.SettlementStatus == SettlementStatus.PlanFail)
                    {
                        return;
                    }
                }
                //如果没有检测到团购失败的商品
                soInfo.BaseInfo.SettlementStatus = SettlementStatus.Success;
            }
        }

        private void WriteLog()
        {
            string subSONoList = string.Join(","
                , SubSOList.Select(item => item.SysNo.ToString()).ToArray());

            string logContent = ResourceHelper.Get("Res_SO_Split_Success", CurrentSO.SysNo, subSONoList);

            base.WriteLog(ECCentral.BizEntity.Common.BizLogType.Sale_SO_Update, logContent);
        }
    }

    [VersionExport(typeof(SOAction), new string[] { "General", "CancelSplit" })]
    public class CancelSOSpliter : SOAction
    {
        private ISODA _soDA;
        protected ISODA SODA
        {
            get
            {
                _soDA = _soDA ?? ObjectFactory<ISODA>.Instance;
                return _soDA;
            }
        }

        /// <summary>
        /// 当前订单编号
        /// </summary>
        protected int SOSysNo
        {
            get
            {
                return CurrentSO.SysNo.Value;
            }
        }

        /// <summary>
        /// 使用的优惠券列表，现在一个订单只能使用一张优惠券，所以列表中只有一条数据，方便以后（以前有人提过一个订单可以使用多张优惠券）可以使用多张优惠券所以用列表表示
        /// </summary>
        private List<SOItemInfo> CouponList
        {
            get
            {
                return CurrentSO.Items.FindAll(item =>
                {
                    return item.ProductType == SOProductType.Coupon;
                });
            }
        }
        private SOIncomeInfo _currentSOIncome;
        SOIncomeInfo CurrentSOIncome
        {
            get
            {
                _currentSOIncome = _currentSOIncome ?? ExternalDomainBroker.GetValidSOIncomeInfo(SOSysNo, SOIncomeOrderType.SO);
                return _currentSOIncome;
            }
        }

        private SOHolder _holder;
        /// <summary>
        /// 当前订单的锁定操作
        /// </summary>
        private SOHolder Holder
        {
            get
            {
                _holder = _holder ?? ObjectFactory<SOHolder>.Instance;
                _holder.CurrentSO = CurrentSO;
                return _holder;
            }
        }
        public List<SOInfo> SubSOList
        {
            get;
            set;
        }
        public override void Do()
        {
            RestoreSOMaster();
        }

        #region 恢复主单为待审核状态，并作废所有子单

        /// <summary>
        /// 恢复主单为待审核状态，并作废所有子单
        /// </summary>
        /// <param name="soSysNo">子单系统编号</param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public void RestoreSOMaster()
        {
            SOInfo masterSO = CurrentSO;
            //查询所有子单
            List<SOInfo> subSOList =ObjectFactory<SOProcessor>.Instance.GetSubSOByMasterSOSysNo(CurrentSO.SysNo.Value);

            if (subSOList == null && subSOList.Count == 0)
            {
                BizExceptionHelper.Throw("SO_CancelSplit_NotExistSubSO");
                return;
            }

            //检查主单和子单的状态
            foreach (SOInfo subSO in subSOList)
            {
                if (subSO.BaseInfo.Status == SOStatus.Abandon
                    || subSO.BaseInfo.Status == SOStatus.OutStock
                    || subSO.BaseInfo.Status == SOStatus.Split)
                {

                    BizExceptionHelper.Throw("SO_CancelSplit_SubIsOutStock");
                }
            }
            if (masterSO.BaseInfo.Status != SOStatus.Split)
            {
                BizExceptionHelper.Throw("SO_CancelSplit_MasterSOStatusIsChanged");
            }

            List<SOBaseInfo> subSOBaseInfoList = subSOList.Select<SOInfo, SOBaseInfo>(subSOInfo => subSOInfo.BaseInfo).ToList();

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                //重新创建主单积分日志
                if (masterSO.BaseInfo.PointPay > 0)
                {
                    ExternalDomainBroker.CancelSplitSOPoint(masterSO.BaseInfo, subSOBaseInfoList);
                }

                if (masterSO.BaseInfo.PrepayAmount > 0)
                {
                    ExternalDomainBroker.AdjustPrePay(new BizEntity.Customer.CustomerPrepayLog
                    {
                        AdjustAmount = -CurrentSO.BaseInfo.PrepayAmount,
                        PrepayType = ECCentral.BizEntity.Customer.PrepayType.SOPay,
                        SOSysNo = SOSysNo,
                        Note = "Res_SO_CancelSplit_MasterSOPrePay",
                        CustomerSysNo = CurrentSO.BaseInfo.CustomerSysNo
                    });

                    subSOList.ForEach(subSOInfo =>
                    {
                        if (subSOInfo.BaseInfo.PrepayAmount.Value != 0M)
                        {
                            ExternalDomainBroker.AdjustPrePay(new BizEntity.Customer.CustomerPrepayLog
                            {
                                AdjustAmount = subSOInfo.BaseInfo.PrepayAmount,
                                PrepayType = ECCentral.BizEntity.Customer.PrepayType.SOPay,
                                SOSysNo = SOSysNo,
                                Note = "Res_SO_CancelSplit_AbandonSubSOPrePay",
                                CustomerSysNo = subSOInfo.BaseInfo.CustomerSysNo
                            });
                        }
                    });
                }
                if (masterSO.BaseInfo.GiftCardPay > 0)
                {
                    Dictionary<int, List<ECCentral.BizEntity.IM.GiftCardRedeemLog>> subSOGiftCardDictionary = new Dictionary<int, List<ECCentral.BizEntity.IM.GiftCardRedeemLog>>();
                    subSOList.ForEach(subSOInfo =>
                    {
                        subSOGiftCardDictionary.Add(subSOInfo.SysNo.Value, subSOInfo.SOGiftCardList);
                    });
                    ExternalDomainBroker.CancelSplitSOGiftCard(masterSO.BaseInfo, subSOGiftCardDictionary);
                }

                //款到发货订单，恢复主单NetPay记录
                if (!masterSO.BaseInfo.PayWhenReceived.Value)
                {
                    ExternalDomainBroker.CancelSplitSOIncome(masterSO.BaseInfo, subSOBaseInfoList);
                }

                SODA.UpdateSOStatus(new SOStatusChangeInfo
                {
                    ChangeTime = DateTime.Now,
                    OldStatus = masterSO.BaseInfo.Status,
                    Status = SOStatus.Origin,
                    OperatorType = ServiceContext.Current.UserSysNo == 0 ? SOOperatorType.System : SOOperatorType.User,
                    OperatorSysNo = ServiceContext.Current.UserSysNo,
                    SOSysNo = CurrentSO.SysNo
                });
                masterSO.BaseInfo.Status = SOStatus.Origin;
                foreach (SOInfo subSO in subSOList)
                {
                    SODA.UpdateSOStatus(new SOStatusChangeInfo
                    {
                        ChangeTime = DateTime.Now,
                        OldStatus = subSO.BaseInfo.Status,
                        Status = SOStatus.Abandon,
                        OperatorType = ServiceContext.Current.UserSysNo == 0 ? SOOperatorType.System : SOOperatorType.User,
                        OperatorSysNo = ServiceContext.Current.UserSysNo,
                        SOSysNo = subSO.SysNo
                    });
                }

                scope.Complete();
            }

            SendMsgToWMS(subSOList);

        }

        private void SendMsgToWMS(List<SOInfo> subSOList)
        {
            foreach (SOInfo subSO in subSOList)
            {
                if (SODA.WMSIsDownloadSO(subSO.SysNo.Value))
                {
                    ObjectFactory<OPCProcessor>.Instance.SendMessageToWMS(subSO, WMSAction.Abandon, OPCCallBackType.NoneCallBack);
                    WriteLog(BizEntity.Common.BizLogType.Sale_SO_EmployeeAbandon, "Res_SO_Abandon");
                }
            }
        }

        #endregion
    }
}
