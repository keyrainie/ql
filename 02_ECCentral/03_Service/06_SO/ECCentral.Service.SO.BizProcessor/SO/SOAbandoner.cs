using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.SO.IDataAccess;
//using ECCentral.Service.ThirdPart.Interface;
using ECCentral.Service.Utility;

namespace ECCentral.Service.SO.BizProcessor
{
    /// <summary>
    /// 作废订单。属性 Parameter 说明 ：
    /// Parameter[0] : bool , 表示订单作废后订单中商品是否立即返还库存,默认为false；
    /// Parameter[1] : bool , 如果有有效的收款记录是否生成负收款单后再作废订单
    /// Parameter[2] : ECCentral.BizEntity.Invoice.SOIncomeRefundInfo , 负收款信息。
    /// </summary>
    [VersionExport(typeof(SOAction), new string[] { "General", "Abandon" })]
    public class SOAbandoner : SOAction
    {
        #region 公用属性

        #region Parameter参数说明
        /* Parameter说明: 
         * Parameter[0] : bool , 是否立即返还库存
         * Parameter[1] : bool , 如果有有效的收款记录是否生成负收款单后再作废订单。
         * Parameter[2] : ECCentral.BizEntity.Invoice.SOIncomeRefundInfo , 负收款信息。
         * Parameter[3] : bool,是否是批量操作
         */
        /// <summary>
        /// 是否立即返还库存,通过参数Parameter[0] 来确定，默认为false;
        /// </summary>
        protected virtual bool IsImmediatelyReturnStock
        {
            get
            {
                return GetParameterByIndex<bool>(0);
            }
        }

        /// <summary>
        /// 是否立即返还库存,通过参数Parameter[1] 来确定，默认为false;
        /// </summary>
        protected virtual bool IsCreateAOAndAbandonSO
        {
            get
            {
                return GetParameterByIndex<bool>(1);
            }
        }

        /// <summary>
        /// 负收款信息,通过参数Parameter[2] 来确定;
        /// </summary>
        protected virtual SOIncomeRefundInfo SOIncomeRefundInfo
        {
            get
            {
                return GetParameterByIndex<SOIncomeRefundInfo>(2);
            }
        }

        /// <summary>
        /// 是否是批量操作
        /// </summary>
        protected virtual bool IsBatchAbandon
        {
            get
            {
                return GetParameterByIndex<bool>(3);
            }
        }

        #endregion

        protected SOOperatorType OperatorType
        {
            get
            {
                int systemUserSysNo;
                int.TryParse(AppSettingManager.GetSetting(SOConst.DomainName, "SystemUserSysNo"), out systemUserSysNo);
                return ServiceContext.Current.UserSysNo == systemUserSysNo ? SOOperatorType.System : SOOperatorType.User;
            }
        }

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

        private SOIncomeInfo _currentSOIncome;
        protected SOIncomeInfo CurrentSOIncome
        {
            get
            {
                _currentSOIncome = _currentSOIncome ?? ExternalDomainBroker.GetValidSOIncomeInfo(SOSysNo, SOIncomeOrderType.SO);
                return _currentSOIncome;
            }
        }

        private SOIncomeInfo _currentAOIncome;
        protected SOIncomeInfo CurrentAOIncome
        {
            get
            {
                _currentAOIncome = _currentAOIncome ?? ExternalDomainBroker.GetValidSOIncomeInfo(SOSysNo, SOIncomeOrderType.AO);
                return _currentAOIncome;
            }
        }

        private SOHolder _holder;
        /// <summary>
        /// 当前订单的锁定操作
        /// </summary>
        protected SOHolder Holder
        {
            get
            {
                _holder = _holder ?? ObjectFactory<SOHolder>.Instance;
                _holder.CurrentSO = CurrentSO;
                return _holder;
            }
        }

        public bool IsOutStockOrder { get; set; }

        private string _OperationName = "作废订单";
        public string OperationName
        {
            get
            {
                return _OperationName;
            }
            set
            {
                _OperationName = value;
            }
        }


        private BizLogType _OperationType = BizEntity.Common.BizLogType.Sale_SO_EmployeeAbandon;
        public BizLogType OperationType
        {
            get
            {
                return _OperationType;
            }
            set
            {
                _OperationType = value;
            }
        }

        private SOStatus _ToSoStatus = SOStatus.Abandon;
        public SOStatus ToSoStatus
        {
            get
            {
                return _ToSoStatus;
            }
            set
            {
                _ToSoStatus = value;
            }
        }

        private string _Note = "订单作废";
        public string Note
        {
            get
            {
                return _Note;
            }
            set
            {
                _Note = value;
            }
        }

        public int? RefundReason { get; set; }
        #endregion

