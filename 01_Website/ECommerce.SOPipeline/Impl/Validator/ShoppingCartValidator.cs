using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Product;

namespace ECommerce.SOPipeline.Impl
{
    /// <summary>
    /// 购物车检查
    /// </summary>
    public class ShoppingCartValidator : IValidate
    {
        public bool Validate(OrderInfo order, out string errorMsg)
        {
            errorMsg = string.Empty;
            List<string> errorMsgList = new List<string>();

            #region [ 商品库存检查 ]
            if (!ValidateQuantity(order, out errorMsgList))
            {
                foreach (var item in errorMsgList)
                {
                    errorMsg += item + "<br />";
                }
                return false;
            }
            #endregion

            foreach (OrderItemGroup itemGroup in order.OrderItemGroupList)
            {
                if (itemGroup.PackageChecked)
                {
                    if (itemGroup.PackageType == 1)
                    {
                        foreach (OrderProductItem item in itemGroup.ProductItemList)
                        {
                            #region 1.商品状态检查
                            if (!item["ProductStatus"].ToString().Equals(((int)ECommerce.Enums.ProductStatus.Show).ToString()))
                            {
                                errorMsg = LanguageHelper.GetText("商品【{0}】未上架！", order.LanguageCode);
                                errorMsg = string.Format(errorMsg, item["ProductTitle"]);
                                return false;
                            }
                            #endregion

                            #region 2.商品库存检查
                            //if (itemGroup.Quantity * item.UnitQuantity > item.TotalInventory)
                            //{
                            //    errorMsg = LanguageHelper.GetText("商品【{0}】库存不足！", order.LanguageCode);
                            //    errorMsg = string.Format(errorMsg, item["ProductTitle"]);
                            //    return false;
                            //}
                            #endregion

                            #region 3.每单限购最小数量检查
                            int minCountPerOrder = 0;
                            int.TryParse(item["MinCountPerOrder"].ToString(), out minCountPerOrder);
                            if (itemGroup.Quantity * item.UnitQuantity < minCountPerOrder)
                            {
                                errorMsg = LanguageHelper.GetText("商品【{0}】每单限购数量{1}-{2}！", order.LanguageCode);
                                errorMsg = string.Format(errorMsg, item["ProductTitle"], item["MinCountPerOrder"], item["MaxCountPerOrder"]);
                                return false;
                            }
                            #endregion

                            #region 4.每单限购最大数量检查
                            int maxCountPerOrder = 0;
                            int.TryParse(item["MaxCountPerOrder"].ToString(), out maxCountPerOrder);
                            if (itemGroup.Quantity * item.UnitQuantity > maxCountPerOrder)
                            {
                                errorMsg = LanguageHelper.GetText("商品【{0}】每单限购数量{1}-{2}！", order.LanguageCode);
                                errorMsg = string.Format(errorMsg, item["ProductTitle"], item["MinCountPerOrder"], item["MaxCountPerOrder"]);
                                return false;
                            }
                            #endregion

                            List<OrderGiftItem> giftList = null;
                            if (order.GiftItemList != null && order.GiftItemList.Count > 0)
                            {
                                giftList = order.GiftItemList.FindAll(m
                                => m.ParentPackageNo == itemGroup.PackageNo
                                && m.MerchantSysNo == item.MerchantSysNo
                                && m.ParentProductSysNo == item.ProductSysNo);
                            }
                            if (giftList == null)
                            {
                                giftList = new List<OrderGiftItem>();
                            }
                            //非赠品池赠品
                            var normalGiftList = giftList.FindAll(m => !m.IsGiftPool);
                            foreach (var gift in normalGiftList)
                            {
                                ProductSalesInfo giftSalesInfo = PipelineDA.GetProductSalesInfoBySysNo(gift.ProductSysNo);
                                gift.TotalInventory = giftSalesInfo.OnlineQty;
                                if (gift.UnitQuantity * gift.ParentCount > gift.TotalInventory)
                                {
                                    errorMsg = LanguageHelper.GetText("赠品【{0}】库存不足！", order.LanguageCode);
                                    errorMsg = string.Format(errorMsg, gift.ProductName);
                                    return false;
                                }
                            }
                            //订单级别用户选择的赠品（包含非赠品池删除后保留的赠品和赠品池选择的赠品）
                            List<OrderGiftItem> orderGiftList = new List<OrderGiftItem>();
                            if (order.GiftItemList != null)
                            {
                                orderGiftList = order.GiftItemList.FindAll(m => m.ParentPackageNo.Equals(0)
                                            && m.ParentProductSysNo.Equals(0)
                                            && ((m.IsGiftPool && m.IsSelect) || !m.IsGiftPool));
                            }

                            var productList = order.OrderItemGroupList.FindAll(m => m.MerchantSysNo == item.MerchantSysNo);
                            var merchantOrderGiftList = orderGiftList.FindAll(m => m.MerchantSysNo == item.MerchantSysNo);
                            if (merchantOrderGiftList != null && merchantOrderGiftList.Count > 0)
                            {
                                for (int index = 0; index < merchantOrderGiftList.Count; index++)
                                {
                                    var Gift = merchantOrderGiftList[index];
                                    var itemGroupList = productList.FindAll(m => m.MerchantSysNo == merchantOrderGiftList[index].MerchantSysNo && m.PackageChecked);
                                    if (itemGroupList.Count > 0 && itemGroupList != null)
                                    {
                                        ProductSalesInfo giftSalesInfo = PipelineDA.GetProductSalesInfoBySysNo(merchantOrderGiftList[index].ProductSysNo);
                                        merchantOrderGiftList[index].TotalInventory = giftSalesInfo.OnlineQty;
                                        if (merchantOrderGiftList[index].UnitQuantity * merchantOrderGiftList[index].ParentCount > merchantOrderGiftList[index].TotalInventory)
                                        {
                                            errorMsg = LanguageHelper.GetText("赠品【{0}】库存不足！", order.LanguageCode);
                                            errorMsg = string.Format(errorMsg, merchantOrderGiftList[index].ProductName);
                                            return false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (itemGroup.PackageType == 0)
                    {
                        foreach (OrderProductItem item in itemGroup.ProductItemList)
                        {
                            if (item.ProductChecked)
                            {
                                #region 1.商品状态检查
                                if (!item["ProductStatus"].ToString().Equals(((int)ECommerce.Enums.ProductStatus.Show).ToString()))
                                {
                                    errorMsg = LanguageHelper.GetText("商品【{0}】未上架！", order.LanguageCode);
                                    errorMsg = string.Format(errorMsg, item["ProductTitle"]);
                                    return false;
                                }
                                #endregion

                                #region 2.商品库存检查
                                //if (itemGroup.Quantity * item.UnitQuantity > item.TotalInventory)
                                //{
                                //    errorMsg = LanguageHelper.GetText("商品【{0}】库存不足！", order.LanguageCode);
                                //    errorMsg = string.Format(errorMsg, item["ProductTitle"]);
                                //    return false;
                                //}
                                #endregion

                                #region 3.每单限购最小数量检查
                                int minCountPerOrder = 0;
                                int.TryParse(item["MinCountPerOrder"].ToString(), out minCountPerOrder);
                                if (itemGroup.Quantity * item.UnitQuantity < minCountPerOrder)
                                {
                                    errorMsg = LanguageHelper.GetText("商品【{0}】每单限购数量{1}-{2}！", order.LanguageCode);
                                    errorMsg = string.Format(errorMsg, item["ProductTitle"], item["MinCountPerOrder"], item["MaxCountPerOrder"]);
                                    return false;
                                }
                                #endregion

                                #region 4.每单限购最大数量检查
                                int maxCountPerOrder = 0;
                                int.TryParse(item["MaxCountPerOrder"].ToString(), out maxCountPerOrder);
                                if (itemGroup.Quantity * item.UnitQuantity > maxCountPerOrder)
                                {
                                    errorMsg = LanguageHelper.GetText("商品【{0}】每单限购数量{1}-{2}！", order.LanguageCode);
                                    errorMsg = string.Format(errorMsg, item["ProductTitle"], item["MinCountPerOrder"], item["MaxCountPerOrder"]);
                                    return false;
                                }
                                #endregion


                                //赠品
                                List<OrderGiftItem> giftList = null;
                                if (order.GiftItemList != null && order.GiftItemList.Count > 0)
                                {
                                    giftList = order.GiftItemList.FindAll(m
                                    => m.ParentPackageNo == itemGroup.PackageNo
                                    && m.MerchantSysNo == item.MerchantSysNo
                                    && m.ParentProductSysNo == item.ProductSysNo);
                                }
                                if (giftList == null)
                                {
                                    giftList = new List<OrderGiftItem>();
                                }
                                //非赠品池赠品
                                var normalGiftList = giftList.FindAll(m => !m.IsGiftPool);
                                foreach (var gift in normalGiftList)
                                {
                                    ProductSalesInfo giftSalesInfo = PipelineDA.GetProductSalesInfoBySysNo(gift.ProductSysNo);
                                    gift.TotalInventory = giftSalesInfo.OnlineQty;
                                    if (gift.UnitQuantity * gift.ParentCount > gift.TotalInventory)
                                    {
                                        errorMsg = LanguageHelper.GetText("赠品【{0}】库存不足！", order.LanguageCode);
                                        errorMsg = string.Format(errorMsg, gift.ProductName);
                                        return false;
                                    }
                                }

                                //订单级别用户选择的赠品（包含非赠品池删除后保留的赠品和赠品池选择的赠品）
                                List<OrderGiftItem> orderGiftList = new List<OrderGiftItem>();
                                if (order.GiftItemList != null)
                                {
                                    orderGiftList = order.GiftItemList.FindAll(m => m.ParentPackageNo.Equals(0)
                                                && m.ParentProductSysNo.Equals(0)
                                                && ((m.IsGiftPool && m.IsSelect) || !m.IsGiftPool));
                                }

                                var productList = order.OrderItemGroupList.FindAll(m => m.MerchantSysNo == item.MerchantSysNo);
                                var merchantOrderGiftList = orderGiftList.FindAll(m => m.MerchantSysNo == item.MerchantSysNo);
                                if (merchantOrderGiftList != null && merchantOrderGiftList.Count > 0)
                                {
                                    for (int index = 0; index < merchantOrderGiftList.Count; index++)
                                    {
                                        var Gift = merchantOrderGiftList[index];
                                        var itemGroupList = productList.FindAll(m => m.MerchantSysNo == merchantOrderGiftList[index].MerchantSysNo && m.PackageChecked);
                                        if (itemGroupList.Count > 0 && itemGroupList != null)
                                        {
                                            ProductSalesInfo giftSalesInfo = PipelineDA.GetProductSalesInfoBySysNo(merchantOrderGiftList[index].ProductSysNo);
                                            merchantOrderGiftList[index].TotalInventory = giftSalesInfo.OnlineQty;
                                            if (merchantOrderGiftList[index].UnitQuantity * merchantOrderGiftList[index].ParentCount > merchantOrderGiftList[index].TotalInventory)
                                            {
                                                errorMsg = LanguageHelper.GetText("赠品【{0}】库存不足！", order.LanguageCode);
                                                errorMsg = string.Format(errorMsg, merchantOrderGiftList[index].ProductName);
                                                return false;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            #region 优惠促销库存检查

            if (order.AttachmentItemList != null)
            {
                foreach (OrderAttachment attachment in order.AttachmentItemList)
                {
                    ProductSalesInfo attachmentSalesInfo = PipelineDA.GetProductSalesInfoBySysNo(attachment.ProductSysNo);
                    attachment.TotalInventory = attachmentSalesInfo.OnlineQty;
                    if (attachment.UnitQuantity * attachment.ParentCount > attachment.TotalInventory)
                    {
                        errorMsg = LanguageHelper.GetText("附件【{0}】库存不足！", order.LanguageCode);
                        errorMsg = string.Format(errorMsg, attachment.ProductName);
                        return false;
                    }
                }
            }
            if (order.GiftItemList != null)
            {
                foreach (OrderGiftItem gift in order.GiftItemList)
                {
                    if (gift.IsSelect)
                    {
                        ProductSalesInfo giftSalesInfo = PipelineDA.GetProductSalesInfoBySysNo(gift.ProductSysNo);
                        gift.TotalInventory = giftSalesInfo.OnlineQty;
                        if (gift.UnitQuantity * gift.ParentCount > gift.TotalInventory)
                        {
                            errorMsg = LanguageHelper.GetText("赠品【{0}】库存不足！", order.LanguageCode);
                            errorMsg = string.Format(errorMsg, gift.ProductName);
                            return false;
                        }
                    }
                }
            }
            if (order.PlusPriceItemList != null)
            {
                foreach (OrderGiftItem gift in order.PlusPriceItemList)
                {
                    if (gift.IsSelect)
                    {
                        ProductSalesInfo giftSalesInfo = PipelineDA.GetProductSalesInfoBySysNo(gift.ProductSysNo);
                        gift.TotalInventory = giftSalesInfo.OnlineQty;
                        if (gift.UnitQuantity > gift.TotalInventory)
                        {
                            errorMsg = LanguageHelper.GetText("加购商品【{0}】库存不足！", order.LanguageCode);
                            errorMsg = string.Format(errorMsg, gift.ProductName);
                            return false;
                        }
                    }
                }
            }

            #endregion

            errorMsg = null;
            return true;
        }

        /// <summary>
        /// 订单所有商品库存检查
        /// Validates the quantity.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="errorMsgList">The error MSG list.</param>
        /// <returns></returns>
        private bool ValidateQuantity(OrderInfo order, out List<string> errorMsgList)
        {
            bool result = true;
            errorMsgList = new List<string>();

            foreach (OrderItemGroup itemGroup in order.OrderItemGroupList)
            {
                if (itemGroup.PackageChecked)
                {
                    if (itemGroup.PackageType == 1)
                    {
                        foreach (OrderProductItem item in itemGroup.ProductItemList)
                        {
                            if (itemGroup.Quantity * item.UnitQuantity > item.TotalInventory)
                            {
                                string msg = LanguageHelper.GetText("商品【{0}】库存不足！", order.LanguageCode);
                                msg = string.Format(msg, item["ProductTitle"]);
                                errorMsgList.Add(msg);
                            }
                        }
                    }
                    else if (itemGroup.PackageType == 0)
                    {
                        foreach (OrderProductItem item in itemGroup.ProductItemList)
                        {
                            if (itemGroup.Quantity * item.UnitQuantity > item.TotalInventory && item.ProductChecked)
                            {
                                string msg = LanguageHelper.GetText("商品【{0}】库存不足！", order.LanguageCode);
                                msg = string.Format(msg, item["ProductTitle"]);
                                errorMsgList.Add(msg);
                            }
                        }
                    }
                }
            }

            result = (errorMsgList.Count <= 0);

            return result;
        }
    }
}
