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

namespace ECCentral.Portal.Basic
{
    public partial class ConstValue
    {
        /// <summary>
        /// 全局的业务多语言代码，由于目前一期不在Portal端实现多语言，
        /// 但是Service端的业务模型是支持多语言的，所以Portal端需要传输Biz LanguageCode的地方请取本值。
        /// </summary>
        public const string BizLanguageCode = "zh-CN";

        /// <summary>
        /// 在导出Excel选择导出全部时，所允许导出的最大数据条数
        /// </summary>
        public const int MaxRowCountLimit = 10001; // 比Service端的限制多1（Service端的限制为10000）

        /// <summary>
        /// 服务端基础地址键名.
        /// </summary>
        public const string Key_ServiceBaseUrl = "ServiceBaseUrl";
        public const string DomainName_Common = "Common";
        public const string Key_SearchMode = "SearchMode";
        /// <summary>
        /// 小于1000000的正两位小数验证
        /// </summary>
        public const string Format_Money = @"^((\d(\d{0,5})?(\.|(\.\d{0,2}0*))?)|(0?.\d{1,2}0*))?$";

        #region Common Domain

        public const string Key_Compare = "Compare";
        public const string Key_ControlPanelDept = "ControlPanelDept";
        public const string Common_AreaMaintainUrlFormat = "/ECCentral.Portal.UI.Common/AreaMaintain";
        public const string Common_CurrencyMaintainUrlFormat = "/ECCentral.Portal.UI.Common/CurrencyMaintain";
        public const string Common_ShipTypeProductAddnewUrlFormat = "/ECCentral.Portal.UI.Common/ShipTypeProductAddNew";
        public const string Common_ShipTypeAreaUnAddnewUrlFormat = "/ECCentral.Portal.UI.Common/ShipTypeAreaUnAddNew";
        public const string Key_BizObjectType = "BizObjectType";
        public const string LanguageOptions = "LanguageOptions";

        public const string ISAAccountKey = "ISAAccount";
        public const string ISAPasswordKey = "ISAPassword";
        public const string ISADomainKey = "ISADomain";

        #endregion Common Domain

        #region Customer Domain

        public const string DomainName_Customer = "Customer";
        public const string CustomerMaintainUrlFormat = "/ECCentral.Portal.UI.Customer/CustomerMaintain/{0}";

        public const string CustomerExperienceLogQueryUrlFormat = "/ECCentral.Portal.UI.Customer/CustomerExperienceLogQuery/{0}";

        public const string CustomerPointLogQueryUrlFormat = "/ECCentral.Portal.UI.Customer/CustomerPointLogQuery/{0}";

        public const string CustomerMaintainCreateUrl = "/ECCentral.Portal.UI.Customer/CustomerMaintain";

        public const string CustomerCouponLogUrlFormat = "/ECCentral.Portal.UI.Customer/CouponCustomerLog/{0}";

        public const string CreateSoByCustomerUrlFormat = "/ECCentral.Portal.UI.SO/SOMaintain/?CustomerSysNo={0}";

        public const string CustomerGiftMaintainUrl = "/ECCentral.Portal.UI.Customer/CustomerGiftMaintain";

        public const string CustomerRightMaintainUrlFormat = "/ECCentral.Portal.UI.Customer/CustomerRightMaintain/{0}";

        public const string CustomerPointsAddQuery = "/ECCentral.Portal.UI.Customer/CustomerPointsAddQuery";

        public const string Customer_RefundAdjustUrl = "/ECCentral.Portal.UI.Customer/RefundAdjustQuery/{0}";
        /// <summary>
        /// 优惠券详细页面
        /// </summary>
        public const string CouponDetailUrlFormat = "/ECCentral.Portal.UI.Customer/CouponDetail/{0}";

        public const string CustomerCalingMaintain = "/ECCentral.Portal.UI.Customer/CustomerCalingMaintain";
        public const string CustomerCalingComplainAdd = "/ECCentral.Portal.UI.Customer/ComplainAdd/{0}";
        public const string CustomerCalingRMAAdd = "/ECCentral.Portal.UI.Customer/RMAAdd/{0}";

        public const string MaliceUserQueryUrlFormat = "/ECCentral.Portal.UI.Customer/MaliceUserQuery/{0}";

