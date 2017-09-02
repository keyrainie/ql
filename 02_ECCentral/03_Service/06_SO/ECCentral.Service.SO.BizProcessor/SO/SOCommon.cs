using System;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using System.Linq;
using System.Transactions;
using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using ECCentral.Service.IBizInteract;

namespace ECCentral.Service.SO.BizProcessor
{
    internal static class SOCommon
    {
        /// <summary>
        /// 验证是否是大件 
        /// </summary>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static bool ValidateIsLarge(decimal weight)//CalcIsLarge(SOEntity soInfo)
        {
            return weight >= AppSettingHelper.LargeProductWeight;
        }


        /// <summary>
        /// 计算订单总重量
        /// </summary>
        /// <param name="weight"></param>
        /// <param name="shipTypeSysNo"></param>
        /// <returns></returns>
        public static int GetSOWeight(SOInfo soInfo)
        {
            int weight = GetTotalItemWeight(soInfo);

            if (soInfo.ShippingInfo.IsWithPackFee)
            {
                if (soInfo.ShippingInfo.Weight > 0 && soInfo.ShippingInfo.Weight <= 1000)
                {
                    weight += 250;
                }
                else if (soInfo.ShippingInfo.Weight > 1000 && soInfo.ShippingInfo.Weight <= 3000)
                {
                    weight += 750;
                }
                else if (soInfo.ShippingInfo.Weight > 3000)
                {
                    weight += 1250;
                }
            }
            return weight;
        }

        public static int GetTotalItemWeight(SOInfo soInfo)
        {
            return soInfo.Items.Sum(p => p.Quantity.Value * p.Weight ?? 0);
        }

        /// <summary>
        /// 计算包裹费
        /// </summary>
        /// <param name="weight"></param>
        /// <returns></returns>
        public static decimal GetSOPackageFee(SOInfo soInfo)
        {
            decimal packCost = 0m;
            if (soInfo.ShippingInfo.IsWithPackFee)
            {
                if (soInfo.ShippingInfo.Weight > 0 && soInfo.ShippingInfo.Weight <= 3000)
                {
                    packCost = 2m;
                }
                else if (soInfo.ShippingInfo.Weight > 1000 && soInfo.ShippingInfo.Weight <= 5000)
                {
                    packCost = 4m;
                }
                else if (soInfo.ShippingInfo.Weight > 5000)
                {
                    packCost = 6m;
                }

            }
            return packCost;
        }

        /// <summary>
        /// 判断订单是否存在多个仓库
        /// </summary>
        /// <param name="soItemList"></param>
        /// <returns></returns>
        public static bool IsMultiStock(List<SOItemInfo> soItemList)
        {
            List<SOItemInfo> soItemListTemp = soItemList.FindAll(item => item.ProductType == SOProductType.Product
                                    || item.ProductType == SOProductType.Gift
                                    || item.ProductType == SOProductType.Award
                                    || item.ProductType == SOProductType.Accessory
                                    || item.ProductType == SOProductType.SelfGift
                                    );
            bool IsMultiStock = false;
            foreach (var item in soItemListTemp)
            {
                foreach (var itemInner in soItemListTemp)
                {
                    if (item.StockSysNo.Value != itemInner.StockSysNo.Value)
                    {
                        IsMultiStock = true;
                        return IsMultiStock;
                    }
                }
            }
            return IsMultiStock;
        }

        /// <summary>
        /// 计算发票金额
        /// </summary>
        /// <param name="cashPay">现金支付金额，即订单商品金额除去优惠券折扣和积分支付折扣的金额</param>
        /// <param name="premiumAmount">保价费(>=0)</param>
        /// <param name="shipPrice">运费(>=0)</param>
        /// <param name="payPrice">手续费(>=0)</param>
        /// <param name="promotionAmount">促销折扣(&lt;=0)</param>
        /// <param name="giftCardPay">礼品卡支付(>=0)</param>
        /// <param name="isPayWhenReceived">是否是货到付款</param>
        /// <param name="isCombine">是否是并单</param>
        /// <returns></returns>
        public static decimal CalculateInvoiceAmount(decimal cashPay, decimal premiumAmount, decimal shipPrice
            , decimal payPrice, decimal promotionAmount, decimal giftCardPay, bool isPayWhenReceived)
        {
            decimal result =
                  cashPay
                + premiumAmount
                + shipPrice
                + payPrice
                + promotionAmount
                - giftCardPay;

            if (isPayWhenReceived)
            {
                result = UtilityHelper.TruncMoney(result);
            }

            return result;
        }


