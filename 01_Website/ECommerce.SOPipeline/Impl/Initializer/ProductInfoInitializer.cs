using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECommerce.Enums;
using ECommerce.Entity.Product;
using ECommerce.Entity;
using ECommerce.Entity.Common;
using System.Web;
using ECommerce.DataAccess.Common;
using System.Web.Caching;

namespace ECommerce.SOPipeline.Impl
{
    /// <summary>
    /// 商品信息初始化
    /// </summary>
    public class ProductInfoInitializer : IInitialize
    {
        public void Initialize(ref OrderInfo order)
        {
            int currencySysno;
            int.TryParse(ConstValue.CurrencySysNo, out currencySysno);
            decimal exchangeRate = PipelineDA.GetCurrencyExchangeRate(currencySysno);
            order["CurrencySysNo"] = currencySysno;
            order["ExchangeRate"] = exchangeRate;

            foreach (OrderItemGroup itemGroup in order.OrderItemGroupList)
            {
                int minCountPerOrder = 0;
                int maxCountPerOrder = 0;
                foreach (OrderProductItem item in itemGroup.ProductItemList)
                {
                    //商品基础信息
                    ProductBasicInfo basicInfo = PipelineDA.GetProductBasicInfoBySysNo(item.ProductSysNo);
                    //商品备案信息
                    //ProductEntryInfo entryInfo = PipelineDA.GetProductEntryInfoBySysNo(item.ProductSysNo);
                    //basicInfo.ProductEntryInfo = entryInfo;
                    //item["ProductStoreType"] = (int)entryInfo.ProductStoreType;
                    //商品备货时间                    
                    //item["LeadTimeDays"] = basicInfo.ProductEntryInfo.LeadTimeDays;

                    //会员价格信息
                    ProductCustomerRankPrice rankPrice=null;
                    if (order.Customer != null && order.Customer.SysNo > 0)
                    {
                        rankPrice = ECommerce.DataAccess.Product.ProductDA.GetProductCustomerRankPrice(order.Customer.SysNo, item.ProductSysNo);
                    }
                    //商品销售信息
                    ProductSalesInfo salesInfo = PipelineDA.GetProductSalesInfoBySysNo(item.ProductSysNo);
                    item.ProductID = basicInfo.Code;
                    item.ProductName = basicInfo.ProductName;
                    item.Weight = basicInfo.Weight;
                    item["ProductTitle"] = basicInfo.ProductTitle;
                    item.DefaultImage = basicInfo.DefaultImage;
                    item.UnitMarketPrice = salesInfo.MarketPrice;
                    item.UnitCostPrice = salesInfo.UnitCostPrice;
                    //套餐不享受会员价格
                    if (itemGroup.PackageNo > 0 && itemGroup.PackageType == 1)
                    {
                        item.UnitSalePrice = salesInfo.CurrentPrice;
                    }
                    else
                    {
                        item.UnitSalePrice = rankPrice != null && rankPrice.RankPrice < salesInfo.CurrentPrice ? rankPrice.RankPrice : salesInfo.CurrentPrice;
                    }
                    item.UnitRewardedPoint = salesInfo.Point;
                    //由calculators计算
                    //item.UnitTaxFee = salesInfo.EntryTax.HasValue ? salesInfo.EntryTax.Value : 0m;
                    //item["TariffRate"] = basicInfo.ProductEntryInfo.TariffRate;
                    item.TotalInventory = salesInfo.OnlineQty;
                    item.MerchantSysNo = basicInfo.VendorSysno;
                    item.MerchantName = basicInfo.VendorInfo.VendorName;
                    itemGroup.MerchantSysNo = basicInfo.VendorSysno;
                    itemGroup.MerchantName = basicInfo.VendorInfo.VendorName;
                    item["ProductStatus"] = (int)basicInfo.ProductStatus;
                    item["Warranty"] = basicInfo.Warranty;

                    salesInfo.MinCountPerOrder = salesInfo.OnlineQty < salesInfo.MinCountPerOrder ? salesInfo.OnlineQty : salesInfo.MinCountPerOrder;
                    salesInfo.MaxCountPerOrder = salesInfo.OnlineQty < salesInfo.MaxCountPerOrder ? salesInfo.OnlineQty : salesInfo.MaxCountPerOrder;

                    item["MinCountPerOrder"] = salesInfo.MinCountPerOrder;
                    item["MaxCountPerOrder"] = salesInfo.MaxCountPerOrder;

                    //每单限购计算，套餐则以套餐内商品最小限购数量为准
                    if (minCountPerOrder < salesInfo.MinCountPerOrder / item.UnitQuantity)
                    {
                        minCountPerOrder = salesInfo.MinCountPerOrder / item.UnitQuantity;
                    }
                    if (maxCountPerOrder.Equals(0) || maxCountPerOrder > salesInfo.MaxCountPerOrder / item.UnitQuantity)
                    {
                        maxCountPerOrder = salesInfo.MaxCountPerOrder / item.UnitQuantity;
                    }


                    decimal price = GetProductStepPrice(item.UnitSalePrice, item.ProductSysNo, itemGroup.Quantity);
                    item.UnitSalePrice = price;

                    //商品分组属性
                    List<ProductSplitGroupProperty> splitGroupProperty = PipelineDA.GetProductSplitGroupPropertyList(item.ProductSysNo);
                    if (splitGroupProperty != null && splitGroupProperty.Count > 0)
                    {
                        item.SplitGroupPropertyDescList = new List<KeyValuePair<string, string>>();
                        foreach (ProductSplitGroupProperty property in splitGroupProperty)
                        {
                            item.SplitGroupPropertyDescList.Add(new KeyValuePair<string, string>(property.PropertyDescription, property.ValueDescription));
                        }
                    }
                }
                itemGroup.MinCountPerSO = minCountPerOrder;
                itemGroup.MaxCountPerSO = maxCountPerOrder;
                //套餐名称
                if (itemGroup.PackageType.Equals(1))
                {
                    var combo = PromotionDA.GetComboByComboSysNo(itemGroup.PackageNo);
                    if (combo != null)
                        itemGroup["PackageName"] = combo.SaleRuleName;
                }
            }
        }


        private static decimal GetProductStepPrice(decimal oldPrice, int productSysNo, int productCount)
        {
            List<ProductStepPrice> productStepPrices = null;

            productStepPrices = CommonDA.GetProductStepPrice(productSysNo);


            if (productStepPrices == null)
            {
                return oldPrice;
            }
            ProductStepPrice productStepPrice = productStepPrices.FirstOrDefault(c => (c.BaseCount <= productCount && productCount <= c.TopCount));
            if (productStepPrice == null)
            {
                return productStepPrices.FirstOrDefault(c => (c.BaseCount == 0 && 0 == c.TopCount)).StepPrice;
            }
            return productStepPrice.StepPrice;
        }
    }
}
