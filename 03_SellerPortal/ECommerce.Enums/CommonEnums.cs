using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ECommerce.Enums
{
    public enum CommonStatus
    {
        [Description("有效")]
        Actived = 1,
        [Description("无效")]
        DeActived = 0
    }

    public enum CommonYesOrNo
    {
        [Description("是")]
        Yes = 1,
        [Description("否")]
        No = 0
    }

    public enum SMSPriority
    {
        VeryLow = 1,
        Low = 2,
        Normal = 3,
        Hight = 4,
        VeryHigh = 5
    }

    public enum SMSType
    {
        /// <summary>
        /// 订单审核
        /// </summary>
        OrderAudit,
        /// <summary>
        /// 订单出库
        /// </summary>
        OrderOutBound,
        /// <summary>
        /// 订单到达
        /// </summary>
        OrderArived,
        /// <summary>
        /// 订单作废
        /// </summary>
        OrderAbandon
    }

    public enum ShipTypeSMSStatus
    {
        A = 0,

        F = -1
    }

     /// <summary>
    /// 业务日志的类型
    /// </summary>
    public enum BizLogType
    {
        /// <summary>
        ///默认退换货政策
        /// </summary>
        [Description("添加默认退换货政策品牌")]
        DefaultRMAPolicy_Add = 610010,
        [Description("编辑默认退换货政策")]
        DefaultRMAPolicy_Edit = 610020,
        [Description("删除默认退换货政策")]
        DefaultRMAPolicy_Del = 610030,

        /// <summary>
        ///品牌延保信息
        /// </summary>
        [Description("添加品牌延保")]
        IM_BrandWarranty_Add = 600010,
        [Description("编辑品牌延保")]
        IM_BrandWarranty_Edit = 600020,
        [Description("删除品牌延保")]
        IM_BrandWarranty_Delete = 600030,

        //xx_xx_xx
        //模块_子模块_操作
        //------------------------------------------------------------------------------------sys 10
        //		user 10, add 10, update 11.
        [Description("增加用户")]
        Sys_User_Add = 101010,
        [Description("更新用户")]
        Sys_User_Update = 101011,
        [Description("用户登录")]
        Sys_User_Login = 101012,
        //		role 11, add 10, update 11
        [Description("增加角色")]
        Sys_Role_Add = 101110,
        [Description("更新角色")]
        Sys_Role_Update = 101111,
        //		user&role 12, add 10, delete 12
        [Description("增加角色")]
        Sys_UserRole_Add = 101210,
        [Description("更新角色")]
        Sys_UserRole_Delete = 101212,
        //		role&privilege 13 add 10, delete 12
        [Description("增加角色")]
        Sys_RolePrivilege_Add = 101310,
        [Description("更新角色")]
        Sys_RolePrivilege_Delete = 101312,

        //------------------------------------------------------------------------------------basic 11
        //		category1 13,
        //						add 10, update 11.
        [Description("增加大类")]
        Basic_Category1_Add = 111310,
        [Description("更新大类")]
        Basic_Category1_Update = 111311,
        [Description("大类取消审核")]
        Basic_Category1_CancelAudit = 111313,
        [Description("大类审核通过")]
        Basic_Category1_Approved = 111314,
        [Description("大类审核未通过")]
        Basic_Category1_Declined = 111315,

        //		category2 14, 
        //						add 10, update 11.
        [Description("增加中类")]
        Basic_Category2_Add = 111410,
        [Description("更新中类")]
        Basic_Category2_Update = 111411,
        [Description("中类取消审核")]
        Basic_Category2_CancelAudit = 111413,
        [Description("中类审核通过")]
        Basic_Category2_Approved = 111414,
        [Description("中类审核未通过")]
        Basic_Category2_Declined = 111415,

        //		category3 15, 
        //						add 10, update 11.
        [Description("增加小类")]
        Basic_Category3_Add = 111510,
        [Description("更新小类")]
        Basic_Category3_Update = 111511,
        [Description("小类取消审核")]
        Basic_Category3_CancelAudit = 111513,
        [Description("小类审核通过")]
        Basic_Category3_Approved = 111514,
        [Description("小类审核未通过")]
        Basic_Category3_Declined = 111515,

        //		categoryAttribute 16,
        //						init 10, update 11, top 12, up 13, down 14, bottom 15
        [Description("初始化小类属性")]
        Basic_CategoryAttribute_Init = 111610,
        [Description("修改小类属性")]
        Basic_CategoryAttribute_Update = 111611,
        [Description("移动到顶端")]
        Basic_CategoryAttribute_Top = 111612,
        [Description("移动到上一个")]
        Basic_CategoryAttribute_Up = 111613,
        [Description("移动到下一个")]
        Basic_CategoryAttribute_Down = 111614,
        [Description("移动到低端")]
        Basic_CategoryAttribute_Bottom = 111615,

        //		manufacturer  17,
        //						add 10, update 11
        [Description("增加生产商")]
        Basic_Manufacturer_Add = 111710,
        [Description("更新生产商")]
        Basic_Manufacturer_Update = 111711,
        //		manufacturer  18,
        //						add 10, update 11
        [Description("增加供应商")]
        Basic_Vendor_Add = 111810,
        [Description("更新供应商")]
        Basic_Vendor_Update = 111811,

        // add by gordon 2008-01-24 [LOG]供应商，item Hold 功能
        [Description("供应商锁定")]
        Basic_Vendor_Hold = 111812,
        [Description("供应商解除锁定")]
        Basic_Vendor_UnHold = 111813,
        // add by gordon 2008-01-24 [LOG]供应商，item Hold 功能

        //		stock  19, add 10, update 11
        [Description("增加仓库")]
        Basic_Stock_Add = 111910,
        [Description("更新仓库")]
        Basic_Stock_Update = 111911,

        //		currency  20, add 10, update 11
        [Description("增加货币")]
        Basic_Currency_Add = 112010,
        [Description("更新货币")]
        Basic_Currency_Update = 112011,

        //		customer 30, add 10, update 11, invalid 12
        [Description("增加客户")]
        Basic_Customer_Add = 113001,
        [Description("修改客户")]
        Basic_Customer_Update = 113011,
        [Description("作废客户")]
        Basic_Customer_Invalid = 113012,
        [Description("手动设定客户VIP级别")]
        Basic_Customer_ManualSetVIPRank = 113013,
        [Description("发送邮件或短信")]
        Basic_Customer_ManualSendEmail = 113014,
        [Description("修改帐期额度和天数")]
        Basic_Customer_SetCreditLimit = 113015,
        [Description("取消验证客户邮箱")]
        Basic_Customer_CancelConfirmEmail = 113016,
        [Description("取消验证客户手机号")]
        Basic_Customer_CancelConfirmPhone = 113017,

        //		ASP  60,
        //			insert area 10, update area 11
        //			insert pay type 12, update pay type 13
        //			insert ship type 14, update ship type 15
        //			insert ship ! area 16, delete 17
        //			insert ship ! pay 18, delete 19
        //			insert ship area price 20, delete 21
        [Description("增加地区")]
        Basic_Area_Add = 116010,
        [Description("更新地区")]
        Basic_Area_Update = 116011,
        [Description("增加支付方式")]
        Basic_PayType_Add = 116012,
        [Description("更新支付方式")]
        Basic_PayType_Update = 116013,
        [Description("增加送货方式")]
        Basic_ShipType_Add = 116014,
        [Description("更新送货方式")]
        Basic_ShipType_Update = 116015,
        [Description("添加送货方式和地区的否关系")]
        Basic_ShipAreaUn_Add = 116016,
        [Description("删除送货方式和地区的否关系")]
        Basic_ShipAreaUn_Delete = 116017,
        [Description("添加送货方式和支付方式的否关系")]
        Basic_ShipPayUn_Add = 116018,
        [Description("删除送货方式和支付方式的否关系")]
        Basic_ShipPayUn_Delete = 116019,
        [Description("添加送货方式和地区的价格")]
        Basic_ShipAreaPrice_Add = 116020,
        [Description("删除送货方式和地区的价格")]
        Basic_ShipAreaPrice_Delete = 116021,

        //		Product 30
        [Description("新增商品")]
        Basic_Product_Add = 113001,
        [Description("更新商品基本信息")]
        Basic_Product_Basic_Update = 113002,
        [Description("更新商品价格")]
        Basic_Product_Price_Update = 113003,
        [Description("更新商品PM")]
        Basic_Product_PM_Update = 113004,
        [Description("更新商品属性")]
        Basic_Product_Attribute_Update = 113005,
        [Description("更新商品重量")]
        Basic_Product_Weight_Update = 113006,
        [Description("更新商品图片")]
        Basic_Product_Pic_Update = 113007,
        [Description("设置商品不可以前台申请退款")]
        Basic_Product_NotPermitRefund = 113008,
        [Description("设置商品可以前台申请退货")]
        Basic_Product_PermitRefund = 113009,
        [Description("更新京东Item编号")]
        Basic_Product_JDItem_Update = 113010,
        [Description("更新产品拍照状态")]
        Ex_Product_TakePictures_Update = 113011,
        [Description("克隆商品")]
        Basic_Product_Clone = 113014,
        [Description("更新商品详细描述")]
        Basic_Product_LongDescription_Update = 113016,
        [Description("更新商品状态")]
        Basic_Product_Status_Update = 113013,
        [Description("复制商品")]
        Basic_Product_Copy = 113015,
        [Description("参加延保类别维护")]
        Basic_Product_CategoryExtendWarranty = 112011,


        // Add by tomato 2007-03-02 [PM]价格审批也要记录日志，和价格修改一样
        [Description("更新商品价格提交审核")]
        Basic_Product_Price_Update_Verify = 113020,
        [Description("更新商品价格取消审核")]
        Basic_Product_Price_Update_CancelVerify = 113021,
        [Description("更新商品价格审核通过")]
        Basic_Product_Price_Update_VerifyOK = 113022,
        [Description("更新商品价格审核未通过")]
        Basic_Product_Price_Update_VerifyCancel = 113023,
        /// <summary>
        /// 更新分期商品成本
        /// add by shadow 2008-11-17
        /// </summary>
        [Description("更新分期商品成本")]
        Basic_Product_UnitCost_Update_Instalment = 113025,
        /// <summary>
        /// 按期数更新分期商品价格
        /// </summary>
        [Description("按期数更新分期商品价格")]
        Basic_Product_Price_Update_Instalment = 113026,

        // add by gordon 2008-06-03
        [Description("PM申请备注更新")]
        Basic_Product_Price_PMRequestMemo = 113030,
        [Description("TL申请备注更新")]
        Basic_Product_Price_TLRequestMemo = 113031,
        // add by gordon 2008-06-03

        [Description("更新供应商合作金额提交审核")]
        Vendor_Update_Verify = 114020,
        [Description("更新供应商合作金额取消审核")]
        Vendor_Update_Cancel_Verify = 114021,
        [Description("更新供应商合作金额审核通过")]
        Vendor_Update_VerifyOK = 114022,
        [Description("更新供应商合作金额审核未通过")]
        Vendor_Update__VerifyCancel = 114023,

        /// <summary>
        /// Add by carl 20070328 .
        /// 为了“产品发布”记载每条产品的日志信息，而专设的枚举值。
        /// </summary>
        [Description("PIM产品发布")]
        Basic_Product_Info_PublishOK = 113024,


        [Description("礼品卡生成功能")]
        Basic_GiftCard_Info_CreateOK = 113099,

        /// <summary>
        /// 更新商品Note
        /// </summary>
        [Description("更新商品Note")]
        Basic_Product_Note_Update = 113017,
        [Description("更新产品锁定状态")]
        Basic_Product_Hold_Update = 113018,
        [Description("采购在途库存调整")]
        Inventory_PurchaseQtyAdjust = 113019,

        [Description("因下架系统撤销置顶")]
        Basic_Product_TopCancelWithGoDown = 113041,
        [Description("保存置顶")]
        Basic_Product_TopSave = 113042,
        [Description("取消置顶")]
        Basic_Product_TopCancel = 113043,
        //------------------------------------------------------------------------------------stock sheet 50
        //		lend 11,
        //
        [Description("借货单主项增加")]
        St_Lend_Master_Insert = 501110,
        [Description("借货单主项修改")]
        St_Lend_Master_Update = 501111,
        [Description("借货单明细插入")]
        St_Lend_Item_Insert = 501112,
        [Description("借货单明细删除")]
        St_Lend_Item_Delete = 501113,
        [Description("借货单明细修改")]
        St_Lend_Item_Update = 501114,

        [Description("借货单废弃")]
        St_Lend_Abandon = 501115,
        [Description("借货单取消废弃")]
        St_Lend_CancelAbandon = 501116,

        [Description("借货单审核")]
        St_Lend_Verify = 501117,
        [Description("借货单取消审核")]
        St_Lend_CancelVerify = 501118,

        [Description("借货单出库")]
        St_Lend_OutStock = 501119,
        [Description("借货单取消出库")]
        St_Lend_CancelOutStock = 501120,

        [Description("借货单还货插入")]
        St_Lend_Return_Insert = 501121,
        [Description("借货单还货删除")]
        St_Lend_Return_Delete = 501122,

        //		adjust 12,
        //
        [Description("损益单主项增加")]
        St_Adjust_Master_Insert = 501210,
        [Description("损益单主项修改")]
        St_Adjust_Master_Update = 501211,
        [Description("损益单明细插入")]
        St_Adjust_Item_Insert = 501212,
        [Description("损益单明细删除")]
        St_Adjust_Item_Delete = 501213,
        [Description("损益单明细修改")]
        St_Adjust_Item_Update = 501214,

        [Description("损益单废弃")]
        St_Adjust_Abandon = 501215,
        [Description("损益单取消废弃")]
        St_Adjust_CancelAbandon = 501216,

        [Description("损益单审核")]
        St_Adjust_Verify = 501217,
        [Description("损益单取消审核")]
        St_Adjust_CancelVerify = 501218,

        [Description("损益单出库")]
        St_Adjust_OutStock = 501219,
        [Description("损益单取消出库")]
        St_Adjust_CancelOutStock = 501220,

        //		shift 13,
        //
        [Description("移仓单主项增加")]
        St_Shift_Master_Insert = 501310,
        [Description("移仓单主项修改")]
        St_Shift_Master_Update = 501311,
        [Description("移仓单明细插入")]
        St_Shift_Item_Insert = 501312,
        [Description("移仓单明细删除")]
        St_Shift_Item_Delete = 501313,
        [Description("移仓单明细修改")]
        St_Shift_Item_Update = 501314,

        [Description("移仓单废弃")]
        St_Shift_Abandon = 501315,
        [Description("移仓单取消废弃")]
        St_Shift_CancelAbandon = 501316,

        [Description("移仓单审核")]
        St_Shift_Verify = 501317,
        [Description("移仓单取消审核")]
        St_Shift_CancelVerify = 501318,

        [Description("移仓单出库")]
        St_Shift_OutStock = 501319,
        [Description("移仓单取消出库")]
        St_Shift_CancelOutStock = 501320,

        [Description("移仓单入库")]
        St_Shift_InStock = 501321,
        [Description("移仓单取消入库")]
        St_Shift_CancelInStock = 501322,

        [Description("移仓单批量Item入库")]
        St_Shift_BatchInsert = 501323,

        //		transfer 14,
        //
        [Description("转换单主项增加")]
        St_Transfer_Master_Insert = 501410,
        [Description("转换单主项修改")]
        St_Transfer_Master_Update = 501411,
        [Description("转换单明细插入")]
        St_Transfer_Item_Insert = 501412,
        [Description("转换单明细删除")]
        St_Transfer_Item_Delete = 50143,
        [Description("转换单明细修改")]
        St_Transfer_Item_Update = 501414,

        [Description("转换单废弃")]
        St_Transfer_Abandon = 501415,
        [Description("转换单取消废弃")]
        St_Transfer_CancelAbandon = 501416,

        [Description("转换单审核")]
        St_Transfer_Verify = 501417,
        [Description("转换单取消审核")]
        St_Transfer_CancelVerify = 501418,

        [Description("转换单出库")]
        St_Transfer_OutStock = 501419,
        [Description("转换单取消出库")]
        St_Transfer_CancelOutStock = 501420,

        //		virtual 15,
        //
        [Description("虚库操作")]
        St_Virtual_Insert = 501510,

        //		Inventory position
        //
        [Description("设置库位")]
        St_Inventory_SetPos = 501610,

        [Description("生成虚库采购单")]
        St_SOVirtualItemRequest_Add = 501511,
        [Description("保存虚库采购单")]
        St_SOVirtualItemRequest_Save = 501512,
        [Description("作废虚库采购单")]
        St_SOVirtualItemRequest_Abandon = 501513,
        [Description("更新虚库采购单CS备注")]
        St_SOVirtualItemRequest_UpdateCSMemo = 501514,
        [Description("更新RMA保修期和RMA率标准")]
        St_Category3SetRMAInfo_Update = 501515,

        //Purchase 20-----------------------------------------------------------
        //	Basket  10

        [Description("采购篮插入")]
        Purchase_Basket_Insert = 201010,
        [Description("采购篮更新")]
        Purchase_Basket_Update = 201011,
        [Description("采购篮删除")]
        Purchase_Basket_Delete = 201012,

        //     PO	11
        [Description("生成采购单")]
        Purchase_Create = 201110,
        [Description("采购单主项修改")]
        Purchase_Master_Update = 201111,
        [Description("采购单明细添加")]
        Purchase_Item_Insert = 201112,
        [Description("采购单明细修改")]
        Purchase_Item_Update = 201113,
        [Description("采购单明细删除")]
        Purchase_Item_Delete = 201114,
        [Description("采购单审核到摊销")]
        Purchase_Verify_Apportion = 201114,
        [Description("采购单审核到入库")]
        Purchase_Verify_InStock = 201116,
        [Description("采购单取消审核")]
        Purchase_CancelVerify = 201117,
        [Description("采购单入库")]
        Purchase_InStock = 201118,
        [Description("采购单取消入库")]
        Purchase_CancelInStock = 201119,
        [Description("采购单作废")]
        Purchase_Abandon = 201120,
        [Description("采购单取消作废")]
        Purchase_CancelAbandon = 201121,

        [Description("采购单摊销主项添加")]
        Purchase_ApportionMaster_Add = 201122,
        [Description("采购单摊销主项删除")]
        Purchase_ApportionMaster_Delete = 201123,
        [Description("采购单摊销明细添加")]
        Purchase_ApportionItem_Add = 201124,
        [Description("采购单摊销明细删除")]
        Purchase_ApportionItem_Delete = 201125,
        [Description("采购单摊销导出")]
        Purchase_Apportion_Export = 201126,
        // Add by tomato 2006-11-14 [PM] 增加一个权限，允许修改PO的Memo
        [Description("采购单入库备注")]
        Purchase_InStockMemo = 201127,
        // Add by tomato 2006-11-14 [PM] 增加一个权限，允许修改PO的Memo
        // Add by tomato 2006-11-30 Partly Receive
        [Description("采购单商品检查")]
        Purchase_Item_Check = 201128,
        // Add by tomato 2006-11-30 Partly Receive
        [Description("采购单自动作废")]
        Purchase_AutoAbandon = 201129,

        [Description("采购单发送邮件")]
        PO_SendMail = 808001,

        #region 代销日志 ---added by seanqu 2006/07/31

        /// <summary>
        /// 生成代销转财务记录
        /// </summary>
        [Description("生成代销转财务记录")]
        POCToAcc_Add = 201130,
        /// 生成Vendor结算单
        /// </summary>
        [Description("生成Vendor结算单")]
        POC_VendorSettle_Create = 201131,
        /// <summary>
        /// 修改Vendor结算单主项
        /// </summary>
        [Description("修改Vendor结算单主项")]
        POC_VendorSettle_Master_Update = 201132,
        /// <summary>
        /// 添加Vendor结算单明细
        /// </summary>
        [Description("添加Vendor结算单明细")]
        POC_VendorSettle_Item_Add = 201133,
        /// <summary>
        /// 删除Vendor结算单明细
        /// </summary>
        [Description("删除Vendor结算单明细")]
        POC_VendorSettle_Item_Delete = 201134,
        /// <summary>
        /// 修改Vendor结算单明细
        /// </summary>
        [Description("修改Vendor结算单明细")]
        POC_VendorSettle_Item_Update = 201135,
        /// <summary>
        /// 审核Vendor结算单
        /// </summary>
        [Description("审核Vendor结算单")]
        POC_VendorSettle_Audit = 201136,
        /// <summary>
        /// 取消审核Vendor结算单
        /// </summary>
        [Description("取消审核Vendor结算单")]
        POC_VendorSettle_CancelAudit = 201137,
        /// <summary>
        /// 结算Vendor结算单
        /// </summary>
        [Description("结算Vendor结算单")]
        POC_VendorSettle_Settle = 201138,
        /// <summary>
        /// 取消结算Vendor结算单
        /// </summary>
        [Description("取消结算Vendor结算单")]
        POC_VendorSettle_CancelSettle = 201139,
        /// <summary>
        /// 作废Vendor结算单
        /// </summary>
        [Description("作废Vendor结算单")]
        POC_VendorSettle_Abandon = 201140,
        /// <summary>
        /// 取消作废Vendor结算单
        /// </summary>
        [Description("取消作废Vendor结算单")]
        POC_VendorSettle_CancelAbandon = 201141,
        /// <summary>
        /// 生成代销财务结算单
        /// </summary>
        [Description("生成代销财务结算单")]
        POC_FinanceSettle_Create = 201142,
        /// <summary>
        /// 作废财务结算单
        /// </summary>
        [Description("作废财务结算单")]
        POC_FinanceSettle_Abandon = 201143,
        /// <summary>
        /// 取消作废财务结算单
        /// </summary>
        [Description("取消作废财务结算单")]
        POC_FinanceSettle_CancelAbandon = 201144,
        /// <summary>
        /// 采购单PM申请备注
        /// 2008-5-21 add by Shadow
        /// </summary>
        [Description("采购单PM申请备注")]
        Purchase_PMRequestMemo = 201145,
        /// <summary>
        /// 采购单TL申请备注
        /// 2008-5-21 add by Shadow
        /// </summary>
        [Description("采购单TL申请备注")]
        Purchase_TLRequestMemo = 201146,

        #endregion 代销日志 ---added by seanqu 2006/07/31

        #region Updated by seanqu 2006/07/05

        //Finance 30
        //				po 11
        [Description("财务采购付款单添加")]
        Finance_Pay_Item_Add = 301110,
        [Description("财务采购付款单修改")]
        Finance_Pay_Item_Update = 301111,
        [Description("财务采购付款单作废")]
        Finance_Pay_Item_Abandon = 301112,
        [Description("财务采购付款单取消作废")]
        Finance_Pay_Item_CancelAbandon = 301113,
        [Description("财务采购付款单支付")]
        Finance_Pay_Item_Pay = 301114,
        [Description("财务采购付款单取消支付")]
        Finance_Pay_Item_CancelPay = 301115,
        [Description("财务采购应付更新")]
        Finance_Pay_Update = 301116,
        [Description("财务采购付款单锁定")]
        Finance_Pay_Item_Lock = 301117,
        [Description("财务采购付款单取消锁定")]
        Finance_Pay_Item_UnLock = 301118,
        [Description("财务采购付款单冲销")]
        Finance_Pay_Item_ReverseAccount = 301119,

        #endregion Updated by seanqu 2006/07/05

        //				so income 12
        [Description("财务销售收款单添加")]
        Finance_SOIncome_Add = 301201,
        [Description("财务销售收款单作废")]
        Finance_SOIncome_Abandon = 301202,
        [Description("财务销售收款单确认")]
        Finance_SOIncome_Confirm = 301203,
        [Description("财务销售收款单取消确认")]
        Finance_SOIncome_UnConfirm = 301204,
        [Description("财务销售收款单确认金额")]
        Finance_SOIncome_UpdateIncomeAmt = 301205,
        [Description("分仓发票号码维护")]
        Finance_SOIncome_UpdateInvoiceNo = 301206,

        //				netpay 13
        [Description("财务NetPay Add&Verify")]
        Finance_NetPay_AddVerified = 301310,
        [Description("财务NetPay Verify")]
        Finance_NetPay_Verify = 301311,
        [Description("财务NetPay Abandon")]
        Finance_NetPay_Abandon = 301312,
        [Description("财务Advance NetPay Check")]
        Finance_AdvancePay_Check = 301313,

        //汇款认领单的日志  frank  12/04/2006
        [Description("汇款单维护")]
        FinanceRemitNoOwnerEdit = 301314,
        [Description("汇款单认领审核")]
        FinanceRemitNoOwnerClaimAudit = 301315,
        [Description("汇款单(含认领信息)删除")]
        FinanceRemitNoOwnerDelete = 301316,

        //             财务给系统帐号增加积分  14
        [Description("财务Add  Accounts Point")]
        Add_NewEggAccounts_Point = 301401,

        //Sale 60
        //              so 06
        [Description("销售单生成")]
        Sale_SO_Create = 600601,
        [Description("销售单审核")]
        Sale_SO_Audit = 600602,
        [Description("销售单取消审核")]
        Sale_SO_CancelAudit = 600603,
        [Description("销售单经理审核")]
        Sale_SO_ManagerAudit = 600604,
        [Description("销售单客户作废")]
        Sale_SO_CustomerAbandon = 600605,
        [Description("销售单员工作废")]
        Sale_SO_EmployeeAbandon = 600606,
        [Description("销售单经理作废")]
        Sale_SO_ManagerAbandon = 600607,
        [Description("销售单取消作废")]
        Sale_SO_CancelAbandon = 600608,
        [Description("销售单已出库待申报")]
        Sale_SO_OutStock = 600609,
        [Description("销售单取消出库")]
        Sale_SO_CancelOutStock = 600610,
        [Description("销售单发票打印")]
        Sale_SO_PrintInvoice = 600611,
        [Description("销售单修改")]
        Sale_SO_Update = 600612,
        [Description("快钱返现-提交订单信息")]
        Sale_SO_FastCashRebate = 600613, // DavidLiu 2006-09-08
        [Description("HoldSO")]
        Sale_SO_HoldSO = 600614,
        [Description("UnHoldSO")]
        Sale_SO_UnHoldSO = 600615,
        [Description("VoideSO")]
        Sale_SO_VoideSO = 600616,
        [Description("更新发票号")]
        Sale_SO_UpdateInvoiceNo = 600617,
        [Description("拆分发票")]
        Sale_SO_SplitInvoice = 600618,
        [Description("取消拆分发票")]
        Sale_SO_CancelSplitInvoice = 600619,
        [Description("创建赠品订单")]
        Sale_SO_CreateGiftSO = 600620,
        [Description("销售单客户修改")]
        Sale_SO_CustomerUpdate = 600621,
        [Description("物流拒收")]
        Sale_SO_AutoRMA = 600622,
        [Description("物流信息")]
        Sale_SO_ShippingInfo = 600623,
        [Description("销售单已申报待通关")]
        Sale_SO_Reported = 600624,
        [Description("销售单已通关发往顾客")]
        Sale_SO_CustomsPass = 600625,

        //				rma 08
        [Description("RMA单生成")]
        Sale_RMA_Create = 600801,
        [Description("RMA单作废")]
        Sale_RMA_Abandon = 600802,
        [Description("RMA单审核")]
        Sale_RMA_Audit = 600803,
        [Description("RMA单取消审核")]
        Sale_RMA_CancelAudit = 600804,
        [Description("RMA单接收商品")]
        Sale_RMA_Receive = 600805,
        [Description("RMA单取消接收")]
        Sale_RMA_CancelReceive = 600806,
        [Description("RMA单处理")]
        Sale_RMA_Handle = 600807,
        [Description("RMA单取消处理")]
        Sale_RMA_CancelHandle = 600808,
        [Description("RMA单结案")]
        Sale_RMA_Close = 600809,
        [Description("RMA单重开")]
        Sale_RMA_Reopen = 600810,

        //				ro 09
        [Description("退货单生成")]
        Sale_RO_Create = 600901,
        [Description("退货单作废")]
        Sale_RO_Abandon = 600902,
        [Description("退货单审核")]
        Sale_RO_Audit = 600903,
        [Description("退货单取消审核")]
        Sale_RO_CancelAudit = 600904,
        [Description("退货单退货")]
        Sale_RO_Return = 600905,
        [Description("退货单取消退货")]
        Sale_RO_CancelReturn = 600906,
        [Description("退货单发票打印")]
        Sale_RO_PrintInvoice = 600907,

        //RMA new Version
        //70
        //				rma requeset 10  ----need cindy maintain
        [Description("RMA单生成")]
        Sale_RMA_Create2 = 700801,
        [Description("RMA单作废")]
        Sale_RMA_Abandon2 = 700802,
        [Description("RMA单审核")]
        Sale_RMA_Audit2 = 700803,

        //				rma outbound 20
        [Description("RMA-送修-生成")]
        RMA_OutBound_Create = 702001,
        [Description("RMA-送修-修改")]
        RMA_OutBound_Update = 702002,
        [Description("RMA-送修-出库")]
        RMA_OutBound_OutStock = 702003,
        [Description("RMA-送修-取消出库")]
        RMA_OutBound_CancelOutStock = 702004,
        [Description("RMA-送修-删除明细")]
        RMA_OutBound_DeleteItem = 702005,
        [Description("RMA-送修-删除明细")]
        RMA_OutBound_InsertItem = 702006,
        [Description("RMA-送修-删除作废")]
        RMA_OutBound_Abandon = 702007,

        //				rma register 30
        [Description("RMA-登记-更新Check")]
        RMA_Register_Check = 703001,
        [Description("RMA-登记-更新Memo")]
        RMA_Register_Memo = 703002,
        [Description("RMA-登记-更新Outbound")]
        RMA_Register_Outbound = 703003,
        [Description("RMA-登记-更新Revert")]
        RMA_Register_Revert = 703004,
        [Description("RMA-登记-更新Refund")]
        RMA_Register_Refund = 703005,
        [Description("RMA-登记-更新Return")]
        RMA_Register_Return = 703006,
        [Description("RMA-登记-更新Close")]
        RMA_Register_Close = 703007,
        [Description("RMA-登记-SetToCC")]
        RMA_Register_ToCC = 703008,
        [Description("RMA-登记-SetToRMA")]
        RMA_Register_ToRMA = 703009,
        //
        [Description("RMA-登记-SetTo登记")]
        RMA_Register_ToRegister = 703012,
        [Description("RMA-登记-SetTo检测")]
        RMA_Register_ToCheck = 703013,
        [Description("RMA-登记-SetTo催讨")]
        RMA_Register_ToASK = 703014,
        [Description("RMA-登记-SetTo发还")]
        RMA_Register_ToRevert = 703015,
        [Description("RMA-登记-SetTo退货入库")]
        RMA_Register_ToReturn = 703016,
        [Description("RMA-登记-SetToExecutive CC")]
        RMA_Register_ToECC = 703017,
        //
        [Description("RMA_登记_审核Revert")]
        RMA_Register_RevertAudit = 703010,
        [Description("RMA_更新_送修返还信息")]
        RMA_Register_ResponseInfo = 703011,
        //    rma revert 40   ----Add by Cindy

        [Description("RMA-送货-生成")]
        RMA_Revert_Create = 704001,
        [Description("RMA-送货-修改")]
        RMA_Revert_Update = 704002,
        [Description("RMA-送货-作废")]
        RMA_Revert_Abandon = 704003,
        [Description("RMA-送货-出库")]
        RMA_Revert_Out = 704004,
        [Description("RMA-送货-取消出库")]
        RMA_Revert_CancelOut = 704005,

        //           rma refund 50
        [Description("RMA-退货-生成")]
        RMA_Refund_Create = 705001,
        [Description("RMA-退货-修改")]
        RMA_Refund_Upate = 705002,
        [Description("RMA-退货-作废")]
        RMA_Refund_Abandon = 705003,
        [Description("RMA-退货-审核")]
        RMA_Refund_Audit = 705004,
        [Description("RMA-退货-取消审核")]
        RMA_Refund_CancelAudit = 705005,
        [Description("RMA-退货-退款")]
        RMA_Refund_Refund = 705006,
        [Description("RMA-退货-取消退款")]
        RMA_Refund_CancelRefund = 705007,

        //			rma return 60  ----Add By Cindy
        [Description("RMA-退货入库货-生成")]
        RMA_Return_Create = 706001,
        [Description("RMA-退货入库-修改")]
        RMA_Return_Update = 706002,
        [Description("RMA-退货入库-作废")]
        RMA_Return_Abandon = 706003,
        [Description("RMA-退货入库-入库")]
        RMA_Return_Return = 706004,
        [Description("RMA-退货入库-取消入库")]
        RMA_Return_CancelReturn = 706005,
        [Description("RMA-退货入库-审核")]
        RMA_Return_Audit = 706006,

        //			rma_request 70  ----Add By Cindy
        [Description("RMA-申请单-生成")]
        RMA_Request_Create = 707001,
        [Description("RMA-申请单-修改")]
        RMA_Request_Update = 707002,
        [Description("RMA-申请单-收货")]
        RMA_Request_Receive = 707003,
        [Description("RMA-申请单-取消收货")]
        RMA_Request_CancelReceive = 707004,
        [Description("RMA-申请单-作废")]
        RMA_Request_Abandon = 707005,
        [Description("RMA-申请单-关闭")]
        RMA_Request_Close = 707006,
        [Description("RMA-申请单-重复生成")]
        RMA_Request_ReCreate = 707007,
        [Description("RMA-申请单-审核通过")]
        RMA_Request_Audit = 707008,
        [Description("RMA-申请单-审核拒绝")]
        RMA_Request_Refused = 707009,


        //RMA自动处理---Add by dawnstar
        [Description("RMA-自动处理-申请单")]
        RMA_Auto_Request = 707008,
        [Description("RMA-自动处理-退款")]
        RMA_Auto_Refund = 707009,
        [Description("RMA-自动处理-退货入库")]
        RMA_Auto_Return = 707010,
        [Description("RMA-申请单-打印")]
        RMA_Request_PrintLabels = 707011,

        //Online 40
        //
        [Description("更新公告")]
        Online_Bulletin_Update = 401001,
        [Description("插入产品列表")]
        Online_List_Insert = 401002,
        [Description("删除产品列表")]
        Online_List_Delete = 401003,

        [Description("插入投票主项")]
        Online_Poll_Insert = 401004,
        [Description("更新投票主项")]
        Online_Poll_Update = 401005,
        [Description("插入投票明细")]
        Online_Poll_InsertItem = 401006,
        [Description("更新投票明细")]
        Online_Poll_UpdateItem = 401007,
        [Description("删除投票明细")]
        Online_Poll_DeleteItem = 401008,
        [Description("投票设定显示")]
        Online_Poll_Show = 401009,
        [Description("投票设定不显示")]
        Online_Poll_NotShow = 401010,

        [Description("客户反馈更新")]
        Online_FeedBack_Update = 401011,
        [Description("客户询问回复")]
        Online_ProductQuestion_Reply = 401012,

        //优惠券
        [Description("增加优惠券活动")]
        PromotionAdd = 402001,           //这里补充了编辑优惠券活动的类型。402017
        [Description("作废优惠券活动")]
        PromotionInvalid = 402002,
        [Description("取消作废优惠券活动")]
        PromotionValid = 402003,
        [Description("增加优惠券号码")]
        PromotionCodeAdd = 402004,              //这里补充删除、编辑优惠券号码的类型，402021
        [Description("作废优惠券号码")]
        PromotionCodeInvalid = 402005,
        [Description("取消作废优惠券号码")]
        PromotionCodeValid = 402006,           //这里补充增加、编辑、删除优惠券规则的类型。  402018
        [Description("分配优惠券号码")]
        PromotionCodeDistribute = 402007,
        [Description("删除用户优惠券号码")]
        PromotionCustomerDelete = 402008,
        [Description("设置优惠券商品范围")]
        PromotionProductAdd = 402009,
        [Description("删除优惠券适用商品")]
        PromotionProductDelete = 402010,

        [Description("维护优惠券活动")]
        PromotionEdit = 402017,
        [Description("增加优惠券规则")]
        PromotionLimitAdd = 402018,
        [Description("编辑优惠券规则")]
        PromotionLimitEdit = 402019,
        [Description("删除优惠券规则")]
        PromotionLimitDelete = 402020,
        [Description("删除优惠券号码")]
        PromotionCodeDelete = 402021,
        [Description("编辑优惠券号码")]
        PromotionCodeEdit = 402022,

        [Description("审核通过优惠券")]
        PromotionApprove = 402023,
        [Description("审核拒绝优惠券")]
        PromotionReject = 402024,
        [Description("审核作废优惠券")]
        PromotionAbandon = 402025,
        [Description("终止优惠券活动")]
        PromotionStop = 402030,
        [Description("优惠券活动提交审核")]
        PromotionSubmit = 402031,
        [Description("优惠券活动取消审核")]
        PromotionCancel = 402032,
        //竞猜
        [Description("增加竞猜活动")]
        BetAdd = 402011,
        [Description("修改竞猜活动")]
        BetUpdate = 402012,
        [Description("增加竞猜选项")]
        BetOptionAdd = 402013,
        [Description("删除竞猜选项")]
        BetOptionDelete = 402014,
        [Description("设置竞猜答案")]
        BetAnswerSet = 402015,
        [Description("竞猜活动开奖")]
        BetOpen = 402016,
        //赠品 4021**
        [Description("赠品-创建主信息")]
        SaleGiftCreateMaster = 402101,
        [Description("赠品-更新主信息")]
        SaleGiftUpdateMaster = 402102,
        [Description("赠品-促销活动规则设置")]
        SaleGiftSetSaleRules = 402103,
        [Description("赠品-赠品设置")]
        SaleGiftSetGiftItemRules = 402104,
        [Description("赠品-提交审核")]
        SaleGiftSubmitAudit = 402105,
        [Description("赠品-取消审核")]
        SaleGiftCancelAudit = 402106,
        [Description("赠品-审核通过")]
        SaleGiftAuditApprove = 402107,
        [Description("赠品-审核拒绝")]
        SaleGiftAuditRefuse = 402108,
        [Description("赠品-作废")]
        SaleGiftVoid = 402109,
        [Description("赠品-终止")]
        SaleGiftStop = 402110,
        //捆绑销售 4022**（销售规则）
        [Description("捆绑销售-创建")]
        ComboCreate = 402201,
        [Description("捆绑销售-更新")]
        ComboUpdate = 402202,
        [Description("捆绑销售-商品对应捆绑规则<成本价，状态改为待审核(status=1)")]
        ComboCheckPriceAndSetStatus = 402203,
        //团购 4023**
        [Description("团购-创建")]
        GroupBuyingCreate = 402301,
        [Description("团购-更新")]
        GroupBuyingUpdate = 402302,
        [Description("团购-作废")]
        GroupBuyingVoid = 402303,
        [Description("团购-终止")]
        GroupBuyingStop = 402304,
        [Description("团购-提交审核")]
        GroupBuyingSubmitAudit = 402305,
        [Description("团购-取消审核")]
        GroupBuyingCancelAudit = 402306,
        [Description("团购-审核通过")]
        GroupBuyingAuditApprove = 402307,
        [Description("团购-审核拒绝")]
        GroupBuyingAuditRefuse = 402308,

        //Add By KiddLiu 2005/07/06
        [Description("修改拍卖状态")]
        Auction_Status_Update = 405001,
        [Description("修改拍卖信息")]
        Auction_Detail_Update = 405002,
        [Description("新增拍卖信息")]
        Auction_Detail_Insert = 405003,
        [Description("禁止显示拍卖评论")]
        AuctionComment_Status_Update = 405004,
        [Description("处理捣蛋者")]
        Auction_Trubermaker_Insert = 405005,
        [Description("拍卖用户确认")]
        Auction_Status_Confirm = 405006,
        [Description("删除拍卖信息")]
        Auction_Status_Delete = 405007, //将Auction表中Status字段值置为AppConst.IntNull
        [Description("修改重置状态")]
        Auction_Reset_Status_Update = 405008,

        [Description("限时抢购增加")]
        Countdown_Insert = 408001,
        [Description("限时抢购修改")]
        Countdown_Update = 408002,
        [Description("限时抢购作废")]
        Countdown_Abandon = 408003,
        [Description("限时抢购终止")]
        Countdown_Interupt = 408004,

        [Description("限时抢购商品价格提交审核")]
        Countdown_Verify = 408005,
        [Description("限时抢购商品价格审核通过")]
        Countdown_VerifyPass = 408006,
        [Description("限时抢购商品价格审核未通过")]
        Countdown_VerifyFaild = 408007,

        //设置配送状态的枚举
        [Description("配送已送达")]
        Delivery_OK = 409001,
        [Description("配送取消")]
        Delivery_Abandon = Delivery_OK + 1,
        [Description("配送未送达")]
        Delivery_Failure = Delivery_OK + 2,
        [Description("配送未送货")]
        Delivery_NoAction = Delivery_OK + 3,
        [Description("配送再指定")]
        Delivery_ReAssign = Delivery_OK + 4,

        //供应商发票库
        [Description("添加供应商发票库")]
        PO_Vendor_Invoice_Add = 409006,
        [Description("修改供应商发票库")]
        PO_Vendor_Invoice_Update = PO_Vendor_Invoice_Add + 1,
        [Description("审核供应商发票库")]
        PO_Vendor_Invoice_Verify = PO_Vendor_Invoice_Add + 2,
        [Description("取消审核供应商发票库")]
        PO_Vendor_Invoice_CancelVerify = PO_Vendor_Invoice_Add + 3,
        [Description("作废供应商发票库")]
        PO_Vendor_Invoice_Abandon = PO_Vendor_Invoice_Add + 4,
        [Description("取消作废供应商发票库")]
        PO_Vendor_Invoice_CancelAbandon = PO_Vendor_Invoice_Add + 5,

        [Description("添加发票录入")]
        InvoiceInput_Add = 500000,
        [Description("发票提交审核")]
        InvoiceInput_SubmitAudit = 500001,
        [Description("发票取消审核")]
        InvoiceInput_CancelAudit = 500002,
        [Description("发票审核通过")]
        InvoiceInput_PassAudit = 500003,
        [Description("发票拒绝审核")]
        InvoiceInput_RefuseAudit = 500004,
        [Description("PO单入库")]
        PO_InStock = 700000,

        [Description("处理电汇邮局收款单")]
        Invoice_PostIncome_Handle = 500100,
        [Description("审核电汇邮局收款单")]
        Invoice_PostIncome_Confirm = 500101,
        [Description("取消审核电汇邮局收款单")]
        Invoice_PostIncome_CancelConfirm = 500102,
        [Description("处理电汇邮局收款单")]
        Invoice_PostIncome_Abandon = 500103,
        [Description("取消作废电汇邮局收款单")]
        Invoice_PostIncome_CancelAbandon = 500104,

        [Description("财务拒绝审核应付记录")]
        Invoice_Payable_RefuseAuditFinancePay = 500200,

        [Description("处理逾期收款跟踪单")]
        Invoice_TrackingInfo_UpdateTrackingInfo = 500300,
        [Description("逾期收款跟踪单责任人变更")]
        Invoice_TrackingInfo_ChangeResponsibleUserName = 500301,

        [Description("End no user")]
        ZZZZZ = 999999,

        [Description("备份库存数据")]
        InventoryBackUp = 999998,

        [Description("退款审核")]
        AuditRefund_Update = 60000,

        #region 广告
        [Description("新建广告")]
        Banner_Add = 666001,

        [Description("更新广告")]
        Banner_Update = 666002,
        [Description("作废广告")]
        Banner_Canel = 666003,
        #endregion

        #region 随心配
        [Description("创建随心配")]
        OptionalAccessories_Create = 667000,

        [Description("更新随心配")]
        OptionalAccessories_Update = 667001,

        [Description("审核随心配")]
        OptionalAccessories_Approve = 667002,

        [Description("删除随心配")]
        OptionalAccessories_Del = 667003,
        #endregion

        #region 配置单
        [Obsolete("此字段已弃用")]
        [Description("新建配置单")]
        ComputerConfig_Add = 668001,
        [Obsolete("此字段已弃用")]
        [Description("编辑配置单")]
        ComputerConfig_Update = 668002,
        [Obsolete("此字段已弃用")]
        [Description("作废配置单")]
        ComputerConfig_Void = 668003,
        [Obsolete("此字段已弃用")]
        [Description("审核配置单")]
        ComputerConfig_Aduit = 668004,


        #endregion

        #region 佣金结算单

        [Description("关闭佣金结算单")]
        Commission_CloseCommission = 600001,

        #endregion 佣金结算单

        #region 投票管理

        [Description("创建投票主题")]
        MKT_Poll_Master_Create = 160000,
        [Description("修改投票主题")]
        MKT_Poll_Master_Update = 160001,
        #endregion

        #region CRL20438 代销结算规则

        /// <summary>
        /// 修改代销结算规则
        /// </summary>
        [Description("修改代销结算规则")]
        ConsignSettleRule_Update = 202001,

        /// <summary>
        /// 创建代销结算规则
        /// </summary>
        [Description("创建代销结算规则")]
        ConsignSettleRule_Create = 202002,

        /// <summary>
        /// 审核代销结算规则
        /// </summary>
        [Description("审核代销结算规则")]
        ConsignSettleRule_Audit = 202003,

        /// <summary>
        /// 作废代销结算规则
        /// </summary>
        [Description("作废代销结算规则")]
        ConsignSettleRule_Abadon = 202004,

        /// <summary>
        /// 终止代销结算规则
        /// </summary>
        [Description("终止代销结算规则")]
        ConsignSettleRule_Stop = 202005,

        #endregion CRL20438 代销结算规则

        #region 销售采购变价单
        [Description("新增变价单")]
        PriceChange_Add = 303001,
        [Description("修改变价单")]
        PriceChange_Update = 303002,
        [Description("作废变价单")]
        PriceChange_Void = 303003,
        [Description("审核变价单")]
        PriceChange_Audit = 303004,
        [Description("启动变价单")]
        PriceChange_Run = 303005,
        [Description("中止变价单")]
        PriceChange_Aborted = 303006,
        [Description("人工启动变价单")]
        PriceChange_ManualRun = 303007,
        [Description("人工中止变价单")]
        PriceChange_ManualAborted = 303008,
        [Description("复制变价单")]
        PriceChange_Clone = 303009,
        #endregion

        #region 成本变价单
        [Description("新增成本变价单")]
        CostChange_Create = 303009,
        [Description("修改成本变价单")]
        CostChange_Update = 303010,
        [Description("作废成本变价单")]
        CostChange_Void = 303011,
        [Description("审核通过成本变价单")]
        CostChange_Audit = 303012,
        [Description("提交审核成本变价单")]
        CostChange_SubmitAudit = 303013,
        [Description("审核退回成本变价单")]
        CostChange_Refuse = 303014,
        [Description("撤销提交审核成本变价单")]
        CostChange_CancelSubmitAudit = 303015
        #endregion
    }

    /// <summary>
    /// 贸易类型  //泰隆贸易类型去掉直邮、自贸、其他，改为非自营，自营
    /// </summary>
    public enum TradeType
    {

        /// <summary>
        /// 直邮
        /// </summary>
        [Description("直邮")]
        DirectMail = 0,
        /// <summary>
        /// 自贸
        /// </summary>
        //[Description("自贸")]
        //FTA = 1,
        ///// <summary>
        ///// 其他
        ///// </summary>
        //[Description("其他")]
        //Other = 2

        ///// <summary>
        ///// 非自营
        ///// </summary>
        //[Description("非自营")]
        //DirectMail = 0,
        ///// <summary>
        ///// 自营
        ///// </summary>
        //[Description("自营")]
        //FTA = 1,
    }

    /// <summary>
    /// 免运费条件金额类型
    /// </summary>
    public enum FreeShippingAmountSettingType
    {
        /// <summary>
        /// 商品总金额
        /// </summary>
        [Description("商品总金额")]
        ProductAmt = 1,

        /// <summary>
        /// 商品总金额 – 折扣 – 优惠券抵用
        /// </summary>
        [Description("商品总金额 – 折扣 – 优惠券抵用")]
        ProductAmtReduceDiscountAmt = 2,

        ///// <summary>
        ///// 商品总金额+关税总金额
        ///// </summary>
        //[Obsolete("此字段已弃用")]
        //ProductAmtAddTariffAmt,

        ///// <summary>
        ///// (商品总金额 – 折扣 – 优惠券抵用) +关税总金额
        ///// </summary>
        //[Obsolete("此字段已弃用")]
        //ProductAmtReduceDiscountAmtAddTariffAmt,

        /// <summary>
        /// 总运费
        /// </summary>
        [Description("总运费")]
        ShippingAmt = 5
    }

    /// <summary>
    /// 免运费规则状态
    /// </summary>
    public enum FreeShippingAmountSettingStatus
    {
        /// <summary>
        /// 有效
        /// </summary>
        [Description("有效")]
        Active,
        /// <summary>
        /// 无效
        /// </summary>
        [Description("无效")]
        DeActive,
    }




}