        public const string Customer_OrderCheckKeywordSetUrl = "/ECCentral.Portal.UI.Customer/KeywordSet/{0}";
        public const string Customer_OrderCheckAmountSetUrl = "/ECCentral.Portal.UI.Customer/AmountSet/{0}";
        public const string Customer_OrderCheckAutoCheckTimeSetUrl = "/ECCentral.Portal.UI.Customer/AutoCheckTimeSet/{0}";
        public const string Customer_OrderCheckShippingTypeSetUrl = "/ECCentral.Portal.UI.Customer/ShippingTypeSet/{0}";
        public const string Customer_OrderCheckPayTypeSetUrl = "/ECCentral.Portal.UI.Customer/PayTypeSet/{0}";
        public const string Customer_OrderCheckFPSetUrl = "/ECCentral.Portal.UI.Customer/FPSet/{0}";
        public const string Customer_OrderCheckCustomerTypeSetUrl = "/ECCentral.Portal.UI.Customer/CustomerTypeSet/{0}";
        public const string Customer_OrderCheckDistributionServiceTypeSetUrl = "/ECCentral.Portal.UI.Customer/DistributionServiceTypeSet/{0}";
        public const string Customer_OrderCheckProductAnd3CSetUrl = "/ECCentral.Portal.UI.Customer/ProductAnd3CSet/{0}";
        public const string Customer_OrderCheckPCSetUrl = "/ECCentral.Portal.UI.Customer/PCSet/{0}";

        public const string Customer_FPCheck_CH = "/ECCentral.Portal.UI.Customer/CHSet/{0}";
        public const string Customer_FPCheck_CC = "/ECCentral.Portal.UI.Customer/CCSet/{0}";
              
        #endregion Customer Domain

        #region SO Domain

        public const string DomainName_SO = "SO";
        public const string Key_FPStatus = "FPStatus";
        public const string Key_KFCType = "KFCType";
        public const string Key_TimeRange = "TimeRange";
        public const string Key_DeliveryDayRangeType = "DeliveryDayRangeType";
        public const string Key_ShipTypeFilter = "ShipTypeFilter";
        public const string Key_HasTrackingNumber = "HasTrackingNumber";
        public const string Key_ShipTimeType = "ShipTimeType";
        public const string Key_SOInernalMemoSource = "SOInernalMemoSource";
        public const string Key_CallBackDegree = "CallBackDegree";
        public const string Key_SOCallType = "CallType";
        public const string Key_ComplainType = "ComplainType";
        public const string Key_SOComplainSourceType = "ComplainSourceType";
        public const string Key_SOResponsibleDept = "ResponsibleDept";
        public const string Key_DeliveryExpOrderType = "DeliveryExp_OrderTye";
        public const string Key_SOCreateResultType = "CreateResultType";
        public const string Key_SOStatusSyncResultType = "StatusSyncResultType";
        public const string Key_SOIsExpiateOrder = "IsExpiateOrder";
        public const string Key_SOIsPhoneOrder = "IsPhoneOrder";
        public const string Key_NeedInvoice = "NeedInvoice";

        public const string SOMaintainUrl = "/ECCentral.Portal.UI.SO/SOMaintain";
        public const string SOMaintainUrlFormat = "/ECCentral.Portal.UI.SO/SOMaintain/?SOSysNo={0}";
        public const string SOMaintainUrlOtherInfoFormat = "/ECCentral.Portal.UI.SO/SOMaintain/?OtherInfo={0}";
        public const string SOProcessUrlFormat = "/ECCentral.Portal.UI.SO/SOProcess/{0}";
        public const string SO_ComplainReplyUrl = "/ECCentral.Portal.UI.SO/ComplainReply/{0}";
        public const string SO_SOInternalMemoDetailUrl = "/ECCentral.Portal.UI.SO/SOInternalMemo/{0}";
        public const string SO_SOAddpublicMemoUrlFormat = "/ECCentral.Portal.UI.SO/AddSOInternalMemo/{0}";        
        /// <summary>
        /// 订单投诉
        /// </summary>
        public const string SOComplainReplyFormat ="/ECCentral.Portal.UI.SO/ComplainReply/{0}";///Model=2

