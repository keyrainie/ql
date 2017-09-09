using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Payment;
using ECommerce.Entity.Shopping;
using ECommerce.Entity.Product;
using ECommerce.Utility.DataAccess;
using ECommerce.Entity;
using ECommerce.Entity.Shipping;
using System.Web;
using System.Web.Caching;
using System.Data;
using ECommerce.Entity.Order;
using ECommerce.Enums;
using ECommerce.Utility;

namespace ECommerce.SOPipeline.Impl
{
    public class PipelineDA
    {
        #region 优惠券
        /// <summary>
        /// 查询优惠券使用数量
        /// </summary>
        /// <param name="couponCodeSysNo"></param>
        /// <returns></returns>
        public static int GetCouponCodeUseQuantity(int couponCodeSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_GetCouponCodeUsedQuantity");
            cmd.SetParameterValue("@CouponCodeSysNo", couponCodeSysNo);
            cmd.SetParameterValue("@Times", 1);
            return cmd.ExecuteScalar<int>();
        }

        /// <summary>
        /// 查询扩展优惠券使用数量
        /// </summary>
        /// <param name="couponCodeSysNo"></param>
        /// <returns></returns>
        public static int CouponSaleRulesExUserQuantity(int couponCodeSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_CouponSaleRulesExUsedQuantity");
            cmd.SetParameterValue("@CouponCodeSysNo", couponCodeSysNo);
            cmd.SetParameterValue("@Times", 1);
            return cmd.ExecuteScalar<int>();
        }

        /// <summary>
        /// 更新优惠券使用数量
        /// </summary>
        /// <param name="couponCodeSysNo">优惠券编号</param>
        public static void UpdateCouponCodeQuantity(int couponCodeSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_UpdateCouponCodeQuantity");
            cmd.SetParameterValue("@CouponCodeSysNo", couponCodeSysNo);
            cmd.SetParameterValue("@Times", 1);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新扩展优惠券使用数量
        /// </summary>
        /// <param name="couponCodeSysNo">优惠券编号</param>
        /// <returns></returns>
        public static void UpdateSaleRulesExQuantity(int couponCodeSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_UpdateSaleRulesExQuantity");
            cmd.SetParameterValue("@CouponCodeSysNo", couponCodeSysNo);
            cmd.SetParameterValue("@Times", 1);
            cmd.ExecuteNonQuery();
        }

        #endregion

        #region 购物车
        /// <summary>
        /// 是否已存在购物车订单
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static bool IsExistShoppingCart(OrderInfo order)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_IsExistShoppingCart");
            cmd.SetParameterValue("@ShoppingCartSysNo", order.ShoppingCartID);
            cmd.SetParameterValue("@SOSysNo", order.ShoppingCartID);
            cmd.SetParameterValue("@CustomerSysNo", order.Customer.SysNo);
            cmd.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            cmd.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            cmd.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
            return cmd.ExecuteScalar<int>() > 0;
        }

        /// <summary>
        /// 创建购物车ID
        /// </summary>
        /// <param name="order"></param>
        public static void CreateShoppingCart(OrderInfo order)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_CreateShoppingCart");
            cmd.SetParameterValue("@ShoppingCartSysNo", order.ShoppingCartID);
            cmd.SetParameterValue("@SOSysNo", order.ID);
            cmd.SetParameterValue("@CustomerSysNo", order.Customer.SysNo);
            cmd.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            cmd.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            cmd.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
            cmd.ExecuteNonQuery();
        }

        #endregion

