using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using ECommerce.DataAccess.Inventory;
using ECommerce.DataAccess.Product;
using ECommerce.Entity.Common;
using ECommerce.Entity.Inventory;
using ECommerce.Entity.Product;
using ECommerce.Entity.Store.Vendor;
using ECommerce.Enums;
using ECommerce.Service.Common;
using ECommerce.Service.Inventory;
using ECommerce.Service.Store;
using ECommerce.Utility;

namespace ECommerce.Service.Product
{
    public static class ProductPurchaseService
    {
        public static QueryResult<ProductPurchaseQueryBasicInfo> QueryProductPurchase(ProductPurchaseQueryFilter queryCriteria)
        {
            int totalCount = 0;
            QueryResult<ProductPurchaseQueryBasicInfo> result = new QueryResult<ProductPurchaseQueryBasicInfo>();

            List<ProductPurchaseQueryBasicInfo> list =
                ProductPurchaseDA.QueryProductPurchase(queryCriteria, out totalCount);

            result.ResultList = list;
            result.PageInfo = new PageInfo
            {
                PageIndex = queryCriteria.PageIndex,
                PageSize = queryCriteria.PageSize,
                TotalCount = totalCount,
            };

            return result;
        }

        #region 商品库存调整单 相关Service:

        /// <summary>
        /// 查询商品库存调整单列表
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <returns></returns>
        public static QueryResult<ProductStockAdjustViewInfo> QueryProductStockAdjustmentList(ProductStockAdjustListQueryFilter queryCriteria)
        {
            return ProductDA.QueryProductStockAdjustmentList(queryCriteria);
        }

        /// <summary>
        /// 获取单个商品库存调整单信息
        /// </summary>
        /// <param name="adjustSysNo"></param>
        /// <returns></returns>
        public static ProductStockAdjustInfo GetProductStockAdjustmentInfo(int adjustSysNo)
        {
            var entity = ProductDA.GetProductStockAdjustmentInfo(adjustSysNo);
            if (null == entity)
            {
                throw new BusinessException(string.Format("找不到编号为{0}的商品库存调整单信息!"));
            }
            return entity;
        }

        /// <summary>
        /// 保存商品库存调整单信息
        /// </summary>
        /// <param name="adjustInfo"></param>
        public static void SaveProductStockAdjustmentInfo(ProductStockAdjustInfo adjustInfo)
        {
            #region [Check操作：]
            if (!adjustInfo.StockSysNo.HasValue)
            {
                throw new BusinessException("请选择一个入库仓库!");
            }
            if (null == adjustInfo.AdjustItemList || adjustInfo.AdjustItemList.Count <= 0)
            {
                throw new BusinessException("请添加至少一个商品!");
            }
            foreach (var adjustItem in adjustInfo.AdjustItemList)
            {
                var currentProductInfo = ProductService.GetProductBySysNo(adjustItem.ProductSysNo.Value);
                if (null == currentProductInfo || currentProductInfo.SysNo <= 0)
                {
                    throw new BusinessException(string.Format("商品编号{0}相关信息不存在!", adjustItem.ProductSysNo.Value));
                }
                //////自贸商品不能创建商品库存调整单:
                //else if (currentProductInfo.ProductTradeType == TradeType.FTA)
                //{
                //    throw new BusinessException(string.Format("商品{0}为自贸商品，不能创建商品库存调整单!", adjustItem.ProductID));
                //}
                else if (currentProductInfo.SellerSysNo.Value != adjustInfo.VendorSysNo.Value)
                {
                    throw new BusinessException(string.Format("商品{0}不属于当前商家，不能进行库存调整操作!", adjustItem.ProductID));
                }
            }

            #endregion
            using (TransactionScope ts = new TransactionScope())
            {
                ProductDA.SaveProductStockAdjustmentInfo(adjustInfo);
                ts.Complete();
            }
        }

        /// <summary>
        /// 更新商品库存调整单状态
        /// </summary>
        /// <param name="newStatus"></param>
        /// <returns></returns>
        public static int UpdateProductStockAdjustmentStatus(ProductStockAdjustStatus newStatus, int adjustSysNo, int currentUserSysNo)
        {
            var currentAdjustInfo = GetProductStockAdjustmentInfo(adjustSysNo);
            if (null == currentAdjustInfo)
            {
                throw new BusinessException(string.Format("找不到编号为{0}的商品库存调整单据！"));
            }
            switch (newStatus)
            {
                case ProductStockAdjustStatus.Abandon:
                case ProductStockAdjustStatus.WaitingAudit:
                    if (currentAdjustInfo.Status != ProductStockAdjustStatus.Origin)
                    {
                        throw new BusinessException(string.Format("操作失败:编号为{0}的商品库存调整单据当前状态不为\"初始态\"！", adjustSysNo));

                    }
                    break;
                case ProductStockAdjustStatus.AuditFaild:
                case ProductStockAdjustStatus.AuditPass:
                    if (currentAdjustInfo.Status != ProductStockAdjustStatus.WaitingAudit)
                    {
                        throw new BusinessException(string.Format("操作失败:编号为{0}的商品库存调整单据当前状态不为\"待审核\"！", adjustSysNo));
                    }
                    break;
                default:
                    break;
            }

            using (TransactionScope ts = new TransactionScope())
            {
                var result = ProductDA.UpdateProductStockAdjustmentStatus(newStatus, adjustSysNo, currentUserSysNo);
                if (newStatus == ProductStockAdjustStatus.AuditPass || newStatus == ProductStockAdjustStatus.AuditFaild)
                {
                    ProductDA.UpdateProductStockAdjustmentAuditDate(adjustSysNo, currentUserSysNo);
                    if (newStatus == ProductStockAdjustStatus.AuditPass)
                    {
                        //审核库存调整单通过，调整商品相关库存(可用库存):
                        foreach (var item in currentAdjustInfo.AdjustItemList)
                        {
                            ProductPurchaseDA.UpdateStockInfoForAdjust(currentAdjustInfo.StockSysNo.Value, item.ProductSysNo.Value, item.AdjustQty.Value);
                        }
                    }
                }
                ts.Complete();
                return result;
            }
        }