        public const string SOElectronicCard_ProductSysNo = "SOElectronicCard_ProductSysNo";

        public const string SOElectronicCard_ProductID = "SOElectronicCard_ProductID";

        public const string SOElectronicCard_ProductName = "SOElectronicCard_ProductName";
        
        public const string SOPhysicalCard_ProductID_Prefix = "SOPhysicalCard_ProductID_Prefix";

        public const string SZYTPrePayCardPayType = "SZYTPrePayCardPayType";
        
        #endregion SO Domain

        #region RMA Domain

        /// <summary>
        /// RMA DomainName
        /// </summary>
        public const string DomainName_RMA = "RMA";
        public const string RMA_CreateRequestUrl = "/ECCentral.Portal.UI.RMA/CreateRMARequest";
        public const string RMA_CreateRefundUrl = "/ECCentral.Portal.UI.RMA/CreateRMARefund";
        public const string RMA_EditRegisterUrl = "/ECCentral.Portal.UI.RMA/RegisterMaintain/{0}";
        public const string RMA_TrackingMaintainUrl = "/ECCentral.Portal.UI.RMA/RMATrackingMaintain/{0}";
        public const string RMA_RequestMaintainUrl = "/ECCentral.Portal.UI.RMA/RMARequestMaintain/{0}";
        public const string RMA_RegisterMaintainUrl = "/ECCentral.Portal.UI.RMA/RegisterMaintain/{0}";
        public const string RMA_RefundMaintainUrl = "/ECCentral.Portal.UI.RMA/RMARefundMaintain/{0}";
        public const string RMA_RegisterQueryUrl = "/ECCentral.Portal.UI.RMA/RegisterQuery/{0}";
        public const string RMA_RefundBalanceQueryUrl = "/ECCentral.Portal.UI.RMA/RefundBalanceQuery/{0}";
        public const string RMA_PrintRequestUrl = "{0}/Pages/RMA/PrintRequest.aspx?sysNo={1}";
        public const string RMA_PrintLabelUrl = "{0}/Pages/RMA/PrintLabel.aspx?sysNo={1}";
        public const string RMA_PrintRegisterUrl = "{0}/Pages/RMA/PrintRegister.aspx?sysNo={1}";
        public const string RMA_PrintRefundUrl = "{0}/Pages/RMA/PrintRefund.aspx?sysNo={1}";
        public const string RMA_RegisterMemoUrl = "/ECCentral.Portal.UI.RMA/RegisterMemo/{0}";
        #endregion RMA Domain

        #region MKT Domain

        public const string DomainName_MKT = "MKT";
        /// <summary>
        /// 统一图片上传跟目录
        /// </summary>
        public const string MKT_UnifiedImageFolder = "UnifiedImage";
        /// <summary>
        /// 楼层Section管理
        /// </summary>
        public const string MKT_FloorSectionMaintainUrlFormat = "/ECCentral.Portal.UI.MKT/FloorSectionMaintain/{0}";
        
        /// <summary>
        /// 帮助中心
        /// </summary>
        public const string MKT_HelpCenterMaintainUrlFormat = "/ECCentral.Portal.UI.MKT/HelpCenterMaintain/{0}";
        /// <summary>
        ///商品推荐
        /// </summary>
        public const string MKT_ProductRecommendMaintainUrlFormat = "/ECCentral.Portal.UI.MKT/ProductRecommendMaintain/{0}";
        /// <summary>
        /// 广告管理
        /// </summary>
        public const string MKT_BannerMaintainUrlFormat = "/ECCentral.Portal.UI.MKT/BannerMaintain/{0}";
        /// <summary>
        /// 团购
        /// </summary>
        public const string MKT_GroupBuyingMaintainUrlFormat = "/ECCentral.Portal.UI.MKT/GroupBuyingMaintain/{0}";

