using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ECCentral.BizEntity.MKT
{
    #region MKT内部可公用枚举
    /// <summary>
    /// 有效，无效公用枚举
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum ADStatus
    {
        /// <summary>
        /// 无效
        /// </summary>
        Deactive = 0,
        /// <summary>
        /// 有效
        /// </summary>
        Active = 1
    }

    /// <summary>
    /// 有效，无效，测试公用枚举    【关键字用到】
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum ADTStatus
    {
        /// <summary>
        /// 无效
        /// </summary>
        Deactive,
        /// <summary>
        /// 有效
        /// </summary>
        Active,
        /// <summary>
        /// 测试
        /// </summary>
        Test
    }

    /// <summary>
    /// 有效，无效,锁定 公用枚举  礼品卡使用到
    /// </summary>
    //[Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    //public enum GiftCardStatus
    //{
    //    /// <summary>
    //    /// 无效
    //    /// </summary>
    //    Deactive,
    //    /// <summary>
    //    /// 有效
    //    /// </summary>
    //    Active,
    //    /// <summary>
    //    /// 锁定 Y
    //    /// </summary>
    //    Lock
    //}

    /// <summary>
    /// 状态显示[所有-显示-不显示]
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum YNStatus
    {
        No = 0,
        Yes = 1
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum YesOrNoBoolean
    {
        No = 0,
        Yes = 1
    }

    /// <summary>
    /// 是或否，对应1和0
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum NYNStatus
    {
        No,
        Yes
    }
    #endregion

    #region Promotion 相关枚举
    /// <summary>
    /// Promotion类型
    /// </summary>
    public enum PromotionType
    {
        /// <summary>
        /// 组合销售
        /// </summary>
        [Description("组合销售")]
        Combo,
        /// <summary>
        /// 优惠券
        /// </summary>
        [Description("优惠券")]
        Coupons,
        /// <summary>
        /// 赠品
        /// </summary>
        [Description("赠品")]
        SaleGift,
        /// <summary>
        /// 限时销售
        /// </summary>
        [Description("限时促销")]
        Countdown,
        /// <summary>
        /// 团购
        /// </summary>
        [Description("团购")]
        GroupBuying,
        /// <summary>
        /// 随心配
        /// </summary>
        [Description("随心配")]
        OptionalAccessories,
        /// <summary>
        /// 销售立减
        /// </summary>
        [Description("销售立减")]
        SaleDiscountRule
    }

    /// <summary>
    /// 关联关系
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum PSRelationType
    {
        /// <summary>
        /// 包含
        /// </summary>
        Include,
        /// <summary>
        /// 排除
        /// </summary>
        Exclude
    }


    /// <summary>
    /// 用户范围限定类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum PSCustomerConditionType
    {
        /// <summary>
        /// 不限
        /// </summary>
        AllCustomer = 0,
        /// <summary>
        /// 指定用户组
        /// </summary>
        CustomerGroup = 1,
        /// <summary>
        /// 自选用户
        /// </summary>
        Customer = 2
    }

    /// <summary>
    /// 订单金额折扣类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum PSDiscountTypeForOrderAmount
    {
        /// <summary>
        /// 订单折扣百分比
        /// </summary>
        OrderAmountPercentage,
        /// <summary>
        /// 订单折扣金额
        /// </summary>
        OrderAmountDiscount
    }

    /// <summary>
    /// 商品价格折扣类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum PSDiscountTypeForProductPrice
    {
        /// <summary>
        /// 单个商品直减金额
        /// </summary>
        ProductPriceDiscount,
        /// <summary>
        /// 单个商品最终售价
        /// </summary>
        ProductPriceFinal
    }

    /// <summary>
    /// 操作类型
    /// </summary>
    public enum PSOperationType
    {
        View,
        Create,
        Edit,
        SubmitAudit,
        CancelAudit,
        AuditApprove,
        AuditRefuse,
        Stop,
        Void
    }

    #region 限时促销

    /// <summary>
    /// 限时促销活动状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum CountdownStatus
    {
        //作废
        Abandon = -1,
        /// <summary>
        /// 待初级审核
        /// </summary>
        WaitForPrimaryVerify = -6,
        /// <summary>
        /// 待高级审核
        /// </summary>
        [Obsolete("此字段已弃用")]
        WaitForVerify = -3,
        //审核未通过
        VerifyFaild = -4,
        //"就绪
        Ready = 0,
        //运行
        Running = 1,
        //完成
        Finish = 2,
        //中止
        Interupt = -2,
        ///"初始" 
        Init = -5
    }

    #endregion

    #region 优惠券
    /// <summary>
    /// 优惠券类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum CouponsMKTType
    {
        MKTInternal,
        MKTEmail,
        MKTNetworkAlliance,
        MKTAFC,
        MKTAmbassador,
        MKTBD,
        MKTOnline,
        MKTOffline,
        MKTPM,
        MKTCS,
        MKTPlace
        //<option value="N">MKT-内网营销优惠券</option>
        //<option value="M">MKT-邮件营销优惠券</option>
        //<option value="W">MKT-网络联盟优惠券</option>
        //<option value="F">MKT-AFC优惠券</option>
        //<option value="A">MKT-泰隆优选大使优惠券</option>
        //<option value="B">MKT-BD合作优惠券</option>                                
        //<option value="O" selected="selected">MKT-线上推广优惠券</option>                                                                
        //<option value="L">MKT-线下推广优惠券</option>
        //<option value="P">PM-产品优惠券</option>
        //<option value="C">CS-补偿性优惠券</option>  
    }

    /// <summary>
    /// 优惠类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum CouponsRuleType
    {
        /// <summary>
        /// 商品折扣
        /// </summary>
        ProductDiscount

        ///// <summary>
        ///// 免运费
        ///// </summary>
        //FreeShipFee

    }



    /// <summary>
    /// 商品范围限定类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum CouponsProductRangeType
    {
        /// <summary>
        /// 所有商品
        /// </summary>
        AllProducts,
        /// <summary>
        /// 只限制类别和品牌:X
        /// </summary>
        LimitCategoryBrand,
        /// <summary>
        /// 限制商品:I
        /// </summary>
        LimitProduct
    }

    /// <summary>
    /// 商品范围限定类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum ProductRangeType
    {
        /// <summary>
        /// 所有商品
        /// </summary>
        All,
        /// <summary>
        /// 限定商品
        /// </summary>
        Limit
    }

    /// <summary>
    /// 触发条件
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum CouponsBindConditionType
    {
        None,
        Rigester,
        Birthday,
        //[Obsolete("此类型已废弃")]
        SO,
        Get,
        //Alipay
    }

    /// <summary>
    /// 优惠券有效期设置类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum CouponsValidPeriodType
    {
        All,
        PublishDayToOneWeek,
        PublishDayToOneMonth,
        PublishDayToTwoMonths,
        PublishDayToThreeMonths,
        PublishDayToSixMonths,
        CustomPeriod
    }


    /// <summary>
    /// 优惠活动状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum CouponsStatus
    {
        /// <summary>
        /// 运行
        /// </summary>
        Run,
        /// <summary>
        /// 完成
        /// </summary>
        Finish,
        /// <summary>
        /// 作废
        /// </summary>
        Void,
        /// <summary>
        /// 初始化
        /// </summary>
        Init,
        /// <summary>
        /// 待审核
        /// </summary>
        WaitingAudit,
        /// <summary>
        /// 就绪
        /// </summary>
        Ready,
        /// <summary>
        /// 中止
        /// </summary>
        Stoped

        //<option value="O">初始态</option>
        //<option value="W">待审核</option>
        //<option value="R">就绪</option>
        //<option value="A">运行</option>
        //<option value="D">作废</option>
        //<option value="F">完成</option>
    }

    /// <summary>
    /// 优惠券代码类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum CouponCodeType
    {
        /// <summary>
        /// 投放型优惠券
        /// </summary>
        ThrowIn,

        /// <summary>
        /// 通用型优惠券
        /// </summary>
        Common
    }

    /// <summary>
    /// 优惠使用状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum CouponCodeUsedStatus
    {
        /// <summary>
        /// 可用
        /// </summary>
        Active,
        /// <summary>
        /// 不可用
        /// </summary>
        Deactive
    }


    #endregion

    #region 团购相关
    /// <summary>
    /// 团购活动状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum GroupBuyingStatus
    {
        /// <summary>
        /// 初始
        /// </summary>
        Init,
        /// <summary>
        /// 待审核
        /// </summary>
        WaitingAudit,
        /// <summary>
        /// 审核未通过
        /// </summary>
        VerifyFaild,
        /// <summary>
        /// 就绪
        /// </summary>
        Pending,
        /// <summary>
        /// 待处理,用于商家商品
        /// </summary>
        WaitHandling,
        /// <summary>
        /// 运行
        /// </summary>
        Active,
        /// <summary>
        /// 作废
        /// </summary>
        Deactive,
        /// <summary>
        /// 完成
        /// </summary>
        Finished
    }

    /// <summary>
    /// 团购活动状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum GroupBuyingStatusForNeweggg
    {
        /// <summary>
        /// 待审核
        /// </summary>
        WaitingAudit = 'O',
        /// <summary>
        /// 就绪
        /// </summary>
        Pending = 'P',
        /// <summary>
        /// 运行
        /// </summary>
        Active = 'A',
        /// <summary>
        /// 作废
        /// </summary>
        Deactive = 'D',
        /// <summary>
        /// 完成
        /// </summary>
        Finished = 'F'
    }

    /// <summary>
    /// 处理状态
    /// </summary>
    public enum GroupBuyingSettlementStatus
    {
        /// <summary>
        /// 团购人数超出最大团购人数，未处理
        /// </summary>
        MoreThan, //C
        /// <summary>
        /// 未处理
        /// </summary>
        No, //N
        /// <summary>
        /// 已处理
        /// </summary>
        Yes //Y
    }
    #endregion

    #region 赠品相关
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum SaleGiftStatus
    {
        /// <summary>
        /// 初始化
        /// </summary>
        Init,
        /// <summary>
        /// 待审核
        /// </summary>
        WaitingAudit,
        /// <summary>
        /// 就绪
        /// </summary>
        Ready,
        /// <summary>
        /// 运行
        /// </summary>
        Run,
        /// <summary>
        /// 中止
        /// </summary>
        Stoped,
        /// <summary>
        /// 完成
        /// </summary>
        Finish,
        /// <summary>
        /// 作废
        /// </summary>
        Void

    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum SaleGiftStatusForNeweggg
    {
        /// <summary>
        /// 就绪
        /// </summary>
        Ready = 'R',
        /// <summary>
        /// 运行
        /// </summary>
        Run = 'A',
        /// <summary>
        /// 中止
        /// </summary>
        Stoped = 'S',
        /// <summary>
        /// 完成
        /// </summary>
        Finish = 'F',
        /// <summary>
        /// 作废
        /// </summary>
        Void = 'D'

    }

    /// <summary>
    /// 赠品类型:单品买赠,同时购买,买满即送,厂商赠品
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum SaleGiftType
    {
        /// <summary>
        /// 单品加购
        /// </summary>
        [Description("单品加购")]
        Single,
        /// <summary>
        /// 同时购买
        /// </summary>
        [Description("同时购买")]
        Multiple,
        /// <summary>
        /// 买满即购
        /// </summary>
        [Description("买满即购")]
        Full,
        /// <summary>
        /// 厂商赠品
        /// </summary>
        [Description("厂商赠品")]
        [Obsolete("此类型已废弃")]
        Vendor,
        ///// <summary>
        ///// 首次下单
        ///// </summary>
        //[Description("首次下单")]
        //FirstOrder,
        ///// <summary>
        ///// 满额加购
        ///// </summary>
        //[Description("满额加购")]
        //Additional
    }

    /// <summary>
    /// 折扣计入方式
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum SaleGiftDiscountBelongType
    {
        [Obsolete("此类型已废弃")]
        BelongGiftItem,
        BelongMasterItem
    }

    /// <summary>
    /// 商品范围规则：Item, Brand, C3, BrandC3Combo
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum SaleGiftSaleRuleType
    {
        Item,
        Brand,
        C3,
        BrandC3Combo
    }
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum SaleGiftGiftItemType
    {
         [Obsolete("此类型已废弃")]
        GiftPool,   //O 赠品池
        AssignGift  //A 非赠品池
    }

    /// <summary>
    /// 组合类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum SaleGiftCombineType
    {
        /// <summary>
        /// 交叉
        /// </summary>
        Cross,

        /// <summary>
        /// 组合
        /// </summary>
        Assemble
    }


    /// <summary>
    /// 赠品商家下log查询 
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum GiftIsOnlineShow
    {
        /// <summary>
        /// 上架
        /// </summary>
        Online = 'Y',
        /// <summary>
        /// 下架
        /// </summary>
        Deline = 'N'
    }
    #endregion

    #region 组合销售
    /// <summary>
    /// 组合销售状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum ComboStatus
    {
        //无效 -1,有效 0,待审核 1
        Active,
        Deactive,
        WaitingAudit
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum ComboType
    {
        /// <summary>
        /// 普通绑定
        /// </summary>
        Common,
        /// <summary>
        /// 加N元送X商品
        /// </summary>
        //NYuanSend
    }

    /// <summary>
    /// 是否显示
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum IsDefaultShow
    {
        /// <summary>
        /// 是
        /// </summary>
        Show = 'Y',
        /// <summary>
        /// 否
        /// </summary>
        Hide = 'N'
    }
    #endregion

    /// <summary>
    /// 审核类型
    /// </summary>
    public enum PromotionAuditType
    {
        Approve,
        Refuse
    }
    #endregion


    #region 新闻相关

    /// <summary>
    /// 公告及促销评论状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum NewsAdvReplyStatus
    {
        /// <summary>
        /// 展示
        /// </summary>
        Show = 0,
        /// <summary>
        /// 系统屏蔽
        /// </summary>
        SystemHide = -1,
        /// <summary>
        /// 手工屏蔽
        /// </summary>
        HandHide = -2
    }
    #endregion

    //<item code="R" name="所有"/>
    //<item code="O" name="待处理"/>
    //<item code="A" name="审核通过"/>
    //<item code="D" name="审核拒绝"/>


    /// <summary>
    /// 帮助主题标识类型 
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum FeatureType
    {
        /// <summary>
        /// 新
        /// </summary>
        New,
        /// <summary>
        /// 热
        /// </summary>
        Hot
    }

    /// <summary>
    /// Banner媒体类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum BannerType
    {
        Image,
        Flash,
        Text,
        HTML,
        Video
    }

    /// <summary>
    /// 用到PageType的页面定义
    /// </summary>
    public enum ModuleType
    {
        /// <summary>
        /// 广告
        /// </summary>
        Banner = 0,
        /// <summary>
        /// SEO管理
        /// </summary>
        SEO = 1,
        /// <summary>
        /// 商品推荐
        /// </summary>
        ProductRecommend = 2,
        /// <summary>
        /// 首页热销排行
        /// </summary>
        HotSale = 3,
        /// <summary>
        /// 投票
        /// </summary>
        Poll = 4,
        /// <summary>
        /// 热门关键字
        /// </summary>
        HotKeywords = 5,
        /// <summary>
        /// 默认关键字
        /// </summary>
        DefaultKeywords = 6,
        /// <summary>
        /// 置顶商品管理
        /// </summary>
        TopItem,
        /// <summary>
        /// 新闻公告管理
        /// </summary>
        NewsAndBulletin
    }

    /// <summary>
    /// 页面类型展现方式
    /// </summary>
    public enum PageTypePresentationType
    {
        /// <summary>
        /// 没有子级分类,比如首页
        /// </summary>
        NoneSubPages,
        /// <summary>
        /// 前台一级分类页面
        /// </summary>
        Category1,
        /// <summary>
        /// 前台二级分类页面
        /// </summary>
        Category2,
        /// <summary>
        /// 前台三级分类页面
        /// </summary>
        Category3,
        /// <summary>
        /// 品牌专区页面
        /// </summary>
        Brand,
        /// <summary>
        /// 品牌旗舰店页面
        /// </summary>
        Flagship,
        /// <summary>
        /// 商家页面
        /// </summary>
        Merchant,
        /// <summary>
        /// 其它促销页面
        /// </summary>
        OtherSales,
        /// <summary>
        /// 前台Apple专区
        /// </summary>
        AppleZone,
        /// <summary>
        /// 品牌专属
        /// </summary>
        BrandExclusive,
        /// <summary>
        /// 类别专属
        /// </summary>
        CategoryExclusive,
        /// <summary>
        /// 专卖店
        /// </summary>
        Stores
    }

    #region 留言评论相关

    /// <summary>
    /// 【评论模式所对应的对象】
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum RemarksType
    {
        /// <summary>
        /// P=评论
        /// </summary>
        Comment,
        /// <summary>
        /// T=公告及促销
        /// </summary>
        Promotion,
        /// <summary>
        ///D=网友讨论
        /// </summary>
        Discuss,
        /// <summary>
        /// C = 购物咨询
        /// </summary>
        Consult
    }

    /// <summary>
    /// 【评论模式用到】
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum RemarkTypeShow
    {
        /// <summary>
        /// 手动展示
        /// </summary>
        Manual,
        /// <summary>
        /// 自动展示
        /// </summary>
        Auto
    }

    /// <summary>
    /// 留言处理状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum CommentProcessStatus
    {
        /// <summary>
        /// 已作废 -1
        /// </summary>
        Invalid = -1,

        /// <summary>
        /// 待处理 0
        /// </summary>
        WaitHandling = 0,

        /// <summary>
        /// 处理中 1
        /// </summary>
        Handling = 1,

        /// <summary>
        /// 处理完毕 2
        /// </summary>
        Finish = 2
    }

    /// <summary>
    /// 评论处理状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
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
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
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
    /// 回复人类型，是否是vendor     
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum ReplyVendor
    {
        /// <summary>
        /// Vendor
        /// </summary>
        YES = 'M',

        /// <summary>
        /// 非VendorO
        /// </summary>
        NO = 'N'
    }


    /// <summary>
    /// 超时类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum OverTimeStatus
    {
        /// <summary>
        /// 超过2个工作时
        /// </summary>
        Over2Hour = 1,

        /// <summary>
        /// 超过1个工作日
        /// </summary>
        Over1Day = 2,

        /// <summary>
        /// 超过3个工作日
        /// </summary>
        Over3Day = 3
    }

    #endregion

    #region 关键字相关
    /// <summary>
    /// 关键字状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum KeywordsStatus
    {
        /// <summary>
        /// 所有R
        /// </summary>
        All,

        /// <summary>
        /// 待审核O
        /// </summary>
        Waiting,

        /// <summary>
        /// 审核通过A
        /// </summary>
        Passed,

        /// <summary>
        /// 审核拒绝= D
        /// </summary>
        Reject
    }

    /// <summary>
    /// 同义词类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum ThesaurusWordsType
    {
        /// <summary>
        /// 单向词 O
        /// </summary>
        Monodirectional,

        /// <summary>
        ///  双向词    T
        /// </summary>
        Doubleaction
    }

    /// <summary>
    /// 关键字对应商品 添加用户类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum KeywordsOperateUserType
    {
        /// <summary>
        /// 来自MKT员工 O
        /// </summary>
        MKTUser,

        /// <summary>
        ///  来自顾客    1
        /// </summary>
        Customer
    }
    #endregion


    /// <summary>
    /// 投票方式
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum PollType
    {
        /// <summary>
        /// S=单选  
        /// </summary>
        Single,
        /// <summary>
        ///  M=多选  
        /// </summary>
        Multiple,
        /// <summary>
        ///  C=混合
        /// </summary>
        Other,
        /// <summary>
        /// 简答=A
        /// </summary>
        ShortAnswer
    }
    public enum AreaRelationType
    {
        Banner,
        News
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum AndOrType
    {
        And,    //与：And，Include
        Or,     //或
        Not     //非：Exclude
    }


    #region 页面促销模板
    /// <summary>
    /// 页面促销模板状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum SaleAdvStatus
    {
        /// <summary>
        /// 有效
        /// </summary>
        Active = 0,
        /// <summary>
        /// 无效
        /// </summary>
        Deactive = -1
    }

    /// <summary>
    /// 展示模式
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum ShowType
    {
        /// <summary>
        /// 表格模板
        /// </summary>
        Table,
        /// <summary>
        /// 图文模板
        /// </summary>
        ImageText
    }

    /// <summary>
    /// 分组模式
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum GroupType
    {
        /// <summary>
        /// 自定义
        /// </summary>
        Custom,
        /// <summary>
        /// 一级类别
        /// </summary>
        LevelOne,
        /// <summary>
        /// 二级类别
        /// </summary>
        LevelTwo,
        /// <summary>
        /// 三级类别
        /// </summary>
        LevelThree
    }

    /// <summary>
    /// 推荐方式
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum RecommendType
    {
        /// <summary>
        /// 普通商品
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 置顶，每行一个
        /// </summary>
        //Top = 1,
        /// <summary>
        /// 显示推荐图标
        /// </summary>
        //ShowRecommendIcon = 2,
        /// <summary>
        /// LP01-每行5产品
        /// </summary>
        //FiveItemPerRow = 3,
        /// <summary>
        /// LP02-左侧合并8产品
        /// </summary>
        //EightItemLeft = 4,
        /// <summary>
        /// LP03-右侧合并8产品
        /// </summary>
        //EightItemRight = 5,
        /// <summary>
        /// LP04-左上合并8产品
        /// </summary>
        //EightItemUpperLeft = 6,
        /// <summary>
        /// LP05-左侧合并6产品
        /// </summary>
        SixItemLeft = 7
    }
    #endregion

    /// <summary>
    /// 有效，无效公用枚举
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum NewsStatus
    {
        /// <summary>
        /// 无效
        /// </summary>
        Deactive,
        /// <summary>
        /// 有效
        /// </summary>
        Active
    }

    /// <summary>
    /// 前台分类级别
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum ECCategoryLevel
    {
        /// <summary>
        /// 一级分类
        /// </summary>
        Category1,
        /// <summary>
        /// 二级分类
        /// </summary>
        Category2,
        /// <summary>
        /// 三级分类
        /// </summary>
        Category3
    }

    /// <summary>
    /// 产品价格举报状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum ProductPriceCompareStatus
    {
        /// <summary>
        /// 待审核
        /// </summary>
        WaitAudit,
        /// <summary>
        /// 审核有效
        /// </summary>
        AuditPass,
        /// <summary>
        /// 审核无效
        /// </summary>
        AuditDecline
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum DisplayLinkStatus
    {
        Hide,
        Display
    }

    /// <summary>
    /// OpenAPI有效，无效枚举
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum OpenAPIStatus
    {
        /// <summary>
        /// 无效
        /// </summary>
        Deactive = 'D',
        /// <summary>
        /// 有效
        /// </summary>
        Active = 'A'
    }

    /// <summary>
    /// 有效，无效枚举
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum IsDefaultStatus
    {
        /// <summary>
        /// 无效
        /// </summary>
        Deactive = 'D',
        /// <summary>
        /// 有效
        /// </summary>
        Active = 'A'
    }

    /// <summary>
    /// OpenAPI有效，无效枚举
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum OpenAPIGenerateInterval
    {
        /// <summary>
        /// 2小时
        /// </summary>
        Two = 2,
        /// <summary>
        /// 5小时
        /// </summary>
        Five = 5,

        /// <summary>
        /// 10小时
        /// </summary>
        Ten = 10,

        /// <summary>
        /// 24小时
        /// </summary>
        TwentyFour = 24
    }

    /// <summary>
    /// hao123折扣频道
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum PositionType
    {
        /// <summary>
        /// 正常活动
        /// </summary>
        NormalActivity = 1,

        /// <summary>
        /// 预告活动
        /// </summary>
        TrailerActivity = 2,
    }

    /// <summary>
    /// hao123折扣频道
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum MajorType
    {
        /// <summary>
        /// 主打
        /// </summary>
        Yes = 1,

        /// <summary>
        /// 非主打
        /// </summary>
        No = 0,
    }

    /// <summary>
    /// 活动类别
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum CategoryType
    {
        PromotionalMerchandise = 1,

        Clothing = 2,

        Cosmetology = 3,

        Shoes = 4,

        ElectronicProducts = 5,

        Dress = 6,

        Package = 7,

        Daily = 8,

        Movement = 9,

        Maternal = 10,

        LuxuryGoods = 11
    }

    /// <summary>
    /// 限制使用优惠券类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum CouponLimitType
    {
        /// <summary>
        /// 抢购
        /// </summary>
        PanicBuying = 'C',
        /// <summary>
        /// 秒杀
        /// </summary>
        Kill = 'K',
        ///// <summary>
        ///// 炸蛋
        ///// </summary>
        //FriedEgg = 'T',

        /// <summary>
        /// 团购
        /// </summary>
        GroupBuying = 'G',

        // /// <summary>
        // /// SIM CARD
        // /// </summary>
        // SimCard='S',
        // /// <summary>
        // /// 合约机
        // /// </summary>
        //ContractMachine='P',

        /// <summary>
        /// 礼品卡
        /// </summary>
        GiftCard = 'L',

        /// <summary>
        /// 手动
        /// </summary>
        Manually = 'M'


        //<option value="">所有</option>
        //<option value="C">抢购</option> 
        //<option value="K">秒杀</option>   
        //<option value="T">炸蛋</option>     
        //<option value="G">团购</option>       
        //<option value="S">SIM CARD</option>     
        //<option value="P">合约机</option>         
        //<option value="L">礼品卡</option>           
        //<option value="M">手动</option>                           

    }
    /// <summary>
    /// 清除类型
    /// </summary>
    public enum ClearType
    {
        /// <summary>
        /// 待清除
        /// </summary>
        WaitClear = 'W',
        /// <summary>
        /// 已清除
        /// </summary>
        CompleteClear = 'F'
    }

    /// <summary>
    /// 商品支付方式状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum PayTypeStatus
    {
        /// <summary>
        /// 有效
        /// </summary>
        A,
        /// <summary>
        /// 已中止
        /// </summary>
        D
    }

    /// <summary>
    /// 动态类别类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum DynamicCategoryType
    {
        /// <summary>
        /// 标网
        /// </summary>
        Standard,
        /// <summary>
        /// 网姐
        /// </summary>
        WangJie
    }

    /// <summary>
    /// 动态类别状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum DynamicCategoryStatus
    {
        Active = 0,
        Deactive = -1
    }

    /// <summary>
    /// 团购类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum GroupBuyingCategoryType
    {
        /// <summary>
        /// 实体商品
        /// </summary>
        Physical,
        /// <summary>
        /// 虚拟商品
        /// </summary>
        Virtual,
        /// <summary>
        /// 0元抽奖
        /// </summary>
        [Obsolete("此类型已废弃")]
        ZeroLottery
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum GroupBuyingFeedbackStatus
    {
        /// <summary>
        /// 未阅读
        /// </summary>
        UnRead,
        /// <summary>
        /// 已阅读
        /// </summary>
        Readed
    }

    /// <summary>
    /// 团购商务合作状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum BusinessCooperationStatus
    {
        /// <summary>
        /// 未处理
        /// </summary>
        UnHandled,
        /// <summary>
        /// 已处理
        /// </summary>
        Handled
    }

    /// <summary>
    /// 团购结算单状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum SettlementBillStatus
    {
        /// <summary>
        /// 未结算
        /// </summary>
        UnSettle,
        /// <summary>
        /// 已结算
        /// </summary>
        Settled
    }

    /// <summary>
    /// 团购券状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum GroupBuyingTicketStatus
    {
        /// <summary>
        /// 未使用
        /// </summary>
        UnUse = 0,
        /// <summary>
        /// 已使用
        /// </summary>
        Used = 1,
        /// <summary>
        /// 已作废
        /// </summary>
        Abandon = -1,
        /// <summary>
        /// 已过期
        /// </summary>
        Expired = -2,
        /// <summary>
        /// 已创建(未付款)
        /// </summary>
        Created = -3
    }

    /// <summary>
    /// 团购类别状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum GroupBuyingCategoryStatus
    {
        /// <summary>
        /// 有效
        /// </summary>
        Valid = 0,
        /// <summary>
        /// 无效
        /// </summary>
        InValid = -1,
    }

    /// <summary>
    /// 楼层分组标签内容类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum FloorItemType
    {
        /// <summary>
        /// 商品
        /// </summary>
        Product = 1,
        /// <summary>
        /// Banner图片
        /// </summary>
        Banner = 2,
        /// <summary>
        /// 品牌
        /// </summary>
        Brand = 3,
        /// <summary>
        /// 文本链接
        /// </summary>
        TextLink = 4,
    }

    /// <summary>
    /// 楼层页面类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum PageCodeType
    {
        /// <summary>
        /// 网站首页
        /// </summary>
        WebsiteHome = 1,
        /// <summary>
        /// 手机首页
        /// </summary>
        AppHome = 1001,
        /// <summary>
        /// 一级类别
        /// </summary>
        C1 = 2,
        /// <summary>
        /// 促销模板
        /// </summary>
        Promotion = 5
    }


    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum ECCCategoryManagerStatus
    {
        [Description("有效")]
        Active = 0,
        [Description("无效")]
        DeActive = -1
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResMKTEnum")]
    public enum ECCCategoryManagerType
    {
        [Description("一级类别")]
        ECCCategoryType1 = 1,
        [Description("二级类别")]
        ECCCategoryType2 = 2,
        [Description("三级类别")]
        ECCCategoryType3 = 3,
    }
}