        /// <summary>
        /// 计算应收金额
        /// </summary>
        /// <param name="cashPay">现金支付金额，即订单商品金额除去优惠券折扣和积分支付折扣的金额</param>
        /// <param name="premiumAmount">保价费(>=0)</param>
        /// <param name="shipPrice">运费(>=0)</param>
        /// <param name="payPrice">手续费(>=0)</param>
        /// <param name="promotionAmount">促销折扣(&lt;=0)</param>
        /// <param name="giftCardPay">礼品卡支付(>=0)</param>
        /// <param name="prepayAmount">余额支付(>=0)</param>
        /// <param name="isPayWhenReceived">是否是货到付款</param>
        /// <param name="isCombine">是否是并单</param>
        /// <returns></returns>
        public static decimal CalculateReceivableAmount(decimal cashPay, decimal premiumAmount, decimal shipPrice
            , decimal payPrice, decimal promotionAmount, decimal giftCardPay, decimal prepayAmount, bool isPayWhenReceived)
        {
            decimal result =
                  cashPay
                + premiumAmount
                + shipPrice
                + payPrice
                + promotionAmount
                - giftCardPay
                - prepayAmount;
            if (isPayWhenReceived)
            {
                result = UtilityHelper.TruncMoney(result);
            }
            return result;
        }

        /// <summary>
        /// 验证配送方式与支付方式是否匹配
        /// </summary>
        /// <param name="sOInfo"></param>
        public static bool ValidatePayTypeAndShipType(SOInfo sOInfo)
        {
            return ObjectFactory<ISODA>.Instance.CheckPayTypeByShipType(sOInfo.BaseInfo.PayTypeSysNo.Value
                                                                       , sOInfo.ShippingInfo.ShipTypeSysNo.Value
                                                                       , sOInfo.BaseInfo.CompanyCode);
        }

        /// <summary>
        /// 获取货到付款禁运三级类别列表
        /// </summary>
        /// <param name="companyCode">公司编号</param>
        /// <returns></returns>
        public static List<int> GetEmbargoC3s(string companyCode)
        {
            List<int> c3Nos = new List<int>();
#warning ConfigKey 需要配置起来
            string getConfigKey = "EmbargoC3s";
            string strC3Nos = ObjectFactory<ICommonBizInteract>.Instance.GetSystemConfigurationValue(getConfigKey, companyCode);
            if (!string.IsNullOrEmpty(strC3Nos))
            {
                List<string> c3NosArray = strC3Nos.Split(',').ToList();
                foreach (var item in c3NosArray)
                {
                    c3Nos.Add(Convert.ToInt32(item));
                }
            }
            return c3Nos;
        }

        /// <summary>
        ///  获取货到付款禁运商品列表
        /// </summary>
        /// <param name="companyCode">公司编号</param>
        /// <returns></returns>
        public static List<int> GetEmbargoProducts(string companyCode)
        {
            List<int> embargoProducts = new List<int>();
#warning ConfigKey 需要配置起来
            string getConfigKey = "EmbargoProducts";
            string strEmbargoProducts = ObjectFactory<ICommonBizInteract>.Instance.GetSystemConfigurationValue(getConfigKey, companyCode);
            if (!string.IsNullOrEmpty(strEmbargoProducts))
            {
                List<string> strEmbargoProductsArray = strEmbargoProducts.Split(',').ToList();
                foreach (var item in strEmbargoProductsArray)
                {
                    embargoProducts.Add(Convert.ToInt32(item));
                }
            }
            return embargoProducts;
        }

        /// <summary>
        /// 计算订单中商品单件最大重量
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static int GetSOSingleMaxWeight(List<SOItemInfo> list)
        {
            int maxWeight = 0;

            if (list == null)
            {
                return maxWeight;
            }
            foreach (SOItemInfo item in list)
            {
                if (item.Weight > maxWeight)
                {
                    maxWeight = item.Weight.Value;
                }
            }
            return maxWeight;
        }

        /// <summary>
        /// 计算订单总重量
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static int GetTotalWeight(List<SOItemInfo> list)
        {
            int weight = 0;
            foreach (SOItemInfo item in list)
            {
                weight += item.Quantity.Value * item.Weight.Value;
            }
            return weight;
        }
    }
}
