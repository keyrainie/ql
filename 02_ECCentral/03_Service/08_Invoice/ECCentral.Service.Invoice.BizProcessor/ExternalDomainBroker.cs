using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity.RMA;
using ECCentral.BizEntity.SO;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.BizProcessor
{
    /// <summary>
    /// 统一管理需要调用外部Domain的接口方法
    /// </summary>
    public static class ExternalDomainBroker
    {
        #region 和SO交互的接口

        /// <summary>
        /// 根据订单编号取得订单基本信息
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <returns></returns>
        public static SOBaseInfo GetSOBaseInfo(int soSysNo)
        {
            return ObjectFactory<ISOBizInteract>.Instance.GetSOBaseInfo(soSysNo);
        }

        /// <summary>
        /// 获取订单商品列表
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        public static List<SOItemInfo> GetSOItemList(int soSysNo)
        {
            return ObjectFactory<ISOBizInteract>.Instance.GetSOItemList(soSysNo);
        }


        
        /// <summary>
        /// 根据订单系统编号列表取得订单基本信息列表
        /// </summary>
        /// <param name="soSysNoList">订单系统编号列表</param>
        /// <returns></returns>
        public static List<SOBaseInfo> GetSOBaseInfoList(List<int> soSysNoList)
        {
            return ObjectFactory<ISOBizInteract>.Instance.GetSOBaseInfoList(soSysNoList);
        }

        /// <summary>
        /// 根据订单系统编号取得订单信息
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <returns></returns>
        public static SOInfo GetSOInfo(int soSysNo)
        {
            return ObjectFactory<ISOBizInteract>.Instance.GetSOInfo(soSysNo);
        }

        /// <summary>
        /// 取得简单的SOBaseInfo对象，该方法用户自动审核收款单
        /// </summary>
        /// <param name="soSysNoList"></param>
        /// <returns></returns>
        public static List<SOInfo> GetSimpleSOInfoList(List<int> soSysNoList)
        {
            return ObjectFactory<ISOBizInteract>.Instance.GetSimpleSOInfoList(soSysNoList);
        }

        /// <summary>
        /// 根据订单编号取得订单的移仓单号
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <returns></returns>
        public static int GetShiftSysNoBySOSysNo(int soSysNO)
        {
            return ObjectFactory<ISOBizInteract>.Instance.GetShiftSysNoBySOSysNo(soSysNO);
        }

        #endregion 和SO交互的接口

        #region 和Inventory交互的接口

        /// <summary>
        /// 审核移仓单
        /// </summary>
        /// <param name="shiftRequestSysNo">移仓单系统编号</param>
        public static void VerifyShiftRequest(int shiftRequestSysNo)
        {
            ObjectFactory<IInventoryBizInteract>.Instance.VerifyShiftRequest(shiftRequestSysNo);
        }
        /// <summary>
        /// 编辑移仓单的金税号
        /// </summary>
        /// <param name="GoldenTaxNo"></param>
        /// <param name="stSysNo"></param>
        /// <returns></returns>
        public static bool EditGoldenTaxNo(string GoldenTaxNo, int stSysNo)
        {
            return ObjectFactory<IInventoryBizInteract>.Instance.EditGoldenTaxNo(GoldenTaxNo, stSysNo);
        }

        #endregion 和Inventory交互的接口

        #region 和Customer交互的接口

        /// <summary>
        /// 调整用户积分
        /// </summary>
        /// <param name="adjustRequest"></param>
        public static void AdjustPoint(AdjustPointRequest adjustRequest)
        {
            ObjectFactory<ICustomerBizInteract>.Instance.AdjustPoint(adjustRequest);
        }

        /// <summary>
        /// 根据CustomerSysNo查询Customer是否存在
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <returns></returns>
        public static bool ExistsCustomer(int customerSysNo)
        {
            return ObjectFactory<ICustomerBizInteract>.Instance.CustomerIsExists(customerSysNo);
        }

        /// <summary>
        /// 根据CustomerSysNo的返回Customer基本信息对象
        /// </summary>
        /// <param name="customerSysNo">Customer SysNo</param>
        /// <returns>Customer基本信息对象列表</returns>
        public static CustomerBasicInfo GetCustomerBasicInfo(int customerSysNo)
        {
            return ObjectFactory<ICustomerBizInteract>.Instance.GetCustomerBasicInfo(customerSysNo);
        }

        /// <summary>
        /// 确认销售收款单时，根据应收款更新客户可用账期额度
        /// </summary>
        /// <param name="customerSysNo">顾客系统编号</param>
        /// <param name="receivableAmount">应收款金额</param>
        public static void UpdateCustomerCreditLimitByReceipt(int customerSysNo, decimal receivableAmount)
        {
            ObjectFactory<ICustomerBizInteract>.Instance.AdjustCustomerCreditLimit(customerSysNo, receivableAmount);
        }

        /// <summary>
        /// 根据订单金额更新客户累计购买金额
        /// </summary>
        /// <param name="customerSysNo">顾客系统编号</param>
        /// <param name="soAmount">订单金额</param>
        public static void UpdateCustomerTotalMoney(int customerSysNo, decimal soAmount)
        {
            ObjectFactory<ICustomerBizInteract>.Instance.UpdateCustomerOrderedAmount(customerSysNo, soAmount);
        }

        /// <summary>
        /// 调整用户账户余额
        /// </summary>
        /// <param name="customerSysNo">顾客系统编号</param>
        /// <param name="soSysNo">订单系统编号</param>
        /// <param name="userSysNo">用户系统编号</param>
        /// <param name="adjustAmt">需要调整的余额数量（正数-余额增加，如RMA退款审核通过时；负数-余额减少，如订单使用账户余额时）</param>
        /// <param name="adjustPrepayType">需要调整的余额类型</param>
        /// <param name="memo">备注</param>
        public static void AdjustCustomerPerpayAmount(int customerSysNo, int soSysNo, decimal adjustAmt, PrepayType adjustPrepayType, string memo)
        {
            CustomerPrepayLog entity = new CustomerPrepayLog();
            entity.CustomerSysNo = customerSysNo;
            entity.SOSysNo = soSysNo;
            entity.AdjustAmount = adjustAmt;
            entity.PrepayType = adjustPrepayType;
            entity.Note = memo;
            ObjectFactory<ICustomerBizInteract>.Instance.AdjustPrePay(entity);
        }

        /// <summary>
        /// 审核补偿退款单
        /// </summary>
        /// <param name="SysNo"></param>
        /// <param name="Status"></param>
        /// <param name="RefundUserSysNo"></param>
        public static void AuditRefundAdjust(int SysNo, RefundAdjustStatus Status, int? RefundUserSysNo, DateTime? AuditTime)
        {
            ObjectFactory<ICustomerBizInteract>.Instance.AuditRefundAdjust(SysNo, Status, RefundUserSysNo, AuditTime);
        }

        /// <summary>
        /// 获取补偿退款单状态
        /// </summary>
        /// <param name="SysNo"></param>
        /// <returns></returns>
        public static RefundAdjustStatus? GetRefundAdjustStatus(int SysNo)
        {
            return ObjectFactory<ICustomerBizInteract>.Instance.GetRefundAdjustStatus(SysNo);
        }

        #endregion 和Customer交互的接口

        #region 和IM交互的接口

        /// <summary>
        /// 创建电子礼品卡,在内部组装sp所需的xml message
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <param name="customerSysNo">顾客编号</param>
        /// <param name="quantity">礼品卡数量</param>
        /// <param name="cashAmt">金额</param>
        /// <param name="memo">备注</param>
        /// <returns>操作结果状态码</returns>
        public static string CreateElectronicGiftCard(int soSysNo, int customerSysNo, int quantity, decimal cashAmt, GiftCardType internalType, string memo, string companyCode)
        {
            return ObjectFactory<IIMBizInteract>.Instance.CreateElectronicGiftCard(soSysNo, customerSysNo, quantity, cashAmt, internalType, "IPP.Invoice", memo, companyCode);
        }

        /// <summary>
        /// 根据PMSysNo获取PM组信息
        /// </summary>
        /// <param name="PMSysNo"></param>
        /// <returns></returns>
        public static ProductManagerGroupInfo GetPMListByPMSysNo(int PMSysNo)
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetPMListByUserSysNo(PMSysNo);
        }

        public static void UpdateProductVirtualPrice(int productSysNo, decimal originalVirtualPrice, decimal newVirtualPrice)
        {
            ObjectFactory<IIMBizInteract>.Instance.UpdateProductVirtualPrice(productSysNo, originalVirtualPrice, newVirtualPrice);
        }

        public static void UpdateProductBasicPrice(int productSysNo, decimal newPrice)
        {
            ObjectFactory<IIMBizInteract>.Instance.UpdateProductBasicPrice(productSysNo, newPrice);
        }

        public static void UpdateProductCurrentPrice(int productSysNo, decimal newPrice)
        {
            ObjectFactory<IIMBizInteract>.Instance.UpdateProductCurrentPrice(productSysNo, newPrice);
        }

        #endregion 和IM交互的接口

        #region 和RMA交互的接口

        /// <summary>
        /// 根据退款调整单系统编号取得退款调整单信息
        /// </summary>
        /// <param name="sysNo">退款调整单系统编号</param>
        /// <returns></returns>
        public static RefundBalanceInfo GetRefundBalanceBySysNo(int roBalanceSysNo)
        {
            return ObjectFactory<IRMABizInteract>.Instance.GetRefundBalanceBySysNo(roBalanceSysNo);
        }

        /// <summary>
        /// 根据退款单系统编号取得退款单信息
        /// </summary>
        /// <param name="roSysNo">退款单系统编号</param>
        /// <returns></returns>
        public static RefundInfo GetRefundBySysNo(int roSysNo)
        {
            return ObjectFactory<IRMABizInteract>.Instance.GetRefundBySysNo(roSysNo);
        }

        /// <summary>
        /// 根据退款单编号更新退款单的退款类型和退款原因
        /// </summary>
        /// <param name="refundSysNo">退款单编号</param>
        /// <param name="refundPayType">退款类型</param>
        /// <param name="refundReason">退款原因</param>
        public static void UpdateRefundPayTypeAndReason(int refundSysNo, int refundPayType, int refundReason)
        {
            ObjectFactory<IRMABizInteract>.Instance.UpdateRefundPayTypeAndReason(refundSysNo, refundPayType, refundReason);
        }

        #endregion 和RMA交互的接口

        #region 和Common交互的接口

        /// <summary>
        /// 获取所有的支付类型列表
        /// </summary>
        /// <returns></returns>
        public static List<PayType> GetPayTypeList()
        {
            return ObjectFactory<ICommonBizInteract>.Instance.GetPayTypeList("8601");
        }

        /// <summary>
        ///根据系统用户的编号获得系统用户信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public static UserInfo GetUserInfoBySysNo(int sysNo)
        {
            return ObjectFactory<ICommonBizInteract>.Instance.GetUserInfoBySysNo(sysNo);
        }


        /// <summary>
        /// 添加操作日志
        /// </summary>
        /// <param name="note"></param>
        /// <param name="logType"></param>
        /// <param name="ticketSysNo"></param>
        /// <param name="companyCode"></param>
        internal static void CreateOperationLog(string note, BizLogType logType, int ticketSysNo, string companyCode)
        {
            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(note, logType, ticketSysNo, companyCode);
        }

        #endregion 和Common交互的接口

        #region 和PO交互的接口

        /// <summary>
        /// 获取PO单信息
        /// </summary>
        /// <param name="purchaseOrderSysNo">PO单编号</param>
        /// <returns>PO单信息</returns>
        public static PurchaseOrderInfo GetPurchaseOrderInfo(int purchaseOrderSysNo)
        {
            return ObjectFactory<IPOBizInteract>.Instance.GetPurchaseOrderInfo(purchaseOrderSysNo);
        }

        /// <summary>
        /// 获取PO单信息
        /// </summary>
        /// <param name="purchaseOrderSysNo">PO单编号</param>
        /// <param name="batchNumber">批次号</param>
        /// <returns>PO单信息</returns>
        public static PurchaseOrderInfo GetPurchaseOrderInfo(int purchaseOrderSysNo, int batchNumber)
        {
            return ObjectFactory<IPOBizInteract>.Instance.GetPurchaseOrderInfo(purchaseOrderSysNo, batchNumber);
        }

        /// <summary>
        /// 获取代销结算单信息
        /// </summary>
        /// <param name="consignSettlementSysNo">代销结算单编号</param>
        /// <returns>代销结算单信息</returns>
        public static ConsignSettlementInfo GetConsignSettlementInfo(int consignSettlementSysNo)
        {
            return ObjectFactory<IPOBizInteract>.Instance.GetConsignSettlementInfo(consignSettlementSysNo);
        }

        /// <summary>
        /// 获取代收结算单信息
        /// </summary>
        /// <param name="gatherSettlementSysNo">代收结算单编号</param>
        /// <returns>代收结算单信息</returns>
        public static GatherSettlementInfo GetGatherSettlementInfo(int gatherSettlementSysNo)
        {
            return ObjectFactory<IPOBizInteract>.Instance.GetGatherSettlementInfo(gatherSettlementSysNo);
        }

        /// <summary>
        /// 获取佣金信息
        /// </summary>
        /// <param name="commissionMasterSysNo">佣金信息编号</param>
        /// <returns>佣金信息</returns>
        public static CommissionMaster GetCommissionMaster(int commissionMasterSysNo)
        {
            return ObjectFactory<IPOBizInteract>.Instance.GetCommissionMaster(commissionMasterSysNo);
        }

        /// <summary>
        /// 判断指定SysNo供应商是否已锁定
        /// </summary>
        /// <param name="vendorSysNo">供应商SysNo</param>
        /// <returns>锁定为true，否则为false</returns>
        public static bool IsHolderVendorByVendorSysNo(int vendorSysNo)
        {
            return ObjectFactory<IPOBizInteract>.Instance.IsHolderVendorByVendorSysNo(vendorSysNo);
        }

        /// <summary>
        /// 判断应付款对应的供应商是否已锁定
        /// </summary>
        /// <param name="poSysNo">采购单SysNo</param>
        /// <returns>锁定为true，否则为false</returns>
        public static bool IsHolderVendorByPOSysNo(int poSysNo)
        {
            return ObjectFactory<IPOBizInteract>.Instance.IsHolderVendorByPOSysNo(poSysNo);
        }

        /// <summary>
        /// 判断应付款对应的供应商是否已锁定
        /// </summary>
        /// <param name="vendorSettleSysNo">代销结算单SysNo</param>
        /// <returns>锁定为true，否则为false</returns>
        public static bool IsHolderVendorByVendorSettleSysNo(int vendorSettleSysNo)
        {
            return ObjectFactory<IPOBizInteract>.Instance.IsHolderVendorByVendorSettleSysNo(vendorSettleSysNo);
        }

        /// <summary>
        /// 判断应付款对应的供应商是否已锁定
        /// </summary>
        /// <param name="collectionSettlementSysNo">代收结算单SysNo</param>
        /// <returns>锁定为true，否则为false</returns>
        public static bool IsHolderVendorByCollectionSettlementSysNo(int collectionSettlementSysNo)
        {
            return ObjectFactory<IPOBizInteract>.Instance.IsHolderVendorByCollectionSettlementSysNo(collectionSettlementSysNo);
        }

        /// <summary>
        /// 判断应付款对应的供应商PM是否已锁定
        /// </summary>
        /// <param name="poSysNo">采购单SysNo</param>
        /// <returns>锁定为true，否则为false</returns>
        public static bool IsHolderVendorPMByPOSysNo(int poSysNo)
        {
            return ObjectFactory<IPOBizInteract>.Instance.IsHolderVendorPMByPOSysNo(poSysNo);
        }

        /// <summary>
        /// 判断应付款对应的供应商PM是否已锁定
        /// </summary>
        /// <param name="vendorSettleSysNo">代销结算单SysNo</param>
        /// <returns>锁定为true，否则为false</returns>
        public static bool IsHolderVendorPMByVendorSettleSysNo(int vendorSettleSysNo)
        {
            return ObjectFactory<IPOBizInteract>.Instance.IsHolderVendorPMByVendorSettleSysNo(vendorSettleSysNo);
        }

        /// <summary>
        /// 根据采购单系统编号取得供应商返点信息
        /// </summary>
        /// <param name="poSysNo">采购单系统编号</param>
        /// <returns>供应商返点信息</returns>
        public static List<EIMSInfo> LoadPurchaseOrderEIMSInfo(int poSysNo)
        {
            return ObjectFactory<IPOBizInteract>.Instance.LoadPurchaseOrderEIMSInfo(poSysNo);
        }

        /// <summary>
        /// 根据供应商系统编号取得供应商基本信息
        /// </summary>
        /// <param name="vendorSysNo">供应商系统编号</param>
        /// <returns>供应商基本信息</returns>
        public static VendorInfo GetVendorBasicInfo(int vendorSysNo)
        {
            return ObjectFactory<IPOBizInteract>.Instance.GetVendorInfoSysNo(vendorSysNo);
        }

        /// <summary>
        ///  获取供应商基本信息
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        public static VendorBasicInfo GetVendorBasicInfoBySysNo(int vendorSysNo)
        {
            return ObjectFactory<IPOBizInteract>.Instance.GetVendorBasicInfoBySysNo(vendorSysNo);
        }

        /// <summary>
        /// 采购单中止入库
        /// </summary>
        /// <param name="poSysNo">采购单系统编号</param>
        public static void StopInStock(int poSysNo)
        {
            ObjectFactory<IPOBizInteract>.Instance.StopInStock(poSysNo);
        }

        /// <summary>
        /// 取得采购单返点系统编号（EIMS系统在IPP系统中记录的信息）
        /// </summary>
        /// <param name="poSysNo">采购单系统编号</param>
        /// <returns>返点信息系统编号</returns>
        public static int? GetPurchaseOrderReturnPointSysNo(int poSysNo)
        {
            return ObjectFactory<IPOBizInteract>.Instance.GetPurchaseOrderReturnPointSysNo(poSysNo);
        }

        /// <summary>
        /// 取得代销结算单返点系统编号（EIMS系统在IPP系统中记录的信息）
        /// </summary>
        /// <param name="consignSettleSysNo">代销结算单系统编号</param>
        /// <returns>返点信息系统编号</returns>
        public static int? GetConsignSettlementReturnPointSysNo(int consignSettleSysNo)
        {
            return ObjectFactory<IPOBizInteract>.Instance.GetConsignSettlementReturnPointSysNo(consignSettleSysNo);
        }



        /// <summary>
        /// 根据供应商系统编号取得PO单系统编号列表
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        public static List<int> GetPOSysNoListByVendorSysNo(int vendorSysNo, List<int> pmSysNoList)
        {
            return ObjectFactory<IPOBizInteract>.Instance.GetPurchaseOrderSysNoListByVendorSysNo(vendorSysNo, pmSysNoList);
        }

        /// <summary>
        /// 根据供应商系统编号取得代销结算单系统编号列表
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        public static List<int> GetVendorSettleSysNoListByVendorSysNo(int vendorSysNo, List<int> pmSysNoList)
        {
            return ObjectFactory<IPOBizInteract>.Instance.GetVendorSettleSysNoListByVendorSysNo(vendorSysNo, pmSysNoList);
        }

        /// <summary>
        /// 根据供应商系统编号取得代收结算单系统编号列表
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        public static List<int> GetCollectionSettlementSysNoListByVendorSysNo(int vendorSysNo)
        {
            return ObjectFactory<IPOBizInteract>.Instance.GetGatherSettlementSysNoListByVendorSysNo(vendorSysNo);
        }

        /// <summary>
        /// 根据供应商系统编号取得供应商财务信息
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        public static VendorFinanceInfo GetVendorFinanceInfoBySysNo(int vendorSysNo)
        {
            return ObjectFactory<IPOBizInteract>.Instance.GetVendorFinanceInfoBySysNo(vendorSysNo);
        }

        /// <summary>
        /// 根据编号取代收代付结算单信息
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        public static CollectionPaymentInfo GetCollectionPaymentInfo(int vendorSysNo)
        {
            return ObjectFactory<IPOBizInteract>.Instance.GetCollectionPaymentInfo(vendorSysNo);
        }

        #endregion 和PO交互的接口

        #region 和Common交互的接口

        public static PayType GetPayType(int payTypeSysNo)
        {
            return ObjectFactory<ICommonBizInteract>.Instance.GetPayType(payTypeSysNo);
        }

        #endregion 和Common交互的接口

        #region 和3PartService交互的接口
        /// <summary>
        /// 退预付卡
        /// </summary>
        /// <param name="refundAmount"></param>
        /// <param name="soSysNo"></param>
        /// <param name="tNumber"></param>
        /// <param name="refundKey"></param>
        /// <returns></returns>
        public static int RefundPrepayCard(decimal refundAmount, int soSysNo, string tNumber, string refundKey)
        {
            return ObjectFactory<IThirdPartBizInteract>.Instance.RefundPrepayCard(refundAmount, soSysNo, tNumber, refundKey);
        }
        #endregion

        /// <summary>
        /// 获取支付类型
        /// </summary>
        /// <param name="payTypeSysNo"></param>
        /// <returns></returns>
        public static PayType GetPayTypeBySysNo(int payTypeSysNo)
        {
            return ObjectFactory<ICommonBizInteract>.Instance.GetPayType(payTypeSysNo);
        }

    }
}