        public override void Do()
        {
            if (Parameter.Length >= 5)
            {
                ToSoStatus = GetParameterByIndex<SOStatus>(4);
            }
            if (ToSoStatus == SOStatus.Reject)
            {
                RejectAbandonSO();
            }
            else if (ToSoStatus == SOStatus.CustomsReject)
            {
                CustomsAbandonSO();
            }
            else
            {
                if (IsCreateAOAndAbandonSO)
                {
                    CreateAOAndAbandonSO(SOIncomeRefundInfo);
                }
                else
                {
                    Abandon();
                }
            }
        }

        protected void WriteLog(SOInfo soInfo)
        {
            WriteLog(OperationType, OperationName);
        }

        #region 作废的检查
        /// <summary>
        /// 作废订单时检查订单的支付信息
        /// </summary>
        /// <param name="soSysNo"></param>
        protected virtual void AbandonSOIncomeCheck() //IPP3:SOCommonBP.SOIncomeCheck(soInfo.BaseInfo.SystemNumber, soInfo.MessageHeaderInfo.CompanyCode);
        {
            //  1.  检查是否有待审核的网上支付记录,如果存在则要作废或生成负收款
            if (ExternalDomainBroker.IsExistOriginNetPay(SOSysNo))
            {
                BizExceptionHelper.Throw("SO_Abandon_IsExistNetPay");
            }

            //  2.  检查是否有有效的收款记录, 如果存在生成负收款
            if (CurrentSOIncome != null && CurrentAOIncome == null)
            {
                if (CurrentSOIncome.Status == SOIncomeStatus.Confirmed)
                {
                    BizExceptionHelper.Throw("SO_Abandon_ExistSOIncome_Confirmed");
                }
                else if (CurrentSOIncome.Status == SOIncomeStatus.Origin)
                {
                    BizExceptionHelper.Throw("SO_Abandon_ExistSOIncome_Origin");
                }
            }
        }

        //作废规则校验
        protected virtual void ValidateAbandonSO(bool isCheckSOIncome)
        {
            //  1. 订单状态检查： 如果是商家仓储并且是待出库状态不允许作废
            switch (CurrentSO.BaseInfo.Status.Value)
            {
                case SOStatus.Split:
                    BizExceptionHelper.Throw("SO_Abandon_SOStatus_Splited");
                    break;
                case SOStatus.Abandon:
                    BizExceptionHelper.Throw("SO_Abandon_SOStatus_Abandon");
                    break;
                case SOStatus.WaitingOutStock:
                    if (CurrentSO.ShippingInfo.StockType == StockType.MET)
                    {
                        BizExceptionHelper.Throw("SO_Abandon_SOStockIsMET");
                    }
                    break;
            }

            //客户不能作废非待审核状态的订单
            if (CurrentSO.BaseInfo.Status != SOStatus.Origin && OperatorType == SOOperatorType.Customer)
            {
                BizExceptionHelper.Throw("SO_Abandon_CustomerAbandon");
            }

            //批量操作不操作子单
            if (IsBatchAbandon && CurrentSO.BaseInfo.SplitType == SOSplitType.SubSO)
            {
                BizExceptionHelper.Throw("SO_Abandon_SubSO_BatchAbandon");
            }

            //  2.  收款单验证
            if (isCheckSOIncome)
            {
                AbandonSOIncomeCheck();
            }
        }
        #endregion

        #region 作废订单，返还订单相关金额

        /// <summary>
        /// 取消订单的使用优惠券，取消订单商品优惠券的分摊，重新设置商品价格。
        /// 注意：此方法还没有保存到数据库中，是通过 SaveCurrentSO()来保存
        /// </summary>
        protected virtual void CancelCoupon()
        {
            #region 取消优惠券使用

            int couponSysNo = CouponList != null && CouponList.Count > 0 ? CouponList[0].ProductSysNo.Value : 0;
            string couponCode = string.Empty;
            //取消优惠券使用
            if (couponSysNo != 0)
            {
                CurrentSO.BaseInfo.CouponAmount = 0;
                foreach (SOItemInfo item in CurrentSO.Items)
                {
                    item.Price = item.OriginalPrice;
                    item.CouponAverageDiscount = 0;
                }
                CurrentSO.SOPromotions.RemoveAll(p =>
                {
                    return p.PromotionType == SOPromotionType.Coupon;
                });
                if (ExternalDomainBroker.CheckCouponIsValid(couponSysNo, out couponCode))
                {
                    // 作废时把礼券代码临时保存在note信息中。
                    CurrentSO.BaseInfo.Note = CurrentSO.BaseInfo.Note + ";作废前使用的礼券代码=" + couponCode;
                }

                #region 取消优惠券的使用
                if (CurrentSO.BaseInfo.SplitType != SOSplitType.SubSO)
                {
                    int shoppingCartSysNo = SODA.GetShoppingCartSysNoBySOSysNo(SOSysNo);
                    ExternalDomainBroker.CancelCoupon(couponCode, SOSysNo, shoppingCartSysNo);
                }
                #endregion
            }
            #endregion
        }

