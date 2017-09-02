using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity;
using ECCentral.Service.PO.IDataAccess;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.PO.BizProcessor
{
    /// <summary>
    /// 采购篮 - BizProcessor
    /// </summary>
    [VersionExport(typeof(PurchaseOrderBasketProcessor))]
    public class PurchaseOrderBasketProcessor
    {
        #region [Fields]
        private IPurchaseOrderBasketDA m_BasketDA;
        private IVendorDA m_VendorDA;
        private VendorProcessor m_VendorProcessor;
        private PurchaseOrderProcessor m_PurchaseOrderProcessor;

        public IPurchaseOrderBasketDA BasketDA
        {
            get
            {
                if (null == m_BasketDA)
                {
                    m_BasketDA = ObjectFactory<IPurchaseOrderBasketDA>.Instance;
                }
                return m_BasketDA;
            }
        }

        public IVendorDA VendorDA
        {
            get
            {
                if (null == m_VendorDA)
                {
                    m_VendorDA = ObjectFactory<IVendorDA>.Instance;
                }
                return m_VendorDA;
            }
        }

        public VendorProcessor VendorProcessor
        {
            get
            {
                if (null == m_VendorProcessor)
                {
                    m_VendorProcessor = ObjectFactory<VendorProcessor>.Instance;
                }
                return m_VendorProcessor;
            }
        }

        public PurchaseOrderProcessor PurchaseOrderProcessor
        {
            get
            {
                if (null == m_PurchaseOrderProcessor)
                {
                    m_PurchaseOrderProcessor = ObjectFactory<PurchaseOrderProcessor>.Instance;
                }
                return m_PurchaseOrderProcessor;
            }
        }
        #endregion
        /// <summary>
        /// 创建/更新 采购篮商品
        /// </summary>
        /// <param name="basketInfo"></param>
        /// <returns></returns>
        public virtual BasketItemsInfo SaveBasket(BasketItemsInfo basketInfo)
        {
            #region [Check 实体逻辑]
            //检查商品数量:
            if (!basketInfo.Quantity.HasValue || basketInfo.Quantity.Value == 0)
            {
                //{0}:该商品数量不能为空
                throw new BizException(string.Format(GetMessageString("Basket_ProductQtyEmpty"), basketInfo.ProductID));
            }
            if (!basketInfo.OrderPrice.HasValue)
            {
                //{0}:该商品结算价不能为空！
                throw new BizException(string.Format(GetMessageString("Basket_SettlePriceEmpty"), basketInfo.ProductID));
            }
            //商品编号:
            if (!basketInfo.ProductSysNo.HasValue || basketInfo.ProductSysNo == 0)
            {
                //{0}:该商品编号不能为空！
                throw new BizException(string.Format(GetMessageString("Basket_ItemSysNoEmpty"), basketInfo.ProductID));
            }
            //商品是否存在于采购篮中:
            else if (BasketDA.CheckProductHasExistInBasket(basketInfo))
            {
                //{0}:该商品已存在于采购篮中！
                throw new BizException(string.Format(GetMessageString("Basket_ItemExists"), basketInfo.ProductID));
            }
            #endregion

            //保存和更新操作:
            if (basketInfo.ItemSysNo == null || !basketInfo.ItemSysNo.HasValue || basketInfo.ItemSysNo.Value == 0)
            {
                //如果不存在SysNo,则为新建操作:
                basketInfo = BasketDA.CreateBasketItem(basketInfo);
                //写LOG：
                //CommonService.WriteLog<BasketItemEntity>(entity, " Created BaksetItem ", entity.SysNo.Value.ToString(), (int)LogType.PO_Basket_Insert);
                ExternalDomainBroker.CreateLog(" Created BaksetItem "
              , BizEntity.Common.BizLogType.Purchase_Basket_Insert
              , basketInfo.ItemSysNo.Value
              , basketInfo.CompanyCode);

            }
            else
            {
                basketInfo = BasketDA.UpdateBasketItem(basketInfo);
                //写LOG：
                //CommonService.WriteLog<BasketItemEntity>(entity, " Updated BaksetItem ", entity.SysNo.Value.ToString(), (int)LogType.PO_Basket_Update);

                ExternalDomainBroker.CreateLog(" Updated BaksetItem "
            , BizEntity.Common.BizLogType.Purchase_Basket_Update
            , basketInfo.ItemSysNo.Value
            , basketInfo.CompanyCode);
            }
            return basketInfo;
        }

        /// <summary>
        /// 根据编号加载采购单商品信息
        /// </summary>
        /// <param name="basketSysNo"></param>
        /// <returns></returns>
        public virtual BasketItemsInfo LoadBasketItemInfoBySysNo(int? basketSysNo)
        {
            return BasketDA.LoadBasketItemBySysNo(basketSysNo);
        }

        public virtual int CreateBasketItemsForPrepare(List<BasketItemsInfo> prepareInfo)
        {
            int createCount = 0;
            if (null != prepareInfo)
            {
                prepareInfo.ForEach(x =>
                {
                    CreateBasketItemForPrepare(x);
                    createCount++;
                });
            }
            return createCount;
        }
        /// <summary>
        /// 创建采购篮商品(备货中心用)
        /// </summary>
        /// <param name="prepareInfo"></param>
        /// <returns></returns>
        public virtual BasketItemsInfo CreateBasketItemForPrepare(BasketItemsInfo prepareInfo)
        {
            if (BasketDA.CheckProductHasExistInBasket(prepareInfo) && !prepareInfo.ItemSysNo.HasValue)
            {
                //添加采购篮中item记录是否重复判断条件：采购员，供应商，item，目标仓库
                //{0}:该商品已存在于采购篮中！
                throw new BizException(string.Format(GetMessageString("Basket_ItemExists"), prepareInfo.ProductID));
            }

            prepareInfo.LastVendorSysNo = BasketDA.GetVendorSysNoByProductNoAndStockSysNo(prepareInfo.ProductSysNo.Value, prepareInfo.StockSysNo.Value);

            #region [Check 实体逻辑]

            if (!prepareInfo.Quantity.HasValue || prepareInfo.Quantity == 0)
            {
                //{0}:该商品数量不能为空！
                throw new BizException(string.Format(GetMessageString("Basket_ProductQtyEmpty"), prepareInfo.ProductID));
            }

            if (!prepareInfo.OrderPrice.HasValue)
            {
                //{0}:该商品结算价不能为空！
                throw new BizException(string.Format(GetMessageString("Basket_SettlePriceEmpty"), prepareInfo.ProductID));
            }

            if (!prepareInfo.ProductSysNo.HasValue || prepareInfo.ProductSysNo == 0)
            {
                //{0}:该商品编号不能为空！
                throw new BizException(string.Format(GetMessageString("Basket_ItemSysNoEmpty"), prepareInfo.ProductID));
            }

            else if (BasketDA.CheckProductHasExistInBasket(prepareInfo) && !prepareInfo.ItemSysNo.HasValue)
            {
                //添加采购篮中item记录是否重复判断条件：采购员，供应商，item，目标仓库
                //{0}:该商品已存在于采购篮中！
                throw new BizException(string.Format(GetMessageString("Basket_ItemExists"), prepareInfo.ProductID));
            }
            #endregion

            //新建操作:
            prepareInfo = BasketDA.CreateBasketItemForPrepare(prepareInfo);
            //写LOG：
            // CommonService.WriteLog<BasketItemEntity>(entity, " Created BaksetItem For Prepare ", entity.SysNo.Value.ToString(), (int)LogType.PO_Basket_Insert);

            ExternalDomainBroker.CreateLog(" Created BaksetItem For Prepare "
                                        , BizEntity.Common.BizLogType.Purchase_Basket_Insert
                                        , prepareInfo.ItemSysNo.Value
                                        , prepareInfo.CompanyCode);

            return prepareInfo;

        }

        /// <summary>
        /// 采购篮添加赠品
        /// </summary>
        /// <param name="itemInfo"></param>
        /// <returns></returns>
        public virtual BasketItemsInfo CreateGiftForBasket(BasketItemsInfo itemInfo)
        {
            #region [Check 实体逻辑]
            if (!itemInfo.Quantity.HasValue || itemInfo.Quantity == 0)
            {
                //{0}:该商品数量不能为空！
                throw new BizException(string.Format(GetMessageString("Basket_ProductQtyEmpty"), itemInfo.ProductID));
            }

            if (!itemInfo.OrderPrice.HasValue)
            {
                //"{0}:该商品结算价不能为空！"
                throw new BizException(string.Format(GetMessageString("Basket_SettlePriceEmpty"), itemInfo.ProductID));
            }

            if (!itemInfo.ProductSysNo.HasValue || itemInfo.ProductSysNo == 0)
            {
                //{0}:该商品编号不能为空！
                throw new BizException(string.Format(GetMessageString("Basket_ItemSysNoEmpty"), itemInfo.ProductID));
            }

            if (itemInfo.ItemSysNo == null || !itemInfo.ItemSysNo.HasValue || itemInfo.ItemSysNo.Value == 0)
            {
                //如果ItemSysNo 为空，则为添加操作 :
                return BasketDA.CreateBasketItemForPrepare(itemInfo);
            }
            else
            {
                return BasketDA.UpdateBasketItemForGift(itemInfo);
            }
            #endregion
        }

        /// <summary>
        /// 批量更新采购篮商品
        /// </summary>
        /// <param name="itemList"></param>
        /// <returns></returns>
        public virtual List<BasketItemsInfo> BatchUpdateBasketItems(List<BasketItemsInfo> itemList)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                if (null != itemList && 0 < itemList.Count)
                {
                    foreach (BasketItemsInfo entity in itemList)
                    {
                        #region [Check 实体逻辑]

                        if (entity == null)
                        {
                            throw new ArgumentNullException("entity");
                        }

                        if (entity.ItemSysNo == 0)
                        {
                            //系统编号不能为空
                            throw new BizException(GetMessageString("Basket_SysNoEmpty"));
                        }

                        if (!entity.OrderPrice.HasValue || entity.OrderPrice < 0)
                        {
                            //{0}:该商品结算价不能为空！
                            throw new BizException(string.Format(GetMessageString("Basket_SettlePriceEmpty"), entity.ProductSysNo.Value));
                        }

                        if (!entity.Quantity.HasValue || entity.Quantity == 0)
                        {
                            //{0}:该商品数量不能为空！
                            throw new BizException(string.Format(GetMessageString("Basket_ProductQtyEmpty"), entity.ProductSysNo.Value));
                        }

                        //目标分仓
                        if (!entity.StockSysNo.HasValue || entity.StockSysNo == 0)
                        {
                            //{0}:该商品目标分仓为空！
                            throw new BizException(string.Format(GetMessageString("Basket_StockEmpty"), entity.ProductSysNo.Value));
                        }

                        if (!entity.IsTransfer.HasValue)//是否中转
                        {
                            //{0}:该商品是否中转为空！
                            throw new BizException(string.Format(GetMessageString("Basket_TransferEmpty"), entity.ProductSysNo.Value));
                        }
                        #endregion
                        BasketDA.UpdateBasketItem(entity);
                    }
                }
                ts.Complete();
            }
            return itemList;
        }

        /// <summary>
        /// 批量删除采购篮商品
        /// </summary>
        /// <param name="itemList"></param>
        /// <returns></returns>
        public virtual List<BasketItemsInfo> BatchDeleteBasketItems(List<BasketItemsInfo> itemList)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                if (null != itemList && 0 < itemList.Count)
                {
                    foreach (BasketItemsInfo entity in itemList)
                    {
                        if (!entity.ItemSysNo.HasValue || entity.ItemSysNo == 0)
                        {
                            //系统编号不能为空！
                            throw new BizException(GetMessageString("Basket_SysNoEmpty"));
                        }

                        BasketDA.DeleteBasketItem(entity);
                        //写LOG:
                        // CommonService.WriteLog<BasketItemEntity>(entity, " Deleted BaksetItem ", entity.SysNo.Value.ToString(), (int)LogType.PO_Basket_Delete);
                        ExternalDomainBroker.CreateLog(" Deleted BaksetItem "
                                    , BizEntity.Common.BizLogType.Purchase_Basket_Delete
                                    , entity.ItemSysNo.Value
                                    , entity.CompanyCode);
                    }
                }
                scope.Complete();
            }
            return itemList;
        }

        /// <summary>
        /// 验证导入的采购篮数据是否合法并创建采购篮
        /// </summary>
        /// <param name="itemList"></param>
        /// <returns></returns>
        public virtual List<BasketItemsInfo> BatchImportAndCreateBasketItem(List<BasketItemsInfo> itemList, bool isThrowException)
        {
            if (null == itemList || 0 == itemList.Count)
            {
                //没有需要导入的数据
                throw new BizException(GetMessageString("Basket_NoDataImport"));
            }
            foreach (var entity in itemList)
            {
                #region [Check实体逻辑]
                if (!entity.LastVendorSysNo.HasValue)
                {
                    //供应商编号无效
                    entity.ErrorMessage += GetMessageString("Basket_VendorSysNoInvalid");
                    if (isThrowException)
                    {
                        throw new BizException(GetMessageString("Basket_VendorSysNoInvalid"));
                    }

                }
                var vendor = VendorDA.LoadVendorInfo(entity.LastVendorSysNo.Value);


                if (vendor == null)
                {
                    //供应商无效
                    entity.ErrorMessage += GetMessageString("Basket_VendorInvalid");
                    if (isThrowException)
                    {
                        throw new BizException(GetMessageString("Basket_VendorInvalid"));
                    }
                }
                else
                {
                    entity.CompanyCode = vendor.CompanyCode;
                    entity.VendorSysNo = vendor.SysNo.Value;
                }

                if (!entity.Quantity.HasValue || entity.Quantity == 0)
                {
                    //{0}:该商品数量不能为空！
                    entity.ErrorMessage += string.Format(GetMessageString("Basket_ProductQtyEmpty"), entity.ProductID);
                    if (isThrowException)
                    {
                        throw new BizException(string.Format(GetMessageString("Basket_ProductQtyEmpty"), entity.ProductID));
                    }
                }

                if (!entity.OrderPrice.HasValue)
                {
                    //采购价格为空
                    entity.ErrorMessage += GetMessageString("Basket_PurchasePriceEmpty");
                    if (isThrowException)
                    {
                        throw new BizException(GetMessageString("Basket_PurchasePriceEmpty"));
                    }
                }

                entity.ProductSysNo = BasketDA.GetItemSysNoByItemID(entity.ProductID, entity.CompanyCode);

                if (!entity.ProductSysNo.HasValue || entity.ProductSysNo == 0)
                {
                    entity.ErrorMessage += GetMessageString("Basket_ProductSysNoInvalid");
                    if (isThrowException)
                    {
                        throw new BizException(GetMessageString("Basket_ProductSysNoInvalid"));
                    }
                }

                entity.StockSysNo = BasketDA.GetStockSysNoByName(entity.StockName, entity.CompanyCode);

                if (!entity.StockSysNo.HasValue || entity.StockSysNo == 0)
                {
                    entity.ErrorMessage += GetMessageString("Basket_StocknameInvalid");
                    if (isThrowException)
                    {
                        throw new BizException(GetMessageString("Basket_StocknameInvalid"));
                    }
                }

                if (BasketDA.CheckProductHasExistInBasket(entity) && !entity.ItemSysNo.HasValue)
                {
                    //添加采购篮中item记录是否重复判断条件：采购员，供应商，item，目标仓库
                    //{0}:该商品已存在于采购篮中！
                    entity.ErrorMessage += string.Format(GetMessageString("Basket_ItemExists"), entity.ProductID);
                    if (isThrowException)
                    {
                        throw new BizException(string.Format(GetMessageString("Basket_ItemExists"), entity.ProductID));
                    }
                }
                #endregion

                if (string.IsNullOrEmpty(entity.ErrorMessage))
                {
                    entity.ReadyQuantity = 0;
                    var resultEntity = BasketDA.CreateBasketItemForPrepare(entity);
                    //写LOG：
                    //CommonService.WriteLog<BasketItemEntity>(entity, " Batch Created BaksetItem ", entity.SysNo.Value.ToString(), (int)LogType.PO_Basket_Insert);

                    ExternalDomainBroker.CreateLog(" Batch Created BaksetItem "
                                       , BizEntity.Common.BizLogType.Purchase_Basket_Insert
                                       , entity.ItemSysNo.Value
                                       , entity.CompanyCode);
                }
            }
            return itemList;
        }

        /// <summary>
        /// 填充每个item的产品线相关数据
        /// </summary>
        /// <param name="list"></param>
        private void FillProductLineInfo(List<BasketItemsInfo> list)
        {
            List<int> tProList = list.Select(x => x.ProductSysNo.HasValue ? x.ProductSysNo.Value : 0).ToList<int>();
            List<ProductPMLine> tList = ExternalDomainBroker.GetProductLineSysNoByProductList(tProList.ToArray());
            tList.ForEach(x =>
            {
                list.ForEach(item =>
                {
                    if (x.ProductSysNo == item.ProductSysNo)
                    {
                        item.ProductLine_SysNo =x.ProductLineSysNo;
                        item.ProductLine_PMSysNo = x.PMSysNo;
                    }
                });
            });
        }

        /// <summary>
        /// 批量创建PO单:
        /// </summary>
        /// <param name="list"></param>
        public virtual BatchCreateBasketResultInfo BatchCreatePurchaseOrder(List<BasketItemsInfo> list)
        {
            List<ProductPMLine> tPMLineList = ExternalDomainBroker.GetProductLineInfoByPM(ServiceContext.Current.UserSysNo);
            bool tIsManager = list[0].IsManagerPM.Value;
            //如果没有产品线权限并且不是高级PM，则不允许创建
            //if ((tPMLineList == null || tPMLineList.Count == 0) && !tIsManager)
            //{
            //    throw new BizException(GetMessageString("Basket_PMNoLine"));
            //}
            FillProductLineInfo(list);
            string createCompanyCode = string.Empty;
            var conditionList = new List<BasketItemsInfo>();
            List<int> POSysNo = new List<int>();
            string errorMsg = "";
            //根据编号加载商品信息:
            string exceptionMsg = CheckBasketInfo(ref list);
            errorMsg += exceptionMsg;
            List<PurchaseOrderInfo> POs = new List<PurchaseOrderInfo>();
            //if (!string.IsNullOrEmpty(exceptionMsg) || exceptionMsg.Length > 0)
            //{
            //    throw new BizException(exceptionMsg);
            //}
            list.ForEach(x =>
            {
                //是否中转:
                if (x.IsTransfer == 1)
                {
                    x.StockSysNo = 5000 + x.StockSysNo;
                }
                createCompanyCode = x.CompanyCode;
            });

            //默认采购单类型为：正常；增值税率:0.17；结算货币:人民币(1)；送货类型:厂房直送(12)           
            var group = from item in list
                        group item by new { item.IsConsign, item.StockSysNo, item.VendorSysNo, item.PMSysNo, item.IsTransfer ,item.ProductLine_SysNo}
                            into g
                            select new
                            {
                                Key = g.Key,
                                ResultList = g
                            };
            foreach (var item in group)
            {
                POs.Clear();
                var ZPO = from i in item.ResultList
                          where i.Quantity >= 0
                          select i;
                var FPO = from i in item.ResultList
                          where i.Quantity < 0
                          select i;
                //正采购
                #region ZPO
                if (ZPO.Count() > 0)
                {
                    PurchaseOrderInfo modelPO = GetInitPO(createCompanyCode);
                    modelPO.VendorInfo = VendorProcessor.LoadVendorInfo(item.Key.VendorSysNo.Value);

                    modelPO.PurchaseOrderBasicInfo.ConsignFlag = (PurchaseOrderConsignFlag)Enum.Parse(typeof(PurchaseOrderConsignFlag), item.Key.IsConsign.Value.ToString());
                    if (null == modelPO.PurchaseOrderBasicInfo.StockInfo)
                    {
                        modelPO.PurchaseOrderBasicInfo.StockInfo = new BizEntity.Inventory.StockInfo();
                    }
                    modelPO.PurchaseOrderBasicInfo.StockInfo.SysNo = item.Key.StockSysNo;
                    if (null == modelPO.PurchaseOrderBasicInfo.ProductManager)
                    {
                        modelPO.PurchaseOrderBasicInfo.ProductManager = new BizEntity.IM.ProductManagerInfo();
                    }
                    //注释原因：因加入产品线验证，此处的归属PM应为产品线的PM
                    //modelPO.PurchaseOrderBasicInfo.ProductManager.SysNo = item.Key.PMSysNo;
                    modelPO.PurchaseOrderBasicInfo.PurchaseOrderType = PurchaseOrderType.Normal;
                    modelPO.PurchaseOrderBasicInfo.PayType.SysNo = VendorDA.LoadVendorPayPeriodType(item.Key.VendorSysNo);
                    string companyCode = string.Empty;
                    foreach (var POitem in ZPO)
                    {
                        modelPO.PurchaseOrderBasicInfo.PurchaseOrderType = PurchaseOrderType.Normal;
                        //增加产品线和主PM字段回写
                        modelPO.PurchaseOrderBasicInfo.IsManagerPM = POitem.IsManagerPM;
                        modelPO.PurchaseOrderBasicInfo.ProductManager.SysNo = POitem.PMSysNo;
                        modelPO.PurchaseOrderBasicInfo.ProductLineSysNo = POitem.ProductLine_SysNo;
                        modelPO.POItems.Add(new PurchaseOrderItemInfo()
                        {
                            ItemSysNo = POitem.ItemSysNo,
                            ProductSysNo = POitem.ProductSysNo,
                            Quantity = 0,
                            ReadyQuantity = POitem.ReadyQuantity,
                            OrderPrice = POitem.OrderPrice,
                            ProductID = POitem.ProductID,
                            PurchaseQty = POitem.Quantity,
                            UnitCost = Decimal.Round(POitem.OrderPrice.Value * modelPO.PurchaseOrderBasicInfo.ExchangeRate.Value, 2),
                            Weight = POitem.Weight,
                            ApportionAddOn = 0,
                            ReturnCost = 0,
                            BriefName = POitem.BriefName,
                            AvailableQty = BasketDA.AvailableQtyByProductSysNO(POitem.ProductSysNo.Value),
                            M1 = BasketDA.M1ByProductSysNO(POitem.ProductSysNo.Value),
                            JingDongPrice = BasketDA.JDPriceByProductSysNO(POitem.ProductSysNo.Value),
                            CompanyCode = POitem.CompanyCode
                        });
                        companyCode = POitem.CompanyCode;
                    }
                    modelPO.CompanyCode = companyCode;
                    POs.Add(modelPO);
                }
                #endregion

                //负采购
                #region FPO
                if (FPO.Count() > 0)
                {
                    PurchaseOrderInfo modelPO = GetInitPO(createCompanyCode);
                    modelPO.VendorInfo = VendorProcessor.LoadVendorInfo(item.Key.VendorSysNo.Value);
                    modelPO.PurchaseOrderBasicInfo.ConsignFlag = (PurchaseOrderConsignFlag)Enum.Parse(typeof(PurchaseOrderConsignFlag), item.Key.IsConsign.Value.ToString());
                    modelPO.PurchaseOrderBasicInfo.StockInfo.SysNo = item.Key.StockSysNo;
                    modelPO.PurchaseOrderBasicInfo.ProductManager.SysNo = item.Key.PMSysNo;
                    modelPO.PurchaseOrderBasicInfo.PurchaseOrderType = PurchaseOrderType.Negative;
                    modelPO.PurchaseOrderBasicInfo.PayType.SysNo = VendorDA.LoadVendorPayPeriodType(item.Key.VendorSysNo);
                    modelPO.PurchaseOrderBasicInfo.MemoInfo.Memo = GetMessageString("Basket_Memo_FPO");
                    string companyCode = string.Empty;
                    foreach (var POitem in FPO)
                    {
                        //增加产品线和主PM字段回写
                        modelPO.PurchaseOrderBasicInfo.IsManagerPM = POitem.IsManagerPM;
                        modelPO.PurchaseOrderBasicInfo.ProductManager.SysNo = POitem.ProductLine_PMSysNo;
                        modelPO.PurchaseOrderBasicInfo.ProductLineSysNo = POitem.ProductLine_SysNo;
                        modelPO.POItems.Add(new PurchaseOrderItemInfo()
                        {
                            ItemSysNo = POitem.ItemSysNo,
                            ProductSysNo = POitem.ProductSysNo,
                            Quantity = 0,
                            OrderPrice = POitem.OrderPrice,
                            ProductID = POitem.ProductID,
                            PurchaseQty = POitem.Quantity,
                            UnitCost = Decimal.Round(POitem.OrderPrice.Value * modelPO.PurchaseOrderBasicInfo.ExchangeRate.Value, 2),
                            Weight = POitem.Weight,
                            ApportionAddOn = 0,
                            ReturnCost = 0,
                            BriefName = POitem.BriefName,
                            AvailableQty = BasketDA.AvailableQtyByProductSysNO(POitem.ProductSysNo.Value),
                            M1 = BasketDA.M1ByProductSysNO(POitem.ProductSysNo.Value),
                            JingDongPrice = BasketDA.JDPriceByProductSysNO(POitem.ProductSysNo.Value),
                            CompanyCode = POitem.CompanyCode
                        });
                        companyCode = POitem.CompanyCode;
                    }
                    modelPO.CompanyCode = companyCode;
                    POs.Add(modelPO);
                }
                #endregion
                int poItemCountNumber = 80;

                if (!string.IsNullOrEmpty(AppSettingManager.GetSetting("PO", "PoItemCountNumber")))
                {
                    poItemCountNumber = int.Parse(AppSettingManager.GetSetting("PO", "PoItemCountNumber"));
                }
                foreach (var poentity in POs)
                {
                    try
                    {
                        if (poentity.PurchaseOrderBasicInfo.MemoInfo == null)
                        {
                            poentity.PurchaseOrderBasicInfo.MemoInfo = new PurchaseOrderMemoInfo();
                        }
                        if (poentity.PurchaseOrderBasicInfo.ETATimeInfo == null)
                        {
                            poentity.PurchaseOrderBasicInfo.ETATimeInfo = new PurchaseOrderETATimeInfo();
                        }
                        if (poentity.EIMSInfo == null)
                        {
                            poentity.EIMSInfo = new PurchaseOrderEIMSInfo() { EIMSInfoList = new List<EIMSInfo>() };
                        }
                        if (poentity.ReceivedInfoList == null)
                        {
                            poentity.ReceivedInfoList = new List<PurchaseOrderReceivedInfo>();
                        }
                        //如果在一个po单中Item超过80种，则自动拆分 每80种商品一单
                        if (poentity.POItems.Count > poItemCountNumber)
                        {
                            //string listvalue = SerializeHelper.JsonSerializer(poentity);
                            PurchaseOrderInfo posResult = poentity;
                            //SerializeHelper.JsonDeserialize<IPP.Oversea.CN.POASNMgmt.WebModel.ViewModels.PO.POModel>(listvalue);
                            var poitems = new List<PurchaseOrderItemInfo>();
                            for (int i = 1; i <= poentity.POItems.Count; i++)
                            {
                                poitems.Add(poentity.POItems[i - 1]);
                                if ((i % poItemCountNumber == 0 && i != 0) || i == poentity.POItems.Count)
                                {
                                    posResult.POItems = poitems;
                                    var poResult = PurchaseOrderProcessor.CreatePO(posResult);
                                    POSysNo.Add(poResult.SysNo.Value);
                                    poitems.Clear();
                                }
                            }
                        }
                        else
                        {
                            var poResult = PurchaseOrderProcessor.CreatePO(poentity);
                            POSysNo.Add(poResult.SysNo.Value);
                        }
                    }
                    catch (Exception ex)
                    {
                        errorMsg += ex.Message + Environment.NewLine;
                    }
                }
            }
            return new BatchCreateBasketResultInfo() { SucessPOSysNos = POSysNo, ErrorMsg = errorMsg };
        }

        /// <summary>
        /// 批量添加赠品操作
        /// </summary>
        /// <param name="sysNoList"></param>
        public virtual void BatchAddGift(List<BasketItemsInfo> list)
        {
            List<BasketItemsInfo> newItemlist = new List<BasketItemsInfo>();
            string WarningMsg = string.Empty;

            #region 持久化列表中数据到数据库中
            BatchUpdateBasketItems(list);
            #endregion

            if (list != null && list.Count > 0)
            {
                #region  查询采购蓝中item的赠品，验证赠品数量是否合主商品相等
                List<int> psysno = new List<int>();
                foreach (BasketItemsInfo bitem in list)
                {
                    psysno.Add(bitem.ProductSysNo.Value);
                }
                psysno.Distinct();

                List<BasketItemsInfo> giftResultList = BasketDA.LoadGiftItemByBasketItem(psysno);

                foreach (var bitem in list)
                {
                    if (!bitem.VendorSysNo.HasValue || bitem.VendorSysNo == 0 || !bitem.StockSysNo.HasValue || !bitem.IsTransfer.HasValue)
                    {
                        #region  验证信息是否完整。
                        if (WarningMsg != string.Empty)
                        {
                            WarningMsg += "," + Environment.NewLine;
                        }
                        WarningMsg += GetMessageString("Basket_CheckInfo_1") + bitem.ProductSysNo + GetMessageString("Basket_CheckInfo_2");
                        if (bitem.VendorSysNo == 0 || !bitem.VendorSysNo.HasValue)
                        {
                            WarningMsg += GetMessageString("Basket_CheckInfo_Vendor");
                        }
                        if (!bitem.StockSysNo.HasValue)
                        {
                            WarningMsg += GetMessageString("Basket_CheckInfo_Stock");
                        }
                        if (!bitem.IsTransfer.HasValue)
                        {
                            WarningMsg += GetMessageString("Basket_CheckInfo_Transfer");
                        }
                        #endregion
                    }
                }
                if (!string.IsNullOrEmpty(WarningMsg))
                {
                    //WarningMsg += "等关键信息，请补全关键信息后再批量添加其赠品!";
                    WarningMsg += GetMessageString("Basket_CheckInfo_Desc");
                    throw new BizException(WarningMsg);
                }
                var group = from item in list
                            group item by new { item.StockSysNo }
                                into g
                                select new
                                {
                                    Key = g.Key,
                                    ResultList = g
                                };
                foreach (var item in group)
                {
                    var newbasketItem = from i in item.ResultList
                                        where i.Quantity >= 0
                                        select i
                                        ;

                    if (giftResultList != null && giftResultList.Count > 0)
                    {
                        foreach (BasketItemsInfo giftentity in giftResultList)
                        {
                            #region 判断赠品数量
                            int masterQty = 0;
                            int giftQty = 0;
                            string stockName = string.Empty;
                            int stocksysno = 0;
                            foreach (var bitem in newbasketItem)
                            {
                                if (bitem.VendorSysNo != 0 && bitem.StockSysNo.HasValue && bitem.IsTransfer.HasValue)
                                {
                                    #region 验证赠品与主商品数量
                                    if (giftentity.MasterProductSysNo == bitem.ProductSysNo)
                                    {
                                        masterQty = bitem.Quantity.Value;
                                        stockName = bitem.StockName;
                                        stocksysno = bitem.StockSysNo.Value;
                                        if (giftQty != 0)
                                        {
                                            foreach (var bi in newbasketItem)
                                            {
                                                if (bi.ProductSysNo == giftentity.GiftSysNo)
                                                {
                                                    bi.Quantity -= masterQty;
                                                }
                                            }
                                        }
                                    }
                                    if (giftentity.GiftSysNo == bitem.ProductSysNo)
                                    {
                                        giftQty += bitem.Quantity.Value;
                                        stockName = bitem.StockName;
                                        stocksysno = bitem.StockSysNo.Value;
                                        if (masterQty != 0)
                                        {
                                            bitem.Quantity -= masterQty;
                                        }
                                    }
                                    #endregion
                                }
                            }
                            #endregion

                            #region 添加赠品
                            if (masterQty != giftQty && masterQty > giftQty)
                            {
                                //先验证采购蓝中是否包含此商品，如果包含并且供应商，目标分仓，
                                //是否中转都一样则直接改采购数量；如果没有或不一样，则新建记录

                                foreach (BasketItemsInfo entity in list)
                                {
                                    //新建                                       
                                    if (entity.ProductSysNo == giftentity.MasterProductSysNo
                                        && entity.StockSysNo == stocksysno)
                                    {
                                        int basketsysno = BasketDA.CheckGiftInBasket(entity, giftentity.GiftSysNo.Value);
                                        if (basketsysno != 0)
                                        {
                                            BasketItemsInfo basketItem = new BasketItemsInfo()
                                            {
                                                ItemSysNo = basketsysno,
                                                CreateUserSysNo = entity.CreateUserSysNo,
                                                OrderPrice = entity.OrderPrice,
                                                Quantity = masterQty - giftQty,
                                                LastVendorSysNo = entity.VendorSysNo,
                                                ProductSysNo = giftentity.GiftSysNo,
                                                IsTransfer = entity.IsTransfer,
                                                StockSysNo = entity.StockSysNo,
                                                CompanyCode = entity.CompanyCode,
                                                ProductID = entity.ProductID,
                                                ReadyQuantity = entity.ReadyQuantity,
                                                IsConsign = entity.IsConsign,
                                                GiftSysNo = entity.GiftSysNo,
                                                PMSysNo = entity.PMSysNo
                                            };
                                            newItemlist.Add(basketItem);
                                            if (newItemlist.Count > 0)
                                            {
                                                //采购篮添加赠品操作:
                                                newItemlist.ForEach(x =>
                                                {
                                                    CreateGiftForBasket(x);
                                                });
                                                newItemlist.Clear();
                                            }
                                        }
                                        else
                                        {
                                            //获取赠品信息:
                                            BasketItemsInfo gift = BasketDA.LoadBasketGiftInfo(giftentity.GiftSysNo.Value);
                                            BasketItemsInfo basketItem = new BasketItemsInfo()
                                            {
                                                OrderPrice = gift.OrderPrice,
                                                Quantity = masterQty - giftQty,
                                                LastVendorSysNo = entity.VendorSysNo,
                                                ProductSysNo = giftentity.GiftSysNo,
                                                IsTransfer = entity.IsTransfer,
                                                CreateTime = DateTime.Now,
                                                StockSysNo = entity.StockSysNo,
                                                CompanyCode = entity.CompanyCode,
                                                ProductID = entity.ProductID,
                                                ReadyQuantity = entity.ReadyQuantity,
                                                CreateUserSysNo = entity.CreateUserSysNo,
                                                IsConsign = entity.IsConsign,
                                                GiftSysNo = entity.GiftSysNo,
                                                PMSysNo = entity.PMSysNo
                                            };
                                            newItemlist.Add(basketItem);
                                            if (newItemlist.Count > 0)
                                            {
                                                //采购篮添加赠品操作:
                                                newItemlist.ForEach(x =>
                                                {
                                                    CreateGiftForBasket(x);
                                                });
                                                newItemlist.Clear();
                                            }
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }
                #endregion
            }

        }

        public virtual List<BasketItemsInfo> GetGiftBasketItems(List<int> productSysNoList)
        {
            return BasketDA.LoadGiftItemByBasketItem(productSysNoList);
        }


        /// <summary>
        /// 验证采购篮商品和供应商代销类型一致:
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private string CheckBasketInfo(ref List<BasketItemsInfo> list)
        {
            StringBuilder strBuid = new StringBuilder();
            List<BasketItemsInfo> tOpList = new List<BasketItemsInfo>();
            foreach (BasketItemsInfo item in list)
            {
                if (item.VendorIsConsign.HasValue && item.IsConsign == item.VendorIsConsign.Value)
                {
                    //检测产品线
                    //if (item.ProductLine_SysNo == null || item.ProductLine_SysNo == 0)
                    //    strBuid.Append(string.Format(GetMessageString("Basket_ProductNoLine"), item.ProductID) + Environment.NewLine);
                    //else
                        tOpList.Add(item);
                }
                else if (!item.VendorIsConsign.HasValue)
                {
                    strBuid.Append(string.Format(GetMessageString("Basket_ProductNoVendor"), item.ProductID) + Environment.NewLine);
                }
                else
                    strBuid.Append(string.Format(GetMessageString("Basket_ConsignNotTheSame"), item.ProductID, item.VendorName) + Environment.NewLine);
            }
            list = tOpList;

            return strBuid.ToString();
        }

        private PurchaseOrderInfo GetInitPO(string companyCode)
        {
            PurchaseOrderInfo modelPO = new PurchaseOrderInfo()
            {
                POItems = new List<PurchaseOrderItemInfo>(),
                PurchaseOrderBasicInfo = new PurchaseOrderBasicInfo(),
                VendorInfo = new VendorInfo() { VendorBasicInfo = new VendorBasicInfo() }
            };
            modelPO.PurchaseOrderBasicInfo.PurchaseOrderStatus = PurchaseOrderStatus.Created;
            modelPO.PurchaseOrderBasicInfo.SettleCompanySysNo = 3201;
            if (null == modelPO.PurchaseOrderBasicInfo.PayType)
            {
                modelPO.PurchaseOrderBasicInfo.PayType = new BizEntity.Common.PayType();
            }
            modelPO.PurchaseOrderBasicInfo.PayType.SysNo = 12;
            modelPO.PurchaseOrderBasicInfo.TaxRate = 0.17M;
            modelPO.PurchaseOrderBasicInfo.CurrencyCode = 1;
            if (null == modelPO.PurchaseOrderBasicInfo.ShippingType)
            {
                modelPO.PurchaseOrderBasicInfo.ShippingType = new BizEntity.Common.ShippingType();
            }
            modelPO.PurchaseOrderBasicInfo.ShippingType.SysNo = 12;
            modelPO.PurchaseOrderBasicInfo.ExchangeRate = ExternalDomainBroker.GetExchangeRateBySysNo(modelPO.PurchaseOrderBasicInfo.CurrencyCode.Value, companyCode);
            modelPO.CompanyCode = companyCode;
            return modelPO;
        }

        private List<BasketItemsInfo> GetBasketItemsInfoBySysNo(List<int> sysNos)
        {
            List<BasketItemsInfo> list = new List<BasketItemsInfo>();
            sysNos.ForEach(x =>
            {
                BasketItemsInfo getInfo = new BasketItemsInfo();
                if (x > 0)
                {
                    getInfo = BasketDA.LoadBasketItemBySysNo(x);
                    if (null != getInfo)
                    {
                        list.Add(getInfo);
                    }
                }
            });
            return list;
        }

        /// <summary>
        /// 获取异常信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetMessageString(string key)
        {
            return ResouceManager.GetMessageString("PO.PurchaseOrderBasket", key);
        }


    }
}
