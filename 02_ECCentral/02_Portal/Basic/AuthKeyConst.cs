using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Core.Components;

namespace ECCentral.Portal.Basic
{
    public class AuthKey
    {
        public AuthKey(string key)
        {
            if (key == null || key.Trim().Length <= 0)
            {
                throw new Exception("The auth key cannot be null or empty!");
            }
            Key = key;
        }

        public string Key
        {
            get;
            private set;
        }
    }

    /// <summary>
    /// 存放权限控制Function Key
    /// </summary>
    public static partial class AuthKeyConst
    {
        /* * 说明：
         * 每个Const常量的命名规则为: Domain名_功能名_权限点
         * 比如：Customer_Customer_Edit，表示Customer Domain中，对Customer信息的Edit权限
         * IM_Item_Create，表示IM中，对Item的创建权限
         * */

        #region Customer Domain

        #region 顾客管理

        #region [顾客管理]
        public readonly static AuthKey Customer_CustomerQuery_CreateSO = new AuthKey("CreateSO");
        public readonly static AuthKey Customer_CustomerQuery_Add = new AuthKey("Add");
        public readonly static AuthKey Customer_CustomerQuery_Edit = new AuthKey("Edit");
        public readonly static AuthKey Customer_CustomerQuery_SetCustomerRights = new AuthKey("SetCustomerRights");
        public readonly static AuthKey Customer_CustomerQuery_BatchActiveImg = new AuthKey("BatchActiveImg");
        public readonly static AuthKey Customer_CustomerQuery_BatchInActiveImg = new AuthKey("BatchInActiveImg");
        public readonly static AuthKey Customer_CustomerQuery_BatchImportCustomer = new AuthKey("BatchImportCustomer");
        public readonly static AuthKey Customer_CustomerQuery_PointLog = new AuthKey("PointLog");
        public readonly static AuthKey Customer_CustomerQuery_ExperienceLog = new AuthKey("ExperienceLog");
        public readonly static AuthKey Customer_CustomerQuery_PromotionLog = new AuthKey("PromotionLog");
        public readonly static AuthKey Customer_CustomerQuery_ViewSecurityQuestion = new AuthKey("ViewSecurityQuestion");
        #endregion

        #region [客户维护]
        public readonly static AuthKey Customer_CustomerMaintain_Add = new AuthKey("Add");
        public readonly static AuthKey Customer_CustomerMaintain_Save = new AuthKey("Save");
        public readonly static AuthKey Customer_CustomerMaintain_AbandonCustomer = new AuthKey("AbandonCustomer");
        public readonly static AuthKey Customer_CustomerMaintain_AddPoint = new AuthKey("AddPoint");
        public readonly static AuthKey Customer_CustomerMaintain_SetCustomerRights = new AuthKey("SetCustomerRights");
        public readonly static AuthKey Customer_CustomerMaintain_ViewPointHistory = new AuthKey("ViewPointHistory");
        public readonly static AuthKey Customer_CustomerMaintain_ViewBadUserHistory = new AuthKey("ViewBadUserHistory");
        public readonly static AuthKey Customer_CustomerMaintain_ViewCouponHistory = new AuthKey("ViewCouponHistory");
        public readonly static AuthKey Customer_CustomerMaintain_CancelConfirmEmail = new AuthKey("CancelConfirmEmail");
        public readonly static AuthKey Customer_CustomerMaintain_CancelConfirmPhone = new AuthKey("CancelConfirmPhone");
        #endregion

        #region [顾客回访管理]
        public readonly static AuthKey Customer_CustomerVist_Export = new AuthKey("Export");
        public readonly static AuthKey Customer_CustomerVist_WaitVisit = new AuthKey("WaitVisit");
        public readonly static AuthKey Customer_CustomerVist_WaitMaintain = new AuthKey("WaitMaintain");
        #endregion

        #region [顾客来电记录查询]
        public readonly static AuthKey Customer_CustomerCalling_Add = new AuthKey("Add");
        public readonly static AuthKey Customer_CustomerCallingList_ReOpen = new AuthKey("ListReOpen");
        public readonly static AuthKey Customer_CustomerCallingList_Open = new AuthKey("ListOpen");
        public readonly static AuthKey Customer_CustomerCallingList_ToComplain = new AuthKey("ListToComplain");
        public readonly static AuthKey Customer_CustomerCallingList_Close = new AuthKey("ListClose");
        public readonly static AuthKey Customer_CustomerCallingList_ToRMA = new AuthKey("ListToRMA");
        public readonly static AuthKey Customer_CustomerComplain_Open = new AuthKey("ComplainOpen");
        public readonly static AuthKey Customer_CustomerRMA_Open = new AuthKey("RMAOpen");
        #endregion

        #region 退款申请管理
        public readonly static AuthKey Customer_RefundRequest_Audit = new AuthKey("Audit");
        public readonly static AuthKey Customer_RefundRequest_RefuseAudit = new AuthKey("RefuseAudit");
        #endregion

        #region 补偿退款单
        public readonly static AuthKey Customer_RefundAdjust_Add = new AuthKey("Add");
        public readonly static AuthKey Customer_RefundAdjust_Audit = new AuthKey("Audit");
        public readonly static AuthKey Customer_RefundAdjust_Refund = new AuthKey("Refund");
        public readonly static AuthKey Customer_RefundAdjust_Void = new AuthKey("Void");
        public readonly static AuthKey Customer_RefundAdjust_Edit = new AuthKey("Edit");
        public readonly static AuthKey Customer_EnergySubsidy_Export = new AuthKey("Export");
        #endregion

        #endregion

        #region 信息管理

        #region [邮件发送]
        public readonly static AuthKey Customer_InfoMgmt_SendMail = new AuthKey("SendMail");
        #endregion

        #region [配送类型-短信内容管理]
        public readonly static AuthKey Customer_ShipTypeSMS_Add = new AuthKey("Add");
        public readonly static AuthKey Customer_ShipTypeSMS_Edit = new AuthKey("Edit");
        #endregion

        #region [发送短信]
        public readonly static AuthKey Customer_SMS_SendSMS = new AuthKey("SendSMS");

        #endregion

        #endregion

        #region CS工具箱

        #region [QC评分类目管理]
        public readonly static AuthKey Customer_QSubject_Add = new AuthKey("Add");
        public readonly static AuthKey Customer_Qsubject_Edit = new AuthKey("Edit");
        #endregion

        #region [FP状态维护]
        public readonly static AuthKey Customer_FPCheck_Save = new AuthKey("Save");
        public readonly static AuthKey Customer_CHSet_Add = new AuthKey("Add");
        public readonly static AuthKey Customer_CHSet_Active = new AuthKey("Active");
        public readonly static AuthKey Customer_CCSet_Save = new AuthKey("Save");
        #endregion

        #region [自动审单维护]

        public readonly static AuthKey Customer_OrderCheck_Save = new AuthKey("Save");

        #region 关键字
        public readonly static AuthKey Customer_OrderCheck_Keywords_Add = new AuthKey("Add");
        public readonly static AuthKey Customer_OrderCheck_Keywords_Active = new AuthKey("Active");
        #endregion

        #region 配送方式
        public readonly static AuthKey Customer_OrderCheck_ShipType_Save = new AuthKey("Save");
        #endregion

        #region 支付方式
        public readonly static AuthKey Customer_OrderCheck_PayType_Save = new AuthKey("Save");
        #endregion

        #region 产品类别及产品
        public readonly static AuthKey Customer_OrderCheck_ProductAnd3C_Add = new AuthKey("Add");
        public readonly static AuthKey Customer_OrderCheck_ProductAnd3C_Active = new AuthKey("Active");
        #endregion

        #region FP状态
        public readonly static AuthKey Customer_OrderCheck_FP_Save = new AuthKey("Save");
        #endregion

        #region 客户类型
        public readonly static AuthKey Customer_OrderCheck_CustomerType_Save = new AuthKey("Save");
        #endregion

        #region 订单金额
        public readonly static AuthKey Customer_OrderCheck_AMT_Add = new AuthKey("Add");
        public readonly static AuthKey Customer_OrderCheck_AMT_Active = new AuthKey("Active");
        #endregion

        #region 自动审核订单时间
        public readonly static AuthKey Customer_OrderCheck_AT_Add = new AuthKey("Add");
        public readonly static AuthKey Customer_OrderCheck_AT_Active = new AuthKey("Active");
        #endregion

        #region 配送服务类型
        public readonly static AuthKey Customer_OrderCheck_DistributionService_Add = new AuthKey("Add");
        public readonly static AuthKey Customer_OrderCheck_DistributionService_Edit = new AuthKey("Edit");
        public readonly static AuthKey Customer_OrderCheck_DistributionService_Active = new AuthKey("Active");
        #endregion

        #endregion

        #region [CS人员管理]
        public readonly static AuthKey Customer_CSSet_Add = new AuthKey("Add");
        public readonly static AuthKey Customer_CSSet_Edit = new AuthKey("Edit");
        #endregion

        #endregion

        #region 奖品管理
        public readonly static AuthKey Customer_Gift_Add = new AuthKey("Add");
        public readonly static AuthKey Customer_Gift_Abandon = new AuthKey("Abandon");
        public readonly static AuthKey Customer_Gift_CancelAbandon = new AuthKey("CancelAbandon");
        public readonly static AuthKey Customer_Gift_SendGiftMsg = new AuthKey("SendGiftMsg");
        public readonly static AuthKey Customer_Gift_SendGiftAwoke = new AuthKey("SendGiftAwoke");
        public readonly static AuthKey Customer_Gift_SendAbandonMsg = new AuthKey("SendAbandonMsg");
        #endregion

        #region 顾客加积分管理
        public readonly static AuthKey Customer_Points_Add = new AuthKey("Add");
        public readonly static AuthKey Customer_Points_Audit = new AuthKey("Audit");
        #endregion

        public readonly static AuthKey Customer_AgentInfo_Add = new AuthKey("NeweggAgentInfoAdd");
        public readonly static AuthKey Customer_AgentInfo_Edit = new AuthKey("NeweggAgentInfoEdit");
        public readonly static AuthKey Customer_MaintainMaliceUser = new AuthKey("MaintainMaliceUser");
        public readonly static AuthKey Customer_SetVIPRank = new AuthKey("SetVipPwdByHand");
        public readonly static AuthKey Customer_Abandon = new AuthKey("AbandonCustomer");
        public readonly static AuthKey Customer_LookCustomerPwd = new AuthKey("LookCustomerPwd");
        public readonly static AuthKey Customer_CallsEvents_Export = new AuthKey("Export");
        public readonly static AuthKey Customer_PointAddRequest_Export = new AuthKey("Export");
        public readonly static AuthKey Customer_PointAddRequest_PriceProtectPointConfirm = new AuthKey("PriceProtectPointConfirm");
        public readonly static AuthKey Customer_PointAddRequest_Submit = new AuthKey("SubmitCustomerPoint");
        public readonly static AuthKey Customer_PointAddRequest_Audit_NoLimit = new AuthKey("NoLimit");
        public readonly static AuthKey Customer_PointAddRequest_Audit_Lessthan300P = new AuthKey("Lessthan300P");
        public readonly static AuthKey Customer_PointAddRequest_Audit_Lessthan80P = new AuthKey("Lessthan80P");
        public readonly static AuthKey Customer_AccountPerid_SetDaysMoney_OverTenThousand = new AuthKey("SetDaysMoney_OverTenThousand");
        public readonly static AuthKey Customer_AccountPerid_SetDaysMoney_SetAccountDaysMoney = new AuthKey("SetAccountDaysMoney");
        public readonly static AuthKey Customer_Experience_ManualAdd = new AuthKey("AddCustomerExperiencebyHand");
        public readonly static AuthKey Customer_PrepayToBank = new AuthKey("PrepayToBank");
        public readonly static AuthKey Customer_AddressInfoEdit = new AuthKey("AddressInfoEdit");
        public readonly static AuthKey Customer_TaxInfoEdit = new AuthKey("TaxInfoEdit");
        public readonly static AuthKey Customer_CommendInfo = new AuthKey("ViewCommendInfo");
        public readonly static AuthKey Customer_CancelConfirmEmail = new AuthKey("CancelConfirmEmail");
        public readonly static AuthKey Customer_CancelConfirmPhone = new AuthKey("CancelConfirmPhone");

        #endregion Customer Domain

        #region Common Domain

        /// <summary>
        /// 控制面板 用户管理 新建用户
        /// </summary>
        public readonly static AuthKey Common_UserMgmt_Add = new AuthKey("Add");

        /// <summary>
        /// 控制面板 用户管理 编辑
        /// </summary>
        public readonly static AuthKey Common_UserMgmt_Edit = new AuthKey("Edit");

        /// <summary>
        /// 控制面板 查询日志 详细
        /// </summary>
        public readonly static AuthKey Common_LogQuery_Details = new AuthKey("Details");

        /// <summary>
        /// 控制面板 日志配置 创建日志配置信息
        /// </summary>
        public readonly static AuthKey Common_LogCategory_Add = new AuthKey("Add");

        /// <summary>
        /// 控制面板 日志配置 编辑日志配置信息
        /// </summary>
        public readonly static AuthKey Common_LogCategory_Edit = new AuthKey("Edit");

        /// <summary>
        /// 控制面板 货币币种新建
        /// </summary>
        public readonly static AuthKey Common_CurrencyQuery_Add = new AuthKey("Add");

        /// <summary>
        /// 控制面板 省、市、区 新建
        /// </summary>
        public readonly static AuthKey Common_AreaQuery_Add = new AuthKey("Add");

        /// <summary>
        /// 控制面板 省、市、区 编辑
        /// </summary>
        public readonly static AuthKey Common_AreaQuery_Edit = new AuthKey("Edit");

        /// <summary>
        /// 控制面板 配送方式-地区(非)新增  选择地区
        /// </summary>
        public readonly static AuthKey Common_ShipTypeAreaUnAddNew_ChooseArea = new AuthKey("ChooseArea");