        /// <summary>
        /// 设置订单为作废状态并保存，同时保存订单商品信息
        /// </summary>
        protected void SaveCurrentSO()
        {
            SOStatusChangeInfo statusChangeInfo = new SOStatusChangeInfo
            {
                ChangeTime = DateTime.Now,
                IsSendMailToCustomer = true,
                OldStatus = CurrentSO.BaseInfo.Status,
                OperatorSysNo = ServiceContext.Current.UserSysNo,
                OperatorType = OperatorType,
                SOSysNo = SOSysNo,
                Status = ToSoStatus
            };

            if (OperatorType == SOOperatorType.System)
            {
                CurrentSO.BaseInfo.Note = statusChangeInfo.Note = Note;
            }

            CurrentSO.BaseInfo.Status = ToSoStatus;
            CurrentSO.StatusChangeInfoList = CurrentSO.StatusChangeInfoList ?? new List<SOStatusChangeInfo>();
            CurrentSO.StatusChangeInfoList.Add(statusChangeInfo);
            if (!SODA.UpdateSOForAbandon(CurrentSO))
            {
                BizExceptionHelper.Throw("SO_Abandon_SOStatus_Abandon");
            }

            //更新订单商品信息，是否要从订单商品中删除优惠券的记录？
            foreach (SOItemInfo item in CurrentSO.Items)
            {
                if (item.ProductType != SOProductType.Coupon)
                {
                    SODA.UpdateSOItemAmountInfo(item);
                }
            }
        }

        /// <summary>
        /// 调整仓库，返还库存
        /// </summary>
        protected virtual void AdjustInventory()
        {
            List<BizEntity.Inventory.InventoryAdjustItemInfo> adjustItemList = new List<BizEntity.Inventory.InventoryAdjustItemInfo>();
            SOProcessor sopro = new SOProcessor();
            DateTime? SOCreatDate = CurrentSO.BaseInfo.CreateTime;
            int SOSysNo = int.Parse(CurrentSO.BaseInfo.SOID);
            foreach (SOItemInfo soItem in CurrentSO.Items)
            {
                //如果订单已经支付直接返还库存
                if (sopro.NetpaySOCheckReturnInventory(SOSysNo))
                {
                    switch (soItem.ProductType.Value)
                    {
                        case SOProductType.Product:
                        case SOProductType.Gift:
                        case SOProductType.Award:
                        case SOProductType.SelfGift:
                        case SOProductType.Accessory:
                            adjustItemList.Add(new BizEntity.Inventory.InventoryAdjustItemInfo
                            {
                                AdjustQuantity = soItem.Quantity.Value,
                                ProductSysNo = soItem.ProductSysNo.Value,
                                StockSysNo = soItem.StockSysNo.Value
                            });
                            break;
                        case SOProductType.Coupon:
                        case SOProductType.ExtendWarranty:
                            break;
                    }
                }
                else
                {
                    //开启促销活动里的商品支持付款后扣减在线库存
                    if (AppSettingManager.GetSetting(SOConst.DomainName, "PaymentInventory").ToString().ToLower() == "true")
                    {
                        //判断商品是否是在促销活动时间里买的商品，如果是，就返还库存
                        if (sopro.CheckReturnInventory(soItem.ProductSysNo.Value, SOCreatDate.Value))
                        {
                            switch (soItem.ProductType.Value)
                            {
                                case SOProductType.Product:
                                case SOProductType.Gift:
                                case SOProductType.Award:
                                case SOProductType.SelfGift:
                                case SOProductType.Accessory:
                                    adjustItemList.Add(new BizEntity.Inventory.InventoryAdjustItemInfo
                                    {
                                        AdjustQuantity = soItem.Quantity.Value,
                                        ProductSysNo = soItem.ProductSysNo.Value,
                                        StockSysNo = soItem.StockSysNo.Value
                                    });
                                    break;
                                case SOProductType.Coupon:
                                case SOProductType.ExtendWarranty:
                                    break;
                            }
                        }
                    }
                    else
                    {
                        switch (soItem.ProductType.Value)
                        {
                            case SOProductType.Product:
                            case SOProductType.Gift:
                            case SOProductType.Award:
                            case SOProductType.SelfGift:
                            case SOProductType.Accessory:
                                adjustItemList.Add(new BizEntity.Inventory.InventoryAdjustItemInfo
                                {
                                    AdjustQuantity = soItem.Quantity.Value,
                                    ProductSysNo = soItem.ProductSysNo.Value,
                                    StockSysNo = soItem.StockSysNo.Value
                                });
                                break;
                            case SOProductType.Coupon:
                            case SOProductType.ExtendWarranty:
                                break;
                        }
                    }
                }
            }
            ExternalDomainBroker.AdjustProductInventory(new BizEntity.Inventory.InventoryAdjustContractInfo
            {
                ReferenceSysNo = SOSysNo.ToString(),
                SourceActionName = IsImmediatelyReturnStock ? BizEntity.Inventory.InventoryAdjustSourceAction.Abandon_RecoverStock : BizEntity.Inventory.InventoryAdjustSourceAction.Abandon,
                IsOutStockAbandon = IsOutStockOrder ? true : false,
                SourceBizFunctionName = BizEntity.Inventory.InventoryAdjustSourceBizFunction.SO_Order,
                AdjustItemList = adjustItemList
            });
        }