        /// <summary>
        /// 团购
        /// </summary>
        public const string MKT_GroupBuyingMaintainUrlFormatEdit = "/ECCentral.Portal.UI.MKT/GroupBuyingMaintain/?op=edt&sysNo={0}";
        /// <summary>
        /// 优惠套餐
        /// </summary>
        public const string MKT_ComboSaleMaintainUrlFormat = "/ECCentral.Portal.UI.MKT/ComboSaleMaintain/{0}";
        /// <summary>
        /// 随心配
        /// </summary>
        public const string MKT_OptionalAccessoriesMaintainUrlFormat = "/ECCentral.Portal.UI.MKT/OptionalAccessoriesMaintain/{0}";
        /// <summary>
        /// 优惠券
        /// </summary>
        public const string MKT_CouponMaintainUrlFormat = "/ECCentral.Portal.UI.MKT/CouponMaintain";

        /// <summary>
        /// 优惠券
        /// </summary>
        public const string MKT_CouponMaintainUrlFormatEdit = "/ECCentral.Portal.UI.MKT/CouponMaintain?sysno={0}&operation=edit";
        /// <summary>
        /// 优惠券领取日志查询
        /// </summary>
        public const string MKT_CouponCodeCustomerLogMaintainUrlFormat = "/ECCentral.Portal.UI.MKT/CouponCodeCustomerLogQuery";
        /// <summary>
        /// 优惠券使用记录查询
        /// </summary>
        public const string MKT_CouponCodeRedeemLogMaintainUrlFormat = "/ECCentral.Portal.UI.MKT/CouponCodeRedeemLogQuery";
        /// <summary>
        /// 赠品
        /// </summary>
        public const string MKT_SaleGiftMaintainUrlFormat = "/ECCentral.Portal.UI.MKT/SaleGiftMaintain";

        /// <summary>
        /// 赠品编辑
        /// </summary>
        public const string MKT_SaleGiftMaintainUrlFormatEdit = "/ECCentral.Portal.UI.MKT/SaleGiftMaintain?sysno={0}&operation=edit";

        /// <summary>
        /// 销售规则
        /// </summary>
        public const string MKT_ComboSaleMaintain = "/ECCentral.Portal.UI.MKT/ComboSaleMaintain/{0}";
        /// <summary>
        /// 赠品批量创建
        /// </summary>
        public const string MKT_BatchCreateSaleGiftUrlFormat = "/ECCentral.Portal.UI.MKT/BatchCreateSaleGift";
        /// <summary>
        /// 限时抢购，促销计划，秒杀
        /// </summary>
        public const string MKT_CountdownMaintainUrlFormat = "/ECCentral.Portal.UI.MKT/CountdownMaintain/{0}";
        /// <summary>
        /// 页面促销模板信息维护
        /// </summary>
        public const string MKT_SaleAdvTemplateMaintainUrlFormat = "/ECCentral.Portal.UI.MKT/SaleAdvTemplateMaintain/{0}";
        /// <summary>
        /// 页面促销模板商品维护
        /// </summary>
        public const string MKT_SaleAdvTemplateItemMaintainUrlFormat = "/ECCentral.Portal.UI.MKT/SaleAdvTemplateItemMaintain/{0}";
        /// <summary>
        /// 广告BBS推广
        /// </summary>
        public const string MKT_AdvEffectMonitorBBSUrlFormat = "/ECCentral.Portal.UI.MKT/AdvEffectMonitorBBS";
        /// <summary>
        /// 投票组管理
        /// </summary>
        public const string MKT_PollItemGroupMaintainUrlFormat = "/ECCentral.Portal.UI.MKT/PollItemGroupMaintain/{0}";

        /// <summary>
        /// 泰隆优选大使公告维护
        /// </summary>
        public const string MKT_AmbassadorNewsMaintainUrlFormat = "/ECCentral.Portal.UI.MKT/AmbassadorNewsMaintain/{0}";

        /// <summary>
        /// OpenAPI 编辑页面
        /// </summary>
        public const string MKT_OpenAPIAddMaintainUrlFormat = "/ECCentral.Portal.UI.MKT/AmbassadorNewsMaintain";

        /// <summary>
        /// OpenAPI 编辑页面
        /// </summary>
        public const string MKT_OpenAPIUpdateMaintainUrlFormat = "/ECCentral.Portal.UI.MKT/AmbassadorNewsMaintain/{0}";

        /// <summary>
        /// 顾客信息
        /// </summary>
        public const string MKT_CustomerMaintainUrlFormat = "/ECCentral.Portal.UI.Customer/CustomerMaintain/{0}";