        /// <summary>
        /// 控制面板 配送方式-地区(非)新增 新建
        /// </summary>
        public readonly static AuthKey Common_ShipTypeAreaUnAddNew_Add = new AuthKey("Add");

        /// <summary>
        /// 控制面板 配送方式  新建
        /// </summary>
        public readonly static AuthKey Common_ShipType_Add = new AuthKey("Add");

        /// <summary>
        /// 控制面板 配送方式 编辑
        /// </summary>
        public readonly static AuthKey Common_ShipType_Edit = new AuthKey("Edit");

        /// <summary>
        /// 控制面板 配送方式-地区-价格 新建
        /// </summary>
        public readonly static AuthKey Common_ShipTypeAreaPrice_Add = new AuthKey("Add");

        /// <summary>
        /// 控制面板 配送方式-地区-价格 编辑
        /// </summary>
        public readonly static AuthKey Common_ShipTypeAreaPrice_Edit = new AuthKey("Edit");

        /// <summary>
        /// 控制面板 配送方式-地区-价格 批量删除
        /// </summary>
        public readonly static AuthKey Common_ShipTypeAreaPrice_BatchDelete = new AuthKey("BatchDelete");

        /// <summary>
        /// 控制面板 配送方式-地区(非)  新建
        /// </summary>
        public readonly static AuthKey Common_ShipTypeAreaUn_Add = new AuthKey("Add");

        /// <summary>
        /// 控制面板 配送方式-地区(非) 批量删除
        /// </summary>
        public readonly static AuthKey Common_ShipTypeAreaUn_BatchDelete = new AuthKey("BatchDelete");

        /// <summary>
        /// 控制面板 配送方式-产品  新建
        /// </summary>
        public readonly static AuthKey Common_ShipTypeProduct_Add = new AuthKey("Add");

        /// <summary>
        /// 控制面板 配送方式-产品 批量删除
        /// </summary>
        public readonly static AuthKey Common_ShipTypeProduct_BatchDelete = new AuthKey("BatchDelete");

        /// <summary>
        /// 控制面板 配置项管理 新增
        /// </summary>
        public readonly static AuthKey Common_SilverlightConfig_Add = new AuthKey("Add");

        /// <summary>
        /// 控制面板 配置项管理 删除
        /// </summary>
        public readonly static AuthKey Common_SilverlightConfig_Delete = new AuthKey("Delete");

        /// <summary>
        /// 控制面板 支付方式 新建
        /// </summary>
        public readonly static AuthKey Common_PayType_Add = new AuthKey("Add");

        /// <summary>
        /// 控制面板 支付方式 编辑
        /// </summary>
        public readonly static AuthKey Common_PayType_Edit = new AuthKey("Edit");

        /// <summary>
        /// 控制面板 仓库24小时服务设置 新建
        /// </summary>
        public readonly static AuthKey Common_AreaDelivery_Add = new AuthKey("Add");

        /// <summary>
        /// 控制面板 仓库24小时服务设置 编辑
        /// </summary>
        public readonly static AuthKey Common_AreaDelivery_Edit = new AuthKey("Edit");

        /// <summary>
        /// 控制面板 仓库24小时服务设置 删除
        /// </summary>
        public readonly static AuthKey Common_AreaDelivery_Delete = new AuthKey("Delete");

        /// <summary>
        /// 控制面板 ReasonCode 添加
        /// </summary>
        public readonly static AuthKey Common_ReasonCode_Add = new AuthKey("Add");

        /// <summary>
        /// 控制面板 ReasonCode 编辑
        /// </summary>
        public readonly static AuthKey Common_ReasonCode_Edit = new AuthKey("Edit");

        /// <summary>
        /// 控制面板 ReasonCode 保存所有激活状态
        /// </summary>
        public readonly static AuthKey Common_ReasonCode_Save = new AuthKey("Save");

        /// <summary>
        /// 控制面板 配送方式-支付方式(非)  新建
        /// </summary>
        public readonly static AuthKey Common_ShipPayTypeUn_Add = new AuthKey("Add");

        /// <summary>
        /// 控制面板 配送方式-支付方式(非)  删除
        /// </summary>
        public readonly static AuthKey Common_ShipPayTypeUn_Delete = new AuthKey("Delete");

        /// <summary>
        /// 控制面板 节假日 新建
        /// </summary>
        public readonly static AuthKey Common_Holiday_Add = new AuthKey("Add");

        /// <summary>
        /// 控制面板 节假日 删除
        /// </summary>
        public readonly static AuthKey Common_Holiday_Delete = new AuthKey("Delete");

        /// <summary>
        /// 控制面板-基本信息维护-免运费配置管理 编辑（添加/更新）
        /// </summary>
        public readonly static AuthKey Common_FreeShippingChargeRule_Edit = new AuthKey("Edit");


        /// <summary>
        /// 控制面板-基本信息维护-免运费配置管理 设置为有效
        /// </summary>
        public readonly static AuthKey Common_FreeShippingChargeRule_Valid = new AuthKey("Valid");


        /// <summary>
        /// 控制面板-基本信息维护-免运费配置管理 设置为无效
        /// </summary>
        public readonly static AuthKey Common_FreeShippingChargeRule_Invalid = new AuthKey("Invalid");


        /// <summary>
        /// 控制面板-基本信息维护-免运费配置管理  删除
        /// </summary>
        public readonly static AuthKey Common_FreeShippingChargeRule_Delete = new AuthKey("Delete");

        #endregion

        #region IM Domain

        #region 配件

        public readonly static AuthKey IM_Accessory_AccessoryMaintain = new AuthKey("AccessoryMaintain");

        #endregion

        #region 商品

        public readonly static AuthKey IM_Product_ProductRefundMaintain = new AuthKey("ItemRefundMaintain");

        public readonly static AuthKey IM_Product_RelativeProductBatchMaintain = new AuthKey("RelativeCategoryMaintain");

        #region 商品查询

        public readonly static AuthKey IM_ProductQuery_ItemMaintainAllType = new AuthKey("Secondhand");

        public readonly static AuthKey IM_ProductQuery_ItemDisplaycolumn = new AuthKey("Displaycolumn");

        public readonly static AuthKey IM_ProductQuery_ExporterTariffApply = new AuthKey("ExporterTariffApply");

        public readonly static AuthKey IM_ProductQuery_BatchOnSale = new AuthKey("BatchOnSale");

        public readonly static AuthKey IM_ProductQuery_MultiLanguage = new AuthKey("MultiLanguage");

        #endregion

        #region 商品编辑

        public readonly static AuthKey IM_ProductMaintain_ItemMaintainAllType = new AuthKey("Secondhand");

        public readonly static AuthKey IM_ProductMaintain_ItemChangeProductStatusToUnShow = new AuthKey("ChangeProductStatusToUnShow");

        public readonly static AuthKey IM_ProductMaintain_ItemBasicInformationMaintain = new AuthKey("ItemBasicInformationMaintain");

        public readonly static AuthKey IM_ProductMaintain_ItemDescriptionMaintain = new AuthKey("UpdateDescription");

        public readonly static AuthKey IM_ProductMaintain_ItemWeightMaintain = new AuthKey("ItemWeightMaintain");

        public readonly static AuthKey IM_ProductMaintain_ItemPictureMaintain = new AuthKey("ItemPictureMaintain");

        public readonly static AuthKey IM_ProductMaintain_ItemWarrantyMaintain = new AuthKey("ItemWarrantyMaintain");

        public readonly static AuthKey IM_ProductMaintain_ItemPriceMaintain = new AuthKey("ItemPriceMaintain");

        public readonly static AuthKey IM_ProductMaintain_ItemPropertyMaintain = new AuthKey("UpdateProperty");

        public readonly static AuthKey IM_ProductMaintain_ItemRegionSalesMaintain = new AuthKey("ItemRegionSalesMaintain");

        public readonly static AuthKey IM_ProductMaintain_ItemDimensionMaintain = new AuthKey("ItemDimensionMaintain");

        public readonly static AuthKey IM_ProductMaintain_JDAndAMItemNumberMaintain = new AuthKey("JDAndAMItemNumberMaintain");

        public readonly static AuthKey IM_ProductMaintain_ItemTakePictures = new AuthKey("ItemTakePictures");

        public readonly static AuthKey IM_ProductMaintain_ItemBasicInformationPMUpdate = new AuthKey("ItemBasicInformationPMUpdate");

        public readonly static AuthKey IM_ProductMaintain_ItemBasicInformationProductTitleOrBriefNameMaintain = new AuthKey("UpdateTitleOrBriefName");

        public readonly static AuthKey IM_ProductMaintain_ItemBasicInformationProductModelMaintain = new AuthKey("UpdateMode");

        public readonly static AuthKey IM_ProductMaintain_ItemBasicInformationProductKeywordsMaintain = new AuthKey("ItemKeywordsMaintainRight");

        public readonly static AuthKey IM_ProductMaintain_ItemBasicInformationInfoFinishMaintain = new AuthKey("ItemMaintenanceInfoFinish");

        public readonly static AuthKey IM_ProductMaintain_ItemPriceRemarkMaintainOnly = new AuthKey("RemarkMaintainOnly");

        public readonly static AuthKey IM_ProductMaintain_ItemPriceUpdateVPMaintain = new AuthKey("UpdateVPMaintain");

        public readonly static AuthKey IM_ProductMaintain_ItemPriceVipPriceMaintain = new AuthKey("EditProductVipPriceRight");

        public readonly static AuthKey IM_ProductMaintain_ItemDisplaycolumn = new AuthKey("Displaycolumn");

        public readonly static AuthKey IM_ProductMaintain_ItemSalesArea = new AuthKey("SalesArea");

        public readonly static AuthKey IM_ProductMaintain_ItemMultiLanguage = new AuthKey("MultiLanguage");

        public readonly static AuthKey IM_ProductMaintain_ItemVirtualPriceEdit = new AuthKey("VirtualPriceEdit");

        public readonly static AuthKey IM_ProductMaintain_ItemProductOnSale = new AuthKey("ProductOnSale");

        public readonly static AuthKey IM_ProductMaintain_ItemProductOnShow = new AuthKey("ProductOnShow");

        public readonly static AuthKey IM_ProductMaintain_ItemProductOnNotShow = new AuthKey("ProductOnNotShow");

        public readonly static AuthKey IM_ProductMaintain_ItemProductAbandon = new AuthKey("ProductAbandon");

        public readonly static AuthKey IM_ProductMaintain_ItemProductBatchActive = new AuthKey("ProductBatchActive");

        public readonly static AuthKey IM_ProductMaintain_ItemProductMaintainEntryInfo = new AuthKey("MaintainEntryInfo");

        /// <summary>
        /// 查询PM列表权限 - 返回PM列表中所有PM(全部：包括无效).
        /// </summary>
        public readonly static AuthKey IM_SeniorPM_Query = new AuthKey("PMManager");
        /// <summary>
        /// 查询PM列表权限 - 返回自己及自己在PM组中所管理的所有有效状态的PM和其有效状态的备份PM及所有无效状态的PM.
        /// </summary>
        public readonly static AuthKey IM_IntermediatePM_Query = new AuthKey("PMIntermediate");
        /// <summary>
        /// 查询PM列表权限 - 返回自己和备份有效状态PM及所有无效状态的PM.
        /// </summary>
        public readonly static AuthKey IM_JuniorPM_Query = new AuthKey("PMPrimary");

        #endregion

        #region 商品价格审核管理

        public readonly static AuthKey IM_ProductPrice_PrimaryAuditPrice = new AuthKey("PrimaryAuditPrice");

        public readonly static AuthKey IM_ProductPrice_AdvancedAuditPrice = new AuthKey("AdvancedAuditPrice");

        public readonly static AuthKey IM_ProductPrice_ItemQuickApprove = new AuthKey("ItemQuickApprove");

        public readonly static AuthKey IM_ProductPrice_ItemPriceMaintain = new AuthKey("ItemPriceMaintain");

        public readonly static AuthKey IM_ProductPrice_ItemPMPrimary = new AuthKey("PMPrimary");

        public readonly static AuthKey IM_ProductPrice_ItemPMManager = new AuthKey("PMManager");

        #endregion

        #region 商家商品管理

        public readonly static AuthKey IM_SellerPortalProduct_ItemVendorPortalNewProductApprove = new AuthKey("ItemVendorPortalNewProductApprove");

        public readonly static AuthKey IM_SellerPortalProduct_ItemVendorPortalNewProductDecline = new AuthKey("ItemVendorPortalNewProductDecline");

        public readonly static AuthKey IM_SellerPortalProduct_ItemVendorPortalNewProductCreateID = new AuthKey("ItemVendorPortalNewProductCreateID");

        public readonly static AuthKey IM_SellerPortalProduct_ItemVendorPortalNewProductSpecialDeny = new AuthKey("ItemVendorPortalNewProductSpecialDeny");

        #endregion

        #endregion

        #region 类别

        public readonly static AuthKey IM_Category_CategoryRequestApply = new AuthKey("CategoryRequestApply");

        public readonly static AuthKey IM_Category_CategoryMaintain = new AuthKey("CategoryMaintain");

        public readonly static AuthKey IM_Category_CategoryRequestApproval = new AuthKey("CategoryRequestApproval");

        public readonly static AuthKey IM_Category_Category3RMAMaintain = new AuthKey("Category3RMAMaintain");

        public readonly static AuthKey IM_Category_CategoryKpiQuotaMaintain = new AuthKey("CategoryKpiQuotaMaintain");

        public readonly static AuthKey IM_Category_Category3MinMarginMaintain = new AuthKey("Category3MinMarginMaintain");

        public readonly static AuthKey IM_Category_CategoryExtendWarrantyMaintain = new AuthKey("CategoryExtendWarrantyMaintain");

        public readonly static AuthKey IM_Category_ExtendWarrantyDisuseBrandMaintain = new AuthKey("ExtendWarrantyDisuseBrandMaintain");

