using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.MKT.BizProcessor
{
    /// <summary>
    /// 调用外部接口的内部通用类。将MKT中调用外部接口的地方都集中到这里。
    /// </summary>
    public static class ExternalDomainBroker
    {
        #region Common

        /// <summary>
        /// 创建操作日志，提供统一的日志记录服务
        /// </summary>
        /// <param name="logInfo"></param>
        /// <returns></returns>
        public static int CreateOperationLog(string note, BizLogType logType, int ticketSysNo, string companyCode)
        {
            return ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(note, logType, ticketSysNo, companyCode);
        }

        public static UserInfo GetUserInfoBySysNo(int sysNo)
        {
            return ObjectFactory<ICommonBizInteract>.Instance.GetUserInfoBySysNo(sysNo);
        }

        public static string GetUserFullName(string userID, bool isSysNo)
        {
            return ObjectFactory<ICommonBizInteract>.Instance.GetUserFullName(userID, isSysNo);
        }

        public static CategorySetting GetCategorySetting(int categorySysNo)
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetCategorySetting(categorySysNo);
        }

        public static int GetInStockDaysByProductSysNo(int productSysNo)
        {
            return ObjectFactory<IInventoryBizInteract>.Instance.GetInStockDaysByProductSysNo(productSysNo);
        }

        public static void UpdateSystemConfigurationValue(string key, string value, string companyCode)
        {
            ObjectFactory<ICommonBizInteract>.Instance.UpdateSystemConfigurationValue(
                key,value,companyCode);
        }


        #endregion

        #region 客户
        /// <summary>
        /// 根据客户编号取得客户信息
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <returns></returns>
        public static ECCentral.BizEntity.Customer.CustomerInfo GetCustomerInfo(int customerSysNo)
        {
            return ObjectFactory<ECCentral.Service.IBizInteract.ICustomerBizInteract>.Instance.GetCustomerInfo(customerSysNo);
        }
        #endregion

        #region 订单
        /// <summary>
        /// 取得订单主要信息
        /// </summary>
        /// <param name="mailInfoMessage"></param>
        /// <param name="isAsync"></param>
        /// <param name="isInternalMail"></param>
        public static ECCentral.BizEntity.SO.SOBaseInfo GetSOInfo(int sysNo)
        {
            return ObjectFactory<ECCentral.Service.IBizInteract.ISOBizInteract>.Instance.GetSOBaseInfo(sysNo);
        }

        /// <summary>
        /// 查询15分钟前待审核的秒杀订单
        /// </summary>
        /// <returns></returns>
        public static List<ECCentral.BizEntity.SO.SOInfo> GetSOStatus()
        {
            return ObjectFactory<ECCentral.Service.IBizInteract.ISOBizInteract>.Instance.GetSOStatus();
        }

        /// <summary>
        /// 更新订单状态
        /// </summary>
        /// <param name="soSysNo"></param>
        public static void MakeOpered(int soSysNo)
        {
            ObjectFactory<ECCentral.Service.IBizInteract.ISOBizInteract>.Instance.MakeOpered(soSysNo);
        }

        #endregion

        #region 库存
        public static List<ProductInventoryInfo> GetProductInventoryInfo(int productSysNo)
        {
            return ObjectFactory<IInventoryBizInteract>.Instance.GetProductInventoryInfo(productSysNo);
        }

        public static List<ProductInventoryInfo> GetProductInventoryInfoByProductSysNoList(List<int> productSysNoList)
        {
            return ObjectFactory<IInventoryBizInteract>.Instance.GetProductTotalInventoryInfoByProductList(productSysNoList);
        }

        #endregion

        #region IM

        public static ProductInfo GetProductInfo(string productID)
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetProductInfo(productID);
        }

        public static ProductInfo GetProductInfo(int productSysNo)
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetProductInfo(productSysNo);
        }

        public static ProductInfo GetSimpleProductInfo(int productSysNo)
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetSimpleProductInfo(productSysNo);
        }

        public static List<ProductInfo> GetProductInfoListByProductSysNoList(List<int> productSysNoList)
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetProductInfoListByProductSysNoList(productSysNoList);
        }

        public static ProductInventoryInfo GetProductTotalInventoryInfo(int productSysNo)
        {
            return ObjectFactory<IInventoryBizInteract>.Instance.GetProductTotalInventoryInfo(productSysNo);
        }

        public static List<ProductInfo> GetProductsInSameGroupWithProductSysNo(int productSysNo)
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetProductsInSameGroupWithProductSysNo(productSysNo).ProductList;
        }

        public static CategoryInfo GetCategory3Info(int sysNo)
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetCategory3Info(sysNo);
        }

        /// <summary>
        /// 判断一个商品是否是附件
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static bool CheckIsAttachment(int productSysNo)
        {
            return ObjectFactory<IIMBizInteract>.Instance.CheckIsAttachment(productSysNo);
        }

        /// <summary>
        ///  提交CS处理
        /// </summary>
        /// <param name="item"></param>
        public static void AddComplain(ECCentral.BizEntity.SO.SOComplaintCotentInfo item)
        {
            ObjectFactory<ISOBizInteract>.Instance.AddComplain(item);
        }
        /// <summary>
        /// 获取商品的最后采购时间
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DateTime? GetLastPoDate(int id)
        {
            return ObjectFactory<IPOBizInteract>.Instance.GetLastPoDate(id);
        }
        /// <summary>
        /// 获取商品近期销售情况
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ProductSalesTrendInfo GetProductSalesTrendInfoTotal(int id)
        {
            return ObjectFactory<IInventoryBizInteract>.Instance.GetProductSalesTrendInfoTotal(id);
        }
        /// <summary>
        /// 获取积分和钱的兑换比例
        /// </summary>
        /// <returns></returns>
        public static decimal GetPointExChangeRate()
        {
            return ObjectFactory<ICustomerBizInteract>.Instance.GetPointToMoneyRatio();
        }
        /// <summary>
        /// 更新商品的PromotionType
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="promotionType"></param>
        public static void UpdateProductPromotionType(int productSysNo, string promotionType)
        {
            ObjectFactory<IIMBizInteract>.Instance.UpdateProductPromotionType(productSysNo, promotionType);
        }
        /// <summary>
        /// 作废禁设虚库记录
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="status"></param>
        /// <param name="note"></param>
        public static void UpdateProductNotAutoSetVirtual(int productSysNo, int status, string note)
        {
            ObjectFactory<IIMBizInteract>.Instance.UpdateProductNotAutoSetVirtual(productSysNo, status, note);
        }
        /// <summary>
        /// 回滚价格
        /// </summary>
        /// <param name="oldEntity"></param>
        public static void RollBackPriceWhenCountdownInterrupted(BizEntity.MKT.CountdownInfo oldEntity)
        {
            ObjectFactory<IIMBizInteract>.Instance.RollBackPriceWhenCountdownInterrupted(oldEntity);
        }
        /// <summary>
        /// 获取单个商品在某个渠道上的库存
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="stockSysNo"></param>
        /// <returns></returns>
        public static ProductInventoryInfo GetProductInventoryInfoByStock(int productSysNo, int stockSysNo)
        {
            return ObjectFactory<IInventoryBizInteract>.Instance.GetProductInventoryInfoByStock(productSysNo, stockSysNo);
        }
        /// <summary>
        /// 设置指定渠道上指定商品的预留库存
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="stockSysNo"></param>
        /// <param name="qty"></param>
        public static void SetProductReservedQty(int productSysNo, int stockSysNo, int qty)
        {
            ObjectFactory<IInventoryBizInteract>.Instance.SetProductReservedQty(productSysNo, stockSysNo, qty);
        }

        /// <summary>
        /// 修改产品扩展信息
        /// </summary>
        /// <param name="SysNo"></param>
        /// <param name="Keywords"></param>
        /// <param name="Keywords0"></param>
        /// <param name="EditUser"></param>
        /// <param name="CompanyCode"></param>
        /// <returns></returns>
        public static void UpdateProductExKeyKeywords(int sysNo, string keywords, string keywords0
            , int editUserSysNo, string companyCode)
        {
            ObjectFactory<IIMBizInteract>.Instance.UpdateProductExKeyKeywords(
                        sysNo, keywords, keywords0, editUserSysNo, companyCode);
        }

        /// <summary>
        /// 获取商品所在商品组的其它商品ID
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static List<string> GetProductGroupIDSFromProductID(string productID)
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetProductGroupIDSFromProductID(productID);
        }

         /// <summary>
        /// 获取商品的组系统编号
        /// </summary>
        /// <param name="productSysNo">商品系统编号</param>
        /// <returns></returns>
        public static int GetProductGroupSysNo(int productSysNo)
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetProductGroupSysNo(productSysNo);
        }

        #endregion

        #region 品牌

        public static BrandInfo GetBrandInfoBySysNo(int sysNo)
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetBrandInfoBySysNo(sysNo);
        }

        /// <summary>
        /// 获取所有品牌列表
        /// </summary>
        /// <returns>所有品牌列表</returns>
        public static List<BrandInfo> GetBrandList()
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetBrandInfoList();
        }

        /// <summary>
        /// 获取所有生产商
        /// </summary>
        /// <returns>所有生产商表</returns>
        public static List<ManufacturerInfo> GetManufacturerList(string companyCode)
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetManufacturerList(companyCode);
        }

        #endregion

        #region PO
        public static BizEntity.PO.VendorBasicInfo GetVendorBasicInfoBySysNo(int vendorSysNo)
        {
            return ObjectFactory<IPOBizInteract>.Instance.GetVendorBasicInfoBySysNo(vendorSysNo);
        }

        public static VendorInfo GetVendorInfoBySysNo(int vendorSysNo)
        {
            return ObjectFactory<IPOBizInteract>.Instance.GetVendorInfoSysNo(vendorSysNo);
        }

        #endregion

        public static PayItemInfo CreatePayItem(PayItemInfo payItem)
        {
            return ObjectFactory<IInvoiceBizInteract>.Instance.CreatePayItem(payItem);
        }

        internal static SOIncomeInfo CreateSOIncome(SOIncomeInfo entity)
        {
            return ObjectFactory<IInvoiceBizInteract>.Instance.CreateSOIncome(entity);
        }

        /// <summary>
        /// 获取所有有效的C1
        /// </summary>
        /// <returns></returns>
        public static List<CategoryInfo> GetCategory1List()
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetCategory1List();
        }
    }
}
