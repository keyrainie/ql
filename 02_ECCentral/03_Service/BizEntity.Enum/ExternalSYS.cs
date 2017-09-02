using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ECCentral.BizEntity.ExternalSYS
{
    [Description("ECCentral.BizEntity.Enum.Resources.ResExternalEnum")]
    public enum ValidStatus
    {
        Active = 'A',
        DeActive = 'D'
    }

    /// <summary>
    /// 收款类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResExternalEnum")]
    public enum ReceiveType
    {
        /// <summary>
        /// 现金
        /// </summary>
        Cash = 1,
        /// <summary>
        /// 帐扣
        /// </summary>
        AccountDeduction = 2,
    }

    /// <summary>
    /// 费用类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResExternalEnum")]
    public enum EIMSType
    {
        /// <summary>
        /// 市场发展基金(MDF)
        /// </summary>
        MDF = 101,
        /// <summary>
        /// 合同返利(VIR)
        /// </summary>
        VIR = 102,
        /// <summary>
        /// 商品管理等费用(Co-op)
        /// </summary>
        Coop = 103,
        /// <summary>
        /// 销售返点(SR)
        /// </summary>
        SR = 104,
        /// <summary>
        /// 价保(PP)
        /// </summary>
        PP = 105,
        /// <summary>
        /// 日常进货奖励(POR)
        /// </summary>
        POR = 106,
        /// <summary>
        /// 市场活动专项费(MKT)
        /// </summary>
        MKT = 130,
        /// <summary>
        /// 现金返点(CR)
        /// </summary>
        CR = 131,
        /// <summary>
        /// 运费返利(FRF)
        /// </summary>
        FRF = 133
    }

    /// <summary>
    /// 单据状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResExternalEnum")]
    public enum InvoiceStatus
    {
        /// <summary>
        /// 全部收到(关闭)
        /// </summary>
        AutoClose = 'F',
        /// <summary>
        /// 打开
        /// </summary>
        Open = 'O',
        /// <summary>
        /// 编辑锁定
        /// </summary>
        Lock = 'E'
    }

    /// <summary>
    /// 广告类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResExternalEnum")]
    public enum AdvertisingType
    {
        /// <summary>
        /// 图片
        /// </summary>
        IMG = 'I',
        /// <summary>
        /// 文字
        /// </summary>
        TEXT = 'T',
        /// <summary>
        /// 自定义
        /// </summary>
        Custom = 'C'
    }
    [Description("ECCentral.BizEntity.Enum.Resources.ResExternalEnum")]
    public enum CPSOrderType
    {
        /// <summary>
        /// 订单
        /// </summary>
        SO,
        /// <summary>
        /// 退款单
        /// </summary>
        RMA
    }

    /// <summary>
    /// 结算单状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResExternalEnum")]
    public enum FinanceStatus
    {
        /// <summary>
        /// 已付款
        /// </summary>
        Paid = 'P',
        /// <summary>
        /// 已结算
        /// </summary>
        Settled = 'S',
        /// <summary>
        /// 未提交申请
        /// </summary>
        UnRequest = 'R',
        /// <summary>
        /// 未结算
        /// </summary>
        Unsettled = 'U',
        /// <summary>
        /// 已作废
        /// </summary>
        Abandon = 'V'
    }

    /// <summary>
    /// 佣金兑现申请单状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResExternalEnum")]
    public enum ToCashStatus
    {
        /// <summary>
        /// 已付款
        /// </summary>
        Paid = 'P',
        /// <summary>
        /// 已申请
        /// </summary>
        Requested = 'R',
        /// <summary>
        /// 已确认
        /// </summary>
        Confirmed = 'C'
    }
    /// <summary>
    /// 审核状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResExternalEnum")]
    public enum AuditStatus
    {
        [Description("审核通过")]
        AuditClearance = 'A',
        [Description("审核未通过")]
        AuditNoClearance = 'D',
        [Description("待审核")]
        AuditReady = 'O'
    }
    /// <summary>
    /// 账户类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResExternalEnum")]
    public enum UserType
    {
        [Description("个人账户")]
        Personal = 'P',

        [Description("企业账户")]
        Enterprise = 'E',

        //[Description("其他账户")]
        //Other = 'O',
    }
    [Description("ECCentral.BizEntity.Enum.Resources.ResExternalEnum")]
    public enum IsActive
    {
        [Description("是")]
        Active = 'A',
        [Description("否")]
        DeActive = 'D'

    }
    [Description("ECCentral.BizEntity.Enum.Resources.ResExternalEnum")]
    public enum IsLock
    {
        [Description("是")]
        Lock = 'L',
        [Description("否")]
        UnLock = 'U'
    }

    /// <summary>
    /// 是否提供发票
    /// </summary>
    public enum CanProvideInvoice
    {
        /// <summary>
        /// 提供发票
        /// </summary>
        [Description("是")]
        Yes = 'Y',

        /// <summary>
        /// 不提供发票
        /// </summary>
        [Description("否")]
        No = 'N'
    }
}