        public readonly static AuthKey IM_Category_AccessoryMaintain = new AuthKey("AccessoryMaintain");

        public readonly static AuthKey IM_Category_RelativeCategoryMaintain = new AuthKey("RelativeCategoryMaintain");

        #endregion

        #region 生产商、品牌

        public readonly static AuthKey IM_Manufacturer_ManufacturerRequestApproval = new AuthKey("ManufacturerRequestApproval");

        public readonly static AuthKey IM_Manufacturer_ManufacturerRequestApply = new AuthKey("ManufacturerRequestApply");

        public readonly static AuthKey IM_Brand_BrandRequestApproval = new AuthKey("BrandRequestApproval");

        public readonly static AuthKey IM_Brand_BrandRequestApply = new AuthKey("BrandRequestApply");

        public readonly static AuthKey IM_Brand_BrandStoreMaintain = new AuthKey("BrandStoreMaintain");

        #endregion

        #region PM管理

        public readonly static AuthKey IM_PM_PSMaintain = new AuthKey("PSMaintain");

        public readonly static AuthKey IM_PM_PMMaintain = new AuthKey("PMMaintain");

        public readonly static AuthKey IM_PM_PMGroupMaintain = new AuthKey("PMGroupMaintain");

        #endregion

        #region 多渠道设置

        public readonly static AuthKey IM_Product_ChannelProductPeriodPriceMaintain = new AuthKey("ChannelProductPeriodPriceApproveMaintain");

        #endregion

        #endregion

        #region Inventory Domain
        #region PM工作指标监控      
        public readonly static AuthKey Inventory_PMMonitoringPerformanceIndicators_ExportExcellAll = new AuthKey("CanExportPMMI");
        #endregion
        #region 备货中心相关权限点        
        public readonly static AuthKey Inventory_TransferStockingCenter_ExportExcellAll = new AuthKey("CanExportTransferStockingCenter");
        public readonly static AuthKey Inventory_TransferStockingCenter_OperatePurchase = new AuthKey("CanOperatePurchase");
        public readonly static AuthKey Inventory_TransferStockingCenter_OperateShift = new AuthKey("OperateShift");
        #endregion
        #region 已分配查询_货卡查询权限点
        public readonly static AuthKey Inventory_ItemAllocatedCardQuery_ExportExcellAll = new AuthKey("CanExportItemAllocatedCardQuery");
        public readonly static AuthKey Inventory_ItemsCardQuery_ExportExcellAll = new AuthKey("CanExportItemsCardQuery");
        #endregion
        #region 库存查询权限点        
        // InventoryMgmt_Inventory_Index@
        public readonly static AuthKey Inventory_InventoryQuery_ExportExcellAll = new AuthKey("CanExportInventoryQuery");
        #endregion
        #region Lend   借货单权限点
        // InventoryMgmt_Lend_Index@
        public readonly static AuthKey Inventory_LendRequestQuery_NavigateCreate = new AuthKey("CanNavigateCreateLendRequest");//查询页面创建按钮
        public readonly static AuthKey Inventory_LendRequestQuery_ExportExcell = new AuthKey("CanExportLendRequestQuery");
        // InventoryMgmt_Lend_Edit@
        public readonly static AuthKey Inventory_LendRequest_Create = new AuthKey("CanCreateLendRequest");
        public readonly static AuthKey Inventory_LendRequest_Return = new AuthKey("CanReturnLendRequest");
        public readonly static AuthKey Inventory_LendRequest_Reset = new AuthKey("CanResetLendRequest");        
        public readonly static AuthKey Inventory_LendRequest_Abandon = new AuthKey("CanAbandonLendRequest");
        public readonly static AuthKey Inventory_LendRequest_CancelAbandon = new AuthKey("CanCancelAbandonLendRequest");
        public readonly static AuthKey Inventory_LendRequest_Audit = new AuthKey("CanAuditLendRequest");
        public readonly static AuthKey Inventory_LendRequest_CancelAudit = new AuthKey("CanCancelAuditLendRequest");
        public readonly static AuthKey Inventory_LendRequest_OutStock = new AuthKey("CanOutStockLendRequest");
        public readonly static AuthKey Inventory_LendRequest_Print = new AuthKey("CanPrintLendRequest");
        #endregion
        #region Adjust   损益单权限点
        // InventoryMgmt_Adjust_Index@       
        public readonly static AuthKey Inventory_AdjustRequestQuery_NavigateCreate = new AuthKey("CanNavigateCreateAdjustRequest"); //查询页面创建按钮
        public readonly static AuthKey Inventory_AdjustRequestQuery_ExportExcell = new AuthKey("CanExportAdjustRequestQuery");        
        // InventoryMgmt_Adjust_Edit@               
        public readonly static AuthKey Inventory_AdjustRequest_Create = new AuthKey("CanCreateAdjustRequest");
        public readonly static AuthKey Inventory_AdjustRequest_Reset = new AuthKey("CanResetAdjustRequest");        
        public readonly static AuthKey Inventory_AdjustRequest_Abandon = new AuthKey("CanAbandonAdjustRequest");
        public readonly static AuthKey Inventory_AdjustRequest_CancelAbandon = new AuthKey("CanCancelAbandonAdjustRequest");
        public readonly static AuthKey Inventory_AdjustRequest_Audit = new AuthKey("CanAuditAdjustRequest");
        public readonly static AuthKey Inventory_AdjustRequest_CancelAudit = new AuthKey("CanCancelAuditAdjustRequest");
        public readonly static AuthKey Inventory_AdjustRequest_OutStock = new AuthKey("CanOutStockAdjustRequest");
        public readonly static AuthKey Inventory_AdjustRequest_Print = new AuthKey("CanPrintAdjustRequest");
        public readonly static AuthKey Inventory_AdjustRequest_Invoice = new AuthKey("CanInvoiceAdjustRequest");        
        public readonly static AuthKey Inventory_AdjustRequest_PrintInvoice = new AuthKey("CanPrintInvoiceAdjustRequest");
        #endregion
        #region Convert   转换单权限点
        // InventoryMgmt_Convert_Index@       
        public readonly static AuthKey Inventory_ConvertRequestQuery_NavigateCreate = new AuthKey("CanNavigateCreateConvertRequest");//查询页面创建按钮
        public readonly static AuthKey Inventory_ConvertRequestQuery_ExportExcell = new AuthKey("CanExportConvertRequestQuery");
        // InventoryMgmt_Convert_Edit@               
        public readonly static AuthKey Inventory_ConvertRequest_Create = new AuthKey("CanCreateConvertRequest");
        public readonly static AuthKey Inventory_ConvertRequest_Reset = new AuthKey("CanResetConvertRequest");
        public readonly static AuthKey Inventory_ConvertRequest_Abandon = new AuthKey("CanAbandonConvertRequest");
        public readonly static AuthKey Inventory_ConvertRequest_CancelAbandon = new AuthKey("CanCancelAbandonConvertRequest");
        public readonly static AuthKey Inventory_ConvertRequest_Audit = new AuthKey("CanAuditConvertRequest");
        public readonly static AuthKey Inventory_ConvertRequest_CancelAudit = new AuthKey("CanCancelAuditConvertRequestt");
        public readonly static AuthKey Inventory_ConvertRequest_OutStock = new AuthKey("CanOutStockConvertRequest");
        public readonly static AuthKey Inventory_ConvertRequest_Print = new AuthKey("CanPrintConvertRequest");        
        #endregion
        #region 移仓单相关权限点
        // InventoryMgmt_InventoryTransfer_Index@       
        public readonly static AuthKey Inventory_ShiftRequestQuery_NavigateCreate = new AuthKey("CanNavigateCreateShiftRequest");//查询页面创建按钮
        public readonly static AuthKey Inventory_ShiftRequestQuery_Total = new AuthKey("CanTotalShiftRequest");//查询页面SyncSAP
        public readonly static AuthKey Inventory_ShiftRequestQuery_SyncSAP = new AuthKey("CanSyncSAPShiftRequest");//查询页面SyncSAP
        public readonly static AuthKey Inventory_ShiftRequestQuery_BatchSpecial = new AuthKey("CanBatchSpecialShiftRequest");//查询页面批量设置特殊移仓单
        public readonly static AuthKey Inventory_ShiftRequestQuery_CancelBatchSpecial = new AuthKey("CanCancelBatchSpecialShiftRequest");//查询页面批量取消特殊移仓单
        public readonly static AuthKey Inventory_ShiftRequestQuery_BatchLog = new AuthKey("CanBatchLogShiftRequest");//查询页面添加跟进日志
        public readonly static AuthKey Inventory_ShiftRequestQuery_ExportExcell = new AuthKey("CanExportShiftRequestQuery");
        // InventoryMgmt_InventoryTransfer_Edit@               
        public readonly static AuthKey Inventory_ShiftRequest_Create = new AuthKey("CanCreateShiftRequest");
        public readonly static AuthKey Inventory_ShiftRequest_Reset = new AuthKey("CanResetShiftRequest");
        public readonly static AuthKey Inventory_ShiftRequest_BatchAddGiftItem = new AuthKey("CanBatchAddGiftItemShiftRequest");
        public readonly static AuthKey Inventory_ShiftRequest_Abandon = new AuthKey("CanAbandonShiftRequest");
        public readonly static AuthKey Inventory_ShiftRequest_CancelAbandon = new AuthKey("CanCancelAbandonShiftRequest");
        public readonly static AuthKey Inventory_ShiftRequest_Audit = new AuthKey("CanAuditShiftRequest");
        public readonly static AuthKey Inventory_ShiftRequest_CancelAudit = new AuthKey("CanCancelAuditShiftRequest");
        public readonly static AuthKey Inventory_ShiftRequest_OutStock = new AuthKey("CanOutStockShiftRequestt");
        public readonly static AuthKey Inventory_ShiftRequest_Print = new AuthKey("CanPrintShiftRequest");
        public readonly static AuthKey Inventory_ShiftRequest_PrintInvoice = new AuthKey("CanPrintInvoiceShiftRequest");

        public readonly static AuthKey Inventory_ShiftRequest_RequestMemo = new AuthKey("CanRequestMemoShiftRequest");
        public readonly static AuthKey Inventory_ShiftRequest_SetSpecial = new AuthKey("CanSetSpecialShiftRequest");
        public readonly static AuthKey Inventory_ShiftRequest_CancelSpecial = new AuthKey("CanCancelSpecialShiftRequest");

        #endregion
        #region 移仓单跟进日志相关权限点
        // InventoryMgmt_InventoryTransfer_Index@             
        public readonly static AuthKey Inventory_ShiftRequestMemoQuery_ExportExcell = new AuthKey("CanExportShiftRequestMemoQuery");
        // InventoryMgmt_InventoryTransfer_Edit@               
        public readonly static AuthKey Inventory_ShiftRequestMemo_NewMemo = new AuthKey("CanCreateShiftRequestMemo");
        public readonly static AuthKey Inventory_ShiftRequestMemo_CloseMemo = new AuthKey("CanCloseShiftRequestMemo");
        #endregion
        #region 移仓篮相关权限点
        //InventoryMgmt_InventoryTransfer_Basket@             
        public readonly static AuthKey Inventory_ShiftRequestItemBasket_ExportExcell = new AuthKey("CanExportShiftRequestItemBasket");       
        public readonly static AuthKey Inventory_ShiftRequestItemBasket_CreateShift = new AuthKey("CanCreateShift");
        public readonly static AuthKey Inventory_ShiftRequestItemBasket_AddItem = new AuthKey("CanAddItem");
        public readonly static AuthKey Inventory_ShiftRequestItemBasket_Modify = new AuthKey("CanModify");
        public readonly static AuthKey Inventory_ShiftRequestItemBasket_Delete = new AuthKey("CanDelete");        
        #endregion
        #region 仓库移仓信息配置关权限点
        //InventoryMgmt_InventoryTransferConfig_Index@             
        public readonly static AuthKey Inventory_ShiftConfigInfoQuery_ExportExcell = new AuthKey("CanExportShiftConfigInfoQuery");
        public readonly static AuthKey Inventory_ShiftConfigInfoQuery_New = new AuthKey("CanNew");
        public readonly static AuthKey Inventory_ShiftConfigInfoQuery_Modify = new AuthKey("CanModify");          
        #endregion
        #region 虚库申请单相关权限点
        //虚库日志：
        //InventoryMgmt_VirtualInventory_Log@             
        public readonly static AuthKey Inventory_VirtualRequestMemoQuery_ExportExcell = new AuthKey("CanExportVirtualRequestMemoQuery");
        //虚库申请单：
        //InventoryMgmt_VirtualInventory_Index@
        public readonly static AuthKey Inventory_VirtualRequestQuery_ExportExcell = new AuthKey("CanExportVirtualRequestQuery");
        public readonly static AuthKey Inventory_VirtualRequestQuery_OperateVirtualRequestMaintain = new AuthKey("CanOperateVirtualRequestMaintain");
        public readonly static AuthKey Inventory_VirtualRequestQuery_OperateVirtualRequestMaintainBatch = new AuthKey("CanOperateVirtualRequestMaintainBatch");
        public readonly static AuthKey Inventory_VirtualRequestQuery_Audit = new AuthKey("CanAuditVirtualRequest");
        //批量创建虚库申请单 
        //是否能够操作低于800元的商品
        public readonly static AuthKey Inventory_VirtualRequestApply_CanOperateItemOfLessThanPrice = new AuthKey("CanOperateItemOfLessThanPrice");
        //是否能够操作二手品
        public readonly static AuthKey Inventory_VirtualRequestApply_CanOperateItemOfSecondHand = new AuthKey("CanOperateItemOfSecondHand");                
        #endregion
        #region 仓库信息相关权限点
        //仓库维护：
        //InventoryMgmt_Warehouse_Index@             
        public readonly static AuthKey Inventory_WarehouseQuery_ExportExcell = new AuthKey("CanExportWarehouseQuery");
        public readonly static AuthKey Inventory_WarehouseQuery_New = new AuthKey("CanNew");
        public readonly static AuthKey Inventory_WarehouseQuery_Refresh = new AuthKey("CanRefresh");            
        //渠道仓库维护查询：
        //InventoryMgmt_Maintain_Stock@             
        public readonly static AuthKey Inventory_StockQuery_ExportExcell = new AuthKey("CanExportStockQuery");
        public readonly static AuthKey Inventory_StockQuery_New = new AuthKey("CanNew");    
        //仓库所有者查询：
        // InventoryMgmt_Warehouse_Maintain@@             
        public readonly static AuthKey Inventory_WarehouseOwnerQuery_ExportExcell = new AuthKey("CanExportWarehouseOwnerQuery");
        public readonly static AuthKey Inventory_WarehouseOwnerQuery_OwnerNew = new AuthKey("CanOwnerNew");                    
        #endregion                
        #region 体验厅调拨单管理权限点
        //添加调拨单
        public readonly static AuthKey Inventory_ExperienceHall_Create = new AuthKey("CanCreateExperienceHall");
        //作废调拨单
        public readonly static AuthKey Inventory_ExperienceHall_Abandon = new AuthKey("CanAbandonExperienceHall");
        //审核调拨单
        public readonly static AuthKey Inventory_ExperienceHall_Audit = new AuthKey("CanAuditExperienceHall");
        //取消审核调拨单
        public readonly static AuthKey Inventory_ExperienceHall_CancelAudit = new AuthKey("CanCancelAuditExperienceHall");
        //体验厅接收
        public readonly static AuthKey Inventory_ExperienceHall_ExperienceIn = new AuthKey("ExperienceInExperienceHall");
        //仓库接收
        public readonly static AuthKey Inventory_ExperienceHall_ExperienceOut = new AuthKey("ExperienceOutExperienceHall");
        #endregion
        #endregion Inventory Domain