        #endregion

        #region 采购单相关Service

        public static PurchaseOrderInfo LoadPurchaseOrderInfo(int poSysNo)
        {
            //1.加载采购单基本信息
            PurchaseOrderInfo poInfo = ProductPurchaseDA.LoadPOMaster(poSysNo);
            if (poInfo == null)
            {
                return poInfo;
            }
            PurchaseOrderETATimeInfo getCheckETMTimeInfo = ProductPurchaseDA.LoadPOETATimeInfo(poInfo.SysNo.Value);
            if (null != getCheckETMTimeInfo)
            {
                poInfo.PurchaseOrderBasicInfo.ETATimeInfo = getCheckETMTimeInfo;
            }
            //2.加载采购单商品列表：
            poInfo.POItems = ProductPurchaseDA.LoadPOItems(poInfo.SysNo.Value);
            foreach (var item in poInfo.POItems)
            {

                ////获取本地货币:
                if (poInfo.PurchaseOrderBasicInfo.CurrencyCode.HasValue)
                {
                    item.CurrencyCode = poInfo.PurchaseOrderBasicInfo.CurrencyCode.Value;
                    CurrencyInfo localCurrency = CommonService.GetCurrencyBySysNo(item.CurrencyCode.Value);
                    item.CurrencySymbol = localCurrency == null ? String.Empty : localCurrency.CurrencySymbol;
                }
            }
            //3.加载采购单供应商信息
            poInfo.VendorInfo = StoreService.LoadVendorInfo(poInfo.SellerSysNo.Value);

            //4.获取polog的入库总金额
            PurchaseOrderLogInfo poLogInfo = ProductPurchaseDA.LoadPOLogInfo(poInfo.SysNo.Value);
            if (null != poLogInfo)
            {
                poInfo.PurchaseOrderBasicInfo.TotalActualPrice = poLogInfo.SumTotalAmt;
            }
            if (poInfo.PurchaseOrderBasicInfo.TotalActualPrice == 0)
            {
                foreach (PurchaseOrderItemInfo pitem in poInfo.POItems)
                {
                    poInfo.PurchaseOrderBasicInfo.TotalActualPrice += pitem.OrderPrice.Value * pitem.Quantity;
                }
            }

            //5.加载采购单收货信息:
            poInfo.ReceivedInfoList = new List<PurchaseOrderReceivedInfo>();
            poInfo.ReceivedInfoList = ProductPurchaseDA.LoadPurchaseOrderReceivedInfo(poInfo.SysNo.Value);
            foreach (PurchaseOrderReceivedInfo revInfo in poInfo.ReceivedInfoList)
            {
                foreach (PurchaseOrderItemInfo item in poInfo.POItems)
                {
                    if (revInfo.ProductSysNo == item.ProductSysNo)
                    {
                        revInfo.PurchaseQty = (item.PurchaseQty.HasValue ? item.PurchaseQty.Value : 0);
                        revInfo.WaitInQty = revInfo.PurchaseQty - revInfo.ReceivedQuantity;
                    }
                }
            }
            //返回PO实体:
            return poInfo;
        }


        public static PurchaseOrderInfo LoadPurchaseOrderInfo(int poSysNo, int sellerSysNo)
        {
            //1.加载采购单基本信息
            PurchaseOrderInfo poInfo = ProductPurchaseDA.LoadPOMaster(poSysNo, sellerSysNo);
            if (poInfo == null)
            {
                return poInfo;
            }
            PurchaseOrderETATimeInfo getCheckETMTimeInfo = ProductPurchaseDA.LoadPOETATimeInfo(poInfo.SysNo.Value);
            if (null != getCheckETMTimeInfo)
            {
                poInfo.PurchaseOrderBasicInfo.ETATimeInfo = getCheckETMTimeInfo;
            }
            //2.加载采购单商品列表：
            poInfo.POItems = ProductPurchaseDA.LoadPOItems(poInfo.SysNo.Value);
            foreach (var item in poInfo.POItems)
            {

                ////获取本地货币:
                if (poInfo.PurchaseOrderBasicInfo.CurrencyCode.HasValue)
                {
                    item.CurrencyCode = poInfo.PurchaseOrderBasicInfo.CurrencyCode.Value;
                    CurrencyInfo localCurrency = CommonService.GetCurrencyBySysNo(item.CurrencyCode.Value);
                    item.CurrencySymbol = localCurrency == null ? String.Empty : localCurrency.CurrencySymbol;
                }
            }
            //3.加载采购单供应商信息
            poInfo.VendorInfo = StoreService.LoadVendorInfo(sellerSysNo);

            //4.获取polog的入库总金额
            PurchaseOrderLogInfo poLogInfo = ProductPurchaseDA.LoadPOLogInfo(poInfo.SysNo.Value);
            if (null != poLogInfo)
            {
                poInfo.PurchaseOrderBasicInfo.TotalActualPrice = poLogInfo.SumTotalAmt;
            }
            if (poInfo.PurchaseOrderBasicInfo.TotalActualPrice == 0)
            {
                foreach (PurchaseOrderItemInfo pitem in poInfo.POItems)
                {
                    poInfo.PurchaseOrderBasicInfo.TotalActualPrice += pitem.OrderPrice.Value * pitem.Quantity;
                }
            }

            //5.加载采购单收货信息:
            poInfo.ReceivedInfoList = new List<PurchaseOrderReceivedInfo>();
            poInfo.ReceivedInfoList = ProductPurchaseDA.LoadPurchaseOrderReceivedInfo(poInfo.SysNo.Value);
            foreach (PurchaseOrderReceivedInfo revInfo in poInfo.ReceivedInfoList)
            {
                foreach (PurchaseOrderItemInfo item in poInfo.POItems)
                {
                    if (revInfo.ProductSysNo == item.ProductSysNo)
                    {
                        revInfo.PurchaseQty = (item.PurchaseQty.HasValue ? item.PurchaseQty.Value : 0);
                        revInfo.WaitInQty = revInfo.PurchaseQty - revInfo.ReceivedQuantity;
                    }
                }
            }
            //返回PO实体:
            return poInfo;
        }

