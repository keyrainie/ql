using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Store.Vendor;
using ECommerce.Enums;
using ECommerce.Utility;

namespace ECommerce.DataAccess
{
    public class EnumCodeMapRegister : IStartup
    {
        public void Start()
        {
            EnumCodeMapper.AddMap<UserStatus>(new Dictionary<UserStatus, string>{
                { UserStatus.Active, "A" },
                { UserStatus.DeActive, "D" }
            });

            EnumCodeMapper.AddMap<RoleStatus>(new Dictionary<RoleStatus, string>{
                { RoleStatus.Active, "A" },
                { RoleStatus.DeActive, "D" }
            });


            EnumCodeMapper.AddMap<SettleOrderStatus>(new Dictionary<SettleOrderStatus, string>{
                { SettleOrderStatus.CLS, "CLS" },
                { SettleOrderStatus.ORG, "ORG" },
                { SettleOrderStatus.SET, "SET" }
            });
            EnumCodeMapper.AddMap<CommissionType>(new Dictionary<CommissionType, string>{
                { CommissionType.DEF, "DEF" },
                { CommissionType.SAC, "SAC" },
                { CommissionType.SOC, "SOC" }
            });

            EnumCodeMapper.AddMap<ProductRangeType>(new Dictionary<ProductRangeType, string>{
                { ProductRangeType.All, "A" },
                { ProductRangeType.Limit, "I" }
            });

            EnumCodeMapper.AddMap<RelationType>(new Dictionary<RelationType, string>{
                { RelationType.Y, "Y" },
                { RelationType.N, "N" }
            });

            EnumCodeMapper.AddMap<CouponStatus>(new Dictionary<CouponStatus, string>{
                { CouponStatus.Init, "O" },
                { CouponStatus.WaitingAudit, "W" },
                { CouponStatus.Ready, "R" },
                { CouponStatus.Run, "A" },
                { CouponStatus.Void, "D" },
                { CouponStatus.Stoped, "S" },
                { CouponStatus.Finish, "F" }
            });

            EnumCodeMapper.AddMap<CouponDiscountRuleType>(new Dictionary<CouponDiscountRuleType, string>{
                { CouponDiscountRuleType.Discount, "D" },
                { CouponDiscountRuleType.Percentage, "P" }
            });

            EnumCodeMapper.AddMap<CouponsBindConditionType>(new Dictionary<CouponsBindConditionType, string>{
                { CouponsBindConditionType.None, "A" },
                { CouponsBindConditionType.SO, "O" },
                { CouponsBindConditionType.Get, "L" }
            });

            EnumCodeMapper.AddMap<CouponValidPeriodType>(new Dictionary<CouponValidPeriodType, int>{
                { CouponValidPeriodType.All, 0 },
                { CouponValidPeriodType.PublishDayToOneWeek, 1 },
                { CouponValidPeriodType.PublishDayToOneMonth, 2 },
                { CouponValidPeriodType.PublishDayToTwoMonths, 3 },
                { CouponValidPeriodType.PublishDayToThreeMonths, 4 },
                { CouponValidPeriodType.PublishDayToSixMonths, 5 },
                { CouponValidPeriodType.CustomPeriod, 6 }
            });

            EnumCodeMapper.AddMap<SaleGiftType>(new Dictionary<SaleGiftType, string>{
                { SaleGiftType.Single, "S" },
                { SaleGiftType.Multiple, "M" },
                { SaleGiftType.Full, "F" }
                //{ SaleGiftType.Vendor, "V" },
                //{ SaleGiftType.Additional, "F" }
            });
            EnumCodeMapper.AddMap<SaleGiftStatus>(new Dictionary<SaleGiftStatus, string>{
                { SaleGiftStatus.Origin, "O" },
                { SaleGiftStatus.WaitingAudit, "P" },
                { SaleGiftStatus.Ready, "R" },
                { SaleGiftStatus.Run, "A" },
                { SaleGiftStatus.Stoped, "S" },
                { SaleGiftStatus.Finish, "F" },
                { SaleGiftStatus.Void, "D" }
            });
            EnumCodeMapper.AddMap<SaleGiftDiscountBelongType>(new Dictionary<SaleGiftDiscountBelongType, string>
            {
                { SaleGiftDiscountBelongType.BelongGiftItem,"G"},
                {SaleGiftDiscountBelongType.BelongMasterItem,"M"}
            });

            EnumCodeMapper.AddMap<Specified>(new Dictionary<Specified, string>
            {
                { Specified.Yes, "Y"},
                { Specified.No, "N"}
            });

            EnumCodeMapper.AddMap<CouponCodeType>(new Dictionary<CouponCodeType, string>{
                { CouponCodeType.Common, "C" },
                { CouponCodeType.ThrowIn, "T" }
            });
            EnumCodeMapper.AddMap<AndOrType>(new Dictionary<AndOrType, string>
            {
                {AndOrType.And,"A"},
                {AndOrType.Not, "N"}
            });

            EnumCodeMapper.AddMap<CouponCodeUsedStatus>(new Dictionary<CouponCodeUsedStatus, string>{
                { CouponCodeUsedStatus.Active, "A" },
                { CouponCodeUsedStatus.Deactive, "D" }
            });

            EnumCodeMapper.AddMap<CustomerType>(new Dictionary<CustomerType, int>{
                { CustomerType.Personal, 0 },
                { CustomerType.Enterprise, 1 },
                { CustomerType.Campus, 2 },
                { CustomerType.Media,3 },
                { CustomerType.Internal,4 }
            });

            EnumCodeMapper.AddExtraCodeMap<CustomerStatus>(CustomerStatus.InValid, -1);

            EnumCodeMapper.AddMap<SOPriceStatus>(new Dictionary<SOPriceStatus, string>{
                { SOPriceStatus.Original, "O" },
                { SOPriceStatus.Deactivate, "D" }
            });

            EnumCodeMapper.AddMap<ShippingStockType>(new Dictionary<ShippingStockType, string>{
                { ShippingStockType.MET, "MET" },
                { ShippingStockType.NEG, "NEG" }
            });

            EnumCodeMapper.AddMap<FreeShippingAmountSettingStatus>(new Dictionary<FreeShippingAmountSettingStatus, string>{
                { FreeShippingAmountSettingStatus.Active, "A" },
                { FreeShippingAmountSettingStatus.DeActive, "D" }
            });
        }
    }
}
