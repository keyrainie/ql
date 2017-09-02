using System.ComponentModel;

namespace ECCentral.BizEntity.IM
{
    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum ProductType
    {
        /// <summary>
        /// 正常品
        /// </summary>
        [Description("正常品")]
        Normal = 0,
        /// <summary>
        /// 二手品
        /// </summary>
        [Description("二手品")]
        OpenBox = 1,
        /// <summary>
        /// 坏品
        /// </summary>
        [Description("坏品")]
        Bad = 2,
        /// <summary>
        /// 虚拟商品
        /// </summary>
        [Description("虚拟商品")]
        Virtual=3
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum ProductCloneType
    {
        [Description("二手品")]
        OpenBox = 'R',

        [Description("拍卖品")]
        Auction = 'P',

        [Description("赠品")]
        Gifts = 'Z',

        [Description("坏品")]
        Bad = 'B'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum ProductGroupImageShow
    {
        [Description("显示")]
        Yes = 'Y',
        [Description("不显示")]
        No = 'N'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum WarrantyShow
    {
        [Description("显示")]
        Yes = 1,
        [Description("不显示")]
        No = 0
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum ProductGroupPolymeric
    {
        [Description("聚合显示")]
        Yes = 'Y',
        [Description("不聚合显示")]
        No = 'N'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum Large
    {
        [Description("大货")]
        Yes = 'Y',
        [Description("非大货")]
        No = 'N'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum IsLarge
    {
        [Description("大货")]
        Yes = 'L',
        [Description("小货")]
        No = 'S',
        [Description("未定义")]
        Undefined = '-'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum ProductStatus
    {
        [Description("上架")]
        Active = 1,
        [Description("上架不展示")]
        InActive_Show = 0,
        [Description("下架")]
        InActive_UnShow = 2,
        [Description("作废")]
        Abandon = -1,
        [Description("待审核")]
        InActive_Auditing = 3,
        [Description("已审核")]
        InActive_Audited = 4,
        [Description("审核未通过")]
        InActive_AuditenNO = 5
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum ProductPayType
    {
        [Description("仅现金")]
        MoneyOnly = 0,
        [Description("均支持")]
        All = 1,
        [Description("仅积分")]
        PointOnly = 2
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum OfferVATInvoice
    {
        [Description("提供增值税发票")]
        Yes = 1,
        [Description("不提供增值税发票")]
        No = 0
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum ValidStatus
    {
        [Description("有效")]
        Active = 'A',
        [Description("无效")]
        DeActive = 'D'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum ProductAccessoryStatus
    {
        [Description("有效")]
        Active = 0,
        [Description("无效")]
        DeActive = -1
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum ProductResourceStatus
    {
        [Description("有效")]
        Active = 'A',
        [Description("无效")]
        DeActive = 'D'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum ProductResourceIsShow
    {
        [Description("是")]
        Yes = 'Y',
        [Description("否")]
        No = 'N'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum ProductRankPriceStatus
    {
        [Description("有效")]
        Active = 0,
        [Description("无效")]
        DeActive = -1
    }

    public enum TimelyPromotionTitleStatus
    {
        [Description("初始")]
        Original = 'O',

        [Description("有效")]
        Active = 'A',

        [Description("无效")]
        DeActive = 'D'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum ManufacturerStatus
    {
        [Description("有效")]
        Active = 0,
        [Description("无效")]
        DeActive = -1
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum CategoryStatus
    {
        [Description("有效")]
        Active = 0,
        [Description("无效")]
        DeActive = -1
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum StockType
    {
        [Description("虚库")]
        Virtual,
        [Description("实库")]
        Real
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum ProductPriceRequestStatus
    {
        [Description("待审核")]
        Origin = 0,
        [Description("审核通过")]
        Approved = 1,
        [Description("审核拒绝")]
        Deny = -1,
        [Description("取消审核")]
        Canceled = -2,
        [Description("待高级审核")]
        NeedSeniorApprove = 9
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum NeweggProductPriceRequestStatus
    {
        [Description("TL审核")]
        Origin = 0,
        [Description("审核通过")]
        Approved = 1,
        [Description("审核拒绝")]
        Deny = -1,
        [Description("取消审核")]
        Canceled = -2,
        [Description("待高级审核")]
        NeedSeniorApprove = 9
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum ProductPriceRequestAuditType
    {
        [Description("初级审核")]
        Audit = 'T',
        [Description("高级审核")]
        SeniorAudit = 'P'
    }

    /// <summary>
    /// 查询价格审核的审核类型(中蛋网)
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum QueryProductPriceRequestAuditType
    {
        [Description("待TL审核")]
        Audit = 0,
        [Description("待TL提交")]
        Submit = 3,
        [Description("待PMD提交")]
        SeniorAudit = 1
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum Currency
    {
        [Description("人民币")]
        RMB = 1
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum WholeSaleLevelType
    {
        [Description("批发等级一")]
        L1 = 1,
        [Description("批发等级二")]
        L2 = 2,
        [Description("批发等级三")]
        L3 = 3
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum GiftCardType
    {
        [Description("标准礼品卡")]
        Standard = 1,
        [Description("CS补偿电子卡")]
        Compensate = 2,
        [Description("RMA退款电子卡")]
        Refund = 3
    }

    /// <summary>
    /// 礼品卡材质类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum CardMaterialType
    {
        [Description("电子卡")]
        Electronic = 1,
        [Description("实物卡")]
        Physical = 2,
        [Description("礼品券")]
        Voucher
    }

    /// <summary>
    /// 礼品卡操作类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum ActionType
    {
        [Description("订单")]
        SO,
        [Description("退货")]
        RMA,
        [Description("退款调整单")]
        ROBalance,
        //bober add
        [Description("作废")]
        MandatoryVoid,
        [Description("锁定")]
        Hold,
        [Description("解锁")]
        UnHold,
        [Description("修改有效期")]
        AdjustExpireDate,
        [Description("转发")]
        ForwardCard
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum GiftCardStatus
    {
        [Description("自动激活")]
        Valid,
        [Description("未激活")]
        InValid,
        [Description("作废")]
        Void,
        [Description("已兑换")]
        Used,
        [Description("手动激活")]
        ManualActive
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum GiftVoucherProductStatus
    {
        [Description("审核失败")]
        AuditFail = -2,
        [Description("作废")]
        Void = -1,
        [Description("等待审核")]
        WaittingAudit = 1,
        [Description("审核通过")]
        Audit = 2
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum GiftVoucherRelateProductStatus
    {
        [Description("可用")]
        Active = 'A',
        [Description("不可用")]
        Deactive='D'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum GVRReqType
    {
        [Description("新增")]
        Add = 0,
        [Description("删除")]
        Delete = 1
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum GVRReqAuditStatus
    {
        [Description("审核成功")]
        AuditSuccess='S',
        [Description("等待审核")]
        AuditWaitting='W',
        [Description("审核失败")]
        AuditFailed = 'F'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum PMStatus
    {
        [Description("有效")]
        Active = 0,
        [Description("无效")]
        DeActive = -1
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum IsDefault
    {
        [Description("否")]
        Active = 0,
        [Description("是")]
        DeActive = 1
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum IsQueryDefault
    {
        [Description("否")]
        Active = 0,
        [Description("是")]
        DeActive = 1

    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum CategoryAccessoriesStatus
    {
        [Description("有效")]
        Active = 0,
        [Description("无效")]
        DeActive = -1
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum PMGroupStatus
    {
        [Description("有效")]
        Active = 0,
        [Description("无效")]
        DeActive = -1
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum ResourcesType
    {
        [Description("图片")]
        Image = 'I',
        [Description("360图片")]
        Image360 = 'D',
        [Description("视频")]
        Video = 'V',
        [Description("不支持")]
        NotSupported = 'N'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum PropertyStatus
    {
        [Description("有效")]
        Active = 1,
        [Description("无效")]
        DeActive = 0
    }


    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum InventorySync
    {
        [Description("是")]
        Yes = 'Y',
        [Description("否")]
        No = 'N'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum CategoryPropertyStatus
    {
        [Description("是")]
        Yes = 1,
        [Description("否")]
        No = 0
    }

    public enum ProductPropertyRequired
    {
        [Description("是")]
        Yes = 1,
        [Description("否")]
        No = 0
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum WebDisplayStyle
    {
        [Description("属性")]
        Property = 0,
        [Description("属性值")]
        PropertyValue = 1,
        [Description("属性+属性值")]
        PropertyAndPropertyValue = 2,
        [Description("属性值+属性")]
        PropertyValueAndProperty = 3
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum PropertyType
    {
        [Description("分组属性")]
        Grouping = 'G',
        [Description("一般属性")]
        Other = 'A'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum Comparison
    {
        [Description("=")]
        Equal = 0,
        [Description("<>")]
        Unequal = 1
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum CheapenRisk
    {
        [Description("A类(0-2周)")]
        A = 1,
        [Description("B类(2周至1月)")]
        B,
        [Description("C类(1月以上)")]
        C
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum PayPeriodType
    {
        [Description("预付款")]
        AdvancePayment = 1,
        [Description("货到付款 ")]
        CashOnDelivery,
        [Description("0-7天")]
        SevenDay,
        [Description("7-15天")]
        FifteenDay,
        [Description("15天以上")]
        OverFifteenDay,
        [Description("其它")]
        Other

    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]   
    public enum MinMarginDays
    {
        /// <summary>
        /// 0-30天
        /// </summary>
        [Description("0-30天")]
        Thirty = 0,
        /// <summary>
        /// 31-60天
        /// </summary>
        [Description("31-60天 ")]
        Sixty,
        /// <summary>
        /// 61-90天
        /// </summary>
        [Description("61-90天")]
        Ninety,
        /// <summary>
        /// 91-120天
        /// </summary>
        [Description("91-120天")]
        OneHundredAndTwenty,
        /// <summary>
        /// 121-180天
        /// </summary>
        [Description("121-180天")]
        OneHundredAndEighty,
        /// <summary>
        /// 超过180天
        /// </summary>
        [Description("超过180天")]
        Other

    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum CategoryExtendWarrantyStatus
    {
        [Description("有效")]
        Active = 'A',
        [Description("无效")]
        DeActive = 'D'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum CategoryExtendWarrantyYears
    {
        [Description("半年")]
        SixMonths = 100,
        [Description("一年")]
        Year = 1,
        [Description("一年半")]
        YearAndSixMonths = 105,
        [Description("二年")]
        TwoYears = 2,
        [Description("二年半")]
        TwoYearsAndSixMonths = 205,
        [Description("三年")]
        ThreeYears = 3
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum CategoryExtendWarrantyDisuseBrandStatus
    {
        [Description("有效")]
        Active = 'A',
        [Description("无效")]
        DeActive = 'D'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum SellerProductRequestStatus
    {
        [Description("待PM审核")]
        WaitApproval = 'O',
        [Description("审核不通过")]
        UnApproved = 'D',
        [Description("审核通过")]
        Approved = 'A',
        [Description("处理中")]
        Processing = 'P',
        [Description("已完成")]
        Finish = 'F'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum SellerProductRequestType
    {
        [Description("新品创建")]
        NewCreated = 'N',
        [Description("参数更新")]
        ParameterUpdate = 'P',
        [Description("图片与描述更新")]
        ImageAndDescriptionUpdate = 'I'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum SellerProductRequestTakePictures
    {
        [Description("是")]
        Yes = 'Y',
        [Description("否")]
        No = 'N'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum SellerProductRequestOfferInvoice
    {
        [Description("是")]
        Yes = 'Y',
        [Description("否")]
        No = 'N'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum SellerProductRequestFileType
    {
        [Description("图片")]
        Image = 'I',
        [Description("文本文档")]
        Text = 'P'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum SellerProductRequestFileStatus
    {
        [Description("待处理")]
        WaitProcess = 'O',
        [Description("处理完成")]
        Finish = 'F'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum NotifyStatus
    {
        [Description("已通知")]
        NotifyYes = 1,
        [Description("未通知")]
        NotifyNo = 0,
        [Description("废弃")]
        NotifyBad = -1,

    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum VirtualType
    {
        [Description("接受预定(保税区：2-3个工作日备货)")]
        Short = 0,
        [Description("接受预定(境外：10个工作日备货)")]
        Long = 1,
        //[Description("依据货源情况而定")]
        //NoSure = 2
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum CategoryType
    {
        [Description("一级类别")]
        CategoryType1 = 1,
        [Description("二级类别")]
        CategoryType2 = 2,
        [Description("三级类别")]
        CategoryType3 = 3,

    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum VirtualRequest
    {
        [Description("待审核")]
        Origin = 0,
        [Description("其他")]
        Other = 999
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum ProductChannelInfoStatus
    {
        [Description("有效")]
        Active = 'A',
        [Description("无效")]
        DeActive = 'D'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum ProductChannelPeriodPriceStatus
    {
        [Description("初始")]
        Init = 'O',
        [Description("待审核")]
        WaitApproved = 'P',
        [Description("就绪")]
        Ready = 'R',
        [Description("运行")]
        Running = 'A',
        [Description("作废")]
        Abandon = 'D',
        [Description("已完成")]
        Finish = 'F'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum ProductChannelPeriodPriceOperate
    {
        [Description("创建编辑")]
        CreateOrEdit,
        [Description("提交")]
        Submit,
        [Description("撤销")]
        CancelSubmit,
        [Description("审核通过")]
        Approve,
        [Description("审核不通过")]
        UnApprove,
        [Description("停止")]
        Stop
    }


    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum BooleanEnum
    {
        [Description("是")]
        Yes = 'Y',
        [Description("否")]
        No = 'N'
    }
    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum BrandStatus
    {
        [Description("有效")]
        Active = 0,
        [Description("无效")]
        DeActive = -1
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum BrandStoreType
    {
        [Description("普通店")]
        OrdinaryStore = 0,
        //[Description("专卖店")]
        //MonopolyStore = 1,
        [Description("旗舰店")]
        FlagshipStore = 2
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum AuthorizedStatus
    {
        [Description("有效")]
        Active = 'A',
        [Description("无效")]
        DeActive = 'D'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum PriorityType
    {
        [Description("1")]
        One = 1,
        [Description("2")]
        Two = 2,
        [Description("3")]
        Three = 3,
        [Description("4")]
        Four = 4

    }
    /// <summary>
    /// 类别审核的三个状态
    /// </summary>
    public enum CategoryAuditStatus
    {
        /// <summary>
        /// 审核通过
        /// </summary>
        CategoryAuditPass = 1,
        /// <summary>
        /// 审核不通过
        /// </summary>
        CategoryAuditNotPass = -1,
        /// <summary>
        /// 取消审核
        /// </summary>
        CategoryAuditCanel = -2,

        /// <summary>
        /// 待审核
        /// </summary>
        CategoryWaitAudit = 0,

    }
    /// <summary>
    /// 类别审核的两种操作
    /// </summary>
    public enum OperationType
    {
        /// <summary>
        /// 新建
        /// </summary>
        Create = 1,
        /// <summary>
        /// 更新
        /// </summary>
        Update = 2
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum PMRangeType
    {
        /// <summary>
        /// PM
        /// </summary>
        [Description("PM")]
        PM,
        /// <summary>
        /// 备份PM
        /// </summary>
        [Description("备份PM")]
        BAKPM
    }


    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    [System.Obsolete("此字段已弃用")]
    public enum UnicomContractPhoneNumberStatus
    {
        /// <summary>
        /// 有效
        /// </summary>
        [Description("有效")]
        Active = 'A',
        /// <summary>
        /// 无效
        /// </summary>
        [Description("无效")]
        DeActive = 'D',

        /// <summary>
        /// 下单
        /// </summary>
        [Description("下单")]
        CreateOrder = 'S'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum CategoryTemplateType
    {
        /// <summary>
        /// 商品标题
        /// </summary>
        [Description("商品标题")]
        TemplateProductTitle = 0,
        /// <summary>
        /// 商品描述
        /// </summary>
        [Description("商品描述")]
        TemplateProductDescription = 1,
        /// <summary>
        /// 商品简名
        /// </summary>
        [Description("商品简名")]
        TemplateProductName = 2,
        /// <summary>
        /// Web
        /// </summary>
        [Description("Web")]
        TemplateWeb = 3
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum RmaPolicyType
    {
        /// <summary>
        /// 标准类型
        /// </summary>
        StandardType = 'p',
        /// <summary>
        /// 扩展类型
        /// </summary>
        ExtendType = 'E',

        /// <summary>
        /// 厂商类型
        /// </summary>
        ManufacturerType = 'M',

        /// <summary>
        /// 折扣类型
        /// </summary>
        DiscountType = 'D',

        /// <summary>
        /// 商家类型
        /// </summary>
        SellerType = 'S'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum RmaPolicyStatus
    {
        /// <summary>
        /// 无效
        /// </summary>
        DeActive = 'D',

        /// <summary>
        /// 有效
        /// </summary>
        Active = 'A'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum IsOnlineRequst
    {
        /// <summary>
        /// 是
        /// </summary>
        YES = 'Y',

        /// <summary>
        /// 否
        /// </summary>
        NO = 'N'
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum RmaLogActionType
    {
        Create = 'C',
        Edit = 'E',
        DeActive = 'D',
        Active = 'A'
    }

    /// <summary>
    /// 采集日期类型
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum CollectDateType
    {
        /// <summary>
        /// 过期日期
        /// </summary>
        ExpiredDate,
        /// <summary>
        /// 生产日期
        /// </summary>
        ManufactureDate
    }
    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum ProductInventoryType
    {
        
        /// <summary>
        /// 普通商品,红旗仓自营 走系统库存
        /// </summary>
        [Description("自营")]
        Normal = 0,
        /// <summary>
        /// 柜台取货（门店库存）
        /// </summary>
        [Description("柜台门店管库存")]
        GetShopInventory = 1,
        /// <summary>
        /// 柜台取货（非门店库存）走系统库存
        /// </summary>
        [Description("柜台门店不管库存")]
        GetShopUnInventory = 2,
        /// <summary>
        /// 总部家电
        /// </summary>
        [Description("总部家电")]
        Company = 3,
        /// <summary>
        /// 厂家送货 走系统库存
        /// </summary>
        [Description("厂家送货")]
        Factory = 4,
        /// <summary>
        /// 双开门
        /// </summary>
        [Description("双开门")]
        TwoDoor = 5,
         /// <summary>
        /// 租赁商品 走系统库存
        /// </summary>
        [Description("租赁商品")]
        Merchent = 6    
    }

    public enum IsSyncShopPrice
    {
        /// <summary>
        /// 不同步门店价格
        /// </summary>
        [Description("不同步")]
        NO = 0,
        /// <summary>
        /// 同步门店价格
        /// </summary>
        [Description("同步")]
        YES = 1
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum GiftVoucherType
    {
        /// <summary>
        /// 不是礼品券可兑换商品
        /// </summary>
        NotVoucherProduct = 0,
        /// <summary>
        /// 礼品券商品，允许前台销售
        /// </summary>
        SaleVoucherProduct = 1,
        /// <summary>
        /// 礼品券商品，不允许前台销售
        /// </summary>
        NoSaleVoucherProduct = 2
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum EntryBizType
    {
        /// <summary>
        /// 一般进口
        /// </summary>
        NormalImport=0,

        /// <summary>
        /// 保税进口
        /// </summary>
        BondedImport=1
    }

    ///// <summary>
    ///// 申报关区
    ///// </summary>
    //[Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    //public enum ApplyDistrict
    //{
    //    /// <summary>
    //    /// 直邮中国商品
    //    /// </summary>
    //    DirectMail_CN= 2244,

    //    /// <summary>
    //    /// 自贸专区商品
    //    /// </summary>
    //    FreeTradeZone = 2216
    //}
    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum IsTariffApply
    {
        /// <summary>
        /// 未报关
        /// </summary>
        No = 0,

        /// <summary>
        /// 已报关
        /// </summary>
        Yes = 1
    }

    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum StoreType
    {
        /// <summary>
        /// 常温
        /// </summary>
        Narmal = 0,

        /// <summary>
        /// 冷藏
        /// </summary>
        Cold = 1,

        /// <summary>
        /// 冷冻
        /// </summary>
        Frozen=2
    }

    /// <summary>
    /// 商品备案状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum ProductEntryStatus
    {
        /// <summary>
        /// 备案失败
        /// </summary>
        EntryFail = -2,
        /// <summary>
        /// 审核失败
        /// </summary>
        AuditFail = -1,
        /// <summary>
        /// 初始化
        /// </summary>
        Origin = 0,
        /// <summary>
        /// 待审核
        /// </summary>
        WaitingAudit = 1,
        /// <summary>
        /// 审核成功
        /// </summary>
        AuditSucess = 2,
        /// <summary>
        /// 备案中
        /// </summary>
        Entry = 3,
        /// <summary>
        /// 备案成功
        /// </summary>
        EntrySuccess = 4
    }

    /// <summary>
    /// 商品备案扩展状态
    /// </summary>
    [Description("ECCentral.BizEntity.Enum.Resources.ResIMEnum")]
    public enum ProductEntryStatusEx
    {
        /// <summary>
        /// 报关失败
        /// </summary>
        CustomsFail = -2,

        /// <summary>
        /// 商检失败
        /// </summary>
        InspectionFail = -1,

        /// <summary>
        /// 商检中
        /// </summary>
        Inspection = 0,

        /// <summary>
        /// 商检成功，待报关
        /// </summary>
        InspectionSucess = 1,

        /// <summary>
        /// 报关中
        /// </summary>
        Customs = 2,

        /// <summary>
        /// 报关成功
        /// </summary>
        CustomsSuccess = 3
    }

    public enum EntryStatusOperation
    {
        /// <summary>
        /// 审核
        /// </summary>
        Audit = 1,

        /// <summary>
        /// 商检
        /// </summary>
        Inspection = 2,

        /// <summary>
        /// 待报关
        /// </summary>
        Customs = 3
    }
}
