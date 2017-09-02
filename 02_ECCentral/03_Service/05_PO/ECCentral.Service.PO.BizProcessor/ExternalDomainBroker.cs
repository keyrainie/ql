using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Utility;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.RMA;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.PO.BizProcessor
{
    /// <summary>
    /// 调用外部接口的内部通用类。将PO中调用外部接口的地方都集中到这里。
    /// </summary>
    public static class ExternalDomainBroker
    {
        static ExternalDomainBroker()
        {

        }

        /// <summary>
        /// 异步发送外部邮件
        /// </summary>
        /// <param name="mailInfoMessage"></param>
        public static void SendExternalEmail(string toAddress, string templateID, KeyValueVariables keyValueVariables, KeyTableVariables keyTableVariables, string languageCode)
        {
            try
            {
                ECCentral.Service.Utility.EmailHelper.SendEmailByTemplate(toAddress, templateID, keyValueVariables, keyTableVariables, languageCode);
            }
            catch (Exception ex)
            {
                ExceptionHelper.HandleException(ex);
            }
        }

        public static void SendExternalEmail(string toAddress, string templateID, KeyValueVariables keyValueVariables, string languageCode)
        {
            SendExternalEmail(toAddress, templateID, keyValueVariables, null, languageCode);
        }

        #region [IM]
        /// <summary>
        /// 获取3级分类信息
        /// </summary>
        /// <param name="c3SysNo"></param>
        /// <returns></returns>
        public static CategoryInfo GetCategory3Info(int c3SysNo)
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetCategory3Info(c3SysNo);
        }

        /// <summary>
        /// 获取2级分类信息
        /// </summary>
        /// <param name="c2SysNo"></param>
        /// <returns></returns>
        public static CategoryInfo GetCategory2Info(int c2SysNo)
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetCategory2Info(c2SysNo);
        }

        /// <summary>
        /// 根据产品编号获取产品信息
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static ProductInfo GetProductInfoBySysNo(int productSysNo)
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetProductInfo(productSysNo);
        }

        /// <summary>
        /// 批量获取产品信息
        /// </summary>
        /// <param name="productSysNoList"></param>
        /// <returns></returns>
        public static List<ProductInfo> GetProductList(List<int> productSysNoList)
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetProductInfoListByProductSysNoList(productSysNoList);
        }

        /// <summary>
        /// 根据编号,获取PMList
        /// </summary>
        /// <param name="pmSysNo"></param>
        /// <returns></returns>
        public static ProductManagerGroupInfo GetPMList(int pmSysNo)
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetPMListByUserSysNo(pmSysNo);
        }
        #endregion

        #region [Inventory]
        /// <summary>
        /// 调整商品库存
        /// </summary>
        /// <param name="adjustInfo"></param>
        public static void AdjustProductInventory(InventoryAdjustContractInfo adjustInfo)
        {
            ObjectFactory<IInventoryBizInteract>.Instance.AdjustProductInventory(adjustInfo);
        }


        /// <summary>
        /// 根据仓库编号获取仓库信息 Ray.L.Xing  泰隆优选定制化 ：泰隆优选不存在多渠道现在用Warehouse 实体存储仓库信息
        /// </summary>
        /// <param name="warehouseNo">泰隆优选仓库编号</param>
        /// <returns></returns>
        public static WarehouseInfo GetWarehouseInfo(int stockSysNo)
        {
             return ObjectFactory<IInventoryBizInteract>.Instance.GetWarehouseInfoBySysNo(stockSysNo);
        }

        /// <summary>
        /// 获取仓库列表   Ray.L.Xing  泰隆优选定制化 ：泰隆优选不存在多渠道 现在用Warehouse 实体存储仓库信息
        /// </summary>
        /// <returns></returns>
        public static List<WarehouseInfo> GetWarehouseList(string companyCode)
        {
            return ObjectFactory<IInventoryBizInteract>.Instance.GetWarehouseList(companyCode);
        }

        #region 注销以下代码（泰隆优选不存在多渠道 ）

    
        ///// <summary>
        ///// 获取仓库信息
        ///// </summary>
        ///// <param name="stockSysNo"></param>
        ///// <returns></returns>
        //public static StockInfo GetStockInfo(int stockSysNo)
        //{
        //    return ObjectFactory<IInventoryBizInteract>.Instance.GetStockInfo(stockSysNo);
        //}

        ///// <summary>
        ///// 获取渠道仓库列表
        ///// </summary>
        ///// <returns></returns>
        //public static List<StockInfo> QueryStockAll()
        //{
        //    return ObjectFactory<IInventoryBizInteract>.Instance.GetStockList("1");
        //}

        #endregion
        /// <summary>
        /// 根据商品编号获取相关的库存信息
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static List<ProductInventoryInfo> GetInventoryInfo(int productSysNo)
        {
            return ObjectFactory<IInventoryBizInteract>.Instance.GetProductInventoryInfo(productSysNo);
        }

        /// <summary>
        /// 获取商品销售信息
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static List<ProductSalesTrendInfo> GetProductSalesTrendInfo(int productSysNo)
        {
            return ObjectFactory<IInventoryBizInteract>.Instance.GetProductSalesTrendInfo(productSysNo);
        }
        

        /// <summary>
        ///  PO - 判断 对应的自动移仓单已出库
        /// </summary>
        /// <param name="shiftid"></param>
        /// <param name="IsCanAbandon"></param>
        /// <param name="shiftSysNo"></param>
        /// <returns></returns>
        public static bool IsAutoShift(int shiftid, out int IsCanAbandon, out int shiftSysNo)
        {
            return ObjectFactory<IInventoryBizInteract>.Instance.IsAutoShift(shiftid, out IsCanAbandon, out  shiftSysNo);
        }

        /// <summary>
        /// PO  - 根据 shiftSysNo 设置 St_Shift 作废
        /// </summary>
        /// <param name="masterSysNo"></param>
        public static void AbandonShift(int masterSysNo)
        {
            ObjectFactory<IInventoryBizInteract>.Instance.AbandonShift(masterSysNo);
        }

        public static void AdjustBatchNumberInventory(string xml)
        {
            ObjectFactory<IInventoryBizInteract>.Instance.AdjustBatchNumberInventory(xml);
        }

        public static void UpdateCostInWhenCostChange(CostChangeInfo costChangeInfo)
        {
            ObjectFactory<IInventoryBizInteract>.Instance.UpdateCostInWhenCostChange(costChangeInfo);
        }
        #endregion

        #region [Invoice]

        /// <summary>
        /// 创建应付款单
        /// </summary>
        /// <param name="payItem"></param>
        /// <returns></returns>
        public static PayItemInfo CreatePayItem(PayItemInfo payItem)
        {
            return ObjectFactory<IInvoiceBizInteract>.Instance.CreatePayItem(payItem);
        }

        /// <summary>
        /// 创建应付款
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static PayableInfo CreatePayable(PayableInfo info)
        {
            return ObjectFactory<IInvoiceBizInteract>.Instance.CreatePayable(info);
        }


        /// <summary>
        /// 批量创建付款单
        /// </summary>
        /// <param name="payItemInfoList"></param>
        public static void BatchCreatePayItem(List<PayItemInfo> payItemInfoList)
        {
            ObjectFactory<IInvoiceBizInteract>.Instance.BatchCreatePayItem(payItemInfoList);
        }

        /// <summary>
        /// 检查付款单是否作废
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public static bool IsAbandonGatherPayItem(int sysNo)
        {
            return ObjectFactory<IInvoiceBizInteract>.Instance.IsAbandonGatherPayItem(sysNo);
        }

        /// <summary>
        ///  PO单终止入库时,创建NewPay
        /// </summary>
        /// <param name="poSysNo"></param>
        /// <param name="batchNo"></param>
        /// <param name="orderStatus"></param>
        /// <param name="orderType"></param>
        /// <param name="inStockAmt"></param>
        /// <param name="companyCode"></param>
        public static void CreatePayByVendor(int poSysNo, int batchNo, int orderStatus, PayableOrderType orderType, decimal? inStockAmt, string companyCode)
        {
            ObjectFactory<IInvoiceBizInteract>.Instance.CreatePayByVendor(poSysNo, batchNo, orderStatus, orderType, inStockAmt, companyCode);
        }

        /// <summary>
        ///  获取PO单已经存在已付款的预付款记录状态
        /// </summary>
        /// <param name="poSysNo"></param>
        /// <returns></returns>
        public static PayItemStatus? GetPOPrePayItemStatus(int poSysNo)
        {
            return ObjectFactory<IInvoiceBizInteract>.Instance.GetPOPrePayItemStatus(poSysNo);
        }

        /// <summary>
        /// 获取负PO的供应商财务应付款
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        public static decimal GetVendorPayBalanceByVendorSysNo(int vendorSysNo)
        {
            return ObjectFactory<IInvoiceBizInteract>.Instance.GetVendorPayBalanceByVendorSysNo(vendorSysNo);
        }

        /// <summary>
        /// 锁定或取消锁定供应商对应的付款单,并返回对付款单处理成功的记录数
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <param name="isLock"></param>
        /// <returns></returns>
        public static int LockOrUnlockPayItemByVendor(int vendorSysNo, bool isLock)
        {
            return ObjectFactory<IInvoiceBizInteract>.Instance.LockOrUnlockPayItemByVendor(vendorSysNo, isLock);
        }

        /// <summary>
        ///  锁定或取消锁定供应商PM对应的付款单,并返回对付款单处理成功的记录数
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <param name="isLock"></param>
        /// <param name="holdPMSysNoList"></param>
        /// <param name="UnHoldPMSysNoList"></param>
        /// <returns></returns>
        public static List<int> LockOrUnlockPayItemByVendorPM(int vendorSysNo, bool isLock, List<int> holdPMSysNoList, List<int> UnHoldPMSysNoList)
        {
            return ObjectFactory<IInvoiceBizInteract>.Instance.LockOrUnlockPayItemByVendorPM(vendorSysNo, isLock, holdPMSysNoList, UnHoldPMSysNoList);
        }
        #endregion

        #region [Common]
        /// <summary>
        /// 写日志记录
        /// </summary>
        /// <param name="note"></param>
        /// <param name="logType"></param>
        /// <param name="ticketSysNo"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static int CreateLog(string note, BizLogType logType, int ticketSysNo, string companyCode)
        {
            return ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(note
               , logType
               , ticketSysNo
               , companyCode);
        }

        /// <summary>
        /// 根据用户SysNo获取名称
        /// </summary>
        /// <param name="userSysNo"></param>
        /// <returns></returns>
        public static string GetUserNameByUserSysNo(int userSysNo)
        {
            return ObjectFactory<ICommonBizInteract>.Instance.GetUserFullName(userSysNo.ToString(), true);
        }

        /// <summary>
        /// 根据货币Code获取汇率
        /// </summary>
        /// <param name="currencyCode"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static decimal GetExchangeRateBySysNo(int currencyCode, string companyCode)
        {
            return ObjectFactory<ICommonBizInteract>.Instance.GetExchangeRateBySysNo(currencyCode, companyCode);
        }

        /// <summary>
        /// 获取支付类型
        /// </summary>
        /// <param name="payTypeSysNo"></param>
        /// <returns></returns>
        public static PayType GetPayTypeBySysNo(int payTypeSysNo)
        {
            return ObjectFactory<ICommonBizInteract>.Instance.GetPayType(payTypeSysNo);
        }

        /// <summary>
        /// 获取系统配置表信息
        /// </summary>
        /// <param name="key"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static string GetSystemConfiguration(string key, string companyCode)
        {
            return ObjectFactory<ICommonBizInteract>.Instance.GetSystemConfigurationValue(key, companyCode);
        }

        /// <summary>
        /// 获取区域信息
        /// </summary>
        /// <param name="areaSysNo"></param>
        /// <returns></returns>
        public static AreaInfo GetAreaInfo(int areaSysNo)
        {
            return ObjectFactory<ICommonBizInteract>.Instance.GetAreaInfo(areaSysNo);
        }

        /// <summary>
        /// 根据传入的ProductSysno,PMSysNo 检测是否匹配
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <param name="pmSysNo"></param>
        /// <returns></returns>
        public static bool CheckOperateRightForCurrentUser(int productSysNo, int pmSysNo)
        {
            return ObjectFactory<ICommonBizInteract>.Instance.CheckOperateRightForCurrentUser(productSysNo, pmSysNo);
        }

        /// <summary>
        /// 根据商品sysNo获取每个商品的产品线和所属PM
        /// </summary>
        /// <param name="productLineSysNo"></param>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public static List<ProductPMLine> GetProductLineSysNoByProductList(int [] productSysNo)
        {
            return ObjectFactory<ICommonBizInteract>.Instance.GetProductLineSysNoByProductList(productSysNo);
        }
        /// <summary>
        /// 根据PM，获取其全部产品线
        /// </summary>
        /// <param name="pmSysNo"></param>
        /// <returns></returns>
        public static List<ProductPMLine> GetProductLineInfoByPM(int pmSysNo)
        {
            return ObjectFactory<ICommonBizInteract>.Instance.GetProductLineInfoByPM(pmSysNo);
        }
        #endregion

        #region [RMA]
        /// <summary>
        /// 根据SysNo获取相关的RMA Register信息
        /// </summary>
        /// <param name="registerSysNo"></param>
        /// <returns></returns>
        public static List<RMARegisterInfo> GetRMARegisterList(List<int> registerSysNo)
        {
            return ObjectFactory<IRMABizInteract>.Instance.GetRMARegisterList(registerSysNo);
        }

        /// <summary>
        /// 根据单件号获取接收仓库
        /// </summary>
        /// <param name="registerSysNo"></param>
        /// <returns></returns>
        public static string[] GetReceiveWarehouseByRegisterSysNo(int registerSysNo)
        {
            return ObjectFactory<IRMABizInteract>.Instance.GetReceiveWarehouseByRegisterSysNo(registerSysNo);
        }

        /// <summary>
        /// 通过多个单件号获取相关的送修单SysNo
        /// </summary>
        /// <param name="registerSysNo"></param>
        /// <returns></returns>
        public static List<int> GetOutBoundSysNoListByRegisterSysNo(string registerSysNoList)
        {
            return ObjectFactory<IRMABizInteract>.Instance.GetOutBoundSysNoListByRegisterSysNo(registerSysNoList);
        }

        /// <summary>
        /// 关闭单件(供应商退款)
        /// </summary>
        /// <param name="registerSysNoList"></param>
        public static void BatchCloseRegisterForVendorRefund(List<int> registerSysNoList)
        {
            ObjectFactory<IRMABizInteract>.Instance.BatchCloseRegisterForVendorRefund(registerSysNoList);
        }

        /// <summary>
        /// 扣减RMA库存中OnVendorQty数量
        /// </summary>
        /// <param name="infoList"></param>
        public static void BatchDeductOnVendorQty(List<VendorRefundInfo> infoList)
        {
            ObjectFactory<IRMABizInteract>.Instance.BatchDeductOnVendorQty(infoList);
        }

        /// <summary>
        /// 批量更新送修单
        /// </summary>
        /// <param name="outBoundSysNoList"></param>
        public static void UpdateOutBound(string outBoundSysNoList)
        {
            ObjectFactory<IRMABizInteract>.Instance.UpdateOutBound(outBoundSysNoList);
        }
        #endregion

        #region [SO]

        /// <summary>
        /// PO - 生成虚库采购单后将对应的订单标识为备货状态
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <param name="vpoStatus"></param>
        public static void UpdateSOCheckShipping(int soSysNo, VirtualPurchaseOrderStatus vpoStatus)
        {
            ObjectFactory<ISOBizInteract>.Instance.UpdateSOCheckShipping(soSysNo, vpoStatus);
        }
        #endregion

    }
}