        public static PurchaseOrderItemInfo AddNewPurchaseOrderItem(PurchaseOrderItemProductInfo productInfo, int sellerSysNo)
        {
            if (productInfo == null)
            {
                throw new BusinessException(L("采购商品不能为空"));
            }
            if (productInfo.PrePurchaseQty <= 0)
            {
                throw new BusinessException(L("采购数量必须大于0"));
            }
            if (productInfo.PurchasePrice < 0m)
            {
                throw new BusinessException(L("采购价不能小于0"));
            }
            PurchaseOrderItemInfo item = ProductPurchaseDA.AddPurchaseOrderItemByProductSysNo(productInfo.SysNo, sellerSysNo);
            if (item == null)
            {
                throw new BusinessException(L("采购商品不存在"));
            }
            //if (item.ProductTradeType != TradeType.FTA)
            //{
            //    throw new BusinessException(L("商品【{0}】不是自贸商品，只能采购交易类型为自贸的商品", item.BriefName));
            //}

            item.OrderPrice = productInfo.PurchasePrice;
            item.PrePurchaseQty = productInfo.PrePurchaseQty;
            //当前成本:
            item.CurrentUnitCost = item.UnitCost;
            item.UnitCostWithoutTax = item.UnitCostWithoutTax;
            item.LineReturnedPointCost = item.UnitCost * productInfo.PrePurchaseQty;
            item.Quantity = 0;
            item.PurchaseQty = 0;
            //调用IM接口,获取Item价格信息:
            item.LastOrderPrice = ProductPurchaseDA.GetLastPriceBySysNo(item.ProductSysNo.Value);

            Entity.Product.ProductInventoryInfo productInventoryInfo = ProductPurchaseDA.GetProductInventoryByProductSysNO(item.ProductSysNo.Value);
            if (productInventoryInfo != null)
            {
                item.AvailableQty = productInventoryInfo.AvailableQty;
                item.UnActivatyCount = productInventoryInfo.UnActivatyCount;
            }
            item.ApportionAddOn = 0;
            ////获取本地货币:
            item.CurrencyCode = 1;
            CurrencyInfo localCurrency = CommonService.GetCurrencyBySysNo(item.CurrencyCode.Value);
            item.CurrencySymbol = localCurrency == null ? String.Empty : localCurrency.CurrencySymbol;

            PurchaseOrderItemInfo getExendPOitem = ProductPurchaseDA.GetExtendPurchaseOrderItemInfo(productInfo.SysNo);
            if (getExendPOitem != null)
            {
                item.LastAdjustPriceDate = getExendPOitem.LastAdjustPriceDate;
                item.LastInTime = getExendPOitem.LastInTime;
                item.UnActivatyCount = getExendPOitem.UnActivatyCount;
            }
            if (ProductPurchaseDA.IsVirtualStockPurchaseOrderProduct(item.ProductSysNo.Value))
            {
                item.IsVirtualStockProduct = true;
            }
            return item;
        }