        #region PO Domain

        #region [供应商]
        /// <summary>
        /// PO - 供应商查询，导出Excel权限
        /// </summary>
        public static readonly AuthKey PO_Vendor_ExportExcel = new AuthKey("CanExportVendorQuery");
        /// <summary>
        /// PO - 供应商 -搜索“可用”状态的供应商
        /// </summary>
        public static readonly AuthKey PO_Vendor_SearchValidVendor = new AuthKey("CanSearchValidVendorList");
        /// <summary>
        /// PO - 供应商 - 编辑供应商佣金信息
        /// </summary>
        public static readonly AuthKey PO_Vendor_EditVendorCommission = new AuthKey("CanEditVendorCommission");
        /// <summary>
        /// PO - 供应商 - 更新供应商状态 (可用，不可用)
        /// </summary>
        public static readonly AuthKey PO_Vendor_UpdateVendorStatus = new AuthKey("CanUpdateVendorStatus");

        /// <summary>
        /// PO - 供应商 - 编辑供应商代理信息(编辑，删除,审核操作)
        /// </summary>
        public static readonly AuthKey PO_Vendor_EditVendorAgentInfo = new AuthKey("CanEditVendorAgentInfo");

        /// <summary>
        /// PO - 供应商 - 编辑供应商财务信息（开户行信息)
        /// </summary>
        public static readonly AuthKey PO_Vendor_EditVendorFinanceInfo = new AuthKey("CanEditVendorFinanceInfo");
        /// <summary>
        /// 供应商 - 显示所有账期类型
        /// </summary>
        public static readonly AuthKey PO_Vendor_ShowAllPayPeriodType = new AuthKey("CanShowAllPayPeriodType");

        /// <summary>
        /// 供应商 - 编辑供应商售后信息
        /// </summary>
        public static readonly AuthKey PO_Vendor_UpdateVendorServiceInfo = new AuthKey("CanUpdateVendorServiceInfo");
        /// <summary>
        /// 供应商 - 锁定供应商
        /// </summary>
        public static readonly AuthKey PO_Vendor_HoldVendor = new AuthKey("CanHoldVendor");
        /// <summary>
        /// 供应商 - 创建和更新供应商
        /// </summary>
        public static readonly AuthKey PO_Vendor_EditAndCreateVendor = new AuthKey("CanEditAndCreateVendor");
        /// <summary>
        /// 供应商 - 提交审核,取消审核 供应商信息
        /// </summary>
        public static readonly AuthKey PO_Vendor_RequestVendor = new AuthKey("CanVendorRequest");
        /// <summary>
        ///  供应商 - 审核通过，审核未通过 供应商信息
        /// </summary>
        public static readonly AuthKey PO_Vendor_VerifyVendor = new AuthKey("CanVendorVerify");
        #endregion

        #region [采购单]
        /// <summary>
        /// 采购篮 - 导入采购篮数据
        /// </summary>
        public static readonly AuthKey PO_Basket_ImportBasket = new AuthKey("CanImportBasket");

        /// <summary>
        /// 采购单 -  创建采购单
        /// </summary>
        public static readonly AuthKey PO_PurchaseOrder_CreatePO = new AuthKey("CanCreatePO");

        /// <summary>
        /// 采购单 - 编辑采购单商品
        /// </summary>
        public static readonly AuthKey PO_PurchaseOrder_EditItem = new AuthKey("CanEditPOItem");
        /// <summary>
        /// 采购单- 显示采购价,总价
        /// </summary>
        public static readonly AuthKey PO_PurchaseOrder_ViewItemPrice = new AuthKey("CanViewItemPrice");

        /// <summary>
        /// 采购单 - 确认来自VendorPortal的PO单
        /// </summary>
        public static readonly AuthKey PO_PurchaseOrder_ConfirmVPPO = new AuthKey("CanConfirmVPPO");
        /// <summary>
        ///  采购单 - 退回来自VendorPortal的PO单
        /// </summary>
        public static readonly AuthKey PO_PurchaseOrder_SendBackVPPO = new AuthKey("CanSendBackVPPO");
        /// <summary>
        ///   采购单 - 更新采购单
        /// </summary>
        public static readonly AuthKey PO_PurchaseOrder_Update = new AuthKey("CanUpdatePO");
        /// <summary>
        /// 采购单 - 确认采购单
        /// </summary>
        public static readonly AuthKey PO_PurchaseOrder_Verify = new AuthKey("CanVerifyPO");
        /// <summary>
        /// 采购单- 取消确认采购单
        /// </summary>
        public static readonly AuthKey PO_PurchaseOrder_CancelVerify = new AuthKey("CanCancelVerifyPO");
        /// <summary>
        /// 采购单 - 作废采购单
        /// </summary>
        public static readonly AuthKey PO_PurchaseOrder_Abandon = new AuthKey("CanAbandonPO");
        /// <summary>
        /// 采购单-  取消作废采购单
        /// </summary>
        public static readonly AuthKey PO_PurchaseOrder_CancelAbandon = new AuthKey("CanCancelAbandonPO");
        /// <summary>
        /// 采购单 - 更新入库备注和到付运费金额
        /// </summary>
        public static readonly AuthKey PO_PurchaseOrder_UpdateInstockMemo = new AuthKey("CanUpdateInstockMemoPO");

        /// <summary>
        /// 采购单 - 打印，显示采购价格
        /// </summary>
        public static readonly AuthKey PO_PurchaseOrder_ViewOrderPriceAndLineCostForPrint = new AuthKey("CanViewOrderPriceAndLineCostForPrintPO");
        /// <summary>
        /// 采购单 - 导出采购单查询结果
        /// </summary>
        public static readonly AuthKey PO_PurchaseOrder_ExportPOList = new AuthKey("CanExportPOList");
        /// <summary>
        /// 供应商退款单 - 导出查询结果
        /// </summary>
        public static readonly AuthKey PO_VendorRefund_Export = new AuthKey("CanExportVendorRefundList");
        /// <summary>
        ///  供应商退款单 - 查询
        /// </summary>
        public static readonly AuthKey PO_VendorRefund_Query = new AuthKey("CanQueryVendorRefundList");
        /// <summary>
        ///  供应商退款单 - PMD审核权限
        /// </summary>
        public static readonly AuthKey PO_VendorRefund_PMDVerify = new AuthKey("CanVendorRefundPMDVerify");
        /// <summary>
        ///  供应商退款单 - PMCC审核权限
        /// </summary>
        public static readonly AuthKey PO_VendorRefund_PMCCVerify = new AuthKey("CanVendorRefundPMCCVerify");

        #region [PO单审核权限]
        /// <summary>
        /// PO单 -  审核采购单(最高权限)
        /// </summary>
        public static readonly AuthKey PO_PurchaseOrder_AuditAll = new AuthKey("CanAuditAll");
        /// <summary>
        /// PO单 -  审核采购单(中级权限)
        /// </summary>
        public static readonly AuthKey PO_PurchaseOrder_AuditGeneric = new AuthKey("CanAuditGeneric");
        /// <summary>
        /// PO单- 具有对采购单审核-发票超期权限
        /// </summary>
        public static readonly AuthKey PO_PurchaseOrder_AuditInvoiceAbsent = new AuthKey("CanAuditInvoiceAbsent");
        /// <summary>
        /// PO单 -  具有对滞收发票PM的权限审核
        /// </summary>
        public static readonly AuthKey PO_PurchaseOrder_AuditLagInvoice = new AuthKey("CanAuditLagInvoice");
        /// <summary>
        /// PO单 -  审核采购单(一般权限)
        /// </summary>
        public static readonly AuthKey PO_PurchaseOrder_AuditLow = new AuthKey("CanAuditLow");
        /// <summary>
        /// PO单 - 具有审核负采购单的权限
        /// </summary>
        public static readonly AuthKey PO_PurchaseOrder_AuditNegativeStock = new AuthKey("CanAuditNegativeStock");
        /// <summary>
        /// PO单 - 具有审核滞销收货PO权限
        /// </summary>
        public static readonly AuthKey PO_PurchaseOrder_AuditLagGoods = new AuthKey("CanAuditLagGoods");

        #endregion

        #region
        #endregion

        #endregion

        #region [虚库采购单]
        /// <summary>
        /// 虚库采购单 - 更新
        /// </summary>
        public static readonly AuthKey PO_VirtualPO_Update = new AuthKey("CanUpdateVSPO");
        /// <summary>
        /// 虚库采购单  作废
        /// </summary>
        public static readonly AuthKey PO_VirtualPO_Abandon = new AuthKey("CanAbandonVSPO");
        /// <summary>
        /// 创建虚库采购单
        /// </summary>
        public static readonly AuthKey PO_VirtualPO_Create = new AuthKey("CanCreateVSPO");
        /// <summary>
        /// 虚库采购单 - CS备注更新
        /// </summary>
        public static readonly AuthKey PO_VirtualPO_UpdateCSMemo = new AuthKey("CanUpdateCSMemoVSPO");
        /// <summary>
        /// 虚库采购单 - 导出查询记录
        /// </summary>
        public static readonly AuthKey PO_VirtualPO_Export = new AuthKey("CanExportVSPOList");
        /// <summary>
        /// 虚库采购单 - 查询
        /// </summary>
        public static readonly AuthKey PO_VirtualPO_Query = new AuthKey("CanQueryPOList");

        #endregion

        #region [代销结算单]
        /// <summary>
        /// 导出代销结算单
        /// </summary>
        public static readonly AuthKey PO_Consign_ExportConsignList = new AuthKey("CanExportConsignList");
        /// <summary>
        /// 导出代销转财务记录
        /// </summary>
        public static readonly AuthKey PO_Consign_ExportConsignToAccountList = new AuthKey("CanExportConsignToAccountList");

        /// <summary>
        /// pm高级权限
        /// </summary>
        public readonly static AuthKey PO_SeniorPM_Query = new AuthKey("PMManager");

        /// <summary>
        /// 新建代销结算单
        /// </summary>
        public readonly static AuthKey PO_Consign_New = new AuthKey("CanNewConsign");

        /// <summary>
        /// 审核代销结算单
        /// </summary>
        public readonly static AuthKey PO_Consign_Query_Audit = new AuthKey("CanAuditConsign");

        /// <summary>
        /// 取消审核代销结算单
        /// </summary>
        public readonly static AuthKey PO_Consign_Edit_CancelAudit = new AuthKey("CanCancelAuditConsign");

        /// <summary>
        /// 结算
        /// </summary>
        public readonly static AuthKey PO_Consign_Edit_Settle = new AuthKey("CanSettle");

        /// <summary>
        /// 取消结算
        /// </summary>
        public readonly static AuthKey PO_Consign_Edit_CancelSettle = new AuthKey("CanCancelSettle");

        /// <summary>
        /// 编辑页面的保存结算
        /// </summary>
        public readonly static AuthKey PO_Consign_Edit_Save = new AuthKey("CanEditSave");

        /// <summary>
        /// 结算单编辑页面添加商品
        /// </summary>
        public readonly static AuthKey PO_Consign_Edit_AddProduct = new AuthKey("CanEditAddProduct");

        /// <summary>
        /// 结算单编辑页面删除商品
        /// </summary>
        public readonly static AuthKey PO_Consign_Edit_DeleteProduct = new AuthKey("CanEditDeleteProduct");

        /// <summary>
        /// 结算单编辑页面作废
        /// </summary>
        public readonly static AuthKey PO_Consign_Edit_Abandon = new AuthKey("CanEditAbandon");

        /// <summary>
        /// 结算单编辑页面取消作废
        /// </summary>
        public readonly static AuthKey PO_Consign_Edit_CancelAbandon = new AuthKey("CanEditCancelAbandon");

        #endregion

        #region [代收结算单]
        public static readonly AuthKey PO_Gather_Query = new AuthKey("CanQueryGather");

        public static readonly AuthKey PO_Gather_Query_New = new AuthKey("CanNewGather");

