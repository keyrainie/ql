using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.SO;

namespace ECCentral.Service.MKT.SqlDataAccess
{
    public class EnumCodeMapRegister : IStartup
    {
        public void Start()
        {
            EnumCodeMapper.AddMap<FeatureType>(new Dictionary<FeatureType, string>{
                { FeatureType.New, "N" },
                { FeatureType.Hot, "H" }
            });

            EnumCodeMapper.AddMap<GroupBuyingStatus>(new Dictionary<GroupBuyingStatus, string>{
                { GroupBuyingStatus.Init, "O" },
                { GroupBuyingStatus.WaitingAudit, "W" },
                { GroupBuyingStatus.VerifyFaild, "N" },
                { GroupBuyingStatus.Pending, "P" },
                { GroupBuyingStatus.WaitHandling, "R" },
                { GroupBuyingStatus.Active, "A" },
                { GroupBuyingStatus.Deactive, "D" },
                { GroupBuyingStatus.Finished, "F" }
            });
            EnumCodeMapper.AddMap<CountdownStatus>(new Dictionary<CountdownStatus, int>{
                { CountdownStatus.Abandon, -1 },
                { CountdownStatus.WaitForVerify, -3 },
                { CountdownStatus.VerifyFaild, -4 },
                { CountdownStatus.Ready, 0 },
                { CountdownStatus.Running, 1 },
                { CountdownStatus.Finish, 2 },
                { CountdownStatus.Interupt, -2 },
                { CountdownStatus.Init, -5 }
            });
            EnumCodeMapper.AddMap<ADStatus>(new Dictionary<ADStatus, string>{
                { ADStatus.Deactive, "D" },
                { ADStatus.Active, "A" }
            });
            EnumCodeMapper.AddMap<IsDefaultStatus>(new Dictionary<IsDefaultStatus, string>{
                { IsDefaultStatus.Deactive, "D" },
                { IsDefaultStatus.Active, "A" }
            });
            EnumCodeMapper.AddMap<ADTStatus>(new Dictionary<ADTStatus, string>{
                { ADTStatus.Deactive, "D" },
                { ADTStatus.Active, "A" },
                { ADTStatus.Test, "T" }
            });

            EnumCodeMapper.AddMap<RemarksType>(new Dictionary<RemarksType, string>{
                { RemarksType.Promotion, "R" },
                { RemarksType.Comment, "P" },
                { RemarksType.Discuss, "D" },
                { RemarksType.Consult, "C" }
            });

            EnumCodeMapper.AddMap<RemarkTypeShow>(new Dictionary<RemarkTypeShow, int>{
                { RemarkTypeShow.Auto, 0},
                { RemarkTypeShow.Manual, -1 }
            });

            EnumCodeMapper.AddMap<YNStatus>(new Dictionary<YNStatus, string>{
                { YNStatus.No, "N" },
                { YNStatus.Yes, "Y" }
            });
            EnumCodeMapper.AddMap<IsDefaultShow>(new Dictionary<IsDefaultShow, string>{
                { IsDefaultShow.Show, "Y" },
                { IsDefaultShow.Hide, "N" }
            });
            EnumCodeMapper.AddMap<GroupBuyingStatusForNeweggg>(new Dictionary<GroupBuyingStatusForNeweggg, string>{
                { GroupBuyingStatusForNeweggg.WaitingAudit, "O" },
                { GroupBuyingStatusForNeweggg.Pending, "P" },
                { GroupBuyingStatusForNeweggg.Active, "A" },
                { GroupBuyingStatusForNeweggg.Finished, "F" },
                { GroupBuyingStatusForNeweggg.Deactive, "D" },
            });



            EnumCodeMapper.AddMap<BannerType>(new Dictionary<BannerType, string>{
                { BannerType.Image, "I" },
                { BannerType.Flash, "F" },
                 { BannerType.HTML, "H" },
                { BannerType.Text, "T" },
                { BannerType.Video, "V" }
            });

            EnumCodeMapper.AddMap<ModuleType>(new Dictionary<ModuleType, int>{
                { ModuleType.Banner, 0 },
                { ModuleType.SEO, 1 },
                 { ModuleType.ProductRecommend, 2 },
                { ModuleType.HotSale, 3 },
                { ModuleType.Poll, 4 },
                 { ModuleType.HotKeywords, 5 },
                { ModuleType.DefaultKeywords, 6 }
            });

            EnumCodeMapper.AddMap<CouponsMKTType>(new Dictionary<CouponsMKTType, string>
            {
                { CouponsMKTType.MKTAFC,"F"},
                { CouponsMKTType.MKTAmbassador,"A"},
                { CouponsMKTType.MKTBD,"B"},
                { CouponsMKTType.MKTCS,"C"},
                { CouponsMKTType.MKTEmail,"M"},
                { CouponsMKTType.MKTInternal,"N"},
                { CouponsMKTType.MKTNetworkAlliance,"W"},
                { CouponsMKTType.MKTOffline,"L"},
                { CouponsMKTType.MKTOnline,"O"},
                { CouponsMKTType.MKTPM,"P"}, 
                { CouponsMKTType.MKTPlace,"S"} 
            });

            EnumCodeMapper.AddMap<CouponsStatus>(new Dictionary<CouponsStatus, string>
            {
                { CouponsStatus.Finish,"F"},
                { CouponsStatus.Init,"O"},
                { CouponsStatus.Ready,"R"},
                { CouponsStatus.Run,"A"},
                { CouponsStatus.Void,"D"},
                { CouponsStatus.WaitingAudit,"W"},
                 { CouponsStatus.Stoped,"S"}
            });

            EnumCodeMapper.AddMap<OpenAPIStatus>(new Dictionary<OpenAPIStatus, string>
            {
                { OpenAPIStatus.Active,"A"},
                { OpenAPIStatus.Deactive,"D"}
            });

            //公告及促销评论状态
            EnumCodeMapper.AddMap<NewsAdvReplyStatus>(new Dictionary<NewsAdvReplyStatus, int>
            {
                { NewsAdvReplyStatus.Show,0},
                { NewsAdvReplyStatus.SystemHide,-1},
                { NewsAdvReplyStatus.HandHide,-2} 
            });

            EnumCodeMapper.AddMap<CouponsProductRangeType>(new Dictionary<CouponsProductRangeType, string>
            {
                {CouponsProductRangeType.AllProducts,"A"},
                {CouponsProductRangeType.LimitCategoryBrand,"X"},
                {CouponsProductRangeType.LimitProduct,"I"}
            });

            EnumCodeMapper.AddMap<CouponsBindConditionType>(new Dictionary<CouponsBindConditionType, string>
            {
                {CouponsBindConditionType.None,"A"},
                {CouponsBindConditionType.Rigester,"R"},
                {CouponsBindConditionType.Birthday,"B"},
                {CouponsBindConditionType.SO,"O"},
                {CouponsBindConditionType.Get,"L"}
                //{CouponsBindConditionType.Alipay,"Z"}
            });
            EnumCodeMapper.AddMap<CouponsValidPeriodType>(new Dictionary<CouponsValidPeriodType, int>
            {
                {CouponsValidPeriodType.All,0},
                {CouponsValidPeriodType.PublishDayToOneWeek,1},
                {CouponsValidPeriodType.PublishDayToOneMonth,2},
                {CouponsValidPeriodType.PublishDayToTwoMonths,3},
                {CouponsValidPeriodType.PublishDayToThreeMonths,4},
                {CouponsValidPeriodType.PublishDayToSixMonths,5},
                {CouponsValidPeriodType.CustomPeriod,6}
            });


            EnumCodeMapper.AddMap<CouponsRuleType>(new Dictionary<CouponsRuleType, string>
            {
                {CouponsRuleType.ProductDiscount,"D"} 
            });

            EnumCodeMapper.AddMap<CouponCodeType>(new Dictionary<CouponCodeType, string>
            {
                {CouponCodeType.Common,"C"},
                {CouponCodeType.ThrowIn,"T"}
            });

            EnumCodeMapper.AddMap<CouponCodeUsedStatus>(new Dictionary<CouponCodeUsedStatus, string>
            {
                {CouponCodeUsedStatus.Active,"A"},
                {CouponCodeUsedStatus.Deactive,"D"}
            });

            EnumCodeMapper.AddMap<PSDiscountTypeForOrderAmount>(new Dictionary<PSDiscountTypeForOrderAmount, string>
            {
                {PSDiscountTypeForOrderAmount.OrderAmountDiscount,"D"},
                {PSDiscountTypeForOrderAmount.OrderAmountPercentage,"P"}
            });

            EnumCodeMapper.AddMap<PSDiscountTypeForProductPrice>(new Dictionary<PSDiscountTypeForProductPrice, string>
            {
                {PSDiscountTypeForProductPrice.ProductPriceDiscount,"Z"},
                {PSDiscountTypeForProductPrice.ProductPriceFinal,"F"}
            });

            EnumCodeMapper.AddMap<SOStatus>(new Dictionary<SOStatus, int>
            {
                {SOStatus.Split , -6},
                {SOStatus.Abandon , -1},
                {SOStatus.Origin , 0},
                {SOStatus.WaitingOutStock , 1},
                {SOStatus.Shipping , 10},
                {SOStatus.WaitingManagerAudit , 3},
                {SOStatus.OutStock , 4},
                {SOStatus.Reported , 41},
                {SOStatus.CustomsPass , 45},
                {SOStatus.Complete , 5},
                {SOStatus.Reject , 6},
                {SOStatus.CustomsReject , 65},
                {SOStatus.ShippingReject , 7}
            });

            EnumCodeMapper.AddMap<CommentProcessStatus>(new Dictionary<CommentProcessStatus, int>
            {
                {CommentProcessStatus.Finish,2},
                {CommentProcessStatus.Handling,1},
                {CommentProcessStatus.Invalid,-1},
                {CommentProcessStatus.WaitHandling,0}
            });
            EnumCodeMapper.AddMap<KeywordsStatus>(new Dictionary<KeywordsStatus, string>
            {
                {KeywordsStatus.All,"R"},
                {KeywordsStatus.Passed,"A"},
                {KeywordsStatus.Reject,"D"},
                {KeywordsStatus.Waiting,"O"}
            });
            EnumCodeMapper.AddMap<ReplyVendor>(new Dictionary<ReplyVendor, string>
            {
                {ReplyVendor.YES,"M"},
                {ReplyVendor.NO,"N"}
            });
            EnumCodeMapper.AddMap<PollType>(new Dictionary<PollType, string>
            {
                {PollType.Single,"S"},
                {PollType.Multiple,"M"},
                {PollType.ShortAnswer,"A"},
                {PollType.Other,"C"}
            });

            EnumCodeMapper.AddMap<SaleGiftStatus>(new Dictionary<SaleGiftStatus, string>
            {
                { SaleGiftStatus.Finish,"F"},
                { SaleGiftStatus.Init,"O"},
                { SaleGiftStatus.Ready,"R"},
                { SaleGiftStatus.Run,"A"},
                { SaleGiftStatus.Void,"D"},
                { SaleGiftStatus.WaitingAudit,"P"},
                { SaleGiftStatus.Stoped,"S"}
            });
            EnumCodeMapper.AddExtraCodeMap<SaleGiftStatus>(SaleGiftStatus.WaitingAudit, new object[] { "P", "W" });
            EnumCodeMapper.AddMap<SaleGiftType>(new Dictionary<SaleGiftType, string>
            {
                { SaleGiftType.Full,"F"},
                {SaleGiftType.Multiple,"M"},
                {SaleGiftType.Single,"S"},
                {SaleGiftType.Vendor,"V"},
                //{SaleGiftType.FirstOrder,"O"},
                //{SaleGiftType.Additional,"B"}
            });

            EnumCodeMapper.AddMap<SaleGiftDiscountBelongType>(new Dictionary<SaleGiftDiscountBelongType, string>
            {
                { SaleGiftDiscountBelongType.BelongGiftItem,"G"},
                {SaleGiftDiscountBelongType.BelongMasterItem,"M"}
            });

            EnumCodeMapper.AddMap<SaleGiftSaleRuleType>(new Dictionary<SaleGiftSaleRuleType, string>
            {                
                {SaleGiftSaleRuleType.Brand,"B"},
                {SaleGiftSaleRuleType.BrandC3Combo,"C"},
                {SaleGiftSaleRuleType.C3,"L"},
                {SaleGiftSaleRuleType.Item,"I"}
            });


            EnumCodeMapper.AddMap<SaleGiftGiftItemType>(new Dictionary<SaleGiftGiftItemType, string>
            {
                {SaleGiftGiftItemType.GiftPool,"O"},
                {SaleGiftGiftItemType.AssignGift, "A"}
            });

            ///捆绑方式，是组合捆绑M还是交叉捆绑S
            EnumCodeMapper.AddMap<SaleGiftCombineType>(new Dictionary<SaleGiftCombineType, string>
            {
                {SaleGiftCombineType.Assemble,"M"},
                {SaleGiftCombineType.Cross, "S"}
            });

            EnumCodeMapper.AddMap<AndOrType>(new Dictionary<AndOrType, string>
            {
                {AndOrType.Or,"O"},
                {AndOrType.And, "A"},
                {AndOrType.Not, "N"}
            });


            EnumCodeMapper.AddMap<ThesaurusWordsType>(new Dictionary<ThesaurusWordsType, string>
            {
                {ThesaurusWordsType.Doubleaction,"T"},
                {ThesaurusWordsType.Monodirectional,"O"}
            });
            EnumCodeMapper.AddMap<KeywordsOperateUserType>(new Dictionary<KeywordsOperateUserType, int>
            {
                {KeywordsOperateUserType.Customer,1},
                {KeywordsOperateUserType.MKTUser,0}
            });

            EnumCodeMapper.AddMap<ComboStatus>(new Dictionary<ComboStatus, int>
            {
                {ComboStatus.Active,0},
                {ComboStatus.Deactive,-1},
                {ComboStatus.WaitingAudit,1}
            });

            EnumCodeMapper.AddMap<ComboType>(new Dictionary<ComboType, int>
            {
                {ComboType.Common,0}
                //{ComboType.NYuanSend,1}
            });

            EnumCodeMapper.AddMap<GroupType>(new Dictionary<GroupType, int>
            {
                {GroupType.Custom,2},
                {GroupType.LevelOne,3},
                {GroupType.LevelTwo,0},
                {GroupType.LevelThree,1}
            });
            EnumCodeMapper.AddMap<AreaRelationType>(new Dictionary<AreaRelationType, string>
            {
                {AreaRelationType.Banner,"B"},
                {AreaRelationType.News,"N"}
            });

            EnumCodeMapper.AddMap<ShowType>(new Dictionary<ShowType, string>
            {
                {ShowType.Table,"T"},
                {ShowType.ImageText,"I"}                
            });

            EnumCodeMapper.AddMap<RecommendType>(new Dictionary<RecommendType, int>
            {
                {RecommendType.Normal,0},
                //{RecommendType.Top,1},
                //{RecommendType.ShowRecommendIcon,2}      
                //{RecommendType.FiveItemPerRow,3},
                //{RecommendType.EightItemLeft,4},
                //{RecommendType.EightItemRight,5},
                //{RecommendType.EightItemUpperLeft,6},
                {RecommendType.SixItemLeft,7}
            });

            //前台显示分类级别
            EnumCodeMapper.AddMap<ECCategoryLevel>(new Dictionary<ECCategoryLevel, string>
            {
                {ECCategoryLevel.Category1,"H"},
                {ECCategoryLevel.Category2,"M"},
                {ECCategoryLevel.Category3,"L"}
            });

            //产品价格举报状态
            EnumCodeMapper.AddMap<ProductPriceCompareStatus>(new Dictionary<ProductPriceCompareStatus, int>
            {
                {ProductPriceCompareStatus.WaitAudit,0},
                {ProductPriceCompareStatus.AuditPass,1},
                {ProductPriceCompareStatus.AuditDecline,2}                
            });

            //产品价格举报DisplayLinkStatus
            EnumCodeMapper.AddMap<DisplayLinkStatus>(new Dictionary<DisplayLinkStatus, int>
            {
                {DisplayLinkStatus.Hide,0},
                {DisplayLinkStatus.Display,1}              
            });
            EnumCodeMapper.AddMap<SaleAdvStatus>(new Dictionary<SaleAdvStatus, int>
            {
                {SaleAdvStatus.Active,0},
                {SaleAdvStatus.Deactive,-1}              
            });

            EnumCodeMapper.AddMap<GroupBuyingSettlementStatus>(new Dictionary<GroupBuyingSettlementStatus, string>
            {
                {GroupBuyingSettlementStatus.MoreThan,"C"},
                {GroupBuyingSettlementStatus.Yes,"Y"},
                {GroupBuyingSettlementStatus.No,"N"}
            });

            EnumCodeMapper.AddMap<GiftIsOnlineShow>(new Dictionary<GiftIsOnlineShow, string>
            {
                {GiftIsOnlineShow.Online,"Y"},
                {GiftIsOnlineShow.Deline,"N"}              
            });
            EnumCodeMapper.AddMap<CouponLimitType>(new Dictionary<CouponLimitType, string>
            {
                {CouponLimitType.PanicBuying ,"C"},
                {CouponLimitType.Kill,"K"},
                //{CouponLimitType.FriedEgg,"T"},
                {CouponLimitType.GroupBuying,"G"},
                {CouponLimitType.GiftCard,"L"},
                {CouponLimitType.Manually,"M"}
    
            });
            EnumCodeMapper.AddMap<ClearType>(new Dictionary<ClearType, string>
            {
                {ClearType.WaitClear,"W"},
                {ClearType.CompleteClear,"F"}              
            });
            EnumCodeMapper.AddMap<DynamicCategoryType>(new Dictionary<DynamicCategoryType, Int32>
            {
                {DynamicCategoryType.Standard,0},
                {DynamicCategoryType.WangJie,1}              
            });
            //EnumCodeMapper.AddMap<DynamicCategoryStatus>(new Dictionary<DynamicCategoryStatus, Int32>
            //{
            //    {DynamicCategoryStatus.Active,0},
            //    {DynamicCategoryStatus.Deactive,-1}              
            //});

            //限购规则类型
            EnumCodeMapper.AddMap<LimitType>(new Dictionary<LimitType, Int32>
            {
                {LimitType.SingleProduct,0},
                {LimitType.Combo,1}              
            });

            //限购规则状态
            EnumCodeMapper.AddMap<LimitStatus>(new Dictionary<LimitStatus, Int32>
            {
                {LimitStatus.Invalid,0},
                {LimitStatus.Valid,1}              
            });

            //销售立减规则类型
            EnumCodeMapper.AddMap<SaleDiscountRuleType>(new Dictionary<SaleDiscountRuleType, Int32>
            {
                {SaleDiscountRuleType.AmountRule,0},
                {SaleDiscountRuleType.QtyRule,1}              
            });

            //销售立减规则状态
            EnumCodeMapper.AddMap<SaleDiscountRuleStatus>(new Dictionary<SaleDiscountRuleStatus, Int32>
            {
                {SaleDiscountRuleStatus.Invalid,0},
                {SaleDiscountRuleStatus.Valid,1}              
            });
            //销售立减规则状态
            EnumCodeMapper.AddMap<GroupBuyingCategoryType>(new Dictionary<GroupBuyingCategoryType, Int32>
            {
                {GroupBuyingCategoryType.Physical,0},
                {GroupBuyingCategoryType.Virtual,1},
                {GroupBuyingCategoryType.ZeroLottery,2}              
            });
            EnumCodeMapper.AddMap<GroupBuyingFeedbackStatus>(new Dictionary<GroupBuyingFeedbackStatus, Int32>
            {
                {GroupBuyingFeedbackStatus.UnRead,0},
                {GroupBuyingFeedbackStatus.Readed,1}                
            });
            EnumCodeMapper.AddMap<BusinessCooperationStatus>(new Dictionary<BusinessCooperationStatus, Int32>
            {
                {BusinessCooperationStatus.UnHandled,0},
                {BusinessCooperationStatus.Handled,1}                
            });
            EnumCodeMapper.AddMap<SettlementBillStatus>(new Dictionary<SettlementBillStatus, Int32>
            {
                {SettlementBillStatus.UnSettle,0},
                {SettlementBillStatus.Settled,1}                
            });
            EnumCodeMapper.AddMap<GroupBuyingCategoryStatus>(new Dictionary<GroupBuyingCategoryStatus, Int32>
            {
                {GroupBuyingCategoryStatus.Valid,0},
                {GroupBuyingCategoryStatus.InValid,-1}                
            });
        }
    }
}