        private static void PreCheckCreatePO(PurchaseOrderInfo poInfo)
        {
            if (poInfo == null)
            {
                throw new BusinessException(L("采购单不能为空！"));
            }
            if (poInfo.VendorInfo == null || string.IsNullOrWhiteSpace(poInfo.VendorInfo.VendorID))
            {
                throw new BusinessException(L("采购单商家信息不能为空！"));
            }
            if (poInfo.POItems == null || poInfo.POItems.Count <= 0)
            {
                throw new BusinessException(L("请添加需要采购的商品！"));
            }
            if (poInfo.PurchaseOrderBasicInfo.ETATimeInfo == null || !poInfo.PurchaseOrderBasicInfo.ETATimeInfo.ETATime.HasValue
                && poInfo.PurchaseOrderBasicInfo.ETATimeInfo.ETATime.Value.CompareTo(DateTime.Today) < 0)
            {
                throw new BusinessException(L("预计到货时间不能小于当前时间 !"));
            }

            var repeatItem = from i in poInfo.POItems
                             group i by new
                             {
                                 i.ProductSysNo,
                                 i.ProductID
                             } into g
                             where g.Count() > 1
                             select g;
            if (repeatItem.Count() > 0)
            {
                string result = string.Empty;
                foreach (var item in repeatItem)
                {
                    result += item.Key.ProductID + ",";
                }
                result = result.TrimEnd(',');
                throw new BusinessException(L("采购单中有重复的商品,商品的ID是：{0} !", result));
            }

            if (!poInfo.PurchaseOrderBasicInfo.StockInfo.SysNo.HasValue)
            {
                throw new BusinessException(L("入库仓库不能为空!"));
            }

            if (!poInfo.PurchaseOrderBasicInfo.PurchaseOrderType.HasValue)
            {
                throw new BusinessException(L("必须选择采购单的类型！"));
            }

            if (poInfo.PurchaseOrderBasicInfo.MemoInfo != null &&
                !string.IsNullOrWhiteSpace(poInfo.PurchaseOrderBasicInfo.MemoInfo.Memo))
            {
                if (poInfo.PurchaseOrderBasicInfo.MemoInfo.Memo.Length > 200)
                {
                    throw new BusinessException(L("备忘录的输入数据不能超过200个字符！"));
                }
            }
            if (poInfo.PurchaseOrderBasicInfo.MemoInfo != null &&
                !string.IsNullOrWhiteSpace(poInfo.PurchaseOrderBasicInfo.MemoInfo.InStockMemo))
            {
                if (poInfo.PurchaseOrderBasicInfo.MemoInfo.InStockMemo.Length > 200)
                {
                    throw new BusinessException(L("入库备注输入数据不能超过200个字符！"));
                }
            }
            poInfo.POItems.ForEach(x =>
            {
                if (poInfo.PurchaseOrderBasicInfo.PurchaseOrderType == PurchaseOrderType.Normal && x.PurchaseQty <= 0)
                {
                    throw new BusinessException(L("正常采购单的采购数量要大于0!"));
                }
                if (poInfo.PurchaseOrderBasicInfo.PurchaseOrderType == PurchaseOrderType.Negative && x.PurchaseQty >= 0)
                {
                    throw new BusinessException(L("负采购单的采购数量要小于0!"));
                }
            });

            // 如果供应商是Invalid状态，不允许新建PO单
            VendorBasicInfo getVendorInfo = StoreService.LoadVendorInfo(int.Parse(poInfo.VendorInfo.VendorID));
            if (null != getVendorInfo)
            {
                if (getVendorInfo.HoldMark.HasValue && getVendorInfo.HoldMark.Value)
                {
                    throw new BusinessException(L("商家已经被锁定,不允许创建采购单!"));
                }
                // 如果供应商是Invalid状态，不允许新建PO单
                if (getVendorInfo.VendorStatus != VendorStatus.Available)
                {
                    throw new BusinessException(L("商家是不可用状态,不允许创建采购单!"));
                }
                //采购单代销属性从供应商上取得
                poInfo.PurchaseOrderBasicInfo.ConsignFlag = (PurchaseOrderConsignFlag)((int)getVendorInfo.ConsignFlag.Value);
                //if ((int)getVendorInfo.ConsignFlag != (int)poInfo.PurchaseOrderBasicInfo.ConsignFlag)
                //{
                //    throw new BusinessException(L("非代销属性的商家不允许创建采购单!"));
                //}
            }
        }

        public static PurchaseOrderInfo CreatePO(PurchaseOrderInfo poInfo)
        {
            //获取ExchangeRate:
            poInfo.PurchaseOrderBasicInfo.CurrencyCode = 1;
            CurrencyInfo localCurrency = CommonService.GetCurrencyBySysNo(poInfo.PurchaseOrderBasicInfo.CurrencyCode.Value);
            poInfo.PurchaseOrderBasicInfo.ExchangeRate = localCurrency.ExchangeRate;
            poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus = PurchaseOrderStatus.Created;
            poInfo.PurchaseOrderBasicInfo.PurchaseOrderType = PurchaseOrderType.Normal;
            ////SellerPortal创建的采购单的账期属性为代销
            //采购单代销属性从供应商上取得
            //poInfo.PurchaseOrderBasicInfo.ConsignFlag = PurchaseOrderConsignFlag.Consign;
            poInfo.PurchaseOrderBasicInfo.TaxRateType = PurchaseOrderTaxRate.Percent000;
            poInfo.PurchaseOrderBasicInfo.TaxRate = ((decimal)poInfo.PurchaseOrderBasicInfo.TaxRateType) / 100;
            PreCheckCreatePO(poInfo);

            List<PurchaseOrderItemInfo> poItems = new List<PurchaseOrderItemInfo>();
            foreach (var item in poInfo.POItems)
            {
                PurchaseOrderItemInfo poItem = AddNewPurchaseOrderItem(new PurchaseOrderItemProductInfo()
                {
                    SysNo = item.ProductSysNo.Value,
                    ProductID = item.ProductID,
                    PrePurchaseQty = item.PrePurchaseQty.Value,
                    PurchasePrice = item.PurchasePrice.Value
                }, int.Parse(poInfo.VendorInfo.VendorID));
                poItems.Add(poItem);
            }
            poInfo.POItems = poItems;
            poInfo.PurchaseOrderBasicInfo.TotalAmt = poInfo.POItems.Sum(item => item.OrderPrice.Value * item.PrePurchaseQty.Value);

            using (ITransaction trans = ECommerce.Utility.TransactionManager.Create())
            {
                //设置初始化值:
                poInfo.SysNo = ProductPurchaseDA.CreatePOSequenceSysNo();
                poInfo.PurchaseOrderBasicInfo.PurchaseOrderID = poInfo.SysNo.Value.ToString();
                poInfo.PurchaseOrderBasicInfo.CreateDate = System.DateTime.Now;
                poInfo.PurchaseOrderBasicInfo.IsApportion = 0;

                //创建操作:
                ProductPurchaseDA.CreatePO(poInfo);
                //ETA时间申请
                poInfo.PurchaseOrderBasicInfo.ETATimeInfo.POSysNo = poInfo.SysNo;
                poInfo.PurchaseOrderBasicInfo.ETATimeInfo.Status = 1;
                poInfo.PurchaseOrderBasicInfo.ETATimeInfo.InUser = poInfo.InUserName;
                ProductPurchaseDA.CreatePOETAInfo(poInfo.PurchaseOrderBasicInfo.ETATimeInfo);
                foreach (PurchaseOrderItemInfo item in poInfo.POItems)
                {
                    item.Quantity = 0;
                    //将采购数量初始化为PrePurchaseQty
                    item.PurchaseQty = item.PrePurchaseQty;
                    item.POSysNo = poInfo.SysNo;
                    //创建PO Item:
                    ProductPurchaseDA.CreatePOItem(item);
                }
                trans.Complete();
            }

            return poInfo;
        }

