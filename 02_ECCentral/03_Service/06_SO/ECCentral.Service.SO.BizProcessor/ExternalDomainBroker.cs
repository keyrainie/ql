using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.SO;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Utility;
using ECCentral.Service.EventMessage;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity.IM.Product;
using ECCentral.BizEntity.Invoice.Invoice;

namespace ECCentral.Service.SO.BizProcessor
{
    /// <summary>
    /// 调用外部接口的内部通用类。将SO中调用外部接口的地方都集中到这里。
    /// </summary>
    public static class ExternalDomainBroker
    {
        #region 发送邮件

        /// <summary>
        /// 发送内部邮件
        /// </summary>
        /// <param name="mailInfoMessage"></param>
        /// <param name="isAsync"></param>
        public static void SendInternalEmail(string templateID, KeyValueVariables keyValueVariables,
            KeyTableVariables keyTableVariables)
        {
            try
            {
                ECCentral.Service.Utility.EmailHelper.SendEmailByTemplate(null, null, null, templateID,
                    keyValueVariables, keyTableVariables, false, true);
            }
            catch (Exception ex)
            {
                ExceptionHelper.HandleException(ex);
            }
        }

        /// <summary>
        /// 异步发送内部邮件
        /// </summary>
        /// <param name="mailInfoMessage"></param>
        /// <param name="isAsync"></param>
        public static void SendInternalEmail(string templateID, KeyValueVariables keyValueVariables)
        {
            SendInternalEmail(templateID, keyValueVariables, null);
        }