        public static readonly AuthKey PO_Gather_Edit_DeleteProduct = new AuthKey("CanDeleteProduct");
        public static readonly AuthKey PO_Gather_Edit_Abandon = new AuthKey("CanAbandon");
        public static readonly AuthKey PO_Gather_Edit_Audit = new AuthKey("CanAudit");
        public static readonly AuthKey PO_Gather_Edit_CancelAudit = new AuthKey("CanCancelAudit");
        public static readonly AuthKey PO_Gather_Edit_Settle = new AuthKey("CanSettle");
        public static readonly AuthKey PO_Gather_Edit_CancelSettle = new AuthKey("CancelSettle");

        #endregion

        #region [佣金信息查询]
        /// <summary>
        /// 佣金 - 关闭佣金
        /// </summary>
        public static readonly AuthKey PO_Commission_Close = new AuthKey("CanCloseCommission");

        public static readonly AuthKey PO_Commission_Create = new AuthKey("CanCreateCommission");

        #endregion

        #region [代收代付结算单查询]
        
        public static readonly AuthKey PO_CollectionPayment_Query_Create = new AuthKey("CanCollectionPaymentQueryCreate");

        public static readonly AuthKey PO_CollectionPayment_Query_Edit = new AuthKey("CanCollectionPaymentEdit");

        public static readonly AuthKey PO_CollectionPayment_New_CanSave = new AuthKey("CanSave");

        /// <summary>
        /// 导出代销结算单
        /// </summary>
        public static readonly AuthKey PO_CollectionPayment_ExportConsignList = new AuthKey("CanExportConsignListCollectionPaymentMaintain");
        /// <summary>
        /// 导出代销转财务记录
        /// </summary>
        public static readonly AuthKey PO_CollectionPayment_ExportConsignToAccountList = new AuthKey("CanExportConsignToAccountListCollectionPaymentMaintain");

        /// <summary>
        /// pm高级权限
        /// </summary>
        public readonly static AuthKey PO_CollectionPayment_SeniorPM_Query = new AuthKey("PMManagerCollectionPaymentMaintain");

        

        /// <summary>
        /// 审核代销结算单
        /// </summary>
        public readonly static AuthKey PO_CollectionPayment_Edit_Audit = new AuthKey("CanAuditCollectionPaymentMaintain");

        /// <summary>
        /// 取消审核代销结算单
        /// </summary>
        public readonly static AuthKey PO_CollectionPayment_Edit_CancelAudit = new AuthKey("CanCancelAuditCollectionPaymentMaintain");

        /// <summary>
        /// 结算
        /// </summary>
        public readonly static AuthKey PO_CollectionPayment_Edit_Settle = new AuthKey("CanSettleCollectionPaymentMaintain");

        /// <summary>
        /// 取消结算
        /// </summary>
        public readonly static AuthKey PO_CollectionPayment_Edit_CancelSettle = new AuthKey("CanCancelSettleCollectionPaymentMaintain");

        /// <summary>
        /// 编辑页面的保存结算
        /// </summary>
        public readonly static AuthKey PO_CollectionPayment_Edit_Save = new AuthKey("CanEditSaveCollectionPaymentMaintain");

        /// <summary>
        /// 结算单编辑页面添加商品
        /// </summary>
        public readonly static AuthKey PO_CollectionPayment_Edit_AddProduct = new AuthKey("CanEditAddProductCollectionPaymentMaintain");

        /// <summary>
        /// 结算单编辑页面删除商品
        /// </summary>
        public readonly static AuthKey PO_CollectionPayment_Edit_DeleteProduct = new AuthKey("CanEditDeleteProductCollectionPaymentMaintain");

        /// <summary>
        /// 结算单编辑页面作废
        /// </summary>
        public readonly static AuthKey PO_CollectionPayment_Edit_Abandon = new AuthKey("CanEditAbandonCollectionPaymentMaintain");

        /// <summary>
        /// 结算单编辑页面取消作废
        /// </summary>
        public readonly static AuthKey PO_CollectionPayment_Edit_CancelAbandon = new AuthKey("CanEditCancelAbandonCollectionPaymentMaintain");


        #endregion

        #region [成本变价单]
        /// <summary>
        /// 成本变价单 - 更新
        /// </summary>
        public static readonly AuthKey PO_CostChange_Update = new AuthKey("CanUpdateCC");
        /// <summary>
        /// 成本变价单  作废
        /// </summary>
        public static readonly AuthKey PO_CostChange_Abandon = new AuthKey("CanAbandonCC");
        /// <summary>
        /// 创建成本变价单
        /// </summary>
        public static readonly AuthKey PO_CostChange_Create = new AuthKey("CanCreateCC");
        /// <summary>
        /// 成本变价单 - 提交审核
        /// </summary>
        public static readonly AuthKey PO_CostChange_Submit = new AuthKey("CanSubmitCC");
        /// <summary>
        /// 成本变价单 - 撤销提交审核
        /// </summary>
        public static readonly AuthKey PO_CostChange_CancelSubmit = new AuthKey("CanCancelSubmitCC");
        /// <summary>
        /// 成本变价单 - 审核
        /// </summary>
        public static readonly AuthKey PO_CostChange_Audit = new AuthKey("CanAuditCC");
        /// <summary>
        /// 成本变价单 - 审核拒绝
        /// </summary>
        public static readonly AuthKey PO_CostChange_Deny = new AuthKey("CanDenyCC");
        /// <summary>
        /// 成本变价单 - 查询
        /// </summary>
        public static readonly AuthKey PO_CostChange_Query = new AuthKey("CanQueryCCList");
        /// <summary>
        /// 成本变价单 - 更新页面添加明细
        /// </summary>
        public static readonly AuthKey PO_CostChange_AddCCItem = new AuthKey("CanAddCCItem");
        /// <summary>
        /// 成本变价单 - 更新页面删除明细
        /// </summary>
        public static readonly AuthKey PO_CostChange_RemoveCCItem = new AuthKey("CanRemoveCCItem");
        /// <summary>
        /// 成本变价单 - 新建页面添加明细
        /// </summary>
        public static readonly AuthKey PO_CostChange_AddNewCCItem = new AuthKey("CanAddNewCCItem");
        /// <summary>
        /// 成本变价单 - 新建页面删除明细
        /// </summary>
        public static readonly AuthKey PO_CostChange_RemoveNewCCItem = new AuthKey("CanRemoveNewCCItem");

        #endregion
        #endregion PO Domain

        #region SO Domain

        #region SOCommon

        public readonly static AuthKey SO_ExcelExport = new AuthKey("ExcelExport");

        public readonly static AuthKey SO_RouterExcelExport = new AuthKey("RouterExcelExport");

        #endregion

        /// <summary>
        /// 订单维护 拆分生成新订单
        /// </summary>
        public readonly static AuthKey SO_SOMaintain_SplitNewSO = new AuthKey("SplitSO");

        /// <summary>
        /// 账户余额支付
        /// </summary>
        public readonly static AuthKey SO_SOMaintain_UsePrepay = new AuthKey("UsePrepay");

        /// <summary>
        /// 手动改价
        /// </summary>
        public readonly static AuthKey SO_SOMaintain_IsWholeSale = new AuthKey("IsWholeSale");
        public readonly static AuthKey SO_SOMaintain_ViewMunalPriceInfo = new AuthKey("ViewMunalPriceInfo");

        /// <summary>
        /// 显示用户积分
        /// </summary>
        public readonly static AuthKey SO_SOMaintain_ShowPoint = new AuthKey("ShowPoint");

        /// <summary>
        /// 是否需要保存客户地址
        /// </summary>
        public readonly static AuthKey SO_SOMaintain_NeedSaveShippingAddress = new AuthKey("NeedSaveShippingAddress");

        /// <summary>
        /// 以旧换新
        /// </summary>
        public readonly static AuthKey SO_SOMaintain_OldChangeNew = new AuthKey("OldChangeNew");

        /// <summary>
        /// 客户基本信息
        /// </summary>
        public readonly static AuthKey SO_SOMaintain_ViewCustomerInfo = new AuthKey("ViewCustomerInfo");

        /// <summary>
        /// 修改财务备注
        /// </summary>
        public readonly static AuthKey SO_SOMaintain_CanUpdateSOFinanceNote = new AuthKey("CanUpdateSOFinanceNote");

        /// <summary>
        /// 修改是否VIP订单
        /// </summary>
        public readonly static AuthKey SO_SOMaintain_CanUpdateIsVIP = new AuthKey("CanUpdateIsVIP");

        /// <summary>
        /// 是否VIP订单自动选中
        /// </summary>
        public readonly static AuthKey SO_SOMaintain_IsVIPRole = new AuthKey("IsVIPRole");

        /// <summary>
        /// 显示平均成本
        /// </summary>
        public readonly static AuthKey SO_SOMaintain_ViewGrossProfit = new AuthKey("ViewGrossProfit");

        public readonly static AuthKey SO_SOMaintain_SOCreate = new AuthKey("SOCreate");

        public readonly static AuthKey SO_SOMaintain_SOAudit = new AuthKey("SOAudit");

        public readonly static AuthKey SO_SOMaintain_SOAuditNetPay = new AuthKey("SOAuditNetPay");

        public readonly static AuthKey SO_SOMaintain_SOCancelAudit = new AuthKey("SOCancelAudit");

        public readonly static AuthKey SO_SOMaintain_SOManagerAudit = new AuthKey("SOManagerAudit");

        public readonly static AuthKey SO_SOMaintain_SOAbandon = new AuthKey("SOAbandon");

        public readonly static AuthKey SO_SOMaintain_SOEmployeeAbandon = new AuthKey("SOEmployeeAbandon");

        //创建财务负款单
        public readonly static AuthKey SO_SOMaintain_CreateNegativeFinanceRecord = new AuthKey("CreateNegativeFinanceRecord");

        public readonly static AuthKey SO_SOMaintain_SOManagerAbandon = new AuthKey("SOManagerAbandon");

        public readonly static AuthKey SO_SOMaintain_HoldSO = new AuthKey("HoldSO");

        public readonly static AuthKey SO_SOMaintain_SyncHoldSO = new AuthKey("SyncHoldSO");

        public readonly static AuthKey SO_SOMaintain_SplitSO = new AuthKey("SplitSO");

        public readonly static AuthKey SO_SOMaintain_CannelSplitSO = new AuthKey("CannelSplitSO");

        //拆分发票（原来IPP为页面控制级别，现在为控件）
        public readonly static AuthKey SO_SOMaintain_SplitInvoice = new AuthKey("SplitInvoice");

        public readonly static AuthKey SO_SOMaintain_SOUpdate = new AuthKey("SOUpdate");

        public readonly static AuthKey SO_SOMaintain_UpdateSOFinanceNote = new AuthKey("UpdateSOFinanceNote");

        public readonly static AuthKey SO_SOMaintain_SOReview = new AuthKey("SOReview");

        public readonly static AuthKey SO_SOMaintain_SONoteAndInvoiceNoteUpdate = new AuthKey("SONoteAndInvoiceNoteUpdate");

        public readonly static AuthKey SO_SOMaintain_ApplyPoint = new AuthKey("ApplyPoint");

        public readonly static AuthKey SO_SOMaintain_DisplayPoint = new AuthKey("DisplayPoint");

        public readonly static AuthKey SO_SOMaintain_SOSIMStatusEidt = new AuthKey("SOSIMStatusEidt");

        public readonly static AuthKey SO_SOMaintain_CreateGiftCardOrder = new AuthKey("CreateGiftCardOrder");

        public readonly static AuthKey SO_SOMaintain_AuditGiftCardOrder = new AuthKey("AuditGiftCardOrder");

        //普票改赠票
        public readonly static AuthKey SO_SOMaintain_InvoiceToVatOpt = new AuthKey("InvoiceToVatOpt");

        /// <summary>
        /// 使用未绑定礼品卡权限
        /// </summary>
        public readonly static AuthKey SO_SOMaintain_UseUnBindingGiftCard = new AuthKey("UseUnBindingGiftCard ");

        public readonly static AuthKey SO_SOMaintain_EditSOCustomerInfo = new AuthKey("EditSOCustomerInfo");

        public readonly static AuthKey SO_SOMaintain_HasBatchUploadItem = new AuthKey("HasBatchUploadItem");

        #region 投诉

        public readonly static AuthKey SO_Complain_ComplainFull = new AuthKey("ComplainFull");

        public readonly static AuthKey SO_Complain_AssignComplainCase = new AuthKey("AssignComplainCase");

        public readonly static AuthKey SO_Complain_RecallAssignedComplainCase = new AuthKey("RecallAssignedComplainCase");

        #endregion

        #region publicMemo

        public readonly static AuthKey SO_publicMemo_SendOrderpublicMemoEmail = new AuthKey("SendOrderpublicMemoEmail");

        #endregion

        #region Delivery

        public readonly static AuthKey SO_Delivery_DeliveryAssign = new AuthKey("DeliveryAssign");

        #endregion

        #region SOInstalment

        /// <summary>
        /// 分期信息查看权限
        /// </summary>
        [Obsolete("此字段已弃用", true)]
        public readonly static AuthKey PrivilegeKey_SOInstalmentReView = new AuthKey("SOInstalmentReView");

        /// <summary>
        /// 分期信息维护权限
        /// </summary>
        [Obsolete("此字段已弃用", true)]
        public readonly static AuthKey PrivilegeKey_SOInstalmentMaintain = new AuthKey("SOInstalmentMaintain");

        /// <summary>
        /// 查看分期付款16位合同号的权限
        /// </summary>
        [Obsolete("此字段已弃用", true)]
        public readonly static AuthKey PrivilegeKey_CanSeeFullInstallmentContractNumber = new AuthKey("ViewFullContractNumber");

        #endregion

        #endregion SO Domain