        /// <summary>
        /// 提交审核
        /// </summary>
        /// <returns></returns>
        public static PurchaseOrderInfo Submit(PurchaseOrderInfo poInfo)
        {
            if (poInfo == null || !poInfo.SysNo.HasValue)
            {
                throw new BusinessException(L("采购单不能为空！"));
            }

            PurchaseOrderInfo localEntity = ProductPurchaseDA.LoadPOMaster(poInfo.SysNo.Value, poInfo.SellerSysNo.Value);
            if (localEntity == null)
            {
                throw new BusinessException(L("采购单不存在！"));
            }
            if (!(localEntity.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.Created
                || localEntity.PurchaseOrderBasicInfo.PurchaseOrderStatus == PurchaseOrderStatus.Returned))
            {
                throw new BusinessException(L("该PO单已经提交审核，不能重复提交!"));
            }
            if (string.IsNullOrEmpty(poInfo.PurchaseOrderBasicInfo.MemoInfo.PMRequestMemo))
            {
                throw new BusinessException(L("申请理由不能为空！"));
            }

            localEntity.PurchaseOrderBasicInfo.MemoInfo.PMRequestMemo = poInfo.PurchaseOrderBasicInfo.MemoInfo.PMRequestMemo + "[" + poInfo.EditUserName + ":" + DateTime.Now.ToString() + "]";
            localEntity.PurchaseOrderBasicInfo.MemoInfo.RefuseMemo = string.Empty;
            localEntity.PurchaseOrderBasicInfo.PurchaseOrderStatus = PurchaseOrderStatus.WaitingAudit;
            ProductPurchaseDA.UpdatePOStatus(localEntity);

            return localEntity;
        }

        /// <summary>
        /// 作废
        /// </summary>
        /// <param name="poInfo"></param>
        public static PurchaseOrderInfo AbandonPO(PurchaseOrderInfo poInfo)
        {
            if (poInfo == null || !poInfo.SysNo.HasValue)
            {
                throw new BusinessException(L("采购单不能为空！"));
            }
            PurchaseOrderInfo localEntity = ProductPurchaseDA.LoadPOMaster(poInfo.SysNo.Value, poInfo.SellerSysNo.Value);
            if (localEntity == null)
            {
                throw new BusinessException(L("采购单不存在！"));
            }
            if (localEntity.PurchaseOrderBasicInfo.PurchaseOrderStatus != PurchaseOrderStatus.Created
                && localEntity.PurchaseOrderBasicInfo.PurchaseOrderStatus != PurchaseOrderStatus.Returned)
            {
                throw new BusinessException(L("采购单不为初始状态，不能作废！"));
            }

            using (ITransaction trans = ECommerce.Utility.TransactionManager.Create())
            {
                localEntity.PurchaseOrderBasicInfo.PurchaseOrderStatus = PurchaseOrderStatus.Abandoned;
                ProductPurchaseDA.UpdatePOStatus(localEntity);

                PurchaseOrderETATimeInfo poetaEntity = new PurchaseOrderETATimeInfo();
                poetaEntity.Status = -1;
                poetaEntity.POSysNo = poInfo.SysNo;
                poetaEntity.EditUser = poInfo.EditUserName;
                ProductPurchaseDA.UpdatePOETAInfo(poetaEntity);

                trans.Complete();
            }
            return localEntity;
        }

        /// <summary>
        /// 审核通过
        /// </summary>
        /// <param name="poInfo"></param>
        public static PurchaseOrderInfo ValidPO(PurchaseOrderInfo poInfo)
        {
            if (poInfo == null || !poInfo.SysNo.HasValue)
            {
                throw new BusinessException(L("采购单不能为空！"));
            }
            PurchaseOrderInfo localEntity = ProductPurchaseDA.LoadPOMaster(poInfo.SysNo.Value, poInfo.SellerSysNo.Value);
            if (localEntity == null)
            {
                throw new BusinessException(L("采购单不存在！"));
            }
            if (localEntity.PurchaseOrderBasicInfo.PurchaseOrderStatus != PurchaseOrderStatus.WaitingAudit)
            {
                throw new BusinessException(L("采购单不为待审核状态，不能审核通过！"));
            }

            using (ITransaction trans = ECommerce.Utility.TransactionManager.Create())
            {
                localEntity.PurchaseOrderBasicInfo.PurchaseOrderStatus = PurchaseOrderStatus.WaitingReport;
                localEntity.PurchaseOrderBasicInfo.AuditDate = DateTime.Now;
                localEntity.PurchaseOrderBasicInfo.AuditUserSysNo = poInfo.EditUserSysNo;
                localEntity.PurchaseOrderBasicInfo.AuditUserName = poInfo.EditUserName;
                ProductPurchaseDA.UpdatePOStatus(localEntity);

                PurchaseOrderETATimeInfo poetaEntity = new PurchaseOrderETATimeInfo();
                poetaEntity.Status = 2;
                poetaEntity.POSysNo = poInfo.SysNo;
                poetaEntity.EditUser = poInfo.EditUserName;
                ProductPurchaseDA.UpdatePOETAInfo(poetaEntity);

                trans.Complete();
            }
            return localEntity;
        }

