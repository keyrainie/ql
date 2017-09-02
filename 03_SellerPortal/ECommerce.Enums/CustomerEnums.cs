using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ECommerce.Enums
{
    public enum CustomerRank
    {
        /// <summary>
        /// 初级会员；铁牌会员
        /// </summary>
        [Description("一钻会员")]
        Ferrum = 1,
        /// <summary>
        /// 铜牌会员
        /// </summary>
        [Description("二钻会员")]
        Copper = 2,
        /// <summary>
        /// 银牌会员
        /// </summary>
        [Description("三钻会员")]
        Silver = 3,
        /// <summary>
        /// 金牌会员
        /// </summary>
        [Description("四钻会员")]
        Golden = 4,
        /// <summary>
        /// 钻石会员
        /// </summary>
        [Description("五钻会员")]
        Diamond = 5,
        /// <summary>
        /// 皇冠会员
        /// </summary>
        [Description("六钻会员")]
        Crown = 6,
        /// <summary>
        /// 至尊会员
        /// </summary>
        [Description("七钻会员")]
        Supremacy = 7,

        [Description("八钻会员")]
        Eight = 8
    }

    public enum CustomerStatus
    {
        [Description("有效")]
        Valid,

        [Description("无效")]
        InValid
    }

    public enum Gender
    {
        /// <summary>
        /// 男
        /// </summary>
        [Description("男")]
        Male = 1,

        /// <summary>
        /// 女
        /// </summary>
        [Description("女")]
        Female = 0
    }

    public enum AdjustPointOperationType
    {
        /// <summary>
        /// 添加积分，扣减积分操作
        /// </summary>
        AddOrReduce = 0,
        /// <summary>
        /// 作废SO,撤销积分使用
        /// </summary>
        Abandon = 1
    }

    /// <summary>
    /// 调积分类型
    /// </summary>
    public enum AdjustPointType
    {
        /// <summary>
        /// 生成订单
        /// </summary>
        CreateOrder = 3,

        /// <summary>
        /// 作废订单
        /// </summary>
        AbandonSO = 5,

        /// <summary>
        /// 订单修改
        /// </summary>
        UpdateSO = 12,

        /// <summary>
        /// 商品评论送积分
        /// </summary>
        Remark = 16,

        /// <summary>
        /// 系统帐号增加积分
        /// </summary>
        AddPointToSysAccounts = 20,

        /// <summary>
        /// 促销活动送积分
        /// </summary>
        SalesGivePoints = 25,

        /// <summary>
        /// 退货产生积分收支额
        /// </summary>
        ReturnProductPoint = 34,

        /// <summary>
        /// 销售折扣与折让
        /// </summary>
        SalesDiscountPoint = 35,

        /// <summary>
        /// 客户多付款－产品调价
        /// </summary>
        ProductPriceAdjustPoint = 37,

        /// <summary>
        /// RMA邮资手动转积分
        /// </summary>
        RMAPostageManuToPoints = 42,

        /// <summary>
        /// 退货现金转积分
        /// </summary>
        RefundCashToPoints = 44
    }

    public enum PrepayType
    {
        /// <summary>
        /// 订单相关 生成订单时使用或者Void订单时退还
        /// </summary>
        SOPay = 1,
        /// <summary>
        /// RO退款
        /// </summary>
        ROReturn = 2,
        /// <summary>
        /// AO退款
        /// </summary>
        BOReturn = 3,
        /// <summary>
        /// RO Balance
        /// </summary>
        RO_BalanceReturn = 4,
        /// <summary>
        /// 多汇款退款
        /// </summary>
        RemitReturn = 5,
        /// <summary>
        /// 余额转银行账户
        /// </summary>
        BalanceReturn = 6
        ///// <summary>
        ///// 补偿退款单
        ///// </summary>
        //RO_AdjustReturn = 7
    }

    public enum CustomerType
    {
        /// <summary>
        /// 个人客户
        /// </summary>
        [Description("个人客户")]
        Personal = 0,

        /// <summary>
        /// 企业客户
        /// </summary>
        [Description("企业客户")]
        Enterprise = 1,

        /// <summary>
        /// 校园客户
        /// </summary>
        [Description("校园客户")]
        Campus = 2,

        /// <summary>
        /// 媒客体户
        /// </summary>
        [Description("媒客体户")]
        Media = 3,

        /// <summary>
        /// 内部用户
        /// </summary>
        [Description("内部用户")]
        Internal = 4
    }

    /// <summary>
    /// 顾客经验值变更原因的类型,（订单送经验值这种情况是没有记录日志的）
    /// </summary>
    public enum ExperienceLogType
    {
        /// <summary>
        /// 引荐的新顾客第一笔订单成功后为引荐人增加经验值
        /// </summary>
        Recommend = 1,

        /// <summary>
        /// 商家订单出库，区别于普通订单
        /// </summary>
        MerchantSOOutbound = 4,
        /// <summary>
        /// 手工添加顾客经验值
        /// </summary>
        ManualSetTotalSOMoney = 2,
        /// <summary>
        /// 参加活动增加
        /// </summary>
        Promotion = 3,
        /// <summary>
        /// 物流拒收
        /// </summary>
        LogisticsRejected = 9

    }
}
