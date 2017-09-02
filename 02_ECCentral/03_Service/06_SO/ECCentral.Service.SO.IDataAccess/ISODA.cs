using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.SO.IDataAccess
{
    public partial interface ISODA
    {

        #region 生成新的订单号
        int NewSOSysNo();
        #endregion

        #region 订单查询相关方法

        /// <summary>
        /// 根据订单系统编号取得订单信息
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <returns></returns>
        SOInfo GetSOBySOSysNo(int soSysNo);

        /// <summary>
        /// 根据子订单系统编号取得主订单信息
        /// </summary>
        /// <param name="subSOSysNo">子订单系统编号</param>
        /// <returns></returns>
        SOInfo GetMasterSOBySubSOSysNo(int subSOSysNo);

        /// <summary>
        /// 取得订单基本信息
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        SOBaseInfo GetSOBaseInfoBySOSysNo(int soSysNo);

        /// <summary>
        /// 取得订单商品信息
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        List<SOItemInfo> GetSOItemsBySOSysNo(int soSysNo);

        /// <summary>
        /// 根据主订单编号取子订单列表
        /// </summary>
        /// <param name="masterSOSysNo"></param>
        /// <returns></returns>
        List<SOInfo> GetSubSOByMasterSOSysNo(int masterSOSysNo);

        /// <summary>
        /// 根据订单编写列表取得多个订单
        /// </summary>
        /// <param name="soSysNos">订单编号列表</param>
        /// <returns></returns>
        List<SOInfo> GetSOBySOSysNoList(List<int> soSysNos);

        /// <summary>
        /// 根据订单编写列表取得多个订单，但取出的不是订单的所有信息
        /// </summary>
        /// <param name="soSysNos">订单编号列表</param>
        /// <returns></returns>
        List<SOInfo> GetSimpleSOInfoList(List<int> soSysNos);

        /// <summary>
        /// 根据订单编写列表取得多个订单的基本信息
        /// </summary>
        /// <param name="soSysNos">订单编号列表</param>
        /// <returns></returns>
        List<SOBaseInfo> GetSOBaseInfoBySOSysNoList(List<int> soSysNos);

        /// <summary>
        /// 根据客户编号获取订单对应的增值税发票
        /// </summary>
        /// <param name="customerSysNo">客户编号</param>
        /// <returns></returns>
        List<SOVATInvoiceInfo> GetSOVATInvoiceInfoByCustomerSysNo(int customerSysNo);

        /// <summary>
        /// 根据客户编号获取客户对应的礼品卡信息
        /// </summary>
        /// <param name="customerSysNo">客户编号</param>
        /// <returns></returns>
        List<GiftCardInfo> QueryGiftCardListInfoByCustomerSysNo(int customerSysNo);

        /// <summary>
        /// 根据优惠券编号，取得使用此优惠券的订单编号
        /// </summary>
        /// <param name="couponSysNo"></param>
        /// <returns></returns>
        int? GetSOSysNoByCouponSysNo(int couponSysNo);

        /// <summary>
        /// 根据订单获取证件姓名
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <returns>证件姓名</returns>
        string GetCertificaterNameBySOSysNo(int soSysNo);

        /// <summary>
        /// 根据订单编号获取VATInvoice信息
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <returns>VATInvoice信息</returns>
        SOVATInvoiceInfo GetSOVATInvoiceInfoBySoSysNo(int soSysNo);

        string GetSysConfigurationValue(string key, string companyCode);

        List<string> TrackingNumberBySoSysno(int sosysno);

        //string GetItemsSHDSysNo(int productSysno, int soSysno);

        #endregion

        #region  订单创建相关方法

        /// <summary>
        /// 将订单主要信息插入数据库，为表IPP3.dbo.SO_Master和IPP3.dbo.SO_CheckShipping插入数据。
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        SOInfo InsertSOMainInfo(SOInfo soInfo);

        void InsertSOCheckShippingInfo(SOInfo soInfo);

        /// <summary>
        /// 分仓逻辑中： 往 Shopping.dbo.ShoppingMaster 里添加数据
        /// </summary>
        /// <param name="info"></param>
        int InsertShoppingMaster(SOInfo info);

        /// <summary>
        /// 分仓逻辑中： 往ShoppingTransaction里插入数据
        /// </summary>
        /// <param name="itemInfo"></param>
        /// <param name="shoppingMasterSysNo"></param>
        /// <param name="CompanyCode"></param>  
        void InsertShoppingTransaction(SOItemInfo itemInfo, int shoppingMasterSysNo, string CompanyCode);

        /// <summary>
        /// 调用存储过程进行分仓
        /// </summary>
        /// <param name="shoppingMasterSysNo"></param>
        /// <param name="areaSysNo"></param>
        /// <param name="shipTypeSysNo"></param>
        /// <param name="status"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        List<SOItemInfo> AllocateStock(int shoppingMasterSysNo, int areaSysNo, int shipTypeSysNo, string companyCode, out AllocateInventoryStatus status);

        /// <summary>
        /// 添加订单主商品
        /// </summary>
        /// <param name="?"></param>
        void InsertSOItemInfo(SOInfo info);

        /// <summary>
        /// 添加订单参与的促销规则
        /// </summary>
        /// <param name="promotionInfo"></param>
        void InsertSOPromotionInfo(SOPromotionInfo promotionInfo, string companyCode);

        /// <summary>
        /// 添加商品扩展信息
        /// </summary>
        /// <param name="soItemInfo"></param>
        /// <param name="companyCode"></param>
        /// <param name="InUser"></param>
        void InsertItemExtend(SOItemInfo soItemInfo, string companyCode, string InUser);

        /// <summary>
        /// 添加毛利分配信息
        /// </summary>
        /// <param name="info"></param>
        void InsertSOItemGossProfit(ItemGrossProfitInfo info);

        /// <summary>
        /// 从数据库删除订单商品信息
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <param name="companyCode">公司编号</param>
        void DeleteSOItem(int soSysNo, string companyCode);

        /// <summary>
        /// 从数据库删除订单商品信息
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        void DeleteSOItemExtend(int soSysNo);

        /// <summary>
        /// 删除主单中对应的Item
        /// </summary>
        /// <param name="SOSysNo"> 主单编号</param>
        /// <param name="ProductSysNo">商品编号</param>
        /// <param name="companyCode">公司编号</param>
        void DeleteSOItemByProductSysNo(int soSysNo, int productSysNo, string companyCode);

        /// <summary>
        /// 更新SOCheckShipping信息
        /// </summary>
        /// <param name="?"></param>
        void UpdateSOCheckShipping(SOInfo soInfo);

        /// <summary>
        /// 设置订单拆分类型
        /// </summary>
        /// <param name="?"></param>
        void SetSoSplitType(SOInfo soInfo);

        /// <summary>
        /// 删除附件
        /// </summary>
        /// <param name="SOSysNo"></param>
        void DeleteSOAccessory(int SOSysNo);

        /// <summary>
        ///是否是 同城发货
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        bool IsSameCityStock(int soSysNo, string companyCode);

        /// <summary>
        /// 存在商品接受预定
        /// </summary>
        /// <param name="?"></param>
        bool ExistEngageItem(int soSysNo, string companyCode);

        /// <summary>
        /// 是否指定仓发货
        /// </summary>
        /// <param name="ShipTypeSysNo">配送方式编号</param>
        /// <returns></returns>
        bool IsOnlyForStockShipType(int ShipTypeSysNo);

        /// <summary>
        /// 计算运费
        /// </summary>
        /// <param name="soTotalWeight">订单重量</param>
        /// <param name="SOAmount">订单商品总金额</param>
        /// <param name="ShipTypeSysNo">配送方式编号</param>
        /// <param name="AreaSysNo">收货区域编号</param>
        /// <param name="CustomerSysNo">客户编号</param>
        /// <param name="soSingelMaxWeight">订单中单品最大重量</param>
        /// <param name="CompanyCode">公司编号</param>
        /// <returns></returns>
        decimal? CaclShipPrice(int soTotalWeight, decimal? soAmount, int? shipTypeSysNo, int? areaSysNo, int? customerSysNo, int soSingelMaxWeight, string companyCode);

        /// <summary>
        /// 持久化订单主信息
        /// </summary>
        /// <param name="soInfo"></param>
        /// <param name="isUpdate">是否是更新订单</param>
        void PersintenceMaster(SOInfo soInfo, bool isUpdate);

        /// <summary>
        /// 持久化订单商品信息
        /// </summary>
        /// <param name="soInfo"></param>
        /// <param name="isUpdate">是否是更新订单</param>
        void PersintenceItem(SOInfo soInfo, bool isUpdate);

        /// <summary>
        /// 持久化订单促销信息（赠品、附件、）
        /// </summary>
        /// <param name="soInfo"></param>
        /// <param name="isUpdate">是否是更新订单</param> 
        void PersintencePromotion(SOInfo soInfo, bool isUpdate);
        /// <summary>
        /// 持久化毛利信息
        /// </summary>
        /// <param name="soInfo"></param>
        /// <param name="isUpdate"></param>
        void PersintenceGrossProfit(SOInfo soInfo, bool isUpdate);
        /// <summary>
        /// 持久化订单礼品卡信息
        /// </summary>
        /// <param name="soInfo"></param>
        /// <param name="isUpdate">是否是更新订单</param> 
        void PersintenceGiftCard(SOInfo soInfo, bool isUpdate);

        /// <summary>
        /// 持久化订单其他相关信息
        /// </summary>
        /// <param name="soInfo"></param>
        /// <param name="isUpdate">是否是更新订单</param>
        void PersintenceExtend(SOInfo soInfo, bool isUpdate);

        /// <summary>
        /// 添加订单邮政自提信息
        /// </summary>
        /// <param name="postInfo">是否是更新订单</param>
        void InsertChinaPost(SOChinaPostInfo postInfo);

        /// <summary>
        ///  验证支付方式与配送方式是否匹配
        /// </summary>
        /// <param name="payTypeSysNo">支付方式编号</param>
        /// <param name="shipTypeSysNo">配送方式编号</param>
        /// <param name="companyCode">公司编号</param>
        /// <returns></returns>
        bool CheckPayTypeByShipType(int payTypeSysNo, int shipTypeSysNo, string companyCode);

        int ChangedGossLogStatus(int soSysNo, string status, string editUser);

        int UpdateSONote(int soSysNo, string note);

        List<ItemExtension> GetSOItemExtensionBySOSysNo(int soSysNo);

        int InsertSOItemExtension(ItemExtension entity);

        /// <summary>
        /// 判断是否可用帐期支付
        /// </summary>
        /// <param name="customerSysNo">顾客编号</param>
        /// <param name="payDays">帐期天数</param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        bool IsAcctConfirmedSO(int customerSysNo, int payDays, string companyCode);
        #endregion

        #region 修改订单状态相关

        /// <summary>
        /// 修改订单状态
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        bool UpdateSOStatus(SOStatusChangeInfo info);

        /// <summary>
        /// 审核订单
        /// </summary>
        /// <param name="info">订单状态的更改信息</param>
        /// <returns>是否操作成功</returns>
        bool UpdateSOStatusForAudit(SOStatusChangeInfo info);

        /// <summary>
        /// 主管审核订单
        /// </summary>
        /// <param name="info">订单状态的更改信息</param>
        /// <returns>是否操作成功</returns>
        bool UpdateSOStatusForManagerAudit(SOStatusChangeInfo info);

        /// <summary>
        /// 拆分订单修改主订单状态为已拆分
        /// </summary>
        /// <param name="info"></param>
        bool UpdateSOStatusToSplit(SOStatusChangeInfo info);

        /// <summary>
        /// 修改订单状态为物流拒收
        /// </summary>
        /// <param name="info"></param>
        bool UpdateSOStatusToReject(SOStatusChangeInfo info);

        /// <summary>
        /// 取消审核
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        bool UpdateSOStatusToOrigin(SOStatusChangeInfo info);

        /// <summary>
        /// 订单出库
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        bool UpdateSOStatusToOutStock(SOStatusChangeInfo info);

        /// <summary>
        /// 作废订单，使用场景：作废订单
        /// </summary>
        bool UpdateSOForAbandon(SOInfo soInfo);

        /// <summary>
        /// 修改订单锁单信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        bool UpdateSOHoldInfo(SOBaseInfo info);

        /// <summary>
        /// 修改订单属性表示订单已开具增值税发票
        /// </summary>
        /// <param name="soSysNos"></param>
        void UpdateSOVATPrinted(List<int> soSysNos);

        /// <summary>
        /// 更改订单是否是并单属性
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        void UpdateSOCombineInfo(int soSysNo);

        /// <summary>
        /// 更新订单出库时间
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <returns></returns>
        void UpdateSOOutStockTime(int soSysNo);

        /// <summary>
        /// 订单拆分发票修改订单信息
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        void UpdateSOForSplitInvoice(int soSysNo, bool isMultiInvoice);

        /// <summary>
        /// 更改订单虚库采购单状态 
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        void UpdateSOCheckShippingVPOStatus(int soSysNo, BizEntity.PO.VirtualPurchaseOrderStatus vpoStatus);

        /// <summary>
        /// 更新订单客户对应的增值税信息,如果有就更新，没有就添加记录
        /// </summary>
        /// <param name="entity"></param>
        void UpdateSOVATInvoice(SOVATInvoiceInfo entity);

        /// <summary>
        /// 判断仓库是否下载了订单
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        bool WMSIsDownloadSO(int soSysNo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        int GetShoppingCartSysNoBySOSysNo(int soSysNo);

        /// <summary>
        /// 根据订单编号和商品编号删除订单商品
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <param name="productSysNo">商品编号</param>
        void DeleteSOItemBySOSysNoAndProductSysNo(int soSysNo, int productSysNo);

        /// <summary>
        /// 根据订单编号取得移仓单编号
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <returns></returns>
        int GetShiftSysNoBySOSysNo(int soSysNo);

        /// <summary>
        /// 修改订单主信息
        /// </summary>
        /// <param name="info"></param>
        void UpdateSOMaster(SOInfo info);

        /// <summary>
        /// 修改订单金额信息，使用场景：作废订单
        /// </summary>
        /// <param name="soBaseInfo"></param>
        void UpdateSOAmountInfo(SOBaseInfo soBaseInfo);

        /// <summary>
        /// 根据sysNo删除item
        /// </summary>
        /// <param name="sysNo"></param>
        void DeleteSOItemBySysNo(int sysNo);

        /// <summary>
        /// 修改订单商品的价格信息，使用场景：作废订单
        /// </summary>
        /// <param name="info"></param>
        void UpdateSOItemAmountInfo(SOItemInfo info);

        /// <summary>
        /// 取消信息延保，使用场景：作废订单
        /// </summary>
        /// <param name="soSysNo"></param>
        void CancelSOExtendWarranty(int soSysNo);

        /// <summary>
        /// 检查此订单是不是客户第一次购买成功的订单。
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <param name="customerSysNo">客户编号</param>
        /// <returns></returns>
        bool IsFirstSO(int soSysNo, int customerSysNo);


        /// <summary>
        /// 手动更改订单仓库
        /// </summary>
        /// <param name="info"></param>
        bool WHUpdateStock(SOWHUpdateInfo info);

        #endregion

        #region 订单虚库相关
        /// <summary>
        /// 检查订单是否存在虚库申请
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        bool IsExistVirtualItemRequest(int soSysNo);

        /// <summary>
        /// 如果有没有作废的虚库申请则作废订单虚库申请
        /// </summary>
        /// <param name="soSysNo"></param>
        void AbandonSOVirtualItemRequest(int soSysNo);
        #endregion

        #region 虚库采购单相关
        /// <summary>
        /// 查询当前itemSysNo已经创建的虚库采购单条数
        /// </summary>
        /// <param name="soItemSysNo"></param>
        /// <returns></returns>
        int GetGeneratedSOVirtualCount(int soItemSysNo);
        #endregion

        #region Newegg数据库 相关的方法
        /// <summary>
        /// 订单是否出库 
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <returns></returns>
        bool IsNeweggOutStock(int soSysNo);
        #endregion

        #region 获取商品推荐信息
        /// <summary>
        /// 获取订单相关商品推荐信息
        /// </summary>
        /// <param name="soSysNo">订单号</param>
        /// <returns></returns>
        List<CommendatoryProductsInfo> GetCommendatoryProducts(int soSysNo);
        #endregion

        #region 团购相关

        /// <summary>
        /// 获取全部团购成功的商品信息
        /// </summary>
        /// <param name="referenceNoArr"></param>
        /// <returns></returns>
        List<ProductGroupBuyInfo> GetAllGoupMaxPreOrder(int[] referenceNoArr);

        #endregion


        StockInfo GetProductStockByProductSysNoAndQty(int productSysNo, int qty);

        bool UpdateSOStatusToReportedFailure(int sosysno);

        /// <summary>
        /// 修改订单状态为已报关
        /// </summary>
        /// <param name="sosysno"></param>
        /// <returns></returns>
        bool UpdateSOStatusToReported(int sosysno);

        /// <summary>
        /// 修改订单SO_CheckShipping的LastChangeStatusDate
        /// </summary>
        /// <param name="sosysno"></param>
        /// <returns></returns>
        bool UpdateSOCheckShippingLastChangeStatusDate(int sosysno);

        /// <summary>
        /// 获取查询物流信息的订单
        /// </summary>
        /// <returns></returns>
        List<string> GetWaitingFinishShippingList(ExpressType expressType);

        /// <summary>
        /// 获取快递100查询物流信息的订单
        /// </summary>
        /// <returns></returns>
        List<string> GetWaitingFinishShippingListForKD100(ExpressType expressType);

        /// <summary>
        /// 根据运单号查询订单号
        /// </summary>
        /// <param name="trackingNumber">运单号</param>
        /// <returns></returns>
        int GetSOSysNoByTrackingNumber(string trackingNumber);

        #region 订单申报相关

        /// <summary>
        /// 获取待申报的订单
        /// </summary>
        /// <returns></returns>
        List<WaitDeclareSO> GetWaitDeclareSO();

        /// <summary>
        /// 创建订单申报记录编号
        /// </summary>
        /// <returns></returns>
        int CreateSODeclareRecordsSysNo();

        /// <summary>
        /// 创建订单申报记录
        /// </summary>
        /// <param name="entity">申报记录</param>
        /// <returns></returns>
        void CreateSODeclareRecords(SODeclareRecords entity);

        /// <summary>
        /// 申报时获取订单信息
        /// </summary>
        /// <param name="SOSysNo">订单号</param>
        /// <returns></returns>
        DeclareOrderInfo DeclareGetOrderInfoBySOSysNo(int SOSysNo);
        /// <summary>
        /// 申报时获取订单支付信息
        /// </summary>
        /// <param name="SOSysNo">订单号</param>
        /// <returns></returns>
        SOPaymentDeclare DeclareGetPaymentInfoBySOSysNo(int SOSysNo);
        /// <summary>
        /// 更新订单支付申报状态
        /// </summary>
        /// <param name="SOSysNo">订单号</param>
        /// <param name="declareTrackingNumber">申报编号</param>
        /// <param name="declareStatus">申报状态</param>
        void DeclareUpdatePaymentDeclareInfo(int SOSysNo, string declareTrackingNumber, int? declareStatus);
        #endregion

        /// <summary>
        /// 修改订单状态为 申报失败订单作废
        /// </summary>
        /// <param name="sosysno"></param>
        /// <returns></returns>
        bool UpdateSOStatusToCustomsPass(int sosysno);
        /// <summary>
        /// 根据订单编号获取关务对接相关信息
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        VendorCustomsInfo LoadVendorCustomsInfo(int soSysNo);
        /// <summary>
        /// 根据商家编号获取关务对接相关信息
        /// </summary>
        /// <param name="merchantSysNo"></param>
        /// <returns></returns>
        VendorCustomsInfo LoadVendorCustomsInfoByMerchant(int merchantSysNo);

        void SOMaintainUpdateNote(SOInfo info);

        /// <summary>
        /// 是否归还库存
        /// </summary>
        /// <param name="productSysNo">商品系统编号</param>
        /// <param name="soCreatDate">订单创建时间</param>
        /// <returns>true:归还</returns>
        bool CheckReturnInventory(int productSysNo, DateTime soCreatDate);

        /// <summary>
        /// 根据订单号查询Netpay
        /// </summary>
        /// <param name="soSysNo">订单号</param>
        /// <returns></returns>
        ECCentral.BizEntity.Invoice.NetPayInfo GetCenterDBNetpayBySOSysNo(int soSysNo);
    }
}