        /// <summary>
        /// 审核拒绝
        /// </summary>
        /// <param name="poInfo"></param>
        public static PurchaseOrderInfo RejectPO(PurchaseOrderInfo poInfo)
        {
            if (poInfo == null || !poInfo.SysNo.HasValue)
            {
                throw new BusinessException(L("采购单不能为空！"));
            }
            PurchaseOrderInfo localEntity = ProductPurchaseDA.LoadPOMaster(poInfo.SysNo.Value, poInfo.SellerSysNo.Value);
            if (localEntity == null)
            {
                throw new BusinessException(L("采购单不存在！"));
            }
            if (string.IsNullOrEmpty(poInfo.PurchaseOrderBasicInfo.MemoInfo.RefuseMemo))
            {
                throw new BusinessException(L("拒绝理由不能为空！"));
            }

            if (localEntity.PurchaseOrderBasicInfo.PurchaseOrderStatus != PurchaseOrderStatus.WaitingAudit)
            {
                throw new BusinessException(L("采购单不为待审核状态，不能审核拒绝！"));
            }
            localEntity.PurchaseOrderBasicInfo.MemoInfo.RefuseMemo = poInfo.PurchaseOrderBasicInfo.MemoInfo.RefuseMemo + "[" + poInfo.EditUserName + ":" + DateTime.Now.ToString() + "]";
            localEntity.PurchaseOrderBasicInfo.PurchaseOrderTPStatus = null;
            localEntity.PurchaseOrderBasicInfo.PurchaseOrderStatus = PurchaseOrderStatus.Returned;
            localEntity.PurchaseOrderBasicInfo.AuditDate = null;
            localEntity.PurchaseOrderBasicInfo.AuditUserSysNo = null;


            using (ITransaction trans = ECommerce.Utility.TransactionManager.Create())
            {
                ProductPurchaseDA.UpdatePOStatus(localEntity);

                PurchaseOrderETATimeInfo poetaEntity = new PurchaseOrderETATimeInfo();
                poetaEntity.Status = -1;
                poetaEntity.POSysNo = poInfo.SysNo;
                poetaEntity.EditUser = poInfo.EditUserName;
                ProductPurchaseDA.UpdatePOETAInfo(poetaEntity);

                trans.Complete();
            }
            return localEntity;
        }