        #region RMA Domain
        //派发
        public readonly static AuthKey RMA_RMATracking_Dispatch = new AuthKey("Dispatch");
        //取消派发
        public readonly static AuthKey RMA_RMATracking_CancelDispatch = new AuthKey("CancelDispatch");
        //导出全部Excel
        public readonly static AuthKey RMA_RMATracking_Export = new AuthKey("Export");
        //RMA跟进日志维护页面的 创建
        public readonly static AuthKey RMA_RMATracking_Edit_CanAdd = new AuthKey("CanAdd");
        //关闭
        public readonly static AuthKey RMA_RMATracking_Edit_CanClose = new AuthKey("CanClose");

        //
        public readonly static AuthKey RMA_RefundBalance_Create = new AuthKey("Create");
        public readonly static AuthKey RMA_RefundBalance_SubmitAudit = new AuthKey("SubmitAudit");
        public readonly static AuthKey RMA_RefundBalance_Refund = new AuthKey("Refund");
        public readonly static AuthKey RMA_RefundBalance_Abandon = new AuthKey("Abandon");

        public readonly static AuthKey RMA_Request_CanAdd = new AuthKey("CanAdd");
        public readonly static AuthKey RMA_Request_CanUpdate = new AuthKey("Update");
        //打印标签
        public readonly static AuthKey RMA_Request_PrintLabel = new AuthKey("PrintLabel");
        //打印
        public readonly static AuthKey RMA_Request_Print = new AuthKey("Print");
        public readonly static AuthKey RMA_Request_CanClose = new AuthKey("CanClose");
        public readonly static AuthKey RMA_Request_CanReceive = new AuthKey("CanReceive");
        public readonly static AuthKey RMA_Request_CanCancelReceive = new AuthKey("CanCancelReceive");
        public readonly static AuthKey RMA_Request_CanAbandon = new AuthKey("CanAbandon");

        public readonly static AuthKey RMA_Request_CanAdjust = new AuthKey("CanAudit");
        public readonly static AuthKey RMA_Request_CanRefused = new AuthKey("CanRefuesed");

        //更新单件信息
        public readonly static AuthKey RMA_Register_CanUpdate = new AuthKey("CanUpdate");
        //更新检测信息
        public readonly static AuthKey RMA_Register_CanUpdateCheckInfo = new AuthKey("CanUpdateCheckInfo");
        //更新送修返还信息
        public readonly static AuthKey RMA_Register_CanUpdateResponseInfo = new AuthKey("CanUpdateResponseInfo");
        public readonly static AuthKey RMA_Register_CanWaitingReturn = new AuthKey("CanWaitingReturn");
        //等待退款
        public readonly static AuthKey RMA_Register_CanWaitingRefund = new AuthKey("CanWaitingRefund");
        //待送修
        public readonly static AuthKey RMA_Register_CanSetWaitingOutBound = new AuthKey("CanSetWaitingOutBound");
        public readonly static AuthKey RMA_Register_CanClose = new AuthKey("CanClose");
        //打开单件
        public readonly static AuthKey RMA_Register_CanReOpen = new AuthKey("CanReOpen");
        //待发货
        public readonly static AuthKey RMA_Register_CanWaitingRevert = new AuthKey("CanWaitingRevert");
        //public readonly static AuthKey RMA_Register_RegisterSetAbandon = new AuthKey("RegisterSetAbandon");
        //单件发货状态审核
        public readonly static AuthKey RMA_Register_CanRevertAudit = new AuthKey("CanRevertAudit");
        //单件处理中心页面的导出全部
        public readonly static AuthKey RMA_Register_Export = new AuthKey("Export");
        //作废单件
        public readonly static AuthKey RMA_Register_CanSetAbandon = new AuthKey("RegisterSetAbandon");
        //RMA单件结案
        public readonly static AuthKey RMA_Register_RegisterCloseCase = new AuthKey("RegisterCloseCase");

        //退款单导出Excel
        public readonly static AuthKey RMA_Refund_Export = new AuthKey("Export");
        public readonly static AuthKey RMA_Refund_New = new AuthKey("CanNew");
        public readonly static AuthKey RMA_Refund_Update = new AuthKey("Update");
        public readonly static AuthKey RMA_Refund_Calculate = new AuthKey("Calculate");
        public readonly static AuthKey RMA_Refund_Abandon = new AuthKey("Abandon");
        public readonly static AuthKey RMA_Refund_SubmitAudit = new AuthKey("SubmitAudit");
        public readonly static AuthKey RMA_Refund_CancelAudit = new AuthKey("CancelAudit");
        public readonly static AuthKey RMA_Refund_Refund = new AuthKey("Refund");
        public readonly static AuthKey RMA_Refund_Print = new AuthKey("Print");

        #endregion RMA Domain

        #region Invoice Domain

        public readonly static AuthKey Invoice_SysAccountAddPoint_AddPoint = new AuthKey("AddPoint");

        //核对网上支付
        public readonly static AuthKey Invoice_NetPay_Abandon = new AuthKey("Abandon");//作废
        public readonly static AuthKey Invoice_NetPay_New = new AuthKey("InvoiceMgmt_NetPay_New");//新建
        public readonly static AuthKey Invoice_NetPay_Approve = new AuthKey("InvoiceMgmt_NetPay_Approve");//审核

        //礼品卡发票明细
        public readonly static AuthKey Invoice_GiftInvoiceDetailReport_Export = new AuthKey("Export");
        public readonly static AuthKey Invoice_GiftInvoiceDetailReport_Print = new AuthKey("Print");

        //移仓单明细表
        public readonly static AuthKey Invoice_ProductShiftDetailReport_ShiftForGIT = new AuthKey("ShiftForGIT");//导入金税库
        public readonly static AuthKey Invoice_ProductShiftDetailReport_ShiftForExe = new AuthKey("ShiftForExe");//导出

        //发票打印
        public readonly static AuthKey Invoice_InvoicePrintAll_Print = new AuthKey("Print");//打印
        public readonly static AuthKey Invoice_InvoicePrintAll_Export = new AuthKey("Export");//导入

        //自印发票系统查询
        public readonly static AuthKey Invoice_InvoiceSelfPrint_Export = new AuthKey("Export");

        //发表明细
        //public readonly static AuthKey Invoice_InvoiceDetailReport_UploadExcel = new AuthKey("UploadExcel");//
        public readonly static AuthKey Invoice_InvoiceDetailReport_Export = new AuthKey("Export");//导出
        //--页面级权限
        public readonly static AuthKey Invoice_InvoiceDetailReport_Import = new AuthKey("InvoiceMgmt_InvoiceReport_Import");

        //SAP确认人配置
        public readonly static AuthKey Invoice_SAPIPPUserQuery_Edit = new AuthKey("Edit");//编辑
       
        

        public readonly static AuthKey Invoice_NetPay_Approve_EmployeeAdd = new AuthKey("ApproveEmployeeAdd");
        public readonly static AuthKey Invoice_NetPay_Approve_WebSiteAdd = new AuthKey("ApproveWebSiteAdd");

        //销售收款单
        public readonly static AuthKey Invoice_SaleIncomeQuery_Confirm_AnyRefundType = new AuthKey("AnyRefundType");
        public readonly static AuthKey Invoice_SaleIncomeQuery_SetIncomeAmount = new AuthKey("SetIncomeAmount");
        public readonly static AuthKey Invoice_SaleIncomeQuery_SetReferenceID = new AuthKey("SetReferenceID");
        public readonly static AuthKey Invoice_SaleIncomeQuery_Confirm = new AuthKey("Confirm");
        public readonly static AuthKey Invoice_SaleIncomeQuery_CancelConfirm_InConfirmDay = new AuthKey("SOIncomeCancelConfirmInConfirmDay");
        public readonly static AuthKey Invoice_SaleIncomeQuery_CancelConfirm_AfterConfirmDay = new AuthKey("SOIncomeCancelConfirmAfterConfirmDay");
        public readonly static AuthKey Invoice_SaleIncomeQuery_Abandon = new AuthKey("Abandon");
        public readonly static AuthKey Invoice_SaleIncomeQuery_AutoConfirm = new AuthKey("AutoConfirm");
        public readonly static AuthKey Invoice_SaleIncomeQuery_Export = new AuthKey("Export");
        public readonly static AuthKey Invoice_SaleIncomeQuery_GetROExport = new AuthKey("GetROExport");
        public readonly static AuthKey Invoice_SaleIncomeQuery_SyncSAPSales = new AuthKey("SyncSAPSales");
        //销售收款单自动确认
        public readonly static AuthKey Invoice_SaleIncomeAutoConfirm_UploadExcel = new AuthKey("UploadExcel");
        public readonly static AuthKey Invoice_SaleIncomeAutoConfirm_SuccessConfirmExport = new AuthKey("SuccessConfirmExport");
        public readonly static AuthKey Invoice_SaleIncomeAutoConfirm_FaultConfirmExport = new AuthKey("FaultConfirmExport");


        //销售-分公司收款单
        public readonly static AuthKey Invoice_InvoiceQuery_Edit = new AuthKey("Edit");
        public readonly static AuthKey Invoice_InvoiceQuery_GetInvoiceAmount = new AuthKey("GetInvoiceAmount");
        public readonly static AuthKey Invoice_InvoiceQuery_Export = new AuthKey("Export");
        public readonly static AuthKey Invoice_InvoiceQuery_SyncSAPSales = new AuthKey("SyncSAPSales");
    

        //核对银行电汇
        public readonly static AuthKey Invoice_PostPay_Create = new AuthKey("Create");
        //核对POS支付
        public readonly static AuthKey Invoice_POSPay_Export = new AuthKey("Export");
        public readonly static AuthKey Invoice_POSPay_BatchConfirm = new AuthKey("BatchConfirm");

        //销售-电汇邮局收款单
        public readonly static AuthKey Invoice_PostIncomeQuery_Confirm = new AuthKey("Confirm");//确认
        public readonly static AuthKey Invoice_PostIncomeQuery_CancelConfrim = new AuthKey("CancelConfrim");//取消确认
        public readonly static AuthKey Invoice_PostIncomeQuery_Abandon = new AuthKey("Abandon");//作废
        public readonly static AuthKey Invoice_PostIncomeQuery_CancelAbandon = new AuthKey("CancelAbandon");//作废
        public readonly static AuthKey Invoice_PostIncomeQuery_Export = new AuthKey("Export");//导出

        public readonly static AuthKey Invoice_PostIncomeQuery_New = new AuthKey("InvoiceMgmt_PostIncome_New");

        public readonly static AuthKey Invoice_PostIncomeImport_Import = new AuthKey("UploadExcel");
        
        //编辑
        public readonly static AuthKey Invoice_PostIncomeQuery_Edit = new AuthKey("InvoiceMgmt_PostIncome_Edit");
        public readonly static AuthKey Invoice_PostIncomeQuery_View = new AuthKey("InvoiceMgmt_PostIncome_View");
       

        public readonly static AuthKey Invoice_PostIncomeConfirm = new AuthKey("Confirm");

        // 退款审核
        public readonly static AuthKey Invoice_AuditRefundQuery_UpdateRefundPayTypeAndReason = new AuthKey("UpdateRefundPayTypeAndReason");
        public readonly static AuthKey Invoice_AuditRefundQuery_Audit = new AuthKey("Audit");
        public readonly static AuthKey Invoice_AuditRefundQuery_AuditReject = new AuthKey("AuditReject");
        public readonly static AuthKey Invoice_AuditRefundQuery_FinPass = new AuthKey("FinPass");
        public readonly static AuthKey Invoice_AuditRefundQuery_FinRefuse = new AuthKey("FinRefuse");
        public readonly static AuthKey Invoice_AuditRefundQuery_CancelAudit = new AuthKey("CancelAudit");
        public readonly static AuthKey Invoice_AuditRefundQuery_Edit = new AuthKey("Edit");
        public readonly static AuthKey Invoice_AuditRefundQuery_AuditAutoRMA = new AuthKey("AuditAutoRMA");
        public readonly static AuthKey Invoice_AuditRefundQuery_Export = new AuthKey("ExportResultToExcel");
        public readonly static AuthKey Invoice_AuditRefundQuery_WLTRefundPoint = new AuthKey("WLTRefundPoint");

        //销售-逾期未收款订单
        public readonly static AuthKey Invoice_ARWindowQuery_UpdateTrackingInfo = new AuthKey("UpdateTrackingInfo");
        public readonly static AuthKey Invoice_ARWindowQuery_CreateTrackingInfo = new AuthKey("CreateTrackingInfo");
        public readonly static AuthKey Invoice_ARWindowQuery_SubmitTrackingInfo = new AuthKey("SubmitTrackingInfo");
        public readonly static AuthKey Invoice_ARWindowQuery_CloseTrackingInfo = new AuthKey("CloseTrackingInfo");
        public readonly static AuthKey Invoice_ARWindowQuery_ExportTrackingInfo = new AuthKey("ExportTrackingInfo");

        public readonly static AuthKey Invoice_ARWindowConfig_AbandonConfig = new AuthKey("AbandonConfig");
        public readonly static AuthKey Invoice_ARWindowConfig_CreateConfig = new AuthKey("CreateConfig");
        public readonly static AuthKey Invoice_ARWindowConfig_ExportConfig = new AuthKey("ExportConfig");

        public readonly static AuthKey Invoice_PayQuery_UpdateInvoiceStatus = new AuthKey("UpdateInvoice");
        public readonly static AuthKey Invoice_PayQuery_Export = new AuthKey("Export");
        public readonly static AuthKey Invoice_PayQuery_IsOnlyNegativeOrder = new AuthKey("OnlyNegativeOrder");
        //采购付款单
        public readonly static AuthKey Invoice_PayItemQuery_Pay = new AuthKey("Pay");
        public readonly static AuthKey Invoice_PayItemQuery_CancelPay = new AuthKey("CancelPay");
        public readonly static AuthKey Invoice_PayItemQuery_Abandon = new AuthKey("Abandon");
        public readonly static AuthKey Invoice_PayItemQuery_CancelAbandon = new AuthKey("CancelAbandon");

