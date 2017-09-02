using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.BizProcessor.SO;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.Service.SO.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ECCentral.BizEntity.Invoice.Invoice;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.SO.BizProcessor
{
    [VersionExport(typeof(SOProcessor))]
    public partial class SOProcessor
    {
        private ISODA SODA = ObjectFactory<ISODA>.Instance;

        private SOLogProcessor _soLogProcessor;
        protected SOLogProcessor SOLogProcessor
        {
            get
            {
                _soLogProcessor = _soLogProcessor ?? ObjectFactory<SOLogProcessor>.Instance;
                return _soLogProcessor;
            }
        }

        #region 订单查询相关

        /// <summary>
        /// 填充调用与外Domain相关的字段
        /// </summary>
        /// <param name="soList"></param>
        protected void FillSOInfo(List<SOInfo> soList)
        {
            if (soList != null && soList.Count > 0)
            {
                List<SOItemInfo> soItemList = new List<SOItemInfo>();
                List<int> productSysNoList = new List<int>();
                List<int> customerSysNoList = new List<int>();
                List<SOItemInfo> exItemList = new List<SOItemInfo>();
                soList.ForEach(soInfo =>
                    {
                        //取得订单使用的礼品卡记录
                        soInfo.SOGiftCardList = ExternalDomainBroker.GetSOGiftCardBySOSysNo(soInfo.SysNo.Value);
                        if (!customerSysNoList.Exists(no => no == soInfo.BaseInfo.CustomerSysNo.Value))
                        {
                            customerSysNoList.Add(soInfo.BaseInfo.CustomerSysNo.Value);
                        }
                        soInfo.Items.ForEach(item =>
                            {
                                item.ProductID = string.Empty;
                                switch (item.ProductType.Value)
                                {
                                    case SOProductType.Coupon:
                                        item.ProductID = item.ProductSysNo.ToString();
                                        soItemList.Add(item);
                                        break;
                                    case SOProductType.ExtendWarranty:
                                        exItemList.Add(item);
                                        break;
                                    default:
                                        {
                                            soItemList.Add(item);
                                            if (!productSysNoList.Exists(sysNo => sysNo == item.ProductSysNo))
                                            {
                                                productSysNoList.Add(item.ProductSysNo.Value);
                                            }
                                            break;
                                        }
                                }
                            });
                    });
                List<CustomerBasicInfo> customerList = ExternalDomainBroker.GetCustomerBasicInfo(customerSysNoList);
                if (customerList != null)
                {
                    customerList.ForEach(c =>
                    {
                        SOInfo soInfo = soList.Find(so => so.BaseInfo.CustomerSysNo == c.CustomerSysNo);
                        if (soInfo != null)
                        {
                            soInfo.BaseInfo.CustomerID = c.CustomerID;
                            soInfo.BaseInfo.CustomerName = c.CustomerName;
                        }
                    });
                }
                if (productSysNoList.Count > 0)
                {
                    List<ECCentral.BizEntity.IM.ProductInfo> productList = ExternalDomainBroker.GetProductInfoListByProductSysNoList(productSysNoList);
                    if (productList != null)
                    {
                        productList.ForEach(product =>
                        {
                            List<SOItemInfo> itemList = soItemList.FindAll(item => { return item.ProductType != SOProductType.Coupon && item.ProductType != SOProductType.ExtendWarranty && item.ProductSysNo == product.SysNo; });
                            foreach (SOItemInfo item in itemList)
                            {
                                item.ProductID = product.ProductID;
                            }
                        });
                        exItemList.ForEach(ei =>
                        {
                            int mpNo = int.TryParse(ei.MasterProductSysNo, out mpNo) ? mpNo : int.MinValue;
                            SOItemInfo mp = soItemList.Find(i => i.ProductSysNo == mpNo);
                            if (mp != null)
                            {
                                ei.ProductID = mp.ProductID + "E";
                            }
                        });
                    }
                }
            }
        }

        /// <summary>
        /// 设置商品库存(可用 、Online)
        /// </summary>
        /// <param name="soList"></param>
        protected void SetInventoryQuantity(SOInfo soInfo)
        {
            List<Int32> productSysNoList = new List<int>();
            foreach (var item in soInfo.Items)
            {
                productSysNoList.Add(item.ProductSysNo.Value);
            }
            List<ProductInventoryInfo> pInventoryInfoList = ExternalDomainBroker.GetProductTotalInventoryInfoByProductList(productSysNoList);
            if (pInventoryInfoList != null && pInventoryInfoList.Count > 0)
            {
                foreach (var item in soInfo.Items)
                {
                    //延保重新设置
                    if (item.ProductType == SOProductType.ExtendWarranty)
                    {
                        continue;
                    }

                    foreach (var itemInventory in pInventoryInfoList)
                    {
                        if (item.ProductSysNo == itemInventory.ProductSysNo)
                        {
                            item.AvailableQty = itemInventory.AvailableQty;
                            item.OnlineQty = (itemInventory.AvailableQty + itemInventory.ConsignQty + itemInventory.VirtualQty);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 根据订单系统编号取得订单信息
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <returns>订单信息</returns>
        public virtual SOInfo GetSOBySOSysNo(int soSysNo)
        {
            SOInfo soInfo = SODA.GetSOBySOSysNo(soSysNo);
            if (soInfo != null)
            {
                List<SOInfo> soList = new List<SOInfo>();
                soList.Add(soInfo);
                FillSOInfo(soList);
                SetInventoryQuantity(soInfo);
                SetItemsStockName(soInfo.Items);
                //SetItemsSHDSysNo(soInfo.Items);
                SetItemOutStockInfo(soInfo);

                soInfo.BaseInfo.CustomerPoint = ExternalDomainBroker.GetCustomerValidPoint(soInfo.BaseInfo.CustomerSysNo.Value);

                List<string> numberList = SODA.TrackingNumberBySoSysno(soSysNo);
                if (numberList != null && numberList.Count > 0)
                {
                    soInfo.ShippingInfo.TrackingNumberStr = numberList.Join(",");
                }
            }

            return soInfo;
        }

        /// <summary>
        /// 设置Item对应的仓库信息（标志虚库不足和待采购）
        /// </summary>
        /// <param name="soInfo"></param>
        public void SetItemOutStockInfo(SOInfo soInfo)
        {
            if (soInfo.ShippingInfo != null
               && soInfo.InvoiceInfo != null
               && soInfo.BaseInfo.SOType != SOType.ElectronicCard
               && soInfo.ShippingInfo.StockType == BizEntity.Invoice.StockType.SELF
               && soInfo.ShippingInfo.ShippingType == BizEntity.Invoice.DeliveryType.SELF
               && soInfo.InvoiceInfo.InvoiceType == BizEntity.Invoice.InvoiceType.SELF)
            {
                var item = ObjectFactory<ISOQueryDA>.Instance.QueryCheckItemAccountQty(soInfo.SysNo.Value);
                foreach (System.Data.DataRow row in item.Rows)
                {
                    var itemEntity = soInfo.Items.Find(p => p.ProductSysNo == (int)row["ProductSysno"]);
                    if (itemEntity != null)
                    {
                        itemEntity.IsLessStock = Convert.ToInt32(row["Stock"]) < 0;
                        var vStock = Convert.ToInt32(row["CalQty"]);
                        if (itemEntity.IsLessStock && itemEntity.StockSysNo.HasValue)
                        {
                            int UnPayOrderQty = ObjectFactory<ISOQueryDA>.Instance.QueryCalUnPayOrderQty(itemEntity.ProductSysNo.Value, itemEntity.StockSysNo.Value);
                            if (UnPayOrderQty + vStock < 0)
                            {
                                itemEntity.IsWaitPO = true;
                            }
                        }
                    }
                }
                //soInfo.Items.ForEach(p => {
                //    if (p.StockSysNo.HasValue)
                //    {
                //        var inventStock = ExternalDomainBroker.GetInventoryInfoByProductSysNo(p.ProductSysNo.Value, p.StockSysNo.Value);
                //        if (inventStock != null)
                //        {
                //            //设定是否缺货
                //            p.IsLessStock = inventStock.AccountQty + inventStock.ConsignQty
                //                - inventStock.OrderQty - inventStock.AllocatedQty - inventStock.InvalidQty < 0;
                //        }
                //    }
                //});
            }
        }

        /// <summary>
        /// 设置订单项的仓库名
        /// </summary>
        /// <param name="list"></param>
        private void SetItemsStockName(List<SOItemInfo> list)
        {
            list.ForEach(p =>
            {
                if (p.StockSysNo.HasValue)
                {
                    var stockInfo = ExternalDomainBroker.GetWarehouseInfo(p.StockSysNo.Value);
                    p.StockName = stockInfo == null ? null : stockInfo.WarehouseName;
                }
            });
        }

        //private void SetItemsSHDSysNo(List<SOItemInfo> list)
        //{
        //    list.ForEach(p =>
        //    {
        //        p.SHDSysNo = SODA.GetItemsSHDSysNo(p.ProductSysNo.Value, p.SOSysNo.Value);
        //    });
        //}

        /// <summary>
        /// 根据子订单系统编号取得主订单信息
        /// </summary>
        /// <param name="subSOSysNo">子订单系统编号</param>
        /// <returns></returns>
        public virtual SOInfo GetMasterSOBySubSOSysNo(int subSOSysNo)
        {
            SOInfo soInfo = SODA.GetMasterSOBySubSOSysNo(subSOSysNo);
            if (soInfo != null)
            {
                List<SOInfo> soList = new List<SOInfo>();
                soList.Add(soInfo);
                FillSOInfo(soList);
            }
            return soInfo;
        }

        /// <summary>
        /// 取得订单基本信息
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        public virtual SOBaseInfo GetSOBaseInfoBySOSysNo(int soSysNo)
        {
            return SODA.GetSOBaseInfoBySOSysNo(soSysNo);
        }

        /// <summary>
        /// 取得订单商品信息
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <returns></returns>
        public virtual List<SOItemInfo> GetSOItemsBySOSysNo(int soSysNo)
        {
            return SODA.GetSOItemsBySOSysNo(soSysNo);
        }

        /// <summary>
        /// 根据订单编写列表取得多个订单
        /// </summary>
        /// <param name="soSysNos">订单编号列表</param>
        /// <returns></returns>
        public virtual List<SOInfo> GetSOBySOSysNoList(List<int> soSysNoList)
        {
            List<SOInfo> soList = SODA.GetSOBySOSysNoList(soSysNoList);
            FillSOInfo(soList);
            return soList;
        }
        /// <summary>
        /// 根据主订单编号，取得子订单列表
        /// </summary>
        /// <param name="masterSOSysNo"></param>
        /// <returns></returns>
        public virtual List<SOInfo> GetSubSOByMasterSOSysNo(int masterSOSysNo)
        {
            List<SOInfo> soList = SODA.GetSubSOByMasterSOSysNo(masterSOSysNo);
            FillSOInfo(soList);
            return soList;
        }

        public virtual List<SOInfo> GetSimpleSOInfoList(List<int> soSysNoList)
        {
            List<SOInfo> soList = SODA.GetSimpleSOInfoList(soSysNoList);
            return soList;
        }

        /// <summary>
        /// 根据订单编写列表取得多个订单的基本信息
        /// </summary>
        /// <param name="soSysNos">订单编号列表</param>
        /// <returns></returns>
        public List<SOBaseInfo> GetSOBaseInfoBySOSysNoList(List<int> soSysNos)
        {
            return SODA.GetSOBaseInfoBySOSysNoList(soSysNos);
        }

        /// <summary>
        /// 根据优惠券编号，取得使用此优惠券的订单编号
        /// </summary>
        /// <param name="couponSysNo"></param>
        /// <returns></returns>
        public int? GetSOSysNoByCouponSysNo(int couponSysNo)
        {
            return SODA.GetSOSysNoByCouponSysNo(couponSysNo);
        }

        /// <summary>
        /// 根据客户编号获取订单对应的增值税发票
        /// </summary>
        /// <param name="customerSysNo">客户编号</param>
        /// <returns></returns>
        public List<SOVATInvoiceInfo> QuerySOVATInvoiceInfo(int customerSysNo)
        {
            return SODA.GetSOVATInvoiceInfoByCustomerSysNo(customerSysNo);
        }

        /// <summary>
        /// 根据客户编号获取客户对应的礼品卡信息
        /// </summary>
        /// <param name="customerSysNo">客户编号</param>
        /// <returns></returns>
        public List<GiftCardInfo> QueryGiftCardListInfo(int customerSysNo)
        {
            return SODA.QueryGiftCardListInfoByCustomerSysNo(customerSysNo);
        }

        /// <summary>
        /// 根据 礼品卡编号 和密码 获取 对应的礼品卡信息
        /// </summary>
        /// <param name="code">礼品卡 卡号</param>
        /// <param name="password">礼品卡 密码</param>
        public GiftCardInfo QueryGiftCardByCodeAndPassword(string code, string password)
        {
            return ExternalDomainBroker.GetGiftCardByCodeAndPassword(code, password);
        }

        /// <summary>
        /// 根据支付方式判断是否为货到付款
        /// </summary>
        /// <param name="payTypeSysNo">支付方式编号</param>
        /// <returns>支持款到发货返回真，否则返回假</returns>
        public bool IsPayWhenReceived(int payTypeSysNo)
        {
            bool isPayWhenRecv = false;
            var payType = ExternalDomainBroker.GetPayTypeBySysNo(payTypeSysNo);
            if (payType != null && payType.IsPayWhenRecv.HasValue)
            {
                isPayWhenRecv = payType.IsPayWhenRecv.Value;
            }
            return isPayWhenRecv;
        }

        #endregion

        /// <summary>
        /// 处理订单
        /// </summary>
        /// <param name="soInfo"></param>
        /// <param name="command"></param>
        public void ProcessSO(SOAction.SOCommandInfo command)
        {
            SOActionFactory.Create(command).Do();
        }

        public SOInfo CloneSO(SOInfo entity)
        {
            //获取原先的订单
            SOInfo oriSO = GetSOBySOSysNo(entity.SysNo.Value);

            if (oriSO.Items.Exists(x => x.PriceType == SOProductPriceType.GoldAcc
                || x.PriceType == SOProductPriceType.GuanAiAcc
                || x.PriceType == SOProductPriceType.SdoAccPrice))
                BizExceptionHelper.Throw("SO_CloneSO_GuanAiAccCannotClone");

            //捆绑销售不能拆分
            if (!CheckSaleRule(oriSO, entity))
            {
                BizExceptionHelper.Throw("SO_CloneSO_ComboCanNotClone");
            }

            oriSO.Items.Clear();
            oriSO.Items = entity.Items;

            oriSO.SysNo = NewSOSysNo();
            oriSO.BaseInfo.HoldStatus = SOHoldStatus.Unhold;
            oriSO.BaseInfo.HoldReason = string.Empty;
            oriSO.BaseInfo.HoldUser = 0;

            //验证
            if (oriSO.Items.Exists(item => item.ProductType == SOProductType.SelfGift))
                BizExceptionHelper.Throw("SO_CloneSO_NeweggGiftCannotClone");

            //合约机订单 团购订单 依旧换新订单不能拆分
            if (oriSO.BaseInfo.SOType == SOType.GroupBuy)
                BizExceptionHelper.Throw("SO_CloneSO_ParticularCannotClone");

            //帐期不能拆分
            if (oriSO.BaseInfo.PayTypeSysNo.Value == 4)
                BizExceptionHelper.Throw("SO_CloneSO_ZhangQiCannotClone");

            //存在AO不能拆分
            if (ExternalDomainBroker.GetValidSOIncomeInfo(entity.SysNo.Value, BizEntity.Invoice.SOIncomeOrderType.AO) != null)
                BizExceptionHelper.Throw("SO_CloneSO_ExistsAOCannotCannotClone");

            ExternalDomainBroker.CreateOperationLog("Create SO", BizLogType.Sale_SO_Create, entity.BaseInfo.SysNo.Value, entity.CompanyCode);

            ProcessSO(new SOAction.SOCommandInfo { Command = SOAction.SOCommand.Create, SOInfo = oriSO });

            string note = oriSO.SysNo.ToString() + "[";
            oriSO.Items.ForEach(item =>
            {
                note += item.ProductSysNo.ToString();
            });
            note += "]";

            SODA.UpdateSONote(oriSO.SysNo.Value, note);

            ExternalDomainBroker.CreateOperationLog("更新订单(拆单)", BizLogType.Sale_SO_Update, entity.BaseInfo.SysNo.Value, entity.CompanyCode);

            return oriSO;

        }

        //销售规则Check
        public static bool CheckSaleRule(SOInfo soEntity, SOInfo newSO)
        {
            // 有问题 覆盖其他Promotion
            soEntity.SOPromotions = ExternalDomainBroker.CalculateSOPromotion(soEntity);
            if (soEntity != null && soEntity.SOPromotions != null && soEntity.SOPromotions.Count > 0)
            {
                foreach (SOPromotionInfo soPromotion in soEntity.SOPromotions)
                {
                    if (soPromotion == null || soPromotion.SOPromotionDetails == null)
                    {
                        continue;
                    }
                    //if (销售规则有效?)
                    //{
                    //比较
                    int count = 0;//用于比较的计数
                    foreach (SOItemInfo item in newSO.Items)
                    {
                        if (soPromotion.SOPromotionDetails.Exists(x => x.MasterProductSysNo == item.ProductSysNo))
                            count++;
                    }
                    if (count > 0 && count < soPromotion.SOPromotionDetails.Count)
                        return false;
                    //}
                }
            }
            return true;
        }

        /// <summary>
        /// 生成一个新订单编号
        /// </summary>
        /// <returns></returns>
        public virtual int NewSOSysNo()
        {
            return SODA.NewSOSysNo();
        }

        /// <summary>
        /// 更改订单状态为物流拒收
        /// </summary>
        /// <param name="soSysNo">订单编</param>
        /// <returns></returns>
        public bool UpdateSOStatusToReject(int soSysNo)
        {
            SOStatusChangeInfo statusChangeInfo = new SOStatusChangeInfo
            {
                ChangeTime = DateTime.Now,
                OldStatus = SOStatus.OutStock,
                OperatorSysNo = ServiceContext.Current.UserSysNo,
                OperatorType = SOOperatorType.User,
                SOSysNo = soSysNo,
                Status = SOStatus.Reject
            };
            //更改订单状态为物流拒收
            bool isSuccess = SODA.UpdateSOStatusToReject(statusChangeInfo);
            //写订单更改日志
            SOLogProcessor.WriteSOLog(ECCentral.BizEntity.Common.BizLogType.Sale_SO_AutoRMA, soSysNo, "物流拒收");
            return isSuccess;
        }

        /// <summary>
        /// 更改订单虚库采购单状态 
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        public void UpdateSOCheckShippingVPOStatus(int soSysNo, BizEntity.PO.VirtualPurchaseOrderStatus vpoStatus)
        {
            SODA.UpdateSOCheckShippingVPOStatus(soSysNo, vpoStatus);
        }

        /// <summary>
        /// 设置订单增值税发票已开具
        /// </summary>
        /// <param name="infos"></param>
        public virtual void SOVATPrinted(List<int> soSysNoList)
        {
            SODA.UpdateSOVATPrinted(soSysNoList);
        }

        /// <summary>
        /// 拆分订单发票
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <param name="invoiceItems"></param>
        public virtual void SplitSOInvoice(int soSysNo, List<ECCentral.BizEntity.Invoice.SubInvoiceInfo> invoiceItems)
        {
            SOBaseInfo soInfo = GetSOBaseInfoBySOSysNo(soSysNo);
            switch (soInfo.Status.Value)
            {
                case SOStatus.Origin:
                case SOStatus.WaitingOutStock:
                    {
                        //拆分发票
                        ExternalDomainBroker.SplitInvoice(invoiceItems);
                        //更新订单信息
                        SODA.UpdateSOForSplitInvoice(soSysNo, true);
                    }
                    break;
                default:
                    BizExceptionHelper.Throw("SO_SplitInvoice_StatusIsError");
                    break;
            }
        }

        /// <summary>
        /// 拆分订单发票
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <param name="invoiceItems"></param>
        public virtual void CancelSplitSOInvoice(int soSysNo)
        {
            SOBaseInfo soInfo = GetSOBaseInfoBySOSysNo(soSysNo);
            switch (soInfo.Status.Value)
            {
                case SOStatus.Origin:
                case SOStatus.WaitingOutStock:
                    {
                        //取消拆分发票
                        ExternalDomainBroker.CancelSplitInvoice(soSysNo);
                        //更新订单信息
                        SODA.UpdateSOForSplitInvoice(soSysNo, false);
                    }
                    break;
                default:
                    BizExceptionHelper.Throw("SO_CacelSplitInvoice_StatusIsError");
                    break;
            }
        }

        /// <summary>
        /// 取得移仓单编号
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <returns></returns>
        public virtual int GetShiftSysNoBySOSysNo(int soSysNo)
        {
            return SODA.GetShiftSysNoBySOSysNo(soSysNo);
        }

        /// <summary>
        /// 发送内部邮件
        /// </summary>
        /// <param name="emailList">发送地址列表</param>
        /// <param name="title">标题可以不填</param>
        /// <param name="content">发送内容</param>
        /// <returns>返回成功发送列表</returns>
        public List<string> SendInternalEmail(List<string> emailList, string title, string content, string language)
        {
            KeyValueVariables vars = new KeyValueVariables();
            vars.Add("Title", title);
            vars.Add("Content", content);
            ExternalDomainBroker.SendExternalEmail(string.Join(";", emailList), "SO_InternalEmail", vars, language);
            return emailList;
        }

        /// <summary>
        /// 计算(价格、费用)
        /// </summary>
        /// <param name="info">订单信息</param>
        public SOInfo Calculate(SOInfo entity)
        {
            SOCaculator cacler = new SOCaculator();
            if (entity != null && entity.SysNo.HasValue)
            {
                cacler.IsUpdate = true;
                cacler.OriginalSOInfo = ObjectFactory<SOProcessor>.Instance.GetSOBySOSysNo(entity.SysNo.Value);
            }
            cacler.Calculate(entity);
            return entity;
        }

        /// <summary>
        ///  订单出库后 普票改增票
        /// </summary>
        /// <param name="entity"></param> 
        public void SetSOVATInvoiveWhenSOOutStock(SOInfo entity)
        {
            SOInfo OriginalSOInfo = ObjectFactory<SOProcessor>.Instance.GetSOBySOSysNo(entity.SysNo.Value);
            SODA.UpdateSOVATInvoice(entity.InvoiceInfo.VATInvoiceInfo);
            ECCentral.BizEntity.SO.SOInvoiceChangeLogInfo invoiceChanageLog = new SOInvoiceChangeLogInfo();
            invoiceChanageLog.SOSysNo = entity.BaseInfo.SysNo.Value;
            invoiceChanageLog.StockSysNo = entity.Items[0].StockSysNo > 0 ? entity.Items[0].StockSysNo.Value : 0;
            invoiceChanageLog.ChangeTime = DateTime.Now;
            invoiceChanageLog.CompanyCode = entity.BaseInfo.CompanyCode;
            invoiceChanageLog.UserSysNo = ServiceContext.Current.UserSysNo;
            if (OriginalSOInfo != null && OriginalSOInfo.InvoiceInfo != null && OriginalSOInfo.InvoiceInfo.IsVAT == true)
            {
                invoiceChanageLog.ChangeType = InvoiceChangeType.VATChange;
                invoiceChanageLog.Note = string.Format(ResouceManager.GetMessageString("SO.SOInfo", "Res_SO_VATInvoice_ChangeInfo")
                                                       , entity.InvoiceInfo.VATInvoiceInfo.SysNo
                                                       , entity.InvoiceInfo.VATInvoiceInfo.CompanyName
                                                       , entity.InvoiceInfo.VATInvoiceInfo.TaxNumber
                                                       , entity.InvoiceInfo.VATInvoiceInfo.BankAccount);
            }
            else
            {
                invoiceChanageLog.ChangeType = InvoiceChangeType.GeneralToVAT;
                invoiceChanageLog.Note = ResourceHelper.Get("Res_SO_InvoiceTOVAT_Change", entity.InvoiceInfo.VATInvoiceInfo.SysNo);
            }
            ObjectFactory<ISOLogDA>.Instance.InsertSOInvoiceChangeLogInfo(invoiceChanageLog);
        }

        /// <summary>
        ///  查询当前itemSysNo已经创建的虚库采购单条数
        /// </summary>
        /// <param name="soItemSysNo"></param>
        /// <returns></returns>
        public virtual int GetGeneratedSOVirtualCount(int soItemSysNo)
        {
            return SODA.GetGeneratedSOVirtualCount(soItemSysNo);
        }

        /// <summary>
        /// 手动更改订单仓库
        /// </summary>
        /// <param name="info"></param>
        public bool WHUpdateStock(SOWHUpdateInfo info)
        {
            return SODA.WHUpdateStock(info);
        }

        #region 审核订单通过发送邮件和短信以及更新数据库

        /// <summary>
        /// 发送邮件和短信
        /// </summary>
        /// <param name="soInfo">订单实体</param>
        /// <param name="IsManagerAuditSO">是否主管审核</param>
        /// <param name="isPaySO"></param>
        public virtual void SendMessage(SOInfo soInfo, bool IsManagerAuditSO, bool isPaySO, bool isAutoAudit)
        {
            var soStatus = soInfo.BaseInfo.Status;
            SOSendMessageProcessor soSendMessage = new SOSendMessageProcessor();

            if (soStatus == SOStatus.WaitingOutStock
                || soStatus == SOStatus.OutStock)
            {
                ProcessSO(new SOAction.SOCommandInfo { Command = SOAction.SOCommand.Split, SOInfo = soInfo, Parameter = new object[] { isAutoAudit } });
            }

            //电子卡订单出库发送shipping消息
            if (soInfo.BaseInfo.Status == SOStatus.OutStock
                 && soInfo.BaseInfo.SOType == SOType.ElectronicCard)
            {
                SODA.CreateEGiftCardOrderInvoice(Convert.ToInt32(soInfo.SysNo));
            }

            if (soInfo.BaseInfo.Status == SOStatus.WaitingOutStock)
            {
                soSendMessage.SOAuditedSendEmailToCustomer(soInfo);
                // 并单不发消息
                if (soInfo.ShippingInfo.IsCombine == true)
                {
                    soSendMessage.SendSMS(soInfo, SMSType.OrderAudit);
                }
            }

            if (!isPaySO)
            {
                SOLogProcessor soLog = new SOLogProcessor();
                if (!IsManagerAuditSO)
                {
                    // 如果是主管审核，判断产品价格是否改动，如果改动就发送邮件给相应ＰＭ
                    if (soInfo.BaseInfo.IsWholeSale == true)
                    {
                        soSendMessage.PriceChangedSendMail(soInfo);
                    }
                    soLog.WriteSOLog(BizEntity.Common.BizLogType.Sale_SO_Audit, "订单主管审核", soInfo);
                }
                else
                {
                    soLog.WriteSOLog(BizEntity.Common.BizLogType.Sale_SO_Audit, "订单审核", soInfo);
                }
            }
        }
        #endregion

        /// <summary>
        /// 锁定所有子单，用于判断是否所有子单已出库
        /// </summary>
        /// <param name="subSOSysNo">子单订单编号</param>
        /// <returns>可全作废返回true,否则返回false</returns>
        public bool IsAllSubSONotOutStockList(int subSOSysNo)
        {
            var soEntity = GetSOBaseInfoBySOSysNo(subSOSysNo);

            if (!soEntity.SOSplitMaster.HasValue)
            {
                BizExceptionHelper.Throw("SO_Hold_SubSOSysNoNotExistMasterSOSysNo", soEntity.SysNo.ToString());
            }

            var soMasterList = GetSubSOByMasterSOSysNo(soEntity.SOSplitMaster.Value);
            return soMasterList.Count(p => p.BaseInfo.Status == SOStatus.OutStock) == 0;

            #region 给仓库发消息锁定，暂时不用

            //try
            //{
            //    //锁定所有子单
            //    foreach (var soMaster in soMasterList)
            //    {
            //        if (!SODA.WMSIsDownloadSO(soMaster.SysNo.Value))
            //        {
            //            continue;
            //        }

            //        List<int> stockSysNoList = (from item in soMaster.Items
            //                                    where item.StockSysNo.HasValue && item.ProductType != SOProductType.Coupon && item.ProductType != SOProductType.ExtendWarranty
            //                                    select item.StockSysNo.Value).Distinct().ToList();
            //        //同步锁定订单
            //        WMSHoldMessage message = new WMSHoldMessage
            //        {
            //            SOSysNo = soMaster.SysNo.Value,
            //            //IPP值为3，这里暂时用WMSActionType.UnHold
            //            ActionType = ECCentral.Service.EventMessage.WMSActionType.UnHold,
            //            UserSysNo = ServiceContext.Current.UserSysNo,
            //            WarehouseSysNoList = stockSysNoList,
            //            Reason = soMaster.BaseInfo.HoldReason
            //        };
            //        EventPublisher.Publish<WMSHoldMessage>(message);
            //    }

            //    return true;
            //}
            //catch (ThirdPartBizException biz_ex)
            //{
            //    throw new BizException(biz_ex.Message);
            //}
            #endregion
        }

        /// <summary>
        /// 批量处理文件中的商品信息，并且返回处理后的结果
        /// </summary>
        /// <param name="fileContent"></param>
        /// <returns></returns>
        public List<SOItemInfo> BatchDealItemFromFile(byte[] fileContent)
        {
            List<SOItemInfo> items = null;

            try
            {
                //格式验证,处理
                var workBook = new HSSFWorkbook(new MemoryStream(fileContent));
                var sheet = workBook.GetSheetAt(0);
                var rows = sheet.GetRowEnumerator();

                //过滤两行
                rows.MoveNext();
                rows.MoveNext();

                if (!rows.MoveNext())
                {
                    BizExceptionHelper.Throw("Res_SO_NoMoveNext");
                }

                items = new List<SOItemInfo>();
                do
                {
                    var item = new SOItemInfo();
                    Row row = (Row)rows.Current;
                    //商品Item
                    Cell cell = row.GetCell(0);
                    if (cell == null || cell.CellType == CellType.BLANK)
                    {
                        //略过
                        continue;
                    }
                    if (string.IsNullOrEmpty(cell.StringCellValue) || cell.StringCellValue.Trim().Length == 0)
                    {
                        BizExceptionHelper.Throw("Res_SO_ProductIDIsNullOrEmpty");
                    }
                    item.ProductID = cell.StringCellValue.Trim();
                    //商品价格
                    cell = row.GetCell(1);
                    if (cell != null && cell.CellType != CellType.BLANK)
                    {
                        if (cell.CellType == CellType.NUMERIC)
                        {
                            decimal price = 0.0m;
                            if (decimal.TryParse(cell.NumericCellValue.ToString(), out price))
                            {
                                item.Price_End = price;
                            }
                            else
                            {
                                BizExceptionHelper.Throw("Res_SO_ProductIDNoPrice", item.ProductID);
                            }
                            if (price < 0)
                            {
                                BizExceptionHelper.Throw("Res_SO_ProductIDNoPriceOne", item.ProductID);
                            }
                        }
                        else
                        {
                            BizExceptionHelper.Throw("Res_SO_ProductIDNoPrice", item.ProductID);
                        }
                    }
                    //商品数量
                    cell = row.GetCell(2);
                    if (cell != null && cell.CellType == CellType.NUMERIC)
                    {
                        int qty = 0;
                        if (int.TryParse(cell.NumericCellValue.ToString(), out qty))
                        {
                            item.Quantity = qty;
                        }
                        else
                        {
                            BizExceptionHelper.Throw("Res_SO_ProductIDNoQuantity", item.ProductID);
                        }
                        if (qty <= 0)
                        {
                            BizExceptionHelper.Throw("Res_SO_ProductIDNoQuantityOne", item.ProductID);
                        }
                    }
                    else
                    {
                        BizExceptionHelper.Throw("Res_SO_ProductIDNoQuantityTwo", item.ProductID);
                    }
                    //价格补偿原因
                    cell = row.GetCell(3);
                    if (cell != null)
                    {
                        item.AdjustPriceReason = cell.StringCellValue;
                    }
                    else
                    {
                        item.AdjustPriceReason = "系统批量上传";
                    }

                    items.Add(item);
                } while (rows.MoveNext());

                int maxCount = 1000;
                if (items.Count > maxCount)
                {
                    BizExceptionHelper.Throw("Res_SO_ProductMaxItem", maxCount.ToString());
                }
                if (items.Count == 0)
                {
                    BizExceptionHelper.Throw("Res_SO_NoMoveNext");
                }

                #region 有效性验证

                //不能有重复的
                var groups = items.GroupBy(p => p.ProductID);
                foreach (var group in groups)
                {
                    if (group.Count() > 1)
                    {
                        BizExceptionHelper.Throw("Res_SO_ProductIDDuplicateData", group.Key);
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                BizExceptionHelper.Throw(ex.Message);
            }
            return items;
        }

        public bool UpdateSOStatusToReportedFailure(int sosysno)
        {
            return SODA.UpdateSOStatusToReportedFailure(sosysno);
        }

        /// <summary>
        /// 修改订单状态为 已申报待通关
        /// </summary>
        /// <param name="sosysno"></param>
        /// <returns></returns>
        public bool UpdateSOStatusToReported(int sosysno)
        {
            SOInfo so = SODA.GetSOBySOSysNo(sosysno);
            if (so.BaseInfo.Status != SOStatus.OutStock)
            {
                BizExceptionHelper.Throw("Res_SO_StatesIsNotOutStockForReported");
            }
            SODA.UpdateSOStatusToReported(sosysno);
            ObjectFactory<SOLogProcessor>.Instance.WriteSOLog(BizLogType.Sale_SO_Reported, "订单报关申报成功，通关中", so);
            return true;
        }

        /// <summary>
        /// 修改订单状态为 已通关发往顾客
        /// </summary>
        /// <param name="sosysno"></param>
        /// <returns></returns>
        public bool UpdateSOStatusToCustomsPass(int sosysno)
        {
            SOAction action = ObjectFactory<SOAction>.NewInstance(new string[] { SOType.General.ToString(), SOAction.SOCommand.OutStock.ToString() });
            action.CurrentSO = GetSOBySOSysNo(sosysno);
            action.Parameter = new object[] { null, SOStatus.CustomsPass };
            action.Do();
            return true;
        }

        /// <summary>
        /// 网关订单查询
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        public TransactionQueryBill QueryBill(string soSysNo)
        {
            return ExternalDomainBroker.QueryBill(soSysNo);
        }

        public ECCentral.BizEntity.Invoice.SOIncomeInfo GetValidSOIncomeInfo(int orderSysNo)
        {
            return ExternalDomainBroker.GetValidSOIncomeInfo(orderSysNo);
        }
        /// <summary>
        /// 根据订单编号获取关务对接相关信息
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        public VendorCustomsInfo LoadVendorCustomsInfo(int soSysNo)
        {
            return SODA.LoadVendorCustomsInfo(soSysNo);
        }

        public void SOMaintainUpdateNote(SOInfo info)
        {
             SODA.SOMaintainUpdateNote(info);
        }


        /// <summary>
        /// 是否归还库存
        /// </summary>
        /// <param name="productSysNo">商品系统编号</param>
        /// <param name="soCreatDate">订单创建时间</param>
        /// <returns>true:归还</returns>
        public bool CheckReturnInventory(int productSysNo, DateTime soCreatDate)
        {
            return SODA.CheckReturnInventory(productSysNo, soCreatDate);
        }

        /// <summary>
        /// 是否订单已经支付
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <returns>true:已支付，归还库存</returns>
        public bool NetpaySOCheckReturnInventory(int soSysNo)
        {
            bool result = false;
            ECCentral.BizEntity.Invoice.NetPayInfo NetPay = new BizEntity.Invoice.NetPayInfo();
            NetPay = SODA.GetCenterDBNetpayBySOSysNo(soSysNo);
            if (NetPay != null && NetPay.Status > (int)ECCentral.BizEntity.Invoice.NetPayStatus.Origin)
            {
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }
    }
}