        /// <summary>
        /// 取消使用延保，
        /// </summary>
        protected virtual void CancelExtendWarranty()
        {
            SODA.CancelSOExtendWarranty(SOSysNo);
        }

        /// <summary>
        /// 取消积分支付，返还积分
        /// </summary>
        protected virtual void CancelPointPay()
        {
            if (CurrentSO.BaseInfo.PointPay > 0)
            {
                ExternalDomainBroker.AdjustPoint(new BizEntity.Customer.AdjustPointRequest
                {
                    CustomerSysNo = CurrentSO.BaseInfo.CustomerSysNo,
                    Point = CurrentSO.BaseInfo.PointPay,
                    PointType = (int)ECCentral.BizEntity.Customer.AdjustPointType.AbandonSO,
                    SOSysNo = SOSysNo,
                    Memo = ResourceHelper.Get("Res_SO_Abandon_AdjustPoint"),
                    Source = SOConst.DomainName,
                    OperationType = ECCentral.BizEntity.Customer.AdjustPointOperationType.Abandon,
                });
            }
        }

        /// <summary>
        /// 取消礼品卡支付，返还礼品卡
        /// </summary>
        protected virtual void CancelGiftCardPay()
        {
            if (CurrentSO.BaseInfo.GiftCardPay > 0)
            {
                ExternalDomainBroker.CancelUseGiftCard(SOSysNo, CurrentSO.CompanyCode);
            }
        }

        /// <summary>
        /// 取消余额支付，返还余额
        /// </summary>
        protected virtual void CancelPrePay()
        {
            if (CurrentSO.BaseInfo.PrepayAmount > 0)
            {
                // 如果存在有效负财务收款单不返还现金帐户金额，AO中已经包含此金额
                if (CurrentAOIncome == null && !IsCreateAOAndAbandonSO)
                {
                    ExternalDomainBroker.AdjustPrePay(new BizEntity.Customer.CustomerPrepayLog
                    {
                        SOSysNo = SOSysNo,
                        CustomerSysNo = CurrentSO.BaseInfo.CustomerSysNo.Value,
                        AdjustAmount = CurrentSO.BaseInfo.PrepayAmount.Value,
                        PrepayType = ECCentral.BizEntity.Customer.PrepayType.SOPay,
                        Note = "Abadon SO",
                    });
                }
            }
        }

        /// <summary>
        /// 作废收款信息
        /// </summary>
        protected virtual void AbandonSOIncome()
        {
            //作废收款信息
            if (CurrentSOIncome != null)
            {
                ExternalDomainBroker.AbandonSOIncome(CurrentSOIncome.SysNo.Value);
            }
        }

        /// <summary>
        /// 取消订单配送，
        /// </summary>
        protected virtual void CancelDelivery()
        {
            // 1.   如果是泰隆优选快递类型，取消配送任务
            //if (CurrentSO.ShippingInfo.FreightUserSysNo != 0)
            //{
            //    ObjectFactory<DeliveryProcessor>.Instance.CancelDelivery(SOSysNo, "作废订单");
            //}

            //  2.  判断WMS是否下载订单，如果下载则要Hold WMS
            bool wmsIsDownload = SODA.WMSIsDownloadSO(SOSysNo);
            if (wmsIsDownload)
            {
                ObjectFactory<OPCProcessor>.Instance.SendMessageToWMS(CurrentSO, WMSAction.Abandon, OPCCallBackType.NoneCallBack);
            }


        }

        /// <summary>
        /// 作废虚库采购申请
        /// </summary>
        protected virtual void AbandonSOVirtualItemRequest()
        {
            // 作废虚库采购申请
            SODA.AbandonSOVirtualItemRequest(SOSysNo);
        }

        /// <summary>
        /// 提交作废订单之前,根据不同订单类型，各自的独有操作。
        /// </summary>
        protected virtual void PreCommit()
        {
        }

        /// <summary>
        /// 取消订单支付
        /// </summary>
        protected virtual void CancelSOPay()
        {
            // 取消使用延保
            CancelExtendWarranty();

            // 客户积分处理，返还客户支付的积分
            CancelPointPay();

            // 礼品卡支付部分处理
            CancelGiftCardPay();

            // 返还通过余额支付的金额并记录日志
            CancelPrePay();
        }