        public readonly static AuthKey Invoice_PayItemQuery_SetReferenceID = new AuthKey("SetReferenceID");
        public readonly static AuthKey Invoice_PayItemQuery_SetReferenceID_AllPath = new AuthKey("InvoiceMgmt_PayItem_Index@SetReferenceID");

        public readonly static AuthKey Invoice_PayItemQuery_UpdateInvoice = new AuthKey("UpdateInvoice");
        public readonly static AuthKey Invoice_PayItemQuery_Export = new AuthKey("Export");

        //采购付款单
        public readonly static AuthKey Invoice_PayItem_InvoiceInputMaintain_Insert = new AuthKey("Insert");
        public readonly static AuthKey Invoice_PayItem_InvoiceInputMaintain_Update = new AuthKey("Update");
        public readonly static AuthKey Invoice_PayItem_InvoiceInputMaintain_PayItemLock = new AuthKey("PayItemLock");
        public readonly static AuthKey Invoice_PayItem_InvoiceInputMaintain_CancelPayItemLock = new AuthKey("CancelPayItemLock");
        public readonly static AuthKey Invoice_PayItem_InvoiceInputMaintain_SetSAPVendor = new AuthKey("SetSAPVendor");
        public readonly static AuthKey Invoice_PayItem_InvoiceInputMaintain_SetSAPCompany = new AuthKey("SetSAPCompany");
        public readonly static AuthKey Invoice_PayItem_InvoiceInputMaintain_SetIPPUser = new AuthKey("SetIPPUser");




        public readonly static AuthKey Invoice_PayItemNew_PayItemLock = new AuthKey("PayItemLock");
        public readonly static AuthKey Invoice_PayItemNew_CancelPayItemLock = new AuthKey("CancelPayItemLock");


        //应付款-发票匹配审核
        public readonly static AuthKey Invoice_InvoiceInput_Export = new AuthKey("Export");
        public readonly static AuthKey Invoice_InvoiceInput_BatchVPCancel = new AuthKey("VendorPotalCancel");
        public readonly static AuthKey Invoice_InvoiceInput_BatchSubmit = new AuthKey("BatchSubmit");
        public readonly static AuthKey Invoice_InvoiceInput_BatchCancel = new AuthKey("BatchCancel");
        public readonly static AuthKey Invoice_InvoiceInput_BatchPass = new AuthKey("BatchPass");
        public readonly static AuthKey Invoice_InvoiceInput_BatchRefuse = new AuthKey("BatchRefuse");
        public readonly static AuthKey Invoice_InvoiceInput_ImportSAP = new AuthKey("ImportSAP");

        public readonly static AuthKey Invoice_InvoiceInputMaintain_Create = new AuthKey("Create");
        public readonly static AuthKey Invoice_InvoiceInputMaintain_Update = new AuthKey("Update");
        public readonly static AuthKey Invoice_InvoiceInputMaintain_VPCancel = new AuthKey("VendorPotalCancel");
        public readonly static AuthKey Invoice_InvoiceInputMaintain_Submit = new AuthKey("Submit");
        public readonly static AuthKey Invoice_InvoiceInputMaintain_Cancel = new AuthKey("Cancel");
        public readonly static AuthKey Invoice_InvoiceInputMaintain_Pass = new AuthKey("Pass");
        public readonly static AuthKey Invoice_InvoiceInputMaintain_Refuse = new AuthKey("Refuse");

        //客户余额退款
        public readonly static AuthKey Invoice_BalanceRefundQuery_CSAudit = new AuthKey("CSAudit");
        public readonly static AuthKey Invoice_BalanceRefundQuery_FinAudit = new AuthKey("Audit");
        public readonly static AuthKey Invoice_BalanceRefundQuery_CancelAudit = new AuthKey("CancelAudit");
        public readonly static AuthKey Invoice_BalanceRefundQuery_Abandon = new AuthKey("Abandon");
        public readonly static AuthKey Invoice_BalanceRefundQuery_SyncSAP = new AuthKey("SyncSAP");
        public readonly static AuthKey Invoice_BalanceRefundQuery_Export = new AuthKey("Export");
        public readonly static AuthKey Invoice_BalanceRefundQuery_SetReferenceID = new AuthKey("SetReferenceID");

        // 余额账户预收查询
        public readonly static AuthKey Invoice_BalanceAccountQuery_Export = new AuthKey("Export");
        public readonly static AuthKey Invoice_BalanceAccountQuery_Query = new AuthKey("Query");

        public readonly static AuthKey Invoice_SAP_SetSAPVendor = new AuthKey("SetSAPVendor");
        public readonly static AuthKey Invoice_SAP_SetSAPCompany = new AuthKey("SetSAPCompany");
        public readonly static AuthKey Invoice_SAP_SetIPPUser = new AuthKey("SetIPPUser");

        //以旧换新补贴款
        public readonly static AuthKey Invoice_UCOldChangeNew_Show = new AuthKey("Show");//查看
        public readonly static AuthKey Invoice_UCOldChangeNew_Print = new AuthKey("Print");//打印
        public readonly static AuthKey Invoice_UCOldChangeNew_BatchRequest = new AuthKey("BatchRequest");//批量提交
        public readonly static AuthKey Invoice_UCOldChangeNew_BatchDisRequest = new AuthKey("BatchDisRequest");//批量撤销
        public readonly static AuthKey Invoice_UCOldChangeNew_BatchPass = new AuthKey("BatchPass");//批量审核
        public readonly static AuthKey Invoice_UCOldChangeNew_BatchRefuse = new AuthKey("BatchRefuse");//批量拒绝
        public readonly static AuthKey Invoice_UCOldChangeNew_BatchReturnMoney = new AuthKey("BatchReturnMoney");//批量退款
        public readonly static AuthKey Invoice_UCOldChangeNew_BatchClose = new AuthKey("BatchClose");//批量关闭
        public readonly static AuthKey Invoice_UCOldChangeNew_BatchAbandon = new AuthKey("BatchAbandon");//批量作废
        public readonly static AuthKey Invoice_UCOldChangeNew_BatchMasterAbandon = new AuthKey("BatchMasterAbandon");//主管批量作废
        public readonly static AuthKey Invoice_UCOldChangeNew_Export = new AuthKey("Export");//导出
        //新加
        public readonly static AuthKey Invoice_UCOldChangeNew_Create = new AuthKey("InvoiceMgmt_OldChangeNew_New");//创建

        public readonly static AuthKey Invoice_UCOldChangeNew_MakeRequest = new AuthKey("InvoiceMgmt_OldChangeNew_New@MakeRequest");//创建
        public readonly static AuthKey Invoice_UCOldChangeNew_DisRequest = new AuthKey("InvoiceMgmt_OldChangeNew_New@DisRequest");//创建

        //供应商问题发票库
        public readonly static AuthKey Invoice_POVendorInvoiceQuery_Export = new AuthKey("Export");
        public readonly static AuthKey Invoice_POVendorInvoiceQuery_Abandon = new AuthKey("Abandon");
        public readonly static AuthKey Invoice_POVendorInvoiceQuery_Audit=new AuthKey("Audit");
        public readonly static AuthKey Invoice_POVendorInvoiceQuery_UnAbandon=new AuthKey("UnAbandon");
        public readonly static AuthKey Invoice_POVendorInvoiceQuery_UnAudit=new AuthKey("UnAudit");
        public readonly static AuthKey Invoice_POVendorInvoiceQuery_Edit = new AuthKey("InvoiceMgmt_POVendorInvoice_Edit");
        public readonly static AuthKey Invoice_POVendorInvoiceQuery_Print = new AuthKey("InvoiceMgmt_POVendorInvoice_Print");
        //供应商问题发票库 新建 原弹出页面权限
        public readonly static AuthKey Invoice_POVendorInvoiceQuery_Create = new AuthKey("InvoiceMgmt_POVendorInvoice_Edit@Create");
        public readonly static AuthKey Invoice_POVendorInvoiceQuery_Update = new AuthKey("InvoiceMgmt_POVendorInvoice_Edit@Update");

        //应付款汇总表

        public readonly static AuthKey Invoice_FinanceQuery_RefuseAudit = new AuthKey("RefuseAudit");
        public readonly static AuthKey Invoice_FinanceQuery_BatchPay = new AuthKey("BatchPay");
        public readonly static AuthKey Invoice_FinanceQuery_Export = new AuthKey("Export");
        public readonly static AuthKey Invoice_FinanceQuery_CaclMerged = new AuthKey("CaclMerged");
        public readonly static AuthKey Invoice_FinanceQuery_PMAudit = new AuthKey("PMAudit");
        public readonly static AuthKey Invoice_FinanceQuery_FNAudit = new AuthKey("FNAudit");

        public readonly static AuthKey Invoice_FinanceQuery_PM = new AuthKey("PMManager");

        public readonly static AuthKey Invoice_PriceChange_Audit = new AuthKey("Invoice_PriceChange_Audit");

        #endregion Invoice Domain

        #region MKT Domain

        #region 礼品卡

        public readonly static AuthKey MKT_GiftCard_FabricationCreateMaintain = new AuthKey("FabricationCreateMaintain");

        public readonly static AuthKey MKT_GiftCard_FabricationUpdateMaintain = new AuthKey("FabricationUpdateMaintain");

        public readonly static AuthKey MKT_GiftCard_FabricationPOMaintain = new AuthKey("FabricationPOMaintain");

        public readonly static AuthKey MKT_GiftCard_FabricationExportMaintain = new AuthKey("FabricationExportMaintain");

        public readonly static AuthKey MKT_GiftCard_FabricationDeleteMaintain = new AuthKey("FabricationDeleteMaintain");

        #endregion

        #region 团购

        public readonly static AuthKey MKT_GroupBuying_Approve = new AuthKey("ProductGroupBuyingApprove");

        #endregion

        #region 赠品

        public readonly static AuthKey MKT_Gift_Approve = new AuthKey("SaleGiftRequestApprove");

        public readonly static AuthKey MKT_Gift_IsSpecialCheck = new AuthKey("GiftEditIsSpecial_Check");

        #endregion

        #region 分类关键字

        public readonly static AuthKey MKT_CategoryKeywords_CreateCommonKeywords = new AuthKey("CreateCommonKeywords");

        public readonly static AuthKey MKT_CategoryKeywords_CreatePropertyKeywords = new AuthKey("CreatePropertyKeywords");

        #endregion

        #region 产品页面关键字

        public readonly static AuthKey MKT_ProductKeyWords_ProductKeyImportMaintain = new AuthKey("ProductKeyImportMaintain");

        public readonly static AuthKey MKT_ProductKeyWords_ProductKeyMaintain = new AuthKey("ProductKeyMaintain");

        #endregion

        #region 前台显示类型管理

        /// <summary>
        /// 两个权限都有，才能对前台显示类别进行操作
        /// </summary>
        public readonly static AuthKey MKT_ECCategory_Btn_Aud_Exec = new AuthKey("Btn_Aud_Exec");
        public readonly static AuthKey MKT_ECCategory_CategoryCustomized = new AuthKey("CategoryCustomized");

        #endregion 前台显示类型管理

        #region 随心配

        public readonly static AuthKey MKT_OptionalAccessories_ApproveMaintain = new AuthKey("OptionalAccessoriesApproveMaintain");

        #endregion

        #region 置顶商品管理

        public readonly static AuthKey MKT_TopItem_BatchMaintain = new AuthKey("TopItemBatchMaintian");

        #endregion

        #region 中文词库关键字

        public readonly static AuthKey MKT_Segment_SegmentAudit = new AuthKey("SegmentAudit");

        public readonly static AuthKey MKT_Segment_SegmentDelete = new AuthKey("SegmentDelete");

        #endregion

        #region DIY装机

        public readonly static AuthKey MKT_ComputerConfig_ApproveMaintain = new AuthKey("ComputerConfigApproveMaintain");

        #endregion

        #region 限时抢购&限时促销
        //限时抢购
        public readonly static AuthKey MKT_Countdown_PrimaryReadCountDown_Check = new AuthKey("PrimaryReadCountDown_Check");//商品初级查看权限
        public readonly static AuthKey MKT_Countdown_AdvancedReadCountDown_Check = new AuthKey("AdvancedReadCountDown_Check");//商品高级查看权限
        public readonly static AuthKey MKT_Countdown_PrimaryAuditCountDown_Check = new AuthKey("PrimaryAuditCountDown_Check");//限时抢购初级审核权限
        public readonly static AuthKey MKT_Countdown_AdvancedAuditCountDown_Check = new AuthKey("AdvancedAuditCountDown_Check");//限时抢购高级审核权限
        public readonly static AuthKey MKT_Countdown_CountDownUpdate_Check = new AuthKey("CountDownUpdate_Check");
        public readonly static AuthKey MKT_Countdown_CountDownUpdateAfterVerify_Check = new AuthKey("CountDownUpdateAfterVerify_Check");
        public readonly static AuthKey MKT_Countdown_CountDownVerify_Check = new AuthKey("CountDownVerify_Check");
        public readonly static AuthKey MKT_Countdown_CountDownInterrupt_Check = new AuthKey("CountDownInterrupt_Check");
        public readonly static AuthKey MKT_Countdown_CountdownUserRightOnAdd_Check = new AuthKey("CountdownUserRightOnAdd_Check");
        public readonly static AuthKey MKT_Countdown_CountdownUserRightOnUpdate_Check = new AuthKey("CountdownUserRightOnUpdate_Check");
        public readonly static AuthKey MKT_Countdown_CountdownUserRightOnVerify_Check = new AuthKey("CountdownUserRightOnVerify_Check");
        public readonly static AuthKey MKT_Countdown_CountdownUserRightOnInterrupt_Check = new AuthKey("CountdownUserRightOnInterrupt_Check");
        public readonly static AuthKey MKT_Countdown_CountdownAreaShow_Check = new AuthKey("CountdownAreaShow_Check");
        public readonly static AuthKey MKT_Countdown_CountdownIsHomePageShow_Check = new AuthKey("CountdownIsHomePageShow_Check");
        public readonly static AuthKey MKT_Countdown_CountdownHasTodaySpecial_Check = new AuthKey("CountdownHasTodaySpecial_Check");
        public readonly static AuthKey MKT_Countdown_UserHasGroupOnRightCheck = new AuthKey("UserHasGroupOnRightCheck");