        /// <summary>
        /// 异步发送内部邮件( 增加地址 传入参数)
        /// </summary>
        /// <param name="toEmailAddress">发送地址</param>
        /// <param name="ccEmailAddress">抄送地址</param>
        /// <param name="templateID">模版ID</param>
        /// <param name="keyValueVariables">内容键值</param>
        /// <param name="languageCode">语言编码</param> 
        public static void SendInternalEmail(string toEmailAddress, string ccEmailAddress, string templateID, KeyValueVariables keyValueVariables, string languageCode)
        {
            try
            {
                ECCentral.Service.Utility.EmailHelper.SendEmailByTemplate(toEmailAddress, ccEmailAddress, null, templateID, keyValueVariables, null, true, true, languageCode);
            }
            catch (Exception ex)
            {
                ExceptionHelper.HandleException(ex);
            }
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

        public static void SendEmail(string toAddress, string ccAddress, string bccAddress, string templateID, KeyValueVariables keyValueVariables, KeyTableVariables keyTableVariables, bool isInternalMail, bool isAsyncMail)
        {
            EmailHelper.SendEmailByTemplate(toAddress, ccAddress, bccAddress, templateID, keyValueVariables, keyTableVariables, isInternalMail, isAsyncMail);
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="phoneNumber">发送到的短信号码，多个号码可以用英文半角的逗号“,”或分号“;”隔开</param>
        /// <param name="content">短信内容</param>
        /// <param name="priority">发送的优先级</param> 
        public static void SendSMS(string phoneNumber, string content, SMSPriority priority)
        {
            try
            {
                ObjectFactory<ECCentral.Service.IBizInteract.ICommonBizInteract>.Instance.SendSMS(phoneNumber, content, priority);
            }
            catch (Exception ex)
            {
                ExceptionHelper.HandleException(ex);
            }
        }
        #endregion 发送邮件

        #region MKT Domain

        internal static bool CheckCouponIsValid(int couponSysNo, out string couponCode)
        {
            return ObjectFactory<IMKTBizInteract>.Instance.CheckCouponIsValid(couponSysNo, out couponCode);
        }

        /// <summary>
        /// 订单使用优惠券
        /// </summary>
        /// <param name="couponSysNo">优惠券编号</param>
        /// <param name="couponCode">优惠券代码</param>        
        /// <param name="customerSysNo">客户编号</param>
        /// <param name="soSysNo">订单编号</param>
        /// <param name="shoppingCartSysNo">购物车编号</param>
        /// <param name="redeemAmount">使用金额</param>
        internal static void ApplyCoupon(int couponSysNo, string couponCode, int customerSysNo, int soSysNo, int shoppingCartSysNo, decimal redeemAmount)
        {
            ObjectFactory<IMKTBizInteract>.Instance.CouponCodeApply(couponSysNo, couponCode, customerSysNo, shoppingCartSysNo, soSysNo, redeemAmount);
        }

        /// <summary>
        /// 订单取消使用优惠券
        /// </summary>
        /// <param name="couponCode">优惠券代码</param>
        /// <param name="soSysNo">订单编号</param>
        /// <param name="shoppingCartSysNo">购物车编号</param>
        internal static void CancelCoupon(string couponCode, int soSysNo, int shoppingCartSysNo)
        {
            ObjectFactory<IMKTBizInteract>.Instance.CouponCodeCancel(couponCode, soSysNo, shoppingCartSysNo);
        }

        /// <summary>
        /// 根据组合促销编号列表取得组合促销信息
        /// </summary>
        /// <param name="comboSysNoList"></param>
        /// <returns></returns>
        public static List<ECCentral.BizEntity.MKT.ComboInfo> GetComboList(List<int> comboSysNoList)
        {
            return ObjectFactory<IMKTBizInteract>.Instance.GetComboList(comboSysNoList);
        }

        /// <summary>
        /// 根据订单信息计算所有促销活动的优惠信息，目前包括：Combo，Coupons，GiftSale
        /// [Jin]已确认
        /// </summary>
        /// <param name="soInfo">订单信息</param>
        /// <returns>订单参与的促销记录</returns>
        internal static List<SOPromotionInfo> CalculateSOPromotion(SOInfo soInfo)
        {
            return CalculateSOPromotion(soInfo, true);
        }

        internal static List<SOPromotionInfo> CalculateSOPromotion(SOInfo soInfo, bool isModifyCoupons)
        {
            return ObjectFactory<IMKTBizInteract>.Instance.CalculateSOPromotion(soInfo, isModifyCoupons);
        }

        /// <summary>
        /// 取得没有处理的团购信息
        /// </summary>
        /// <param name="companyCode">如果为null,表示取得所有没有处理的团购信息</param>
        /// <returns></returns>
        public static List<ECCentral.BizEntity.MKT.GroupBuyingInfo> GetGroupBuyInfoForNeedProcess(string companyCode)
        {
            return ObjectFactory<IMKTBizInteract>.Instance.GetGroupBuyInfoForNeedProcess(companyCode);
        }

        /// <summary>
        /// 取得团购编号取得团购信息
        /// </summary>
        /// <param name="sysNo"> 团购编号</param>
        /// <returns></returns>
        internal static ECCentral.BizEntity.MKT.GroupBuyingInfo GetGroupBuyInfoBySysNo(int sysNo)
        {
            return ObjectFactory<IMKTBizInteract>.Instance.GetGroupBuyInfoBySysNo(sysNo);
        }
        /// <summary>
        /// 修改团购处理状态
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="settlementStatus"></param>
        public static void UpdateGroupBuySettlementStatus(int sysNo, ECCentral.BizEntity.MKT.GroupBuyingSettlementStatus settlementStatus)
        {
            ObjectFactory<IMKTBizInteract>.Instance.UpdateGroupBuySettlementStatus(sysNo, settlementStatus);
        }

        /// <summary>
        /// 根据商品编号获取正在参加团购的商品编号
        /// </summary>
        /// <param name="products">待验证的商品编号</param>
        /// <returns>正在参加团购的商品编号</returns>
        internal static List<int> GetProductsOnGroupBuying(IEnumerable<int> products)
        {
            return ObjectFactory<IMKTBizInteract>.Instance.GetProductsOnGroupBuying(products);
        }

        #endregion MKT Domain

        #region Customer Domain

        /// <summary>
        /// 取得积分换算成钱的比率，如果是积分换算成钱，则除以此值；如果是钱换算成积分，则乘以此值
        /// </summary>
        /// <returns></returns>
        public static decimal GetPointToMoneyRatio()
        {
            return ObjectFactory<ICustomerBizInteract>.Instance.GetPointToMoneyRatio();
        }

        /// <summary>
        /// 根据客户编号取得客户信息
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <returns></returns>
        internal static CustomerInfo GetCustomerInfo(int customerSysNo)
        {
            return ObjectFactory<ICustomerBizInteract>.Instance.GetCustomerInfo(customerSysNo);
        }

        internal static List<CustomerBasicInfo> GetCustomerBasicInfo(List<int> customerSysNoList)
        {
            return ObjectFactory<ICustomerBizInteract>.Instance.GetCustomerBasicInfo(customerSysNoList);
        }

        internal static CustomerBasicInfo GetCustomerBasicInfo(int customSysNo)
        {
            return ObjectFactory<ICustomerBizInteract>.Instance.GetCustomerBasicInfo(customSysNo);
        }

        /// <summary>
        /// 获取顾客等级
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <returns></returns>
        internal static CustomerRank GetCustomerRank(int customerSysNo)
        {
            return ObjectFactory<ICustomerBizInteract>.Instance.GetCustomerRank(customerSysNo);
        }

        /// <summary>
        /// 领取奖品
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="productSysNo"></param>
        /// <param name="soSysNo"></param>
        internal static void GetGift(int customerSysNo, int productSysNo, int soSysNo)
        {
            ObjectFactory<ICustomerBizInteract>.Instance.GetGift(customerSysNo, productSysNo, soSysNo);
        }

        /// <summary>
        /// 关闭客户来电
        /// </summary>
        /// <param name="sourceSysNo"></param>
        /// <param name="callingReferenceType"></param>
        /// <param name="note"></param>
        internal static void CallingCustomStatus(int sourceSysNo, CallingReferenceType callingReferenceType, string note)
        {
            ObjectFactory<ICustomerBizInteract>.Instance.CloseCallsEvents(callingReferenceType, sourceSysNo, note);
        }

        /// <summary>
        /// 创建补偿积分申请单，并记录日志，返回补偿积分申请单编号
        /// </summary>
        /// <param name="customerSysNo">顾客系统编号</param>
        /// <param name="soSysNo">订单系统编号</param>
        /// <param name="adjustAccount">调整积分的账户信息</param>
        /// <param name="point">需要调整的积分（正数-增加积分；负数-减少积分）</param>
        /// <param name="logType">日志类型</param>
        /// <param name="productSysNoList">商品系统编号列表</param>
        /// <param name="memo">备注</param>
        /// <returns>补偿积分申请单系统编号</returns>
        public static int CreateAdjustPointRequest(int customerSysNo, int soSysNo, string adjustAccount, int point, int logType, List<int> productSysNoList, string memo)
        {
            return ObjectFactory<ICustomerBizInteract>.Instance.CreateAdjustPointRequest(customerSysNo, soSysNo, adjustAccount, point, logType, productSysNoList, memo);
        }

        /// <summary>
        /// 调整积分检查
        /// </summary>
        /// <param name="info"></param>
        internal static void AdjustPointPrecheck(ECCentral.BizEntity.Customer.AdjustPointRequest info) //IPP3: InvoiceServiceAdapter.AjustPointPrecheck(int customerSysNo, int pointPay, Common.PointLogType pointLogType, int sysNo, string note, MessageHeaderInfo header)
        {
            ObjectFactory<ICustomerBizInteract>.Instance.AdjustPointPreCheck(info);
        }

        ///// <summary>
        ///// 调整账期额度
        ///// </summary>
        ///// <param name="customerSysNo"></param>
        ///// <param name="receivableAmount"></param>
        //internal static void AdjustCustomerCreditLimit(int customerSysNo, decimal receivableAmount)
        //{
        //    ObjectFactory<ICustomerBizInteract>.Instance.AdjustCustomerCreditLimit(customerSysNo, receivableAmount);
        //}

        /*//
        /// <summary>
        /// 调整账户余额前的检查
        /// </summary>
        /// <param name="info"></param>
        internal static void AdjustPrePayPreCheck(CustomerPrepayLog info) //IPP3: CustomerServiceAdapter.SetCustomerPrepayPrecheck()
        {
            ObjectFactory<ICustomerBizInteract>.Instance.AdjustPrePayPreCheck(info);
        }
        /// <summary>
        /// 调整账期前的检查
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="amount"></param>
        internal static void AdjustCreditPayPreCheck(int customerSysNo, decimal amount)
        {
            ObjectFactory<ICustomerBizInteract>.Instance.AdjustCreditLimitPreCheck(customerSysNo, amount);
        }
        //*/

        /// <summary>
        /// 调整账户余额
        /// </summary>
        /// <param name="info"></param>
        internal static void AdjustPrePay(CustomerPrepayLog info) //IPP3: CustomerPrepayLogV31.BalanceAccount()
        {
            ObjectFactory<ICustomerBizInteract>.Instance.AdjustPrePay(info);
        }

        /// <summary>
        /// 调整积分
        /// </summary>
        /// <param name="info"></param>
        internal static void AdjustPoint(ECCentral.BizEntity.Customer.AdjustPointRequest info)
        {
            ObjectFactory<ICustomerBizInteract>.Instance.AdjustPoint(info);
        }

        //internal static void AdjustPerpayAmount(int customerSysNo,soSysNo)
        //{
        //    ObjectFactory<ICustomerBizInteract>.Instance.AdjustCustomerPerpayAmount(customerSysNo, soSysNo, adjustAmount, adjustPrepayType, memo);
        //}

        /// <summary>
        /// 订单拆分时调整积分
        /// </summary>
        /// <param name="info"></param>
        internal static void SplitSOPoint(SOBaseInfo masterSO, List<SOBaseInfo> subSOList)
        {
            ObjectFactory<ICustomerBizInteract>.Instance.SplitSOPointLog(masterSO, subSOList);
        }

        /// <summary>
        /// 取消订单拆分时调整积分
        /// </summary>
        /// <param name="info"></param>
        internal static void CancelSplitSOPoint(SOBaseInfo masterSO, List<SOBaseInfo> subSOList)
        {
            ObjectFactory<ICustomerBizInteract>.Instance.CancelSplitSOPointLog(masterSO, subSOList);
        }

        ///// <summary>
        /////  调整账期
        ///// </summary>
        ///// <param name="customerSysNo"></param>
        ///// <param name="amount"></param>
        //internal static void AdjustCreditPay(int customerSysNo, decimal amount)
        //{
        //    ObjectFactory<ICustomerBizInteract>.Instance.AdjustCustomerCreditLimit(customerSysNo, amount);
        //}

        /// <summary>
        /// 返还订单的奖品 订单作废时需返还奖品
        /// </summary>
        /// <param name="customerSysNo">客户编号</param>
        /// <param name="soSysNo">订单编号</param>
        internal static void ReturnAwardForSO(int soSysNo)//IMaintainGiftStatusV31.VoidGiftForSO()
        {
            ObjectFactory<ICustomerBizInteract>.Instance.ReturnGiftForSO(soSysNo);
        }

        /// <summary>
        /// 根据客户编号 获取客户权限
        /// </summary>
        /// <param name="customerSysNo">客户编号</param>
        /// <returns></returns>
        internal static List<CustomerRight> GetCustomerRight(int customerSysNo)
        {
            return ObjectFactory<ICustomerBizInteract>.Instance.GetCustomerRight(customerSysNo);
        }

        /// <summary>
        /// 设置客户增值税发票信息
        /// </summary>
        /// <param name="customerSysNo">客户编号</param>
        /// <param name="bankAcct">账户</param>
        /// <param name="companyName">公司名称</param>
        /// <param name="companyAddress">公司地址</param>
        /// <param name="companyPhone">公司电话</param>
        /// <param name="taxAccount">税号</param>
        /// <param name="isDefault">是否存在认证证书</param>
        /// <param name="IsUpdate">是更新还是创建</param>
        internal static void SetCustomerValueAddedTax(int VATSysNo, int customerSysNo, string bankAcct, string companyName, string companyAddress, string companyPhone, string taxAccount, bool IsReceTaxpayerCertificate, bool IsUpdate)
        {
            ObjectFactory<ICustomerBizInteract>.Instance.SetCustomerValueAddedTax(VATSysNo, customerSysNo, bankAcct, companyName, companyAddress, companyPhone, taxAccount, IsReceTaxpayerCertificate, IsUpdate);
        }

        /// <summary>
        /// 调整客户经验值
        /// </summary>
        /// <param name="customerSysNo">客户编号</param>
        /// <param name="adjustAmount">金额</param>
        /// <param name="type"></param>
        /// <param name="memo"></param>
        internal static void AdjustCustomerExperience(int customerSysNo, decimal adjustAmount, ExperienceLogType type, string memo)
        {
            ObjectFactory<ICustomerBizInteract>.Instance.AdjustCustomerExperience(customerSysNo, adjustAmount, type, memo);
        }

        /// <summary>
        /// 获取FPCheck列表
        /// </summary>
        /// <param name="companyCode">公司编码</param>
        /// <returns>FPCheck列表</returns>
        internal static List<FPCheck> GetFPCheckList(string companyCode)
        {
            return ObjectFactory<ICustomerBizInteract>.Instance.GetFPCheckList(companyCode);
        }

        //获取恶意用户
        /// <summary>
        /// 获取恶意用户
        /// </summary>
        /// <param name="companyCode">公司编码</param>
        /// <returns>恶意用户列表</returns>
        internal static List<CustomerInfo> GetMalevolenceCustomers(string companyCode)
        {
            return ObjectFactory<ICustomerBizInteract>.Instance.GetMalevolenceCustomerList(companyCode);
        }

        /// <summary>
        /// 获取自动审单列表
        /// </summary>
        /// <param name="companyCode">公司编号</param>
        /// <returns>自动审单列表</returns>
        internal static List<OrderCheckMaster> GetCSTBOrderCheckMasterList(string companyCode)
        {
            return ObjectFactory<ICustomerBizInteract>.Instance.GetOrderCheckList(companyCode);
        }

        internal static int GetCustomerValidPoint(int customerSysNo)
        {
            return ObjectFactory<ICustomerBizInteract>.Instance.GetCustomerVaildScore(customerSysNo);
        }

        #endregion Customer Domain

        #region Invoice Domain

        /// <summary>
        /// 取得有效的订单支付信息
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static ECCentral.BizEntity.Invoice.SOIncomeInfo GetValidSOIncomeInfo(int soSysNo, ECCentral.BizEntity.Invoice.SOIncomeOrderType type)
        {
            return ObjectFactory<IInvoiceBizInteract>.Instance.GetValidSOIncome(soSysNo, type);
        }

        /// <summary>
        /// 赠品订单自动创建
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <param name="payAmount">支付金额</param>
        /// <param name="payTypeSysNo">支付类型</param>
        /// <param name="companyCode">公司编号</param>
        /// <returns>NetPay信息</returns> 
        internal static ECCentral.BizEntity.Invoice.NetPayInfo CreatNetPay(int soSysNo, decimal payAmount, int payTypeSysNo, string companyCode)
        {
            NetPayInfo netpay = new NetPayInfo();
            netpay.SOSysNo = soSysNo;
            netpay.PayAmount = payAmount;
            netpay.PayTypeSysNo = payTypeSysNo;
            netpay.RelatedSoSysNo = soSysNo;
            netpay.CompanyCode = companyCode;
            netpay.Note = "赠品订单自动创建";
            netpay.Source = NetPaySource.Bank;
            return ObjectFactory<IInvoiceBizInteract>.Instance.CreateNetPay(netpay, null, false);
        }

        /// <summary>
        /// 根据订单编号取得有效的支付信息
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        internal static ECCentral.BizEntity.Invoice.NetPayInfo GetSOValidNetPay(int soSysNo)
        {
            return ObjectFactory<IInvoiceBizInteract>.Instance.GetSOValidNetPay(soSysNo);
        }

        /// <summary>
        /// 是否存在待审核的网上支付记录
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        internal static bool IsExistOriginNetPay(int soSysNo)
        {
            return ObjectFactory<IInvoiceBizInteract>.Instance.IsExistOriginNetPay(soSysNo);
        }

        internal static void AbandonSOIncome(int soIncomeSysNo)
        {
            ObjectFactory<IInvoiceBizInteract>.Instance.AbandonSOIncome(soIncomeSysNo);
        }

        /// <summary>
        /// 创建财务收款单
        /// </summary>
        /// <param name="entity">需要创建的财务收款单信息</param>
        /// <returns>创建后的财务收款单，SysNo为数据持久化后的系统编号</returns>
        internal static BizEntity.Invoice.SOIncomeInfo CreateSOIncome(BizEntity.Invoice.SOIncomeInfo soIncomeInfo)
        {
            return ObjectFactory<IInvoiceBizInteract>.Instance.CreateSOIncome(soIncomeInfo);
        }

        /// <summary>
        /// 创建销售退款单
        /// </summary>
        /// <param name="entity">销售退款单信息</param>
        /// <returns>创建后的销售退款单</returns>
        internal static BizEntity.Invoice.SOIncomeRefundInfo CreateSOIncomeRefundInfo(BizEntity.Invoice.SOIncomeRefundInfo refundInfo)
        {
            return ObjectFactory<IInvoiceBizInteract>.Instance.CreateSOIncomeRefund(refundInfo);
        }

        /// <summary>
        /// 根据订单编号取得发票信息
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        internal static List<BizEntity.Invoice.InvoiceMasterInfo> GetSOInvoiceMaster(int soSysNo)
        {
            return ObjectFactory<IInvoiceBizInteract>.Instance.GetSOInvoiceMaster(soSysNo);
        }

        internal static BizEntity.Invoice.InvoiceInfo CreateInvoice(BizEntity.Invoice.InvoiceInfo invoiceInfo)
        {
            return ObjectFactory<IInvoiceBizInteract>.Instance.CreateInvoice(invoiceInfo);
        }

        internal static void SplitSOIncome(SOBaseInfo masterSO, List<SOBaseInfo> subSOList)
        {
            ObjectFactory<IInvoiceBizInteract>.Instance.CreateSplitSOIncome(masterSO, subSOList);
        }

        internal static void CancelSplitSOIncome(SOBaseInfo masterSO, List<SOBaseInfo> subSOList)
        {
            ObjectFactory<IInvoiceBizInteract>.Instance.CancelSplitSOIncome(masterSO, subSOList);
        }

        /// <summary>
        /// 审核网上支付
        /// </summary>
        /// <param name="netpaySysNo">网上支付记录编号</param>
        internal static void AuditNetPay(int netpaySysNo)
        {
            ObjectFactory<IInvoiceBizInteract>.Instance.AuditNetPay(netpaySysNo);
        }

        internal static void AuditNetPay4GroupBuy(int netpaySysNo)
        {
            ObjectFactory<IInvoiceBizInteract>.Instance.AuditNetPay4GroupBuy(netpaySysNo);
        }

        /// <summary>
        /// 拆分发票
        /// </summary>
        /// <param name="invoiceItems"></param>
        internal static void SplitInvoice(List<ECCentral.BizEntity.Invoice.SubInvoiceInfo> invoiceItems)
        {
            ObjectFactory<IInvoiceBizInteract>.Instance.CreateSubInvoiceItems(invoiceItems);
        }

        /// <summary>
        /// 取消拆分发票
        /// </summary>
        /// <param name="invoiceItems"></param>
        internal static void CancelSplitInvoice(int soSysNo)
        {
            ObjectFactory<IInvoiceBizInteract>.Instance.DeleteSubInvoiceBySOSysNo(soSysNo);
        }

        internal static void UpdateSOIncomeOrderAmount(int soIncomeSysNo, decimal amount)
        {
            ObjectFactory<IInvoiceBizInteract>.Instance.UpdateSOIncomeOrderAmt(soIncomeSysNo, amount);
        }

        internal static void CreateAOForJob(int soSysNo, ECCentral.BizEntity.Invoice.RefundPayType refundPayType, string note, int? refundReason)
        {
            ObjectFactory<IInvoiceBizInteract>.Instance.CreateAOForJob(soSysNo, refundPayType, note, refundReason);
        }

        internal static TransactionQueryBill QueryBill(string soSysNo)
        {
           return ObjectFactory<IInvoiceBizInteract>.Instance.QueryBill(soSysNo);
        }

        /// <summary>
        /// 根据单据类型和单据编号取得有效的销售收款单
        /// </summary>
        /// <param name="orderSysNo">单据系统编号</param>
        /// <returns></returns>
        internal static SOIncomeInfo GetValidSOIncomeInfo(int orderSysNo)
        {
            return ObjectFactory<IInvoiceBizInteract>.Instance.GetValid(orderSysNo);
        }

        #endregion Invoice Domain

        #region IM Domain

        /// <summary>
        /// 取消礼品卡使用
        /// </summary>
        /// <param name="soSysNo"></param>
        internal static void CancelUseGiftCard(int soSysNo, string companyCode)
        {
            ObjectFactory<IIMBizInteract>.Instance.CancelUsedGiftCard(soSysNo, companyCode);
        }

        /// <summary>
        /// 根据商品列表取得商品编号。
        /// </summary>
        /// <param name="productSysNoList"></param>
        /// <returns></returns>
        internal static List<ProductInfo> GetProductInfoListByProductSysNoList(List<int> productSysNoList)
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetProductInfoListByProductSysNoList(productSysNoList);
        }

        /// <summary>
        /// 获取商品的简单信息
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <returns>简单信息</returns>
        internal static ProductInfo GetProductSimpleInfo(int productSysNo)
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetSimpleProductInfo(productSysNo);
        }