        public const string MKT_SOMaintainUrlFormat = "/ECCentral.Portal.UI.SO/SOMaintain/?SOSysNo={0}";

        /// <summary>
        /// 泰隆优选促销楼层
        /// </summary>
        public const string MKT_FloorPromotionUrlFormat = "/ECCentral.Portal.UI.MKT/FloorPromotion/{0}";

        #endregion MKT Domain

        #region IM Domain

        public const string DomainName_IM = "IM";       

        public const string IM_ManufacturerMaintainUrlFormat = "/ECCentral.Portal.UI.IM/ManufacturerMaintain/{0}";
        public const string IM_ManufacturerMaintainCreateFormat = "/ECCentral.Portal.UI.IM/ManufacturerMaintain";
        public const string IM_BrandQueryUrlFormat = "/ECCentral.Portal.UI.IM/BrandQuery/{0}";
        public const string IM_BrandMaintainUrlFormat = "/ECCentral.Portal.UI.IM/BrandMaintain/{0}";
        public const string IM_BrandMaintainCreateFormat = "/ECCentral.Portal.UI.IM/BrandMaintain";
        public const string IM_AccessoryMaintainUrlFormat = "/ECCentral.Portal.UI.IM/AccessoryMaintain/{0}";
        public const string IM_AccessoryMaintainCreateFormat = "/ECCentral.Portal.UI.IM/AccessoryMaintain";
        public const string IM_ProductMaintainUrlFormat = "/ECCentral.Portal.UI.IM/ProductMaintain/{0}";
        public const string IM_ProductMaintainCreateFormat = "/ECCentral.Portal.UI.IM/ProductCreate";
        public const string IM_ProductAttachmentMaintainUrlFormat = "/ECCentral.Portal.UI.IM/ProductAttachmentMaintain/{0}";
        public const string IM_ProductAttachmentMaintainCreateFormat = "/ECCentral.Portal.UI.IM/ProductAttachmentMaintain";
        public const string IM_ProductGroupMaintainUrlFormat = "/ECCentral.Portal.UI.IM/ProductGroupMaintain/{0}";
        public const string IM_ProductGroupMaintainCreateFormat = "/ECCentral.Portal.UI.IM/ProductGroupMaintain";
        public const string IM_CategoryMaintainUrlFormat = "/ECCentral.Portal.UI.IM/CategoryMaintain/{0}";
        public const string IM_CategoryMaintainCreateFormat = "/ECCentral.Portal.UI.IM/CategoryMaintain";
        public const string IM_PMMaintainUrlFormat = "/ECCentral.Portal.UI.IM/PMMaintain/{0}";
        public const string IM_PMMaintainCreateFormat = "/ECCentral.Portal.UI.IM/PMMaintain";
        public const string IM_PMGroupMaintainUrlFormat = "/ECCentral.Portal.UI.IM/PMGroupMaintain/{0}";
        public const string IM_PMGroupMaintainCreateFormat = "/ECCentral.Portal.UI.IM/PMGroupMaintain";
        public const string IM_ProductResourcesUrlFormat = "/ECCentral.Portal.UI.IM/ProductResourcesManagement/{0}";
        public const string IM_ProductQueryPriceChangeLogUrlFormat = "/ECCentral.Portal.UI.IM/ProductQueryPriceChangeLog/{0}";
        public const string IM_ProductGroupMaintainFormat = "/ECCentral.Portal.UI.IM/ProductGroupMaintain/{0}";
        public const string IM_ProductPurchaseHistoryUrlFormat = "/ECCentral.Portal.UI.PO/PurchaseOrderItemHistory/{0}";

        public const string IM_ElectronicCard_ProductSysNo_Key = "IM_ElectronicCard_ProductSysNo";

        public const string IM_ManufacturerRequestUrl = "/ECCentral.Portal.UI.IM/ManufacturerRequest";
        public const string IM_CategoryRequestApprovalUrl = "/ECCentral.Portal.UI.IM/CategoryRequestApproval";
                
        public const string IM_ProductNotifyFormat = "/ECCentral.Portal.UI.IM/ProductNotify/{0}";        
        public const string IM_ProductlinkPriceUrlFormat = "/ECCentral.Portal.UI.IM/ProductMaintain?ProductSysNo={0}&operation=pricelink";

