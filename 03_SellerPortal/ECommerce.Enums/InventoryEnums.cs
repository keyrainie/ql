using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Enums
{
    /// <summary>
    /// 成本库存锁定操作
    /// </summary>
    public enum CostLockType
    {
        /// <summary>
        /// 不处理锁定成本库存
        /// </summary>
        NoUse,
        /// <summary>
        /// 锁定
        /// </summary>
        Lock,
        /// <summary>
        /// 解锁
        /// </summary>
        Unlock
    }

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
    /// 单据代销标志
    /// </summary>
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


    public enum InventoryAdjustSourceAction
    {
        Default,
        /// <summary>
        /// 取消/作废，不恢复库存
        /// </summary>
        Abandon,
        AbandonForPO,
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

    public enum ConsignToAccountType
    {
        /// <summary>
        /// 销售单
        /// </summary>
        SO = 0,
        /// <summary>
        /// 物流拒收
        /// </summary>
        //ShippingRefuse = 1,
        /// <summary>
        /// Manual
        /// </summary>
        Manual = 1,
        /// <summary>
        /// 损益单
        /// </summary>
        Adjust = 2,
        /// <summary>
        /// RMA发货
        /// </summary>
        RMA = 3
        /// <summary>
        /// 正常品入库
        /// </summary>
        //Normal = 4

        /// <summary>
        /// 补偿退款单
        /// </summary>
        //RO_Adjust = 6
    }

    /// <summary>
    /// AccountLog状态
    /// </summary>
    public enum ConsignToAccountLogStatus
    {
        /// <summary>
        /// 待结算
        /// </summary>
        Origin,
        Finance,
        /// <summary>
        /// 已结算
        /// </summary>
        Settled,
        /// <summary>
        /// 系统已建
        /// </summary>
        SystemCreated,
        /// <summary>
        /// 人工已建
        /// </summary>
        ManualCreated
    }


    /// <summary>
    /// 结算类型
    /// </summary>
    public enum SettleType
    {
        /// <summary>
        /// 传统结算
        /// </summary>
        O,
        /// <summary>
        /// 佣金百分比结算
        /// </summary>
        P
    }
}