        //限时促销
        public readonly static AuthKey MKT_PromotionSchedule_PrimaryAuditPromotionSchedule_Check = new AuthKey("PrimaryAuditPromotionSchedule_Check");//限时促销初级审核权限
        public readonly static AuthKey MKT_PromotionSchedule_AdvancedAuditPromotionSchedule_Check = new AuthKey("AdvancedAuditPromotionSchedule_Check");//限时促销高级审核权限
        public readonly static AuthKey MKT_PromotionSchedule_PromotionScheduleUserRightOnAdd_Check = new AuthKey("PromotionScheduleUserRightOnAdd_Check");
        public readonly static AuthKey MKT_PromotionSchedule_PromotionScheduleUserRightOnUpdate_Check = new AuthKey("PromotionScheduleUserRightOnUpdate_Check");
        public readonly static AuthKey MKT_PromotionSchedule_PromotionScheduleUserRightOnVerify_Check = new AuthKey("PromotionScheduleUserRightOnVerify_Check");
        public readonly static AuthKey MKT_PromotionSchedule_PromotionScheduleUserRightOnInterrupt_Check = new AuthKey("PromotionScheduleUserRightOnInterrupt_Check");
        public readonly static AuthKey MKT_PromotionSchedule_PromotionScheduleUserRightOnWaitForVerify_Check = new AuthKey("PromotionScheduleUserRightOnWaitForVerify_Check");
        public readonly static AuthKey MKT_PromotionSchedule_CountdownHasTodaySpecial_Check = new AuthKey("CountdownHasTodaySpecial_Check");

        #endregion

        #region 评分项定义

        public readonly static AuthKey MKT_ReviewScore_Add = new AuthKey("Btn_ReviewScoreItem_Add");

        #endregion

        #region 优惠券


        public readonly static AuthKey MKT_CouponCode_Approve = new AuthKey("CouponApprove_Check");

        public readonly static AuthKey MKT_CouponCode_Edit = new AuthKey("CouponEdit_Check");

        public readonly static AuthKey MKT_CouponCodeStop_Approve = new AuthKey("CouponApproveStop_Check");

        #endregion

        #region 页面促销模板

        public readonly static AuthKey MKT_SaleAdvTemplate_Hold = new AuthKey("SaleAdvTemplateEditIsHold");

        public readonly static AuthKey MKT_SaleAdvTemplate_Save = new AuthKey("SaleAdvTemplateEditSave");

        #endregion

        #region SEO

        public readonly static AuthKey MKT_SEO_SEOMetadataEdit = new AuthKey("SEOMetadataEdit");

        #endregion

        #region 咨询管理

        public readonly static AuthKey MKT_ProductConsult_BatchApprove = new AuthKey("ProductConsultListActive");

        public readonly static AuthKey MKT_ProductConsult_BatchCancel = new AuthKey("ProductConsultListCancel");

        public readonly static AuthKey MKT_ProductConsult_BatchRead = new AuthKey("ProductConsultListRead");

        public readonly static AuthKey MKT_ProductConsult_ReplyBatchApprove = new AuthKey("ProductReplyListActive");

        public readonly static AuthKey MKT_ProductConsult_ReplyBatchCancel = new AuthKey("ProductReplyListCancel");

        public readonly static AuthKey MKT_ProductConsult_ReplyBatchRead = new AuthKey("ProductReplyListRead");

        #endregion

        #region 评论管理

        public readonly static AuthKey MKT_ProductReview_Approve = new AuthKey("Btn_Aud_Aud");

        public readonly static AuthKey MKT_ProductReview_ReplyApprove = new AuthKey("Btn_Aud_Reply_Aud");

        public readonly static AuthKey MKT_ProductReview_Cancel = new AuthKey("Btn_Expire_Aud");

        public readonly static AuthKey MKT_ProductReview_ReplyCancel = new AuthKey("Btn_Expire_Reply_Aud");

        public readonly static AuthKey MKT_ProductReview_Read = new AuthKey("Btn_Read_Aud");

        public readonly static AuthKey MKT_ProductReview_ReplyRead = new AuthKey("Btn_Read_Reply_Aud");

        public readonly static AuthKey MKT_ProductReview_Export = new AuthKey("ProductExportExcelRight");

        #endregion

        #region 讨论管理

        public readonly static AuthKey MKT_ProductDiscuss_Approve = new AuthKey("Btn_Aud_Aud");

        public readonly static AuthKey MKT_ProductDiscuss_ReplyApprove = new AuthKey("Btn_Aud_Reply_Aud");

        public readonly static AuthKey MKT_ProductDiscuss_Cancel = new AuthKey("Btn_Expire_Aud");

        public readonly static AuthKey MKT_ProductDiscuss_ReplyCancel = new AuthKey("Btn_Expire_Reply_Aud");

        public readonly static AuthKey MKT_ProductDiscuss_Read = new AuthKey("Btn_Read_Aud");

        public readonly static AuthKey MKT_ProductDiscuss_ReplyRead = new AuthKey("Btn_Read_Reply_Aud");

        public readonly static AuthKey MKT_ProductDiscuss_Export = new AuthKey("ProductExportExcelRight");

        #endregion

        #region 广告商管理

        public readonly static AuthKey MKT_Advertisers_BatchApprove = new AuthKey("BatchUpdateStatus");

        public readonly static AuthKey MKT_Advertisers_Save = new AuthKey("MaintainAdvertisers");

        #endregion

        #endregion

        #region ExternalSYS Domain

        #region CPS

        #endregion

        #region EIMS

        #region 应计报表
        /// <summary>
        /// 应计返利报表（周期）数据导出
        /// </summary>
        public readonly static AuthKey EIMS_AccruedByPeriod_Export = new AuthKey("Export");

        /// <summary>
        /// 应计返利报表（PM）数据导出
        /// </summary>
        public readonly static AuthKey EIMS_AccruedByPM_Export = new AuthKey("Export");

        /// <summary>
        /// 应计返利报表（合同）数据导出
        /// </summary>
        public readonly static AuthKey EIMS_AccruedByRule_Export = new AuthKey("Export");

        /// <summary>
        /// 应计返利报表（供应商）数据导出
        /// </summary>
        public readonly static AuthKey EIMS_AccruedByVendor_Export = new AuthKey("Export");
        #endregion

        #region 收款报表
        /// <summary>
        /// 应收帐单（单据）数据导出
        /// </summary>
        public readonly static AuthKey EIMS_ARReceive_Export = new AuthKey("Export");

        /// <summary>
        /// 供应商对帐单 数据导出
        /// </summary>
        public readonly static AuthKey EIMS_ReceiveByVendor_Export = new AuthKey("Export");

        /// <summary>
        /// 收款报表 数据导出
        /// </summary>
        public readonly static AuthKey EIMS_ReceiveByYear_Export = new AuthKey("Export");
        #endregion

        #region 综合报表
        /// <summary>
        /// 合同与对应单据 数据导出
        /// </summary>
        public readonly static AuthKey EIMS_UnbilledRuleList_Export = new AuthKey("Export");

        /// <summary>
        /// 综合报表 数据导出
        /// </summary>
        public readonly static AuthKey EIMS_Comprehensive_Export = new AuthKey("Export");

        /// <summary>
        /// EIMS单据报表 数据导出
        /// </summary>
        public readonly static AuthKey EIMS_Invoice_Export = new AuthKey("Export");
        #endregion

        #region 单据管理
        /// <summary>
        /// EIMS结算类型变更单据查询 数据导出
        /// </summary>
        public readonly static AuthKey EIMS_EventMemo_Export = new AuthKey("Export");

        /// <summary>
        /// EIMS发票录入
        /// </summary>
        public readonly static AuthKey EIMS_InvoiceEntry_Input = new AuthKey("Input");

        /// <summary>
        /// EIMS发票批量录入
        /// </summary>
        public readonly static AuthKey EIMS_InvoiceEntry_BathcInput = new AuthKey("BatchInput");

        /// <summary>
        /// EIMS发票录入编辑
        /// </summary>
        public readonly static AuthKey EIMS_InvoiceEntry_Edit = new AuthKey("Edit");

        #endregion

        #endregion

        #region VendorPortal
        /// <summary>
        /// 商家管理 模版下载
        /// </summary>
        public readonly static AuthKey Vendor_ProductTemplate_Download = new AuthKey("Download");

        /// <summary>
        /// 商家管理 角色管理 编辑
        /// </summary>
        public readonly static AuthKey Vendor_Role_Edit = new AuthKey("Edit");

        /// <summary>
        /// 商家管理 角色管理 添加角色
        /// </summary>
        public readonly static AuthKey Vendor_Role_Add = new AuthKey("Add");

        /// <summary>
        /// 商家管理 角色管理 批量生效
        /// </summary>
        public readonly static AuthKey Vendor_Role_BatchEffect = new AuthKey("BatchEffect");

        /// <summary>
        /// 商家管理 角色管理 批量作废
        /// </summary>
        public readonly static AuthKey Vendor_Role_BatchAbandon = new AuthKey("BatchAbandon");

        /// <summary>
        /// 角色管理 添加角色 保存
        /// </summary>
        public readonly static AuthKey Vendor_RoleAdd_Save = new AuthKey("Save");

        /// <summary>
        /// 角色管理 添加角色 作废
        /// </summary>
        public readonly static AuthKey Vendor_RoleAdd_Abandon = new AuthKey("Abandon");

        /// <summary>
        /// 角色管理 添加角色 生效
        /// </summary>
        public readonly static AuthKey Vendor_RoleAdd_Effect = new AuthKey("Effect");

        /// <summary>
        /// 商家管理 账户管理 添加账户
        /// </summary>
        public readonly static AuthKey Vendor_UserMgmt_Add = new AuthKey("Add");

        /// <summary>
        /// 商家管理 账户管理 批量生效
        /// </summary>
        public readonly static AuthKey Vendor_UserMgmt_BatchEffect = new AuthKey("BatchEffect");

        /// <summary>
        /// 商家管理 账户管理 批量作废
        /// </summary>
        public readonly static AuthKey Vendor_UserMgmt_BatchAbandon = new AuthKey("BatchAbandon");

        /// <summary>
        /// 商家管理 账户管理 编辑
        /// </summary>
        public readonly static AuthKey Vendor_UserMgmt_Edit = new AuthKey("Edit");

        /// <summary>
        /// 商家管理 账户管理 添加账户 保存
        /// </summary>
        public readonly static AuthKey Vendor_UserMgmt_Save = new AuthKey("Save");

        /// <summary>
        /// 商家管理 账户管理 添加账户 生效
        /// </summary>
        public readonly static AuthKey Vendor_UserMgmt_Effect = new AuthKey("Effect");

        /// <summary>
        /// 商家管理 系统日志 点击详情
        /// </summary>
        public readonly static AuthKey Vendor_PortalLog_Details = new AuthKey("Details");

        #endregion

        #endregion
    }

    public static class AuthMgr
    {
        public static bool HasFunction(AuthKey authKey)
        {
            //return true; // 这行为临时代码，等修改了SilverlightFramework后回删除该代码，并把下面代码注释去掉
            var authMenuItems = ComponentFactory.GetComponent<IAuth>().GetAuthorizedMenuItems();
            foreach (var item in authMenuItems)
            {
                if (item.AuthKey == authKey.Key)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 获取KeyStore中的绝对权限(不添加路径和@)
        /// </summary>
        public static bool HasFunctionAbsolute(AuthKey authKey)
        {
            return ComponentFactory.GetComponent<IAuth>().HasFunction(authKey.Key);
        }

        public static bool HasFunctionPoint(AuthKey authKey)
        {
            //return true; // 这行为临时代码，等修改了SilverlightFramework后回删除该代码，并把下面代码注释去掉
            string pointKey = authKey.Key;
            Request request = CPApplication.Current.CurrentPage.Context.Request;
            string linkPath = string.Format("/{0}/{1}", request.ModuleName, request.ViewName);
            string pageKey;
            if (!TryFindAuthKeyOfLinkPath(linkPath, out pageKey)) // 没有找到对应的MenuItem，说明该页面都没有访问权限
            {
                return false;
            }
            pageKey = (pageKey == null || pageKey.Trim().Length == 0) ? string.Empty : pageKey.Trim() + "@";
            return ComponentFactory.GetComponent<IAuth>().HasFunction(pageKey + pointKey);
        }

        private static bool TryFindAuthKeyOfLinkPath(string linkPath, out string authKey)
        {
            var authMenuItems = ComponentFactory.GetComponent<IAuth>().AuthorizedNavigateToList();
            foreach (var item in authMenuItems)
            {
                if (item.URL == linkPath)
                {
                    authKey = item.AuthKey;
                    return true;
                }
            }
            authKey = null;
            return false;
        }
    }

    public static class AuthKeyControlMgr
    {
        /// <summary>
        /// 根据权限验证控件的显示状态
        /// </summary>
        /// <param name="key">多组权限，以交集验证，如果有一个没通过，都将不显示</param>
        /// <returns></returns>
        public static Visibility GetVisibilityByRight(params AuthKey [] key)
        {
            var result = Visibility.Visible;
            foreach (var item in key)
            {
                if (!AuthMgr.HasFunctionPoint(item))
                {
                    result = Visibility.Collapsed;
                    break;
                }
            }
            
            return result;
        }

        public static void CollapsedWhenNotRight(this UIElement control, params AuthKey[] key)
        {
            if (control.Visibility == Visibility.Visible)
            {
                control.Visibility = GetVisibilityByRight(key);
            }
        }
    }
}