        internal static ProductInfo GetProductInfo(string productID)
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetProductInfo(productID);
        }

        /// <summary>
        /// 根据商品ID获取所在Domain的简单信息
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        internal static ProductDomain GetDomainByProductID(string productID)
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetDomainByProductID(productID);
        }

        internal static void CreateElectronicGiftCard(int soSysNo, int customerSysNo, decimal cashAmt, int quantity, string companyCode, string memo)
        {
            ObjectFactory<IIMBizInteract>.Instance.CreateElectronicGiftCard(soSysNo, customerSysNo, quantity, cashAmt, GiftCardType.Standard, "IPP.Order", memo, companyCode);
        }

        internal static List<GiftCardRedeemLog> GetSOGiftCardBySOSysNo(int soSysNo)
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetGiftCardRedeemLog(soSysNo, BizEntity.IM.ActionType.SO);
        }

        /// <summary>
        /// 根据 礼品卡编号 和密码 获取 对应的礼品卡信息
        /// </summary>
        /// <param name="code">礼品卡 卡号</param>
        /// <param name="password">礼品卡 密码</param>
        internal static GiftCardInfo GetGiftCardByCodeAndPassword(string code, string password)
        {
            string TempPassword = CryptoManager.GetCrypto(CryptoAlgorithm.DES).Encrypt(password);
            GiftCardInfo giftCard = null;
            List<string> codeList = new List<string>();
            codeList.Add(code);
            List<ECCentral.BizEntity.IM.GiftCardInfo> infoList = ObjectFactory<IIMBizInteract>.Instance.GetGiftCardsByCodeList(codeList);
            if (infoList != null && infoList.Count > 0)
            {
                foreach (var item in infoList)
                {
                    if (item.Password == TempPassword)
                    {
                        giftCard = item;
                    }
                }
            }
            return giftCard;
        }

        /// <summary>
        /// 拆分订单，礼品处理
        /// </summary>
        /// <param name="masterSOBaseInfo">主订单基本信息</param>
        /// <param name="subGiftCards">所有子订单对应的礼品卡拆分，key:子订单编号，value:拆分到子订单的礼品卡</param>
        internal static void SplitSOGiftCard(SOBaseInfo masterSOBaseInfo, Dictionary<int, List<ECCentral.BizEntity.IM.GiftCardRedeemLog>> subGiftCards)
        {
            List<ECCentral.BizEntity.IM.GiftCard> giftCardList = (from subSOGiftCard in subGiftCards
                                                                  from giftCard in subSOGiftCard.Value
                                                                  select new ECCentral.BizEntity.IM.GiftCard
                                                                  {
                                                                      Code = giftCard.Code,
                                                                      ConsumeAmount = giftCard.Amount.Value,
                                                                      ReferenceSOSysNo = subSOGiftCard.Key
                                                                  }).ToList();

            ObjectFactory<ECCentral.Service.IBizInteract.IIMBizInteract>.Instance.GiftCardSplitSO(masterSOBaseInfo.SysNo.Value, masterSOBaseInfo.CustomerSysNo.Value, giftCardList, masterSOBaseInfo.CompanyCode);
        }

        /// <summary>
        /// 取消拆分订单，礼品处理
        /// </summary>
        /// <param name="masterSOBaseInfo">主订单基本信息</param>
        /// <param name="subGiftCards">所有子订单对应的礼品卡拆分，key:子订单编号，value:拆分到子订单的礼品卡</param>
        internal static void CancelSplitSOGiftCard(SOBaseInfo masterSOBaseInfo, Dictionary<int, List<ECCentral.BizEntity.IM.GiftCardRedeemLog>> subGiftCards)
        {
            List<ECCentral.BizEntity.IM.GiftCard> giftCardList = (from subSOGiftCard in subGiftCards
                                                                  from giftCard in subSOGiftCard.Value
                                                                  select new ECCentral.BizEntity.IM.GiftCard
                                                                  {
                                                                      Code = giftCard.Code,
                                                                      ConsumeAmount = giftCard.Amount.Value,
                                                                      ReferenceSOSysNo = subSOGiftCard.Key
                                                                  }).ToList();
            ObjectFactory<ECCentral.Service.IBizInteract.IIMBizInteract>.Instance.GiftCardSplitSORollback(masterSOBaseInfo.SysNo.Value, masterSOBaseInfo.CustomerSysNo.Value, giftCardList, masterSOBaseInfo.CompanyCode);
        }

        /// <summary>
        /// 礼品卡扣减接口(改单）
        /// </summary>
        /// <param name="usedGiftCardList">所使用的礼品卡</param>
        /// <param name="companyCode">公司编号</param>
        /// <returns></returns>
        internal static void GiftCardDeduction(List<GiftCard> usedGiftCardList, string companyCode)
        {
            ObjectFactory<IIMBizInteract>.Instance.GiftCardDeduction(usedGiftCardList, companyCode);
        }

        /// <summary>
        /// 礼品卡使用接口(创建订单使用）
        /// </summary>
        /// <param name="giftCardPay">订单中使用礼品卡的金额(在SO中已经计算好了)</param>
        /// <param name="usedGiftCardList">礼品卡支付 涉及到的礼品卡</param>
        /// <param name="companyCode">公司编号</param>
        internal static void GiftCardConsumeForSOCreate(decimal giftCardPay, List<GiftCardRedeemLog> usedGiftCardList, string companyCode)
        {
            ObjectFactory<IIMBizInteract>.Instance.GiftCardConsumeForSOCreate(giftCardPay, usedGiftCardList, companyCode);
        }

        /// <summary>
        /// 礼品卡使用接口(更新订单使用 更新更新礼品卡使用方式为 先作废礼品卡使用  在创建礼品卡使用）
        /// </summary>
        /// <param name="giftCardPay">订单中使用礼品卡的金额(在SO中已经计算好了)</param>
        /// <param name="usedGiftCardList">礼品卡支付 涉及到的礼品卡</param>
        /// <param name="companyCode">公司编号</param>
        internal static void GiftCardVoidForSOUpdate(decimal giftCardPay, List<GiftCardRedeemLog> usedGiftCardList, string companyCode)
        {
            ObjectFactory<IIMBizInteract>.Instance.GiftCardVoidForSOUpdate(giftCardPay, usedGiftCardList, companyCode);
        }

        internal static CategoryInfo GetCategory3Info(int c3SysNo)
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetCategory3Info(c3SysNo);
        }

        internal static CategoryInfo GetCategory2Info(int c2SysNo)
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetCategory2Info(c2SysNo);
        }

        internal static CategoryInfo GetCategory1Info(int c1SysNo)
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetCategory1Info(c1SysNo);
        }

        /// <summary>
        /// 计算商品关税
        /// </summary>
        /// <param name="productSysNoList">商品编号列表</param>
        /// <returns>List&lt;商品编号,关税&gt;</returns>
        internal static List<KeyValuePair<int, decimal>> GetProductTax(List<int> productSysNoList)
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetProductTax(productSysNoList);
        }


        /// <summary>
        /// 取得商品入境设置
        /// </summary>
        /// <param name="productSysNoList">商品编号列表</param>
        /// <returns></returns>
        internal static List<ProductEntryInfo> GetProductEntryInfoListByProductSysNoList(List<int> productSysNoList)
        {
            return ObjectFactory<IIMBizInteract>.Instance.GetProductEntryInfoListByProductSysNoList(productSysNoList);
        }

        #endregion IM Domain

        #region Inventory Domain

        private static IInventoryBizInteract _inventoryBizInteract;
        private static IInventoryBizInteract InventoryBizInteract
        {
            get
            {
                _inventoryBizInteract = _inventoryBizInteract ?? ObjectFactory<IInventoryBizInteract>.Instance;
                return _inventoryBizInteract;
            }
        }

        /// <summary>
        /// 调整库存
        /// </summary>
        /// <param name="inventoryAdjustContractInfo"></param>
        /// <returns></returns>
        internal static bool AdjustProductInventory(InventoryAdjustContractInfo inventoryAdjustContractInfo)
        {
            return InventoryBizInteract.AdjustProductInventory(inventoryAdjustContractInfo);
        }

        /// <summary>
        /// 根据仓库编号获取仓库信息 Ray.L.Xing  泰隆优选定制化 ：泰隆优选不存在多渠道
        /// </summary>
        /// <param name="warehouseNo">泰隆优选仓库编号</param>
        /// <returns></returns>
        public static WarehouseInfo GetWarehouseInfo(int stockSysNo)
        {
            return InventoryBizInteract.GetWarehouseInfoBySysNo(stockSysNo);
        }

        /// <summary>
        /// 获取仓库列表   Ray.L.Xing  泰隆优选定制化 ：泰隆优选不存在多渠道
        /// </summary>
        /// <returns></returns>
        public static List<WarehouseInfo> GetWarehouseList(string companyCode)
        {
            return InventoryBizInteract.GetWarehouseList(companyCode); ;
        }

        //public static StockInfo GetStockInfo(int stockSysNo)
        //{
        //    return InventoryBizInteract.GetStockInfo(stockSysNo);
        //}
        //public static List<StockInfo> GetStockList(string webChannelID)
        //{
        //    return InventoryBizInteract.GetStockList(webChannelID); ;
        //}
        /// <summary>
        /// 根据商品系统编号List 获取 商品总库存
        /// </summary>
        /// <param name="productIDList">商品系统编号List</param>
        /// <returns></returns>
        public static List<ProductInventoryInfo> GetProductTotalInventoryInfoByProductList(List<int> productIDList)
        {
            return InventoryBizInteract.GetProductTotalInventoryInfoByProductList(productIDList);
        }

        //获取分仓商品库存
        /// <summary>
        /// 获取分仓商品库存
        /// </summary>
        /// <param name="productSysNo">商品编号</param>
        /// <param name="stockSysNo">仓库编号</param>
        /// <returns>单个商品指定仓库库存</returns>
        public static ProductInventoryInfo GetInventoryInfoByProductSysNo(int productSysNo, int stockSysNo)
        {
            return InventoryBizInteract.GetProductInventoryInfoByStock(productSysNo, stockSysNo);
        }

        /// <summary>
        /// 获取本地仓库编码
        /// </summary>
        /// <param name="areaSysNo">地区编码</param>
        /// <param name="companyCode">公司编码</param>
        /// <returns>本地仓库编码</returns>
        public static string GetLocalWarehouseNumber(int areaSysNo, string companyCode)
        {
            return InventoryBizInteract.GetLocalWarehouseNumber(areaSysNo, companyCode);
        }

        #endregion Inventory Domain

        #region Common Domain

        private static ICommonBizInteract _commonBizInteract;
        private static ICommonBizInteract CommonBizInteract
        {
            get
            {
                _commonBizInteract = _commonBizInteract ?? ObjectFactory<ICommonBizInteract>.Instance;
                return _commonBizInteract;
            }
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
            CommonBizInteract.CreateOperationLog(note, logType, ticketSysNo, companyCode);
        }

        /// <summary>
        /// 根据县区编号取得地区信息
        /// </summary>
        /// <param name="districtSysNo"></param>
        /// <returns></returns>
        public static ECCentral.BizEntity.Common.AreaInfo GetAreaInfoByDistrictSysNo(int districtSysNo)
        {
            return CommonBizInteract.GetAreaInfo(districtSysNo);
        }

        /// <summary>
        /// 取得订单支付类型信息
        /// </summary>
        /// <param name="payTypeSysNo"></param>
        /// <returns></returns>
        public static ECCentral.BizEntity.Common.PayType GetPayTypeBySysNo(int payTypeSysNo)
        {
            return CommonBizInteract.GetPayType(payTypeSysNo);//IPP3:CommonDA.GetPayTypeBySysNo(soInfo.SOMaster.PayTypeSysNo, soInfo.MessageHeaderInfo.CompanyCode);
        }

        internal static string GetReasonCodePath(int reasonCodeSysNo, string companyCode)
        {
            return CommonBizInteract.GetReasonCodePath(reasonCodeSysNo, companyCode);
        }

        internal static List<PayType> GetPayTypeList(string companyCode)
        {
            return CommonBizInteract.GetPayTypeList(companyCode);
        }

        /// <summary>
        /// 取得订单支付类型信息
        /// </summary>
        /// <param name="payTypeSysNo"></param>
        /// <returns></returns>
        public static ECCentral.BizEntity.Common.ShippingType GetShippingTypeBySysNo(int shippingTypeSysNo)
        {
            return CommonBizInteract.GetShippingType(shippingTypeSysNo);//IPP3:CommonDA.GetPayTypeBySysNo(soInfo.SOMaster.PayTypeSysNo, soInfo.MessageHeaderInfo.CompanyCode);
        }

        /// <summary>
        /// 获取专用配送方式 禁运规则
        /// </summary>
        /// <param name="shipTypeSysNo">配送方式编号</param>
        /// <param name="companyCode">公司编号</param>
        /// <returns></returns>
        internal static List<ItemShipRuleInfo> GetSpecialItemShipRule(int shipTypeSysNo, string companyCode)
        {
            return CommonBizInteract.GetSpecialItemShipRule(shipTypeSysNo, companyCode);
        }

        public static UserInfo GetUserInfoBySysNo(int userSysNo)
        {
            return CommonBizInteract.GetUserInfoBySysNo(userSysNo);
        }

        public static string GetUniqueUserName(int userSysNo)
        {
            return CommonBizInteract.GetUniqueUserName(userSysNo);
        }

        public static List<ShipTypeAreaUnInfo> GetShipTypeAreaUnList(string companyCode)
        {
            return CommonBizInteract.GetShipTypeAreaUnList(companyCode);
        }

        /// <summary>
        /// 获取商品配送规则
        /// </summary>
        /// <param name="c3SysNoStr">商品3级分类编号</param>
        /// <param name="productSysNoStr">商品编号序列</param>
        /// <param name="provinceSysNo">省编号</param>
        /// <param name="citySysNo">市编号</param>
        /// <param name="areaSysno">区编号</param>
        /// <param name="companyCode">公司编号</param>
        /// <returns></returns>
        internal static List<ItemShipRuleInfo> GetItemShipRuleList(string c3SysNoStr, string productSysNoStr, int? provinceSysNo, int? citySysNo, int? areaSysno, string companyCode)
        {
            return CommonBizInteract.GetItemShipRuleList(c3SysNoStr, productSysNoStr, provinceSysNo, citySysNo, areaSysno, companyCode);
        }

        /// <summary>
        /// 获取节假日信息
        /// </summary>
        /// <param name="blockedService">区域服务标记</param>
        /// <param name="companyCode">公司编码</param>
        /// <returns>节假日时间列表</returns>
        internal static List<DateTime> GetHolidayList(string blockedService, string companyCode)
        {
            return CommonBizInteract.GetHolidayList(blockedService, companyCode);
        }

        internal static List<Holiday> GetHolidayAfterToday(string companyCode)
        {
            return CommonBizInteract.GetAllHolidaysAfterToday(companyCode);
        }

        #region 取得系统配置

        /// <summary>
        /// 取得系统配置
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public static string GetSystemConfigurationValue(string key, string companyCode)
        {
            return CommonBizInteract.GetSystemConfigurationValue(key, companyCode);
        }

        /// <summary>
        /// 取得邮政自提的运送方式系统编号
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        internal static int ChinaPostShipTypeID(string companyCode)
        {
            string shipTypeID = GetSystemConfigurationValue("ChinaPostShipTypeID", companyCode);
            int id = int.TryParse(shipTypeID, out id) ? id : 0;
            return id;
        }

        /// <summary>
        /// 记录biz日志
        /// </summary>
        /// <param name="note">日志内容</param>
        /// <param name="logType">日志类型</param>
        /// <param name="ticketSysNo">单据号</param>
        /// <param name="companyCode">公司编码</param>
        /// <returns>影响行数</returns>
        internal static int WriteBizLog(string note, BizLogType logType, int ticketSysNo, string companyCode)
        {
            return CommonBizInteract.CreateOperationLog(note, logType, ticketSysNo, companyCode);
        }

        #endregion 取得系统配置

        #endregion Common Domain

        #region PO Domain
        /// <summary>
        /// 批量创建代销转库存记录
        /// </summary>
        /// <param name="consignToAcctLogInfos"></param>
        public static void BatchCreateConsignToAcctLogsInventory(List<ConsignToAcctLogInfo> consignToAcctLogInfos)
        {
            ObjectFactory<IPOBizInteract>.Instance.BatchCreateConsignToAcctLogsInventory(consignToAcctLogInfos);
        }
        #endregion
    }

}
