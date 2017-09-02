using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ECommerce.Enums
{
    public enum ECustomerGender
    {
        [Description("男")]
        Man = 1,

        [Description("女")]
        Woman = 0
    }

    /// <summary>
    /// 商品和顾客对应的公司类型
    /// </summary>
    public enum CompanyType
    {
        Newegg = 0,

        AZ = 1
    }

    public enum CustomerStatusType
    {
        Valid = 0,

        InValid = -1
    }

    public enum TokenType
    {
        [Description("TokenTypeE")]
        E,

        [Description("TokenTypeP")]
        P
    }


    /// <summary>
    /// 用户等级枚举
    /// </summary>
    public enum CustomerRankType
    {
        [Description("游客")]
        Unknown = 0,

        [Description("一钻会员")]
        JuniorMember = 1,

        [Description("二钻会员")]
        BronzeMember = 2,

        [Description("三钻会员")]
        SilverMember = 3,

        [Description("四钻会员")]
        GoldMember = 4,

        [Description("五钻会员")]
        DiamondMember = 5,

        [Description("六钻会员")]
        CrownMember = 6,

        [Description("七钻会员")]
        EggEmperor = 7
    }
    public enum CustomerSourceType
    {
        [Description("None")]
        None = 0,

        [Description("支付宝")]
        AliPay = 1,

        [Description("支付宝")]
        TenPay = 2,

        [Description("支付宝")]
        NeweggAlly = 3,

        [Description("支付宝")]
        PingAn = 4,

        [Description("关爱通")]
        [Obsolete("BaseDel_ZhongZhi", true)]
        ZhongZhi = 5,

        [Description("支付宝")]
        NeweggSelf = 6,

        [Description("支付宝")]
        Kds = 7,

        [Description("新浪微博")]
        Sina = 13,

        [Description("盛大通行证")]
        Sdo = 17,

        [Description("跨境通")]
        Kuajingtong = 40,

        [Description("腾讯QQ")]
        TencentQQ = 41,

        [Description("泰隆银行")]
        TLYH=99
    }
    #region 余额相关+ 积分
    public enum PrepayLogType
    {
        All = 0,
        [Description("订单支付")]
        SOPay = 1,
        ROReturn = 2,
        [Description("订单作废")]
        AOReturn = 3,
        RO_BalanceReturn = 4,
        RemitReturn = 5,
        ReturnFromPrepay = 6,

        /// <summary>
        /// 后台充值
        /// </summary>
        SupplementPrepay = 7,

        /// <summary>
        /// 作废订单
        /// </summary>
        CancellationOrderReturn = 8
    }

    public enum PrepayQueryTimeType
    {
        All = 0,
        LatestWeek = 1,
    }

    public enum PointType
    {
        [Description("积分作废")]
        DisusePoint = -1,
        [Description("历史积分")]
        历史积分 = 0,
        [Description("Email确认")]
        Recruit = 1,
        [Description("老客户回馈")]
        Veteran = 2,
        [Description("生成订单")]
        CreateOrder = 3,
        [Description("作废订单")]
        AbandonSO = 5,
        [Description("作废订单取消")]
        CancelAbandonSO = 6,
        [Description("退货")]
        ReturnProduct = 7,
        [Description("取消退货")]
        CancelReturn = 8,
        [Description("取消出库")]
        CancelOutstock = 9,
        [Description("积分转移")]
        TransferPoint = 10,   //积分转移
        [Description("购物得分")]
        AddPointLater = 11,
        [Description("订单修改")]
        UpdateSO = 12,
        [Description("批发扣除")]
        WholeSale = 13,
        [Description("买卡")]
        InfoProduct = 14,
        [Description("其他")]
        BizRequest = 15,
        [Description("订单评论")]
        Remark = 16,
        [Description("注册送积分")]
        NewRegister = 17,
        [Description("DIY活动积分增减")]
        DIY = 18,
        [Description("系统转移积分")]
        SysTransferPoint = 19,
        [Description("系统帐号增加积分")]
        AddPointToSysAccounts = 20,
        [Description("参加竞猜")]
        BetReductPoint = 21,
        [Description("竞猜所得")]
        BetAddPoint = 22,
        [Description("新用户第一次购物赠送积分")]
        NewCustomerFirstBuy = 23,
        [Description("自动提升精华赠送积分")]
        SetScoreAuto = 24,
        [Description("市场促销活动增送积分")]
        促销活动送积分 = 25,
        [Description("运费促销活动增减积分")]
        ShippingPromotionCampaign = 26,
        [Description("支付宝直接支付送积分")]
        DirectAlipay = 27,
        [Description("优惠券5.8元兑换25个新蛋积分")]
        PromotionCodeConvertToPoint = 28,
        [Description("理光转积分")]
        RicohTransferPoint = 29,
        [Description("黄金会员以上RMA收货送积分")]
        RMAGivePoint = 30,
        [Description("黄金会员以上RMA取消收货回收积分")]
        RMACancelPoint = 31,
        [Description("作废订单加积分")]
        AddPointAbandonSO = 32,
        [Description("取消作废订单加积分")]
        CancleAddPointAbandonSO = 33,
        [Description("退货产生积分收支额")]
        ReturnProductPoint = 34,
        [Description("商品购买")]
        SalesDiscountPoint = 35,
        [Description("补偿性积分")]
        CompensatePoint = 36,
        [Description("客户多付款－产品调价")]
        ProductPriceAdjustPoint = 37,
        [Description("VIP客户送积分")]
        VIPCustomerPoint = 38,
        [Description("多汇款转积分")]
        多汇款转积分 = 39,
        [Description("AO单转积分")]
        AO单转积分 = 40,
        [Description("RMA邮资转积分(系统自动转积分)")]
        RMA邮资系统自动转积分 = 41,
        [Description("RMA邮资转积分(手动转积分)")]
        RMA邮资手动转积分 = 42,
        [Description("礼品卡充积分")]
        礼品卡充积分 = 43,
        [Description("退货现金转积分")]
        退货现金转积分 = 44,
        [Description("用于出售的积分")]
        用于出售的积分 = 45,
        [Description("公关礼券")]
        公关礼券 = 46,
        [Description("员工福利")]
        员工福利 = 47,
        [Description("元宵活动积分兑换")]
        元宵活动 = 48,
        [Description("盛大积分兑换")]
        盛大积分兑换 = 49,
        [Description("校园推广送积分")]
        AmbassadorAgentSOPoint = 50,
        [Description("校园代理佣金")]
        AmbassadorRecommendSOPoint = 51,
        [Description("泰隆优选商家评论积分")]
        泰隆优选商家评论积分 = 61,
        [Description("泰隆优选商家折扣积分")]
        泰隆优选商家折扣积分 = 62,
        [Description("MKT-Promotion")]
        MKT_Promotion = 63,
        [Description("MKT-alipay")]
        MKT_alipay = 64,
        [Description("团购抽奖消费")]
        GroupBuyingLotteryPoint = 65,
        [Description("手机验证")]
        MobileVerification = 80,
        [Description("邮箱验证")]
        EmailVerification = 81
    }
    #endregion

    public enum NotifyStatus
    {
        [Description("作废")]
        Abandon = -1,
        [Description("未通知")]
        Unnotify = 0,
        [Description("已通知")]
        Notified = 1
    }

    /// <summary>
    /// 证件类型
    /// </summary>
    public enum IDCardType
    {
        /// <summary>
        /// 身份证
        /// </summary>
        [Description("身份证")]
        IdentityCard = 0,
        /// <summary>
        /// 护照
        /// </summary>
        [Description("护照")]
        Passport
    }

    /// <summary>
    /// 加密模式
    /// </summary>
    public enum EncryptMode
    {
        /// <summary>
        /// SHA1,并使用Salt，该方法为默认的加密方式
        /// </summary>
        [Description("SHA1加密")]
        SHA1 = 0,
        /// <summary>
        /// MD5加密算法，为了兼容KJT外部系统导入的密码
        /// </summary>
        [Description("MD5加密")]
        MD5 = 1
    }
}