        public const string IM_RmaPolicyLogManagementUrlFormat = "/ECCentral.Portal.UI.IM/RmaPolicyLogManagement/{0}";
        public const string IM_LogQueryUrlFormat = "/ECCentral.Portal.UI.Common/LogQuery/{0}";

        public const string Key_Product_Orgin = "Product_Orgin";

        #endregion IM Domain

        #region PO Domain

        public const string DomainName_PO = "PO";
        public const string PO_KeyPurchaseOrderCompanyMappingDefaultStock = "PurchaseOrderCompanyMappingDefaultStock";
        public const string PO_PurchaseOrderMaintain = "/ECCentral.Portal.UI.PO/PurchaseOrderMaintain/{0}";
        public const string PO_ConsignMaintain = "/ECCentral.Portal.UI.PO/ConsignMaintain/{0}";
        public const string PO_VendorRMARefundMaintain = "/ECCentral.Portal.UI.PO/VendorRMARefundMaintain/{0}";
        public const string PO_GatherMaintain = "/ECCentral.Portal.UI.PO/GatherMaintain/{0}";
        public const string PO_CommissionItemView = "/ECCentral.Portal.UI.PO/CommissionItemView/{0}";
        public const string PO_CollectionPaymentItemView = "/ECCentral.Portal.UI.PO/CollectionPaymentMaintain/{0}";
        public const string PO_CommissionNew = "/ECCentral.Portal.UI.PO/CommissionNew";
        public const string PO_VirtualStockPurchaseOrderNew = "/ECCentral.Portal.UI.PO/VirtualStockPurchaseOrderNew/{0},{1}";

        public const string Key_POVendorAgentLevel = "VendorAgentLevel";

        public const string PO_PurchaseOrderQuery = "/ECCentral.Portal.UI.PO/PurchaseOrderQuery/?ProductSysNo={0}&QueryStatus=1,2,3,-2,5,6";
        public const string PO_PurchaseOrderWaitingInStockQuery = "/ECCentral.Portal.UI.PO/PurchaseOrderQuery/?ProductSysNo={0}&QueryStatus=3,6";

        public const string PO_VendorMaintain = "/ECCentral.Portal.UI.PO/VendorMaintain/{0}";
        public const string PO_ProductMaintain = "/ECCentral.Portal.UI.IM/ProductMaintain/{0}";
        public const string PO_CostChangeMaintain = "/ECCentral.Portal.UI.PO/CostChangeMaintain/{0}";
        public const string PO_ConsignAdjustMaintain = "/ECCentral.Portal.UI.PO/ConsignAdjustMaintain/{0}";
        public const string PO_GroupSettleMaintain = "/ECCentral.Portal.UI.MKT/GroupBuyingSettlementQuery/{0}";
       
        #endregion PO Domain

        #region Inventory Domain

        public const string DomainName_Inventory = "Inventory";

        public const string Key_ShiftShippingType = "ShiftShippingType";
        public const string Key_StockShiftConfigShippingType = "StockShiftConfigShippingType";
        public const string Key_InventoryCompanyName = "InventoryCompanyName";

        public const string Inventory_LendRequestMaintainUrlFormat = "/ECCentral.Portal.UI.Inventory/LendRequestMaintain/{0}";
        public const string Inventory_LendRequestMaintainCreateFormat = "/ECCentral.Portal.UI.Inventory/LendRequestMaintain";
        public const string Inventory_ConvertRequestMaintainUrlFormat = "/ECCentral.Portal.UI.Inventory/ConvertRequestMaintain/{0}";
        public const string Inventory_ConvertRequestMaintainCreateFormat = "/ECCentral.Portal.UI.Inventory/ConvertRequestMaintain";
        public const string Inventory_AdjustRequestMaintainUrlFormat = "/ECCentral.Portal.UI.Inventory/AdjustRequestMaintain/{0}";
        public const string Inventory_AdjustRequestMaintainCreateFormat = "/ECCentral.Portal.UI.Inventory/AdjustRequestMaintain";
        public const string Inventory_ShiftRequestMaintainUrlFormat = "/ECCentral.Portal.UI.Inventory/ShiftRequestMaintain/{0}";
        public const string Inventory_ShiftRequestMaintainCreateFormat = "/ECCentral.Portal.UI.Inventory/ShiftRequestMaintain";
        public const string Inventory_ShiftRequestMemoMaintainUrlFormat = "/ECCentral.Portal.UI.Inventory/ShiftRequestMemoMaintain/{0}";