        /// <summary>
        /// 作废订单
        /// </summary>
        /// <param name="isSynch">是否同步作废</param>
        /// <param name="IsImmediatelyReturnStock">是否立即返还库存</param>
        /// <param name="isAO"></param>
        protected virtual void AbandonSO()
        {       
            int couponSysNo = CouponList != null && CouponList.Count > 0 ? CouponList[0].ProductSysNo.Value : 0;
            string couponCode = string.Empty;

            var shopProducts = CurrentSO.Items.Where(p=>p.InventoryType== BizEntity.IM.ProductInventoryType.GetShopInventory);
     
            //ERP库存调整
            ERPInventoryAdjustInfo erpAdjustInfo = new ERPInventoryAdjustInfo 
            {
                OrderSysNo = this.CurrentSO.SysNo.Value,
                OrderType = "SO",
                AdjustItemList = new List<ERPItemInventoryInfo>(),
                Memo = "作废减少销售数量"
            };

            foreach (var item in shopProducts)
            {
                ERPItemInventoryInfo adjustItem = new ERPItemInventoryInfo
                {                    
                    ProductSysNo = item.ProductSysNo,
                    B2CSalesQuantity = -item.Quantity.Value
                };

                erpAdjustInfo.AdjustItemList.Add(adjustItem);
            }

            if (erpAdjustInfo.AdjustItemList.Count > 0)
            {
                string adjustXml = ECCentral.Service.Utility.SerializationUtility.ToXmlString(erpAdjustInfo);
                ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(string.Format("作废减少销售数量：{0}", adjustXml)
                  , BizLogType.Sale_SO_VoideSO
                  , this.CurrentSO.SysNo.Value
                  , this.CurrentSO.CompanyCode);
                //ObjectFactory<IAdjustERPInventory>.Instance.AdjustERPInventory(erpAdjustInfo);

            }

            //IsolationLevel与上层事务IsolationLevel有冲突，去除
            //TransactionOptions options = new TransactionOptions
            //{
            //    IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            //};
            //using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
            
            #region 如果是拆分后的子单，还需要更新主单信息。
            if (CurrentSO.BaseInfo.SplitType == SOSplitType.SubSO)
            {
                //重新计算主订单SOAmt,ShipPrice,PrepayAmt,PremiumAmt
                SOInfo masterSO = SODA.GetSOBySOSysNo(CurrentSO.BaseInfo.SOSplitMaster.Value);
                masterSO.BaseInfo.PremiumAmount -= CurrentSO.BaseInfo.PremiumAmount;
                masterSO.BaseInfo.PrepayAmount -= CurrentSO.BaseInfo.PrepayAmount;
                masterSO.BaseInfo.ShipPrice -= CurrentSO.BaseInfo.ShipPrice;
                masterSO.BaseInfo.SOAmount -= CurrentSO.BaseInfo.SOAmount;
                masterSO.BaseInfo.GainPoint -= CurrentSO.BaseInfo.GainPoint;
                masterSO.BaseInfo.PointPay -= CurrentSO.BaseInfo.PointPay;
                masterSO.BaseInfo.PromotionAmount -= CurrentSO.BaseInfo.PromotionAmount;
                masterSO.BaseInfo.PayPrice -= CurrentSO.BaseInfo.PayPrice;
                masterSO.BaseInfo.CouponAmount -= CurrentSO.BaseInfo.CouponAmount;
                masterSO.BaseInfo.PointPayAmount -= CurrentSO.BaseInfo.PointPayAmount;

                //删除对应主单中的商品
                foreach (SOItemInfo item in CurrentSO.Items)
                {
                    //除优惠券外的商品
                    if (item.ProductType != SOProductType.Coupon)
                    {
                        SODA.DeleteSOItemBySOSysNoAndProductSysNo(masterSO.SysNo.Value, item.ProductSysNo.Value);
                    }
                }
                //更新主订单金额
                SODA.UpdateSOAmountInfo(masterSO.BaseInfo);
            }

            #endregion 如果是子单，还需要更新主单信息

            //取消优惠券使用
            CancelCoupon();

            //  保存作废订单信息
            SaveCurrentSO();
            //if (AppSettingManager.GetSetting(SOConst.DomainName, "PaymentInventory").ToString().ToLower() != "true")
            //{
            //    // 调整仓库，返还商品库存
            //    AdjustInventory();
            //}
            //SOProcessor sopro = new SOProcessor();
            //DateTime? SOCreatDate = CurrentSO.BaseInfo.CreateTime;
            //foreach (var item in CurrentSO.Items)
            //{
            //    if (sopro.CheckReturnInventory(item.ProductSysNo.Value, SOCreatDate.Value))
            //    {
            //        // 调整仓库，返还商品库存
            //        AdjustInventory();
            //    }
            //}

            //调整仓库，返还商品库存
            AdjustInventory();

            //订单支付
            CancelSOPay();

            // 如果订单有奖品，则要退还奖品。
            if (CurrentSO.Items != null && CurrentSO.Items.Exists(x => x.ProductType == SOProductType.Award))
            {
                ExternalDomainBroker.ReturnAwardForSO(SOSysNo);
            }
            // 取消配送
            CancelDelivery();

            // 作废订单的虚库申请
            AbandonSOVirtualItemRequest();
            // 提交作废订单之前，所做的操作。
            PreCommit();
          
            WriteLog(CurrentSO);
        }