        #region Order
        /// <summary>
        /// 根据主商品编号判断此赠品活动是否在运行中
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns>true:运行中；false：</returns>
        public static bool CheckGiftSaleListByProductSysNo(int productSysNo)
        {
            bool result = false;
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_CheckGiftSaleListByProductSysNo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            int i = cmd.ExecuteScalar<Int32>();
            if (i > 0)
            {
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 根据主商品编号判断此限时抢购活动是否在运行中
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns>true:运行中；false：</returns>
        public static bool CheckCountDownByProductSysNo(int productSysNo)
        {
            bool result = false;
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_CheckCountDownByProductSysNo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            int i = cmd.ExecuteScalar<Int32>();
            if (i > 0)
            {
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 生成订单编号
        /// </summary>
        /// <returns>订单编号</returns>
        public static int GenerateSOSysNo()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_GenerateSOSysNo");
            return cmd.ExecuteScalar<int>();
        }

        /// <summary>
        /// 扣减商品库存：获取库存模式
        /// </summary>
        /// <param name="item">订单详细</param>
        public static int GetInventoryType(OrderItem item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_GetInventoryType");
            cmd.SetParameterValue("@ProductSysNo", item.ProductSysNo);
            return cmd.ExecuteScalar<int>();
        }

        /// <summary>
        /// 扣减商品库存：更新总的库存
        /// </summary>
        /// <param name="item">订单详细</param>
        public static int UpdateInventory(OrderItem item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_UpdateInventory");
            cmd.SetParameterValue("@ProductSysNo", item.ProductSysNo);
            cmd.SetParameterValue("@Quantity", item["Quantity"]);
            cmd.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            cmd.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            cmd.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 扣减商品库存：更新对应仓库的库存
        /// </summary>
        /// <param name="item">订单详细</param>
        public static int UpdateInventoryStock(OrderItem item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_UpdateInventoryStock");
            cmd.SetParameterValue("@ProductSysNo", item.ProductSysNo);
            cmd.SetParameterValue("@Quantity", item["Quantity"]);
            cmd.SetParameterValue("@WarehouseNumber", item.WarehouseNumber);
            cmd.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            cmd.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            cmd.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 创建主订单信息
        /// </summary>
        /// <param name="order">订单信息</param>
        public static void CreateSOMaster(OrderInfo order)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_CreateSOMaster");
            cmd.SetParameterValue("@SOSysNo", order.ID);
            cmd.SetParameterValue("@SOAmount", order.TotalProductAmount);
            cmd.SetParameterValue("@Status", SOStatus.Original);
            cmd.SetParameterValue("@StockSysNo", order["WarehouseNumber"]);
            cmd.SetParameterValue("@CashPayAmount", order.TotalProductAmount - order.PointPayAmount - order.CouponAmount);
            cmd.SetParameterValue("@CustomerSysNo", order.Customer.SysNo);
            cmd.SetParameterValue("@DiscountAmount", -1 * order.TotalDiscountAmount);
            cmd.SetParameterValue("@InvoiceNo", order.Receipt.VATInvoiceTaxPayerNo);//增值税信息编号
            cmd.SetParameterValue("@InvoiceNote", order.Receipt.PersonalInvoiceContent);//发票备注
            cmd.SetParameterValue("@IsLarge", 0);//是否大货
            cmd.SetParameterValue("@IsMobilePhone", order.OrderSource);
            cmd.SetParameterValue("@IsPremium", 0);
            cmd.SetParameterValue("@IsUseChequesPay", 0);
            cmd.SetParameterValue("@IsUsePrepay", (int)order["IsUsePrepay"]);
            cmd.SetParameterValue("@IsWholeSale", 0);
            cmd.SetParameterValue("@IsVAT", order.Receipt.IsVATInvoice ? 1 : 0);
            cmd.SetParameterValue("@ReceiveName", order.Receipt.PersonalInvoiceTitle);
            cmd.SetParameterValue("@Memo", order.Memo);
            cmd.SetParameterValue("@OrderDataTime", order.InDate);//下单时间
            cmd.SetParameterValue("@PayPrice", order.CommissionAmount);
            cmd.SetParameterValue("@PayTypeSysNo", order.PayTypeID);
            cmd.SetParameterValue("@PointPay", order.PointPay);
            cmd.SetParameterValue("@TotalRewardedPoint", order.TotalRewardedPoint);
            cmd.SetParameterValue("@PremiumAmt", 0);//保价费
            cmd.SetParameterValue("@PrepayAmt", order.BalancePayAmount);
            cmd.SetParameterValue("@GiftCardPay", order.GiftCardPayAmount);
            cmd.SetParameterValue("@PromotionCodeSysNo", order.MerchantCouponCodeSysNo);
            cmd.SetParameterValue("@PlatPromotionCodeSysNo", order.CouponCodeSysNo);
            cmd.SetParameterValue("@PromotionCustomerSysNo", order.Customer.SysNo);
            cmd.SetParameterValue("@ReceiveContact", order.Contact.Name);
            cmd.SetParameterValue("@ReceiveAreaSysNo", order.Contact.AddressAreaID);
            cmd.SetParameterValue("@ReceiveAddress", order.Contact.AddressDetail);
            cmd.SetParameterValue("@ReceiveCellPhone", order.Contact.MobilePhone);
            cmd.SetParameterValue("@ReceivePhone", order.Contact.Phone);
            cmd.SetParameterValue("@ReceiveZip", order.Contact.ZipCode);
            cmd.SetParameterValue("@ShipTypeSysNo", order.ShipTypeID);
            cmd.SetParameterValue("@ShipAmount", order.ShippingAmount);
            //订单的总关税
            cmd.SetParameterValue("@TariffAmt", order.TaxAmount);
            //快递信息
            cmd.SetParameterValue("@DeliveryMemo", null);
            cmd.SetParameterValue("@DeliveryDate", null);
            cmd.SetParameterValue("@DeliveryTimeRange", 0);
            //货币、当时汇率
            cmd.SetParameterValue("@CurrencySysNo", (int)order["CurrencySysNo"]);
            cmd.SetParameterValue("@ExchangeRate", (decimal)order["ExchangeRate"]);

            cmd.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            cmd.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            cmd.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
            //社团ID
            cmd.SetParameterValue("@SocietyID", order.Customer.SocietyID);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 创建订单详细
        /// </summary>
        /// <param name="item">订单详细</param>
        public static void CreateSOItem(OrderItem item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_CreateSOItem");
            cmd.SetParameterValue("@SOSysNo", item["SOSysNo"]);
            cmd.SetParameterValue("@ProductSysNo", item.ProductSysNo);
            cmd.SetParameterValue("@BriefName", item.ProductName);
            cmd.SetParameterValue("@Weight", item.Weight.HasValue ? item.Weight : 0);
            cmd.SetParameterValue("@OriginalPrice", item.UnitSalePrice);
            cmd.SetParameterValue("@Price", item["Price"]);
            cmd.SetParameterValue("@UnitCostPrice", item.UnitCostPrice);
            cmd.SetParameterValue("@UnitCostWithoutTax", item.UnitCostPrice);//不含税成本
            cmd.SetParameterValue("@IsMemberPrice", 0);
            cmd.SetParameterValue("@WarehouseNumber", item.WarehouseNumber);
            cmd.SetParameterValue("@Warranty", item["Warranty"]);
            cmd.SetParameterValue("@GiftSysNo", item["GiftSysNo"]);
            cmd.SetParameterValue("@ProductType", item["ProductType"]);//订单商品类型
            cmd.SetParameterValue("@PointType", (int)ProductPayType.MoneyPayOnly);
            cmd.SetParameterValue("@PromotionDiscount", item["PromotionDiscount"]);
            cmd.SetParameterValue("@PlatPromotionDiscount", item["PlatPromotionDiscount"]);
            cmd.SetParameterValue("@MasterProductCode", item["MasterProductCode"]);
            cmd.SetParameterValue("@DiscountAmt", -1 * ((decimal)item["UnitDiscountAmt"] * (int)item["Quantity"]));
            cmd.SetParameterValue("@Point", item["Point"]);
            cmd.SetParameterValue("@Quantity", item["Quantity"]);

            cmd.SetParameterValue("@TariffAmt", 0);//关税金额
            cmd.SetParameterValue("@TariffCode", "");//关税代码
            cmd.SetParameterValue("@TariffRate", 0);//单个关税
            cmd.SetParameterValue("@EntryRecord", "");//报关编号
            cmd.SetParameterValue("@ProductStoreType", item["ProductStoreType"]);//储存运输方式
            //货币、当时汇率
            cmd.SetParameterValue("@CurrencySysNo", (int)item["CurrencySysNo"]);
            cmd.SetParameterValue("@ExchangeRate", (decimal)item["ExchangeRate"]);

            cmd.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            cmd.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            cmd.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 创建订单整单折扣信息
        /// </summary>
        /// <param name="dtoInfo">促销信息</param>
        public static void CreateSalesRuleInfo(DTOInfo dtoInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_CreateSalesRuleInfo");
            cmd.SetParameterValue("@SOSysNo", dtoInfo["SOSysNo"]);
            cmd.SetParameterValue("@SaleRuleSysNo", dtoInfo["SaleRuleSysNo"]);
            cmd.SetParameterValue("@SaleRuleName", dtoInfo["SaleRuleName"]);
            cmd.SetParameterValue("@Discount", (decimal)dtoInfo["Discount"]);
            cmd.SetParameterValue("@Times", (int)dtoInfo["Times"]);
            cmd.SetParameterValue("@Note", dtoInfo["Note"]);
            cmd.SetParameterValue("@ReferenceType", 1);
            cmd.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            cmd.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            cmd.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新优惠券信息
        /// </summary>
        /// <param name="order">订单信息</param>
        public static void CreateSONewPromotionLog(OrderInfo order)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_UpdatePromotionCodeRedeemLog");
            cmd.SetParameterValue("@SOSysNo", order.ID);
            cmd.SetParameterValue("@CouponCodeSysNo", order.CouponCodeSysNo);
            cmd.SetParameterValue("@CustomerSysNo", order.Customer.SysNo);
            cmd.SetParameterValue("@ShoppingCartSysNo", order.ShoppingCartID);
            cmd.SetParameterValue("@RedeemAmount", order.CouponAmount - order.MerchantCouponAmount);
            cmd.ExecuteNonQuery();
        }
        public static void CreateSONewMerchantPromotionLog(OrderInfo order)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_UpdatePromotionCodeRedeemLog");
            cmd.SetParameterValue("@SOSysNo", order.ID);
            cmd.SetParameterValue("@CouponCodeSysNo", order.MerchantCouponCodeSysNo);
            cmd.SetParameterValue("@CustomerSysNo", order.Customer.SysNo);
            cmd.SetParameterValue("@ShoppingCartSysNo", order.ShoppingCartID);
            cmd.SetParameterValue("@RedeemAmount", order.MerchantCouponAmount);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新用户积分信息
        /// </summary>
        /// <param name="item">积分信息</param>
        /// <returns>1000099：更新成功</returns>
        public static string UpdatePointForCustomer(OrderInfo order)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_UpdatePointForCustomer");
            cmd.SetParameterValue("@SOSysNo", order.ID);
            cmd.SetParameterValue("@CustomerSysNo", order.Customer.SysNo);
            cmd.SetParameterValue("@Point", -1 * order.PointPay);
            cmd.SetParameterValue("@CurrencyCode", (int)order["CurrencySysNo"]);
            cmd.SetParameterValue("@Memo", order["Memo"]);
            cmd.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            cmd.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            cmd.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
            return cmd.ExecuteScalar<string>();
        }

        /// <summary>
        /// 更新用户余额
        /// </summary>
        /// <param name="order">订单信息</param>
        public static void UpdateCustomerPrepayBasic(OrderInfo order)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_UpdateCustomerPrepayBasic");
            cmd.SetParameterValue("@SOSysNo", order.ID);
            cmd.SetParameterValue("@CustomerSysNo", order.Customer.SysNo);
            cmd.SetParameterValue("@ValidPrepayAmt", -1 * order.BalancePayAmount);
            cmd.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            cmd.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            cmd.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新用户扩展信息
        /// </summary>
        /// <param name="order">订单信息</param>
        public static void UpdateCustomerExtend(OrderInfo order)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_UpdateCustomerExtend");
            cmd.SetParameterValue("@CustomerSysNo", order.Customer.SysNo);
            cmd.SetParameterValue("@LastReceiveAreaID", order.Contact.AddressAreaID);
            cmd.SetParameterValue("@PayTypeID", order.PayTypeID);
            cmd.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            cmd.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            cmd.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 订单检查运费
        /// </summary>
        /// <param name="order">订单信息</param>
        public static void UpdateSOCheckShipping(OrderInfo order)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_UpdateSOCheckShipping");
            cmd.SetParameterValue("@SOSysNo", order.ID);
            cmd.SetParameterValue("@CustomerSysNo", order.Customer.SysNo);
            cmd.SetParameterValue("@SOWeightAmount", order.TotalWeight);
            cmd.SetParameterValue("@ShippingFee", order.ShippingAmount);
            cmd.SetParameterValue("@SOType", order.SOType);
            cmd.SetParameterValue("@IsPhoneOrder", order.OrderSource);

            cmd.SetParameterValue("@ShippingType", order["ShippingType"]);
            cmd.SetParameterValue("@StockType", order["StockType"]);
            cmd.SetParameterValue("@InvoiceType", order["InvoiceType"]);
            cmd.SetParameterValue("@LocalWHSysNo", order["LocalWHSysNo"]);
            cmd.SetParameterValue("@MemoForCustomer", order.WarmTips);
            cmd.SetParameterValue("@ReferenceSysNo", order["ReferenceSysNo"]);
            cmd.SetParameterValue("@NeedInvoice", string.IsNullOrWhiteSpace(order.Receipt.PersonalInvoiceTitle) ? 0 : 1);
            cmd.SetParameterValue("@MerchantSysNo", order["MerchantSysNo"]);

            cmd.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            cmd.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            cmd.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 创建订单扩展信息
        /// </summary>
        /// <param name="order">订单信息</param>
        public static void CreateSOMasterExtension(OrderInfo order)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_CreateSOMasterExtension");
            cmd.SetParameterValue("@SOSysNo", order.ID);
            cmd.SetParameterValue("@ExtensionName", string.Empty);
            cmd.SetParameterValue("@ExtensionValue", string.Empty);
            cmd.SetParameterValue("@OrderDataTime", order.InDate);
            cmd.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            cmd.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            cmd.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 订单商品附件
        /// </summary>
        /// <param name="item">订单附件信息</param>
        public static void CreateSOItemAttachmentAccessory(OrderAttachment item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_CreateSOItemAccessory");
            cmd.SetParameterValue("@SOSysNo", item["SOSysNo"]);
            cmd.SetParameterValue("@PromotionSysNo", 0);
            cmd.SetParameterValue("@MasterProductSysNo", item.ParentProductSysNo);
            cmd.SetParameterValue("@ProductSysNo", item.ProductSysNo);
            cmd.SetParameterValue("@Quantity", item.UnitQuantity);
            cmd.SetParameterValue("@Type", 'A');
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 订单商品赠品
        /// </summary>
        /// <param name="item">订单赠品信息</param>
        public static void CreateSOItemGiftAccessory(OrderGiftItem item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_CreateSOItemAccessory");
            cmd.SetParameterValue("@SOSysNo", item["SOSysNo"]);
            cmd.SetParameterValue("@PromotionSysNo", item.ActivityNo);
            cmd.SetParameterValue("@MasterProductSysNo", item.ParentProductSysNo);
            cmd.SetParameterValue("@ProductSysNo", item.ProductSysNo);
            cmd.SetParameterValue("@Quantity", item.UnitQuantity);
            cmd.SetParameterValue("@Type", GetSaleGiftTypeString(item.SaleGiftType));
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 获取活动类型对应的数据库的值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetSaleGiftTypeString(SaleGiftType type)
        {
            switch (type)
            {
                case SaleGiftType.Single:
                    return "S";
                case SaleGiftType.Multiple:
                    return "M";
                case SaleGiftType.Full:
                    return "F";
                case SaleGiftType.Vendor:
                    return "V";
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// 促销活动订单赠送
        /// </summary>
        /// <param name="item">订单赠品信息</param>
        public static void CreateSOGiftMaster(DTOInfo dtoInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_CreateSOGiftMaster");
            cmd.SetParameterValue("@SOSysNo", dtoInfo["SOSysNo"]);
            cmd.SetParameterValue("@PromotionSysNo", dtoInfo["ActivityNo"]);
            cmd.SetParameterValue("@Type", GetSaleGiftTypeString((SaleGiftType)dtoInfo["SaleGiftType"]));
            cmd.SetParameterValue("@Count", dtoInfo["Count"]);//活动应用次数
            cmd.SetParameterValue("@Order", dtoInfo["Order"]);

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 促销活动订单赠品列表
        /// </summary>
        /// <param name="item">订单赠品信息</param>
        public static void CreateSOGiftItem(OrderGiftItem item)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_CreateSOGiftItem");
            cmd.SetParameterValue("@SOSysNo", item["SOSysNo"]);
            cmd.SetParameterValue("@PromotionSysNo", item.ActivityNo);
            cmd.SetParameterValue("@ProductSysNo", item.ProductSysNo);
            cmd.SetParameterValue("@Quantity", item.UnitQuantity);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 订单赠品毛利分配
        /// </summary>
        /// <param name="order">订单信息</param>
        public static void CreateSOItemGrossProfit(OrderInfo order)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_CreateSOItemGrossProfit");
            cmd.SetParameterValue("@SOSysNo", order.ID);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 记录订单支付日志
        /// </summary>
        /// <param name="order">订单信息</param>
        public static void UpdateFinanceNetPayForPrepay(OrderInfo order)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_UpdateFinanceNetPayForPrepay");
            cmd.SetParameterValue("@SOSysNo", order.ID);
            cmd.SetParameterValue("@CashPay", order.TotalProductAmount - order.PointPayAmount - order.CouponAmount);
            cmd.SetParameterValue("@DiscountAmount", -1 * order.TotalDiscountAmount);
            cmd.SetParameterValue("@PayPrice", order.CommissionAmount);
            cmd.SetParameterValue("@ShippingPrice", order.ShippingAmount);
            cmd.SetParameterValue("@TaxAmount", order.TaxAmount);
            cmd.SetParameterValue("@PointPayAmt", order.PointPayAmount);
            cmd.SetParameterValue("@PrepayAmount", order.BalancePayAmount);
            cmd.SetParameterValue("@GiftCardPayAmt", order.GiftCardPayAmount);
            cmd.SetParameterValue("@PayTypeID", order.PayTypeID);
            cmd.SetParameterValue("@CustomerName", order.Contact.Name);
            cmd.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            cmd.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            cmd.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 记录订单购买日志
        /// operatorType=0: Create SO
        /// operatorType=200: Create SO Success
        /// </summary>
        /// <param name="order">订单信息</param>
        public static void UpdateSalesOrderLog(OrderInfo order, int operatorType)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_UpdateSalesOrderLog");
            cmd.SetParameterValue("@SOSysNo", order.ID);
            cmd.SetParameterValue("@CustomerSysNo", order.Customer.SysNo);
            cmd.SetParameterValue("@CustomerIPAddress", order.IPAddress);
            cmd.SetParameterValue("@OperatorType", operatorType);
            cmd.SetParameterValue("@Note", order["Note"]);
            cmd.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            cmd.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            cmd.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 记录订单商品扩展信息
        /// </summary>
        /// <param name="product"></param>
        public static void CreateSOItemExtension(OrderProductItem product)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_CreateSOItemExtension");
            cmd.SetParameterValue("@SOSysNo", product["SOSysNo"]);
            cmd.SetParameterValue("@ProductSysNo", product.ProductSysNo);
            cmd.SetParameterValue("@ReferenceSysNo", product.SpecialActivitySysNo);
            cmd.SetParameterValue("@Type", product["ItemExtensionType"]);
            cmd.ExecuteNonQuery();
        }

        public static void CreateGroupBuyingTicket(OrderInfo order)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_CreateGroupBuyingTicket");
            cmd.SetParameterValue("@GroupBuyingSysNo", order.OrderItemGroupList[0].ProductItemList[0].SpecialActivitySysNo);
            cmd.SetParameterValue("@OrderSysNo", order.ID);
            cmd.SetParameterValue("@Tel", order.VirualGroupBuyOrderTel);
            cmd.SetParameterValue("@CustomerSysNo", order.Customer.SysNo);
            cmd.SetParameterValue("@PayType", order.PayTypeID);
            cmd.SetParameterValue("@LanguageCode", ConstValue.LanguageCode);
            cmd.SetParameterValue("@CompanyCode", ConstValue.CompanyCode);
            cmd.SetParameterValue("@StoreCompanyCode", ConstValue.StoreCompanyCode);
            cmd.ExecuteNonQuery();
        }

        public static VendorInfo GetVendorInfo(int vendorSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_GetVendorInfoBySysNo");
            cmd.SetParameterValue("@SysNo", vendorSysNo);
            return cmd.ExecuteEntity<VendorInfo>();
        }

        /// <summary>
        /// 订单创建实名认证信息
        /// </summary>
        /// <param name="order"></param>
        public static void CreateSOAuthentication(OrderInfo order)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_CreateSOAuthentication");
            cmd.SetParameterValue("@SOSysNo", order.ID);
            cmd.SetParameterValue("@CustomerSysNo", order.Customer.SysNo);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 用户下单频率检查
        /// </summary>
        /// <param name="customerSysno"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        internal static bool CheckCustomerOrderFrequency(int customerSysno, TimeSpan timeSpan)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_CheckCustomerOrderFrequency");
            cmd.SetParameterValue("@CustomerSysNo", customerSysno);
            cmd.SetParameterValue("@TimeSpan", timeSpan.Minutes);
            object result = cmd.ExecuteScalar();
            if (result == null || result is DBNull)
            {
                return true;
            }
            int count = (int)result;
            return count <= 0;
        }

        #endregion

        #region 商品
        /// <summary>
        /// 获取商品基本信息
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <returns></returns>
        public static ProductBasicInfo GetProductBasicInfoBySysNo(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_GetProductBasicInfoBySysNo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            return cmd.ExecuteEntity<ProductBasicInfo>();
        }
        /// <summary>
        /// 获取商品销售信息(库存以及销售价格)
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <returns></returns>
        public static ProductSalesInfo GetProductSalesInfoBySysNo(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_GetProductSalesInfoBySysNo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            return cmd.ExecuteEntity<ProductSalesInfo>();
        }
        /// <summary>
        /// 获取商品销售信息(库存以及销售价格)
        /// </summary>
        /// <param name="productSysNoList">商品编号列表</param>
        /// <returns></returns>
        public static List<ProductSalesInfo> GetProductSalesInfoBySysNoList(List<int> productSysNoList)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Pipeline_GetProductSalesInfoBySysNoList");
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(cmd))
            {
                builder.ConditionConstructor.AddInCondition(
                                                                QueryConditionRelationType.AND,
                                                                "prod.SysNo",
                                                                DbType.Int32,
                                                                productSysNoList);

                cmd.CommandText = builder.BuildQuerySql();
                return cmd.ExecuteEntityList<ProductSalesInfo>();
            }
        }

        /// <summary>
        /// 根据商品组编号获取商品分组属性列表
        /// </summary>
        /// <param name="productSysno">商品编号</param>
        /// <returns></returns>
        public static List<ProductSplitGroupProperty> GetProductSplitGroupPropertyList(int productSysno)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_GetProductSplitGroupPropertyList");
            cmd.SetParameterValue("@ProductSysno", productSysno);
            return cmd.ExecuteEntityList<ProductSplitGroupProperty>();
        }

        /// <summary>
        /// 取得商品进口信息
        /// </summary>
        /// <param name="productSysno">商品编号</param>
        /// <returns></returns>
        public static ProductEntryInfo GetProductEntryInfoBySysNo(int productSysno)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_GetProductEntryInfoBySysNo");
            cmd.SetParameterValue("@ProductSysNo", productSysno);
            return cmd.ExecuteEntity<ProductEntryInfo>();
        }

        /// <summary>
        /// 取得商品的入境报关设置
        /// </summary>
        /// <param name="allItemSysNoList"></param>
        public static List<ProductEntryInfo> GetProductEntryInfo(List<int> productSysNoList)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Pipeline_GetProductEntryInfoList");
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(cmd))
            {
                builder.ConditionConstructor.AddInCondition(
                                                                QueryConditionRelationType.AND,
                                                                "ProductSysNo",
                                                                DbType.Int32,
                                                                productSysNoList);

                cmd.CommandText = builder.BuildQuerySql();
                return cmd.ExecuteEntityList<ProductEntryInfo>();
            }
        }
        #endregion

        #region Inventory
        /// <summary>
        /// 分仓
        /// </summary>
        /// <param name="request"></param>
        public static WarehouseAllocateResponse AllocateWarehouse(WarehouseAllocateRequest request)
        {
            string xml = ECommerce.Utility.SerializationUtility.XmlSerialize(request);
            DataCommand command = DataCommandManager.GetDataCommand("Inventory_AllocateInventoryForSaleOrder");
            command.SetParameterValue("@WarehouseAllocateMsg", xml);

            WarehouseAllocateResponse response = new WarehouseAllocateResponse();
            response.AllocateItemInventoryInfoList = command.ExecuteEntityList<AllocatedItemInventoryInfo>();
            response.AllocateResult = Convert.ToInt32(command.GetParameterValue("ReturnValue"));
            return response;
        }

        internal static List<string> GetAllStockCountryCode()
        {
            DataCommand command = DataCommandManager.GetDataCommand("Inventory_GetAllStockCountryCode");
            return command.ExecuteFirstColumn<string>();

        }
        #endregion

        #region Payment

        internal static PayTypeInfo GetPayTypeBySysNo(string payTypeID)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_GetPayTypeBySysNo");
            cmd.SetParameterValue("@SysNo", Convert.ToInt32(payTypeID));
            return cmd.ExecuteEntity<PayTypeInfo>();
        }

