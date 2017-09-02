using System;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.IM.BizProcessor.IMAOP;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using System.Collections.Generic;
using System.Transactions;

namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(ProductPriceProcessor))]
    public class ProductPriceProcessor
    {
        private readonly IProductDA _productDA = ObjectFactory<IProductDA>.Instance;

        private readonly IProductCommonInfoDA _productCommonInfoDA = ObjectFactory<IProductCommonInfoDA>.Instance;

        private readonly IProductPriceDA _productPriceDA = ObjectFactory<IProductPriceDA>.Instance;

        private readonly ProductPriceRequestProcessor _productPriceRequestBP = ObjectFactory<ProductPriceRequestProcessor>.Instance;

        [ProductInfoChange]
        public void UpdateProductPriceInfo(ProductInfo productInfo)
        {
            var product = _productDA.GetProductInfoBySysNo(productInfo.SysNo);
            if (product.ProductStatus == ProductStatus.Active)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductPrice", "CheckProductPriceActiveProductCannotSave"));
            }

            var promotionList = ExternalDomainBroker.GetProductPromotionDiscountInfoList(productInfo.SysNo);

            CheckNoPromotion(promotionList);

            product.ProductBasicInfo.Note = productInfo.ProductBasicInfo.Note;

            ProcessNoRequestPrice(productInfo.ProductPriceRequest, product.ProductPriceInfo);

            DoubleCheckProductPrice(productInfo.ProductPriceRequest);
            /*
            var auditType = ProductPriceRequestAuditType.Audit;

            var auditMessage = CheckProductPriceRequest(productInfo, product.ProductPriceInfo, product.ProductBasicInfo.ProductCategoryInfo,
                                     ref auditType);

            if (auditMessage.Length > 0)
            {
                throw new BizException(auditMessage);
            }
            */
            _productPriceDA.UpdateProductBasicPrice(productInfo.SysNo, productInfo.ProductPriceRequest);
            if (productInfo.ProductPriceRequest.VirtualPrice != product.ProductPriceInfo.VirtualPrice)
            {
                _productPriceDA.UpdateProductVirtualPrice(productInfo.SysNo, product.ProductPriceInfo.VirtualPrice, productInfo.ProductPriceRequest.VirtualPrice);
            }
            if (productInfo.ProductPriceInfo.IsSyncShopPrice != product.ProductPriceInfo.IsSyncShopPrice)
            {
                _productPriceDA.UpdateProductSyncShopPrice(productInfo.SysNo, productInfo.ProductPriceInfo.IsSyncShopPrice);
            }
            _productPriceDA.UpdateProductPrice(productInfo.SysNo, productInfo.ProductPriceRequest);
            #region Check当前商品调价后所在销售规则中差价
            ExternalDomainBroker.CheckComboPriceAndSetStatus(productInfo.SysNo);
            #endregion

            _productCommonInfoDA.UpdateProductCommonInfoNote(product, productInfo.OperateUser);
        }

        private void CheckNoPromotion(IEnumerable<ProductPromotionDiscountInfo> promotionList)
        {
            if (promotionList != null)
            {
                promotionList.Where(promotion => promotion.PromotionType == PromotionType.Countdown
                    || promotion.PromotionType == PromotionType.GroupBuying
                    || promotion.PromotionType == PromotionType.SaleGift).ForEach(promotion =>
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductPrice", "CheckNoPromotionResult1")
                        + EnumHelper.GetDescription(promotion.PromotionType)
                        + ResouceManager.GetMessageString("IM.ProductPrice", "CheckNoPromotionResult2") + "，"
                        + EnumHelper.GetDescription(promotion.PromotionType)
                        + ResouceManager.GetMessageString("IM.ProductPrice", "CheckNoPromotionResult3")
                        + promotion.ReferenceSysNo
                        + "，" + ResouceManager.GetMessageString("IM.ProductPrice", "CheckNoPromotionResult4") + "！");
                });
            }
        }

        [ProductInfoChange]
        public void AuditRequestProductPrice(ProductInfo productInfo)
        {
            var exceptionBuilder = new StringBuilder();

            var product = _productDA.GetProductInfoBySysNo(productInfo.SysNo);

            if (product.ProductPriceRequest.SysNo.HasValue)
            {
                exceptionBuilder.AppendLine(ResouceManager.GetMessageString("IM.ProductPrice", "AuditRequestProductPriceResult1"));
            }

            if (String.IsNullOrEmpty(productInfo.ProductPriceRequest.PMMemo))
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductPrice", "AuditRequestProductPriceResult2"));
            }

            if (ExternalDomainBroker.CheckMarketIsActivity(productInfo.SysNo))
            {
                exceptionBuilder.AppendLine(ResouceManager.GetMessageString("IM.ProductPrice", "AuditRequestProductPriceResult3"));
            }

            ProcessNoRequestPrice(productInfo.ProductPriceRequest, product.ProductPriceInfo);

            if (productInfo.ProductPriceRequest.CurrentPrice.HasValue
                && (product.ProductStatus == ProductStatus.Active
                || product.ProductStatus == ProductStatus.InActive_Show)
                && (productInfo.ProductPriceRequest.CurrentPrice.Value == IMConst.ProductPriceZero
                || productInfo.ProductPriceRequest.CurrentPrice.Value == IMConst.ProductDefaultPrice))
            {
                exceptionBuilder.AppendLine(ResouceManager.GetMessageString("IM.ProductPrice", "AuditRequestProductPriceResult4"));
            }

            ProductPriceRequestAuditType auditType = ProductPriceRequestAuditType.Audit;
            /*
            var auditMessage = CheckProductPriceRequest(productInfo, product.ProductPriceInfo,
                                                        product.ProductBasicInfo.ProductCategoryInfo, ref auditType);

            if (auditMessage.Length == 0)
            {
                exceptionBuilder.AppendLine(ResouceManager.GetMessageString("IM.ProductPrice", "AuditRequestProductPriceResult5"));
            }
            */
            if (exceptionBuilder.Length > 0)
            {
                throw new BizException(exceptionBuilder.ToString());
            }
            productInfo.ProductPriceRequest.AuditType = auditType;

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
            options.Timeout = TimeSpan.FromMinutes(5);
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                if (productInfo.ProductPriceRequest.VirtualPrice != product.ProductPriceInfo.VirtualPrice)
                {
                    _productPriceDA.UpdateProductVirtualPrice(productInfo.SysNo, product.ProductPriceInfo.VirtualPrice, productInfo.ProductPriceRequest.VirtualPrice);
                }
                _productPriceDA.UpdateProductBasicPrice(productInfo.SysNo, productInfo.ProductPriceRequest);
                _productPriceRequestBP.InsertProductPriceRequest(productInfo.SysNo, productInfo.ProductPriceRequest);

                //提交价格审核之后发送消息
                var getProduct = _productDA.GetProductInfoBySysNo(productInfo.SysNo);
                EventPublisher.Publish<ECCentral.Service.EventMessage.IM.ProductPriceAuditSubmitMessage>(new ECCentral.Service.EventMessage.IM.ProductPriceAuditSubmitMessage()
                {
                    SubmitUserSysNo = ServiceContext.Current.UserSysNo,
                    ProductSysNo = productInfo != null ? productInfo.SysNo : 0,
                    RequestSysNo = (getProduct != null && getProduct.ProductPriceRequest != null && getProduct.ProductPriceRequest.SysNo.HasValue) ? getProduct.ProductPriceRequest.SysNo.Value : 0
                });

                scope.Complete();
            }
        }

        [ProductInfoChange]
        public void CancelAuditProductPriceRequest(ProductInfo productInfo)
        {
            var product = _productDA.GetProductInfoBySysNo(productInfo.SysNo);

            if (product.ProductPriceRequest.SysNo.HasValue
                && product.ProductPriceRequest.RequestStatus == ProductPriceRequestStatus.Origin)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    ProductPriceRequestInfo productPriceRequestInfo = _productPriceRequestBP.GetProductPriceRequestInfoBySysNo(product.ProductPriceRequest.SysNo.Value);
                    _productPriceRequestBP.CancelAuditProductPriceRequest(productPriceRequestInfo,
                                                                               ProductPriceRequestStatus.Canceled);
                    //申请取消之后发送消息
                    EventPublisher.Publish<ECCentral.Service.EventMessage.IM.CanceledUpdateProductPriceRequestMessage>(new ECCentral.Service.EventMessage.IM.CanceledUpdateProductPriceRequestMessage()
                    {
                        CancelUserSysNo = ServiceContext.Current.UserSysNo,
                        ProductSysNo = product.SysNo,
                        RequestSysNo = product.ProductPriceRequest.SysNo.Value
                    });

                    scope.Complete();
                }
            }
            else
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductPrice", "CancelAuditProductPriceRequestResult"));
            }
        }

        private void DoubleCheckProductPrice(ProductPriceRequestInfo requestInfo)
        {
            if (requestInfo == null)
            {
                throw new ArgumentNullException("requestInfo");
            }
            if (requestInfo.BasicPrice < IMConst.ProductPriceZero)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductPrice", "ProductBasicPriceIsNull"));
            }

            if (requestInfo.CashRebate < IMConst.ProductPriceZero)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductPrice", "ProductCashRebateIsNull"));
            }

            if (requestInfo.Point < IMConst.ProductPriceZero)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductPrice", "ProducPointIsNull"));
            }

            //if (requestInfo.CurrentPrice.HasValue && (requestInfo.CurrentPrice < 0m || requestInfo.CurrentPrice > Math.Floor(requestInfo.CurrentPrice.Value)))
            //{
            //    throw new BizException(ResouceManager.GetMessageString("IM.ProductPrice", "ProducCurrentPriceIsNull"));
            //}

            if (!(requestInfo.CurrentPrice.HasValue && requestInfo.CurrentPrice.Value > IMConst.ProductPriceZero))
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductPrice", "ProducCurrentPriceLargerThranZero"));
            }
        }

        private string CheckProductPriceRequest(ProductInfo productInfo, ProductPriceInfo originalPriceInfo, CategoryInfo categoryInfo, ref ProductPriceRequestAuditType auditType)
        {
            var auditMessage = new StringBuilder();

            var requestInfo = productInfo.ProductPriceRequest;

            #region BasicPrice

            if (requestInfo.CurrentPrice < requestInfo.Point)
            {
                auditMessage.AppendLine(ResouceManager.GetMessageString("IM.ProductPrice", "CheckProductPriceRequestResult1"));
            }

            //if (requestInfo.CurrentPrice + requestInfo.CashRebate >= requestInfo.BasicPrice)
            //{
            //    throw new BizException("商品售价加返现必须小于市场价格！");
            //}

            if (requestInfo.CurrentPrice >= 2 * requestInfo.UnitCost)
            {
                auditMessage.AppendLine(ResouceManager.GetMessageString("IM.ProductPrice", "CheckProductPriceRequestResult2"));
            }

            if (requestInfo.CurrentPrice < originalPriceInfo.CurrentPrice)
            {
                var checkMarginResult = CheckMargin(requestInfo.CurrentPrice.Value, productInfo, categoryInfo, ResouceManager.GetMessageString("IM.ProductPrice", "CheckProductPriceRequestResult3"),
                                                    ref auditType);
                if (checkMarginResult.Length > 0)
                {
                    auditMessage.AppendLine(checkMarginResult);
                }
            }

            if (requestInfo.MaxCountPerDay < requestInfo.MinCountPerOrder)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductPrice", "CheckProductPriceRequestResult4"));
            }

            #endregion

            #region RankPrice

            var rankPriceList =
                productInfo.ProductPriceRequest.ProductRankPrice.Where(
                    rankPrice => rankPrice.Status == ProductRankPriceStatus.Active).ToList();

            if (rankPriceList.Any() && rankPriceList.OrderBy(rankPrice => rankPrice.RankPrice).First().RankPrice >= requestInfo.CurrentPrice)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductPrice", "AuditRequestProductPriceResult5"));
            }

            if (rankPriceList.Select(rankPrice => rankPrice.RankPrice).Distinct().Count() != rankPriceList.Count)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductPrice", "AuditRequestProductPriceResult6"));
            }

            if (rankPriceList.Any(rankPrice => !rankPrice.RankPrice.HasValue))
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductPrice", "AuditRequestProductPriceResult7"));
            }

            if (!rankPriceList
                .OrderBy(rankPrice => rankPrice.Rank)
                .Select(rankPrice => rankPrice.Rank.ToEnumDesc()).Join("")
                .Equals(rankPriceList
                        .OrderByDescending(rankPrice => rankPrice.RankPrice)
                        .Select(rankPrice => rankPrice.Rank.ToEnumDesc()).Join("")))
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductPrice", "AuditRequestProductPriceResult8"));
            }

            foreach (var rankPrice in rankPriceList)
            {
                auditMessage.Append(CheckMargin(rankPrice.RankPrice.HasValue ? rankPrice.RankPrice.Value : 0,
                                                productInfo, categoryInfo, rankPrice.Rank.ToEnumDesc(), ref auditType));
            }
       
            #endregion

            #region VolumePrice

            var volumePriceList = productInfo.ProductPriceRequest.ProductWholeSalePriceInfo;

            if (volumePriceList.Any() && volumePriceList.OrderBy(volumePrice => volumePrice.Price).First().Price >= requestInfo.CurrentPrice)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductPrice", "AuditRequestProductPriceResult9"));
            }

            if (volumePriceList.Any(volumePrice => !volumePrice.Qty.HasValue))
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductPrice", "AuditRequestProductPriceResult10"));
            }

            if (volumePriceList.Any(volumePrice => !volumePrice.Price.HasValue))
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductPrice", "AuditRequestProductPriceResult11"));
            }

            if (!volumePriceList
                .OrderBy(volumePrice => volumePrice.Level)
                .Select(volumePrice => volumePrice.Level.ToEnumDesc()).Join("")
                .Equals(volumePriceList
                        .OrderBy(volumePrice => volumePrice.Qty)
                        .Select(volumePrice => volumePrice.Level.ToEnumDesc()).Join("")))
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductPrice", "AuditRequestProductPriceResult12"));
            }

            if (!volumePriceList
                .OrderBy(volumePrice => volumePrice.Level)
                .Select(volumePrice => volumePrice.Level.ToEnumDesc()).Join("")
                .Equals(volumePriceList
                        .OrderByDescending(volumePrice => volumePrice.Price)
                        .Select(volumePrice => volumePrice.Level.ToEnumDesc()).Join("")))
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductPrice", "AuditRequestProductPriceResult13"));
            }

            #endregion

            #region VIPPrice

            if (requestInfo.IsUseAlipayVipPrice == IsUseAlipayVipPrice.Yes)
            {
                if (!requestInfo.AlipayVipPrice.HasValue)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductPrice", "AuditRequestProductPriceResult14"));
                }

                if (requestInfo.AlipayVipPrice >= requestInfo.CurrentPrice)
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.ProductPrice", "AuditRequestProductPriceResult15"));
                }

                auditMessage.Append(CheckMargin(requestInfo.AlipayVipPrice.Value,
                                                productInfo, categoryInfo, ResouceManager.GetMessageString("IM.ProductPrice", "AuditRequestProductPriceResult16"), ref auditType));
            }

            #endregion

            return auditMessage.ToString();
        }

        private string CheckMargin(decimal checkPrice, ProductInfo productInfo, CategoryInfo categoryInfo, string priceName, ref ProductPriceRequestAuditType auditType)
        {
            var message = new StringBuilder();
            if (categoryInfo.SysNo.HasValue)
            {
                var setting = ObjectFactory<CategorySettingProcessor>.Instance.GetCategorySettingBySysNo(categoryInfo.SysNo.Value, productInfo.SysNo);
                var priceInfo = productInfo.ProductPriceRequest;
                if (priceInfo.UnitCost != 0)
                {
                    var margin = GetMargin(checkPrice,
                                           priceInfo.Point.HasValue ? priceInfo.Point.Value : 0,
                                           priceInfo.UnitCost);
                    if (margin < setting.PrimaryMargin && margin > setting.SeniorMargin)
                    {
                        message.AppendLine(ResouceManager.GetMessageString("IM.ProductPrice", "CheckMarginResult1") + priceName + ResouceManager.GetMessageString("IM.ProductPrice", "CheckMarginResult2"));
                    }
                    if (margin < setting.SeniorMargin)
                    {
                        message.AppendLine(priceName + ResouceManager.GetMessageString("IM.ProductPrice", "CheckMarginResult3"));
                        auditType = ProductPriceRequestAuditType.SeniorAudit;
                    }

                    var productPromotionDiscountInfoList = ExternalDomainBroker.GetProductPromotionDiscountInfoList(productInfo.SysNo);

                    if (productPromotionDiscountInfoList.Any())
                    {
                        productPromotionDiscountInfoList.ForEach(promotionDiscountInfo =>
                        {
                            var promotionMargin = GetMargin(checkPrice,
                                priceInfo.Point.HasValue ? priceInfo.Point.Value : 0, priceInfo.UnitCost, promotionDiscountInfo.Discount);
                            var enumName = EnumHelper.GetEnumDesc(promotionDiscountInfo.PromotionType);
                            var promotionMarginPercent = (promotionMargin * 100).TruncateDecimal(2) + "%";
                            if (promotionMargin < setting.PrimaryMargin)
                            {
                                message.AppendLine(ResouceManager.GetMessageString("IM.ProductPrice", "CheckMarginResult4") + priceName + ResouceManager.GetMessageString("IM.ProductPrice", "CheckMarginResult5") + enumName
                                    + promotionDiscountInfo.ReferenceSysNo + ResouceManager.GetMessageString("IM.ProductPrice", "CheckMarginResult6")
                                    + promotionMarginPercent + ResouceManager.GetMessageString("IM.ProductPrice", "CheckMarginResult7"));
                            }
                        });
                    }
                }
            }
            else
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductPrice", "CheckMarginResult8"));
            }
            return message.ToString();
        }

        #region Check毛利率，供外部访问

        /// <summary>
        /// Check商品本身及促销活动中毛利率，供外部访问
        /// </summary>
        /// <param name="productPriceReqInfo">价格信息,必须参数包括：CurrentPrice,Point,UnitCost,CategorySysNo</param>
        /// <param name="productSysNo">商品Sysno</param>
        /// <param name="priceName">所修改价格的名称</param>
        /// <param name="productMarginReturnMsg">返回商品本身的毛利率Check结果</param>
        /// <returns></returns>
        public List<ProductPromotionMarginInfo> CheckMargin(ProductPriceRequestInfo productPriceReqInfo,
                                                                int productSysNo,
                                                                string priceName,
                                                                decimal discount,
                                                                ref string productMarginReturnMsg)
        {
            List<ProductPromotionMarginInfo> returnList = new List<ProductPromotionMarginInfo>();
            var categoryInfo = productPriceReqInfo.Category;
            if (categoryInfo.SysNo.HasValue)
            {
                var setting = ObjectFactory<CategorySettingProcessor>.Instance.GetCategorySettingBySysNo(categoryInfo.SysNo.Value, productSysNo);
                var priceInfo = productPriceReqInfo;
                if (priceInfo.UnitCost >= 0)
                {
                    var margin = GetMargin(priceInfo.CurrentPrice.Value,
                                           priceInfo.Point.HasValue ? priceInfo.Point.Value : 0,
                                           priceInfo.UnitCost, discount);
                    if (margin < setting.PrimaryMargin && margin > setting.SeniorMargin)
                    {
                        productMarginReturnMsg = ResouceManager.GetMessageString("IM.ProductPrice", "CheckMarginResult1") + priceName + ResouceManager.GetMessageString("IM.ProductPrice", "CheckMarginResult2");
                    }
                    if (margin < setting.SeniorMargin)
                    {
                        productMarginReturnMsg = priceName + ResouceManager.GetMessageString("IM.ProductPrice", "CheckMarginResult3");
                        //auditType = ProductPriceRequestAuditType.SeniorAudit;
                    }

                    var productPromotionDiscountInfoList = ExternalDomainBroker.GetProductPromotionDiscountInfoList(productSysNo);

                    if (productPromotionDiscountInfoList.Any())
                    {
                        string msgTmp = string.Empty;
                        productPromotionDiscountInfoList.ForEach(promotionDiscountInfo =>
                        {
                            var promotionMargin = GetMargin(priceInfo.CurrentPrice.Value,
                                priceInfo.Point.HasValue ? priceInfo.Point.Value : 0, priceInfo.UnitCost, promotionDiscountInfo.Discount);
                            var enumName = EnumHelper.GetEnumDesc(promotionDiscountInfo.PromotionType);
                            var promotionMarginPercent = (promotionMargin * 100).TruncateDecimal(2) + "%";
                            if (promotionMargin < setting.PrimaryMargin)
                            {
                                msgTmp = ResouceManager.GetMessageString("IM.ProductPrice", "CheckMarginResult4") + priceName + ResouceManager.GetMessageString("IM.ProductPrice", "CheckMarginResult5") + enumName
                                    + promotionDiscountInfo.ReferenceSysNo + ResouceManager.GetMessageString("IM.ProductPrice", "CheckMarginResult6")
                                    + promotionMarginPercent + ResouceManager.GetMessageString("IM.ProductPrice", "CheckMarginResult7");
                            }
                            returnList.Add(new ProductPromotionMarginInfo()
                            {
                                PromotionType = promotionDiscountInfo.PromotionType,
                                ReferenceSysNo = promotionDiscountInfo.ReferenceSysNo,
                                Margin = promotionMargin,
                                ReturnMsg = msgTmp
                            });
                        });
                    }
                }
            }
            else
            {
                throw new BizException(ResouceManager.GetMessageString("IM.ProductPrice", "CheckMarginResult8"));
            }
            return returnList;
        }
        #endregion

        public decimal GetMargin(decimal currentPrice, int point, decimal unitCost)
        {
            return GetMargin(currentPrice, point, unitCost, 0m);
        }

        public decimal GetMargin(decimal currentPrice, int point, decimal unitCost, decimal discount)
        {
            if ((currentPrice - point / 10m) == 0)
            {
                return 0;
            }
            return (currentPrice - discount - (decimal)point / 10 - unitCost) / (currentPrice - point / 10m);
        }

        public decimal GetGiftMargin(decimal currentPrice, int point, decimal unitCost, decimal discount)
        {
            if ((currentPrice - point / 10m) == 0)
            {
                return 0;
            }
            return (-unitCost) / (currentPrice - point / 10m);
        }
        public decimal GetMarginAmount(decimal currentPrice, int point, decimal unitCost)
        {
            return (currentPrice - (decimal)point / 10 - unitCost);
        }
        /// <summary>
        /// 如果没有修改Request，则赋原值
        /// </summary>
        private void ProcessNoRequestPrice(ProductPriceRequestInfo requestInfo, ProductPriceInfo priceInfo)
        {
            if (!requestInfo.CurrentPrice.HasValue)
            {
                requestInfo.CurrentPrice = priceInfo.CurrentPrice;
            }
            if (!requestInfo.CashRebate.HasValue)
            {
                requestInfo.CashRebate = priceInfo.CashRebate;
            }
            if (!requestInfo.Point.HasValue)
            {
                requestInfo.Point = priceInfo.Point;
            }

            requestInfo.UnitCost = priceInfo.UnitCost;

            requestInfo.ProductRankPrice.Where(rankPrice => !rankPrice.RankPrice.HasValue).ForEach(
                rankPrice =>
                rankPrice.RankPrice =
                priceInfo.ProductRankPrice.First(p => p.Rank == rankPrice.Rank).RankPrice);

            requestInfo.ProductWholeSalePriceInfo.ForEach(
                delegate(ProductWholeSalePriceInfo volumePrice)
                {
                    var f = priceInfo.ProductWholeSalePriceInfo.FirstOrDefault(
                                               p => p.Level == volumePrice.Level);
                    int? qty = f != null ? f.Qty : default(int?);
                    decimal? price = f != null ? f.Price : default(decimal?);

                    volumePrice.Qty = volumePrice.Qty.HasValue
                                          ? volumePrice.Qty.Value
                                          : qty;

                    volumePrice.Price = volumePrice.Price.HasValue
                                            ? volumePrice.Price
                                            : price;
                });
            if (!requestInfo.AlipayVipPrice.HasValue)
            {
                requestInfo.AlipayVipPrice = priceInfo.AlipayVipPrice;
            }
        }

        public virtual PriceChangeLogInfo GetPriceChangeLogInfoByProductSysNo(int productSysNo, DateTime startTime, DateTime endTime)
        {
            return _productPriceDA.GetPriceChangeLogInfoByProductSysNo(productSysNo, startTime, endTime);
        }

        public virtual void RollBackPriceWhenCountdownInterrupted(CountdownInfo countdownInfo)
        {

        }

        public virtual void UpdateProductVirtualPrice(int productSysNo, decimal originalVirtualPrice, decimal newVirtualPrice)
        {
            _productPriceDA.UpdateProductVirtualPrice(productSysNo, originalVirtualPrice, newVirtualPrice);
        }

        public virtual void UpdateProductBasicPrice(int productSysNo, decimal newPrice)
        {
            _productPriceDA.UpdateProductBasicPriceOnly(productSysNo, newPrice);
        }

        public virtual void UpdateProductCurrentPrice(int productSysNo, decimal newPrice)
        {
            _productPriceDA.UpdateProductCurrentPriceOnly(productSysNo, newPrice);
        }

    }
}
