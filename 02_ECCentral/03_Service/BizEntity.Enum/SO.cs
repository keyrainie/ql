using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ECCentral.BizEntity.SO
{
    /// <summary>
    /// 订单类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResSOEnum")]
    public enum SOType
    {
        /// <summary>
        /// 0:表示普通订单
        /// </summary>
        General = 0,
        ///// <summary>
        ///// 1:表示以旧换新订单;
        ///// </summary>
        //[Obsolete("此字段已弃用", true)]
        //OldChangeNew = 1,
        ///// <summary>
        ///// 2:表示分期订单; 
        ///// </summary>
        //[Obsolete("此字段已弃用",true)]
        //Instalment = 2,
        /// <summary>
        /// 3:表示赠品订单，目前由后台使用
        /// </summary>
        [Obsolete("此字段已弃用")]
        Gift = 3,
        /// <summary>
        /// 4:电子卡
        /// </summary>
        [Obsolete("此字段已弃用")]
        ElectronicCard = 4,
        /// <summary>
        /// 5:礼品券订单
        /// </summary>
        PhysicalCard = 5,
        ///// <summary>
        ///// 6:表示SIM卡订单
        ///// </summary>
        //[Obsolete("此字段已弃用")]
        //SIM = 6,
        /// <summary>
        /// 7:团购订单
        /// </summary>
        GroupBuy = 7,
        ///// <summary>
        ///// 8:合约机
        ///// </summary>
        //[Obsolete("此字段已弃用")]
        //ContractPhone = 8,
        ///// <summary>
        ///// 9:联通0元购机订单
        ///// </summary>
        //[Obsolete("此字段已弃用")]
        //UnicomFreeBuy = 9,
        ///// <summary>
        ///// 10:购机结算订单
        ///// </summary>
        //[Obsolete("此字段已弃用")]
        //BuyMobileSettlement = 10,   

        [Obsolete("此字段已弃用")]
        GiftVolumes = 12,

        /// <summary>
        /// 虚拟团购
        /// </summary>
        VirualGroupBuy = 71
    }

    /// <summary>
    /// 订单状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResSOEnum")]
    public enum SOStatus
    {
        /// <summary>
        /// 已拆分,已将此订单拆分为了多个子订单
        /// </summary>
        [Obsolete("此字段已弃用")]
        Split = -6,
        /// <summary>
        /// BackOrder
        /// 主要是因为前后台库存不同步,产生的并发异常单.即前台认为有库存,下单成功了，但到后台发现实际已经没有库存了,就会标记为BackOrder.
        /// </summary>
        // BackOrder = -5,
        /// <summary>
        /// 系统自动作废
        /// </summary>
        SystemCancel = -4,
        /// <summary>
        /// 已作废
        /// </summary>
        Abandon = -1,
        /// <summary>
        /// 待审核
        /// </summary>
        Origin = 0,
        /// <summary>
        /// 待出库
        /// </summary>
        WaitingOutStock = 1,
        /// <summary>
        /// 待海关申报
        /// </summary>
        [Obsolete("该状态已过时")]
        WaitingPay = 2,

        /// <summary>
        /// 移仓在途
        /// </summary>
        [Obsolete("此字段已弃用")]
        Shipping = 10,
        /// <summary>
        /// 待核收
        /// </summary>
        //  WaitingPay = 2,//去掉此状态
        /// <summary>
        /// 待主管审
        /// </summary>
        [Obsolete("此字段已弃用")]
        WaitingManagerAudit = 3,
        /// <summary>
        /// 已出库待申报
        /// </summary>
        OutStock = 4,

        /// <summary>
        /// 海关申报
        /// </summary>
        Waiting = 40,

        /// <summary>
        /// 已申报待通关
        /// </summary>
        Reported = 41,

        /// <summary>
        /// 已通关发往顾客
        /// </summary>
        [Obsolete("此字段已弃用")]
        CustomsPass = 45,

        /// <summary>
        /// 订单完成
        /// </summary>
        Complete = 5,
        /// <summary>
        /// 申报失败订单作废
        /// </summary>
        [Obsolete("此字段已弃用")]
        Reject = 6,

        /// <summary>
        /// 通关失败订单作废
        /// </summary>
        [Obsolete("此字段已弃用")]
        CustomsReject = 65,

        /// <summary>
        /// 订单拒收
        /// </summary>
        ShippingReject = 7
    }

    /// <summary>
    /// 订单商品类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResSOEnum")]
    public enum SOProductType
    {
        /// <summary>
        /// 正常商品,IPP3:ForSale
        /// </summary>
        Product = 0,
        /// <summary>
        /// 赠品
        /// </summary>
        Gift = 1,
        /// <summary>
        /// 奖品,IPP3:CustomerGift
        /// </summary>
        Award = 2,
        /// <summary>
        /// 优惠券
        /// </summary>
        Coupon = 3,
        /// <summary>
        /// 延保
        /// </summary>
        ExtendWarranty = 4,
        /// <summary>
        /// 附件,配件
        /// </summary>
        Accessory = 5,
        /// <summary>
        /// 礼品, IPP3:NewEggGift
        /// </summary>
        SelfGift = 6
    }

    /// <summary>
    /// 订单中使用的促销类型,请不要和MKT中的促销类型搞混了,两个有一样的地方,但不完全一样.
    /// </summary>
    public enum SOPromotionType
    {
        /// <summary>
        /// 组合销售
        /// </summary>
        Combo = 0,
        /// <summary>
        /// 优惠券
        /// </summary>
        Coupon = 1,
        /// <summary>
        /// 附件
        /// </summary>
        Accessory = 2,
        /// <summary>
        /// 厂商赠品
        /// </summary>
        VendorGift = 3,
        /// <summary>
        /// 赠品规则
        /// </summary>
        SelfGift = 4,
        /// <summary>
        /// 销售立减
        /// </summary>
        SaleDiscountRule=5
    }

    /// <summary>
    /// 订单商品活动类型
    /// </summary>
    public enum SOProductActivityType
    {   //团购
        GroupBuy,
        //关爱 支付宝 账户支付
        SpecialAccount,
        //节能惠民
        E_Promotion

    }

    /// <summary>
    /// 发票修改类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResSOEnum")]
    public enum InvoiceChangeType
    {
        /// <summary>
        /// 普票改普票
        /// </summary>
        GeneralChange,
        /// <summary>
        /// 普票改增票
        /// </summary>
        GeneralToVAT,
        /// <summary>
        /// 增票改普票
        /// </summary>
        VATToGeneral,
        /// <summary>
        /// 修改增票信息
        /// </summary>
        VATChange
    }

    /// <summary>
    /// 移动端类型
    /// </summary>
    public enum PhoneType
    {
        /// <summary>
        /// 非移动设备
        /// </summary>
        None = 0,
        /// <summary>
        /// 电话系统链接过来创建的订单
        /// </summary>
        Phone = 1,
        /// <summary>
        /// 微信下单
        /// </summary>
        Wechat = 2,
        /// <summary>
        /// IPhone 设备下单
        /// </summary>
        IPhone = 3,
        /// <summary>
        /// Android 设备下单
        /// </summary>
        Android = 4

    }

    /// <summary>
    /// 订单配送承诺
    /// </summary>
    public enum SODeliveryPromise
    {
        /// <summary>
        /// 不承诺
        /// </summary>
        NoPromise = 0,
        /// <summary>
        /// 24小时内配送
        /// </summary>
        In24H = 1
    }

    /*
    /// <summary>
    /// 订单配送频率
    /// </summary>
    public enum SODeliveryFrequency
    {
        /// <summary>
        /// 一日一送
        /// </summary>
        OneInDay = 1,
        /// <summary>
        /// 一日两送
        /// </summary>
        TwoInDay = 2,
        /// <summary>
        /// 工作日送
        /// </summary>
        WorkDays = 3,
        /// <summary>
        /// 一日六送
        /// </summary>
        SixInDays,
    }
    //*/

    /// <summary>
    /// 订单锁定状态
    /// </summary>
    public enum SOHoldStatus
    {
        /// <summary>
        /// 表示自动解锁单；
        /// </summary>
        //AutoUnhold = -1, //建议由Unhold状态代替，统一由一个状态表示
        /// <summary>
        /// 表示解锁状态；(表示订单操作状态正常)
        /// </summary>
        Unhold = 0,
        /// <summary>
        /// 表示后台锁单；
        /// </summary>
        BackHold = 1,
        /// <summary>
        /// 表示前台锁单；(后台系统不可以更改此单)
        /// </summary>
        WebHold = 2,
        /// <summary>
        /// 表示解锁中；(即正在处理中的状态)
        /// </summary>
        Processing = 3,
        /// <summary>
        /// 订单未正式生成,但订单已被取消
        /// </summary>
        //CancelNoOrder = 4  //建议不要此状态，由订单状态可以标识
        /// <summary>
        /// 订单已生成,但订单已被取消
        /// </summary>
        //Cancel = 5, //建议不要此状态，由订单状态可以标识
    }

    /// <summary>
    /// 订单拆分类型
    /// </summary>
    public enum SOSplitType
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 系统强制拆单
        /// </summary>
        Force = 1,
        /// <summary>
        /// 客户选择拆单
        /// </summary>
        Customer = 2,
        /// <summary>
        /// 被拆分子订单
        /// </summary>
        SubSO = 3
    }

    #region Complain

    /// <summary>
    /// 订单投诉状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResSOEnum")]
    public enum SOComplainStatus
    {
        /// <summary>
        /// 已作废
        /// </summary>
        Abandoned = -1,
        /// <summary>
        /// 待处理
        /// </summary>
        Origin = 0,
        /// <summary>
        /// 处理中
        /// </summary>
        Dealing = 1,
        /// <summary>
        /// 处理完毕
        /// </summary>
        Complete = 2,
        /// <summary>
        /// 升级投诉
        /// </summary>
        Upgrade = 3,
        /// <summary>
        /// 复核处理
        /// </summary>
        Review = 4,
        /// <summary>
        /// 已达成处理意见
        /// </summary>
        Agreement = 5
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResSOEnum")]
    public enum OutdatedType
    {
        /// <summary>
        /// 超过2个工作小时
        /// </summary>
        MoreTwoHours = 0,
        /// <summary>
        /// 超过1个工作日
        /// </summary>
        MoreOneDay = 1,
        /// <summary>
        /// 超过3个工作日
        /// </summary>
        MoreThreeDays = 2
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResSOEnum")]
    public enum SOInternalMemoStatus
    {
        /// <summary>
        /// 需要跟进
        /// </summary>
        FollowUp = 1,
        /// <summary>
        /// 处理完成
        /// </summary>
        Complete = 0
    }

    /// <summary>
    /// 投诉回复类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResSOEnum")]
    public enum SOComplainReplyType
    {
        Phone = 0,
        Email = 1,
        Other = 3
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResSOEnum")]
    public enum CSSOComplainType
    {
        [Description("产品类投诉")]
        Product,
        [Description("仓储类投诉")]
        Stock,
        [Description("配送类投诉")]
        Ship,
        [Description("客服类投诉")]
        CS,
        [Description("售后类投诉")]
        AS,
        [Description("市场类投诉")]
        MKT,
        [Description("财务类投诉")]
        FIN,
        [Description("系统类投诉")]
        SYS,
        [Description("综合类投诉")]
        INT,
        [Description("一般咨询")]
        NOR,
        [Description("其他类投诉")]
        OTHDPT
    }

    #endregion

    [Description("ECCentral.BizEntity.Enum.Resources.ResSOEnum")]
    public enum SOPendingStatus
    {
        /// <summary>
        /// 已改单
        /// </summary>
        ChangeOrder = -1,
        /// <summary>
        /// 未处理
        /// </summary>
        Origin = 0,
        /// <summary>
        /// 已处理 
        /// </summary>
        Complete = 1
    }

    /// <summary>
    /// 结算状态
    /// </summary>
    public enum SettlementStatus
    {
        /// <summary>
        /// 团购失败
        /// </summary>
        PlanFail = -1, //P
        /// <summary>
        /// 处理成功
        /// </summary>
        Success = 0,//S
        /// <summary>
        /// 处理失败
        /// </summary>
        Fail = 1, //F
    }

    /// <summary>
    /// 订单操作者类型
    /// </summary>
    public enum SOOperatorType
    {
        /// <summary>
        /// 客户操作
        /// </summary>
        Customer,
        /// <summary>
        /// 客服人员
        /// </summary>
        User,
        /// <summary>
        /// 系统
        /// </summary>
        System
    }

    /// <summary>
    /// 配送状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResSOEnum")]
    public enum DeliveryStatus
    {
        /// <summary>
        /// 已送达
        /// </summary>
        OK = 0,
        /// <summary>
        /// 改派
        /// </summary>
        Reassign = 1,
        /// <summary>
        /// 取消
        /// </summary>
        Abandon = 2,
        /// <summary>
        /// 未送达
        /// </summary>
        Failure = 3,
        /// <summary>
        /// 未送货
        /// </summary>
        NoAction = 4,
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResSOEnum")]
    public enum DeliveryType
    {
        ALL = 0,
        /// <summary>
        /// SO送货
        /// </summary>
        SO = 1,
        /// <summary>
        /// RMA所有单据
        /// </summary>
        RMAALL = 2,
        /// <summary>
        /// RMA上门取件
        /// </summary>
        RMARequest = 3,
        /// <summary>
        /// Vendor送修
        /// </summary>
        VendorMend = 4,
        /// <summary>
        /// Vendor送修返还
        /// </summary>
        VendorReturn = 5,
        /// <summary>
        /// RMA发货
        /// </summary>
        RMARevert = 6,
    }

    public enum AllocateInventoryStatus
    {
        [Description("Failed")]
        Failed = -1,
        [Description("Success")]
        Success = 0,
        [Description("SuccessAndUpdated")]
        SuccessAndUpdated = 1

    }

    /// <summary>
    /// 订单价格信息状态
    /// </summary>
    public enum SOPriceStatus
    {
        /// <summary>
        /// 原始的
        /// </summary>
        Original,
        /// <summary>
        /// 无效的
        /// </summary>
        Deactivate
    }

    public enum SOExtendWarrantyStatus
    {
        /// <summary>
        /// 作废
        /// </summary>
        Abandon = -1,
        /// <summary>
        /// 有效的
        /// </summary>
        Origin = 0
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

    /// <summary>
    /// SIM卡状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResSOEnum")]
    [Obsolete("此类型已弃用",true)]
    public enum SIMStatus
    {
        /// <summary>
        /// 待激活
        /// </summary>
        Original,
        /// <summary>
        /// 已激活
        /// </summary>
        Activated,
        /// <summary>
        /// 取消激活
        /// </summary>
        CancelActivate,
        /// <summary>
        /// 激活失败
        /// </summary>
        Fail
    }

    /// <summary>
    /// 证件类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResSOEnum")]
    public enum CertificateType
    {
        /// <summary>
        /// 身份证
        /// </summary>
        IdentityCard = 0,
        /// <summary>
        /// 军官证
        /// </summary>
        OfficerCard = 1,
        /// <summary>
        /// 士兵证
        /// </summary>
        SoldierCard = 2,
        /// <summary>
        /// 组织机构代码
        /// </summary>
        OrganizationCode = 3
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResSOEnum")]
    public enum ValidStatus
    {
        /// <summary>
        /// 有效
        /// </summary>
        Active,
        /// <summary>
        /// 无效
        /// </summary>
        DeActive
    }


    [Description("ECCentral.BizEntity.Enum.Resources.ResSOEnum")]
    public enum ItemGrossProfitActionType
    {
        /// <summary>
        /// 创建订单
        /// </summary>
        CreateSO,
        /// <summary>
        /// 修改订单
        /// </summary>
        ChangeSO
    }
    public enum GrossProfitType
    {
        Normal = 1
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResSOEnum")]
    public enum WMSAction
    {
        /// <summary>
        /// 锁定
        /// </summary>
        Hold = 'H',
        /// <summary>
        /// 取消审核订单时锁定
        /// </summary>
        CancelAuditHold = 'C',
        /// <summary>
        /// 作废订单时锁定
        /// </summary>
        AbandonHold = 'D',
        /// <summary>
        /// 解锁
        /// </summary>
        UnHold = 'U',
        /// <summary>
        /// 作废
        /// </summary>
        Abandon = 'V'
    }

    /// <summary>
    /// 回调类型
    /// </summary>
    public enum OPCCallBackType
    {
        /// <summary>
        /// 锁定订单回调
        /// </summary>
        HoldCallBack = 0,

        /// <summary>
        /// 取消审核订单回调
        /// </summary>
        CancelAuditCallBack = 1,

        /// <summary>
        /// 作废订单回调
        /// </summary>
        AbandonCallBack = 2,

        /// <summary>
        /// 创建AO并作废订单回调
        /// </summary>
        AOAbandonCallBack = 3,

        /// <summary>
        /// 超范围Pending订单回调
        /// </summary>
        PendingSO = 4,

        /// <summary>
        /// 未实现
        /// </summary>
        NoneCallBack
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResSOEnum")]
    public enum OPCStatus
    {
        /// <summary>
        /// 打开
        /// </summary>
        Open = 'O',
        /// <summary>
        /// 关闭
        /// </summary>
        Close = 'C',
        /// <summary>
        /// 失败
        /// </summary>
        Fail = 'F',
        /// <summary>
        /// 错误
        /// </summary>
        Error = 'E'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResSOEnum")]
    public enum OPCTransStatus : int
    {
        [Description("未处理")]
        Origin = 'O',
        [Description("成功")]
        Success = 'S',
        [Description("失败")]
        Fail = 'F',
        [Description("错误")]
        Error = 'E'
    }
    /*//
    /// <summary>
    /// 配送状态 ,
    /// </summary>
    public enum ReceivingStatus
    {
        /// <summary>
        /// 已出库待发货
        /// </summary>
        Waiting,//WAT
        /// <summary>
        /// 发往泰隆优选仓库
        /// </summary>
        Send,//SED
        /// <summary>
        /// 已送达
        /// </summary>
        Arrived,// ARE,
        /// <summary>
        /// 仓库拒收
        /// </summary>
        Reject,//DEC
    }
    //*/

    /// <summary>
    /// 是否特殊订单
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResSOEnum")]
    public enum SOIsSpecialOrder
    {
        //普通订单
        Normal,
        //全免费的ACER赠品
        ACER,
        //阿斯利康订单
        Astrazeneca,
        //理光用户订单
        LGOrder
    }

    /// <summary>
    /// 是否当前数据
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResSOEnum")]
    public enum SOIsCurrentData
    {
        //当前数据
        Current,
        //所有数据
        All
    }

    /// <summary>
    /// 是否有货
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResSOEnum")]
    public enum EnoughFlag
    {
        //有库存
        InStock = 0,
        //待采购
        WaitForPurchase = 1,
        //待移仓
        WaitForMoveWarehouse = 2,
    }

    /// <summary>
    /// 送货时段
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResSOEnum")]
    public enum DeliveryTimeRange
    {
        [Description("未指定")]
        UnSpecified = 0,
        [Description("上午")]
        AM = 1,
        [Description("下午")]
        PM = 2
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResSOEnum")]
    public enum PackageSignStatus
    {
        //数据异常
        PackageDataodd = 0,
        //签收异常
        PackageSignodd = 1,
        //待签收
        PackageWaitSign = 2,
        //已签收
        PackageAddreadySign = 3,
        //次日签收
        PackageToDaySign = 4,
        //二日内签收
        PackageTwoDaySign = 5,
        //三日内签收
        PackageThreeDaySign = 6,
        //四日内签收
        PackageFourDaySign = 7
    }

    /// <summary>
    /// 欺诈订单类型
    /// </summary>
    public enum FPSOType
    { 
        KeYi = 1,
        ChuanHuo = 2,
        ChaoHuo = 3
    }

    /// <summary>
    /// 可疑单状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResSOEnum")]
    public enum FPStatus
    {
        /// <summary>
        /// 已验证
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 可疑
        /// </summary>
        Suspect = 1,
        /// <summary>
        /// 串货
        /// </summary>
        ChuanHuo = 2,
        /// <summary>
        /// 炒货
        /// </summary>
        ChaoHuo = 3
    }

    /// <summary>
    /// KFC类型
    /// </summary>
    public enum KFCType
    {
        Normal = 0,
        KeYi = 1,
        QiZha = 2
    }

    [Obsolete("此类型已弃用",true)]
    public enum InstalmentStatus
    {
        /// <summary>
        /// 客户录入
        /// </summary>
        Customer = 'W',
        /// <summary>
        /// CS录入
        /// </summary>
        CS = 'C',
        /// <summary>
        /// 未录入
        /// </summary>
        None = 'N',
        /// <summary>
        /// 网银
        /// </summary>
        Bank = 'B'
    }

    public enum ExpressType
    {
        /// <summary>
        /// 顺丰
        /// </summary>
        SF = 1,
        /// <summary>
        /// 圆通
        /// </summary>
        YT = 2,
        /// <summary>
        /// 快递100
        /// </summary>
        KD100 = 100
    }
}