        public const string Inventory_VirtualRequestMaintainCreate = "/ECCentral.Portal.UI.Inventory/VirtualRequestMaintain";
        public const string Inventory_VirtualRequestMaintainFormat = "/ECCentral.Portal.UI.Inventory/VirtualRequestMaintain/{0}";
        public const string Inventory_VirtualRequestMaintainCreateFormat = "/ECCentral.Portal.UI.Inventory/VirtualRequestMaintain?product={0}";
        public const string Inventory_VirtualRequestMaintainBatchCreateFormat = "/ECCentral.Portal.UI.Inventory/VirtualRequestMaintainBatch";
        public const string Inventory_VirtualRequestMaintainAuditFormat = "/ECCentral.Portal.UI.Inventory/VirtualRequestAudit";
        public const string Inventory_StockMaintainUrlFormat = "/ECCentral.Portal.UI.Inventory/StockMaintain/{0}";
        public const string Inventory_StockMaintainCreateFormat = "/ECCentral.Portal.UI.Inventory/StockMaintain";
        public const string Inventory_WarehouseMaintainUrlFormat = "/ECCentral.Portal.UI.Inventory/WarehouseMaintain/{0}";
        public const string Inventory_WarehouseMaintainCreateFormat = "/ECCentral.Portal.UI.Inventory/WarehouseMaintain";
        public const string Inventory_WarehouseOwnerMaintainUrlFormat = "/ECCentral.Portal.UI.Inventory/WarehouseOwnerMaintain/{0}";

        public const string Inventory_InventoryQueryFormat = "/ECCentral.Portal.UI.Inventory/InventoryQuery/{0}";
        public const string Inventory_ItemAllocatedCardQueryFormat = "/ECCentral.Portal.UI.Inventory/ItemAllocatedCardQuery/{0}";
        public const string Inventory_ItemCardQueryFormat = "/ECCentral.Portal.UI.Inventory/ItemCardQuery/{0}";
        
        public const string Inventory_ProductStockingCenterIndex = "/ECCentral.Portal.UI.Inventory/TransferStockingCenter";
        public const string Inventory_ProductStockingCenter = "/ECCentral.Portal.UI.Inventory/TransferStockingCenter/{0}";

        public const string Inventory_UnmarketableInventoryQuery = "/ECCentral.Portal.UI.Inventory/UnmarketableInventoryQuery/{0}";
        public const string Inventory_ItemQueryFormat = "/ECCentral.Portal.UI.Inventory/ItemsCardQuery/{0}";

        public const string Inventory_ExperienceHallAllocateOrderUrlFormat = "/ECCentral.Portal.UI.Inventory/ExperienceMaintain/{0}";
        public const string Inventory_ExperienceHallAllocateOrderNewUrlFormat = "/ECCentral.Portal.UI.Inventory/ExperienceMaintain";
        #endregion Inventory Domain

        #region Invoice Domain

        public const string DomainName_Invoice = "Invoice";
        public const string Key_LoseType = "TrackingInfoLoseType"; //损失类型
        public const string Key_DocumentType = "DocumentType";
        public const string NetPayMaintainUrlFormat = "/ECCentral.Portal.UI.Invoice/NetPayMaintain/{0}";
        public const string InvoiceInputMaintainUrlFormat = "/ECCentral.Portal.UI.Invoice/InvoiceInputMaintain/{0}";
        public const string SaleIncomeAutoConfirmUrl = "/ECCentral.Portal.UI.Invoice/SaleIncomeAutoConfirm";
        public const string PostIncomeImportUrl = "/ECCentral.Portal.UI.Invoice/PostIncomeImport";
        public const string PayItemListOrderQueryUrl = "/ECCentral.Portal.UI.Invoice/PayItemListOrderQuery";
        public const string Invoice_PayItemMaintainUrl = "/ECCentral.Portal.UI.Invoice/PayItemMaintain/{0}";
        //public const string Invoice_CurrencyFormat = "￥#####0.00";
        public const string Invoice_DecimalFormat = "#########0.00";
        public const string Invoice_LongTimeFormat = "yyyy-MM-dd HH:mm:ss";
        public const string Invoice_ShortTimeFormat = "yyyy-MM-dd";
        public const decimal Invoice_TaxRateBase = 1.17M; //默认税率 17%