        /// <summary>
        /// 作废订单
        /// </summary>
        /// <param name="OperatorType">作废订单的用户类型</param>
        /// <param name="IsImmediatelyReturnStock">是否立即将订单中商品的库存返回给仓库，true表示是立即返回，false表示不是</param>
        public virtual void Abandon()
        {
            //  1.  判断订单是否被前台锁定，锁定后不能作废
            Holder.CheckSOIsWebHold();

            //  2.  订单作废检查，检查是否有有效收款，积分使用，余额使用，优惠券使用，账期支付等
            try
            {
                ValidateAbandonSO(true);
            }
            catch (BizException bizex)
            {
                if (CurrentSO.BaseInfo.HoldStatus == SOHoldStatus.BackHold)
                {
                    throw bizex;
                }
                CurrentSO.BaseInfo.HoldReason = ResourceHelper.Get("Res_SO_Abandon_Hold");

                Holder.Hold(ECCentral.Service.SO.BizProcessor.SOHolder.SOHoldReason.AbandonOrder, OPCCallBackType.HoldCallBack);

                throw new BizException(String.Format("{0}\r\n{1}", bizex.Message, ResourceHelper.Get("SO_Abandon_Hold")));
            }

            //  3.  先锁定订单，再作订单。如果已经锁定就直接作废，否则先锁定订单
            bool isHold = CurrentSO.BaseInfo.HoldStatus == SOHoldStatus.BackHold;

            if (!isHold)
            {
                isHold = Holder.Hold(ECCentral.Service.SO.BizProcessor.SOHolder.SOHoldReason.AbandonOrder, OPCCallBackType.AbandonCallBack);

                if (!isHold)
                {
                    BizExceptionHelper.Throw("SO_Abandon_HoldIsAsyn");
                }
            }
            if (isHold)
            {
                //TransactionOptions option = new TransactionOptions();
                //option.IsolationLevel = IsolationLevel.ReadUncommitted;
                //option.Timeout = TransactionManager.DefaultTimeout;
                //using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, option))
                //{
                //    AbandonSO();
                //    PublishMessage();
                //    scope.Complete();
                //}
                TransactionOptions options = new TransactionOptions
                {
                    IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
                };
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
                {
                    AbandonSO();
                    scope.Complete();
                }
                PublishMessage();


                if (ServiceContext.Current.UserSysNo == 1)
                {
                    SOSendMessageProcessor soMail = new SOSendMessageProcessor();
                    //soMail.AbandonSOEmailToCustomer(this.CurrentSO);
                }
            }
        }

        protected void PublishMessage()
        {
            EventPublisher.Publish<ECCentral.Service.EventMessage.SO.SOAbandonedMessage>
                (new EventMessage.SO.SOAbandonedMessage
                {
                    SOSysNo = SOSysNo,
                    AbandonedUserName = ServiceContext.Current.UserDisplayName,
                    AbandonedUserSysNo = ServiceContext.Current.UserSysNo,
                    CustomerSysNo = CurrentSO.BaseInfo.CustomerSysNo.Value,
                    MasterSOSysNo = CurrentSO.BaseInfo.SplitType == SOSplitType.SubSO ? CurrentSO.BaseInfo.SOSplitMaster.GetValueOrDefault() : SOSysNo,
                    MerchantSysNo = CurrentSO.BaseInfo.Merchant.SysNo.GetValueOrDefault(),
                    OrderAmount = CurrentSO.BaseInfo.SOTotalAmount,
                    OrderTime = CurrentSO.BaseInfo.CreateTime.Value
                });
        }

        #endregion

