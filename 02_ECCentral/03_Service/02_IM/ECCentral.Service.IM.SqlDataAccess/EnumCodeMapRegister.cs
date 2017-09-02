using System.Collections.Generic;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.IM;
using ECCentral.Service.Utility;
using ValidStatus = ECCentral.BizEntity.IM.ValidStatus;

namespace ECCentral.Service.IM.SqlDataAccess
{
    public class EnumCodeMapRegister : IStartup
    {
        public void Start()
        {
            EnumCodeMapper.AddMap(new Dictionary<ValidStatus, string>
                                                   {
                                                       {ValidStatus.Active,"A"},
                                                       {ValidStatus.DeActive,"D"}
                                                   });


            EnumCodeMapper.AddMap(new Dictionary<ProductType, string>
                                                   {
                                                       {ProductType.Normal,"O"},
                                                       {ProductType.OpenBox,"S"},
                                                       {ProductType.Bad, "B"},
                                                       {ProductType.Virtual, "V"}
                                                   });

            EnumCodeMapper.AddMap(new Dictionary<Large, string>
                                                   {
                                                       {Large.Yes,"Y"},
                                                       {Large.No,"N"}
                                                   });

            EnumCodeMapper.AddMap(new Dictionary<ProductGroupImageShow, string>
                                                   {
                                                       {ProductGroupImageShow.Yes,"Y"},
                                                       {ProductGroupImageShow.No,"N"}
                                                   });

            EnumCodeMapper.AddMap(new Dictionary<ProductGroupPolymeric, string>
                                                   {
                                                       {ProductGroupPolymeric.Yes,"Y"},
                                                       {ProductGroupPolymeric.No,"N"}
                                                   });

            EnumCodeMapper.AddExtraCodeMap(ProductGroupPolymeric.No, new object[] { "n" });

            EnumCodeMapper.AddMap(new Dictionary<PropertyStatus, int>
                                                   {
                                                       {PropertyStatus.Active,1},
                                                       {PropertyStatus.DeActive,0}
                                                   });

            EnumCodeMapper.AddMap(new Dictionary<ProductAccessoryStatus, int>
                                                   {
                                                       {ProductAccessoryStatus.Active,0},
                                                       {ProductAccessoryStatus.DeActive,-1}
                                                   });
            EnumCodeMapper.AddMap(new Dictionary<PropertyType, string>
                                                   {
                                                       {PropertyType.Grouping,"G"},
                                                       {PropertyType.Other,"A"}
                                                   });

            EnumCodeMapper.AddMap(new Dictionary<ProductPropertyRequired, int>
                                                   {
                                                       {ProductPropertyRequired.Yes,1},
                                                       {ProductPropertyRequired.No,0}
                                                   });

            EnumCodeMapper.AddExtraCodeMap(PropertyType.Other, new object[] { "O" });
            EnumCodeMapper.AddMap(new Dictionary<ProductResourceStatus, string>
                                                   {
                                                       {ProductResourceStatus.Active,"A"},
                                                       {ProductResourceStatus.DeActive,"D"}
                                                   });

            EnumCodeMapper.AddMap(new Dictionary<ProductResourceIsShow, string>
                                                   {
                                                       {ProductResourceIsShow.Yes,"Y"},
                                                       {ProductResourceIsShow.No,"N"}
                                                   });

            EnumCodeMapper.AddMap(new Dictionary<ResourcesType, string>
                                                   {
                                                       {ResourcesType.Image,"I"},
                                                       {ResourcesType.Image360,"D"},
                                                       {ResourcesType.Video,"V"}
                                                   });

            EnumCodeMapper.AddMap(new Dictionary<IsLarge, string>
                                                   {
                                                       {IsLarge.Yes,"L"},
                                                       {IsLarge.No,"S"},
                                                       {IsLarge.Undefined,"-"}
                                                   });

            EnumCodeMapper.AddMap(new Dictionary<OfferVATInvoice, int>
                                                   {
                                                       {OfferVATInvoice.Yes,1},
                                                       {OfferVATInvoice.No,0}
                                                   });


            EnumCodeMapper.AddMap(new Dictionary<WarrantyShow, int>
                                                   {
                                                       {WarrantyShow.Yes,1},
                                                       {WarrantyShow.No,0}
                                                   });
            EnumCodeMapper.AddMap(new Dictionary<CustomerRank, int>
                                                   {
                                                       {CustomerRank.Ferrum,1},
                                                       {CustomerRank.Copper,2},
                                                       {CustomerRank.Silver,3},
                                                       {CustomerRank.Golden,4},
                                                       {CustomerRank.Diamond,5},
                                                       {CustomerRank.Crown,6},
                                                       {CustomerRank.Supremacy,7}
                                                   });

            EnumCodeMapper.AddMap(new Dictionary<ProductRankPriceStatus, int>
                                                   {
                                                       {ProductRankPriceStatus.Active,0},
                                                       {ProductRankPriceStatus.DeActive,-1}
                                                   });

            EnumCodeMapper.AddMap(new Dictionary<WholeSaleLevelType, int>
                                                   {
                                                       {WholeSaleLevelType.L1,1},
                                                       {WholeSaleLevelType.L2,2},
                                                       {WholeSaleLevelType.L3,3}
                                                   });
            EnumCodeMapper.AddMap(new Dictionary<GiftCardStatus, string>
                                                   {
                                                       {GiftCardStatus.ManualActive,"H"},
                                                       {GiftCardStatus.InValid,"D"},
                                                       {GiftCardStatus.Void,"V"},
                                                       {GiftCardStatus.Used,"U"},
                                                       {GiftCardStatus.Valid,"A"}
                                                   });
            EnumCodeMapper.AddMap(new Dictionary<CategoryExtendWarrantyStatus, string>
                                                   {
                                                       {CategoryExtendWarrantyStatus.Active,"A"},
                                                       {CategoryExtendWarrantyStatus.DeActive,"D"}                                                       
                                                   });

            EnumCodeMapper.AddMap(new Dictionary<CategoryExtendWarrantyDisuseBrandStatus, string>
                                                   {
                                                       {CategoryExtendWarrantyDisuseBrandStatus.Active,"A"},
                                                       {CategoryExtendWarrantyDisuseBrandStatus.DeActive,"D"}                                                       
                                                   });

            EnumCodeMapper.AddMap(new Dictionary<ActionType, string>
                                                   {
                                                       {ActionType.Hold,"Hold"},
                                                       {ActionType.AdjustExpireDate,"AdjustExpireDate"},
                                                       {ActionType.ForwardCard,"ForwardCard"},
                                                       {ActionType.MandatoryVoid,"MandatoryVoid"},
                                                       {ActionType.UnHold,"UnHold"},
                                                       {ActionType.SO,"SO"},
                                                       {ActionType.RMA,"RMA"},
                                                       {ActionType.ROBalance,"ROBalance"}
                                                   });

            EnumCodeMapper.AddMap(new Dictionary<SellerProductRequestStatus, string>
                                                   {
                                                       {SellerProductRequestStatus.Approved,"A"},
                                                       {SellerProductRequestStatus.Processing,"P"},
                                                       {SellerProductRequestStatus.UnApproved,"D"},
                                                       {SellerProductRequestStatus.WaitApproval,"O"},
                                                       {SellerProductRequestStatus.Finish,"F"}                                                       
                                                   });

            EnumCodeMapper.AddMap(new Dictionary<SellerProductRequestType, string>
                                                   {                                                     
                                                       {SellerProductRequestType.NewCreated,"N"},
                                                       {SellerProductRequestType.ParameterUpdate,"P"},
                                                       {SellerProductRequestType.ImageAndDescriptionUpdate,"I"}
                                                   });

            EnumCodeMapper.AddMap(new Dictionary<SellerProductRequestTakePictures, string>
                                                   {
                                                       {SellerProductRequestTakePictures.Yes,"Y"},
                                                       {SellerProductRequestTakePictures.No,"N"}                                                       
                                                   });

            EnumCodeMapper.AddMap(new Dictionary<SellerProductRequestFileType, string>
                                                   {
                                                       {SellerProductRequestFileType.Image,"I"},
                                                       {SellerProductRequestFileType.Text,"P"}                                                       
                                                   });

            EnumCodeMapper.AddMap(new Dictionary<SellerProductRequestFileStatus, string>
                                                   {
                                                       {SellerProductRequestFileStatus.WaitProcess,"O"},
                                                       {SellerProductRequestFileStatus.Finish,"F"}                                                       
                                                   });

            EnumCodeMapper.AddMap(new Dictionary<ProductPriceRequestStatus, int>
                                                   {
                                                       {ProductPriceRequestStatus.Origin,0},
                                                       {ProductPriceRequestStatus.Approved,1},
                                                       {ProductPriceRequestStatus.Deny,-1},
                                                       {ProductPriceRequestStatus.Canceled,-2},
                                                       {ProductPriceRequestStatus.NeedSeniorApprove,9},
                                                   });

            EnumCodeMapper.AddMap(new Dictionary<ProductPriceRequestAuditType, string>
                                                   {
                                                       {ProductPriceRequestAuditType.Audit,"T"},
                                                       {ProductPriceRequestAuditType.SeniorAudit,"P"}
                                                   });

            EnumCodeMapper.AddMap(new Dictionary<ProductIsTakePicture, string>
                                                   {
                                                       {ProductIsTakePicture.Yes,"Y"},
                                                       {ProductIsTakePicture.No,"N"}
                                                   });

            EnumCodeMapper.AddMap(new Dictionary<SellerProductRequestOfferInvoice, string>
                                                   {
                                                       {SellerProductRequestOfferInvoice.Yes,"Y"},
                                                       {SellerProductRequestOfferInvoice.No,"N"}
                                                   });
            EnumCodeMapper.AddMap(new Dictionary<ProductStatus, int>
                                      {
                                          { ProductStatus.Abandon, -1 }, 
                                          { ProductStatus.Active, 1 }, 
                                          { ProductStatus.InActive_Show, 0 }, 
                                          { ProductStatus.InActive_UnShow, 2 }
                                      });
            EnumCodeMapper.AddMap(new Dictionary<IsDefault, int>
                                                   {
                                                       {IsDefault.Active,0},
                                                       {IsDefault.DeActive,1}
                                                   });

            EnumCodeMapper.AddMap(new Dictionary<TimelyPromotionTitleStatus, string>
                                                   {
                                                       {TimelyPromotionTitleStatus.Original,"O"},
                                                       {TimelyPromotionTitleStatus.Active,"A"},
                                                       {TimelyPromotionTitleStatus.DeActive,"D"}
                                                   });

            EnumCodeMapper.AddMap(new Dictionary<ProductIsVirtualPic, string>
                                                   {
                                                       {ProductIsVirtualPic.Yes,"Y"},
                                                       {ProductIsVirtualPic.No,"N"}
                                                   });

            EnumCodeMapper.AddMap(new Dictionary<ProductIsAccessoryShow, string>
                                                   {
                                                       {ProductIsAccessoryShow.Yes,"Y"},
                                                       {ProductIsAccessoryShow.No,"N"}
                                                   });

            EnumCodeMapper.AddMap(new Dictionary<NotifyStatus, int>
                                                   {
                                                       {NotifyStatus.NotifyYes,1},
                                                       {NotifyStatus.NotifyBad,-1},
                                                       {NotifyStatus.NotifyNo,0}
                                                   });

            EnumCodeMapper.AddMap(new Dictionary<ProductInfoFinishStatus, string>
                                                   {
                                                       {ProductInfoFinishStatus.Yes,"Y"},
                                                       {ProductInfoFinishStatus.No,"N"}
                                                   });

            EnumCodeMapper.AddMap(new Dictionary<ThirdPartner, string>
                                                   {
                                                       {ThirdPartner.Hengzhong,"H"},
                                                       {ThirdPartner.Xijie,"X"},
                                                       {ThirdPartner.Belle,"B"},
                                                       {ThirdPartner.Yieke,"Y"},
                                                       {ThirdPartner.Ingrammicro,"I"}
                                                   });

            EnumCodeMapper.AddExtraCodeMap(new Dictionary<StockRules, object[]>
                                                   {
                                                       {StockRules.Limit,new object[]{"L"}},
                                                       {StockRules.Direct,new object[]{"U"}},
                                                       {StockRules.Customer,new object[]{"C","T","D"}},
                                                   });

            EnumCodeMapper.AddMap(new Dictionary<ProductMappingStatus, string>
                                                   {
                                                       {ProductMappingStatus.Active,"A"},
                                                       {ProductMappingStatus.DeActive,"D"}
                                                   });

            EnumCodeMapper.AddMap(new Dictionary<IsUseAlipayVipPrice, string>
                                                   {
                                                       {IsUseAlipayVipPrice.Yes,"Y"},
                                                       {IsUseAlipayVipPrice.No,"N"}
                                                   });

            EnumCodeMapper.AddMap(new Dictionary<IsAutoAdjustPrice, int>
                                                   {
                                                       {IsAutoAdjustPrice.Yes,1},
                                                       {IsAutoAdjustPrice.No,0}
                                                   });
            EnumCodeMapper.AddMap(new Dictionary<InventorySync, string>
                                                   {
                                                       {InventorySync.Yes,"Y"},
                                                       {InventorySync.No,"N"}
                                                   });
            EnumCodeMapper.AddMap(new Dictionary<BooleanEnum, string>
                                                   {
                                                       {BooleanEnum.Yes,"Y"},
                                                       {BooleanEnum.No,"N"}
                                                   });

            EnumCodeMapper.AddMap(new Dictionary<ProductChannelInfoStatus, string>
                                                   {
                                                       {ProductChannelInfoStatus.Active,"A"},
                                                       {ProductChannelInfoStatus.DeActive,"D"}
                                                   });

            EnumCodeMapper.AddMap(new Dictionary<ProductChannelPeriodPriceStatus, string>
                                                   {
                                                       {ProductChannelPeriodPriceStatus.Finish,"F"},
                                                       {ProductChannelPeriodPriceStatus.Init,"O"},
                                                       {ProductChannelPeriodPriceStatus.Abandon,"D"},
                                                       {ProductChannelPeriodPriceStatus.Ready,"R"},
                                                       {ProductChannelPeriodPriceStatus.Running,"A"},
                                                       {ProductChannelPeriodPriceStatus.WaitApproved,"P"}
                                                   });
            EnumCodeMapper.AddMap(new Dictionary<AuthorizedStatus, string>
                                                   {
                                                       {AuthorizedStatus.Active,"A"},
                                                       {AuthorizedStatus.DeActive,"D"}
                                                   });

            EnumCodeMapper.AddMap(new Dictionary<UnicomContractPhoneNumberStatus, string>
                                                   {
                                                       {UnicomContractPhoneNumberStatus.Active,"A"},
                                                       {UnicomContractPhoneNumberStatus.DeActive,"D"},
                                                       {UnicomContractPhoneNumberStatus.CreateOrder,"S"}
                                                   });
            EnumCodeMapper.AddMap(new Dictionary<CategoryTemplateType, int>
                                                   {
                                                       {CategoryTemplateType.TemplateProductTitle,0},
                                                       {CategoryTemplateType.TemplateProductDescription,1},
                                                       {CategoryTemplateType.TemplateProductName,2},
                                                       {CategoryTemplateType.TemplateWeb,3}
                                 });
            EnumCodeMapper.AddMap(new Dictionary<RmaPolicyType, string>
                                                   {
                                                       {RmaPolicyType.DiscountType,"D"},
                                                       {RmaPolicyType.ExtendType,"E"},
                                                       {RmaPolicyType.ManufacturerType,"M"},
                                                       {RmaPolicyType.StandardType,"P"},
                                                        {RmaPolicyType.SellerType,"S"}
                                 });
      EnumCodeMapper.AddMap(new Dictionary<RmaPolicyStatus, string>
                                                   {
                                                       {RmaPolicyStatus.Active,"A"},
                                                       {RmaPolicyStatus.DeActive,"D"},
                                                     
                                 });
      EnumCodeMapper.AddMap(new Dictionary<IsOnlineRequst, string>
                                                   {
                                                       {IsOnlineRequst.YES,"Y"},
                                                       {IsOnlineRequst.NO,"N"},
                                                     
                                 });
      EnumCodeMapper.AddMap(new Dictionary<RmaLogActionType, string>
                                                   {
                                                       {RmaLogActionType.Create,"C"},
                                                       {RmaLogActionType.Active,"A"},
                                                       {RmaLogActionType.DeActive,"D"},
                                                            {RmaLogActionType.Edit,"E"},

                                                     
                                 });

      EnumCodeMapper.AddMap(new Dictionary<CollectDateType, string>
                                                   {
                                                       {CollectDateType.ExpiredDate,"E"},
                                                       {CollectDateType.ManufactureDate,"P"}
                                                     
                                 });

      EnumCodeMapper.AddMap(new Dictionary<GiftVoucherRelateProductStatus, string> 
      {
          {GiftVoucherRelateProductStatus.Active,"A"},
          {GiftVoucherRelateProductStatus.Deactive,"D"}
      });

      EnumCodeMapper.AddMap(new Dictionary<GiftVoucherProductStatus, int> 
      {
          {GiftVoucherProductStatus.AuditFail,-2},
          {GiftVoucherProductStatus.Void,-1},
          {GiftVoucherProductStatus.WaittingAudit,1},
          {GiftVoucherProductStatus.Audit,2}
      });

      EnumCodeMapper.AddMap(new Dictionary<GVRReqType, int> 
      {
          {GVRReqType.Add,0},
          {GVRReqType.Delete,1}
      });

      EnumCodeMapper.AddMap(new Dictionary<GVRReqAuditStatus, string> 
      {
          {GVRReqAuditStatus.AuditWaitting,"W"},
          {GVRReqAuditStatus.AuditFailed,"F"},
          {GVRReqAuditStatus.AuditSuccess,"S"}
      });
     
        }
    }
}