        public static string Invoice_ToCurrencyString(decimal? amt)
        {
            //fixbug：有符号时，符号在￥前面
            return string.Format("￥" + "{0:N2}", amt ?? 0);
        }

        public const string Key_AuditStatus = "AuditStatus";
        public const string Key_VendorPayPeriod = "VendorPayPeriod";
        public const string Key_ProductType = "ProductType";
        public const string Key_OldChangeNewStatus = "OldChangeNewStatus";

        public const string Invoice_SAPVendorQuery = "/ECCentral.Portal.UI.Invoice/SAPVendorQuery";
        public const string Invoice_SAPCompanyQuery = "/ECCentral.Portal.UI.Invoice/SAPCompanyQuery";
        public const string Invoice_SAPIPPUserQuery = "/ECCentral.Portal.UI.Invoice/SAPIPPUserQuery";

        #endregion Invoice Domain       

        #region ExternalSYS

        #region External URL Config Keys in ControlMenuConfiguration

        public const string ConfigKey_External_WebsiteProductImageUrl = "WebsiteProductImageUrl";
        public const string ConfigKey_External_WebsiteProductOtherImageUrl = "WebsiteProductOtherImageUrl";
        public const string ConfigKey_External_WebSiteProductUrl = "WebSiteProductUrl";
        public const string ConfigKey_External_BrandAuthorizedImageUrl = "BrandAuthorizedImageUrl";
        public const string ConfigKey_External_BrandProductPreviewUrl = "BrandProductPreviewUrl";
        public const string ConfigKey_External_WebSiteProductPreviewUrl = "WebSiteProductPreviewUrl";
        public const string ConfigKey_External_JingDongPriceUrl = "JingDongPriceUrl";

        public const string ConfigKey_External_WebsiteProductDetailBySysNoUrl = "WebsiteProductDetailBySysNoUrl";
        public const string ConfigKey_External_WebsitePromotionUrl = "WebsitePromotionUrl";
        public const string ConfigKey_External_NewsUploadP80ImageUrl = "NewsUploadP80ImageUrl";
        public const string ConfigKey_External_NewsUploadOriginalImageUrl = "NewsUploadOriginalImageUrl";

        public const string ConfigKey_External_CPSAdvertisingImageUrl = "CPSAdvertisingImageUrl";

        public const string ConfigKey_External_SellerPortalImageServicePath = "SellerPortalImageServicePath";
        public const string ConfigKey_External_SellerPortalFileServicePath = "SellerPortalFileServicePath";
        public const string ConfigKey_External_ReviewMaintainImageUrl = "ReviewMaintainImageUrl";

        #endregion


        public const string DomainName_ExternalSYS = "ExternalSYS";

        public const string Key_ExternalSYSLogType = "VendorSystemLogType";

        public const string Key_EIMSType = "EIMSType";

        public const string Key_ReceivedType = "ReceivedType";

        public const string Key_InvoiceStatus = "InvoiceStatus";

        public const string Key_RuleStatus = "RuleStatus";

        public const string Key_FormStatus = "FormStatus";

        public const string Key_TaxRate = "TaxRate";

        public const string ExternalSYS_VendorInfoUrlFormat = "/ECCentral.Portal.UI.PO/VendorMaintain/{0}";
       
        #region CPS
        public const string ExternalSYS_FinanceQuery = "/ECCentral.Portal.UI.ExternalSYS/FinanceQuery/{0}";
        public const string ExternalSYS_OrderManagement = "/ECCentral.Portal.UI.ExternalSYS/OrderManagement/{0}";

        #endregion
        
        #endregion

        #region Category

        public const int Category1Level = 1;

        public const int Category2Level = 2;

        public const int Category3Level = 3;
        #endregion
    }
}
