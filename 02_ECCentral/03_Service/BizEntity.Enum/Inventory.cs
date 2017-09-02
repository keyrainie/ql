using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.Inventory
{
    
    [Description("ECCentral.BizEntity.Enum.Resources.ResInventoryEnum")]
    public enum ValidStatus
    {
        /// <summary>
        /// 有效
        /// </summary>
        Valid = 0,
        /// <summary>
        /// 无效
        /// </summary>
        InValid = -1
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResInventoryEnum")]
    public enum YNStatus
    {
        Yes = 1,
        No = 0
    }


    [Description("ECCentral.BizEntity.Enum.Resources.ResInventoryEnum")]
    public enum WarehouseType
    {
        /// <summary>
        /// 实仓
        /// </summary>
        Real = 1,
        /// <summary>
        /// 虚仓
        /// </summary>
        Virtual = 2
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResInventoryEnum")]
    public enum WarehouseOwnerType
    {
        /// <summary>
        /// 自有
        /// </summary>
        Self = 1,
        /// <summary>
        /// 第三方
        /// </summary>
        ThirdParty = 2
    }

    /// <summary>
    /// 单据代销标志
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInventoryEnum")]
    public enum RequestConsignFlag
    {

        /// <summary>
        /// 代销
        /// </summary>
        Consign = 1,
        /// <summary>
        /// 非代销
        /// </summary>
        NotConsign = 0,

         /// <summary>
        /// 代收代付
        /// </summary>
        GatherPay = 4



    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResInventoryEnum")]
    public enum AdjustRequestStatus
    {
        /// <summary>
        /// 初始
        /// </summary>
        Origin = 1,
        /// <summary>
        /// 已审核
        /// </summary>
        Verified = 2,
        /// <summary>
        /// 已申报
        /// </summary>
        Reported = 4,
        /// <summary>
        /// 已出库
        /// </summary>
        OutStock = 3,
        /// <summary>
        /// 已作废
        /// </summary>
        Abandon = -1
    }

    /// <summary>
    /// 转换单 - 状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInventoryEnum")]
    public enum ConvertRequestStatus
    {
        /// <summary>
        /// 初始
        /// </summary>
        Origin = 1,
        /// <summary>
        /// 已审核
        /// </summary>
        Verified = 2,
        /// <summary>
        /// 已出库
        /// </summary>
        OutStock = 3,
        /// <summary>
        /// 已作废
        /// </summary>
        Abandon = -1
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResInventoryEnum")]
    public enum LendRequestStatus
    {
        /// <summary>
        /// 初始
        /// </summary>        
        Origin = 1,
        /// <summary>
        /// 已审核
        /// </summary>        
        Verified = 2,
        /// <summary>
        /// 已出库
        /// </summary>        
        OutStock = 3,
        /// <summary>
        /// 作废
        /// </summary>                
        Abandon = -1,
        /// <summary>
        /// 部分归还
        /// </summary>                
        ReturnPartly = 4,
        /// <summary>
        /// 全部归还
        /// </summary>                
        ReturnAll = 5
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResInventoryEnum")]
    public enum ExperienceHallStatus
    {
        /// <summary>
        /// 已作废
        /// </summary>
        Abandon = -1,
        /// <summary>
        /// 已创建
        /// </summary>
        Created = 0,

        /// <summary>
        /// 已审核
        /// </summary>
        Audit = 2,

        /// <summary>
        /// 已调拨
        /// </summary>
        Experienced = 1
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResInventoryEnum")]
    public enum AllocateType
    {
        /// <summary>
        /// 调拨入体验厅
        /// </summary>
        ExperienceIn = 0,
        /// <summary>
        /// 调拨出体验厅
        /// </summary>
        ExperienceOut = 1
    }

    /// <summary>
    /// 移仓单状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInventoryEnum")]
    public enum ShiftRequestStatus
    {

        /// <summary>
        /// 初始
        /// </summary>
        Origin = 1,
        /// <summary>
        /// 已审核
        /// </summary>
        Verified = 2,
        /// <summary>
        /// 已出库
        /// </summary>
        OutStock = 3,
        /// <summary>
        /// 已入库
        /// </summary>
        InStock = 4,
        /// <summary>
        /// 部分入库
        /// </summary>
        PartlyInStock = 5,
        /// <summary>
        /// 作废
        /// </summary>
        Abandon = -1
    }

    /// <summary>
    /// 移仓跟进日志状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInventoryEnum")]
    public enum ShiftRequestMemoStatus
    {

        /// <summary>
        /// 需要跟进
        /// </summary>
        FollowUp = 1,
        /// <summary>
        /// 处理完毕
        /// </summary>
        Finished = 0
    }

    /// <summary>
    /// 虚库单状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInventoryEnum")]
    public enum VirtualRequestStatus
    {
        /// <summary>
        /// 审核未通过
        /// </summary>        
        Rejected = -1,
        /// <summary>
        /// 待审核
        /// </summary>
        Origin = 0,
        /// <summary>
        /// 已审核
        /// </summary>
        Approved = 1,
        /// <summary>
        /// 运行中
        /// </summary>
        Running = 2,
        /// <summary>
        /// 已完成
        /// </summary>
        Completed = 3,
        /// <summary>
        /// 关闭中
        /// </summary>
        Closing = 4
    }

    /// <summary>
    /// 虚库单操作类型状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInventoryEnum")]
    public enum VirtualRequestActionType
    {
        /// <summary>
        /// 运行
        /// </summary>
        Run,

        /// <summary>
        /// 关闭
        /// </summary>
        Close
    }


    [Description("ECCentral.BizEntity.Enum.Resources.ResInventoryEnum")]
    public enum ConvertProductType
    {
        [Description("源商品")]
        Source = 1,
        [Description("目标商品")]
        Target = 2
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResInventoryEnum")]
    public enum AdjustRequestProperty
    {
        /// <summary>
        /// 盘点损益
        /// </summary>
        CheckStock,
        /// <summary>
        /// 物流全赔损益
        /// </summary>
        AllShipping,
        /// <summary>
        /// 物流部分赔损益
        /// </summary>
        PartialShipping
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResInventoryEnum")]
    public enum ShiftRequestType
    {
        /// <summary>
        /// 正移仓单
        /// </summary>
        Positive = 0,

        /// <summary>
        /// 负移仓单
        /// </summary>
        Negative = 1,

        /// <summary>
        /// Po直接生成移仓单
        /// </summary>
        CreateByPo = 2,

        /// <summary>
        /// SO直接生成移仓单
        /// </summary>
        CreateBySo = 3
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResInventoryEnum")]
    public enum SpecialShiftRequestType
    {
        /// <summary>
        /// 非特殊移仓单
        /// </summary>
        Default = 0,

        /// <summary>
        /// 自动-延期出库
        /// </summary>
        DelayOutStock = 1,

        /// <summary>
        /// 自动-紧急延期入库
        /// </summary>
        UrgencyDelayInStock = 2,

        /// <summary>
        /// 自动-普通延期入库
        /// </summary>
        NormalDelayInStock = 3,

        /// <summary>
        /// 手动
        /// </summary>
        HandWork = 4
    }

    //[Description("ECCentral.BizEntity.Enum.Resources.ResInventoryEnum")]
    //public enum ConsignFlag
    //{
    //    [Description("代销")]
    //    Consign,
    //    [Description("非代销")]
    //    NotConsign
    //}

    /// <summary>
    /// 库存调整模式类型:
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInventoryEnum")]
    public enum InventoryAdjustModeType
    {
        Increase,
        Overwrite

    }
    /// <summary>
    /// 库存调整原因类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInventoryEnum")]
    public enum InventoryAdjustReasonType
    {
        InventoryDoc_SetVirtualInventory = 501510,
        InventoryDoc_Lend = 501119,
        InventoryDoc_Adjust = 501219,

        InventoryDoc_Transfer = 501319, //IT
        InventoryDoc_Transfer_Consign = 501320, //IT

        InventoryDoc_Convert = 501419,
        RMA_Revert = 704004,
        PO_PurchaseOrder = 201118,
        PurchaseQty_Adjust = 113017,
        Order_SalesOrder = 600609,
        Other = 0
    }

    /// <summary>
    /// 库存操作类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInventoryEnum")]
    public enum InventoryActionType
    {
        Adjust_Create,
        Adjust_Abandon,
        Adjust_CancelAbandon,
        Adjust_Update,
        Adjust_OutStock,

        Convert_Create,
        Convert_Abandon,
        Convert_CancelAbandon,
        Convert_Update,
        Convert_OutStock,

        Lend_Create,
        Lend_Abandon,
        Lend_CancelAbandon,
        Lend_Update,
        Lend_OutStock,
        Lend_Return,

        Shift_Create,
        Shift_Update,
        Shift_Abandon,
        Shift_AbandonforPO,
        Shift_CancelAbandon,
        Shift_InStock,
        Shift_OutStock,

        Virtual_Create
    }
    /// <summary>
    /// 单据商品类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInventoryEnum")]
    public enum RequestProductItemType
    {
        /// <summary>
        /// 普通
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 二手品
        /// </summary>
        SecondHand = 1,
        /// <summary>
        /// 坏品
        /// </summary>
        Bad = 2,
        /// <summary>
        /// 其它
        /// </summary>
        OtherItem = 3
    }

    /// <summary>
    /// 入库状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResInventoryEnum")]
    public enum InStockStatus
    {
        /// <summary>
        /// 零入库
        /// </summary>
        InStockZero = 0,
        /// <summary>
        /// 部分入库
        /// </summary>
        InStockPartly = 1,
        /// <summary>
        /// 完全入库
        /// </summary>
        InStockWhole = 2
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResInventoryEnum")]
    public enum InventoryAdjustSourceBizFunction
    {
        /// <summary>
        /// 损益单调整库存
        /// </summary>
        Inventory_AdjustRequest,
        /// <summary>
        /// 转换单调整库存
        /// </summary>
        Inventory_ConvertRequest,
        /// <summary>
        /// 移仓
        /// </summary>
        Inventory_ShiftRequest,
        /// <summary>
        /// 借货单
        /// </summary>
        Inventory_LendRequest,
        /// <summary>
        /// 虚库
        /// </summary>
        Inventory_VirtualRequest,
        /// <summary>
        /// SO
        /// </summary>
        SO_Order,
        /// <summary>
        /// PO
        /// </summary>
        PO_Order,
        /// <summary>
        /// RMA
        /// </summary>
        RMA_OPC,
        /// <summary>
        /// Seller
        /// </summary>
        Seller_Inventory,
        /// <summary>
        /// 渠道
        /// </summary>
        Channel_Inventory,
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResInventoryEnum")]
    public enum InventoryAdjustSourceAction
    {
        Default,
        /// <summary>
        /// 取消/作废，不恢复库存
        /// </summary>
        Abandon,
        AbandonForPO,
        [Description("作废，并恢复库存")]
        Abandon_RecoverStock,
        Allocate,
        Audit,
        CancelAudit,
        CancelAbandon,
        Close,
        Create,
        CreateForJob,
        Change,
        InStock,
        OutStock,
        Pending,
        Return,
        Reject,
        Run,
        //ShipOut,
        StopInStock,
        Update,
        //Void,
        //Void_Recover,
        WHUpdate
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResInventoryEnum")]
    public enum VirtualTransferType
    { 
        Yes,
        No,
        Empty
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResInventoryEnum")]
    public enum RMAShiftStatus
    {
        /// <summary>
        /// 作废
        /// </summary>
        Abandon = -1,
        /// <summary>
        /// 初始
        /// </summary>
        Origin = 0,
        /// <summary>
        /// 已审核
        /// </summary>
        Verified = 1,
        /// <summary>
        /// 已出库
        /// </summary>
        OutStock = 2,
        /// <summary>
        /// 部分入库
        /// </summary>
        PartlyInStock = 3,
        /// <summary>
        /// 已入库
        /// </summary>
        InStock = 4
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResInventoryEnum")]
    public enum RMAShiftType
    {
        IT=0,
        RT=1,
        OPC_RMA=2
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResInventoryEnum")]
    public enum BatchStatus
    {
        [Description("正常")]
        A,
        [Description("临期")]
        R,
        [Description("过期")]
        I
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResInventoryEnum")]
    public enum CustomsCodeMode
    {
        /// <summary>
        /// 直邮进口模式
        /// </summary>
        DirectImportMode = 2244,
        /// <summary>
        /// 浦东机场自贸模式
        /// </summary>
        PudongAirportTradeMode = 2216,
        /// <summary>
        /// 洋山自贸模式
        /// </summary>
        YangshanTradeMode = 2249,
        /// <summary>
        /// 外高桥自贸模式
        /// </summary>
        WaigaoqiaoTradeMode = 2218
    }
    

}