        #region 创建财务负收款并作废订单
        /// <summary>
        /// 创建财务负收款并作废订单
        /// </summary>
        /// <param name="refundInfo"></param>
        /// <returns></returns>
        public void CreateAOAndAbandonSO(SOIncomeRefundInfo refundInfo)
        {
            IsOutStockOrder = false;
            if (CurrentSO.BaseInfo.Status == SOStatus.CustomsPass)
            {
                IsOutStockOrder = true;
            }
            if (CurrentSO.BaseInfo.Status != SOStatus.Origin
                && CurrentSO.BaseInfo.Status != SOStatus.WaitingOutStock
                //&& CurrentSO.BaseInfo.Status != SOStatus.OutStock
                )
            {
                BizExceptionHelper.Throw("SO_CreateAO_SOStatusIsError");
            }

            if (CurrentSOIncome == null)
            {
                BizExceptionHelper.Throw("SO_CreateAO_SOIncomeIsNull");
            }

            SOIncomeInfo soIncomeInfo = new SOIncomeInfo
            {
                OrderAmt = -CurrentSOIncome.OrderAmt,
                OrderType = SOIncomeOrderType.AO,
                Note = ResourceHelper.Get("Res_SO_Abandon_CreateAO"),
                ReferenceID = "",
                Status = SOIncomeStatus.Origin,
                OrderSysNo = CurrentSO.SysNo,
                IncomeAmt = -(CurrentSOIncome.OrderAmt - CurrentSOIncome.PrepayAmt - CurrentSOIncome.GiftCardPayAmt),
                PayAmount = -(CurrentSOIncome.OrderAmt - CurrentSOIncome.PrepayAmt - CurrentSOIncome.GiftCardPayAmt),
                IncomeStyle = CurrentSOIncome.IncomeStyle,
                PrepayAmt = -CurrentSOIncome.PrepayAmt,
                GiftCardPayAmt = -CurrentSOIncome.GiftCardPayAmt,
                PointPay = -CurrentSOIncome.PointPay,
                CompanyCode = CurrentSO.CompanyCode
            };

            SOIncomeRefundInfo soIncomeRefundInfo = new SOIncomeRefundInfo
            {
                OrderSysNo = refundInfo.SOSysNo,
                OrderType = RefundOrderType.AO,
                SOSysNo = refundInfo.SOSysNo,
                RefundPayType = refundInfo.RefundPayType,
                BankName = refundInfo.BankName,
                BranchBankName = refundInfo.BranchBankName,
                CardNumber = refundInfo.CardNumber,
                CardOwnerName = refundInfo.CardOwnerName,
                PostAddress = refundInfo.PostAddress,
                PostCode = refundInfo.PostCode,
                ReceiverName = refundInfo.ReceiverName,
                Note = refundInfo.Note,
                HaveAutoRMA = false,
                RefundPoint = 0,
                RefundReason = refundInfo.RefundReason,
                CompanyCode = CurrentSO.CompanyCode
            };

            if (refundInfo.RefundPayType == RefundPayType.CashRefund)
            {
                soIncomeRefundInfo.Status = RefundStatus.Audit;
            }
            else
            {
                soIncomeRefundInfo.Status = RefundStatus.Origin;
            }
            if (refundInfo.RefundPayType == RefundPayType.TransferPointRefund)
            {
                soIncomeInfo.IncomeAmt = 0;
                soIncomeRefundInfo.RefundCashAmt = 0;
                soIncomeRefundInfo.RefundPoint = Convert.ToInt32(Decimal.Round(CurrentSO.BaseInfo.SOAmount.Value * ExternalDomainBroker.GetPointToMoneyRatio(), 0));
                soIncomeRefundInfo.RefundGiftCard = CurrentSOIncome.GiftCardPayAmt;
            }
            else
            {
                soIncomeRefundInfo.RefundCashAmt = CurrentSOIncome.OrderAmt - CurrentSOIncome.GiftCardPayAmt;
                soIncomeRefundInfo.RefundGiftCard = CurrentSOIncome.GiftCardPayAmt;
            }

            ValidateAbandonSO(false);

            bool isHold = CurrentSO.BaseInfo.HoldStatus == SOHoldStatus.BackHold;
            if (IsOutStockOrder)
            {
                isHold = true;
            }
            //如果后台锁定
            if (!isHold)
            {
                isHold = Holder.Hold(SOHolder.SOHoldReason.AbandonOrder, OPCCallBackType.AOAbandonCallBack);
                if (!isHold)
                {
                    BizExceptionHelper.Throw("SO_Abandon_HoldIsAsyn");
                }
            }
            if (isHold) //如果订单已经锁定
            {
                CreateAOAndAbandonSO(soIncomeInfo, soIncomeRefundInfo);
                if (IsOutStockOrder)
                {
                    SODA.UpdateSOStatusToReportedFailure(CurrentSO.SysNo.Value);
                    CurrentSO.BaseInfo.Status = SOStatus.Reject;
                }
                SendMessage();
            }
        }