        /// <summary>
        /// 采购单确认入库(接口调用)
        /// </summary>
        /// <param name="poSysNo"></param>
        public static PurchaseOrderInfo WaitingInstockPO(PurchaseOrderInfo poInfo, List<KeyValuePair<string, int>> productList)
        {
            if (null == poInfo || !poInfo.SysNo.HasValue)
            {
                throw new BusinessException("找不到相关的采购单信息!");

            }
            if (poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus != PurchaseOrderStatus.Reporting)
            {
                throw new BusinessException("该采购单状态不是'申报中'状态，不能进行入库确认操作!");
            }
            if (productList == null || productList.Count <= 0)
            {
                throw new BusinessException("该采购单商品明细传入为空!");

            }
            //poInfo.PurchaseOrderBasicInfo.ETATimeInfo.ETATime = localEntity.PurchaseOrderBasicInfo.ETATimeInfo.ETATime;
            //poInfo.PurchaseOrderBasicInfo.ETATimeInfo.HalfDay = localEntity.PurchaseOrderBasicInfo.ETATimeInfo.HalfDay;
            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = System.Transactions.TransactionManager.DefaultTimeout;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {

                #region 更新PO单Item的PurchaseQty:
                if (null != productList && productList.Count > 0)
                {
                    foreach (var item in productList)
                    {
                        var poItem = poInfo.POItems.SingleOrDefault(x => x.ProductID.Trim().ToLower() == item.Key.Trim().ToLower());
                        if (null != poItem)
                        {


                            if (poItem.PrePurchaseQty < item.Value)
                            {
                                throw new BusinessException(string.Format("采购单编号：{0}，商品ID为{1}的实际采购数量(ItemNum)大于计划采购数量!", poInfo.SysNo.Value, item.Key));
                            }

                            poItem.PurchaseQty = item.Value;
                            ProductPurchaseDA.UpdatePOItemPurchaseQty(poItem.ItemSysNo.Value, item.Value);
                        }
                        else
                        {
                            throw new BusinessException(string.Format("采购单编号：{0}，找不到商品ID为{1}的采购单商品信息!", poInfo.SysNo.Value, item.Key));
                        }
                    }
                }
                #endregion

                #region 更新采购单状态:
                poInfo.PurchaseOrderBasicInfo.PurchaseOrderStatus = PurchaseOrderStatus.WaitingInStock;
                poInfo.PurchaseOrderBasicInfo.MemoInfo.RefuseMemo = string.Empty;

                ProductPurchaseDA.WaitingInStockPO(poInfo);
                #endregion

                #region 更新POItem信息:
                List<KeyValuePair<int, int>> kv = new List<KeyValuePair<int, int>>();
                foreach (PurchaseOrderItemInfo item in poInfo.POItems)
                {
                    kv.Add(new KeyValuePair<int, int>(item.ProductSysNo.Value, item.PurchaseQty.Value));

                    //总仓有效库存 上月销售总量
                    PurchaseOrderItemInfo tempPoItem = ProductPurchaseDA.LoadExtendPOItem(item.ProductSysNo.Value);
                    item.M1 = tempPoItem.M1;
                    item.AvailableQty = tempPoItem.AvailableQty;
                    item.UnitCostWithoutTax = item.UnitCostWithoutTax ?? 0;
                    item.CurrentUnitCost = tempPoItem.CurrentUnitCost;
                    item.CurrentPrice = tempPoItem.CurrentPrice;
                    item.LastInTime = tempPoItem.LastInTime;
                    item.LastAdjustPriceDate = tempPoItem.LastAdjustPriceDate;
                    item.LastOrderPrice = tempPoItem.LastOrderPrice;
                    ProductPurchaseDA.UpdatePOItem(item);
                }
                #endregion

                #region 设置采购在途数量,（代销PO该业务逻辑不变）

                ProductPurchaseInstockAdjustInventoryInfo inventoryAdjustInfo = new ProductPurchaseInstockAdjustInventoryInfo()
                {
                    ReferenceSysNo = poInfo.SysNo.Value,
                    SourceActionName = "Audit",
                    AdjustItemList = new List<Entity.Inventory.InventoryAdjustItemInfo>()
                };
                kv.ForEach(x =>
                {
                    inventoryAdjustInfo.AdjustItemList.Add(new InventoryAdjustItemInfo()
                    {
                        ProductSysNo = x.Key,
                        StockSysNo = poInfo.PurchaseOrderBasicInfo.StockInfo.SysNo.Value,
                        AdjustQuantity = x.Value
                    });
                });

                foreach (InventoryAdjustItemInfo adjustItem in inventoryAdjustInfo.AdjustItemList)
                {
                    //this.CurrentAdjustItemInfo = adjustItem;
                    //this.AdjustQuantity = adjustItem.AdjustQuantity;
                    //ProcessAdjustItemInfo();
                    CostLockType costLockAction = CostLockType.NoUse;
                    ProductQueryInfo productInfo = ProductService.GetProductBySysNo(adjustItem.ProductSysNo);
                    if (productInfo == null || productInfo.SysNo <= 0)
                    {
                        throw new BusinessException(string.Format("欲调库存的商品不存在，商品编号：{0}", adjustItem.ProductSysNo));
                    }
                    InventoryDA.InitProductInventoryInfo(adjustItem.ProductSysNo, adjustItem.StockSysNo);
                    var inventoryType = InventoryDA.GetProductInventroyType(adjustItem.ProductSysNo);
                    ECommerce.Entity.Inventory.ProductInventoryInfo stockInventoryCurrentInfo = InventoryDA.GetProductInventoryInfoByStock(adjustItem.ProductSysNo, adjustItem.StockSysNo);
                    ECommerce.Entity.Inventory.ProductInventoryInfo totalInventoryCurrentInfo = InventoryDA.GetProductTotalInventoryInfo(adjustItem.ProductSysNo);

                    ECommerce.Entity.Inventory.ProductInventoryInfo stockInventoryAdjustInfo = new Entity.Inventory.ProductInventoryInfo()
                    {
                        ProductSysNo = adjustItem.ProductSysNo,
                        StockSysNo = adjustItem.StockSysNo
                    };

                    ECommerce.Entity.Inventory.ProductInventoryInfo totalInventoryAdjustInfo = new ECommerce.Entity.Inventory.ProductInventoryInfo()
                    {
                        ProductSysNo = adjustItem.ProductSysNo
                    };
                    //获取负po的成本库存
                    List<ProductCostIn> productCostInList = InventoryDA.GetProductCostIn(adjustItem.ProductSysNo, Convert.ToInt32(inventoryAdjustInfo.ReferenceSysNo), adjustItem.StockSysNo);


                    List<ProductCostIn> adjustProductCostInList = new List<ProductCostIn>();
                    int CanUseQuantity = 0;
                    //区分正负PO单
                    if (adjustItem.AdjustQuantity < 0)
                    {
                        //负PO单审核, AdjustQty<0, 减少可用库存, 增加已分配库存
                        stockInventoryAdjustInfo.AvailableQty = adjustItem.AdjustQuantity;
                        totalInventoryAdjustInfo.AvailableQty = adjustItem.AdjustQuantity;

                        if (adjustItem.AdjustQuantity < 0)
                        {
                            //AllocatedQty(-,->0),小于0则自动调为0。       
                            if (stockInventoryCurrentInfo.AllocatedQty + adjustItem.AdjustQuantity < 0)
                            {
                                stockInventoryAdjustInfo.AllocatedQty = -stockInventoryCurrentInfo.AllocatedQty;
                            }
                            else
                            {
                                stockInventoryAdjustInfo.AllocatedQty = adjustItem.AdjustQuantity;
                            }

                            if (totalInventoryCurrentInfo.AllocatedQty + adjustItem.AdjustQuantity < 0)
                            {
                                totalInventoryAdjustInfo.AllocatedQty = -totalInventoryCurrentInfo.AllocatedQty;
                            }
                            else
                            {
                                totalInventoryAdjustInfo.AllocatedQty = adjustItem.AdjustQuantity;
                            }

                        }
                        else
                        {
                            stockInventoryAdjustInfo.AllocatedQty = adjustItem.AdjustQuantity;
                            totalInventoryAdjustInfo.AllocatedQty = adjustItem.AdjustQuantity;
                        }

                        //标识要锁定成本库存
                        costLockAction = CostLockType.Lock;
                        int temp = Math.Abs(adjustItem.AdjustQuantity);
                        //锁定库存
                        foreach (var item in productCostInList)
                        {
                            CanUseQuantity = item.LeftQuantity - item.LockQuantity;
                            //可用数量大于要锁定数量，直接累加加到锁定数量
                            if (CanUseQuantity >= temp)
                            {
                                item.LockQuantity += temp;
                                adjustProductCostInList.Add(item);
                                break;
                            }
                            else if (CanUseQuantity > 0) //可用数量不足且大于0，
                            {
                                //调整数量减少相应值，进行一次锁定分配
                                temp = temp - CanUseQuantity;
                                //将可用加到锁定数量上
                                item.LockQuantity += CanUseQuantity;
                                adjustProductCostInList.Add(item);
                            }
                        }
                    }
                    else
                    {
                        //正PO单审核, AdjustQty>0, 增加采购库存
                        stockInventoryAdjustInfo.PurchaseQty = adjustItem.AdjustQuantity;
                        totalInventoryAdjustInfo.PurchaseQty = adjustItem.AdjustQuantity;
                    }

                    //预检调整后的商品库存是否合法  
                    Entity.Inventory.ProductInventoryInfo stockInventoryAdjustAfterAdjust = InventoryService.PreCalculateInventoryAfterAdjust(stockInventoryCurrentInfo, stockInventoryAdjustInfo);
                    Entity.Inventory.ProductInventoryInfo totalInventoryAdjustAfterAdjust = InventoryService.PreCalculateInventoryAfterAdjust(totalInventoryCurrentInfo, totalInventoryAdjustInfo);

                    bool isNeedCompareAvailableQtyAndAccountQty = true;
                    InventoryService.PreCheckGeneralRules(stockInventoryAdjustAfterAdjust, ref isNeedCompareAvailableQtyAndAccountQty);
                    InventoryService.PreCheckGeneralRules(totalInventoryAdjustAfterAdjust, ref isNeedCompareAvailableQtyAndAccountQty);

                    //调整商品库存:

                    InventoryDA.AdjustProductStockInventoryInfo(stockInventoryAdjustInfo);
                    InventoryDA.AdjustProductTotalInventoryInfo(totalInventoryAdjustInfo);
                    //如果需要调整锁定库存
                    if (costLockAction != CostLockType.NoUse && adjustProductCostInList != null)
                    {
                        InventoryDA.LockProductCostInList(adjustProductCostInList);
                    }
                }
                #endregion

                #region 如果是负采购单,调整批次库存:
                if (poInfo.PurchaseOrderBasicInfo.PurchaseOrderType == PurchaseOrderType.Negative)
                {
                    SetInventoryInfo(poInfo, "");
                }

                #endregion

                scope.Complete();
            }
            return poInfo;
        }

