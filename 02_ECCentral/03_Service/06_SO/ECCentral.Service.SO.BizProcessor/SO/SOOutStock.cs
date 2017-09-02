using System;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using System.Linq;
using System.Transactions;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.SO.BizProcessor
{


    /// <summary>
    /// 订单出库：目前只支持
    /// </summary>
    [VersionExport(typeof(SOAction), new string[] { "General", "OutStock" })]
    public class SOOutStock : SOAction
    {
        /* Parameter说明: 
         * Parameter[0] : string , 出库的SSB信息
         * 
         * 
         */

        /// <summary>
        /// xml格式的string , 出库的SSB信息
        /// </summary>
        protected SalesOrderInfo SOOutStockMessage
        {
            get
            {
                return GetParameterByIndex<SalesOrderInfo>(0, null);
            }
        }

        public SOStatus? ToSoStatus
        {
            get
            {
                return GetParameterByIndex<SOStatus?>(1, null);
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
        private ISOLogDA _soLogDA;
        protected ISOLogDA SOLogDA
        {
            get
            {
                _soLogDA = _soLogDA ?? ObjectFactory<ISOLogDA>.Instance;
                return _soLogDA;
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
        private SOHolder _holder;
        /// <summary>
        /// 当前订单的锁定操作
        /// </summary>
        SOHolder Holder
        {
            get
            {
                _holder = _holder ?? ObjectFactory<SOHolder>.Instance;
                _holder.CurrentSO = CurrentSO;
                return _holder;
            }
        }

        public override void Do()
        {
            if (ToSoStatus.HasValue && ToSoStatus.Value == SOStatus.CustomsPass)
            {
                CustomsPass();
            }
            else
            {
                Shipped();
            }
        }

        protected virtual void WriteLog()
        {
            WriteLog(ECCentral.BizEntity.Common.BizLogType.Sale_SO_OutStock, "订单出库");
        }



        /// <summary>
        /// 发货(含泰隆优选出库和商家发货)，根据订单类型判断
        /// </summary>
        /// <param name="xml"></param>
        public void Shipped()
        {
            SalesOrderInfo soShippedEntity = SOOutStockMessage;

            Shipout(SOOutStockMessage.SONumber, 0, SOOutStockMessage.CompanyCode, SOOutStockMessage.METShipViaCode, SOOutStockMessage.METPackageNumber);

        }

        /// <summary>
        /// 出库
        /// </summary>
        /// <param name="soSysNo">销售单编号</param>
        /// <param name="user">用户名称（商家仓储）</param>
        /// <param name="userSysno">用户编号</param>
        /// <param name="shipOutType">出库类型：0商家发货 | 1泰隆优选出库</param>
        /// <param name="companyCode">公司编码</param>
        public void Shipout(int soSysNo, int shipOutType, string companyCode, string metShipViaCode, string metPackageNumber)
        {
            XElement orderConfig = AppSettingHelper.OrderBizConfig;
            int userSysno = int.Parse(orderConfig.Element(XName.Get("SellerPortalUserInfo")).Element(XName.Get("UserSysNo")).Value); // int.Parse(orderConfig.SellerPortalUserInfo.UserSysNo);

            SOInfo soInfo = CurrentSO;
            //1.检查SO信息
            ValidateSOInfo(soInfo);

            #region 修改订单状态，调整库存，创建代销转财务记录

            TransactionOptions option = new TransactionOptions();
            option.IsolationLevel = IsolationLevel.ReadUncommitted;
            option.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, option))
            {
                SOStatusChangeInfo statusChangeInfo = new SOStatusChangeInfo
                {
                    SOSysNo = soSysNo,
                    ChangeTime = DateTime.Now,
                    OldStatus = soInfo.BaseInfo.Status,
                    Status = SOStatus.OutStock,
                    OperatorSysNo = userSysno,
                    OperatorType = userSysno == 0 ? SOOperatorType.System : SOOperatorType.User
                };
                //修改订单状态
                SODA.UpdateSOStatusToOutStock(statusChangeInfo);

                soInfo.BaseInfo.Status = SOStatus.OutStock; //设置出库状态

                //如果是商家配送将不记日志
                //添加商家出库日志表
                metShipViaCode = metShipViaCode ?? "";
                if (!string.IsNullOrEmpty(metShipViaCode))
                {
                    if (metShipViaCode.Length > 50)
                    {
                        metShipViaCode = metShipViaCode.Substring(0, 50);
                    }
                    SOLogDA.InsertMerchantShippingLog(soInfo.SysNo.Value, ServiceContext.Current.UserSysNo, metShipViaCode, metPackageNumber);
                }


                List<BizEntity.Inventory.InventoryAdjustItemInfo> adjustItemList = new List<BizEntity.Inventory.InventoryAdjustItemInfo>();
                foreach (SOItemInfo soItem in soInfo.Items)
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
                //调整库存
                ExternalDomainBroker.AdjustProductInventory(new BizEntity.Inventory.InventoryAdjustContractInfo
                {
                    SourceActionName = BizEntity.Inventory.InventoryAdjustSourceAction.OutStock,
                    SourceBizFunctionName = BizEntity.Inventory.InventoryAdjustSourceBizFunction.SO_Order,
                    ReferenceSysNo = soInfo.SysNo.Value.ToString(),
                    AdjustItemList = adjustItemList
                });

                //Bowen:代码调整，加入事务中 2013-08-08
                //模式2，3创建代销转财务日志
                CreateConsigenToAccInfo(soInfo);


                #region 更新客户等级以及积分经验值
                //增加客户经验值
                //更新客户等级            
                //调整客户经验值（内部修改客户等级）
                int customerSysNo = soInfo.BaseInfo.CustomerSysNo.Value;
                decimal adjustValue = soInfo.BaseInfo.CashPay + soInfo.BaseInfo.PayPrice.Value + soInfo.BaseInfo.ShipPrice.Value + soInfo.BaseInfo.PremiumAmount.Value + soInfo.BaseInfo.PromotionAmount.Value;

                string logMemo = string.Format("SO#{0}:购物加经验值。", soInfo.SysNo);

                ExternalDomainBroker.AdjustCustomerExperience(customerSysNo, adjustValue, BizEntity.Customer.ExperienceLogType.MerchantSOOutbound, logMemo);
                //增加推荐用户的经验值
                AddExperienceByRecommend(soInfo);

                //给款到发货用户加积分
                AddPointForCustomer(soInfo);
                #endregion 更新客户等级以及积分经验值

                #region 财务应收

                //创建收款单
                ECCentral.BizEntity.Invoice.SOIncomeInfo soIncomeInfo = ExternalDomainBroker.GetValidSOIncomeInfo(soInfo.SysNo.Value, BizEntity.Invoice.SOIncomeOrderType.SO);
                if (soIncomeInfo == null)
                {
                    soIncomeInfo = new BizEntity.Invoice.SOIncomeInfo();
                    soIncomeInfo.OrderType = BizEntity.Invoice.SOIncomeOrderType.SO;
                    soIncomeInfo.OrderSysNo = soInfo.SysNo;
                    soIncomeInfo.OrderAmt = UtilityHelper.TruncMoney(soInfo.BaseInfo.SOTotalAmount);
                    soIncomeInfo.IncomeAmt = UtilityHelper.TruncMoney(soInfo.BaseInfo.OriginalReceivableAmount);
                    soIncomeInfo.PrepayAmt = Math.Max(soInfo.BaseInfo.PrepayAmount.Value, 0);
                    soIncomeInfo.IncomeStyle = ECCentral.BizEntity.Invoice.SOIncomeOrderStyle.Normal;
                    //soIncomeInfo.IncomeUserSysNo = soInfo.LastEditUserSysNumber ?? 0;
                    //soIncomeInfo.IncomeTime = DateTime.Now;
                    soIncomeInfo.Status = ECCentral.BizEntity.Invoice.SOIncomeStatus.Origin;
                    soIncomeInfo.GiftCardPayAmt = soInfo.BaseInfo.GiftCardPay;
                    soIncomeInfo.PointPay = soInfo.BaseInfo.PointPay;
                    soIncomeInfo.PayAmount = soInfo.BaseInfo.OriginalReceivableAmount;
                    if (soInfo.BaseInfo.SOSplitMaster.HasValue)
                    {
                        soIncomeInfo.MasterSoSysNo = soInfo.BaseInfo.SOSplitMaster;  //获取母单号
                    }
                    ExternalDomainBroker.CreateSOIncome(soIncomeInfo);
                }

                #endregion 财务应收

                this.PublishMessage();
                scope.Complete();
            }


            #endregion

            SOSendMessageProcessor messageProcessor = ObjectFactory<SOSendMessageProcessor>.Instance;
            //发送邮件
            messageProcessor.SOOutStockSendEmailToCustomer(soInfo);

            //发送短信提醒
            //发送短信
            messageProcessor.SendSMS(soInfo, BizEntity.Customer.SMSType.OrderOutBound);

            if (soInfo.InvoiceInfo.IsVAT.Value && soInfo.InvoiceInfo.InvoiceType == ECCentral.BizEntity.Invoice.InvoiceType.SELF)
            {
                //增票提醒短信
                messageProcessor.SendVATSMS(soInfo);
                //发送增值税发票SSB 
                EventPublisher.Publish<ECCentral.Service.EventMessage.ImportVATSSBMessage>(new ECCentral.Service.EventMessage.ImportVATSSBMessage
                {
                    SOSysNo = soInfo.SysNo.Value,
                    StockSysNo = soInfo.Items[0].StockSysNo.Value,
                    OrderType = EventMessage.ImportVATOrderType.SO
                });
            }

            //调用OverseaInvoiceReceiptManagement.dbo.UP_InvoiceSync
            //插入Inovice_Master
            EventPublisher.Publish<ECCentral.Service.EventMessage.CreateInvoiceSSBMessage>(new ECCentral.Service.EventMessage.CreateInvoiceSSBMessage
            {
                CompanyCode = soInfo.CompanyCode,
                InvoiceNo = soInfo.InvoiceInfo.InvoiceNo,
                SOSysNo = soInfo.SysNo.Value,
                StockSysNo = soInfo.Items[0].StockSysNo.Value
            });
            //记录日志            
            WriteLog(ECCentral.BizEntity.Common.BizLogType.Sale_SO_OutStock, "订单出库");
        }


        /// <summary>
        /// 创建代销转财务记录
        /// </summary>
        /// <param name="header">common info</param>
        /// <param name="soInfo">so entity</param> 
        private void CreateConsigenToAccInfo(SOInfo soInfo)
        {
            if (soInfo.ShippingInfo.StockType == ECCentral.BizEntity.Invoice.StockType.MET
                && soInfo.InvoiceInfo.InvoiceType == ECCentral.BizEntity.Invoice.InvoiceType.SELF)
            {
                List<ConsignToAcctLogInfo> consignToAcclist = new List<ConsignToAcctLogInfo>();
                List<SOItemInfo> soItemList = soInfo.Items;
                int? vendorSysno = soInfo.Merchant.MerchantID;
                soItemList = soItemList.Where(item => item.ProductType == SOProductType.Product
                    || item.ProductType == SOProductType.Gift
                    || item.ProductType == SOProductType.Award
                    || item.ProductType == SOProductType.Accessory
                    || item.ProductType == SOProductType.SelfGift).ToList();
                //todo:代收或代销？需确认
                soItemList.ForEach(item =>
                {
                    consignToAcclist.Add(new ConsignToAcctLogInfo
                    {
                        CreateCost = item.NoTaxCostPrice,
                        OrderSysNo = soInfo.BaseInfo.SysNo,
                        Point = item.GainPoint,
                        ProductSysNo = item.ProductSysNo,
                        ProductQuantity = item.Quantity,
                        SalePrice = item.Price,
                        StockSysNo = item.StockSysNo,
                        VendorSysNo = vendorSysno,
                        OutStockTime = DateTime.Now,
                        CompanyCode = soInfo.CompanyCode
                    });
                });

                ExternalDomainBroker.BatchCreateConsignToAcctLogsInventory(consignToAcclist);
            }
        }
        /// <summary>
        /// 校验SO信息
        /// </summary>
        /// <param name="soInfo">So info</param>
        private void ValidateSOInfo(SOInfo soInfo)
        {
            if (soInfo.BaseInfo.Status.HasValue && soInfo.BaseInfo.Status.Value != SOStatus.WaitingOutStock)
            {
                string errorMsg = string.Format("SO单{0}的状态不是“待出库”，不能执行出库操作！", soInfo.BaseInfo.SOID);
                BizExceptionHelper.Throw(errorMsg);
            }
        }

        /// <summary>
        /// 如果是新客户的第一张订单，为推荐人增加经验值
        /// </summary>
        /// <param name="soInfo">订单</param>
        /// <param name="customerInfo">订单客户</param>
        public void AddExperienceByRecommend(SOInfo soInfo)
        {
            SOBaseInfo soBaseInfo = soInfo.BaseInfo;
            ECCentral.BizEntity.Customer.CustomerBasicInfo customerInfo = ExternalDomainBroker.GetCustomerInfo(soBaseInfo.CustomerSysNo.Value).BasicInfo;

            string companyCode = soInfo.CompanyCode;
            // 对象为空
            if (customerInfo == null || soInfo == null) return;

            // 该客户没有推荐人
            if (!customerInfo.RecommendedByCustomerSysNo.HasValue || customerInfo.RecommendedByCustomerSysNo.Value == 0) return;

            // 判断订单是否是该客户的第一张成功出库的订单
            if (!SODA.IsFirstSO(soInfo.SysNo.Value, soBaseInfo.CustomerSysNo.Value)) return;


            // 获取推荐人信息
            ECCentral.BizEntity.Customer.CustomerBasicInfo rmdCustomerInfo = ExternalDomainBroker.GetCustomerInfo(customerInfo.RecommendedByCustomerSysNo.Value).BasicInfo;

            // 没有相应的推荐人信息
            if (rmdCustomerInfo == null) return;

            // 推荐人不能是客户自己
            if (customerInfo.CustomerSysNo == rmdCustomerInfo.CustomerSysNo) return;

            // 如果推荐人被禁用，不对其增加经验值
            if (rmdCustomerInfo.Status.Value == BizEntity.Customer.CustomerStatus.InValid) return;

            // 订单金额
            decimal soAmount = soBaseInfo.CashPay + soBaseInfo.PayPrice.Value + soBaseInfo.ShipPrice.Value + soBaseInfo.PremiumAmount.Value + soBaseInfo.PromotionAmount.Value;
            // 推荐人经验值的增加数量为订单金额的10%
            decimal rate = AppSettingHelper.RecommendExperienceRatio;
            decimal addExperience = decimal.Round(soAmount * rate, 0); // 应该增加的经验值，4舍5入到整数

            // 如果增加值<1,
            if (addExperience > 0)
            {
                string logMemo = string.Format("SO#{0}:引荐新用户，首次购物成功，加经验值。", soInfo.SysNo);
                ExternalDomainBroker.AdjustCustomerExperience(rmdCustomerInfo.CustomerSysNo.Value, addExperience, BizEntity.Customer.ExperienceLogType.Recommend, logMemo);
                ObjectFactory<SOSendMessageProcessor>.Instance.RecommendCustomerAddExperienceSendMail(rmdCustomerInfo, customerInfo.CustomerID, addExperience);
            }
        }

        /// <summary>
        /// 给用户加积分
        /// </summary>
        /// <param name="soInfo"></param>
        public void AddPointForCustomer(SOInfo soInfo)
        {
            if (!soInfo.BaseInfo.PayWhenReceived.Value && soInfo.BaseInfo.GainPoint != 0)
            {
                //给款到发货的客户加积分
                ExternalDomainBroker.AdjustPoint(new BizEntity.Customer.AdjustPointRequest
                {
                    CustomerSysNo = soInfo.BaseInfo.CustomerSysNo,
                    Memo = string.Format("SO#:{0}-购物送积分", soInfo.SysNo),
                    OperationType = ECCentral.BizEntity.Customer.AdjustPointOperationType.AddOrReduce,
                    Point = soInfo.BaseInfo.GainPoint,
                    PointType = (int)ECCentral.BizEntity.Customer.AdjustPointType.SalesDiscountPoint,
                    SOSysNo = soInfo.SysNo,
                    Source = SOConst.DomainName,
                });
            }
        }

        private string GetOrginComputerNo(int? stockA, string companyCode)
        {
            if (!stockA.HasValue)
            {
                return string.Empty;
            }

            string result = string.Empty;
            string whcomfig = ExternalDomainBroker.GetSystemConfigurationValue("GrouldSettingValue", companyCode);
            if (string.IsNullOrEmpty(whcomfig))
            {
                BizExceptionHelper.Throw("Res_SO_SysConfigurationError");
            }


            Dictionary<string, string> dic_whs = new Dictionary<string, string>();
            string[] strs = whcomfig.Split(new char[] { ',' });
            foreach (string str in strs)
            {
                string[] srs = str.Split(new char[] { ':' });
                dic_whs.Add(srs[0], srs[1]);
            }

            foreach (var wh in dic_whs)
            {
                if (wh.Key == stockA.ToString())
                {
                    result = wh.Value;
                }
            }

            if (result == string.Empty)
            {
                BizExceptionHelper.Throw("Res_SO_NotExistsKeyStock", stockA.Value.ToString());
            }
            return result;
        }

        protected virtual void PublishMessage()
        {
            EventPublisher.Publish<ECCentral.Service.EventMessage.SO.SOOutStockMessage>
                (new EventMessage.SO.SOOutStockMessage
                {
                    SOSysNo = SOSysNo,
                    OutStockUserName = ServiceContext.Current.UserDisplayName,
                    OutStockUserSysNo = ServiceContext.Current.UserSysNo,
                    CustomerSysNo = CurrentSO.BaseInfo.CustomerSysNo.Value,
                    MasterSOSysNo = CurrentSO.BaseInfo.SplitType == SOSplitType.SubSO ? CurrentSO.BaseInfo.SOSplitMaster.GetValueOrDefault() : SOSysNo,
                    MerchantSysNo = CurrentSO.BaseInfo.Merchant.SysNo.GetValueOrDefault(),
                    OrderAmount = CurrentSO.BaseInfo.SOTotalAmount,
                    OrderTime = CurrentSO.BaseInfo.CreateTime.Value
                });
        }

        public void CustomsPass()
        {
            XElement orderConfig = AppSettingHelper.OrderBizConfig;
            int userSysno = int.Parse(orderConfig.Element(XName.Get("SellerPortalUserInfo")).Element(XName.Get("UserSysNo")).Value); // int.Parse(orderConfig.SellerPortalUserInfo.UserSysNo);

            SOInfo soInfo = CurrentSO;
            //1.检查SO信息
            //ValidateSOInfo(soInfo);
            if (soInfo.BaseInfo.Status.HasValue && soInfo.BaseInfo.Status.Value != SOStatus.Reported)
            {
                string errorMsg = string.Format("SO单{0}的状态不是“已申报待通关”，不能执行出库操作！", soInfo.BaseInfo.SOID);
                BizExceptionHelper.Throw(errorMsg);
            }

            #region 修改订单状态，调整库存，创建代销转财务记录

            TransactionOptions option = new TransactionOptions();
            option.IsolationLevel = IsolationLevel.ReadUncommitted;
            option.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, option))
            {
                soInfo.BaseInfo.Status = SOStatus.CustomsPass; //设置出库状态
                SODA.UpdateSOStatusToCustomsPass(soInfo.SysNo.Value);
                List<BizEntity.Inventory.InventoryAdjustItemInfo> adjustItemList = new List<BizEntity.Inventory.InventoryAdjustItemInfo>();
                foreach (SOItemInfo soItem in soInfo.Items)
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
                //调整库存
                ExternalDomainBroker.AdjustProductInventory(new BizEntity.Inventory.InventoryAdjustContractInfo
                {
                    SourceActionName = BizEntity.Inventory.InventoryAdjustSourceAction.OutStock,
                    SourceBizFunctionName = BizEntity.Inventory.InventoryAdjustSourceBizFunction.SO_Order,
                    ReferenceSysNo = soInfo.SysNo.Value.ToString(),
                    AdjustItemList = adjustItemList
                });

                //Bowen:代码调整，加入事务中 2013-08-08
                //模式2，3创建代销转财务日志
                CreateConsigenToAccInfo(soInfo);


                #region 更新客户等级以及积分经验值
                //增加客户经验值
                //更新客户等级            
                //调整客户经验值（内部修改客户等级）
                int customerSysNo = soInfo.BaseInfo.CustomerSysNo.Value;
                decimal adjustValue = soInfo.BaseInfo.CashPay + soInfo.BaseInfo.PayPrice.Value + soInfo.BaseInfo.ShipPrice.Value + soInfo.BaseInfo.PremiumAmount.Value + soInfo.BaseInfo.PromotionAmount.Value;

                string logMemo = string.Format("SO#{0}:购物加经验值。", soInfo.SysNo);

                ExternalDomainBroker.AdjustCustomerExperience(customerSysNo, adjustValue, BizEntity.Customer.ExperienceLogType.MerchantSOOutbound, logMemo);
                //增加推荐用户的经验值
                AddExperienceByRecommend(soInfo);

                //给款到发货用户加积分
                AddPointForCustomer(soInfo);
                #endregion 更新客户等级以及积分经验值

                #region 财务应收

                //创建收款单
                ECCentral.BizEntity.Invoice.SOIncomeInfo soIncomeInfo = ExternalDomainBroker.GetValidSOIncomeInfo(soInfo.SysNo.Value, BizEntity.Invoice.SOIncomeOrderType.SO);
                if (soIncomeInfo == null)
                {
                    soIncomeInfo = new BizEntity.Invoice.SOIncomeInfo();
                    soIncomeInfo.OrderType = BizEntity.Invoice.SOIncomeOrderType.SO;
                    soIncomeInfo.OrderSysNo = soInfo.SysNo;
                    soIncomeInfo.OrderAmt = UtilityHelper.TruncMoney(soInfo.BaseInfo.SOTotalAmount);
                    soIncomeInfo.IncomeAmt = UtilityHelper.TruncMoney(soInfo.BaseInfo.OriginalReceivableAmount);
                    soIncomeInfo.PrepayAmt = Math.Max(soInfo.BaseInfo.PrepayAmount.Value, 0);
                    soIncomeInfo.IncomeStyle = ECCentral.BizEntity.Invoice.SOIncomeOrderStyle.Normal;
                    //soIncomeInfo.IncomeUserSysNo = soInfo.LastEditUserSysNumber ?? 0;
                    //soIncomeInfo.IncomeTime = DateTime.Now;
                    soIncomeInfo.Status = ECCentral.BizEntity.Invoice.SOIncomeStatus.Origin;
                    soIncomeInfo.GiftCardPayAmt = soInfo.BaseInfo.GiftCardPay;
                    soIncomeInfo.PointPay = soInfo.BaseInfo.PointPay;
                    soIncomeInfo.PayAmount = soInfo.BaseInfo.OriginalReceivableAmount;
                    if (soInfo.BaseInfo.SOSplitMaster.HasValue)
                    {
                        soIncomeInfo.MasterSoSysNo = soInfo.BaseInfo.SOSplitMaster;  //获取母单号
                    }
                    ExternalDomainBroker.CreateSOIncome(soIncomeInfo);
                }

                #endregion 财务应收

                //this.PublishMessage();
                scope.Complete();
            }


            #endregion

            SOSendMessageProcessor messageProcessor = ObjectFactory<SOSendMessageProcessor>.Instance;
            //发送邮件
            messageProcessor.SOOutStockSendEmailToCustomer(soInfo);

            //发送短信提醒
            //发送短信
            messageProcessor.SendSMS(soInfo, BizEntity.Customer.SMSType.OrderOutBound);

            if (soInfo.InvoiceInfo.IsVAT.Value && soInfo.InvoiceInfo.InvoiceType == ECCentral.BizEntity.Invoice.InvoiceType.SELF)
            {
                //增票提醒短信
                messageProcessor.SendVATSMS(soInfo);
                //发送增值税发票SSB 
                EventPublisher.Publish<ECCentral.Service.EventMessage.ImportVATSSBMessage>(new ECCentral.Service.EventMessage.ImportVATSSBMessage
                {
                    SOSysNo = soInfo.SysNo.Value,
                    StockSysNo = soInfo.Items[0].StockSysNo.Value,
                    OrderType = EventMessage.ImportVATOrderType.SO
                });
            }

            //调用OverseaInvoiceReceiptManagement.dbo.UP_InvoiceSync
            //插入Inovice_Master
            ObjectFactory<ECCentral.Service.IBizInteract.IInvoiceBizInteract>.Instance.SOOutStockInvoiceSync(soInfo.SysNo.Value, soInfo.Items[0].StockSysNo.Value, soInfo.InvoiceInfo.InvoiceNo, soInfo.CompanyCode);
            //EventPublisher.Publish<ECCentral.Service.EventMessage.CreateInvoiceSSBMessage>(new ECCentral.Service.EventMessage.CreateInvoiceSSBMessage
            //{
            //    CompanyCode = soInfo.CompanyCode,
            //    InvoiceNo = soInfo.InvoiceInfo.InvoiceNo,
            //    SOSysNo = soInfo.SysNo.Value,
            //    StockSysNo = soInfo.Items[0].StockSysNo.Value
            //});
            //记录日志            
            WriteLog(ECCentral.BizEntity.Common.BizLogType.Sale_SO_CustomsPass, "通关成功");
        }
    }
}