using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ECommerce.Enums
{
    public enum ProductStatus
    {
        /// <summary>
        /// 上架
        /// </summary>
        [Description("上架")]
        Active = 1,
        /// <summary>
        /// 上架仅展示
        /// </summary>
        [Description("上架仅展示")]
        InActive_Show = 0,
        /// <summary>
        /// 下架
        /// </summary>
        [Description("下架")]
        InActive_UnShow = 2,
        /// <summary>
        /// 作废
        /// </summary>
        [Description("作废")]
        Abandon = -1,
        /// <summary>
        /// 待审核
        /// </summary>
        [Description("待审核")]
        InActive_Auditing = 3,
        /// <summary>
        /// 已审核
        /// </summary> 
        [Description("已审核")]
        InActive_Audited = 4,
        /// <summary>
        /// 审核未通过
        /// </summary> 
        [Description("审核未通过")]
        InActive_AuditenNO = 5
    }

    /// <summary>
    /// 页面链接模式
    /// </summary>
    public enum FPCLinkUrlModeType
    {
        [Description("标准模式")]
        Standard = 0,
        [Description("促销活动页")]
        Promotion = 1,
        [Description("自定义Url")]
        Define = 999
    }

    /// <summary>
    /// 前台展示方式
    /// </summary>
    public enum UIModeType
    {
        [Description("实物类")]
        Physical = 0,
        //[Description("票务类")]
        //Service = 1,
        //[Description("酒店类")]
        //Ticket = 2
    }

    public enum ValidStatus
    {
        [Description("有效")]
        Active = 'A',
        [Description("无效")]
        DeActive = 'D'
    }

    /// <summary>
    /// 评论处理状态
    /// </summary>
    public enum ReviewProcessStatus
    {
        /// <summary>
        /// 待处理
        /// </summary>
        WaitHandling = 0,

        /// <summary>
        /// 处理中
        /// </summary>
        Handling = 1,

        /// <summary>
        /// 处理完毕
        /// </summary>
        Finish = 2,

        /// <summary>
        /// 复核处理
        /// </summary>
        Review = 4
    }

    /// <summary>
    /// 评论类型
    /// </summary>
    public enum ReviewType
    {
        /// <summary>
        /// 普通评论
        /// </summary>
        [Description("普通评论")]
        CommonReview = 0,

        /// <summary>
        /// 晒单评论
        /// </summary>
        [Description("晒单评论")]
        SDReview = 1
    }

    /// <summary>
    /// 付款类型
    /// </summary>
    public enum ProductPointType
    {
        /// <summary>
        /// 仅现金
        /// </summary>
        [Description("仅现金")]
        MoneyOnly = 0,
        ///// <summary>
        ///// 均支持
        ///// </summary>
        //[Description("均支持")]
        //All = 1,
        ///// <summary>
        ///// 仅积分
        ///// </summary>
        //[Description("仅积分")]
        //PointOnly = 2
    }

    /// <summary>
    /// 商品备案状态
    /// </summary>
    public enum ProductEntryStatus
    {
        /// <summary>
        /// 备案失败
        /// </summary>
        [Description("备案失败")]
        EntryFail = -2,
        /// <summary>
        /// 审核失败
        /// </summary>
        [Description("审核失败")]
        AuditFail = -1,
        /// <summary>
        /// 初始化
        /// </summary>
        [Description("初始化")]
        Origin = 0,
        /// <summary>
        /// 待审核
        /// </summary>
        [Description("待审核")]
        WaitingAudit = 1,
        /// <summary>
        /// 审核成功
        /// </summary>
        [Description("审核成功")]
        AuditSucess = 2,
        /// <summary>
        /// 备案中
        /// </summary>
        [Description("备案中")]
        Entry = 3,
        /// <summary>
        /// 备案成功
        /// </summary>
        [Description("备案成功")]
        EntrySuccess = 4
    }

    /// <summary>
    /// 商品业务类型
    /// </summary>
    public enum ProductBizType
    {
        /// <summary>
        /// 一般进口
        /// </summary>
        [Description("一般进口")]
        Normal = 0,
        /// <summary>
        /// 保税进口
        /// </summary>
        [Description("保税进口")]
        Tax = 1
    }

    /// <summary>
    /// 运输存储方式
    /// </summary>
    public enum ProductStoreType
    {
        /// <summary>
        /// 常温
        /// </summary>
        [Description("常温")]
        Normal = 0,
        /// <summary>
        /// 冷藏
        /// </summary>
        [Description("冷藏")]
        Refrigeration = 1,
        /// <summary>
        /// 冷冻
        /// </summary>
        [Description("冷冻")]
        Freeze = 2
    }

    public enum PurchaseOrderConsignFlag
    {
        /// <summary>
        /// 经销
        /// </summary>
        [Description("经销")]
        UnConsign = 0,
        /// <summary>
        /// 代销
        /// </summary>
        [Description("代销")]
        Consign = 1,
        /// <summary>
        /// 代收代付
        /// </summary>
        [Description("代收代付")]
        GatherPay = 4
    }

    public enum PurchaseOrderStatus
    {
        [Description("初始化")]
        Origin = -3,

        [Description("已退回")]
        Returned = -2,

        [Description("自动作废")]
        AutoAbandoned = -1,

        [Description("已作废")]
        Abandoned = 0,

        [Description("已创建")]
        Created = 1,

        [Description("待入库")]
        WaitingInStock = 3,

        [Description("已入库")]
        InStocked = 4,

        [Description("待审核")]
        WaitingAudit = 5,

        [Description("部分入库")]
        PartlyInStocked = 6,

        [Description("手动关闭")]
        ManualClosed = 7,

        [Description("系统关闭")]
        SystemClosed = 8,

        [Description("商家关闭")]
        VendorClosed = 9,

        //[Description("待PM确认")]
        //WaitingPMConfirm = 10,

        [Description("待申报")]
        WaitingReport = 11,

        [Description("申报中")]
        Reporting = 12,
    }

    public enum PurchaseOrderType
    {
        [Description("正常")]
        Normal = 0,

        [Description("负采购")]
        Negative = 1,

        [Description("调价单")]
        Adjust = 3
    }

    public enum PurchaseOrderETAHalfDayType
    {
        /// <summary>
        /// 上午
        /// </summary>
        [Description("上午")]
        AM,
        /// <summary>
        /// 下午
        /// </summary>
        [Description("下午")]
        PM
    }

    /// <summary>
    /// PM查询条件枚举
    /// </summary>
    public enum PMQueryType
    {
        [Description("返回自己和有效状态的备份PM")]
        Self,

        [Description("如果帐号同时是TL，返回自己及自己在PM组中所管理的所有有效状态的PM和其有效状态的备份PM,否则仅自已和有效状态的备份PM")]
        Team,

        [Description("返回PM列表中所有有效状态的PM")]
        AllValid,

        [Description("返回自己和备份有效状态PM及所有无效状态的PM.")]
        SelfAndInvalid,

        [Description("返回自己及自己在PM组中所管理的所有有效状态的PM和其有效状态的备份PM及所有无效状态的P")]
        TeamAndInvalid,

        [Description("返回PM列表中所有PM(全部：包括无效)")]
        All,

        [Description("无信息")]
        None
    }

    public enum PaySettleCompany
    {
        [Description("上海")]
        SH = 3270,

        [Description("北京")]
        BJ = 3271
    }
    /// <summary>
    /// 商品库存调整单状态
    /// </summary>
    public enum ProductStockAdjustStatus
    {
        /// <summary>
        /// 作废
        /// </summary>
        [Description("作废")]
        Abandon = -2,
        /// <summary>
        /// 审核不通过
        /// </summary>
        [Description("审核不通过")]
        AuditFaild = -1,
        /// <summary>
        /// 初稿
        /// </summary>
        [Description("初稿")]
        Origin = 0,
        /// <summary>
        /// 待审核
        /// </summary>
        [Description("待审核")]
        WaitingAudit = 1,
        /// <summary>
        /// 审核通过
        /// </summary>
        [Description("审核通过")]
        AuditPass = 2
    }


    /// <summary>
    /// 增值税率类型
    /// </summary>
    public enum PurchaseOrderTaxRate
    {
        /// <summary>
        /// 0%
        /// </summary>
        Percent000 = 0,
        /// <summary>
        /// 4%
        /// </summary>
        Percent004 = 4,
        /// <summary>
        /// 6%
        /// </summary>
        Percent006 = 6,
        /// <summary>
        /// 13%
        /// </summary>
        Percent013 = 13,
        /// <summary>
        /// 17%
        /// </summary>
        Percent017 = 17,
    }

    /// <summary>
    /// PO SSB消息类型
    /// </summary>
    public enum PurchaseOrderSSBMsgType
    {
        /// <summary>
        /// 创建
        /// </summary>
        I,

        /// <summary>
        /// 更新
        /// </summary>
        U,

        /// <summary>
        /// 关闭
        /// </summary>
        C
    }
    /// <summary>
    /// 采集日期类型
    /// </summary>
    public enum CollectDateType
    {
        /// <summary>
        /// 过期日期
        /// </summary>
        [Description("过期日期")]
        ExpiredDate,
        /// <summary>
        /// 生产日期
        /// </summary>
        [Description("生产日期")]
        ManufactureDate
    }

    /// <summary>
    /// 订单产品价格类型
    /// </summary>
    public enum SOProductPriceType
    {
        /// <summary>
        /// 正常价格
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 会员价格
        /// </summary>
        Member = 1,
        /// <summary>
        /// 批发价格
        /// </summary>
        WholeSale = 2,
        /// <summary>
        /// 金账户专享价格
        /// </summary>
        GoldAcc = 3,
        /// <summary>
        /// 关爱通专享价格
        /// </summary>
        GuanAiAcc = 4,
        /// <summary>
        /// 盛大会员专享价格
        /// </summary>
        SdoAccPrice = 5
    }
}