        /// <summary>
        /// 调整批次库存
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="sysNo"></param>
        private static void SetInventoryInfo(PurchaseOrderInfo entity, string sysNo)
        {
            string xml = @"<Message>
                              <Header>
                                <NameSpace>http://soa.ECommerce.com/CustomerProfile</NameSpace>
                                <Action>#Audit#</Action>
                                <Version>V10</Version>
                                <Type>NPO</Type>
                                <CompanyCode>#CompanyCode#</CompanyCode>      
                                <Tag>POInstock</Tag>
                                <Language>zh-CN</Language>
                                <From>IPP</From>
                                <GlobalBusinessType>Listing</GlobalBusinessType>
                                <StoreCompanyCode>#StoreCompanyCode#</StoreCompanyCode>   
                                <TransactionCode>05-001-0-001</TransactionCode>
                              </Header>
                                <Body>
                                   <Number>#Number#</Number> 
                                   <InUser>#InUser#</InUser>  
                                   ######
                                </Body>
                            </Message>";
            string batch = @"<ItemBatchInfo>
                                       <BatchNumber>#BatchNumber#</BatchNumber>  
                                       <Status></Status>
                                       <ProductNumber>#ProductNumber#</ProductNumber> 
                                       <ExpDate></ExpDate>
                                       <MfgDate></MfgDate>
                                       <LotNo></LotNo>
                                       <Stocks>
                                          <Stock>
                                          <Quantity>#Quantity#</Quantity>            
                                          <AllocatedQty>#Quantity#</AllocatedQty>  
                                          <WarehouseNumber>#WarehouseNumber#</WarehouseNumber> 
                                          </Stock>
                                       </Stocks>
                                   </ItemBatchInfo>";
            string newxml = xml.Replace("#InUser#", entity.PurchaseOrderBasicInfo.AuditUserSysNo.ToString())
                             .Replace("#Number#", entity.SysNo.ToString())
                             .Replace("#StoreCompanyCode#", entity.CompanyCode)
                             .Replace("#CompanyCode#", entity.CompanyCode);
            if (sysNo == "")
            {
                newxml = newxml.Replace("#Audit#", "Audit");
            }
            else
            {
                newxml = newxml.Replace("#Audit#", "CancelAudit");
            }
            StringBuilder strb = new StringBuilder();

            foreach (var item in entity.POItems)
            {

                if (!ProductPurchaseDA.IsBatchProduct(item))
                {
                    continue;
                }

                string[] strs = item.BatchInfo.Split(new char[] { ';' });
                foreach (string str in strs)
                {
                    string[] strChild = str.Split(new char[] { ':' });
                    if (strChild.Length == 3)
                    {
                        strb.Append(batch.Replace("#WarehouseNumber#", strChild[1])
                             .Replace("#Quantity#", sysNo + strChild[2])
                             .Replace("#ProductNumber#", item.ProductSysNo.ToString())
                             .Replace("#BatchNumber#", strChild[0])
                             );
                    }
                }
            }

            InventoryDA.AdjustBatchNumberInventory(newxml.Replace("######", strb.ToString()));
        }


        public static List<PurchaseOrderSSBLogInfo> LoadPOSSBLog(int poSysNo, PurchaseOrderSSBMsgType msgType)
        {
            return ProductPurchaseDA.LoadPOSSBLog(poSysNo, msgType);
        }

        public static string GetItemAccessoriesStringByPurchaseOrder(List<int?> productSysNoList, string companyCode)
        {
            return ProductPurchaseDA.GetItemAccessoriesStringByPurchaseOrder(productSysNoList, companyCode);
        }
        #endregion

        private static string L(string key, params object[] args)
        {
            string multiLangText = ECommerce.WebFramework.LanguageHelper.GetText(key);
            return string.Format(multiLangText, args);
        }
    }
}