        /// <summary>
        /// 根据货币编号取得货币汇率
        /// </summary>
        /// <param name="currencySysno">货币编号</param>
        /// <returns>货币汇率</returns>
        public static decimal GetCurrencyExchangeRate(int currencySysno)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_GetCurrencyExchangeRate");
            cmd.SetParameterValue("@CurrencySysno", currencySysno);
            object o = cmd.ExecuteScalar();
            if (o == null || o is DBNull)
            {
                return 0m;
            }
            else
            {
                decimal exchangeRate;
                decimal.TryParse(o.ToString(), out exchangeRate);
                return exchangeRate;
            }
        }

        #endregion

        #region Shipping

        public static Area GetAreaBySysNo(int areaID)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_GetAreaBySysNo");
            cmd.SetParameterValue("@SysNo", Convert.ToInt32(areaID));
            return cmd.ExecuteEntity<Area>();
        }

        /// <summary>
        /// 取得所有的配送方式
        /// </summary>
        /// <returns></returns>
        public static List<ShipTypeInfo> GetAllShippingTypeList()
        {
            List<ShipTypeInfo> allShippingTypeList = new List<ShipTypeInfo>();

            string cacheKey = "Pipeline_AllShippingTypeList";
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                allShippingTypeList = (List<ShipTypeInfo>)HttpRuntime.Cache[cacheKey];
            }

            if (allShippingTypeList == null || allShippingTypeList.Count <= 0)
            {
                DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_GetAllShippingTypeList");
                allShippingTypeList = cmd.ExecuteEntityList<ShipTypeInfo>();
            }

            if (allShippingTypeList != null && allShippingTypeList.Count > 0)
            {
                var cacheShipTypeList = ECommerce.Utility.SerializationUtility.DeepClone(allShippingTypeList);
                HttpRuntime.Cache.Insert(cacheKey, cacheShipTypeList, null, DateTime.Now.AddSeconds(CacheTime.Middle), Cache.NoSlidingExpiration);
            }

            return allShippingTypeList;
        }

        /// <summary>
        /// 根据配送地区编号和支付方式编号取得所有支持的配送方式列表
        /// </summary>
        /// <param name="addressAreaID">配送地区编号</param>
        /// <param name="payTypeID">支付方式编号</param>
        /// <returns></returns>
        public static List<ShipTypeInfo> GetSupportedShipTypeList(int addressAreaID, PaymentCategory? paymentCategory)
        {
            Area currentAreaInfo = PipelineDA.GetAreaBySysNo(addressAreaID);
            if (currentAreaInfo == null)
            {
                return new List<ShipTypeInfo>(0);
            }
            List<ShipTypeInfo> allShippingTypeList = PipelineDA.GetAllShippingTypeList();
            List<ShipTypeAndAreaUnInfo> allShippingTypeAndAreaUnList = PipelineDA.GetAllShippingTypeAndAreaUnList();

            List<ShipTypeAndAreaUnInfo> shippingAndAreaUnList = new List<ShipTypeAndAreaUnInfo>();

            //step1  移除不支持送达地区的配送方式
            if (allShippingTypeAndAreaUnList != null && allShippingTypeAndAreaUnList.Count > 0)
            {
                allShippingTypeAndAreaUnList.ForEach(unInfo =>
                {
                    if (unInfo.AreaID == currentAreaInfo.SysNo)
                    {
                        allShippingTypeList.RemoveAll(shipType => shipType.ShipTypeSysNo == unInfo.ShippingTypeID);
                    }
                    if (unInfo.AreaID == currentAreaInfo.CitySysNo)
                    {
                        allShippingTypeList.RemoveAll(shipType => shipType.ShipTypeSysNo == unInfo.ShippingTypeID);
                    }
                    if (unInfo.AreaID == currentAreaInfo.ProvinceSysNo)
                    {
                        allShippingTypeList.RemoveAll(shipType => shipType.ShipTypeSysNo == unInfo.ShippingTypeID);
                    }
                });
            }
            //step2 移除不支持货到付款的配送方式
            if (allShippingTypeList.Count > 0 && paymentCategory == PaymentCategory.PayWhenRecv)
            {
                allShippingTypeList.RemoveAll(shipType => !shipType.IsPayWhenRecv);
            }

            return allShippingTypeList;
        }

        /// <summary>
        /// 取得配送方式-地区-非的配置
        /// </summary>
        /// <returns></returns>
        public static List<ShipTypeAndAreaUnInfo> GetAllShippingTypeAndAreaUnList()
        {
            List<ShipTypeAndAreaUnInfo> allShippingTypeAndAreaUnList = new List<ShipTypeAndAreaUnInfo>();

            string cacheKey = "Pipeline_AllShippingTypeAndAreaUnList";
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                allShippingTypeAndAreaUnList = (List<ShipTypeAndAreaUnInfo>)HttpRuntime.Cache[cacheKey];
            }

            if (allShippingTypeAndAreaUnList == null || allShippingTypeAndAreaUnList.Count <= 0)
            {
                DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_GetShippingTypeAndAreaUnList");
                allShippingTypeAndAreaUnList = cmd.ExecuteEntityList<ShipTypeAndAreaUnInfo>();
            }

            if (allShippingTypeAndAreaUnList != null && allShippingTypeAndAreaUnList.Count > 0)
            {
                HttpRuntime.Cache.Insert(cacheKey, allShippingTypeAndAreaUnList, null, DateTime.Now.AddSeconds(CacheTime.Middle), Cache.NoSlidingExpiration);
            }

            return allShippingTypeAndAreaUnList;
        }

        /// <summary>
        /// 取得配送方式-支付方式-非的配置
        /// </summary>
        /// <returns></returns>
        public static List<ShipTypeAndPayTypeUnInfo> GetAllShipTypeAndPayTypeUnInfo()
        {
            List<ShipTypeAndPayTypeUnInfo> allPayTypeAndShipTypeUnList = new List<ShipTypeAndPayTypeUnInfo>();

            string cacheKey = "Pipeline_AllShipTypeAndPayTypeUnList";
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                allPayTypeAndShipTypeUnList = (List<ShipTypeAndPayTypeUnInfo>)HttpRuntime.Cache[cacheKey];
            }

            if (allPayTypeAndShipTypeUnList == null || allPayTypeAndShipTypeUnList.Count <= 0)
            {
                DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_GetShipTypeAndPayTypeUnList");
                allPayTypeAndShipTypeUnList = cmd.ExecuteEntityList<ShipTypeAndPayTypeUnInfo>();
            }

            if (allPayTypeAndShipTypeUnList != null && allPayTypeAndShipTypeUnList.Count > 0)
            {
                HttpRuntime.Cache.Insert(cacheKey, allPayTypeAndShipTypeUnList, null, DateTime.Now.AddSeconds(CacheTime.Middle), Cache.NoSlidingExpiration);
            }

            return allPayTypeAndShipTypeUnList;
        }

        public static List<ShippingInfo> GetAllShippingFee(List<ShippingFeeQueryInfo> qryList)
        {
            string xml = ECommerce.Utility.SerializationUtility.XmlSerialize(qryList);
            DataCommand command = DataCommandManager.GetDataCommand("Shipping_GetAllShippingFee");
            command.SetParameterValue("@ReqMsg", xml);
            return command.ExecuteEntityList<ShippingInfo>();
        }

        public static void SetCustomerShippingAddressAsDefault(int contactAddressSysNo, int customerSysNo)
        {
            DataCommand dataCommand = DataCommandManager.GetDataCommand("Shipping_SetShippingAddressAsDefault");
            dataCommand.SetParameterValue("@SysNo", contactAddressSysNo);
            dataCommand.SetParameterValue("@CustomerSysNo", customerSysNo);
            dataCommand.ExecuteNonQuery();
        }

        public static List<FreeShippingItemConfig> GetFreeShippingConfig()
        {
            List<FreeShippingItemConfig> list = new List<FreeShippingItemConfig>();
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_GetFreeShippingConfig");

            DataSet ds = cmd.ExecuteDataSet();
            if (ds != null)
            {
                list = DataMapper.GetEntityList<FreeShippingItemConfig, List<FreeShippingItemConfig>>(ds.Tables[0].Rows);
                foreach (var item in list)
                {
                    item.ProductSettingValue = ds.Tables[1].Rows.Cast<DataRow>()
                        .Where(row => ((int)row["RuleSysNo"]) == item.SysNo)
                        .Select(row => (int)row["ProductSysNo"])
                        .ToList();
                }
            }
            return list;
        }

        /// <summary>
        /// 获取商家对应仓库的配送方式
        /// </summary>
        /// <param name="sellerSysNo">商家编号</param>
        /// <param name="stockSysNo">仓库编号</param>
        /// <returns></returns>
        public static ShipTypeInfo Pipeline_GetMerchantStockShippingType(int sellerSysNo, int stockSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_GetMerchantStockShippingType");
            cmd.SetParameterValue("@SellerSysNo", sellerSysNo);
            cmd.SetParameterValue("@StockSysNo", stockSysNo);
            return cmd.ExecuteEntity<ShipTypeInfo>();
        }
        /// <summary>
        /// 获取商家仓库的所有配送方式
        /// </summary>
        /// <param name="sellerSysNo">商家编号</param>
        /// <returns></returns>
        public static List<ShipTypeInfo> Checkout_GetStockShippingType(int sellerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Checkout_GetStockShippingType");
            cmd.SetParameterValue("@SellerSysNo", sellerSysNo);
            return cmd.ExecuteEntityList<ShipTypeInfo>();
        }

        /// <summary>
        /// 是否存在运费设置
        /// </summary>
        /// <param name="shipTypeSysNo">配送方式</param>
        /// <param name="areaSysNo">收货地区</param>
        /// <returns></returns>
        public static bool Pipeline_ExistsShipTypeAreaPrice(int shipTypeSysNo, int areaSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_ExistsShipTypeAreaPrice");
            cmd.SetParameterValue("@ShipTypeSysNo", shipTypeSysNo);
            cmd.SetParameterValue("@AreaSysNo", areaSysNo);
            DataTable dt = cmd.ExecuteDataTable();
            if (dt != null && dt.Rows.Count > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 根据收货市ID获得所有的可以配送到该市的商品ProductSysNo
        /// </summary>
        /// <param name="areaSysNo">收货地区</param>
        /// <returns></returns>
        public static List<ProductShippingPrice> Pipeline_GetAllProductRestrictedAreaByAreaSysNo(int areaSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_GetAllProductRestrictedAreaByAreaSysNo");
            cmd.SetParameterValue("@AreaSysNo", areaSysNo);
            List<ProductShippingPrice> result = cmd.ExecuteEntityList<ProductShippingPrice>();
            return result;
        }


        #endregion

        #region Customer

        public static bool IsExistCustomer(int customerSysno)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Shipping_IsExistCustomer");
            command.SetParameterValue("@CustomerSysNo", customerSysno);
            object obj = command.ExecuteScalar();
            if (obj == null || obj is DBNull)
            {
                return false;
            }
            return ((int)obj) > 0;
        }
        #endregion

        #region Advertisement Effect Monitor

        /// <summary>
        /// 保存广告活动跟踪数据
        /// </summary>
        /// <param name="info">广告活动跟踪数据</param>
        public static void CreateAdvEffectMonitor(AdvEffectMonitorInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_CreateAdvEffectMonitor");
            cmd.SetParameterValue("@CMP", info.CMP);
            cmd.SetParameterValue("@OperationType", info.OperationType);
            cmd.SetParameterValue("@CustomerSysNo", info.CustomerSysNo);
            cmd.SetParameterValue("@ReferenceSysNo", info.ReferenceSysNo);
            cmd.SetParameterValue("@CompanyCode", info.CompanyCode);
            cmd.SetParameterValue("@LanguageCode", info.LanguageCode);
            cmd.SetParameterValue("@StoreCompanyCode", info.StoreCompanyCode);
            cmd.SetParameterValue("@CMP1", info.CMP1);
            cmd.SetParameterValue("@CMP2", info.CMP2);
            cmd.SetParameterValue("@CMP3", info.CMP3);
            cmd.SetParameterValue("@CMP4", info.CMP4);
            cmd.ExecuteNonQuery();
        }

        #endregion

        #region GiftCard

        internal static List<GiftCardInfo> QueryGiftCardInfoList(List<string> giftCardCodeList)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Pipeline_QueryGiftCardInfoList");
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(cmd))
            {
                builder.ConditionConstructor.AddInCondition(
                                                                QueryConditionRelationType.AND,
                                                                "Code",
                                                                DbType.String,
                                                                giftCardCodeList);

                cmd.CommandText = builder.BuildQuerySql();
                return cmd.ExecuteEntityList<GiftCardInfo>();
            }
        }

        internal static List<GiftCardInfo> GetCustomerBindingGiftCardInfoList(int customerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_GetCustomerBindingGiftCardInfoList");
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);

            return cmd.ExecuteEntityList<GiftCardInfo>();
        }

        internal static void UpdateGiftCardInfo(GiftCardInfo giftCardInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_UpdateGiftCardInfo");
            cmd.SetParameterValue("@SysNo", giftCardInfo.SysNo);
            cmd.SetParameterValue("@UseAmount", giftCardInfo.UseAmount);
            cmd.ExecuteNonQuery();
        }

        internal static void CreateGiftCardRedeemLog(GiftCardInfo giftCardInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_CreateGiftCardRedeemLog");
            cmd.SetParameterValue("@Code", giftCardInfo.Code);
            cmd.SetParameterValue("@CustomerSysNo", giftCardInfo["CustomerSysNo"]);
            cmd.SetParameterValue("@ConsumeAmount", giftCardInfo.UseAmount);
            cmd.SetParameterValue("@ReferenceSOSysNo", giftCardInfo["SOSysNo"]);
            cmd.SetParameterValue("@CurrencySysNo", giftCardInfo["CurrencySysNo"]);
            cmd.ExecuteNonQuery();
        }

        internal static void CreateGiftCardOperateLog(string msg)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Pipeline_CreateGiftCardOperateLog");
            cmd.SetParameterValue("@Msg", msg);
            cmd.ExecuteNonQuery();
        }

        #endregion

        #region 仓库
        internal static List<SimpleStockInfo> QueryStockInfoList(List<int> sysNoList)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Pipeline_QueryStockInfoList");
            using (DynamicQuerySqlBuilder builder = new DynamicQuerySqlBuilder(cmd))
            {
                builder.ConditionConstructor.AddCondition(QueryConditionRelationType.AND, "[Status]", DbType.Int32, "@Status", QueryConditionOperatorType.Equal, 0);
                builder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND, "[SysNo]", DbType.Int32, sysNoList);
                cmd.CommandText = builder.BuildQuerySql();
                return cmd.ExecuteEntityList<SimpleStockInfo>();
            }
        }
        #endregion
    }
}
