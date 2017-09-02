using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Store.Vendor
{
    /// <summary>
    /// 供应商审核申请类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum VendorModifyRequestType
    {
        /// <summary>
        /// 财务信息
        /// </summary>
        Finance = 0,
        /// <summary>
        /// 基本信息
        /// </summary>
        Vendor = 1,
        /// <summary>
        /// 代理信息
        /// </summary>
        Manufacturer = 2,
        /// <summary>
        /// 售后信息
        /// </summary>
        AfterSale = 3,
        /// <summary>
        /// 下单日期
        /// </summary>
        BuyWeekDay = 4,
        ///手工添加
        Manual = 5
    }
    /// <summary>
    /// 供应商财务结算方式
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResPOEnum")]
    public enum VendorSettlePeriodType
    {
        /// <summary>
        /// 手工结算
        /// </summary>
        Manual = 1,
        /// <summary>
        /// 本月结，每月10/25日
        /// </summary>
        PerMonth = 2
    }
    public enum ManufacturerStatus
    {
        [Description("有效")]
        Active = 0,
        [Description("无效")]
        DeActive = -1
    }
    public enum SalesPolicyType
    {
        /// <summary>
        /// 价格库存与时间无关
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 价格库存与时间相关
        /// </summary>
        TimeRelated = 1
    }
    public enum BusinessType
    {
        /// <summary>
        /// 实物
        /// </summary>
        Physical,
        /// <summary>
        /// 酒店房间
        /// </summary>
        Room,
        /// <summary>
        /// 票务类
        /// </summary>
        Ticket,
        /// <summary>
        /// 普通服务类
        /// </summary>
        Service,
        /// <summary>
        /// 导游
        /// </summary>
        TourGuide,
        /// <summary>
        /// 租车
        /// </summary>
        CarRental,
        /// <summary>
        /// 机票
        /// </summary>
        //AirTicket
    }
    public enum CategoryStatus
    {
        [Description("有效")]
        Active = 0,
        [Description("无效")]
        DeActive = -1
    }
    public enum BrandStoreType
    {
        [Description("普通店")]
        OrdinaryStore = 0,
        [Description("专卖店")]
        MonopolyStore = 1,
        [Description("旗舰店")]
        FlagshipStore = 2
    }
    public enum ValidStatus
    {
        [Description("有效")]
        Active = 'A',
        [Description("无效")]
        DeActive = 'D'
    }
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
    public enum VendorModifyRequestStatus
    {
        /// <summary>
        /// 待审核
        /// </summary>
        [Description("待审核")]
        Apply = 0,
        /// <summary>
        /// 审核通过
        /// </summary>
        [Description("审核通过")]
        VerifyPass = 1,
        /// <summary>
        /// 审核未通过
        /// </summary>
        [Description("审核未通过")]
        VerifyUnPass = -1,
        /// <summary>
        /// 取消审核
        /// </summary>
        [Description("取消审核")]
        CancelVerify = -2,
        /// <summary>
        /// 普通（日志记录）
        /// </summary>
        [Description("普通（日志记录）")]
        Common = -3,
        /// <summary>
        /// 草稿,未提交的审核,可以删除
        /// </summary>
        [Description("未提交")]
        Draft = -4
    }
    public enum VendorModifyActionType
    {
        /// <summary>
        /// 普通
        /// </summary>
        Normal,
        //新增
        Add,
        /// <summary>
        /// 修改
        /// </summary>
        Edit,
        /// <summary>
        /// 删除
        /// </summary>
        Delete,
        /// <summary>
        /// 其它
        /// </summary>
        Other
    }
    public enum VendorRowState
    {
        Unchanged,

        Added,

        Deleted,

        Modified
    }
    public enum VendorStatus
    {
        /// <summary>
        /// 不可用
        /// </summary>
        UnAvailable = -1,
        /// <summary>
        /// 可用
        /// </summary>
        Available = 0
    }
    public enum VendorIsCooperate
    {/// <summary>
        /// 是
        /// </summary>
        Yes = 1,
        /// <summary>
        /// 否
        /// </summary>
        No = 0
    }
    public enum VendorType
    {/// <summary>
        /// 来自IPP
        /// </summary>
        IPP = 0,
        /// <summary>
        /// 来自VendorPortal
        /// </summary>
        VendorPortal = 1
    }
    public enum VendorConsignFlag
    {/// <summary>
        /// 经销
        /// </summary>
        Sell = 0,
        /// <summary>
        /// 代销
        /// </summary>
        Consign = 1,
        /// <summary>
        /// 代收
        /// </summary>
        Gather = 3
    }
    public enum VendorRank
    {/// <summary>
        /// A
        /// </summary>
        A,
        /// <summary>
        /// B
        /// </summary>
        B,
        /// <summary>
        /// C
        /// </summary>
        C
    }
    public enum VendorInvoiceType
    { 
        /// <summary>
        /// 新蛋开票
        /// </summary>
        [Description("平台经销供应商")]
        NEG,
        /// <summary>
        /// 商家开票
        /// </summary>
        [Description("平台入驻卖家")]
        MET,
        /// <summary>
        /// 导购模式
        /// </summary>
        [Description("导购模式")]
        GUD
    }
    public enum VendorStockType
    {
        /// <summary>
        /// 自贸仓（原：跨境通仓储）
        /// </summary>
        [Description("自贸仓")]
        NEG,
        /// <summary>
        /// 直邮（原：商家仓储）
        /// </summary>
        [Description("直邮")]
        MET,
        /// <summary>
        /// 自贸仓+直邮
        /// </summary>
        [Description("自贸仓+直邮")]
        NAM,
    }
    public enum VendorShippingType
    { /// <summary>
        /// 新蛋配送
        /// </summary>
        NEG,
        /// <summary>
        /// 商家配送
        /// </summary>
        MET
    }
    /// <summary>
    /// 供应商代理状态
    /// </summary>
    public enum VendorAgentStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 存在待审核状态的代理请求
        /// </summary>
        Requesting = 1,
        Draft=2
    }
    //public enum VendorRank { }
    //public enum VendorRank { }
    //public enum VendorRank { }
    //public enum VendorRank { }
}
