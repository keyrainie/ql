using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Enum.Resources;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.SO;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.SO.SqlDataAccess
{
    public class EnumCodeMapRegister : IStartup
    {
        public void Start()
        {
            EnumCodeMapper.AddMap<ECCentral.BizEntity.SO.SODeliveryPromise>(new Dictionary<ECCentral.BizEntity.SO.SODeliveryPromise, string>{
                { ECCentral.BizEntity.SO.SODeliveryPromise.In24H, "24H" },
                { ECCentral.BizEntity.SO.SODeliveryPromise.NoPromise, "" }
            });
            EnumCodeMapper.AddMap<ECCentral.BizEntity.SO.SOPriceStatus>(new Dictionary<ECCentral.BizEntity.SO.SOPriceStatus, string>{
                { ECCentral.BizEntity.SO.SOPriceStatus.Original, "O" },
                { ECCentral.BizEntity.SO.SOPriceStatus.Deactivate, "D" }
            });
            EnumCodeMapper.AddMap<ECCentral.BizEntity.SO.SettlementStatus>(new Dictionary<ECCentral.BizEntity.SO.SettlementStatus, string>{
                { ECCentral.BizEntity.SO.SettlementStatus.PlanFail, "P" },
                { ECCentral.BizEntity.SO.SettlementStatus.Fail, "F" },
                { ECCentral.BizEntity.SO.SettlementStatus.Success , "S" }
            });

            EnumCodeMapper.AddExtraCodeMap<SOStatus>(new Dictionary<SOStatus, object[]>{
                {SOStatus.Abandon, new object[] { -1, -2, -3, -4 }},
                {SOStatus.Origin, new object[] { 0, 2 }}
            });

            EnumCodeMapper.AddExtraCodeMap<SOComplainReplyType>(SOComplainReplyType.Phone, ResSOEnum.SOComplainReplyType_Phone);
            EnumCodeMapper.AddExtraCodeMap<SOComplainReplyType>(SOComplainReplyType.Email, ResSOEnum.SOComplainReplyType_Email);

            EnumCodeMapper.AddMap<ECCentral.BizEntity.SO.SOSplitType>(new Dictionary<ECCentral.BizEntity.SO.SOSplitType, string>{
                { ECCentral.BizEntity.SO.SOSplitType.Normal, "0" },
                { ECCentral.BizEntity.SO.SOSplitType.Force, "1" },
                { ECCentral.BizEntity.SO.SOSplitType.Customer , "2" },
                { ECCentral.BizEntity.SO.SOSplitType.SubSO , "3" }
            });

            EnumCodeMapper.AddMap<ECCentral.BizEntity.SO.SOProductType>(new Dictionary<ECCentral.BizEntity.SO.SOProductType, string>{
                { ECCentral.BizEntity.SO.SOProductType.Product, "0" },
                { ECCentral.BizEntity.SO.SOProductType.Gift, "1" },
                { ECCentral.BizEntity.SO.SOProductType.Award , "2" },
                { ECCentral.BizEntity.SO.SOProductType.Coupon , "3" },
                { ECCentral.BizEntity.SO.SOProductType.ExtendWarranty, "4" },
                { ECCentral.BizEntity.SO.SOProductType.Accessory , "5" },
                { ECCentral.BizEntity.SO.SOProductType.SelfGift , "6" }
            });
            EnumCodeMapper.AddMap<ECCentral.BizEntity.SO.SOProductPriceType>(new Dictionary<ECCentral.BizEntity.SO.SOProductPriceType, string>{
                { ECCentral.BizEntity.SO.SOProductPriceType.Normal, "0" },
                { ECCentral.BizEntity.SO.SOProductPriceType.Member, "1" },
                { ECCentral.BizEntity.SO.SOProductPriceType.WholeSale , "2" }
            });
            EnumCodeMapper.AddMap<ECCentral.BizEntity.IM.ProductPayType>(new Dictionary<ECCentral.BizEntity.IM.ProductPayType, string>{
                { ECCentral.BizEntity.IM.ProductPayType.MoneyOnly, "0" },
                { ECCentral.BizEntity.IM.ProductPayType.All, "1" },
                { ECCentral.BizEntity.IM.ProductPayType.PointOnly, "2" }
            });

            EnumCodeMapper.AddMap<ECCentral.BizEntity.SO.WMSAction>(new Dictionary<ECCentral.BizEntity.SO.WMSAction, string>{
                { ECCentral.BizEntity.SO.WMSAction.Abandon, "V" },
                { ECCentral.BizEntity.SO.WMSAction.AbandonHold, "D" },
                { ECCentral.BizEntity.SO.WMSAction.CancelAuditHold , "C" },
                { ECCentral.BizEntity.SO.WMSAction.Hold , "H" },
                { ECCentral.BizEntity.SO.WMSAction.UnHold , "U" }
            });

            EnumCodeMapper.AddMap<ECCentral.BizEntity.SO.OPCStatus>(new Dictionary<ECCentral.BizEntity.SO.OPCStatus, string>{
                { ECCentral.BizEntity.SO.OPCStatus.Close, "C" },
                { ECCentral.BizEntity.SO.OPCStatus.Error, "E" },
                { ECCentral.BizEntity.SO.OPCStatus.Fail , "F" },
                { ECCentral.BizEntity.SO.OPCStatus.Open , "O" }
            });
            EnumCodeMapper.AddMap<ECCentral.BizEntity.SO.OPCTransStatus>(new Dictionary<ECCentral.BizEntity.SO.OPCTransStatus, string>{
                { ECCentral.BizEntity.SO.OPCTransStatus.Origin, "O" },
                { ECCentral.BizEntity.SO.OPCTransStatus.Error, "E" },
                { ECCentral.BizEntity.SO.OPCTransStatus.Fail , "F" },
                { ECCentral.BizEntity.SO.OPCTransStatus.Success , "S" }
            });
            EnumCodeMapper.AddMap(new Dictionary<ECCentral.BizEntity.SO.ValidStatus, string>
                                                   {
                                                       {ECCentral.BizEntity.SO.ValidStatus.Active,"A"},
                                                       {ECCentral.BizEntity.SO.ValidStatus.DeActive,"D"}
                                                   });


            EnumCodeMapper.AddMap(new Dictionary<ECCentral.BizEntity.SO.SOProductActivityType, string>
                                                   {
                                                       {ECCentral.BizEntity.SO.SOProductActivityType.GroupBuy,"G"},
                                                       {ECCentral.BizEntity.SO.SOProductActivityType.SpecialAccount,"C"},
                                                       {ECCentral.BizEntity.SO.SOProductActivityType.E_Promotion,"E"}
                                                   });
        }
    }
}