        protected virtual void CreateAOAndAbandonSO(SOIncomeInfo soIncomeInfo, SOIncomeRefundInfo soIncomeRefundInfo)
        {
            TransactionOptions options = new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            };
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                AbandonSO();
                ExternalDomainBroker.CreateSOIncome(soIncomeInfo);
                ExternalDomainBroker.CreateSOIncomeRefundInfo(soIncomeRefundInfo);
                scope.Complete();
            }
        }

        /// <summary>
        /// 发送邮件及短信
        /// </summary>
        public virtual void SendMessage()
        {
            SOSendMessageProcessor soMail = new SOSendMessageProcessor();
            //soMail.AbandonSOEmailToCustomer(this.CurrentSO);
            soMail.SendSMS(this.CurrentSO, BizEntity.Customer.SMSType.OrderAbandon);
        }

        public void RejectAbandonSO()
        {
            IsOutStockOrder = false;
            this.Note = "申报失败订单作废";
            this.ToSoStatus = SOStatus.Reject;
            this.OperationName = "申报失败";
            this.OperationType = BizLogType.Sale_SO_EmployeeAbandon;
            if (CurrentSOIncome == null)
            {
                BizExceptionHelper.Throw("SO_CreateAO_SOIncomeIsNull");
            }

            CustomsAbandonSO();
            //发送邮件 短信
            SendMessage();
        }

        public void CustomsReject()
        {
            IsOutStockOrder = false;
            this.Note = "通关失败订单作废";
            this.ToSoStatus = SOStatus.CustomsReject;
            this.OperationName = "通关失败";
            this.OperationType = BizLogType.Sale_SO_EmployeeAbandon;
            if (CurrentSOIncome == null)
            {
                BizExceptionHelper.Throw("SO_CreateAO_SOIncomeIsNull");
            }
            CustomsAbandonSO();
            //发送邮件 短信
            SendMessage();
        }

        private void CustomsAbandonSO()
        {
            SOIncomeInfo soIncomeInfo = new SOIncomeInfo
            {
                OrderAmt = -CurrentSOIncome.OrderAmt,
                OrderType = SOIncomeOrderType.AO,
                Note = this.Note,//ResourceHelper.Get("Res_SO_Abandon_CreateAO"),
                ReferenceID = "",
                Status = SOIncomeStatus.Origin,
                OrderSysNo = CurrentSO.SysNo,
                IncomeAmt = -(CurrentSOIncome.OrderAmt - CurrentSOIncome.PrepayAmt - CurrentSOIncome.GiftCardPayAmt),
                PayAmount = -(CurrentSOIncome.OrderAmt - CurrentSOIncome.PrepayAmt - CurrentSOIncome.GiftCardPayAmt),
                IncomeStyle = CurrentSOIncome.IncomeStyle,
                PrepayAmt = -CurrentSOIncome.PrepayAmt,
                GiftCardPayAmt = -CurrentSOIncome.GiftCardPayAmt,
                PointPay = -CurrentSOIncome.PointPay,
                CompanyCode = CurrentSO.CompanyCode
            };

            SOIncomeRefundInfo soIncomeRefundInfo = new SOIncomeRefundInfo
            {
                OrderSysNo = SOSysNo,
                OrderType = RefundOrderType.AO,
                SOSysNo = SOSysNo,
                RefundPayType = RefundPayType.NetWorkRefund,
                //BankName = refundInfo.BankName,
                //BranchBankName = refundInfo.BranchBankName,
                //CardNumber = refundInfo.CardNumber,
                //CardOwnerName = refundInfo.CardOwnerName,
                //PostAddress = refundInfo.PostAddress,
                //PostCode = refundInfo.PostCode,
                //ReceiverName = refundInfo.ReceiverName,
                Note = this.Note,
                HaveAutoRMA = false,
                RefundPoint = 0,
                RefundReason = RefundReason,
                CompanyCode = CurrentSO.CompanyCode
            };

            soIncomeRefundInfo.Status = RefundStatus.Origin;

            soIncomeRefundInfo.RefundCashAmt = CurrentSOIncome.OrderAmt - CurrentSOIncome.GiftCardPayAmt;
            soIncomeRefundInfo.RefundGiftCard = CurrentSOIncome.GiftCardPayAmt;
            //验证
            ValidateAbandonSO(false);
            //作废订单
            CreateAOAndAbandonSO(soIncomeInfo, soIncomeRefundInfo);
        }
        #endregion
    }

    /// <summary>
    /// 实物卡作废。属性 Parameter 说明 ：
    /// Parameter[0] : bool , 表示订单作废后订单中商品是否立即返还库存,默认为false；
    /// Parameter[1] : bool , 如果有有效的收款记录是否生成负收款单后再作废订单
    /// Parameter[2] : ECCentral.BizEntity.Invoice.SOIncomeRefundInfo , 负收款信息。
    /// </summary>
    [VersionExport(typeof(SOAction), new string[] { "PhysicalCard", "Abandon" })]
    public class PhysicalCardSOAbandoner : SOAbandoner
    {
        protected override void CancelExtendWarranty()
        {
            //实物卡，电子卡没有延保不需要取消
        }
    }

    /// <summary>
    /// 电子卡订单作废。属性 Parameter 说明 ：
    /// Parameter[0] : bool , 表示订单作废后订单中商品是否立即返还库存,默认为false；
    /// Parameter[1] : bool , 如果有有效的收款记录是否生成负收款单后再作废订单
    /// </summary>
    [VersionExport(typeof(SOAction), new string[] { "ElectronicCard", "Abandon" })]
    public class ElectronicCardSOAbandoner : PhysicalCardSOAbandoner
    {
        protected override void AdjustInventory()
        {
            // 电子卡订单不需要调整仓库
        }

        protected override void AbandonSOVirtualItemRequest()
        {
            // 电子卡订单不会有虚库申请
        }

        protected override void CancelDelivery()
        {
            // 电子卡不用配送，所以也不需要取消配送
        }
    }
}